using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CS.Filter
{
    public class ErrorAttribute : ActionFilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            //获取异常信息，入库保存
            Exception Error = filterContext.Exception;
            string ClientIP = filterContext.HttpContext.Request.UserHostAddress;
            string ClientURL = filterContext.HttpContext.Request.Url.AbsoluteUri;

            string Message = Error.Message;//错误信息

            try
            {
                InsertSysLog(ClientIP, ClientURL, Message);
            }
            catch (Exception ex)
            {

            }

            string Url = HttpContext.Current.Request.RawUrl;//错误发生地址
            filterContext.ExceptionHandled = true;
            HttpContext.Current.Session["errorMsg"] = Message;
            filterContext.Result = new RedirectResult("/Error/Index");//跳转至错误提示页面
        }

        protected void InsertSysLog(string ClientIP, string ClientURL, string errorContent)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into SysLog");
            strSql.Append(" (RequestURL,ErrorContent,RequestIP)");
            strSql.Append(" values (");
            strSql.Append("@RequestURL,@ErrorContent,@RequestIP)");

            SqlParameter[] parameters = {
                    new SqlParameter("@RequestURL",SqlDbType.NVarChar),
                    new SqlParameter("@ErrorContent",SqlDbType.NText ),
                    new SqlParameter("@RequestIP", SqlDbType.NVarChar)
            };
            parameters[0].Value = ClientURL;
            parameters[1].Value = errorContent;
            parameters[2].Value = ClientIP;
            DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters);
        }
    }
}