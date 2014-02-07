using MoCS.Business.Facade;
using MoCS.Business.Objects;
using MoCS.WebClient.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MoCS.WebClient.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to the C# Assignment Framework";
            //Get the enrollments for the active user
            Team team = SessionUtil.GetTeamFromFormsAuthentication();

            if (team != null)
            {
                List<AssignmentEnrollment> enrollmentList = ClientFacade.Instance.GetAssignmentEnrollmentsForTeam(team.Id);

                team = ClientFacade.Instance.GetTeamById(team.Id);

                // Set up the model
                var hm = new HomeModel
                {
                    TeamName = team.Name,
                    MembersModel = new MembersModel {Members = team.Members},
                    EnrollmentList = new List<EnrollmentModel>()
                };

                foreach (AssignmentEnrollment ae in enrollmentList)
                {
                    hm.EnrollmentList.Add(new EnrollmentModel
                    {
                        AssignmentEnrollmentId = ae.Id,
                        AssignmentName = ae.TournamentAssignment.Assignment.Name,
                        TournamentName = ae.TournamentAssignment.Tournament.Name
                    });
                }

                return View(hm);
            }
            return View();
        }

        [Authorize]
        public ActionResult SelectEnrollment(int assignmentEnrollmentId)
        {
            Team team = SessionUtil.GetTeamFromFormsAuthentication();

            AssignmentEnrollment ae = ClientFacade.Instance.GetAssignmentEnrollmentById(assignmentEnrollmentId);

            if (ae != null && ae.Team.Id == team.Id)
            {
                // Set session
                SessionUtil.SetSession(ae.TournamentAssignment.Tournament, ae.TournamentAssignment, ae.TournamentAssignment.Assignment, ae);
            }
            else
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "CurrentAssignment");
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateTeamMembers(HomeModel model)
        {
            Team team = SessionUtil.GetTeamFromFormsAuthentication();

            team = ClientFacade.Instance.GetTeamById(team.Id);

            team.Members = model.MembersModel.Members;

            ClientFacade.Instance.UpdateTeam(team);

            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult Manual()
        {
            return View();
        }
    }
}
