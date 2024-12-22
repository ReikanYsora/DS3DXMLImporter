using DS3DXMLImporter.Loaders;
using DS3XMLImporter.Models;
using System.IO;

namespace DS3DXMLImporter.Parsers.Interfaces
{
    public interface IParser
    {
        DS3DXMLStructure ParseStructure(Stream stream);

        DS3DXMLStructure ParseStructure(ILoader loader);
    }
}
