using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Filter;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class HuayuMainController : Controller
    {
        //
        // GET: /HuayuMain/
        [HuayuRequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }

    }
}
