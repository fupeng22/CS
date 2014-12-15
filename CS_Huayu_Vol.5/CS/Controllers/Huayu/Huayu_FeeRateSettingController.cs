using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Filter;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using System.Text;
using Microsoft.Reporting.WebForms;
using System.IO;
using SQLDAL;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_FeeRateSettingController : Controller
    {
        SQLDAL.T_User tUser = new SQLDAL.T_User();
        Model.M_User mUser = new Model.M_User();
        public const string STR_TOP_ID = "top";
        public const string strFileds = "CategoryName,CategoryValue,CategoryUnit,mMemo,CategoryID,CategoryPID,parentID,state,isLeaf,ID,frID";

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
        public string GetData(string order, string page, string rows, string sort, string id)
        {
            string strId = id == null ? STR_TOP_ID : id;
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "FeeRate";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "frID";

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
            param[6].Value = " CategoryPID='" + strId+"' ";

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            StringBuilder sb = new StringBuilder("");
            switch (strId)
            {
                case STR_TOP_ID:
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
                                case "parentID":
                                    if (j != strFiledArray.Length - 1)
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i]["CategoryPID"].ToString());
                                    }
                                    else
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i]["CategoryPID"].ToString());
                                    }
                                    break;
                                case "ID":
                                    if (j != strFiledArray.Length - 1)
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i]["CategoryID"].ToString());
                                    }
                                    else
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i]["CategoryID"].ToString());
                                    }
                                    break;
                                case "state":
                                    if (dt.Rows[i]["isLeaf"].ToString() == "1")
                                    {
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "open");
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "open");
                                        }
                                    }
                                    else
                                    {
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "closed");
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "closed");
                                        }
                                    }
                                    break;
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
                    break;
                default:
                    sb.Append("[");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("{");

                        string[] strFiledArray = strFileds.Split(',');
                        for (int j = 0; j < strFiledArray.Length; j++)
                        {
                            switch (strFiledArray[j])
                            {
                                case "parentID":
                                    if (j != strFiledArray.Length - 1)
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i]["CategoryPID"].ToString());
                                    }
                                    else
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i]["CategoryPID"].ToString());
                                    }
                                    break;
                                case "ID":
                                    if (j != strFiledArray.Length - 1)
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i]["CategoryID"].ToString());
                                    }
                                    else
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i]["CategoryID"].ToString());
                                    }
                                    break;
                                case "state":
                                    if (dt.Rows[i]["isLeaf"].ToString() == "1")
                                    {
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "open");
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "open");
                                        }
                                    }
                                    else
                                    {
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "closed");
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "closed");
                                        }
                                    }
                                    break;
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
                    break;
            }

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

        [HttpPost]
        public string UpdateFeeRate(string frID,string CategoryValue,string CategoryUnit,string mMemo)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"原因未知\"}";
            frID = Server.UrlDecode(frID);
            CategoryValue = Server.UrlDecode(CategoryValue);
            CategoryUnit = Server.UrlDecode(CategoryUnit);
            mMemo = Server.UrlDecode(mMemo);
            try
            {
               
                if (frID != "")
                {
                    if ((new T_FeeRate()).UpdateFeeSetting(frID, CategoryValue, CategoryUnit,mMemo))
                    {
                        strRet = "{\"result\":\"ok\",\"message\":\"" + "成功修改了此费率信息" + "\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"" + "修改过程中出现了问题" + "\"}";
                    }
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "未指定需要修改的费率信息" + "\"}";
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }

        [HttpPost]
        public string getFeeRateValue(string categoryID)
        {
            string strRet = "0.00";
            categoryID = Server.UrlDecode(categoryID);
            strRet = new T_FeeRate().GetFeeRateValue(categoryID);
            return strRet;
        }
    }
}
