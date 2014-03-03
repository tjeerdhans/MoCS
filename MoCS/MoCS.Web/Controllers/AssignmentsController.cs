using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoCS.Web.Controllers
{
    [HandleError]
    [Authorize(Roles = "Admin")]
    public class AssignmentsController : Controller
    {
        //
        // GET: /Assignments/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
	}
}