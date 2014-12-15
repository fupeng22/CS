using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Filter;

namespace CS.Controllers.customs
{
    public class CustomerMainController : Controller
    {
        //
        // GET: /CustomerMain/
        [CustomerRequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string CreateStatusJSON()
        {
            return "[{\"id\":\"-99\",\"text\":\"--请选择--\",\"selected\":true},{\"id\":\"0\",\"text\":\"放行\"},{\"id\":\"1\",\"text\":\"等待预检\"},{\"id\":\"2\",\"text\":\"查验放行\"},{\"id\":\"3\",\"text\":\"查验扣留\"},{\"id\":\"4\",\"text\":\"查验待处理\"},{\"id\":\"99\",\"text\":\"查验退货\"}]";
        }

        [HttpPost]
        public string CreateReleaseFlagJSON()
        {
            return "[{\"id\":\"-99\",\"text\":\"--请选择--\",\"selected\":true},{\"id\":\"0\",\"text\":\"海关检疫都未放行\"},{\"id\":\"1\",\"text\":\"海关放行,检疫未放行\"},{\"id\":\"2\",\"text\":\"检疫放行,海关未放行\"},{\"id\":\"3\",\"text\":\"海关检疫全放行\"},{\"id\":\"99\",\"text\":\"查验退货\"}]";
        }

        [HttpPost]
        public string CreateSortJSON()
        {
            return "[{\"id\":\"--请选择--\",\"text\":\"--请选择--\",\"selected\":true},{\"id\":\"0\",\"text\":\"未过机\"},{\"id\":\"1\",\"text\":\"已过机\"}]";
        }
    }
}
