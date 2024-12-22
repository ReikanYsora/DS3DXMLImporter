using System;

namespace DS3DXMLImporter.Exceptions
{
    public class UnsupportedFormatException : Exception
    {
        #region CONSTRUCTOR
        public UnsupportedFormatException(string msg) : base(msg) { }
        #endregion
    }
}
