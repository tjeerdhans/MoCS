using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoCS.Web.Controllers
{
    public class TeamsController : Controller
    {
        //
        // GET: /Team/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }
	}
}