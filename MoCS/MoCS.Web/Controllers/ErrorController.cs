using System.Web.Mvc;

namespace MoCS.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult Index()
        {
            return View("Error");
        }
        public ViewResult NotFound()
        {
            Response.StatusCode = 404;  //you may want to set this to 200
            return View();
        }

        public ActionResult Unauthorized()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}