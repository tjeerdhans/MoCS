using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    [DataContract(Namespace = "")]
    public class SubmitContract
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int AssignmentId { get; set; }
        [DataMember]
        public int AssignmentEnrollmentId { get; set; }
        [DataMember]
        public int TeamId { get; set; }
        [DataMember]
        public int TournamentAssignmentId { get; set; }

        [DataMember]
        public byte[] Data { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string FileContents { get; set; }
        [DataMember]
        public DateTime SubmitDate { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public bool IsProcessed { get; set; }
        [DataMember]
        public string ProcessingDetails { get; set; }
        [DataMember]
        public long SecondsSinceEnrollment { get; set; }

        //Team properties
        [DataMember]
        public string TeamName { get; set; }
        [DataMember]
        public string TeamMembers { get; set; }

        //Assignment properties
        [DataMember]
        public string AssignmentName { get; set; }

        //[DataMember]
        //public DateTime StatusDate { get; set; }

        //[DataMember]
        //public DateTime StartDate { get; set; }



    }

    [CollectionDataContract(Name = "SubmitsContract", Namespace = "")]
    public class SubmitsContract : List<SubmitContract> { }
}
