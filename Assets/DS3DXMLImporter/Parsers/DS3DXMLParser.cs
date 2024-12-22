using DS3DXMLImporter.Loaders;
using DS3DXMLImporter.Models;
using DS3DXMLImporter.Parsers.Interfaces;
using DS3XMLImporter.Models;
using DS3XMLImporter.Models.Interfaces;
using DS3XMLImporter.Parsers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DS3DXMLImporter.Parsers
{
    internal class DS3DXMLParser : IParser
    {
        #region METHODS
        public DS3DXMLStructure ParseStructure(ILoader loader)
        {
            return ParseStructure(loader.Load());
        }

        public DS3DXMLStructure ParseStructure(Stream stream)
        {
            IDS3DXMLArchive fileArchive = DS3DXMLFile.Create(stream);
            XDocument xmlManifest = ReadManifest(fileArchive);
            DS3DXMLHeader header = ParserHelper.GetHeader(xmlManifest);

            DS3DXMLManager internalModel = new DS3DXMLManager(header)
            {
                ReferencesRep = ParseReferenceRep(xmlManifest, fileArchive),
                InstancesRep = ParseInstanceRep(xmlManifest),
                References3D = ParseReference3D(xmlManifest),
                Instances3D = ParseInstance3D(xmlManifest)
            };

            // return the model definition
            return internalModel.ToStructure();
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
