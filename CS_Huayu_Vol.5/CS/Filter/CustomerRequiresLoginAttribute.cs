using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SQLDAL;

namespace CS.Filter
{
    public class CustomerRequiresLoginAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在action method之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;
            if (new T_User().LoginValidate())
            {
                if (filterContext.HttpContext.Session["Global_Customer_UserName"] == null)
                {
                    filterContext.HttpContext.Response.Redirect("~/Login/Index" + "?URLRet=" + redirectOnSuccess + "&comment=0");
                }
            }
            else
            {
                throw new Exception("<font style='color:red'>系统使用期限已到，请联系开发商重新注册，请谅解!</font>");
            }
        }
    }
}