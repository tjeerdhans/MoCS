using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    [DataContract(Namespace = "")]
    public class TeamContract
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public bool IsAdmin { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public string Members { get; set; }
        [DataMember]
        public DateTime CreateDate { get; set; }
    }

    [Serializable]
    [CollectionDataContract(Name = "TeamsContract", Namespace = "")]
    public class TeamsContract : List<TeamContract> { }
}
