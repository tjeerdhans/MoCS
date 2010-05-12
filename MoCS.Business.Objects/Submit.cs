using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business.Objects
{
    public class Submit
    {
        public int Id { get; set; }
        public DateTime SubmitDate { get; set; }
        public AssignmentEnrollment AssignmentEnrollment { get; set; }
        public TournamentAssignment TournamentAssignment { get; set; }
        public Team Team { get; set; }
        public string Status { get; set; }
        public byte[] Data { get; set; }
        public long SecondsSinceEnrollment { get; set; }
        public bool IsProcessed { get; set; }
        public string ProcessingDetails { get; set; }
        public string FileName { get; set; }
        public string FileContents { get; set; }
    }
}
