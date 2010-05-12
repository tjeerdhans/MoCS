using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoCS.WebClient.Models;
using MoCS.WebClient.ServiceProxy;
using MoCS.Business.Objects;
using System.Text;

namespace MoCS.WebClient.Controllers
{
    public class CurrentAssignmentController : Controller
    {
        private Credentials credentials;
        //
        // GET: /CurrentAssignment/

        public ActionResult Index()
        {
            credentials = new Credentials()
            {
                Username = User.Identity.Name,
                Password = (string)Session["password"]
            };
            int teamId = Session["teamId"] != null ? (int)Session["teamId"] : -1;
            int tournamentId = Session["tournamentId"] != null ? (int)Session["tournamentId"] : -1;
            int tournamentAssignmentId = Session["tournamentAssignmentId"] != null ? (int)Session["tournamentAssignmentId"] : -1;
            TournamentAssignment ta = new TournamentAssignment();

            if (tournamentId != -1 && tournamentAssignmentId != -1)
            {
                ta = MoCSServiceProxy.Instance.GetTournamentAssignment(tournamentAssignmentId, credentials);
            }
            else
            {
                return RedirectToAction("Index", "Assignments");
            }

            // Find enrollment
            MoCSServiceProxy.Instance.GetAssignmentEnrollmentsForTeamForTournament(teamId, tournamentId, credentials);


            // Construct the model
            CurrentAssignmentModel caModel = new CurrentAssignmentModel();

            caModel.AssignmentName = ta.Assignment.FriendlyName;
            caModel.AssignmentTagline = ta.Assignment.Tagline;
            caModel.AssignmentCategory = ta.Assignment.Category;
            caModel.AssignmentDifficulty = ta.Assignment.Difficulty;
            caModel.AssignmentEnrollmentTime = DateTime.Now;

            caModel.DownloadURL = "http://google.com";


            // Get the AssignmentFiles and Submits if the team has enrolled for this assignment

            if (false)
            {


                caModel.TabContentModelList = new List<TabContentModel>();
                foreach (var tc in ta.Assignment.AssignmentFiles)
                {
                    caModel.TabContentModelList.Add(new TabContentModel()
                    {
                        Name = tc.Name,
                        ContentType = "plaintext",
                        Content = UTF8Encoding.UTF8.GetString(tc.Data)
                    });
                }

                caModel.SubmitModelList = new List<SubmitModel>();
                foreach (var submit in ta.SubmitList)
                {
                    caModel.SubmitModelList.Add(new SubmitModel()
                    {
                        FileURL = "",
                        Result = submit.Status,
                        ResultDetailsURL = "",
                        SubmitDate = submit.SubmitDate
                    });
                }
            }

            return View(caModel);
        }

    }
}
