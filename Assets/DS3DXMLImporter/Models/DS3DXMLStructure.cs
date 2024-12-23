using DS3DXMLImporter.Models.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Metadata;

namespace DS3XMLImporter.Models
{
    public class DS3DXMLStructure
    {
        #region PROPERTIES
        public DS3DXMLHeader Header { get; set; }

        public Dictionary<string, MeshDefinition> MeshDefinitions { get; private set; }

        public ProductStructureElement Root { get; private set; }

        public Dictionary<int, ReferenceRep> ReferencesRep { get; private set; }

        public Dictionary<int, InstanceRep> InstancesRep { get; private set; }

        public Dictionary<int, Reference3D> References3D { get; private set; }

        public Dictionary<int, Instance3D> Instances3D { get; private set; }
        #endregion

        #region CONSTRUCTOR
        public DS3DXMLStructure(DS3DXMLHeader header, Dictionary<int, ReferenceRep> referencesRep, Dictionary<int, InstanceRep> instancesRep, Dictionary<int, Reference3D> references3D, Dictionary<int, Instance3D> instances3D, Dictionary<string, MeshDefinition> meshDefinitions)
        {
            Header = header;    
            ReferencesRep = referencesRep;
            InstancesRep = instancesRep;
            References3D = references3D;
            Instances3D = instances3D;
            MeshDefinitions = meshDefinitions;

            Reference3D ref3DTopParent = References3D.Values.OrderBy(x => x.ID).FirstOrDefault();
            Root = new ProductStructureElement();
            Root.ID = ref3DTopParent.ID;

            if (instances3D.ContainsKey(ref3DTopParent.ID))
            {
                Root.Position = instances3D[ref3DTopParent.ID].Position;
                Root.Rotation = instances3D[ref3DTopParent.ID].Rotation;
                Root.Name = instances3D[ref3DTopParent.ID].Name;
            }
            else
            {
                Root.Position = Vector3.zero;
                Root.Rotation = Quaternion.identity;
                Root.Name = ref3DTopParent.Name;
            }

            Root.Children = CreateTreeStructure(Root);
        }
        #endregion

        #region METHODS
        private List<ProductStructureElement> CreateTreeStructure(ProductStructureElement part)
        {
            List<ProductStructureElement> children = new List<ProductStructureElement>();
            List<Instance3D> children3DInstances = Instances3D.Where(x => x.Value.AggregatedBy == part.ID).Select(x => x.Value).ToList();

            foreach (Instance3D child3DInstance in children3DInstances)
            {
                if (References3D.ContainsKey(child3DInstance.InstanceOf))
                {
                    Reference3D tempRef3D = References3D[child3DInstance.InstanceOf];
                    List<int> children3DIds = InstancesRep.Where(x => x.Value.AggregatedBy == child3DInstance.InstanceOf).Select(x => x.Value.InstanceOf).ToList();
                    ProductStructureElement childPart = new ProductStructureElement
                    {
                        ID = tempRef3D.ID,
                        Name = child3DInstance.Name,
                        Position = child3DInstance.Position,
                        Rotation = child3DInstance.Rotation
                    };

                    foreach (int child3DId in children3DIds)
                    {
                        if (ReferencesRep.ContainsKey(child3DId))
                        {
                            ReferenceRep tempReferenceRep = ReferencesRep[child3DId];
                            childPart.MeshDefinition = MeshDefinitions.Where(x => x.Value.AssociatedFile == tempReferenceRep.AssociatedFile).Select(x => x.Value).FirstOrDefault();
                            break;
                        }
                    }

                    childPart.Children = CreateTreeStructure(childPart);
                    children.Add(childPart);
                }
            }

            return children;
        }
        #endregion
    }
}
