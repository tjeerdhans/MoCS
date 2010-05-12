using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Model
{
    public class ViewModelFactory
    {


        internal static AssignmentViewModel CreateAssignment(MoCS.Service.DataContracts.Assignment a)
        {
            AssignmentViewModel avm = new AssignmentViewModel();

            if (a.AssignmentId != null && a.AssignmentId.Length>0)
            {
                avm.AssignmentId = Convert.ToInt32(a.AssignmentId);
            }

            if (a.TournamentId != null && a.TournamentId.Length > 0)
            {
                avm.TournamentId = Convert.ToInt32(a.TournamentId);
            }

            if (a.TournamentAssignmentId != null && a.TournamentAssignmentId.Length > 0)
            {
                avm.TournamentAssignmentId = Convert.ToInt32(a.TournamentAssignmentId);
            }

            if (a.TeamTournamentAssignmentId != null && a.TeamTournamentAssignmentId.Length > 0)
            {
                avm.TeamTournamentAssignmentId = Convert.ToInt32(a.TeamTournamentAssignmentId);
            }
            
            /////////////////////////////////////////////
            avm.Name = a.AssignmentName;
            avm.Hint = a.Hint;
            avm.Difficulty = a.Difficulty;
            avm.Points = a.Points;
            avm.Category = a.Category;
            avm.DisplayName = a.DisplayName;
            avm.Active = a.Active;
            avm.ZipFile = a.Zipfile;
            avm.Files = new Dictionary<string, AssignmentFileViewModel>();
            avm.PointsWon = a.PointsWon;

            avm.Points1 = a.Points1;
            avm.Points2 = a.Points2;
            avm.Points3 = a.Points3;
            avm.AssignmentOrder = a.AssignmentOrder;




            if (a.StartDate.HasValue && a.StartDate.Value!=DateTime.MinValue)
            {
                avm.StartDate = a.StartDate.Value;
                avm.TeamHasStarted = true;
            }
            else
            {
                avm.TeamHasStarted = false;
            }

            avm.SubmitStatusCode = a.SubmitStatusCode;

            if (a.Files != null)
            {
                foreach (string key in a.Files.Keys)
                {
                    AssignmentFileViewModel afvm = new AssignmentFileViewModel();
                    afvm.Name = key;
                    afvm.Contents = a.Files[key].Contents;
                    avm.Files.Add(key, afvm);
                }
            }


            return avm;

        }

        internal static TeamSubmitViewModel CreateTeamSubmit(MoCS.Service.DataContracts.Submit s)
        {
            TeamSubmitViewModel tsvm = new TeamSubmitViewModel();
            tsvm.SubmitId = s.SubmitId;
            tsvm.IsFinished = s.IsFinished;
         //   tsvm.Details = s.Details;
            tsvm.CurrentStatusCode = s.CurrentStatusCode;
            tsvm.StatusDate = s.StatusDate;
            tsvm.SubmitDate = s.SubmitDate;
         //   tsvm.FileContents = s.FileContents;
            return tsvm;
        }

 
        internal static TeamViewModel CreateTeam(MoCS.Client.Business.Entities.Team t)
        {
            TeamViewModel ctvm = new TeamViewModel();
            ctvm.Name = t.TeamName;
            ctvm.Points = t.Points;
            ctvm.TeamMembers = t.TeamMembers;
            return ctvm;
        }


        internal static MonitorSubmitViewModel CreateSubmit(MoCS.Service.DataContracts.Submit s)
        {
            MonitorSubmitViewModel m = new MonitorSubmitViewModel();
            m.CurrentStatusCode = s.CurrentStatusCode;
            m.IsFinished = s.IsFinished;
            m.StatusDate = s.StatusDate;
            m.TeamName = s.TeamName;
            m.SubmitDate = s.SubmitDate;
            m.TeamMembers = s.TeamMembers;

            return m;
        }


        public static string ConvertStatus(int statusCode)
        {
            switch (statusCode)
            {
                case 0:
                    return "Submitted";
                case 1:
                    return "Success";
                case 20:
                    return "Error: Compilation";
                case 21:
                    return "Error: Validation";
                  case 22:
                    return "Error: Testing";
                 case 23:
                    return "Error: Server";
                case 90:
                    return "Error: Unknown";
                  case 99:
                    return "Error: Unknown";
                default:
                    return statusCode.ToString();
  
            }

        }

        internal static TeamViewModel CreateTeam(MoCS.Service.DataContracts.Team team)
        {
            TeamViewModel ctvm = new TeamViewModel();
            ctvm.Name = team.TeamName;
            ctvm.Points = team.Points;
            ctvm.TeamMembers = team.TeamMembers;
            ctvm.Me = (TeamSession.Instance.TeamName.ToUpper() == team.TeamName.ToUpper());
            return ctvm;
        }
    }
}
