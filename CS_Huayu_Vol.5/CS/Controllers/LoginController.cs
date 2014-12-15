using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace CS.Controllers
{
    public class LoginController : Controller
    {
        SQLDAL.T_User tUser = new SQLDAL.T_User();
        Model.M_User mUser = new Model.M_User();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public string Login(FormCollection collection)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"登录不成功，原因未知\"}";
            string strUserName = "";
            string strPassword = "";
            string strComment = "";
            string strCompany = "";
            try
            {
                strUserName = collection["txtUserName"].ToString();
                strPassword = collection["txtPassword"].ToString();
                strComment = collection["txtComment"].ToString();

                if (strUserName == "" || strComment == "" || strPassword == "")
                {
                    strRet = "{\"result\":\"error\",\"message\":\"登录不成功，请填写完整信息<br/>(用户名、密码以及登录类型必须填写)\"}";
                }
                else
                {
                    DataSet ds = tUser.CheckLogin(strUserName, strPassword, Convert.ToInt32(strComment));
                    if (ds != null)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            strCompany = dt.Rows[0]["company"].ToString();
                            switch (strComment)
                            {
                                case "0":
                                    Session["Global_Customer_UserName"] = strUserName;
                                    break;
                                case "1":
                                    Session["Global_Huayu_UserName"] = strUserName;
                                    break;
                                case "2":
                                    Session["Global_Forwarder_UserName"] = strUserName;
                                    break;
                                default:
                                    break;
                            }

                            Session["Global_Comment"] = strComment;
                            Session["Global_Company"] = strCompany;

                            strRet = "{\"result\":\"ok\",\"message\":\"登录成功" + "\"}";
                        }
                        else
                        {
                            strRet = "{\"result\":\"error\",\"message\":\"登录不成功，" + "信息不正确" + "\"}";
                        }

                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"登录不成功，" + "信息不正确" + "\"}";
                    }

                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"登录不成功，" + ex.Message.Replace("\"","").Replace("\r\n","") + "\"}";
            }

            return strRet;
        }

        
        [HttpGet]
        public string LogOut()
        {
            string strRet = "{\"result\":\"error\",\"message\":\"原因未知\"}";

            try
            {
                if (Session["Global_Comment"] == null)
                {
                    Session["Global_Customer_UserName"] = null;

                    Session["Global_Huayu_UserName"] = null;

                    Session["Global_Forwarder_UserName"] = null;
                }
                else
                {
                    switch (Session["Global_Comment"].ToString())
                    {
                        case "0":
                            Session["Global_Customer_UserName"] = null;
                            break;
                        case "1":
                            Session["Global_Huayu_UserName"] = null;
                            break;
                        case "2":
                            Session["Global_Forwarder_UserName"] = null;
                            break;
                        default:
                            break;
                    }
                }

                strRet = "{\"result\":\"ok\",\"message\":\"" + "成功注销" + "\"}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }
    }
}
