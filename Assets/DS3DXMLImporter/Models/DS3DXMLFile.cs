using DS3XMLImporter.Models;
using DS3XMLImporter.Models.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DS3DXMLImporter.Models
{
    internal class DS3DXMLFile : IDS3DXMLArchive
    {
        #region CONSTANTS
        private const string MANIFEST_FILENAME = "Manifest.xml";
        private const string MATERIAL_FILENAME_V5 = "CATMaterialRef.3dxml";
        private const string MATERIAL_FILENAME_V6 = "CATMaterialDisciplines.3dxml";
        #endregion

        #region ATTRIBUTES
        private IDictionary<string, Stream> _files;
        #endregion

        #region PROPERTIES
        public IList<string> ContainedFiles
        {
            get
            {
                return _files.Keys.ToList();
            }
        }
        #endregion

        #region CONSTRUCTOR
        private DS3DXMLFile(IDictionary<string, Stream> files)
        {
            _files = files;
        }
        #endregion

        #region METHODS
        public static IDS3DXMLArchive Create(Stream data)
        {
            Dictionary<string, Stream> dict = new Dictionary<string, Stream>();

            using (var zipArchive = new ZipArchive(data))
            {
                foreach (var entry in zipArchive.Entries)
                {
                    var name = entry.Name.ToLower();
                    var fileStream = new MemoryStream();
                    entry.Open().CopyTo(fileStream);
                    name.Replace(".3Drep", ".xml");
                    dict.Add(name, fileStream);
                }
            }

            DS3DXMLFile archive = new DS3DXMLFile(dict);
            data.Close();

            return archive;
        }

        public XDocument GetManifest()
        {
            return GetNextDocument(MANIFEST_FILENAME);
        }

        public XDocument GetCATMaterials(DS3DXMLHeader header)
        {
            switch (header.Version)
            {
                default:
                case DS3DXMLVersion.Unknown:
                    return null;
                case DS3DXMLVersion.V5:
                    return GetNextDocument(MATERIAL_FILENAME_V5);
                case DS3DXMLVersion.V6:
                    return GetNextDocument(MATERIAL_FILENAME_V6);
            }
        }

        public XDocument GetNextDocument(string name)
        {
            name = name.ToLower();

            if (!_files.ContainsKey(name))
            {
                throw new FileNotFoundException();
            }

            Stream fileStream = _files[name];
            fileStream.Seek(0, SeekOrigin.Begin);
            XmlReader reader = XmlReader.Create(fileStream);
            reader.MoveToContent();

            return XDocument.Load(reader);
        }
        #endregion
    }
}
