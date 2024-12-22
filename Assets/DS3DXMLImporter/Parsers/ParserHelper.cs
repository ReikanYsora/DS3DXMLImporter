using DS3XMLImporter.Models;
using DS3XMLImporter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DS3XMLImporter.Parsers
{
    internal class ParserHelper
    {
        #region METHODS
        public static string GetName(XDocument data)
        {
            XElement titleNode = data.XPathSelectElement("Model_3dxml");

            return titleNode.Value;
        }

        public static DS3DXMLHeader GetHeader(XDocument xmlDocument)
        {
            DS3DXMLHeader header = new DS3DXMLHeader();

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("3dxml", "http://www.3ds.com/xsd/3DXML");

            IEnumerable<XElement> xmlHeader = xmlDocument.Root.Element("{http://www.3ds.com/xsd/3DXML}Header").Elements();

            if (xmlHeader == null)
            {
                throw new ArgumentException(@"The given XML file seems to hold no header information. Please make sure that you are using the correct XDocument.");
            }

            foreach (var elem in xmlHeader.ToList())
            {
                switch (elem.Name.ToString().Replace("{http://www.3ds.com/xsd/3DXML}", "").ToLower())
                {
                    case "title":
                        header.Name = elem.Value;
                        break;
                    case "author":
                        header.Author = elem.Value;
                        break;
                    case "schema":
                        header.Schema = elem.Value;
                        break;
                    case "schemaversion":
                        header.Schema = elem.Value;
                        break;
                    case "generator":
                        header.Generator = elem.Value;
                        break;
                    case "created":
                        try
                        {
                            header.Created = DateTime.Parse(elem.Value);
                        }
                        catch (Exception)
                        {

                        }
                        break;
                }
            }

            return header;
        }

        public static XDocument ReadManifest(IDS3DXMLArchive archiveModel)
        {
            XDocument manifest = archiveModel.GetManifest();
            XElement rootElement = manifest.Root.Element("Root");

            if (rootElement != null && !rootElement.IsEmpty)
            {
                manifest = archiveModel.GetNextDocument(CleanUpFileName(rootElement.Value));
            }

            return manifest;
        }

        public static string CleanUpFileName(string filename)
        {
            return filename.Split(":".ToCharArray()).Last();
        }

        public static IEnumerable<XElement> RootDescendants(XDocument document, string name)
        {
            return document.Root.Descendants("{http://www.3ds.com/xsd/3DXML}" + name);
        }

        public static T ValueOfDescendant<T>(XElement xElement, string name, Func<string, T> mapping, T defaultValue)
        {
            XElement element = xElement.Descendants().FirstOrDefault(x => x.Name.LocalName == name);

            if (element != null)
            {
                return mapping(element.Value);
            }

            return defaultValue;
        }
        #endregion
    }
}
