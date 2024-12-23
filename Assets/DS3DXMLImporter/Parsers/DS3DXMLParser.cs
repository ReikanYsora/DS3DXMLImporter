using DS3DXMLImporter.Exceptions;
using DS3DXMLImporter.Loaders;
using DS3DXMLImporter.Models;
using DS3DXMLImporter.Models.Unity;
using DS3XMLImporter.Models;
using DS3XMLImporter.Models.Interfaces;
using DS3XMLImporter.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                IDS3DXMLArchive fileArchive = DS3DXMLFile.Create(stream);
                XDocument xmlManifest = ReadManifest(fileArchive);

                _header = ParserHelper.GetHeader(xmlManifest);
                _referencesRep = ParseReferenceRep(xmlManifest, fileArchive);
                _instancesRep = ParseInstanceRep(xmlManifest);
                _references3D = ParseReference3D(xmlManifest);
                _instances3D = ParseInstance3D(xmlManifest);

                Reference3D product = Get<Reference3D>(1);
                IEnumerable<TransformDefinition> transformParts = Traverse(product, null).ToList();

                DS3DXMLStructure structure = new DS3DXMLStructure(
                    _header,
                    _referencesRep.ToDictionary(x => x.ID, y => y),
                    _instancesRep.ToDictionary(x => x.ID, y => y),
                    _references3D.ToDictionary(x => x.ID, y => y),
                    _instances3D.ToDictionary(x => x.ID, y => y),
                    transformParts.ToList()
                );

                OnParseCompleted?.Invoke(structure);
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

        private IList<T> Aggregated<T>(int aggregatedBy)
        {
            Type type = typeof(T);

            if (type == typeof(Instance3D))
            {
                IEnumerable<Instance3D> tempInstance3D = _instances3D.Where(x => x.AggregatedBy == aggregatedBy);

                return tempInstance3D.Select(x => (T)Convert.ChangeType(x, type)).ToList();
            }
            else if (type == typeof(InstanceRep))
            {
                IEnumerable<InstanceRep> tempInstanceRep = _instancesRep.Where(x => x.AggregatedBy == aggregatedBy);

                return tempInstanceRep.Select(x => (T)Convert.ChangeType(x, type)).ToList();
            }

            throw new TypeNotFoundException(type.Name);
        }

        private IEnumerable<TransformDefinition> Traverse(Reference3D ref3D, Instance3D instance3D)
        {
            IList<Instance3D> instance3Ds = Aggregated<Instance3D>(ref3D.ID);
            IList<InstanceRep> instanceReps = Aggregated<InstanceRep>(ref3D.ID);

            IEnumerable<ReferenceRep> referenceReps = instanceReps.Select(x => Get<ReferenceRep>(x.InstanceOf));

            return referenceReps.Select(referenceRep => TransformDefinition.FromReferenceRep(referenceRep, instance3D)).Concat(instance3Ds.SelectMany(instance => Traverse(Get<Reference3D>(instance.InstanceOf), instance)));
        }

        private IList<Reference3D> ParseReference3D(XDocument document)
        {
            return ParserHelper.RootDescendants(document, "Reference3D").Select(Reference3DParser.FromXDocument).ToList();
        }

        private IList<Instance3D> ParseInstance3D(XDocument document)
        {
            return ParserHelper.RootDescendants(document, "Instance3D").Select(Instance3DParser.FromXDocument).ToList();
        }

        private IList<InstanceRep> ParseInstanceRep(XDocument document)
        {
            return ParserHelper.RootDescendants(document, "InstanceRep").Select(InstanceRepParser.FromXDocument).ToList();
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
