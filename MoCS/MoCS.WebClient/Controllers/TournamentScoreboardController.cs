using MoCS.Business.Facade;
using MoCS.WebClient.Models;
using System.Web.Mvc;

namespace MoCS.WebClient.Controllers
{
    public class TournamentScoreboardController : Controller
    {
        //
        // GET: /TournamentScoreboard/

        [Authorize]
        public ActionResult Index()
        {
            var t = SessionUtil.GetTournamentFromSession();

            if (t == null)
            {
                return RedirectToAction("Index", "Assignments");
            }

            // Get a list of TournamentAssignments with associated enrollments
            // Each enrollment has the last submit

            var taList = ClientFacade.Instance.GetTournamentScoreboard(t.Id);

            var model = new TournamentScoreboardModel();
            model.Fill(taList);

            // Make the page reload every 20 seconds
            Response.AddHeader("refresh", "20");

            return View(model);

        }

    }
}
