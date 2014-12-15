using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using System.Text;
using CS.Filter;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace CS.Controllers.customs
{
    [ErrorAttribute]
    public class Huayu_UserMaintainController : Controller
    {
        SQLDAL.T_User tUser = new SQLDAL.T_User();
        Model.M_User mUser = new Model.M_User();

        public const string strFileds = "userName,userPassword,authority,comment,authorityDescription,commentDescription,company,CompanyFullName,LinkPerson,IdentityCode,LinkTel,CompanyPhone,CompanyAddr,LinkMail,iSendFirstPickGoodsEmail,iSendUnReleaseGoodsEmail,iSendRejectGoodsEmail,userID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/HuayuUserMaintain.rdlc";
        //
        // GET: /Forwarder_QueryCompany/
        [HuayuRequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 分页查询类
        /// </summary>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public string GetData(string order, string page, string rows, string sort)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Users";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "userID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = sort + " " + order;

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(page);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = "";

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            StringBuilder sb = new StringBuilder("");
            sb.Append("{");
            sb.AppendFormat("\"total\":{0}", Convert.ToInt32(param[7].Value.ToString()));
            sb.Append(",\"rows\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("{");

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        default:
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            break;
                    }
                }

                if (i == dt.Rows.Count - 1)
                {
                    sb.Append("}");
                }
                else
                {
                    sb.Append("},");
                }
            }
            dt = null;
            if (sb.ToString().EndsWith(","))
            {
                sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }


        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Users";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "userID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = sort + " " + order;

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(page);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = "";

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];
            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("userName", Type.GetType("System.String"));
            dtCustom.Columns.Add("userPassword", Type.GetType("System.String"));
            dtCustom.Columns.Add("authority", Type.GetType("System.String"));
            dtCustom.Columns.Add("comment", Type.GetType("System.String"));
            dtCustom.Columns.Add("authorityDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("commentDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("company", Type.GetType("System.String"));
            dtCustom.Columns.Add("CompanyFullName", Type.GetType("System.String"));
            dtCustom.Columns.Add("LinkPerson", Type.GetType("System.String"));
            dtCustom.Columns.Add("IdentityCode", Type.GetType("System.String"));
            dtCustom.Columns.Add("LinkTel", Type.GetType("System.String"));
            dtCustom.Columns.Add("CompanyPhone", Type.GetType("System.String"));
            dtCustom.Columns.Add("CompanyAddr", Type.GetType("System.String"));
            dtCustom.Columns.Add("LinkMail", Type.GetType("System.String"));
            dtCustom.Columns.Add("iSendFirstPickGoodsEmail", Type.GetType("System.String"));
            dtCustom.Columns.Add("iSendUnReleaseGoodsEmail", Type.GetType("System.String"));
            dtCustom.Columns.Add("iSendRejectGoodsEmail", Type.GetType("System.String"));
            dtCustom.Columns.Add("userID", Type.GetType("System.String"));
            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }

                }
                if (drCustom["userID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("HuayuUserMaintain_DS", dtCustom);

            localReport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding = "UTF-8";
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
                " <OutputFormat>PDF</OutputFormat>" +
                " <PageWidth>12in</PageWidth>" +
                " <PageHeigth>11in</PageHeigth>" +
                " <MarginTop>0.5in</MarginTop>" +
                " <MarginLeft>1in</MarginLeft>" +
                " <MarginRight>1in</MarginRight>" +
                " <MarginBottom>0.5in</MarginBottom>" +
                " </DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            return File(renderedBytes, mimeType);
        }


        [HttpGet]
        public ActionResult Excel(string order, string page, string rows, string sort, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Users";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "userID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = sort + " " + order;

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(page);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = "";

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("userName", Type.GetType("System.String"));
            dtCustom.Columns.Add("userPassword", Type.GetType("System.String"));
            dtCustom.Columns.Add("authority", Type.GetType("System.String"));
            dtCustom.Columns.Add("comment", Type.GetType("System.String"));
            dtCustom.Columns.Add("authorityDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("commentDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("company", Type.GetType("System.String"));
            dtCustom.Columns.Add("CompanyFullName", Type.GetType("System.String"));
            dtCustom.Columns.Add("LinkPerson", Type.GetType("System.String"));
            dtCustom.Columns.Add("IdentityCode", Type.GetType("System.String"));
            dtCustom.Columns.Add("LinkTel", Type.GetType("System.String"));
            dtCustom.Columns.Add("CompanyPhone", Type.GetType("System.String"));
            dtCustom.Columns.Add("CompanyAddr", Type.GetType("System.String"));
            dtCustom.Columns.Add("LinkMail", Type.GetType("System.String"));
            dtCustom.Columns.Add("iSendFirstPickGoodsEmail", Type.GetType("System.String"));
            dtCustom.Columns.Add("iSendUnReleaseGoodsEmail", Type.GetType("System.String"));
            dtCustom.Columns.Add("iSendRejectGoodsEmail", Type.GetType("System.String"));
            dtCustom.Columns.Add("userID", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }

                }
                if (drCustom["userID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("HuayuUserMaintain_DS", dtCustom);

            localReport.DataSources.Add(reportDataSource);

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = localReport.Render(
               "Excel", null, out mimeType, out encoding, out extension,
               out streamids, out warnings);
            string strFileName = Server.MapPath(STR_TEMPLATE_EXCEL);
            FileStream fs = new FileStream(strFileName, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            string strOutputFileName = "系统用户信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            switch (browserType.ToLower())
            {
                case "safari":
                    break;
                case "mozilla":
                    break;
                default:
                    strOutputFileName = HttpUtility.UrlEncode(strOutputFileName);
                    break;
            }

            return File(strFileName, "application/vnd.ms-excel", strOutputFileName);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string txtUserID = collection["txtUserID"].ToString();
                string txtUserPassword = collection["txtUserPassword"].ToString();
                string txtReUserPassword = collection["txtReUserPassword"].ToString();
                string txtCompany = collection["txtCompany"].ToString();
                string ddAuthority = collection["ddAuthority"].ToString();
                string ddComment = collection["ddComment"].ToString();
                string txtLinkPerson = collection["txtLinkPerson"].ToString();
                string txtLinkTel = collection["txtLinkTel"].ToString();
                string txtIndentityCpde = collection["txtIndentityCpde"].ToString();
                string txtCompanyFullName = collection["txtCompanyFullName"].ToString();
                string txtCompanyPhone = collection["txtCompanyPhone"].ToString();
                string txtCompanyAddr = collection["txtCompanyAddr"].ToString();
                string txtLinkMail = collection["txtLinkMail"].ToString();
                string chkiSendFirstPickGoodsEmail = collection["chkiSendFirstPickGoodsEmail"] == null ? "0" : "1";// collection["chkiSendFirstPickGoodsEmail"].ToString();
                string chkiSendUnReleaseGoodsEmail = collection["chkiSendUnReleaseGoodsEmail"] == null ? "0" : "1"; // collection["chkiSendUnReleaseGoodsEmail"].ToString();
                string chkiSendRejectGoodsEmail = collection["chkiSendRejectGoodsEmail"] == null ? "0" : "1";   //collection["chkiSendRejectGoodsEmail"].ToString();

                mUser.UserName = txtUserID;
                mUser.UserPassword = txtUserPassword;
                mUser.Authority = Convert.ToInt32(ddAuthority);
                mUser.Comment = Convert.ToInt32(ddComment); ;
                mUser.Company = txtLinkPerson;
                mUser.LinkPerson = txtCompany;
                mUser.LinkTel = txtLinkTel;
                mUser.CompanyFullName = txtCompanyFullName;
                mUser.CompanyPhone = txtCompanyPhone;
                mUser.IdentityCode = txtIndentityCpde;
                mUser.CompanyAddr = txtCompanyAddr;
                mUser.LinkMail = txtLinkMail;
                mUser.iSendFirstPickGoodsEmail = Convert.ToInt32(chkiSendFirstPickGoodsEmail);
                mUser.iSendUnReleaseGoodsEmail = Convert.ToInt32(chkiSendUnReleaseGoodsEmail);
                mUser.iSendRejectGoodsEmail = Convert.ToInt32(chkiSendRejectGoodsEmail);
                tUser.addUser(mUser);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult Edit(int id)
        {
            mUser = new Model.M_User();
            DataSet ds = tUser.GetUser(id.ToString());
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    mUser.UserID = Convert.ToInt32(dt.Rows[0]["userID"]);
                    mUser.UserName = dt.Rows[0]["userName"].ToString();
                    mUser.UserPassword = dt.Rows[0]["userPassword"].ToString();
                    mUser.Authority = Convert.ToInt32(dt.Rows[0]["authority"]);
                    mUser.Comment = Convert.ToInt32(dt.Rows[0]["comment"]);
                    mUser.Company = dt.Rows[0]["company"].ToString();
                    mUser.LinkPerson = dt.Rows[0]["LinkPerson"].ToString();
                    mUser.LinkTel = dt.Rows[0]["LinkTel"].ToString();
                    mUser.CompanyFullName = dt.Rows[0]["CompanyFullName"].ToString();
                    mUser.CompanyPhone = dt.Rows[0]["CompanyPhone"].ToString();
                    mUser.CompanyAddr = dt.Rows[0]["CompanyAddr"].ToString();
                    mUser.IdentityCode = dt.Rows[0]["IdentityCode"].ToString();
                    mUser.LinkMail = dt.Rows[0]["LinkMail"].ToString();
                    mUser.iSendFirstPickGoodsEmail = Convert.ToInt32(dt.Rows[0]["iSendFirstPickGoodsEmail"].ToString());
                    mUser.iSendUnReleaseGoodsEmail = Convert.ToInt32(dt.Rows[0]["iSendUnReleaseGoodsEmail"].ToString());
                    mUser.iSendRejectGoodsEmail = Convert.ToInt32(dt.Rows[0]["iSendRejectGoodsEmail"].ToString());
                }
            }
            else
            {

            }

            return View(mUser);
        }


        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                mUser = new Model.M_User();
                string txtUserID = collection["txtUserID"].ToString();
                string txtUserPassword = collection["txtUserPassword"].ToString();
                string txtReUserPassword = collection["txtReUserPassword"].ToString();
                string txtCompany = collection["txtCompany"].ToString();
                string ddAuthority = collection["ddAuthority"].ToString();
                string ddComment = collection["ddComment"].ToString();
                string txtLinkPerson = collection["txtLinkPerson"].ToString();
                string txtLinkTel = collection["txtLinkTel"].ToString();
                string txtIndentityCpde = collection["txtIndentityCpde"].ToString();
                string txtCompanyFullName = collection["txtCompanyFullName"].ToString();
                string txtCompanyPhone = collection["txtCompanyPhone"].ToString();
                string txtCompanyAddr = collection["txtCompanyAddr"].ToString();
                string txtLinkMail = collection["txtLinkMail"].ToString();
                string chkiSendFirstPickGoodsEmail = collection["chkiSendFirstPickGoodsEmail"] == null ? "0" : "1";// collection["chkiSendFirstPickGoodsEmail"].ToString();
                string chkiSendUnReleaseGoodsEmail = collection["chkiSendUnReleaseGoodsEmail"] == null ? "0" : "1"; // collection["chkiSendUnReleaseGoodsEmail"].ToString();
                string chkiSendRejectGoodsEmail = collection["chkiSendRejectGoodsEmail"] == null ? "0" : "1";   //collection["chkiSendRejectGoodsEmail"].ToString();

                mUser.UserName = txtUserID;
                mUser.UserPassword = txtUserPassword;
                mUser.Authority = Convert.ToInt32(ddAuthority);
                mUser.Comment = Convert.ToInt32(ddComment); ;
                mUser.Company = txtCompany;
                mUser.LinkPerson = txtCompany;
                mUser.LinkTel = txtLinkTel;
                mUser.CompanyFullName = txtCompanyFullName;
                mUser.CompanyPhone = txtCompanyPhone;
                mUser.IdentityCode = txtIndentityCpde;
                mUser.CompanyAddr = txtCompanyAddr;
                mUser.LinkMail = txtLinkMail;
                mUser.iSendFirstPickGoodsEmail = Convert.ToInt32(chkiSendFirstPickGoodsEmail);
                mUser.iSendUnReleaseGoodsEmail = Convert.ToInt32(chkiSendUnReleaseGoodsEmail);
                mUser.iSendRejectGoodsEmail = Convert.ToInt32(chkiSendRejectGoodsEmail);
                mUser.UserID = id;

                tUser.UpdateUser(mUser);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult Delete(int id)
        {
            return View();
        }


        [HttpPost]
        public ActionResult Delete(string ids)
        {
            try
            {
                string[] idsArr = ids.Split(',');
                for (int i = 0; i < idsArr.Length; i++)
                {
                    if (idsArr[i].Trim() != "")
                    {
                        tUser.deleteUser(Convert.ToInt32(idsArr[i]));
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        [HttpGet]
        public string ExistUserName(string strUsername)
        {
            string strRet = "false";
            strUsername = Server.UrlDecode(strUsername);
            strRet = tUser.UserExists(strUsername).ToString();
            return strRet;
        }


        [HttpGet]
        public string ExistUserName_Update(int id, string strUsername)
        {
            string strRet = "false";
            strUsername = Server.UrlDecode(strUsername);
            strRet = tUser.UserExists(id, strUsername).ToString();
            return strRet;
        }
    }
}
