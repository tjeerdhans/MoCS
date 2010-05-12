using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    [DataContract(Namespace = "")]
    public class Assignment
    {
        [DataMember]
        public string AssignmentId { get; set; }
        [DataMember]
        public string AssignmentName { get; set; }
        [DataMember]
        public string Hint {get;set;}
        [DataMember]
        public int Points {get;set;}
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string Difficulty {get;set;}
        [DataMember]
        public string DisplayName {get;set;}
        [DataMember]
        public string Category {get;set;}
        [DataMember]
        public Dictionary<string, AssignmentFile> Files {get;set;}
        [DataMember]
        public byte[] Zipfile { get; set; }
        [DataMember]
        public DateTime? StartDate {get;set;}
        [DataMember]
        public int SubmitStatusCode {get;set;}
        [DataMember]
        public int PointsWon {get;set;}

        [DataMember]
        public string TournamentId{get;set;}
        
        [DataMember]
        public string TournamentAssignmentId{get;set;}

        [DataMember]
        public string TeamTournamentAssignmentId{get;set;}

        [DataMember]
        public string TeamId { get; set; }

        [DataMember]
        public int AssignmentOrder {get;set;}

        [DataMember]
        public int Points1 {get;set;}

        [DataMember]
        public int Points2 {get;set;}

        [DataMember]
        public int Points3 {get;set;}

 
    }

    [DataContract(Namespace = "")]
    public class AssignmentFile
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public byte[] Contents { get; set; }
    }



    [CollectionDataContract(Name = "Assignments", Namespace = "")]
    public class Assignments : List<Assignment> { }
}
