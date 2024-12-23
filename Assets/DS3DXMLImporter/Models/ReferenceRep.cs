using DS3DXMLImporter.Models;
using System.Collections.Generic;

namespace DS3XMLImporter.Models
{
    public class ReferenceRep
    {
        #region PROPERTIES
        public int ID { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string Usage { get; set; }

        public string Type { get; set; }

        public string Format { get; set; }

        public string AssociatedFile { get; set; }

        public IList<TriangleGeometry> TriangleGeometries { get; set; }
        #endregion
    }
}
