using System.Web.Mvc;
using MoCS.Data.Repositories;

namespace MoCS.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        //
        // GET: /Users/
        public ActionResult Index()
        {
            return View();
        }
	}
}