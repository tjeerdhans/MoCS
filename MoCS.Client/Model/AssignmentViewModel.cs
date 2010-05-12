using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Model
{
    public class AssignmentViewModel
    {
        public int AssignmentId { get; set; }
        public int TournamentId { get; set; }
        public int TournamentAssignmentId { get; set; }
        public int TeamTournamentAssignmentId { get; set; }

        public string Hint { get; set; }
        public string Name { get; set; }
        public string Difficulty { get;set;}
        public string Category {get;set;}
        public string Author {get;set;}
        public int Points {get;set;}
        public string DisplayName { get; set; }
        public bool Active { get; set; }
        public byte[] ZipFile { get; set; }

        public DateTime? StartDate {get;set;}
        public bool TeamHasStarted { get; set; }

        public int PointsWon { get; set; }
        public int SubmitStatusCode {get;set;}

        public int Points1 {get;set;}
        public int Points2 {get;set;}
        public int Points3 {get;set;}
        public int AssignmentOrder {get;set;}
 
        public Dictionary<string, AssignmentFileViewModel> Files;

        public AssignmentViewModel()
        {
            Files = new Dictionary<string, AssignmentFileViewModel>();
        }

        public string StatusDescription
        {
            get
            {
                if (!Active)
                {
                    return "CLOSED";
                }

                if (!TeamHasStarted)
                {
                    return "NOT STARTED";
                }
                else
                {
                    if (TeamHasStarted && SubmitStatusCode == 1)
                    {
                        return "FINISHED";
                    }
                    else
                    {
                        return "IN PROGRESS";
                    }

                }
            }
        }
    }
}
