using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business.Objects
{
    public class AssignmentEnrollment
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public Team Team { get; set; }
        public List<Submit> SubmitList { get; set; }
        public bool IsActive { get; set; }
        public TournamentAssignment TournamentAssignment { get; set; }
    }
}
