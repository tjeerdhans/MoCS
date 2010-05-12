using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    [DataContract(Namespace = "")]
    public class AssignmentFileContract
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public byte[] Data { get; set; }
    }

    [CollectionDataContract(Namespace = "", Name = "AssignmentFilesContract")]
    public class AssignmentFilesContract : List<AssignmentFileContract> { }
}
