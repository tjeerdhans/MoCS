using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    [DataContract(Namespace="")]
    public class AssignmentZipFileContract
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public byte[] Data { get; set; }
    }
}
