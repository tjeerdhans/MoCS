using Microsoft.AspNet.Identity;
using MoCS.Data.Entity;
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

            var teamModels = _unitOfWork.TeamsRepository.GetAll().Select(t => new TeamModel(t)).ToList();

            // get the admin user names and team member names
            using (var appContext = new ApplicationDbContext())
            {
                foreach (var teamModel in teamModels)
                {
                    var teamId = teamModel.Id;
                    var adminUser = appContext.Users.SingleOrDefault(u => u.Id == teamModel.AdminUserId);
                    if (adminUser != null)
                    {
                        teamModel.AdminUserName = adminUser.UserName;
                    }
                    var teamMembers = appContext.Users.Where(u => u.TeamId == teamId);
                    teamModel.TeamMemberNames = teamMembers.Select(t => t.UserName).ToList();
                }
            }

            return View(teamModels);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            TeamModel teamModel;
            using (var appContext = new ApplicationDbContext())
            {
                var userId = User.Identity.GetUserId();
                var users = User.IsInRole("Admin")
                    ? appContext.Users.ToList()
                    : appContext.Users.Where(u => u.Id == userId).ToList();
                teamModel = new TeamModel { AdminUserId = userId };
                teamModel.FillAdminUsersSelectListItems(users);
                var teamMemberUsers = appContext.Users.ToList();
                teamMemberUsers.ForEach(u =>
                {
                    if (u.TeamId != null)
                    {
                        u.Id = "Disabled";
                        var teamName = _unitOfWork.TeamsRepository.Single(t => t.Id == u.TeamId).Name;
                        teamName = teamName.Length > 7 ? teamName.Substring(0, 6) + ".." : teamName;
                        u.UserName = string.Format("{0} (already in team '{1}')", u.UserName, teamName);
                    }
                });
                teamModel.FillTeamMemberUsersSelectListItems(teamMemberUsers);
            }
            return View("AddOrEdit", teamModel);
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
                using (var appContext = new ApplicationDbContext())
                {
                    var userId = User.Identity.GetUserId();
                    var users = User.IsInRole("Admin") ? appContext.Users.ToList() : appContext.Users.Where(u => u.Id == userId).ToList();
                    teamModel.FillAdminUsersSelectListItems(users);
                    var teamMemberUsers = appContext.Users.ToList();
                    teamMemberUsers.ForEach(u =>
                    {
                        if (u.TeamId != null)
                        {
                            u.Id = "Disabled";
                        }
                    });
                    teamModel.FillTeamMemberUsersSelectListItems(teamMemberUsers);
                }
                return View("AddOrEdit", teamModel);
            }

            // add the team
            var team = teamModel.ToTeam();

            // assign the team to the team members
            using (var appContext = new ApplicationDbContext())
            {
                foreach (var teamMember in teamModel.TeamMemberIds)
                {
                    var user = appContext.Users.SingleOrDefault(u => u.Id == teamMember);
                    if (user != null)
                    {
                        user.TeamId = teamModel.Id;
                    }
                }
                appContext.SaveChanges();
            }

            _unitOfWork.TeamsRepository.Add(team);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            // only allowed to edit teams of which we are the admin (or if logged on user is in admin role)
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

            TeamModel teamModel;
            using (var appContext = new ApplicationDbContext())
            {
                var users = User.IsInRole("Admin")
                    ? appContext.Users.ToList()
                    : appContext.Users.Where(u => u.Id == userId).ToList();
                teamModel = new TeamModel(team);// { Users = users };
                teamModel.FillAdminUsersSelectListItems(users);
                teamModel.TeamMemberIds =
                    appContext.Users.Where(u => u.TeamId == teamModel.Id).Select(u => u.Id).ToList();
                var teamMemberUsers = appContext.Users.ToList();
                teamMemberUsers.ForEach(u =>
                 {
                     if (u.TeamId != null && u.TeamId != teamModel.Id)
                     {
                         u.Id = "Disabled";
                         var teamName = _unitOfWork.TeamsRepository.Single(t => t.Id == u.TeamId).Name;
                         teamName = teamName.Length > 7 ? teamName.Substring(0, 6) + ".." : teamName;
                         u.UserName = string.Format("{0} (already in team '{1}')", u.UserName, teamName);
                     }
                 });
                teamModel.FillTeamMemberUsersSelectListItems(teamMemberUsers);
            }
            return View("AddOrEdit", teamModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamModel teamModel)
        {
            // validate
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
                using (var appContext = new ApplicationDbContext())
                {
                    var users = appContext.Users.ToList();
                    teamModel.FillAdminUsersSelectListItems(users);

                    var teamMemberUsers = appContext.Users.ToList();
                    teamMemberUsers.ForEach(u =>
                    {
                        if (u.TeamId != null)
                        {
                            u.Id = "Disabled";
                            u.UserName = u.UserName + " (already in another team)";
                        }
                    });
                    teamModel.FillTeamMemberUsersSelectListItems(teamMemberUsers);
                }
                return View("AddOrEdit", teamModel);
            }

            // do the changes
            team.Name = teamModel.Name;
            team.Description = teamModel.Description;
            team.AdminUser = teamModel.AdminUserId;

            // reassign the team to the team members
            using (var appContext = new ApplicationDbContext())
            {
                var originalTeamMembers = appContext.Users.Where(u => u.TeamId == teamModel.Id).ToList();
                originalTeamMembers.ForEach(t => t.TeamId = null);
                foreach (var teamMember in teamModel.TeamMemberIds)
                {
                    var user = appContext.Users.SingleOrDefault(u => u.Id == teamMember);
                    if (user != null)
                    {
                        user.TeamId = teamModel.Id;
                    }
                }
                appContext.SaveChanges();
            }

            //save
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            // only allowed to edit teams of which we are the admin (or if logged on user is in admin role)
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

            // remove the team from the team members
            using (var appContext = new ApplicationDbContext())
            {
                var originalTeamMembers = appContext.Users.Where(u => u.TeamId == id).ToList();
                originalTeamMembers.ForEach(t => t.TeamId = null);
                appContext.SaveChanges();
            }

            _unitOfWork.TeamsRepository.Delete(team);
            // TODO remove any AssignmentEnrollments
            // TODO remove any submits
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}