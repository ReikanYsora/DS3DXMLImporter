using DS3DXMLImporter.Models.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace DS3XMLImporter.Models
{
    public class Instance3D
    {
        #region PROPERTIES
        public int ID { get; private set; }

        public string Name { get; private set; }

        public int AggregatedBy { get; set; }

        public int InstanceOf { get; set; }

        public List<ElementNodeData> ElementsData { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public double[] RelativeMatrix { get; set; }
        #endregion

        #region CONSTRUCTOR
        public Instance3D(int id, string name)
        {
            ID = id;
            Name = name;
            ElementsData = new List<ElementNodeData>();
        }
        #endregion
    }
}
