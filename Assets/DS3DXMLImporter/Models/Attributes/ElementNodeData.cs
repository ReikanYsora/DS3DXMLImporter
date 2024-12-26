using System;
using System.Collections.Generic;

namespace DS3DXMLImporter.Models.Attributes
{
    public struct ElementNodeData
    {
        #region PROPERTIES
        public string Name { get; set; }

        public ElementType ElementType { get; set; }

        public ElementValueType ValueType { get; set; }

        public IComparable Value { get; set; }

        public List<ElementNodeData> Children { get; set; }
        #endregion

        #region CONSTRUCTOR
        public ElementNodeData(string name, ElementType elementType, ElementValueType valueType, IComparable value)
        {
            Name = name;
            ElementType = elementType;
            ValueType = valueType;
            Value = value;
            Children = new List<ElementNodeData>();
        }
        #endregion
    }

    public enum ElementType
    {
        Attribute,
        Element,
        None
    }

    public enum ElementValueType
    {
        String,
        Integer,
        Double,
        Boolean,
        DateTime,
        None
    }
}