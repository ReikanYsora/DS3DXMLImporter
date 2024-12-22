using System.Collections.Generic;
using System.Xml.Linq;

namespace DS3XMLImporter.Models.Interfaces
{
    public interface IDS3DXMLArchive
    {
        XDocument GetManifest();

        XDocument GetNextDocument(string name);

        IList<string> ContainedFiles { get; }
    }
}
