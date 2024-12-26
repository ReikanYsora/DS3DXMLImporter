using DS3DXMLImporter.Exceptions;
using DS3DXMLImporter.Loaders;
using DS3DXMLImporter.Models;
using DS3DXMLImporter.Models.Attributes;
using DS3DXMLImporter.Models.Unity;
using DS3XMLImporter.Models;
using DS3XMLImporter.Models.Interfaces;
using DS3XMLImporter.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DS3DXMLImporter.Parsers
{
    internal class DS3DXMLParser
    {
        #region ATTRIBUTES
        private DS3DXMLHeader _header;
        private IList<ReferenceRep> _referencesRep;
        private IList<InstanceRep> _instancesRep;
        private IList<Reference3D> _references3D;
        private IList<Instance3D> _instances3D;
        private Dictionary<string, MeshDefinition> _meshDefinitions;
        #endregion

        #region EVENTS
        public delegate void OnParseCompletedEventHandler(DS3DXMLStructure structure);
        public event OnParseCompletedEventHandler OnParseCompleted;
        #endregion

        #region METHODS
        public void ParseStructure(ILoader loader)
        {
            ParseStructure(loader.Load());
        }

        public void ParseStructure(Stream stream)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    IDS3DXMLArchive fileArchive = DS3DXMLFile.Create(stream);
                    XDocument xmlManifest = ReadManifest(fileArchive);

                    _header = ParserHelper.GetHeader(xmlManifest);
                    _referencesRep = ParseReferenceRep(xmlManifest, fileArchive);
                    _instancesRep = ParseInstanceRep(xmlManifest);
                    _references3D = ParseReference3D(xmlManifest);
                    _instances3D = ParseInstance3D(xmlManifest);
                    _meshDefinitions = ConvertToMeshDefinitions(_referencesRep);

                    DS3DXMLStructure structure = new DS3DXMLStructure(
                        _header,
                        _referencesRep.ToDictionary(x => x.ID, y => y),
                        _instancesRep.ToDictionary(x => x.ID, y => y),
                        _references3D.ToDictionary(x => x.ID, y => y),
                        _instances3D.ToDictionary(x => x.ID, y => y),
                        _meshDefinitions
                    );

                    OnParseCompleted?.Invoke(structure);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            });
        }

        public T Get<T>(int id)
        {
            Type type = typeof(T);

            if (type == typeof(Reference3D))
            {
                return (T)Convert.ChangeType(_references3D.First(x => x.ID == id), type);
            }

            if (type == typeof(Instance3D))
            {
                return (T)Convert.ChangeType(_instances3D.First(x => x.ID == id), type);
            }

            if (type == typeof(ReferenceRep))
            {
                return (T)Convert.ChangeType(_referencesRep.First(x => x.ID == id), type);
            }

            if (type == typeof(InstanceRep))
            {
                return (T)Convert.ChangeType(_instancesRep.First(x => x.ID == id), type);
            }

            throw new TypeNotFoundException(type.Name);
        }

        private Dictionary<string, MeshDefinition> ConvertToMeshDefinitions(IEnumerable<ReferenceRep> referenceReps)
        {
            Dictionary<string, MeshDefinition> meshDefinitions = new Dictionary<string, MeshDefinition>();

            foreach (ReferenceRep referenceRep in referenceReps)
            {
                meshDefinitions.Add(referenceRep.AssociatedFile, MeshDefinition.FromReferenceRep(referenceRep));
            }

            return meshDefinitions;
        }

        private IList<Reference3D> ParseReference3D(XDocument document)
        {
            return ParserHelper.RootDescendants(document, "Reference3D").Select(x => Reference3DParser.FromXDocument(x)).ToList();
        }

        private IList<Instance3D> ParseInstance3D(XDocument document)
        {
            return ParserHelper.RootDescendants(document, "Instance3D").Select(x => Instance3DParser.FromXDocument(x)).ToList();
        }

        private IList<InstanceRep> ParseInstanceRep(XDocument document)
        {
            return ParserHelper.RootDescendants(document, "InstanceRep").Select(x => InstanceRepParser.FromXDocument(x)).ToList();
        }

        private IList<ReferenceRep> ParseReferenceRep(XDocument xml, IDS3DXMLArchive archive)
        {
            return ReferenceRepParser.ParseReferenceReps(xml, archive);
        }

        private XDocument ReadManifest(IDS3DXMLArchive fileArchive)
        {
            return ParserHelper.ReadManifest(fileArchive);
        }
        #endregion
    }
}
