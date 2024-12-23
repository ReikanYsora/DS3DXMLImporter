using DS3DXMLImporter.Models.Unity;
using System.Collections.Generic;
using UnityEngine;
public class ProductStructureElement
{
    #region PROPERTIES
    public int ID { get; internal set; }

    public string Name { get; internal set; }

    public MeshDefinition MeshDefinition { get; internal set; }

    public Vector3 Position { get; internal set; }

    public Quaternion Rotation { get; internal set; }

    public List<ProductStructureElement> Children { get; internal set; }
    #endregion

    #region CONSTRUCTOR 
    public ProductStructureElement()
    {
        Children = new List<ProductStructureElement>();
    }
    #endregion
}
