using DS3XMLImporter.Models;
using DS3XMLImporter.Parsers;
using System;
using System.Xml.Linq;

namespace DS3DXMLImporter.Parsers
{
    public class InstanceRepParser
    {
        #region METHODS
        public static InstanceRep Parse(XElement instanceRepXElement)
        {
            int id = int.Parse(instanceRepXElement.Attribute(XName.Get("id")).Value);
            string name = instanceRepXElement.Attribute(XName.Get("name")).Value;

            InstanceRep instanceRep = new InstanceRep(id, name)
            {
                AggregatedBy = ParserHelper.ValueOfDescendant(instanceRepXElement, "IsAggregatedBy", Convert.ToInt32, 0),
                InstanceOf = ParserHelper.ValueOfDescendant(instanceRepXElement, "IsInstanceOf", Convert.ToInt32, 0)
            };

            instanceRep.ElementsData = ParserHelper.ParseElements(instanceRepXElement);

            return instanceRep;
        }
        #endregion
    }
}
