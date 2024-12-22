using DS3DXMLImporter.Models;
using System.Collections.Generic;

namespace DS3XMLImporter.Models
{
    public class DS3DXMLStructure
    {
        #region PROPERTIES
        public DS3DXMLHeader Header { get; set; }

        public IList<Reference3D> References3D { get; set; }

        public IList<Instance3D> Instances3D { get; set; }

        public IList<InstanceRep> InstancesRep { get; set; }

        public IList<ReferenceRep> ReferencesRep { get; set; }

        public DS3DXMLModel Model { get; set; }
        #endregion
    }
}
