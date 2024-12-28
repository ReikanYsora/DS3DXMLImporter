using DS3DXMLImporter.Loaders;
using DS3DXMLImporter.Models;
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

                    //Parse "Header"
                    _header = ParserHelper.GetHeader(xmlManifest);
                    OnParseProgressionChanged?.Invoke(0.2f);

                    //Parse "ReferenceRep" nodes
                    _referencesRep = new Dictionary<int, ReferenceRep>();
                    IEnumerable<XElement> xmlReferenceReps = xmlManifest.Root.Descendants("{http://www.3ds.com/xsd/3DXML}ReferenceRep");

                    int minProgress = 0;
                    int maxProgress = xmlReferenceReps.Count();

                    foreach (XElement referenceRep in xmlReferenceReps)
                    {
                        ReferenceRep tempReferenceRep = ReferenceRepParser.Parse(referenceRep, fileArchive, scale);
                        _referencesRep.Add(tempReferenceRep.ID, tempReferenceRep);
                        minProgress++;
                        OnParseProgressionChanged?.Invoke((float) Math.Round(0.2f + (minProgress * (0.4f / maxProgress)), 2));
                    }

                    //Parse "InstanceRep" nodes
                    _instancesRep = new Dictionary<int, InstanceRep>();
                    IEnumerable<XElement> xmlInstanceReps = xmlManifest.Root.Descendants("{http://www.3ds.com/xsd/3DXML}InstanceRep");

                    minProgress = 0;
                    maxProgress = xmlInstanceReps.Count();

                    foreach (XElement instanceRep in xmlInstanceReps)
                    {
                        InstanceRep tempInstanceRep = InstanceRepParser.Parse(instanceRep);
                        _instancesRep.Add(tempInstanceRep.ID, tempInstanceRep);
                        minProgress++;
                        OnParseProgressionChanged?.Invoke((float)Math.Round(0.6f + (minProgress * (0.1f / maxProgress)), 2));
                    }

                    //Parse "Reference3D" nodes
                    _references3D = new Dictionary<int, Reference3D>();
                    IEnumerable<XElement> xmlReference3Ds = xmlManifest.Root.Descendants("{http://www.3ds.com/xsd/3DXML}Reference3D");

                    minProgress = 0;
                    maxProgress = xmlReference3Ds.Count();

                    foreach (XElement reference3D in xmlReference3Ds)
                    {
                        Reference3D tempReference3D = Reference3DParser.Parse(reference3D);
                        _references3D.Add(tempReference3D.ID, tempReference3D);
                        minProgress++;
                        OnParseProgressionChanged?.Invoke((float)Math.Round(0.7f + (minProgress * (0.1f / maxProgress)), 2));
                    }

                    //Parse "Instance3D" nodes
                    _instances3D = new Dictionary<int, Instance3D>();
                    IEnumerable<XElement> xmlInstance3Ds = xmlManifest.Root.Descendants("{http://www.3ds.com/xsd/3DXML}Instance3D");

                    minProgress = 0;
                    maxProgress = xmlInstance3Ds.Count();

                    foreach (XElement instance3D in xmlInstance3Ds)
                    {
                        Instance3D tempInstance3D = Instance3DParser.Parse(instance3D, scale);
                        _instances3D.Add(tempInstance3D.ID, tempInstance3D);
                        minProgress++;
                        OnParseProgressionChanged?.Invoke((float)Math.Round(0.8f + (minProgress * (0.1f / maxProgress)), 2));
                    }

                    //Convert geometry data to Unity mesh definition
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
            Dictionary<string, MeshDefinition> meshDefinitions = new Dictionary<string, MeshDefinition>();

            foreach (ReferenceRep referenceRep in referenceReps)
            {
                meshDefinitions.Add(referenceRep.AssociatedFile, MeshDefinition.FromReferenceRep(referenceRep));
            }

            return meshDefinitions;
        }
        #endregion
    }
}
