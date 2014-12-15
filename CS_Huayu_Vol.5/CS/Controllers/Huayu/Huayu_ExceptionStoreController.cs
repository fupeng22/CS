using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using System.Text;
using SQLDAL;
using CS.Filter;
using Microsoft.Reporting.WebForms;
using System.IO;
namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_ExceptionStoreController : Controller
    {
        SQLDAL.T_WayBillFlow tWayBillFlow = new T_WayBillFlow();

        public const string strFileds = "wbStorageDate,operateDate,wbCompany,Handler,wbSerialNum,ExceptionStatus,swbSerialNum,StatusDecription,ExceptionStatusDescription,ExceptionDescription,status,wbID,swbID,HandleDescription,HandleDate,Operator,ExceptionTypeDescription,ExceptionType,ExceptionOperator,ExceptionDate,WbfID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/HuayuExceptionStore.rdlc";
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
        /// 
        public string GetData(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtVoyage, string txtCode, string txtSubWayBillCode, string txtExceptionBeginD, string txtExceptionEndD, string txtExceptionType, string txtExceptionStatus)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_AllException_InOutStoreWayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "WbfID";

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
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());
            txtSubWayBillCode = Server.UrlDecode(txtSubWayBillCode.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and ((wbStorageDate is null or wbStorageDate='') or (wbStorageDate>='{0}' and wbStorageDate<='{1}')) ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" ((wbStorageDate is null or wbStorageDate='') or (wbStorageDate>='{0}' and wbStorageDate<='{1}')) ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
            }

            if (txtExceptionBeginD != "" && txtExceptionEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (substring(exceptionDate,1,10)>='{0}' and substring(exceptionDate,1,10)<='{1}') ", Convert.ToDateTime(txtExceptionBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtExceptionEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (substring(exceptionDate,1,10)>='{0}' and substring(exceptionDate,1,10)<='{1}') ", Convert.ToDateTime(txtExceptionBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtExceptionEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (wbCompany like '%{0}%') ", txtVoyage);
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
                    strWhereTemp = strWhereTemp + string.Format(" (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
            }

            if (txtExceptionType != "" && txtExceptionType != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (ExceptionType in ({0})) ", txtExceptionType);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (ExceptionType in ({0})) ", txtExceptionType);
                }
            }

            if (txtExceptionStatus != "" && txtExceptionStatus != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (ExceptionStatus={0}) ", txtExceptionStatus);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (ExceptionStatus={0}) ", txtExceptionStatus);
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
                        case "operateDate":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                            }
                            break;
                        case "ExceptionDate":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                            }
                            break;
                        case "HandleDate":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
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
        public ActionResult Print(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtVoyage, string txtCode, string txtSubWayBillCode, string txtExceptionBeginD, string txtExceptionEndD, string txtExceptionType, string txtExceptionStatus)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_AllException_InOutStoreWayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "WbfID";

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
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());
            txtSubWayBillCode = Server.UrlDecode(txtSubWayBillCode.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and ((wbStorageDate is null or wbStorageDate='') or (wbStorageDate>='{0}' and wbStorageDate<='{1}')) ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" ((wbStorageDate is null or wbStorageDate='') or (wbStorageDate>='{0}' and wbStorageDate<='{1}')) ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
            }

            if (txtExceptionBeginD != "" && txtExceptionEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (substring(exceptionDate,1,10)>='{0}' and substring(exceptionDate,1,10)<='{1}') ", Convert.ToDateTime(txtExceptionBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtExceptionEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (substring(exceptionDate,1,10)>='{0}' and substring(exceptionDate,1,10)<='{1}') ", Convert.ToDateTime(txtExceptionBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtExceptionEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (wbCompany like '%{0}%') ", txtVoyage);
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
                    strWhereTemp = strWhereTemp + string.Format(" (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
            }

            if (txtExceptionType != "" && txtExceptionType != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (ExceptionType in ({0})) ", txtExceptionType);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (ExceptionType in ({0})) ", txtExceptionType);
                }
            }

            if (txtExceptionStatus != "" && txtExceptionStatus != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (ExceptionStatus={0}) ", txtExceptionStatus);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (ExceptionStatus={0}) ", txtExceptionStatus);
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
            dtCustom.Columns.Add("operateDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("Handler", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("StatusDecription", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionStatusDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("status", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbID", Type.GetType("System.String"));
            dtCustom.Columns.Add("HandleDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("HandleDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("Operator", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionTypeDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionType", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionOperator", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("WbfID", Type.GetType("System.String"));
            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "operateDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
                        case "ExceptionDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
                        case "HandleDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }

                if (drCustom["WbfID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ExceptionStore_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtVoyage, string txtCode, string txtSubWayBillCode, string txtExceptionBeginD, string txtExceptionEndD, string txtExceptionType, string txtExceptionStatus, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_AllException_InOutStoreWayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "WbfID";

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
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());
            txtSubWayBillCode = Server.UrlDecode(txtSubWayBillCode.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and ((wbStorageDate is null or wbStorageDate='') or (wbStorageDate>='{0}' and wbStorageDate<='{1}')) ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" ((wbStorageDate is null or wbStorageDate='') or (wbStorageDate>='{0}' and wbStorageDate<='{1}')) ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
            }

            if (txtExceptionBeginD != "" && txtExceptionEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (substring(exceptionDate,1,10)>='{0}' and substring(exceptionDate,1,10)<='{1}') ", Convert.ToDateTime(txtExceptionBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtExceptionEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (substring(exceptionDate,1,10)>='{0}' and substring(exceptionDate,1,10)<='{1}') ", Convert.ToDateTime(txtExceptionBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtExceptionEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (wbCompany like '%{0}%') ", txtVoyage);
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
                    strWhereTemp = strWhereTemp + string.Format(" (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
            }

            if (txtExceptionType != "" && txtExceptionType != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (ExceptionType in ({0})) ", txtExceptionType);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (ExceptionType in ({0})) ", txtExceptionType);
                }
            }

            if (txtExceptionStatus != "" && txtExceptionStatus != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (ExceptionStatus={0}) ", txtExceptionStatus);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (ExceptionStatus={0}) ", txtExceptionStatus);
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
            dtCustom.Columns.Add("operateDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("Handler", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("StatusDecription", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionStatusDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("status", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbID", Type.GetType("System.String"));
            dtCustom.Columns.Add("HandleDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("HandleDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("Operator", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionTypeDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionType", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionOperator", Type.GetType("System.String"));
            dtCustom.Columns.Add("ExceptionDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("WbfID", Type.GetType("System.String"));
            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "operateDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
                        case "ExceptionDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
                        case "HandleDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }

                if (drCustom["WbfID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ExceptionStore_DS", dtCustom);

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

            string strOutputFileName = "异常件信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        
        public ActionResult ExceptionHandle(string ids)
        {
            ViewData["ExceptionHandle_Ids"] = ids;
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
        public string GetExceptionHandleData(string order, string page, string rows, string sort, string ids)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_AllException_InOutStoreWayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "swbID";

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

            ids = Server.UrlDecode(ids.ToString());

            string strWhereTemp = "";
            if (ids != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (WbfID in ({0}))", ids);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (WbfID in ({0}))", ids);
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
                        case "operateDate":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
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
        public string UpdateHandleStatus(string ids, string ExceptionStatus, string HandleDescription, string strOperator)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"提交失败，原因未知\"}";
            ids = Server.UrlDecode(ids.ToString());
            ExceptionStatus = Server.UrlDecode(ExceptionStatus.ToString());
            HandleDescription = Server.UrlDecode(HandleDescription.ToString());
            strOperator = Server.UrlDecode(strOperator.ToString());

            try
            {

                if (tWayBillFlow.UpdateHandleStatus(ids, ExceptionStatus, HandleDescription, strOperator))
                {
                    strRet = "{\"result\":\"ok\",\"message\":\"" + "提交成功" + "\"}";
                }
                else
                {
                    strRet = "{\"result\":\"ok\",\"message\":\"" + "提交失败" + "\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }

        
        [HttpPost]
        public string PatchInOutStore(string ids, string iType)
        {
            string strDesc = "";
            switch (iType)
            {
                case "1":
                    strDesc = "入仓";
                    break;
                case "3":
                    strDesc = "出仓";
                    break;
                default:
                    break;
            }
            string strRet = "{\"result\":\"error\",\"message\":\"" + strDesc + "失败，原因未知\"}";

            try
            {
                new T_WayBillFlow_Exception().AddExceptionRecord(ids);
                tWayBillFlow.PatchInOutStore(ids, Convert.ToInt32(iType), Session["Global_Huayu_UserName"] == null ? "" : Session["Global_Huayu_UserName"].ToString());

                strRet = "{\"result\":\"ok\",\"message\":\"" + strDesc + "完成\"}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }
    }
}
