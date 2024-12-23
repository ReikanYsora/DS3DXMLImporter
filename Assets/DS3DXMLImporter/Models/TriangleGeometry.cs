using UnityEngine;

namespace DS3DXMLImporter.Models
{
    public struct TriangleGeometry
    {
        #region PROPERTIES
        public Vector3 Vertex1 { get; private set; }
        public Vector3 Vertex2 { get; private set; }
        public Vector3 Vertex3 { get; private set; }

        public Vector3 Normal1 { get; private set; }
        public Vector3 Normal2 { get; private set; }
        public Vector3 Normal3 { get; private set; }

        public Color Color1 { get; private set; }
        public Color Color2 { get; private set; }
        public Color Color3 { get; private set; }
        #endregion

        public TriangleGeometry(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 n1, Vector3 n2, Vector3 n3, Color c1, Color c2, Color c3)
        {
            Vertex1 = v1 / 1000f;
            Vertex2 = v2 / 1000f;
            Vertex3 = v3 / 1000f;
            Normal1 = n1 / 1000f;
            Normal2 = n2 / 1000f;
            Normal3 = n3 / 1000f;
            Color1 = c1;
            Color2 = c2;
            Color3 = c3;
        }
    }
}
