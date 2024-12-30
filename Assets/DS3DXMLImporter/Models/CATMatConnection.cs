using DS3DXMLImporter.Models.Attributes;
using System.Collections.Generic;

namespace DS3XMLImporter.Models
{
    public class CATMatConnection
    {
        #region PROPERTIES
        public int ID { get; set; }

        public string Name { get; set; }

        public int AggregatedBy { get; set; }

        public List<ElementNodeData> ElementsData { get; set; } = new List<ElementNodeData>();
        #endregion
    }
}
