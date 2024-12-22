using System.IO;

namespace DS3DXMLImporter.Loaders
{
    public interface ILoader
    {
        Stream Load();

        void Close();
    }
}
