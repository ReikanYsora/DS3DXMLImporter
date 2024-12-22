using DS3DXMLImporter.Exceptions;
using DS3DXMLImporter.Models.Unity;
using DS3XMLImporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DS3DXMLImporter.Models
{
    internal class DS3DXMLManager
    {
        #region PROPERTIES
        public DS3DXMLHeader Header {  get; private set; }

        public IList<ReferenceRep> ReferencesRep { get; set; }

        public IList<Reference3D> References3D { get; set; }

        public IList<InstanceRep> InstancesRep { get; set; }

        public IList<Instance3D> Instances3D { get; set; }
        #endregion

        #region CONSTRUCTOR
        public DS3DXMLManager(DS3DXMLHeader header)
        {
            Header = header;
        }
        #endregion

        #region METHODS
        public T Get<T>(int id)
        {
            Type type = typeof(T);

            if (type == typeof(Reference3D))
            {
                return (T)Convert.ChangeType(References3D.First(x => x.ID == id), type);
            }

            if (type == typeof(Instance3D))
            {
                return (T)Convert.ChangeType(Instances3D.First(x => x.ID == id), type);
            }

            if (type == typeof(ReferenceRep))
            {
                return (T)Convert.ChangeType(ReferencesRep.First(x => x.ID == id), type);
            }

            if (type == typeof(InstanceRep))
            {
                return (T)Convert.ChangeType(InstancesRep.First(x => x.ID == id), type);
            }

            throw new TypeNotFoundException(type.Name);
        }

        private IList<T> Aggregated<T>(int aggregatedBy)
        {
            Type type = typeof(T);

            if (type == typeof(Instance3D))
            {
                IEnumerable<Instance3D> tempInstance3D = Instances3D.Where(x => x.AggregatedBy == aggregatedBy);

                var t = Instances3D.Where(x => x.Name == "prd-ADCO01-00689556.1").ToList();

                return tempInstance3D.Select(x => (T)Convert.ChangeType(x, type)).ToList();
            }
            else if (type == typeof(InstanceRep))
            {
                IEnumerable<InstanceRep> tempInstanceRep = InstancesRep.Where(x => x.AggregatedBy == aggregatedBy);
                return tempInstanceRep.Select(x => (T)Convert.ChangeType(x, type)).ToList();
            }

            throw new TypeNotFoundException(type.Name);
        }

        private IEnumerable<TransformDefinition> Traverse(Reference3D ref3D, Instance3D instance3D)
        {
            IList<Instance3D> instance3Ds = Aggregated<Instance3D>(ref3D.ID);
            IList<InstanceRep> instanceReps = Aggregated<InstanceRep>(ref3D.ID);

            IEnumerable<ReferenceRep> referenceReps = instanceReps.Select(x => Get<ReferenceRep>(x.InstanceOf));
            return referenceReps.Select(x => TransformDefinition.FromReferenceRep(x, instance3D)).Concat(instance3Ds.SelectMany(x => Traverse(Get<Reference3D>(x.InstanceOf), x)));
        }

        public DS3DXMLStructure ToStructure()
        {
            Reference3D product = Get<Reference3D>(1);
            IEnumerable<TransformDefinition> transformParts = Traverse(product, null).ToList();

            // get all direct children of the root element
            DS3DXMLModel model = new DS3DXMLModel
            {
                Name = Header.Name,
                Author = Header.Author,
                TransformDefinitions = transformParts.ToList()
            };

            DS3DXMLStructure structure = new DS3DXMLStructure
            {
                Header = Header,
                InstancesRep = InstancesRep,
                ReferencesRep = ReferencesRep,
                Instances3D = Instances3D,
                References3D = References3D,
                Model = model
            };

            return structure;
        }
        #endregion
    }
}
