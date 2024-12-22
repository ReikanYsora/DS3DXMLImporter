using DS3XMLImporter.Models;
using System.Xml.Linq;

namespace DS3DXMLImporter.Parsers
{
    public class Reference3DParser
    {
        #region METHODS
        public static Reference3D FromXDocument(XElement xElement)
        {
            var id = int.Parse(xElement.Attribute(XName.Get("id")).Value);
            var name = xElement.Attribute(XName.Get("name")).Value;

            return new Reference3D(id, name);
        }
        #endregion
    }    
}
