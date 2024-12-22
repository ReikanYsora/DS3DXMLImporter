using System.IO;

namespace DS3DXMLImporter.Loaders
{
    public class FileLoader : ILoader
    {
        #region ATTRIBUTES
        private FileStream _fileStream;
        #endregion

        #region CONSTRUCTOR
        public FileLoader(FileStream stream)
        {
            _fileStream = stream;
        }
        #endregion

        #region METHODS
        public Stream Load()
        {
            return _fileStream;
        }

        public void Close()
        {
            _fileStream.Close();
        }
        #endregion
    }
}
