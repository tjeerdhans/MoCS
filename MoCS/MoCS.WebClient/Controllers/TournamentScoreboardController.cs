using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoCS.WebClient.Models;
using MoCS.Business.Objects;
using System.Web.Security;
using MoCS.Business.Facade;

namespace MoCS.WebClient.Controllers
{
    public class TournamentScoreboardController : Controller
    {
        //
        // GET: /TournamentScoreboard/

        [Authorize]
        public ActionResult Index()
        {
            Team team = SessionUtil.GetTeamFromFormsAuthentication();
            Tournament t = SessionUtil.GetTournamentFromSession();
            TournamentAssignment ta = SessionUtil.GetTournamentAssignmentFromSession();
            Assignment a = SessionUtil.GetAssignmentFromSession();

            if (t == null)
            {
                return RedirectToAction("Index", "Assignments");
            }


            // Get a list of TournamentAssignments with associated enrollments
            // Each enrollment has the last submit

            List<TournamentAssignment> taList = ClientFacade.Instance.GetTournamentScoreboard(t.Id);

            TournamentScoreboardModel model = new TournamentScoreboardModel();
            model.Fill(taList);

            // Make the page reload every 20 seconds
            Response.AddHeader("refresh", "20");

            return View(model);

        }

    }
}
