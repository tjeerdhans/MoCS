using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoCS.Data.Entity;
using MoCS.Data.Repositories;
using MoCS.Web.Models;

namespace MoCS.Web.Controllers
{
    public class TeamsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        //
        // GET: /Team/
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Index()
        {
            var teams = _unitOfWork.TeamsRepository.GetAll().ToList();
            return View(teams);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            var appContext = new ApplicationDbContext();
            var users = appContext.Users.ToList();
            var model = new TeamModel { Users = users };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Add(TeamModel teamModel)
        {
            if (!ModelState.IsValid)
            {
                var appContext = new ApplicationDbContext();
                var users = appContext.Users.ToList();
                teamModel.Users = users;
                return View(teamModel);
            }

            // add the team
            var team = teamModel.ToTeam();

            _unitOfWork.TeamsRepository.Add(team);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

    }
}