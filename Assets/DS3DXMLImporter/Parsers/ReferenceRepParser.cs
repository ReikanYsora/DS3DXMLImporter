using DS3DXMLImporter.Models;
using DS3XMLImporter.Models;
using DS3XMLImporter.Models.Interfaces;
using DS3XMLImporter.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            XDocument xmlReferenceRep;
            IList<TriangleGeometry> triangles = new List<TriangleGeometry>();

            if (externalFileName != null && externalFileName.Any())
            {
                xmlReferenceRep = archive.GetNextDocument(ParserHelper.CleanUpFileName(externalFileName));
            }
            else
            {
                xmlReferenceRep = referenceRepGeometry.Document;
            }

            IList<XElement> bagReps = GetBagRepXmlElements(xmlReferenceRep);

            if (bagReps != null)
            {
                foreach (var bagRep in bagReps)
                {
                    IList<XElement> faces = bagRep.Descendants("{http://www.3ds.com/xsd/3DXML}Faces").Where(x => x.Parent.Name.LocalName.ToLower() != "polygonallod").ToList();
                    IList<Vector3> verticies = GetVerticesFromXml(bagRep, scale);
                    IList<Vector3> normals = GetNormalsFromXml(bagRep, scale);

                    foreach (var face in faces.Elements("{http://www.3ds.com/xsd/3DXML}Face"))
                    {
                        triangles = triangles.Concat(GetTriangles(face, verticies, normals)).ToList();
                        triangles = triangles.Concat(GetFans(face, verticies, normals)).ToList();
                        triangles = triangles.Concat(GetStrips(face, verticies, normals)).ToList();
                    }
                }
            }

            return triangles;
        }

        private static IList<TriangleGeometry> GetTriangles(XElement face, IList<Vector3> verticies, IList<Vector3> normals)
        {
            IList<TriangleGeometry> triangles = new List<TriangleGeometry>();
            XAttribute triangleAttribute = face.Attribute("triangles");

            if (triangleAttribute == null)
            {
                return triangles;
            }

            XElement surfaceAttributes = face.Element("{http://www.3ds.com/xsd/3DXML}SurfaceAttributes");
            Color color = new Color(0f, 0f, 0f, 0f);

            if (surfaceAttributes != null)
            {
                XElement colorElement = surfaceAttributes.Element("{http://www.3ds.com/xsd/3DXML}Color");

                if (color != null)
                {
                    float red = float.Parse(colorElement.Attribute("red").Value, CultureInfo.InvariantCulture);
                    float green = float.Parse(colorElement.Attribute("green").Value, CultureInfo.InvariantCulture);
                    float blue = float.Parse(colorElement.Attribute("blue").Value, CultureInfo.InvariantCulture);
                    float alpha = float.Parse(colorElement.Attribute("alpha").Value, CultureInfo.InvariantCulture);
                    color = new Color(red, green, blue, alpha);
                }
            }

            var faceStringAry = face.Attribute("triangles").Value.Trim().Split(' ').ToArray();

            for (int i = 0; i < faceStringAry.Length; i += 3)
            {
                Vector3 x = verticies[int.Parse(faceStringAry[i + 1])];
                Vector3 y = verticies[int.Parse(faceStringAry[i])];
                Vector3 z = verticies[int.Parse(faceStringAry[i + 2])];
                Vector3 nx = normals[int.Parse(faceStringAry[i + 1])];
                Vector3 ny = normals[int.Parse(faceStringAry[i])];
                Vector3 nz = normals[int.Parse(faceStringAry[i + 2])];
                Color c1 = color;
                Color c2 = color;
                Color c3 = color;

                triangles.Add(new TriangleGeometry(x, y, z, nx, ny, nz, c1, c2, c3));
            }

            return triangles;
        }

        private static IList<TriangleGeometry> GetFans(XElement face, IList<Vector3> verticies, IList<Vector3> normals)
        {
            XAttribute fansAttribute = face.Attribute("fans");

            if (fansAttribute == null)
            {
                return new List<TriangleGeometry>();
            }

            XElement surfaceAttributes = face.Element("{http://www.3ds.com/xsd/3DXML}SurfaceAttributes");
            Color color = new Color(0f, 0f, 0f, 0f);

            if (surfaceAttributes != null)
            {
                XElement colorElement = surfaceAttributes.Element("{http://www.3ds.com/xsd/3DXML}Color");

                if (color != null)
                {
                    float red = float.Parse(colorElement.Attribute("red").Value, CultureInfo.InvariantCulture);
                    float green = float.Parse(colorElement.Attribute("green").Value, CultureInfo.InvariantCulture);
                    float blue = float.Parse(colorElement.Attribute("blue").Value, CultureInfo.InvariantCulture);
                    float alpha = float.Parse(colorElement.Attribute("alpha").Value, CultureInfo.InvariantCulture);
                    color = new Color(red, green, blue, alpha);
                }
            }

            IList<Color> colors = new List<Color>();

            for (int i = 0; i < verticies.Count; i++)
            {
                colors.Add(color);
            }

            return fansAttribute.Value.Trim().Split(',')
                .Select(x => x.Split(' ').Select(y => Convert.ToInt32(y)).ToList())
                .SelectMany(x => FanToTriangles(x, verticies, normals, colors))
                .ToList();
        }

        private static IList<TriangleGeometry> GetStrips(XElement face, IList<Vector3> verticies, IList<Vector3> normals)
        {
            XAttribute stripsAttributes = face.Attribute("strips");

            if (stripsAttributes == null)
            {
                return new List<TriangleGeometry>();
            }

            XElement surfaceAttributes = face.Element("{http://www.3ds.com/xsd/3DXML}SurfaceAttributes");
            Color color = new Color(0f, 0f, 0f, 0f);

            if (surfaceAttributes != null)
            {
                XElement colorElement = surfaceAttributes.Element("{http://www.3ds.com/xsd/3DXML}Color");

                if (color != null)
                {
                    float red = float.Parse(colorElement.Attribute("red").Value, CultureInfo.InvariantCulture);
                    float green = float.Parse(colorElement.Attribute("green").Value, CultureInfo.InvariantCulture);
                    float blue = float.Parse(colorElement.Attribute("blue").Value, CultureInfo.InvariantCulture);
                    float alpha = float.Parse(colorElement.Attribute("alpha").Value, CultureInfo.InvariantCulture);
                    color = new Color(red, green, blue, alpha);
                }
            }

            IList<Color> colors = new List<Color>();

            for (int i = 0; i < verticies.Count; i++)
            {
                colors.Add(color);
            }

            return stripsAttributes.Value.Trim().Split(',')
                .Select(x => x.Split(' ').Select(y => Convert.ToInt32(y)).ToList())
                .SelectMany(x => StripToTriangles(x, verticies, normals, colors))
                .ToList();
        }

        private static IList<TriangleGeometry> FanToTriangles(IList<int> indices, IList<Vector3> verticies, IList<Vector3> normals, IList<Color> colors)
        {
            int center = indices[0];
            List<TriangleGeometry> list = new List<TriangleGeometry>();

            for (int i = 1; i < indices.Count - 1; i++)
            {
                list.Add(new TriangleGeometry(verticies[center], verticies[indices[i + 1]], verticies[indices[i]], normals[center], normals[indices[i + 1]], normals[indices[i]], colors[center], colors[indices[i + 1]], colors[indices[i]]));
            }

            return list;
        }

        private static IList<TriangleGeometry> StripToTriangles(IList<int> indices, IList<Vector3> verticies, IList<Vector3> normals, IList<Color> colors)
        {
            List<TriangleGeometry> list = new List<TriangleGeometry>();
            bool op = true;

            for (int i = 1; i < indices.Count - 1; i++)
            {
                int prev = i - 1;
                int next = i + 1;
                list.Add(new TriangleGeometry(verticies[indices[i]], verticies[indices[op ? prev : next]], verticies[indices[op ? next : prev]], normals[indices[i]], normals[indices[op ? prev : next]], normals[indices[op ? next : prev]], colors[indices[i]], colors[indices[op ? prev : next]], colors[indices[op ? next : prev]]));
                op = !op;
            }

            return list;
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
            IEnumerable<XElement> vertexPositionsXml = bagRep.Descendants("{http://www.3ds.com/xsd/3DXML}Positions").Where(x => x.Parent.Name.LocalName == "VertexBuffer");

            if (vertexPositionsXml.Count() > 1)
            {
                throw new ArgumentException(string.Format(@"Too much <Positions> tag in root {0}", bagRep.Document.Root.Name.LocalName));
            }

            List<Vector3> vertices = new List<Vector3>();
            XElement vertexPositionXml;

            try
            {
                vertexPositionXml = vertexPositionsXml.First();
            }
            catch (Exception)
            {
                return vertices;
            }

            foreach (var cordinates in vertexPositionXml.Value.Split(','))
            {
                string[] coordinateAry = cordinates.Split(' ');
                float x = 0f;
                float y = 0f;
                float z = 0f;

                x = float.Parse(coordinateAry[0], CultureInfo.InvariantCulture);
                y = float.Parse(coordinateAry[1], CultureInfo.InvariantCulture);
                z = float.Parse(coordinateAry[2], CultureInfo.InvariantCulture);

                vertices.Add(new Vector3(x, z, y) / scale);
            }

            if (vertices.Count == 0)
            {
                throw new FormatException(string.Format(@"<Positions> is missing in root {0}", bagRep.Document.Root.Name.LocalName));
            }

            return vertices;
        }

        private static IList<Vector3> GetNormalsFromXml(XElement bagRep, float scale)
        {
            IEnumerable<XElement> normalPositionsXml = bagRep.Descendants("{http://www.3ds.com/xsd/3DXML}Normals").Where(x => x.Parent.Name.LocalName == "VertexBuffer");

            if (normalPositionsXml.Count() > 1)
            {
                throw new ArgumentException(string.Format(@"Too much <Normals> tag in root {0}", bagRep.Document.Root.Name.LocalName));
            }

            List<Vector3> normals = new List<Vector3>();
            XElement normalPositionsXML;

            try
            {
                normalPositionsXML = normalPositionsXml.First();
            }
            catch (Exception)
            {
                return normals;
            }

            foreach (var cordinates in normalPositionsXML.Value.Split(','))
            {
                string[] coordinateAry = cordinates.Split(' ');
                float x = 0f;
                float y = 0f;
                float z = 0f;

                x = float.Parse(coordinateAry[0], CultureInfo.InvariantCulture);
                y = float.Parse(coordinateAry[1], CultureInfo.InvariantCulture);
                z = float.Parse(coordinateAry[2], CultureInfo.InvariantCulture);

                normals.Add(new Vector3(x, z, y) / scale);
            }

            if (normals.Count == 0)
            {
                throw new FormatException(string.Format(@"<Normals> is missing in root {0}", bagRep.Document.Root.Name.LocalName));
            }

            return normals;
        }
        #endregion
    }
}
