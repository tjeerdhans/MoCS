using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business
{
    public class AssignmentEnrollment : TournamentAssignment
    {
        public int? TeamTournamentAssignmentId{get;set;}
        public int TeamId {get;set;}
        public DateTime? StartDate{get;set;}
    }
}