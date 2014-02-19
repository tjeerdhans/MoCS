using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using MoCS.Data.Repositories;
using MoCS.Web.Code;
using MoCS.Web.Models;
using System.Linq;
using System.Web.Mvc;

namespace MoCS.Web.Controllers
{
    [HandleError]
    public class TeamsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        //
        // GET: /Team/
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var appContext = new ApplicationDbContext();

            var model = _unitOfWork.TeamsRepository.GetAll().Select(t => new TeamModel(t, appContext.Users.SingleOrDefault(u => u.Id == t.AdminUser).UserName));

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            var appContext = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();
            var users = User.IsInRole("Admin") ? appContext.Users.ToList() : appContext.Users.Where(u => u.Id == userId).ToList();
            var model = new TeamModel { Users = users, AdminUser = userId };
            return View("AddOrEdit", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(TeamModel teamModel)
        {
            // Check if the chosen team name isn't already taken
            if (_unitOfWork.TeamsRepository.Any(t => t.Name == teamModel.Name))
            {
                ModelState.AddModelError("Name", "That team name is already taken.");
            }
            if (!ModelState.IsValid)
            {
                var appContext = new ApplicationDbContext();
                var userId = User.Identity.GetUserId();
                var users = User.IsInRole("Admin") ? appContext.Users.ToList() : appContext.Users.Where(u => u.Id == userId).ToList();
                teamModel.Users = users;
                return View("AddOrEdit", teamModel);
            }

            // add the team
            var team = teamModel.ToTeam();

            _unitOfWork.TeamsRepository.Add(team);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            // only show teams of which we are the admin (or show all if logged on user is in admin role

            var team = _unitOfWork.TeamsRepository.SingleOrDefault(t => t.Id == id);
            if (team == null)
            {
                throw new MoCSHttpException(404, "Invalid team id. Try again, dearie.");
            }
            var userId = User.Identity.GetUserId();
            if (team.AdminUser != userId && !User.IsInRole("Admin"))
            {
                throw new MoCSHttpException(403, "You are not allowed to edit this team. Talk to your (team) admin.");
            }

            var appContext = new ApplicationDbContext();
            var users = appContext.Users.ToList();
            var model = new TeamModel(team) { Users = users };

            return View("AddOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamModel teamModel)
        {
            var team = _unitOfWork.TeamsRepository.SingleOrDefault(t => t.Id == teamModel.Id);
            if (team == null)
            {
                throw new MoCSHttpException(404, "Invalid team id. Please reload the page.");
            }
            var userId = User.Identity.GetUserId();
            if (team.AdminUser != userId && !User.IsInRole("Admin"))
            {
                throw new MoCSHttpException(403, "You are not allowed to edit this team. Talk to your (team) admin.");
            }
            // Check if the chosen team name isn't already taken
            if (_unitOfWork.TeamsRepository.Any(t => t.Id != team.Id && t.Name == teamModel.Name))
            {
                ModelState.AddModelError("Name", "That team name is already taken.");
            }
            if (!ModelState.IsValid)
            {
                var appContext = new ApplicationDbContext();
                var users = appContext.Users.ToList();
                teamModel.Users = users;
                return View("AddOrEdit", teamModel);
            }

            // do the changes
            team.Name = teamModel.Name;
            team.Description = teamModel.Description;
            team.AdminUser = teamModel.AdminUser;

            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}