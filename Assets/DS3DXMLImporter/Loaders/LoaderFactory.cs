using Microsoft.Win32.SafeHandles;
using System.IO;

namespace DS3DXMLImporter.Loaders
{
    public static class LoaderFactory
    {
        #region METHODS
        public static ILoader CreateFileLoader(string fileName)
        {
            return new FileLoader(new FileStream(fileName, FileMode.Open));
        }

        public static ILoader CreateFileLoader(SafeFileHandle handle)
        {
            return new FileLoader(new FileStream(handle, FileAccess.Read));
        }

        public static ILoader CreateStreamLoader(Stream stream)
        {
            return new StreamLoader(stream);
        }
        #endregion
    }
}
