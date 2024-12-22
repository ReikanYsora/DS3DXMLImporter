using System;
using System.Collections.Generic;

namespace DS3DXMLImporter.Models
{
    public struct MeshProperty
    {
        #region PROPERTIES
        public float Accuracy { get; private set; }

        public IList<TriangleGeometry> TriangleGeometries;

        public Boolean HasLod { get; private set; }
        #endregion

        #region CONSTRUCTOR
        public MeshProperty(float accuracy, IList<TriangleGeometry> triangleGeometries)
        {
            Accuracy = accuracy;
            TriangleGeometries = triangleGeometries;
            HasLod = true;
        }

        public MeshProperty(IList<TriangleGeometry> triangleGeometries)
        {
            Accuracy = 100.0f;
            TriangleGeometries = triangleGeometries;
            HasLod = false;
        }
        #endregion
    }
}
