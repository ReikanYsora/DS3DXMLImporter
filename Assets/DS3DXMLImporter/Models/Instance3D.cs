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

        public Instance3D(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
        #endregion
    }
}
