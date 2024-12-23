using DS3DXMLImporter.Loaders;
using DS3DXMLImporter.Models.Unity;
using DS3DXMLImporter.Parsers;
using DS3XMLImporter.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Loader : MonoBehaviour
{
    //private const string PATH_TEST = "C:\\Users\\jcrem\\Desktop\\3DXML\\V6\\XMLtess_prd-ADCO01-02446340_00_A.1_BATTERY_A6_BATTERY_DMU_BASELINE_Not_mature_In Work_BsF_(1).3dxml";
    private const string PATH_TEST_V5 = "C:\\Users\\jcrem\\Desktop\\3DXML\\V5\\XMLtess_prd-ADCO01-02446340_00_A.1_BATTERY_A6_BATTERY_DMU_BASELINE_Not_mature_In Work_BsF_v5v2.3dxml";
    public Material VertexColor;
    private Dictionary<string, Mesh> _meshes = new Dictionary<string, Mesh>();

    private void Start()
    {
        DS3DXMLParser parser = new DS3DXMLParser();
        parser.OnParseCompleted += OnParseCompleted;

        ILoader loader = LoaderFactory.CreateFileLoader(PATH_TEST_V5);
        parser.ParseStructure(loader);
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

        if (element.TransformDefinition != null)
        {
            tempObject.transform.position = element.TransformDefinition.Position;
            tempObject.transform.rotation = element.TransformDefinition.Rotation;

            if (element.TransformDefinition.Vertices != null && element.TransformDefinition.Vertices.Count > 0)
            {
                MeshFilter filter = tempObject.AddComponent<MeshFilter>();
                MeshRenderer renderer = tempObject.AddComponent<MeshRenderer>();
                filter.sharedMesh = CreateMesh(element.TransformDefinition);
                renderer.material = VertexColor;
            }
        }
        else
        {
            tempObject.transform.position = Vector3.zero;
            tempObject.transform.rotation = Quaternion.identity;
        }

        tempObject.transform.SetParent(parent);

        foreach (ProductStructureElement child in element.Children)
        {
            CreateElement(child, tempObject.transform);
        }
    }

    private Mesh CreateMesh(TransformDefinition element)
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
