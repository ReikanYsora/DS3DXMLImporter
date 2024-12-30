using DS3XMLImporter.Models;
using DS3XMLImporter.Models.Interfaces;
using DS3XMLImporter.Parsers;
using System;
using System.Xml.Linq;

namespace DS3DXMLImporter.Parsers
{
    public class CATMatConnectionParser
    {
        #region METHODS
        public static CATMatConnection Parse(XElement cadMaterialXElement, IDS3DXMLArchive archive, float scale)
        {
            int id = int.Parse(cadMaterialXElement.Attribute(XName.Get("id")).Value);
            string name = cadMaterialXElement.Attribute(XName.Get("name")).Value;

            CATMatConnection instanceRep = new CATMatConnection
            {
                ID = id,
                Name = name,
                AggregatedBy = ParserHelper.ValueOfDescendant(cadMaterialXElement, "IsAggregatedBy", Convert.ToInt32, 0)
            };

            instanceRep.ElementsData = ParserHelper.ParseElements(cadMaterialXElement);

            return instanceRep;
        }
        #endregion
    }
}
