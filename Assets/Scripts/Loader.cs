using DS3DXMLImporter.Loaders;
using DS3DXMLImporter.Models.Attributes;
using DS3DXMLImporter.Models.Unity;
using DS3DXMLImporter.Parsers;
using DS3XMLImporter.Models;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Loader : MonoBehaviour
{
    private const string PATH_TEST_V6 = "C:\\Users\\jcrem\\Desktop\\3DXML\\V6\\XMLtess_prd-ADCO01-02446340_00_A.1_BATTERY_A6_BATTERY_DMU_BASELINE_Not_mature_In Work_BsF_(1).3dxml";
    private const string PATH_TEST_V5 = "C:\\Users\\jcrem\\Desktop\\3DXML\\V5\\XMLtess_prd-ADCO01-02446340_00_A.1_BATTERY_A6_BATTERY_DMU_BASELINE_Not_mature_In Work_BsF_v5v2.3dxml";
    public Material VertexColor;

    private void Start()
    {
        //DS3DXMLParser parserv5 = new DS3DXMLParser();
        //parserv5.OnParseCompleted += OnParseCompleted;

        DS3DXMLParser parserv6 = new DS3DXMLParser();
        parserv6.OnParseCompleted += OnParseCompleted;


        //ILoader loaderv5 = LoaderFactory.CreateFileLoader(PATH_TEST_V5);
        //parserv5.ParseStructure(loaderv5);

        ILoader loaderv6 = LoaderFactory.CreateFileLoader(PATH_TEST_V6);
        parserv6.ParseStructure(loaderv6);
    }

    private void OnParseCompleted(DS3DXMLStructure structure)
    {
        Dispatcher.RunOnMainThread(() =>
        {
            GameObject parent = new GameObject("Import");
            CreateElement(structure.Root, parent.transform);
        });
    }

    private void CreateElement(ProductStructureElement element, Transform parent)
    {
        GameObject tempObject = new GameObject(element.Name);
        tempObject.transform.SetParent(parent);
        tempObject.transform.position = element.Position;
        tempObject.transform.rotation = element.Rotation;

        if (element.MeshDefinition != null && element.MeshDefinition.Vertices != null && element.MeshDefinition.Vertices.Count > 0)
        {
            MeshFilter filter = tempObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = tempObject.AddComponent<MeshRenderer>();
            filter.sharedMesh = CreateMesh(element.MeshDefinition);
            renderer.material = VertexColor;
        }

        foreach (ProductStructureElement child in element.Children)
        {
            CreateElement(child, tempObject.transform);
        }
    }

    private Mesh CreateMesh(MeshDefinition element)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = element.Vertices.Count > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16;
        mesh.vertices = element.Vertices.ToArray();
        mesh.normals = element.Normals.ToArray();
        mesh.colors = element.Colors.ToArray();
        mesh.triangles = element.Triangles.ToArray();

        return mesh;
    }
}
