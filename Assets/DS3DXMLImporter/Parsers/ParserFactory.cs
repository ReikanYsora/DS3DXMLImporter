using DS3DXMLImporter.Parsers.Interfaces;

namespace DS3DXMLImporter.Parsers
{
    public static class ParserFactory
    {
        public static IParser Create()
        {
            return new DS3DXMLParser();
        }
    }
}
