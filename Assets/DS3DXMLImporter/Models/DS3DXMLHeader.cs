using System;

namespace DS3XMLImporter.Models
{
    public class DS3DXMLHeader
    {
        #region PROPERTIES
        public string Name { get; set; }

        public string Generator { get; internal set; }

        public string Schema { get; set; }

        public string Author { get; set; }

        public DateTime Created { get; set; }
        #endregion
    }
}
