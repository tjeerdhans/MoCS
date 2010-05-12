using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business.Objects
{
    public class TournamentAssignment
    {
        public int Id { get; set; }
        public int AssignmentOrder { get; set; }
        public int Points1 { get; set; }
        public int Points2 { get; set; }
        public int Points3 { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public Tournament Tournament { get; set; }
        public Assignment Assignment { get; set; }
        public List<Submit> SubmitList { get; set; }
        public List<AssignmentEnrollment> AssignmentEnrollmentList { get; set; }
    }
}
