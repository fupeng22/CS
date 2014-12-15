﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using SQLDAL;
using System.Data;
using CS.Filter;

namespace CS.Controllers.Forwarder
{
    [ErrorAttribute]
    public class Forwarder_changePasswordController : Controller
    {
        SQLDAL.T_User tUser = new T_User();
        Model.M_User mUser = new M_User();
        //
        // GET: /Forwarder_changePassword/
        [ForwarderRequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        public string ChangePWD(string oldPassword, string newPassword, string newComfirm)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"未修改成功\"}";

            string strOldPassword = Server.UrlDecode(oldPassword);
            string strNewPassword = Server.UrlDecode(newPassword);
            string strReNewPassword = Server.UrlDecode(newComfirm);

            try
            {
                DataSet userDs = tUser.CheckLogin(Session["Global_Forwarder_UserName"] == null ? "" : Session["Global_Forwarder_UserName"].ToString(), strOldPassword, Convert.ToInt32(Session["Global_Comment"]));
                if (userDs != null)
                {
                    mUser = new M_User();
                    mUser.UserName = Session["Global_Forwarder_UserName"] == null ? "" : Session["Global_Forwarder_UserName"].ToString();
                    mUser.UserPassword = strNewPassword;

                    if (tUser.changePassword(mUser))
                    {
                        strRet = "{\"result\":\"ok\",\"message\":\"" + "密码修改成功" + "\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"" + "密码修改失败" + "\"}";
                    }
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "旧密码输入错误" + "\"}";
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }
            Session["Global_Forwarder_UserName"] = null;

            return strRet;
        }

    }
}
