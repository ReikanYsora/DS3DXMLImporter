using System;

namespace DS3DXMLImporter.Exceptions
{
    public class TypeNotFoundException : Exception
    {
        #region PROPERTIES
        public string Type { get; set; }
        #endregion

        #region CONSTRUCTOR
        public TypeNotFoundException(string type)
        {
            Type = type;
        }
        #endregion
    }
}
