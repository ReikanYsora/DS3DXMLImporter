using DS3XMLImporter.Models;
using DS3XMLImporter.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace DS3DXMLImporter.Parsers
{
    public class Instance3DParser
    {
        #region METHODS
        public static Instance3D FromXDocument(XElement xElement)
        {
            int id = int.Parse(xElement.Attribute(XName.Get("id")).Value);
            string name = xElement.Attribute(XName.Get("name")).Value;

            Instance3D instance = new Instance3D(id, name)
            {
                AggregatedBy = ParserHelper.ValueOfDescendant(xElement, "IsAggregatedBy", Convert.ToInt32, 0),
                InstanceOf = ParserHelper.ValueOfDescendant(xElement, "IsInstanceOf", Convert.ToInt32, 0),
                RelativeMatrix = ParserHelper.ValueOfDescendant<IList<double>>(xElement, "RelativeMatrix", ParseList, new List<double>())
            };
            return instance;
        }

        public static IList<double> ParseList(string s)
        {
            string[] elements = s.Split();
            List<double> list = elements.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToList();

            return list;
        }
        #endregion
    }
}
