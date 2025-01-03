using DS3DXMLImporter.Models;
using DS3XMLImporter.Models;
using DS3XMLImporter.Models.Interfaces;
using DS3XMLImporter.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace DS3DXMLImporter.Parsers
{
    public class ReferenceRepParser
    {
        #region METHODS
        internal static ReferenceRep Parse(XElement referenceRepXElement, IDS3DXMLArchive archive, float scale)
        {
            ReferenceRep referenceRep = new ReferenceRep();

            foreach (var attribut in referenceRepXElement.Attributes())
            {
                switch (attribut.Name.LocalName.ToLower())
                {
                    default:
                        break;
                    case "id":
                        referenceRep.ID = Convert.ToInt32(attribut.Value);
                        break;
                    case "name":
                        referenceRep.Name = attribut.Value;
                        break;
                    case "type":
                        referenceRep.Type = attribut.Value;
                        break;
                    case "version":
                        referenceRep.Version = attribut.Value;
                        break;
                    case "format":
                        referenceRep.Format = attribut.Value;
                        break;
                    case "usage":
                        referenceRep.Usage = attribut.Value;
                        break;
                    case "associatedfile":
                        referenceRep.AssociatedFile = attribut.Value;
                        break;
                }
            }

            referenceRep.TriangleGeometries = GetGeometry(referenceRepXElement, referenceRep.AssociatedFile, archive, scale);
            referenceRep.ElementsData = ParserHelper.ParseElements(referenceRepXElement);

            return referenceRep;
        }

        private static IList<TriangleGeometry> GetGeometry(XElement referenceRepGeometry, string externalFileName, IDS3DXMLArchive archive, float scale)
        {
            XDocument xmlReferenceRep = externalFileName?.Any() == true ? archive.GetNextDocument(ParserHelper.CleanUpFileName(externalFileName)) : referenceRepGeometry.Document;
            IList<XElement> bagReps = GetBagRepXmlElements(xmlReferenceRep);

            if (bagReps == null || !bagReps.Any())
            {
                return Array.Empty<TriangleGeometry>();
            }

            List<TriangleGeometry> triangles = new List<TriangleGeometry>();

            foreach (var bagRep in bagReps)
            {
                IList<Vector3> vertices = GetVerticesFromXml(bagRep, scale);
                IList<Vector3> normals = GetNormalsFromXml(bagRep, scale);

                IEnumerable<XElement> faces = bagRep
                    .Descendants("{http://www.3ds.com/xsd/3DXML}Faces")
                    .Where(x => x.Parent.Name.LocalName.ToLowerInvariant() != "polygonallod")
                    .SelectMany(faceGroup => faceGroup.Elements("{http://www.3ds.com/xsd/3DXML}Face"));

                foreach (var face in faces)
                {
                    AddTriangles(triangles, face, vertices, normals);
                }
            }

            return triangles;
        }

        private static void AddTriangles(List<TriangleGeometry> triangles, XElement face, IList<Vector3> vertices, IList<Vector3> normals)
        {
            triangles.AddRange(GetTriangles(face, vertices, normals));
            triangles.AddRange(GetFans(face, vertices, normals));
            triangles.AddRange(GetStrips(face, vertices, normals));
        }

        private static IList<TriangleGeometry> GetTriangles(XElement face, IList<Vector3> vertices, IList<Vector3> normals)
        {
            List<TriangleGeometry> triangles = new List<TriangleGeometry>();

            XAttribute triangleAttribute = face.Attribute("triangles");

            if (triangleAttribute == null)
            {
                return triangles;
            }

            Color color = ExtractColor(face);

            string[] faceStringAry = triangleAttribute.Value.Trim().Split(' ');

            if (faceStringAry.Length % 3 != 0)
            {
                throw new FormatException("Invalid triangle indices data.");
            }

            int triangleCount = faceStringAry.Length / 3;

            triangles = new List<TriangleGeometry>(triangleCount);

            Parallel.For(0, triangleCount, i =>
            {
                int index1 = int.Parse(faceStringAry[i * 3]);
                int index2 = int.Parse(faceStringAry[i * 3 + 1]);
                int index3 = int.Parse(faceStringAry[i * 3 + 2]);

                TriangleGeometry triangle = new TriangleGeometry(
                    vertices[index2], vertices[index1], vertices[index3],
                    normals[index2], normals[index1], normals[index3],
                    color, color, color
                );

                lock (triangles)
                {
                    triangles.Add(triangle);
                }
            });

            return triangles;
        }

        private static IList<TriangleGeometry> GetFans(XElement face, IList<Vector3> vertices, IList<Vector3> normals)
        {
            XAttribute fansAttribute = face.Attribute("fans");

            if (fansAttribute == null)
            {
                return new List<TriangleGeometry>();
            }

            Color color = ExtractColor(face);

            string[] fansArray = fansAttribute.Value.Trim().Split(',');
            List<TriangleGeometry> triangles = new List<TriangleGeometry>();

            triangles = fansArray.AsParallel()
                .Select(fan =>
                {
                    List<int> indices = fan.Split(' ')
                                           .Select(int.Parse)
                                           .ToList();

                    return FanToTriangles(indices, vertices, normals, color);
                })
                .SelectMany(tri => tri)
                .ToList();

            return triangles;
        }

        private static IList<TriangleGeometry> FanToTriangles(List<int> indices, IList<Vector3> vertices, IList<Vector3> normals, Color color)
        {
            var triangles = new List<TriangleGeometry>();

            Vector3 sharedVertex = vertices[indices[0]];
            Vector3 sharedNormal = normals[indices[0]];

            for (int i = 1; i < indices.Count - 1; i++)
            {
                Vector3 v1 = vertices[indices[i]];
                Vector3 v2 = vertices[indices[i + 1]];
                Vector3 n1 = normals[indices[i]];
                Vector3 n2 = normals[indices[i + 1]];

                triangles.Add(new TriangleGeometry(
                    sharedVertex, v1, v2,
                    sharedNormal, n1, n2,
                    color, color, color 
                ));
            }

            return triangles;
        }

        private static IList<TriangleGeometry> GetStrips(XElement face, IList<Vector3> vertices, IList<Vector3> normals)
        {
            XAttribute stripsAttributes = face.Attribute("strips");

            if (stripsAttributes == null)
            {
                return new List<TriangleGeometry>();
            }

            Color color = ExtractColor(face);

            string[] stripsArray = stripsAttributes.Value.Trim().Split(',');

            List<TriangleGeometry> triangles = stripsArray.AsParallel()
                .Select(strip =>
                {
                    List<int> indices = strip.Split(' ')
                                             .Select(int.Parse)
                                             .ToList();

                    return StripToTriangles(indices, vertices, normals, color);
                })
                .SelectMany(tri => tri)
                .ToList();

            return triangles;
        }

        private static IList<TriangleGeometry> StripToTriangles(IList<int> indices, IList<Vector3> vertices, IList<Vector3> normals, Color color)
        {
            List<TriangleGeometry> triangles = new List<TriangleGeometry>();
            bool reverseOrder = true;

            for (int i = 1; i < indices.Count - 1; i++)
            {
                int prev = i - 1;
                int next = i + 1;

                triangles.Add(new TriangleGeometry(
                    vertices[indices[i]],
                    vertices[indices[reverseOrder ? prev : next]],
                    vertices[indices[reverseOrder ? next : prev]],
                    normals[indices[i]],
                    normals[indices[reverseOrder ? prev : next]],
                    normals[indices[reverseOrder ? next : prev]],
                    color, color, color
                ));

                reverseOrder = !reverseOrder;
            }

            return triangles;
        }

        private static Color ExtractColor(XElement face)
        {
            XElement surfaceAttributes = face.Element("{http://www.3ds.com/xsd/3DXML}SurfaceAttributes");

            if (surfaceAttributes == null)
            {
                return new Color(0f, 0f, 0f, 0f);
            }

            XElement colorElement = surfaceAttributes.Element("{http://www.3ds.com/xsd/3DXML}Color");

            if (colorElement == null)
            {
                return new Color(0f, 0f, 0f, 0f);
            }

            return new Color(
                float.Parse(colorElement.Attribute("red").Value, CultureInfo.InvariantCulture),
                float.Parse(colorElement.Attribute("green").Value, CultureInfo.InvariantCulture),
                float.Parse(colorElement.Attribute("blue").Value, CultureInfo.InvariantCulture),
                float.Parse(colorElement.Attribute("alpha").Value, CultureInfo.InvariantCulture)
            );
        }

        private static IList<XElement> GetBagRepXmlElements(XDocument threeDReferenceRepXmlElement)
        {
            try
            {
                return threeDReferenceRepXmlElement.Descendants("{http://www.3ds.com/xsd/3DXML}Rep").Where(FlattenBagReps).Where(FacesWithoutPolyLod).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool FlattenBagReps(XElement arg)
        {
            return arg.Elements().All(x => x.Name.LocalName != "Rep");
        }

        private static bool FacesWithoutPolyLod(XElement arg)
        {
            return arg.Descendants("{http://www.3ds.com/xsd/3DXML}Faces").Any(x => x.Parent.Name.LocalName != "PolygonalLOD");
        }

        private static IList<Vector3> GetVerticesFromXml(XElement bagRep, float scale)
        {
            XElement vertexPositionXml = bagRep.Descendants("{http://www.3ds.com/xsd/3DXML}Positions")
                                               .FirstOrDefault(x => x.Parent.Name.LocalName == "VertexBuffer");

            if (vertexPositionXml == null)
            {
                return new List<Vector3>();
            }

            string[] coordinatesArray = vertexPositionXml.Value.Split(',');

            List<Vector3> vertices = coordinatesArray.AsParallel()
                                           .Select(c =>
                                           {
                                               string[] coordinateAry = c.Split(' ');
                                               float x = float.Parse(coordinateAry[0], CultureInfo.InvariantCulture);
                                               float y = float.Parse(coordinateAry[1], CultureInfo.InvariantCulture);
                                               float z = float.Parse(coordinateAry[2], CultureInfo.InvariantCulture);
                                               return new Vector3(x, z, y) / scale;
                                           })
                                           .ToList();

            if (vertices.Count == 0)
            {
                throw new FormatException(string.Format(@"<Positions> is missing in root {0}", bagRep.Document.Root.Name.LocalName));
            }

            return vertices;
        }

        private static IList<Vector3> GetNormalsFromXml(XElement bagRep, float scale)
        {
            XElement vertexPositionXml = bagRep.Descendants("{http://www.3ds.com/xsd/3DXML}Normals")
                                               .FirstOrDefault(x => x.Parent.Name.LocalName == "VertexBuffer");

            if (vertexPositionXml == null)
            {
                return new List<Vector3>();
            }

            string[] coordinatesArray = vertexPositionXml.Value.Split(',');

            List<Vector3> normals = coordinatesArray.AsParallel()
                                           .Select(c =>
                                           {
                                               string[] coordinateAry = c.Split(' ');
                                               float x = float.Parse(coordinateAry[0], CultureInfo.InvariantCulture);
                                               float y = float.Parse(coordinateAry[1], CultureInfo.InvariantCulture);
                                               float z = float.Parse(coordinateAry[2], CultureInfo.InvariantCulture);
                                               return new Vector3(x, z, y) / scale;
                                           })
                                           .ToList();

            if (normals.Count == 0)
            {
                throw new FormatException(string.Format(@"<Normals> is missing in root {0}", bagRep.Document.Root.Name.LocalName));
            }

            return normals;
        }
        #endregion
    }
}
