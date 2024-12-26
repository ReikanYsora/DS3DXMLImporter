using DS3XMLImporter.Models;
using DS3XMLImporter.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace DS3DXMLImporter.Parsers
{
    public class Instance3DParser
    {
        #region METHODS
        public static Instance3D FromXDocument(XElement instance3DXElement)
        {
            int id = int.Parse(instance3DXElement.Attribute(XName.Get("id")).Value);
            string name = instance3DXElement.Attribute(XName.Get("name")).Value;
            IList<double> relativeMatrix = ParserHelper.ValueOfDescendant(instance3DXElement, "RelativeMatrix", ParseList, new List<double>());
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            ConvertMatrix(relativeMatrix.ToArray(), out position, out rotation);

            Instance3D instance3D = new Instance3D(id, name)
            {
                AggregatedBy = ParserHelper.ValueOfDescendant(instance3DXElement, "IsAggregatedBy", Convert.ToInt32, 0),
                InstanceOf = ParserHelper.ValueOfDescendant(instance3DXElement, "IsInstanceOf", Convert.ToInt32, 0),
                Position = position,
                Rotation = rotation
            };

            instance3D.ElementsData = ParserHelper.ParseElements(instance3DXElement);

            return instance3D;
        }

        private static IList<double> ParseList(string s)
        {
            string[] elements = s.Split();
            List<double> list = elements.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToList();

            return list;
        }
        private static void ConvertMatrix(double[] matrix, out Vector3 position, out Quaternion rotation)
        {
            float m11 = (float)matrix[0];
            float m12 = (float)matrix[1];
            float m13 = (float)matrix[2];
            float m21 = (float)matrix[3];
            float m22 = (float)matrix[4];
            float m23 = (float)matrix[5];
            float m31 = (float)matrix[6];
            float m32 = (float)matrix[7];
            float m33 = (float)matrix[8];
            float px = (float)matrix[9];
            float py = (float)matrix[10];
            float pz = (float)matrix[11];

            position = new Vector3(px, pz, -py);

            Vector3 unityX = new Vector3(m11, m13, -m12);
            Vector3 unityY = new Vector3(m21, m23, -m22);
            Vector3 unityZ = new Vector3(m31, m33, -m32);

            Matrix4x4 rotationMatrix = new Matrix4x4();
            rotationMatrix.SetColumn(0, unityX);
            rotationMatrix.SetColumn(1, unityY);
            rotationMatrix.SetColumn(2, unityZ);
            rotationMatrix.SetColumn(3, new Vector4(0, 0, 0, 1));

            rotation = Quaternion.LookRotation(rotationMatrix.GetColumn(2), rotationMatrix.GetColumn(1));
        }
        #endregion
    }
}
