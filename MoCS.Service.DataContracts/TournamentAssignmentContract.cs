using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    [DataContract(Namespace = "")]
    public class TournamentAssignmentContract
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int TournamentId { get; set; }
        [DataMember]
        public int AssignmentId { get; set; }
        [DataMember]
        public int AssignmentOrder { get; set; }
        [DataMember]
        public int Points1 { get; set; }
        [DataMember]
        public int Points2 { get; set; }
        [DataMember]
        public int Points3 { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public DateTime CreateDate { get; set; }

        #region Assignment-specific
        [DataMember]
        public string AssignmentName { get; set; }
        [DataMember]
        public string Tagline { get; set; }
        [DataMember]
        public string FriendlyName { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public int Version { get; set; }
        [DataMember]
        public int Difficulty { get; set; }
        [DataMember]
        public string Category { get; set; }
        //[DataMember]
        //public Dictionary<string, AssignmentFileContract> Files { get; set; }
        //[DataMember]
        //public byte[] Zipfile { get; set; }

        #endregion


    }

    [CollectionDataContract(Name = "TournamentAssignmentsContract", Namespace = "")]
    public class TournamentAssignmentsContract : List<TournamentAssignmentContract> { }
}
