using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Business.Entities
{
    public class TournamentAssignment : Assignment
    {   
        public int TournamentAssignmentId {get;set;}
        public int TournamentId {get;set;}
        public int AssignmentOrder{get;set;}
        public int Points1{get;set;}
        public int Points2 { get; set; }
        public int Points3{get;set;}
    }
}
