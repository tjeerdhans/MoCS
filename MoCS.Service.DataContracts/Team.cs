using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    public enum TeamStatus
    {
        Active,
        Closed
    }

    public enum TeamType
    {
        Normal,
        Administrator
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class Team
    {
        [DataMember]
        public string TeamId { get; set; }
        [DataMember]
        public string TeamName { get; set; }
        //[DataMember]
        //public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public TeamStatus TeamStatus { get; set; }
        [DataMember]
        public TeamType TeamType { get; set; }
        [DataMember]
        public int Points { get; set; }
        [DataMember]
        public string TeamMembers { get; set; }
    }

    [Serializable]
    [CollectionDataContract(Name = "Teams", Namespace = "")]
    public class Teams : List<Team> { }
}
