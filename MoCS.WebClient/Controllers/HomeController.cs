using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoCS.WebClient.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to the C# Assignment Framework";
            //Get the tournaments (assignment sets)


            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
