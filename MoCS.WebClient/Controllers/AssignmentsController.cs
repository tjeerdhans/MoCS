using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoCS.WebClient.Models;
using MoCS.WebClient.ServiceProxy;
using MoCS.Business.Objects;
using System.Web.Routing;

namespace MoCS.WebClient.Controllers
{
    public class AssignmentsController : Controller
    {
        //
        // GET: /Assignments/

        private Credentials credentials;

        //protected override void Initialize(RequestContext requestContext)
        //{
        //    //credentials = new Credentials()
        //    //{
        //    //    Username = User.Identity.Name,
        //    //    Password = (string)Session["password"]
        //    //};

        //    base.Initialize(requestContext);
        //}

        public ActionResult Index()
        {
            credentials = new Credentials()
            {
                Username = User.Identity.Name,
                Password = (string)Session["password"]
            };
            // Get the tournaments
            TournamentsModel tournaments = new TournamentsModel();
            List<Tournament> beTournamentList = new List<Tournament>();
            beTournamentList = MoCSServiceProxy.Instance.GetTournaments(credentials);

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

        public ActionResult Assignments()
        {
            credentials = new Credentials()
            {
                Username = User.Identity.Name,
                Password = (string)Session["password"]
            };

            int tournamentId = Session["tournamentId"] != null ? (int)Session["tournamentId"] : -1;
            if (Request.QueryString["tournamentId"] != null)
            {
                tournamentId = int.Parse(Request.QueryString["tournamentId"]);

                // Get the tournament
                Tournament tournament = MoCSServiceProxy.Instance.GetTournament(tournamentId, credentials);
                Session["tournamentId"] = tournamentId;
                Session["tournamentName"] = tournament.Name;
            }
            if (tournamentId == -1)
            {
                return RedirectToAction("Index");
            }

            // Get the assignments of the selected tournament
            TournamentAssignmentsModel taModel = new TournamentAssignmentsModel();

            List<TournamentAssignment> beTournamentAssignmentList = new List<TournamentAssignment>();

            beTournamentAssignmentList = MoCSServiceProxy.Instance.GetTournamentAssignmentsForTournament(tournamentId, credentials);

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

            ViewData["tournamentId"] = Session["tournamentId"];
            ViewData["tournamentName"] = Session["tournamentName"];

            return View(taModel);
        }

        public ActionResult Select()
        {            
            int tournamentAssignmentId;          
            if (!int.TryParse(Request.QueryString["tournamentAssignmentId"], out tournamentAssignmentId))
            {                
                return RedirectToAction("Assignments");
            }

            //return View();
            Session["tournamentAssignmentId"] = tournamentAssignmentId;
            Session["assignmentName"] = Request.QueryString["assignmentName"];

            return RedirectToAction("Assignments");
        }

    }
}
