using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SQLDAL;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using DBUtility;
using CS.Filter;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_AnanalyseStatisticController : Controller
    {
        SQLDAL.T_WayBill tWayBill = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();
        public const string strFileds = "dStorageDate,vCompany,vSerialNum,iTotalNum,dTotalWeight,InStoreNum,OutStoreNum,ReleaseNum,NotReleaseNum,wbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/HuayuAnanalyseStatistic.rdlc";
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
        public string GetData(string order, string page, string rows, string sort, string txtvCompany, string txtvStorageBeginDate, string txtvStorageEndDate, string txtvSerialNum)
        {
            int maxCount = 0;
            SqlParameter[] parameters = {
                      
                        new SqlParameter("@vCompany_input",SqlDbType.NVarChar),
                        new SqlParameter("@vStorageBeginDate_input",SqlDbType.NVarChar),
                        new SqlParameter("@vStorageEndDate_input",SqlDbType.NVarChar),
                        new SqlParameter("@vSerialNum_input",SqlDbType.NVarChar)};

            parameters[0].Value = txtvCompany;
            parameters[1].Value = txtvStorageBeginDate == "" ? null : Convert.ToDateTime(txtvStorageBeginDate).ToString("yyyyMMdd");
            parameters[2].Value = txtvStorageEndDate == "" ? null : Convert.ToDateTime(txtvStorageEndDate).ToString("yyyyMMdd");
            parameters[3].Value = txtvSerialNum;
            DataSet ds = SqlServerHelper.RunProcedure("sp_WayBill_AnanalyseStatistic", parameters, "Default");
            DataTable dt = ds.Tables[0];

            dt.DefaultView.Sort = sort + " " + order;
            dt = dt.DefaultView.ToTable();

            StringBuilder sb = new StringBuilder("");
            sb.Append("{");
            sb.AppendFormat("\"total\":{0}", Convert.ToInt32(dt.Rows.Count));
            sb.Append(",\"rows\":[");

            if (Convert.ToInt32(page) > dt.Rows.Count / Convert.ToInt32(rows) && Convert.ToInt32(page) <= dt.Rows.Count / Convert.ToInt32(rows) + 1)
            {
                maxCount = dt.Rows.Count;
            }
            else
            {
                maxCount = Convert.ToInt32(rows) * Convert.ToInt32(page);
            }

            for (int i = Convert.ToInt32(rows) * (Convert.ToInt32(page) - 1); i < maxCount; i++)
            {
                sb.Append("{");

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "vCompany"://格式化公司(保存的是用户名，取出公司名)
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : ((new T_User()).GetUserByUserName(dt.Rows[i][strFiledArray[j]].ToString())));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : ((new T_User()).GetUserByUserName(dt.Rows[i][strFiledArray[j]].ToString())));
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
            return sb.ToString();
        }

        
        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort, string txtvCompany, string txtvStorageBeginDate, string txtvStorageEndDate, string txtvSerialNum)
        {
            int maxCount = 0;
            SqlParameter[] parameters = {
                      
                        new SqlParameter("@vCompany_input",SqlDbType.NVarChar),
                        new SqlParameter("@vStorageBeginDate_input",SqlDbType.NVarChar),
                        new SqlParameter("@vStorageEndDate_input",SqlDbType.NVarChar),
                        new SqlParameter("@vSerialNum_input",SqlDbType.NVarChar)};

            parameters[0].Value = txtvCompany;
            parameters[1].Value = txtvStorageBeginDate == "" ? null : Convert.ToDateTime(txtvStorageBeginDate).ToString("yyyyMMdd");
            parameters[2].Value = txtvStorageEndDate == "" ? null : Convert.ToDateTime(txtvStorageEndDate).ToString("yyyyMMdd");
            parameters[3].Value = txtvSerialNum;
            DataSet ds = SqlServerHelper.RunProcedure("sp_WayBill_AnanalyseStatistic", parameters, "Default");
            DataTable dt = ds.Tables[0];

            dt.DefaultView.Sort = sort + " " + order;
            dt = dt.DefaultView.ToTable();

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("dStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("vCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("vSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("iTotalNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("dTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("InStoreNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStoreNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("ReleaseNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("NotReleaseNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));

            DataRow drCustom = null;

            if (Convert.ToInt32(page) > dt.Rows.Count / Convert.ToInt32(rows) && Convert.ToInt32(page) <= dt.Rows.Count / Convert.ToInt32(rows) + 1)
            {
                maxCount = dt.Rows.Count;
            }
            else
            {
                maxCount = Convert.ToInt32(rows) * Convert.ToInt32(page);
            }

            for (int i = Convert.ToInt32(rows) * (Convert.ToInt32(page) - 1); i < maxCount; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "vCompany"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : ((new T_User()).GetUserByUserName(dt.Rows[i][strFiledArray[j]].ToString()));
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }
                if (drCustom["wbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("HuayuAnanalyseStatistic_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtvCompany, string txtvStorageBeginDate, string txtvStorageEndDate, string txtvSerialNum, string browserType)
        {
            int maxCount = 0;
            SqlParameter[] parameters = {
                      
                        new SqlParameter("@vCompany_input",SqlDbType.NVarChar),
                        new SqlParameter("@vStorageBeginDate_input",SqlDbType.NVarChar),
                        new SqlParameter("@vStorageEndDate_input",SqlDbType.NVarChar),
                        new SqlParameter("@vSerialNum_input",SqlDbType.NVarChar)};

            parameters[0].Value = txtvCompany;
            parameters[1].Value = txtvStorageBeginDate == "" ? null : Convert.ToDateTime(txtvStorageBeginDate).ToString("yyyyMMdd");
            parameters[2].Value = txtvStorageEndDate == "" ? null : Convert.ToDateTime(txtvStorageEndDate).ToString("yyyyMMdd");
            parameters[3].Value = txtvSerialNum;
            DataSet ds = SqlServerHelper.RunProcedure("sp_WayBill_AnanalyseStatistic", parameters, "Default");
            DataTable dt = ds.Tables[0];

            dt.DefaultView.Sort = sort + " " + order;
            dt = dt.DefaultView.ToTable();

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("dStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("vCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("vSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("iTotalNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("dTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("InStoreNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStoreNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("ReleaseNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("NotReleaseNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));

            DataRow drCustom = null;

            if (Convert.ToInt32(page) > dt.Rows.Count / Convert.ToInt32(rows) && Convert.ToInt32(page) <= dt.Rows.Count / Convert.ToInt32(rows) + 1)
            {
                maxCount = dt.Rows.Count;
            }
            else
            {
                maxCount = Convert.ToInt32(rows) * Convert.ToInt32(page);
            }

            for (int i = Convert.ToInt32(rows) * (Convert.ToInt32(page) - 1); i < maxCount; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "vCompany"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : ((new T_User()).GetUserByUserName(dt.Rows[i][strFiledArray[j]].ToString()));
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }
                if (drCustom["wbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("HuayuAnanalyseStatistic_DS", dtCustom);

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

            string strOutputFileName = "货物出入仓统计_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
    }
}
