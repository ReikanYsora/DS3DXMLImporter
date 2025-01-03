using DS3DXMLImporter.Models.Attributes;
using DS3XMLImporter.Models;
using DS3XMLImporter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace DS3XMLImporter.Parsers
{
    internal class ParserHelper
    {
        #region METHODS
        public static DS3DXMLHeader GetHeader(XDocument xmlDocument)
        {
            DS3DXMLHeader header = new DS3DXMLHeader();
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

        public static IList<float> ParseFloatList(string s)
        {
            string[] elements = s.Split();
            return elements.Select(x => (float)Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToList();
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

        public static List<ElementNodeData> ParseElements(XElement node)
        {
            List<ElementNodeData> result = new List<ElementNodeData>();

            foreach (XAttribute attribute in node.Attributes())
            {
                (IComparable typedValue, ElementValueType type) = ConvertValue(attribute.Value);
                result.Add(new ElementNodeData(attribute.Name.LocalName, ElementType.Attribute, type, typedValue));
            }

            foreach (XElement child in node.Elements())
            {
                if (child.HasElements)
                {
                    ElementNodeData childNode = new ElementNodeData(child.Name.LocalName, ElementType.Element, ElementValueType.None, null)
                    {
                        Children = ParseElements(child)
                    };

                    result.Add(childNode);
                }
                else
                {
                    (IComparable typedValue, ElementValueType type) = ConvertValue(child.Value);
                    result.Add(new ElementNodeData(child.Name.LocalName, ElementType.Element, type, typedValue));
                }
            }

            return result;
        }

        private static (IComparable Value, ElementValueType Type) ConvertValue(string value)
        {
            if (long.TryParse(value, out long longValue))
            {
                //01/01/1985 to 01/01/2100
                if (longValue >= 473385600 && longValue <= 4102444800) 
                {
                    try
                    {
                        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(longValue).DateTime;
                        return (dateTime, ElementValueType.DateTime);
                    }
                    catch{}
                }

                return (longValue, ElementValueType.Integer);
            }

            if (int.TryParse(value, out int intValue))
            {
                return (intValue, ElementValueType.Integer);
            }

            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
            {
                return (doubleValue, ElementValueType.Double);
            }

            if (bool.TryParse(value, out bool boolValue))
            {
                return (boolValue, ElementValueType.Boolean);
            }

            return (value, ElementValueType.String);
        }
        #endregion
    }
}
