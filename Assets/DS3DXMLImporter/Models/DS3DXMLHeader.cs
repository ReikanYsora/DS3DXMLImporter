using System;

namespace DS3XMLImporter.Models
{
    public class DS3DXMLHeader
    {
        #region PROPERTIES
        public string Name { get; set; }

        public DS3DXMLVersion Version { get; set; }

        public string Generator { get; internal set; }

        public string Schema { get; set; }

        public string Author { get; set; }

        public DateTime Created { get; set; }
        #endregion
    }

    public enum DS3DXMLVersion
    {
        Unknown, V5, V6
    }
}
