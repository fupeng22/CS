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
using System.Configuration;
using System.Net.Mail;
using SQLDAL;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_WayBillDailyReportController : Controller
    {
        public const string strFileds = @"wbStorageDate,wbCompany,wbSerialNum,wbVoyage,wbSRport,wbrCode,CustomsCategory,wbTotalNumber,wbTotalWeight,InStoreDate,OutStoreDate,WayBillActualWeight,OperateFee,PickGoodsFee,KeepGoodsFee,ShiftGoodsFee,RejectGoodsFee,CollectionKeepGoodsFee,ActualPay,PayMethod,Receipt,ShouldPayUnit,shouldPay,ReceptMethod,SalesMan,mMemo,wbrID,wbr_wbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/Huayu_WayBillDailyReport.rdlc";

        public string STR_TIMEOUT = ConfigurationManager.AppSettings["MaxTimeOut"];
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
        public string GetData(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtCode, string txtWbrCode)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBillDailyReport";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wbrID";

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

            txtBeginD = Server.UrlDecode(txtBeginD.ToString());
            txtEndD = Server.UrlDecode(txtEndD.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());
            txtWbrCode = Server.UrlDecode(txtWbrCode.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtWbrCode!="")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbrCode like '%{0}%') ", txtWbrCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbrCode like '%{0}%') ", txtWbrCode);
                }
            }

            param[6].Value = strWhereTemp;

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
        public ActionResult Print(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtCode, string txtWbrCode)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBillDailyReport";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wbrID";

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

            txtBeginD = Server.UrlDecode(txtBeginD.ToString());
            txtEndD = Server.UrlDecode(txtEndD.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());
            txtWbrCode = Server.UrlDecode(txtWbrCode.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtWbrCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbrCode like '%{0}%') ", txtWbrCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbrCode like '%{0}%') ", txtWbrCode);
                }
            }

            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbVoyage", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSRport", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbrCode", Type.GetType("System.String"));
            dtCustom.Columns.Add("CustomsCategory", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("InStoreDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStoreDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("WayBillActualWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("OperateFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("PickGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("KeepGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("ShiftGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("RejectGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("CollectionKeepGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("ActualPay", Type.GetType("System.String"));
            dtCustom.Columns.Add("PayMethod", Type.GetType("System.String"));
            dtCustom.Columns.Add("Receipt", Type.GetType("System.String"));
            dtCustom.Columns.Add("ShouldPayUnit", Type.GetType("System.String"));
            dtCustom.Columns.Add("shouldPay", Type.GetType("System.String"));
            dtCustom.Columns.Add("ReceptMethod", Type.GetType("System.String"));
            dtCustom.Columns.Add("SalesMan", Type.GetType("System.String"));
            dtCustom.Columns.Add("mMemo", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbrID", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbr_wbID", Type.GetType("System.String"));

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
                if (drCustom["wbrID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("Huayu_WayBillDailyReport_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtCode, string txtWbrCode, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBillDailyReport";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wbrID";

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

            txtBeginD = Server.UrlDecode(txtBeginD.ToString());
            txtEndD = Server.UrlDecode(txtEndD.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());
            txtWbrCode = Server.UrlDecode(txtWbrCode.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtWbrCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbrCode like '%{0}%') ", txtWbrCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbrCode like '%{0}%') ", txtWbrCode);
                }
            }

            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbVoyage", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSRport", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbrCode", Type.GetType("System.String"));
            dtCustom.Columns.Add("CustomsCategory", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("InStoreDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStoreDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("WayBillActualWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("OperateFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("PickGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("KeepGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("ShiftGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("RejectGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("CollectionKeepGoodsFee", Type.GetType("System.String"));
            dtCustom.Columns.Add("ActualPay", Type.GetType("System.String"));
            dtCustom.Columns.Add("PayMethod", Type.GetType("System.String"));
            dtCustom.Columns.Add("Receipt", Type.GetType("System.String"));
            dtCustom.Columns.Add("ShouldPayUnit", Type.GetType("System.String"));
            dtCustom.Columns.Add("shouldPay", Type.GetType("System.String"));
            dtCustom.Columns.Add("ReceptMethod", Type.GetType("System.String"));
            dtCustom.Columns.Add("SalesMan", Type.GetType("System.String"));
            dtCustom.Columns.Add("mMemo", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbrID", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbr_wbID", Type.GetType("System.String"));

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
                if (drCustom["wbrID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("Huayu_WayBillDailyReport_DS", dtCustom);

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

            string strOutputFileName = "销售日报表信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
        public string SendMail_PDF(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtCode, string txtWbrCode,string seleReciever)
        {
            string strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因未知\"}";

            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBillDailyReport";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wbrID";

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

            try
            {
                txtBeginD = Server.UrlDecode(txtBeginD.ToString());
                txtEndD = Server.UrlDecode(txtEndD.ToString());
                txtCode = Server.UrlDecode(txtCode.ToString());
                txtWbrCode = Server.UrlDecode(txtWbrCode.ToString());
                seleReciever = Server.UrlDecode(seleReciever.ToString());

                string strWhereTemp = "";
                if (txtBeginD != "" && txtEndD != "")
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + string.Format(" and (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + string.Format("  (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                    }
                }

                if (txtCode != "")
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + string.Format("  (wbSerialNum like '%{0}%') ", txtCode);
                    }
                }

                if (txtWbrCode != "")
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + string.Format(" and (wbrCode like '%{0}%') ", txtWbrCode);
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + string.Format("  (wbrCode like '%{0}%') ", txtWbrCode);
                    }
                }

                param[6].Value = strWhereTemp;

                param[7] = new SqlParameter();
                param[7].SqlDbType = SqlDbType.Int;
                param[7].ParameterName = "@RecordCount";
                param[7].Direction = ParameterDirection.Output;

                DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
                DataTable dt = ds.Tables["result"];

                DataTable dtCustom = new DataTable();
                dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbVoyage", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbSRport", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbrCode", Type.GetType("System.String"));
                dtCustom.Columns.Add("CustomsCategory", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
                dtCustom.Columns.Add("InStoreDate", Type.GetType("System.String"));
                dtCustom.Columns.Add("OutStoreDate", Type.GetType("System.String"));
                dtCustom.Columns.Add("WayBillActualWeight", Type.GetType("System.String"));
                dtCustom.Columns.Add("OperateFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("PickGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("KeepGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("ShiftGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("RejectGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("CollectionKeepGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("ActualPay", Type.GetType("System.String"));
                dtCustom.Columns.Add("PayMethod", Type.GetType("System.String"));
                dtCustom.Columns.Add("Receipt", Type.GetType("System.String"));
                dtCustom.Columns.Add("ShouldPayUnit", Type.GetType("System.String"));
                dtCustom.Columns.Add("shouldPay", Type.GetType("System.String"));
                dtCustom.Columns.Add("ReceptMethod", Type.GetType("System.String"));
                dtCustom.Columns.Add("SalesMan", Type.GetType("System.String"));
                dtCustom.Columns.Add("mMemo", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbrID", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbr_wbID", Type.GetType("System.String"));

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
                    if (drCustom["wbrID"].ToString() != "")
                    {
                        dtCustom.Rows.Add(drCustom);
                    }
                }
                dt = null;
                LocalReport localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
                ReportDataSource reportDataSource = new ReportDataSource("Huayu_WayBillDailyReport_DS", dtCustom);

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

                try
                {
                    string FileName = Server.MapPath("~/Temp/PDF/") + "销售日报表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    using (FileStream wf = new FileStream(FileName, FileMode.Create))
                    {
                        wf.Write(renderedBytes, 0, renderedBytes.Length);
                        wf.Flush();
                        wf.Close();

                        if (NetMail_SendMail(FileName, seleReciever, "0"))
                        {
                            strResult = "{\"result\":\"ok\",\"message\":\"发送邮件成功\"}";
                        }
                        else
                        {
                            strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败\"}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因:" + ex.Message + "\"}";
                }
            }
            catch (Exception ex)
            {
                strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因:" + ex.Message + "\"}";
            }

            return strResult;
        }


        [HttpPost]
        public string SendMail_Excel(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtCode, string txtWbrCode, string seleReciever, string browserType)
        {
            string strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因未知\"}";

            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBillDailyReport";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wbrID";

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
            try
            {
                txtBeginD = Server.UrlDecode(txtBeginD.ToString());
                txtEndD = Server.UrlDecode(txtEndD.ToString());
                txtCode = Server.UrlDecode(txtCode.ToString());
                txtWbrCode = Server.UrlDecode(txtWbrCode.ToString());
                seleReciever = Server.UrlDecode(seleReciever.ToString());

                string strWhereTemp = "";
                if (txtBeginD != "" && txtEndD != "")
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + string.Format(" and (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + string.Format("  (OutStoreDate>='{0}' and OutStoreDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                    }
                }

                if (txtCode != "")
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + string.Format("  (wbSerialNum like '%{0}%') ", txtCode);
                    }
                }

                if (txtWbrCode != "")
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + string.Format(" and (wbrCode like '%{0}%') ", txtWbrCode);
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + string.Format("  (wbrCode like '%{0}%') ", txtWbrCode);
                    }
                }

                param[6].Value = strWhereTemp;

                param[7] = new SqlParameter();
                param[7].SqlDbType = SqlDbType.Int;
                param[7].ParameterName = "@RecordCount";
                param[7].Direction = ParameterDirection.Output;

                DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
                DataTable dt = ds.Tables["result"];

                DataTable dtCustom = new DataTable();
                dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbVoyage", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbSRport", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbrCode", Type.GetType("System.String"));
                dtCustom.Columns.Add("CustomsCategory", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
                dtCustom.Columns.Add("InStoreDate", Type.GetType("System.String"));
                dtCustom.Columns.Add("OutStoreDate", Type.GetType("System.String"));
                dtCustom.Columns.Add("WayBillActualWeight", Type.GetType("System.String"));
                dtCustom.Columns.Add("OperateFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("PickGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("KeepGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("ShiftGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("RejectGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("CollectionKeepGoodsFee", Type.GetType("System.String"));
                dtCustom.Columns.Add("ActualPay", Type.GetType("System.String"));
                dtCustom.Columns.Add("PayMethod", Type.GetType("System.String"));
                dtCustom.Columns.Add("Receipt", Type.GetType("System.String"));
                dtCustom.Columns.Add("ShouldPayUnit", Type.GetType("System.String"));
                dtCustom.Columns.Add("shouldPay", Type.GetType("System.String"));
                dtCustom.Columns.Add("ReceptMethod", Type.GetType("System.String"));
                dtCustom.Columns.Add("SalesMan", Type.GetType("System.String"));
                dtCustom.Columns.Add("mMemo", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbrID", Type.GetType("System.String"));
                dtCustom.Columns.Add("wbr_wbID", Type.GetType("System.String"));

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
                    if (drCustom["wbrID"].ToString() != "")
                    {
                        dtCustom.Rows.Add(drCustom);
                    }
                }
                dt = null;
                LocalReport localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
                ReportDataSource reportDataSource = new ReportDataSource("Huayu_WayBillDailyReport_DS", dtCustom);

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

                string strOutputFileName = "销售日报表信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

                try
                {
                    string FileName = Server.MapPath("~/Temp/PDF/") + strOutputFileName;
                    using (FileStream wf = new FileStream(FileName, FileMode.Create))
                    {
                        wf.Write(bytes, 0, bytes.Length);
                        wf.Flush();
                        wf.Close();
                        if (NetMail_SendMail(FileName, seleReciever, "1"))
                        {
                            strResult = "{\"result\":\"ok\",\"message\":\"发送邮件成功\"}";
                        }
                        else
                        {
                            strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败\"}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因:" + ex.Message + "\"}";
                }


            }
            catch (Exception ex)
            {
                strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因:" + ex.Message + "\"}";
            }
            return strResult;
        }


        private Boolean NetMail_SendMail(string AttachmentFileName, string strReciever, string FileType)
        {
            Boolean bOK = false;
            SmtpClient client = new SmtpClient();
            MailAddress mailTo = null;
            MailMessage mail = null;
            string strSubJect = "";
            string strBody = "";

            string STR_SENDER_SMTP = "";
            string STR_SENDER_USERMAIL = "";
            string STR_SENDER_USERPWD = "";
            string STR_SENDER_USERNAME = "";

            STR_SENDER_SMTP = new T_EmailManagement().GetEmailContent(EmailType.EmailSenderSMTP);
            STR_SENDER_USERMAIL = new T_EmailManagement().GetEmailContent(EmailType.EmailSenderUserName);
            STR_SENDER_USERPWD =Util.CryptographyTool.Decrypt( new T_EmailManagement().GetEmailContent(EmailType.EmailSenderPwd),"HuayuTAT");
            STR_SENDER_USERNAME = STR_SENDER_USERMAIL;

            client.Host = STR_SENDER_SMTP;
            client.Credentials = new System.Net.NetworkCredential(STR_SENDER_USERMAIL, STR_SENDER_USERPWD);
            try
            {
                mail = new MailMessage();
                mail.From = new MailAddress(STR_SENDER_USERMAIL, STR_SENDER_USERNAME, Encoding.GetEncoding(936));
                mailTo = new MailAddress(strReciever, "", Encoding.GetEncoding(936));
                mail.To.Add(mailTo);
                //mail.CC.Add(STR_CARBONCODY);抄送
                switch (FileType)
                {
                    case "0":
                        mail.Attachments.Add(new Attachment(AttachmentFileName, System.Net.Mime.MediaTypeNames.Application.Pdf));
                        break;
                    case "1":
                        mail.Attachments.Add(new Attachment(AttachmentFileName, System.Net.Mime.MediaTypeNames.Application.Octet));
                        break;
                    default:
                        break;
                }

                strSubJect = new T_EmailManagement().GetEmailContent(EmailType.EmailSubject_SendDialyReport).Replace("[Date]", DateTime.Now.ToString("yyyyMMddHHmmss")).Replace("【Date]", DateTime.Now.ToString("yyyyMMddHHmmss")).Replace("[Date】", DateTime.Now.ToString("yyyyMMddHHmmss")).Replace("【Date】", DateTime.Now.ToString("yyyyMMddHHmmss"));
                strBody = new T_EmailManagement().GetEmailContent(EmailType.EmailBody_SendDialyReport);

                mail.Subject = strSubJect;
                mail.Body = strBody;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                client.Timeout = Convert.ToInt32(STR_TIMEOUT);
                client.Send(mail);

                bOK = true;
            }
            catch (Exception ex)
            {
                
            }

            return bOK;
        }
    }
}
