using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business.Objects
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Members { get; set; }
        public DateTime CreateDate { get; set; }
        public long Score { get; set; }
        public bool IsAdmin { get; set; }
        public List<AssignmentEnrollment> AssignmentEnrollmentList { get; set; }
        public List<Submit> SubmitList { get; set; }
    }
}
