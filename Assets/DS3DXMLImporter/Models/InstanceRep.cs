namespace DS3XMLImporter.Models
{
    public class InstanceRep
    {
        #region PROPERTIES
        public int ID { get; private set; }

        public string Name { get; private set; }

        public int AggregatedBy { get; set; }

        public int InstanceOf { get; set; }
        #endregion

        #region CONSTRUCTOR
        public InstanceRep(int id, string name)
        {
            ID = id;
            Name = name;
        }
        #endregion
    }
}
