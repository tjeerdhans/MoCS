using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoCS.WebClient.Models;
using MoCS.Business.Objects;
using System.Web.Routing;
using MoCS.Business.Facade;
using System.Web.Security;

namespace MoCS.WebClient.Controllers
{
    public class AssignmentsController : Controller
    {
        //
        // GET: /Assignments/

        //protected override void Initialize(RequestContext requestContext)
        //{
        //    //credentials = new Credentials()
        //    //{
        //    //    Username = User.Identity.Name,
        //    //    Password = (string)Session["password"]
        //    //};

        //    base.Initialize(requestContext);
        //}

        [Authorize]
        public ActionResult Index()
        {
            Team team = SessionUtil.GetTeamFromFormsAuthentication();
            Tournament t = SessionUtil.GetTournamentFromSession();
            TournamentAssignment ta = SessionUtil.GetTournamentAssignmentFromSession();
            Assignment a = SessionUtil.GetAssignmentFromSession();

            // Get the tournaments
            TournamentsModel tournaments = new TournamentsModel();
            List<Tournament> beTournamentList = new List<Tournament>();

            beTournamentList = ClientFacade.Instance.GetTournaments();

            foreach (Tournament beTournament in beTournamentList)
            {
                tournaments.Add(new TournamentModel()
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
            Team team = SessionUtil.GetTeamFromFormsAuthentication();
            Tournament t = SessionUtil.GetTournamentFromSession();
            TournamentAssignment ta = SessionUtil.GetTournamentAssignmentFromSession();
            Assignment a = SessionUtil.GetAssignmentFromSession();

            if (t == null)
            {
                return RedirectToAction("Index");
            }


            // Get the assignments of the selected tournament
            TournamentAssignmentsModel taModel = new TournamentAssignmentsModel();

            List<TournamentAssignment> beTournamentAssignmentList = new List<TournamentAssignment>();

            beTournamentAssignmentList = ClientFacade.Instance.GetTournamentAssignmentsForTournament(t.Id);

            beTournamentAssignmentList.Sort((ta1, ta2) => ta1.AssignmentOrder.CompareTo(ta2.AssignmentOrder));

            foreach (TournamentAssignment beTA in beTournamentAssignmentList)
            {
                taModel.Add(new TournamentAssignmentModel()
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
            //Team team = SessionUtil.GetTeamFromFormsAuthentication();
            //Tournament t = SessionUtil.GetTournamentFromSession();
            //TournamentAssignment ta = SessionUtil.GetTournamentAssignmentFromSession();
            //Assignment a = SessionUtil.GetAssignmentFromSession();

            // Get the tournament
            Tournament tournament = ClientFacade.Instance.GetTournamentById(tournamentId);

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
            //Team team = SessionUtil.GetTeamFromFormsAuthentication();
            Tournament t = SessionUtil.GetTournamentFromSession();
            if (t == null)
            {
                return RedirectToAction("Index");
            }
            // Check for existence of tournamentAssignment
            TournamentAssignment ta = ClientFacade.Instance.GetTournamentAssignmentById(tournamentAssignmentId, false);
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
            SessionUtil.SetSession(t, ta, new Assignment() { Id = ta.Assignment.Id, Name = ta.Assignment.Name }, null);

            return RedirectToAction("Index", "CurrentAssignment");
        }

    }
}
