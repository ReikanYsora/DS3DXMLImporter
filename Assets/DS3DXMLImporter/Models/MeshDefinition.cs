using DS3XMLImporter.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DS3DXMLImporter.Models
{
    public class MeshDefinition
    {
        #region PROPERTIES
        public string AssociatedFile { get; private set; }

        public IList<Vector3> Vertices { get; private set; }

        public IList<int> Triangles { get; private set; }

        public IList<Vector3> Normals { get; private set; }

        public IList<Color> Colors { get; private set; }
        #endregion

        #region METHODS
        public static MeshDefinition FromReferenceRep(ReferenceRep rep)
        {
            return new MeshDefinition
            {
                AssociatedFile = rep.AssociatedFile,
                Vertices = rep.TriangleGeometries.SelectMany(x => new List<Vector3> { x.Vertex1, x.Vertex2, x.Vertex3 }).ToList(),
                Triangles = Enumerable.Range(0, rep.TriangleGeometries.Count * 3).ToList(),
                Normals = rep.TriangleGeometries.SelectMany(x => new List<Vector3> { x.Normal1, x.Normal2, x.Normal3 }).ToList(),
                Colors = rep.TriangleGeometries.SelectMany(x => new List<Color> { x.Color1, x.Color2, x.Color3 }).ToList()
            };
        }
        #endregion
    }
}
