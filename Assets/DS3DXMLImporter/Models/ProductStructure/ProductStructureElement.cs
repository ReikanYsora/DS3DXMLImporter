using DS3DXMLImporter.Models.Unity;
using System.Collections.Generic;

public class ProductStructureElement
{
    #region PROPERTIES
    public int ID { get; internal set; }

    public string Name { get; internal set; }

    public TransformDefinition TransformDefinition { get; internal set; }

    public List<ProductStructureElement> Children { get; internal set; }
    #endregion

    #region CONSTRUCTOR 
    public ProductStructureElement(int id, string name)
    {
        ID = id;
        Name = name;
        Children = new List<ProductStructureElement>();
    }
    #endregion
}
