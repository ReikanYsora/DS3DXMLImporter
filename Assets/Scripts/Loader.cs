using DS3DXMLImporter;
using DS3DXMLImporter.Models;
using DS3DXMLImporter.Models.Unity;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Loader : MonoBehaviour
{
    private const string PATH_TEST = "C:\\Users\\jcrem\\Desktop\\3DXML\\V6\\XMLtess_prd-ADCO01-02446340_00_A.1_BATTERY_A6_BATTERY_DMU_BASELINE_Not_mature_In Work_BsF_(1).3dxml";
    public Material VertexColor;

    private void Start()
    {
        DS3DXMLV6Importer manager = new DS3DXMLV6Importer(PATH_TEST);

        GameObject parent = new GameObject("Import");
        CreateElement(manager.Root, parent.transform);
    }

    private void CreateElement(ProductStructureElement element, Transform parent)
    {
        GameObject tempObject = new GameObject(element.Name);
        tempObject.transform.SetParent(parent);

        if (element.TransformDefinition != null)
        {
            tempObject.transform.position = element.TransformDefinition.Position;
            tempObject.transform.rotation = element.TransformDefinition.Rotation.rotation;

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

        foreach(ProductStructureElement child in element.Children)
        {
            CreateElement(child, tempObject.transform);
        }
    }

    private Mesh CreateMesh(TransformDefinition element)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = element.Vertices.ToArray();
        mesh.normals = element.Normals.ToArray();
        mesh.colors = element.Colors.ToArray();
        mesh.triangles = element.Triangles.ToArray();

        return mesh;
    }
}
