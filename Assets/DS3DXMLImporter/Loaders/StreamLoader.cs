using System.IO;

namespace DS3DXMLImporter.Loaders
{
    public class StreamLoader : ILoader
    {
        #region ATTRIBUTES
        private Stream _stream;
        #endregion

        #region CONSTRUCTOR
        public StreamLoader(Stream stream)
        {
            _stream = stream;
        }
        #endregion

        #region METHODS
        public Stream Load()
        {
            return _stream;
        }

        public void Close()
        {
            _stream.Close();
        }
        #endregion
    }
}
