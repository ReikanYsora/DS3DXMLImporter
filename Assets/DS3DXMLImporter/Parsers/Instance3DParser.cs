using DS3XMLImporter.Models;
using DS3XMLImporter.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace DS3DXMLImporter.Parsers
{
    public class Instance3DParser
    {
        #region METHODS
        public static Instance3D Parse(XElement instance3DXElement, float scale)
        {
            int id = int.Parse(instance3DXElement.Attribute(XName.Get("id")).Value);
            string name = instance3DXElement.Attribute(XName.Get("name")).Value;
            IList<float> relativeMatrix = ParserHelper.ValueOfDescendant(instance3DXElement, "RelativeMatrix", ParserHelper.ParseFloatList, new List<float>());
            var tempConverter = MatrixConverter(relativeMatrix.ToList(), scale);

            Instance3D instance3D = new Instance3D(id, name)
            {
                AggregatedBy = ParserHelper.ValueOfDescendant(instance3DXElement, "IsAggregatedBy", Convert.ToInt32, 0),
                InstanceOf = ParserHelper.ValueOfDescendant(instance3DXElement, "IsInstanceOf", Convert.ToInt32, 0),
                Position = tempConverter.position,
                Rotation = tempConverter.rotation
            };

            instance3D.ElementsData = ParserHelper.ParseElements(instance3DXElement);

            return instance3D;
        }

        public static (Vector3 position, Quaternion rotation) MatrixConverter(List<float> relativeMatrix, float scale)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetRow(0, new Vector4(relativeMatrix[0], relativeMatrix[1], relativeMatrix[2], relativeMatrix[9]));
            matrix.SetRow(1, new Vector4(relativeMatrix[3], relativeMatrix[4], relativeMatrix[5], relativeMatrix[10]));
            matrix.SetRow(2, new Vector4(relativeMatrix[6], relativeMatrix[7], relativeMatrix[8], relativeMatrix[11]));
            matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));

            Vector3 position = matrix.GetColumn(3);
            position = new Vector3(position.x, position.z, position.y) / scale;
            Quaternion tempRotation = Quaternion.LookRotation(matrix.GetRow(2), matrix.GetRow(1));
            Quaternion rotation = new Quaternion(tempRotation.x, tempRotation.z, tempRotation.y, -tempRotation.w);

            return (position, rotation);
        }
        #endregion
    }

}
