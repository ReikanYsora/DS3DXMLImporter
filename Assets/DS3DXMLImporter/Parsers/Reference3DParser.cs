using DS3XMLImporter.Models;
using DS3XMLImporter.Parsers;
using System.Xml.Linq;

namespace DS3DXMLImporter.Parsers
{
    public class Reference3DParser
    {
        #region METHODS
        public static Reference3D Parse(XElement reference3DXElement)
        {
            int id = int.Parse(reference3DXElement.Attribute(XName.Get("id")).Value);
            string name = reference3DXElement.Attribute(XName.Get("name")).Value;
            Reference3D reference3D = new Reference3D(id, name);
            reference3D.ElementsData = ParserHelper.ParseElements(reference3DXElement);

            return reference3D;
        }
        #endregion
    }    
}
