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

        public IList<double> RelativeMatrix { get; set; }

        public Instance3D(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public Vector3 Position => new Vector3((float)RelativeMatrix[9], (float)RelativeMatrix[10], (float)RelativeMatrix[11]);

        public Matrix4x4 Rotation => new Matrix4x4(
            new Vector4((float)RelativeMatrix[0], (float)RelativeMatrix[1], (float)RelativeMatrix[2], 0f),
            new Vector4((float)RelativeMatrix[3], (float)RelativeMatrix[4], (float)RelativeMatrix[5], 0f),
            new Vector4((float)RelativeMatrix[6], (float)RelativeMatrix[7], (float)RelativeMatrix[8], 0f),
            new Vector4((float)RelativeMatrix[9], (float)RelativeMatrix[10], (float)RelativeMatrix[11], 1f));
        #endregion
    }
}
