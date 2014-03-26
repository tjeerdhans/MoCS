using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MoCS.Data.Repositories;
using MoCS.Web.Models;

namespace MoCS.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();


        //
        // GET: /Users/
        public ActionResult Index()
        {
            var model = new UserModel();
            using (var appContext = new ApplicationDbContext())
            {
                model.UserModelItems = appContext.Users.ToList().Select(u => new UserModelItem(u, _unitOfWork.TeamsRepository.Single(t => t.Id == u.TeamId)));
            }

            return View(model);
        }
    }
}