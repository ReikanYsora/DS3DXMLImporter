using DS3DXMLImporter.Loaders;
using DS3DXMLImporter.Models.Unity;
using DS3DXMLImporter.Parsers;
using DS3XMLImporter.Models;
using System.Collections.Generic;
using System.Linq;

namespace DS3DXMLImporter
{
    public class DS3DXMLV6Importer
    {
        #region ATTRIBUTES
        private Dictionary<int, Reference3D> _references3D = new Dictionary<int, Reference3D>();
        private Dictionary<int, Instance3D> _instances3D = new Dictionary<int, Instance3D>();
        private Dictionary<int, InstanceRep> _instanceReps = new Dictionary<int, InstanceRep>();
        private Dictionary<int, ReferenceRep> _referencesReps = new Dictionary<int, ReferenceRep>();
        public ProductStructureElement Root;
        public DS3DXMLHeader Header;
        private IList<TransformDefinition> TransformDefinitions;
        public int ProductStructureCount = 0;
        #endregion

        #region CONSTRUCTOR
        public DS3DXMLV6Importer(string path)
        {
            DS3DXMLParser parser = (DS3DXMLParser)ParserFactory.Create();
            ILoader loader = LoaderFactory.CreateFileLoader(path);
            DS3DXMLStructure structure = parser.ParseStructure(loader);

            _references3D = structure.References3D.ToDictionary(x => x.ID, y => y);
            _instances3D = structure.Instances3D.ToDictionary(x => x.ID, y => y);
            _instanceReps = structure.InstancesRep.ToDictionary(x => x.ID, y => y);
            _referencesReps = structure.ReferencesRep.ToDictionary(x => x.ID, y => y);
            Header = structure.Header;
            TransformDefinitions = structure.Model.TransformDefinitions;

            Reference3D ref3DTopParent = structure.References3D.OrderBy(x => x.ID).FirstOrDefault();
            Root = new ProductStructureElement(ref3DTopParent.ID, ref3DTopParent.Name);
            ProductStructureCount++;
            Root.Children = CreateTreeStructure(Root);
        }
        #endregion

        #region METHODS
        private List<ProductStructureElement> CreateTreeStructure(ProductStructureElement part)
        {
            List<ProductStructureElement> children = new List<ProductStructureElement>();
            List<Instance3D> children3DInstances = _instances3D.Where(x => x.Value.AggregatedBy == part.ID).Select(x => x.Value).ToList();

            foreach (Instance3D child3DInstance in children3DInstances)
            {
                if (_references3D.ContainsKey(child3DInstance.InstanceOf))
                {
                    Reference3D tempRef3D = _references3D[child3DInstance.InstanceOf];
                    List<int> children3DIds = _instanceReps.Where(x => x.Value.AggregatedBy == child3DInstance.InstanceOf).Select(x => x.Value.InstanceOf).ToList();
                    ProductStructureElement childPart = new ProductStructureElement(tempRef3D.ID, tempRef3D.Name);

                    foreach (int child3DId in children3DIds)
                    {
                        if (_referencesReps.ContainsKey(child3DId))
                        {
                            ReferenceRep tempReferenceRep = _referencesReps[child3DId];

                            childPart.TransformDefinition = TransformDefinitions.Where(x => tempReferenceRep.AssociatedFile.Contains(x.Name)).FirstOrDefault();
                            break;
                        }
                    }

                    childPart.Children = CreateTreeStructure(childPart);
                    children.Add(childPart);
                }
            }

            ProductStructureCount++;

            return children;
        }
        #endregion
    }
}