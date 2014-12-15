using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SQLDAL;
using Model;
using CS.Filter;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_EmailManagementController : Controller
    {
        //
        // GET: /Huayu_EmailManagement/
        [HuayuRequiresLoginAttribute]
        public ActionResult Index()
        {
            DataSet ds = new T_EmailManagement().GetAllEmail();
            M_EmailManagement m_EmailManagement = new M_EmailManagement();
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    m_EmailManagement.eId = Convert.ToInt32(dt.Rows[0]["eId"].ToString());
                    m_EmailManagement.EmailSMTP = dt.Rows[0]["EmailSMTP"].ToString();
                    m_EmailManagement.EmailUserName = dt.Rows[0]["EmailUserName"].ToString();
                    m_EmailManagement.EmailPwd =Util.CryptographyTool.Decrypt( dt.Rows[0]["EmailPwd"].ToString(),"HuayuTAT");
                    m_EmailManagement.FirstPickGoodEmail_Subject = dt.Rows[0]["FirstPickGoodEmail_Subject"].ToString();
                    m_EmailManagement.FirstPickGoodEmail_Body = dt.Rows[0]["FirstPickGoodEmail_Body"].ToString();
                    m_EmailManagement.UnReleaseGoodEmail_Subject = dt.Rows[0]["UnReleaseGoodEmail_Subject"].ToString();
                    m_EmailManagement.UnReleaseGoodEmail_Body = dt.Rows[0]["UnReleaseGoodEmail_Body"].ToString();
                    m_EmailManagement.RejectGoodEmail_Subject = dt.Rows[0]["RejectGoodEmail_Subject"].ToString();
                    m_EmailManagement.RejectGoodEmail_Body = dt.Rows[0]["RejectGoodEmail_Body"].ToString();
                    m_EmailManagement.SendDialyReport_Subject = dt.Rows[0]["SendDialyReport_Subject"].ToString();
                    m_EmailManagement.SendDialyReport_Body = dt.Rows[0]["SendDialyReport_Body"].ToString();
                    m_EmailManagement.mMemo = dt.Rows[0]["mMemo"].ToString();
                }
            }
            return View(m_EmailManagement);
        }

        [HttpPost]
        public string SaveEmail(string txt_EmailSMTP, string txt_EmailUserName, string txt_EmailPwd, string txt_FirstPickGoodEmail_Subject, string txt_FirstPickGoodEmail_Body, string txt_UnReleaseGoodEmail_Subject, string txt_UnReleaseGoodEmail_Body, string txt_RejectGoodEmail_Subject, string txt_RejectGoodEmail_Body)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"提交失败,原因未知\"}";

            string str_txt_EmailSMTP = Server.UrlDecode(txt_EmailSMTP);
            string str_txt_EmailUserName = Server.UrlDecode(txt_EmailUserName);
            string str_txt_EmailPwd = Server.UrlDecode(txt_EmailPwd);
            string str_txt_FirstPickGoodEmail_Subject = Server.UrlDecode(txt_FirstPickGoodEmail_Subject);
            string str_txt_FirstPickGoodEmail_Body = Server.UrlDecode(txt_FirstPickGoodEmail_Body);
            string str_txt_UnReleaseGoodEmail_Subject = Server.UrlDecode(txt_UnReleaseGoodEmail_Subject);
            string str_txt_UnReleaseGoodEmail_Body = Server.UrlDecode(txt_UnReleaseGoodEmail_Body);
            string str_txt_RejectGoodEmail_Subject = Server.UrlDecode(txt_RejectGoodEmail_Subject);
            string str_txt_RejectGoodEmail_Body = Server.UrlDecode(txt_RejectGoodEmail_Body);

            M_EmailManagement m_EmailManagement = new M_EmailManagement();
            T_EmailManagement t_EmailManagement = new T_EmailManagement();

            try
            {
                t_EmailManagement.DeleteAllEmail();
                m_EmailManagement.EmailSMTP = str_txt_EmailSMTP;
                m_EmailManagement.EmailUserName = str_txt_EmailUserName;
                m_EmailManagement.EmailPwd = str_txt_EmailPwd;
                m_EmailManagement.FirstPickGoodEmail_Subject = str_txt_FirstPickGoodEmail_Subject;
                m_EmailManagement.FirstPickGoodEmail_Body = str_txt_FirstPickGoodEmail_Body;
                m_EmailManagement.UnReleaseGoodEmail_Subject = str_txt_UnReleaseGoodEmail_Subject;
                m_EmailManagement.UnReleaseGoodEmail_Body = str_txt_UnReleaseGoodEmail_Body;
                m_EmailManagement.RejectGoodEmail_Subject = str_txt_RejectGoodEmail_Subject;
                m_EmailManagement.RejectGoodEmail_Body = str_txt_RejectGoodEmail_Body;
                m_EmailManagement.mMemo = "";
                t_EmailManagement.InsertEmail(m_EmailManagement);
                strRet = "{\"result\":\"ok\",\"message\":\"提交成功\"}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"提交失败,原因:" + ex.Message + "\"}";
            }

            return strRet;
        }

        [HttpPost]
        [ValidateInput(false)]
        public string SaveEmailForm(FormCollection collection)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"提交失败,原因未知\"}";

            string str_txt_EmailSMTP = collection["txt_EmailSMTP"].ToString();
            string str_txt_EmailUserName = collection["txt_EmailUserName"].ToString();
            string str_txt_EmailPwd = collection["txt_EmailPwd"].ToString();
            string str_txt_FirstPickGoodEmail_Subject = collection["txt_FirstPickGoodEmail_Subject"].ToString();
            string str_txt_FirstPickGoodEmail_Body = collection["hid_txt_FirstPickGoodEmail_Body"].ToString();
            string str_txt_UnReleaseGoodEmail_Subject = collection["txt_UnReleaseGoodEmail_Subject"].ToString();
            string str_txt_UnReleaseGoodEmail_Body = collection["hid_txt_UnReleaseGoodEmail_Body"].ToString();
            string str_txt_RejectGoodEmail_Subject = collection["txt_RejectGoodEmail_Subject"].ToString();
            string str_txt_RejectGoodEmail_Body = collection["hid_txt_RejectGoodEmail_Body"].ToString();
            string str_txt_SendDialyReportMail_Subject = collection["txt_SendDialyReportMail_Subject"].ToString();
            string str_txt_SendDialyReportMail_Body = collection["hid_txt_SendDialyReportMail_Body"].ToString();

            M_EmailManagement m_EmailManagement = new M_EmailManagement();
            T_EmailManagement t_EmailManagement = new T_EmailManagement();

            try
            {
                t_EmailManagement.DeleteAllEmail();
                m_EmailManagement.EmailSMTP = str_txt_EmailSMTP;
                m_EmailManagement.EmailUserName = str_txt_EmailUserName;
                m_EmailManagement.EmailPwd =Util.CryptographyTool.Encrypt( str_txt_EmailPwd,"HuayuTAT");
                m_EmailManagement.FirstPickGoodEmail_Subject = str_txt_FirstPickGoodEmail_Subject;
                m_EmailManagement.FirstPickGoodEmail_Body = str_txt_FirstPickGoodEmail_Body;
                m_EmailManagement.UnReleaseGoodEmail_Subject = str_txt_UnReleaseGoodEmail_Subject;
                m_EmailManagement.UnReleaseGoodEmail_Body = str_txt_UnReleaseGoodEmail_Body;
                m_EmailManagement.RejectGoodEmail_Subject = str_txt_RejectGoodEmail_Subject;
                m_EmailManagement.RejectGoodEmail_Body = str_txt_RejectGoodEmail_Body;
                m_EmailManagement.SendDialyReport_Subject = str_txt_SendDialyReportMail_Subject;
                m_EmailManagement.SendDialyReport_Body = str_txt_SendDialyReportMail_Body;
                m_EmailManagement.mMemo = "";
                t_EmailManagement.InsertEmail(m_EmailManagement);
                strRet = "{\"result\":\"ok\",\"message\":\"提交成功\"}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"提交失败,原因:" + ex.Message + "\"}";
            }

            return strRet;
        }
    }
}
