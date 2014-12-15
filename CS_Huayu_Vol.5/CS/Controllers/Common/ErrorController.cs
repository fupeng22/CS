using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CS.Controllers.Common
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            ViewData["errorMsg"]=HttpContext.Session["errorMsg"];
            return View();
        }

    }
}
