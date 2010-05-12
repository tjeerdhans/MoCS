using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Business.Entities
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public string Hint { get; set; }
        public string AssignmentName { get; set; }
        public string Difficulty { get; set; }
        public int Points { get;set; }
        public Dictionary<string, AssignmentFile> Files;
        public string DisplayName { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public bool Active { get; set; }
        public bool IsValid {get;set;}
        //property only used by TeamAssignments
        //public DateTime StartDate { get; set; }
        public byte[] ZipFile { get; set; }

    //    public bool TeamHasStarted{get;set;}
     //   public int SubmitStatusCode { get; set; }

    //    public int PointsWon { get; set; } 

        public Assignment()
        {
            Files = new Dictionary<string, AssignmentFile>();
        }
    }
}
