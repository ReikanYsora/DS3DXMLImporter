using DS3XMLImporter.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DS3DXMLImporter.Models.Unity
{
    public class TransformDefinition
    {
        #region PROPERTIES
        public IList<Vector3> Vertices { get; private set; }

        public IList<int> Triangles { get; private set; }

        public IList<Vector3> Normals { get; private set; }

        public IList<Color> Colors { get; private set; }

        public string Name { get; private set; }

        public Matrix4x4 Rotation { get; set; }

        public Vector3 Position { get; set; }
        #endregion

        #region METHODS
        public static TransformDefinition FromReferenceRep(ReferenceRep rep, Instance3D instance3D)
        {
            IList<TriangleGeometry> triangles = rep.MeshProperty.TriangleGeometries;

            return new TransformDefinition
            {
                Name = rep.Name,
                Vertices = triangles.SelectMany(x => new List<Vector3> { x.Vertex1, x.Vertex2, x.Vertex3 }).ToList(),
                Triangles = Enumerable.Range(0, triangles.Count * 3).ToList(),
                Normals = triangles.SelectMany(x => new List<Vector3> { x.Normal1, x.Normal2, x.Normal3 }).ToList(),
                Colors = triangles.SelectMany(x => new List<Color> { x.Color1, x.Color2, x.Color3 }).ToList(),
                Position = instance3D.Position,
                Rotation = instance3D.Rotation
            };
        }
        #endregion
    }
}
