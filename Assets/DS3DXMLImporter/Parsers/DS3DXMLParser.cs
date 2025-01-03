using DS3DXMLImporter.Loaders;
using DS3DXMLImporter.Models;
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
    public class DS3DXMLParser
    {
        #region ATTRIBUTES
        private DS3DXMLHeader _header;
        private Dictionary<int, ReferenceRep> _referencesRep;
        private Dictionary<int, InstanceRep> _instancesRep;
        private Dictionary<int, Reference3D> _references3D;
        private Dictionary<int, Instance3D> _instances3D;
        private Dictionary<string, MeshDefinition> _meshDefinitions;
        #endregion

        #region EVENTS
        public delegate void OnParseProgressionChangedEventHandler(float progress);
        public event OnParseProgressionChangedEventHandler OnParseProgressionChanged;
        public delegate void OnParseCompletedEventHandler(DS3DXMLStructure structure);
        public event OnParseCompletedEventHandler OnParseCompleted;
        #endregion

        #region METHODS
        public void ParseStructure(ILoader loader, float scale = 1000f)
        {
            ParseStructure(loader.Load(), scale);
        }

        public void ParseStructure(Stream stream, float scale)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    OnParseProgressionChanged?.Invoke(0.0f);

                    IDS3DXMLArchive fileArchive = DS3DXMLFile.Create(stream);
                    XDocument xmlManifest = ParserHelper.ReadManifest(fileArchive);
                    OnParseProgressionChanged?.Invoke(0.1f);

                    // Parse "Header"
                    _header = ParserHelper.GetHeader(xmlManifest);
                    OnParseProgressionChanged?.Invoke(0.2f);

                    List<XElement> xmlReferenceReps = xmlManifest.Root.Descendants("{http://www.3ds.com/xsd/3DXML}ReferenceRep").ToList();
                    List<XElement> xmlInstanceReps = xmlManifest.Root.Descendants("{http://www.3ds.com/xsd/3DXML}InstanceRep").ToList();
                    List<XElement> xmlReference3Ds = xmlManifest.Root.Descendants("{http://www.3ds.com/xsd/3DXML}Reference3D").ToList();
                    List<XElement> xmlInstance3Ds = xmlManifest.Root.Descendants("{http://www.3ds.com/xsd/3DXML}Instance3D").ToList();

                    _referencesRep = new Dictionary<int, ReferenceRep>(xmlReferenceReps.Count);
                    _instancesRep = new Dictionary<int, InstanceRep>(xmlInstanceReps.Count);
                    _references3D = new Dictionary<int, Reference3D>(xmlReference3Ds.Count);
                    _instances3D = new Dictionary<int, Instance3D>(xmlInstance3Ds.Count);

                    // Parse "ReferenceRep" nodes
                    Parallel.ForEach(xmlReferenceReps, (referenceRep, _, index) =>
                    {
                        ReferenceRep tempReferenceRep = ReferenceRepParser.Parse(referenceRep, fileArchive, scale);

                        lock (_referencesRep)
                        {
                            _referencesRep.Add(tempReferenceRep.ID, tempReferenceRep);
                        }

                        OnParseProgressionChanged?.Invoke(0.2f + (float)(index / (double)xmlReferenceReps.Count * 0.4f));
                    });

                    // Parse "InstanceRep" nodes
                    Parallel.ForEach(xmlInstanceReps, (instanceRep, _, index) =>
                    {
                        InstanceRep tempInstanceRep = InstanceRepParser.Parse(instanceRep);

                        lock (_instancesRep)
                        {
                            _instancesRep.Add(tempInstanceRep.ID, tempInstanceRep);
                        }

                        OnParseProgressionChanged?.Invoke(0.6f + (float)(index / (double)xmlInstanceReps.Count * 0.1f));
                    });

                    // Parse "Reference3D" nodes
                    Parallel.ForEach(xmlReference3Ds, (reference3D, _, index) =>
                    {
                        Reference3D tempReference3D = Reference3DParser.Parse(reference3D);

                        lock (_references3D)
                        {
                            _references3D.Add(tempReference3D.ID, tempReference3D);
                        }

                        OnParseProgressionChanged?.Invoke(0.7f + (float)(index / (double)xmlReference3Ds.Count * 0.1f));
                    });

                    // Parse "Instance3D" nodes
                    Parallel.ForEach(xmlInstance3Ds, (instance3D, _, index) =>
                    {
                        Instance3D tempInstance3D = Instance3DParser.Parse(instance3D, scale);

                        lock (_instances3D)
                        {
                            _instances3D.Add(tempInstance3D.ID, tempInstance3D);
                        }

                        OnParseProgressionChanged?.Invoke(0.8f + (float)(index / (double)xmlInstance3Ds.Count * 0.1f));
                    });

                    // Convert geometry data to Unity mesh definitions
                    _meshDefinitions = ConvertToMeshDefinitions(_referencesRep.Values);

                    DS3DXMLStructure structure = new DS3DXMLStructure(
                        _header,
                        _referencesRep,
                        _instancesRep,
                        _references3D,
                        _instances3D,
                        _meshDefinitions
                    );

                    OnParseProgressionChanged?.Invoke(1.0f);
                    OnParseCompleted?.Invoke(structure);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            });
        }

        private Dictionary<string, MeshDefinition> ConvertToMeshDefinitions(IEnumerable<ReferenceRep> referenceReps)
        {
            int estimatedSize = referenceReps is ICollection<ReferenceRep> collection ? collection.Count : 0;
            Dictionary<string, MeshDefinition> meshDefinitions = new Dictionary<string, MeshDefinition>(estimatedSize);

            Parallel.ForEach(referenceReps, referenceRep =>
            {
                MeshDefinition meshDefinition = MeshDefinition.FromReferenceRep(referenceRep);

                lock (meshDefinitions)
                {
                    if (!meshDefinitions.ContainsKey(referenceRep.AssociatedFile)) // Eviter les doublons
                    {
                        meshDefinitions[referenceRep.AssociatedFile] = meshDefinition;
                    }
                }
            });

            return meshDefinitions;
        }
        #endregion
    }
}
