using DS3DXMLImporter.Models.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace DS3DXMLImporter.Models
{
    public class DS3DXMLModel
    {
        #region PROPERTIES
        public string Name { get; set; }

        public string Author { get; set; }

        public IList<Face> Faces { get; set; }

        public IList<TransformDefinition> TransformDefinitions { get; set; }

        public IList<Vector3> Vertices { get; }

        public IList<int> Triangles { get; }

        public IList<Vector3> Normals { get; }

        public IList<Color> Colors { get; }
        #endregion
    }
}
