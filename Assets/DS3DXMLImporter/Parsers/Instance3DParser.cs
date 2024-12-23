using DS3XMLImporter.Models;
using DS3XMLImporter.Parsers;
using System;
using System.Xml.Linq;
using UnityEngine;

namespace DS3DXMLImporter.Parsers
{
    public class Instance3DParser
    {
        #region METHODS
        public static Instance3D FromXDocument(XElement xElement)
        {
            int id = int.Parse(xElement.Attribute(XName.Get("id")).Value);
            string name = xElement.Attribute(XName.Get("name")).Value;
            string relativeMatrix = ParserHelper.ValueOfDescendant(xElement, "RelativeMatrix", Convert.ToString, "");
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            if (!string.IsNullOrEmpty(relativeMatrix))
            {
                MatrixParser matrixParser = new MatrixParser(relativeMatrix);
                position = matrixParser.Position / 1000f;
                rotation = matrixParser.Rotation;
            }

            Instance3D instance = new Instance3D(id, name)
            {
                AggregatedBy = ParserHelper.ValueOfDescendant(xElement, "IsAggregatedBy", Convert.ToInt32, 0),
                InstanceOf = ParserHelper.ValueOfDescendant(xElement, "IsInstanceOf", Convert.ToInt32, 0),
                Position = position,
                Rotation = rotation
            };

            return instance;
        }
        #endregion
    }
}
