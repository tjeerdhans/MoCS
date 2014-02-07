using MoCS.Business.Facade;
using MoCS.Business.Objects;
using MoCS.WebClient.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MoCS.WebClient.Controllers
{
    public class AssignmentsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            // Get the tournaments
            var tournaments = new TournamentsModel();

            var beTournamentList = ClientFacade.Instance.GetTournaments();

            foreach (var beTournament in beTournamentList)
            {
                tournaments.Add(new TournamentModel
                {
                    Id = beTournament.Id,
                    Name = beTournament.Name
                });
            }

            return View(tournaments);
        }

        [Authorize]
        public ActionResult Assignments()
        {
            var t = SessionUtil.GetTournamentFromSession();

            if (t == null)
            {
                return RedirectToAction("Index");
            }

            // Get the assignments of the selected tournament
            var taModel = new TournamentAssignmentsModel();

            List<TournamentAssignment> beTournamentAssignmentList = ClientFacade.Instance.GetTournamentAssignmentsForTournament(t.Id);

            beTournamentAssignmentList.Sort((ta1, ta2) => ta1.AssignmentOrder.CompareTo(ta2.AssignmentOrder));

            foreach (var beTA in beTournamentAssignmentList)
            {
                taModel.Add(new TournamentAssignmentModel
                {
                    Id = beTA.Id,
                    IsActive = beTA.IsActive,
                    AssignmentId = beTA.Assignment.Id,
                    AssignmentName = beTA.Assignment.Name,
                    Author = beTA.Assignment.Author,
                    Category = beTA.Assignment.Category,
                    Difficulty = beTA.Assignment.Difficulty,
                    FriendlyName = beTA.Assignment.FriendlyName,
                    Tagline = beTA.Assignment.Tagline,
                    Points = beTA.Points1
                });
            }

            ViewData["tournamentId"] = t.Id;
            ViewData["tournamentName"] = t.Name;

            return View(taModel);
        }

        [Authorize]
        public ActionResult SelectTournament(int tournamentId)
        {
            // Get the tournament
            var tournament = ClientFacade.Instance.GetTournamentById(tournamentId);

            if (tournament == null)
            {
                return RedirectToAction("Index");
            }

            // set the session context 
            SessionUtil.SetSession(tournament, null, null, null);

            return RedirectToAction("Assignments");

        }

        [Authorize]
        public ActionResult SelectAssignment(int assignmentId, int tournamentAssignmentId, string assignmentName)
        {
            var t = SessionUtil.GetTournamentFromSession();
            if (t == null)
            {
                return RedirectToAction("Index");
            }
            // Check for existence of tournamentAssignment
            var ta = ClientFacade.Instance.GetTournamentAssignmentById(tournamentAssignmentId, false);
            if (ta == null)
            {
                return RedirectToAction("Assignments");
            }

            //Check if the tournamentassignment is active
            if (!ta.IsActive)
            {
                return RedirectToAction("Assignments");
            }

            // Check if the tournamentassignment is part of the selected tournament
            if (ta.Tournament.Id != t.Id)
            {
                return RedirectToAction("Index");
            }

            // Set session context
            SessionUtil.SetSession(t, ta, new Assignment { Id = ta.Assignment.Id, Name = ta.Assignment.Name }, null);

            return RedirectToAction("Index", "CurrentAssignment");
        }

    }
}
