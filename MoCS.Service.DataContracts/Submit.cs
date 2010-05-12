using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    [DataContract(Namespace = "")]
    public class Submit
    {
        [DataMember]
        public string AssignmentId { get; set; }
        [DataMember]
        public string SubmitId { get; set; }
        [DataMember]
        public string TeamId { get; set; }
        [DataMember]
        public string TeamName { get; set; }
        [DataMember]
        public string TeamMembers { get; set; }
        [DataMember]
        public string TeamTournamentAssignmentId { get; set; }
        [DataMember]    
        public byte[] Payload { get; set; } 
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public DateTime SubmitDate { get; set; }
        [DataMember]
        public int CurrentStatusCode {get;set;}
        [DataMember]
        public DateTime StatusDate { get; set; }
        [DataMember]
        public bool IsFinished {get;set;}
        [DataMember]
        public string Details { get; set; }
        [DataMember]
        public DateTime StartDate {get;set;}
        [DataMember]
        public string FileContents { get; set; }
        [DataMember]
        public int TournamentAssignmentId { get; set; }

        public double DeltaTime
        {
            get { return SubmitDate.Subtract(StartDate).TotalMilliseconds; }
        }

    }

    [CollectionDataContract(Name = "Submits", Namespace = "")]
    public class Submits : List<Submit> { }
}
