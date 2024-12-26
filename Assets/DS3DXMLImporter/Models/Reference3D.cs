using DS3DXMLImporter.Models.Attributes;
using System.Collections.Generic;

namespace DS3XMLImporter.Models
{
    public class Reference3D
    {
        #region PROPERTIES
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<ElementNodeData> ElementsData { get; set; }
        #endregion

        #region CONSTRUCTOR
        public Reference3D(int id, string name)
        {
            ID = id;
            Name = name;
            ElementsData = new List<ElementNodeData>();
        }
        #endregion
    }
}
