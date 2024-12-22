using DS3XMLImporter.Models;
using DS3XMLImporter.Parsers;
using System;
using System.Xml.Linq;

namespace DS3DXMLImporter.Parsers
{
    public class InstanceRepParser
    {
        #region METHODS
        public static InstanceRep FromXDocument(XElement xElement)
        {
            int id = int.Parse(xElement.Attribute(XName.Get("id")).Value);
            string name = xElement.Attribute(XName.Get("name")).Value;

            return new InstanceRep(id, name)
            {
                AggregatedBy = ParserHelper.ValueOfDescendant(xElement, "IsAggregatedBy", Convert.ToInt32, 0),
                InstanceOf = ParserHelper.ValueOfDescendant(xElement, "IsInstanceOf", Convert.ToInt32, 0)
            };
        }
        #endregion
    }
}
