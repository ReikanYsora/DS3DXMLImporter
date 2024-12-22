namespace DS3XMLImporter.Models
{
    public class Reference3D
    {
        #region PROPERTIES
        public int ID { get; set; }

        public string Name { get; set; }
        #endregion

        #region CONSTRUCTOR
        public Reference3D(int id, string name)
        {
            ID = id;
            Name = name;
        }
        #endregion
    }
}
