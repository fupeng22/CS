using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using System.Text;
using Model;
using SQLDAL;
using CS.Filter;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace CS.Controllers.customs
{
    [ErrorAttribute]
    public class Customer_CheckController : Controller
    {
        Model.M_WayBill wayBillModel = new M_WayBill();
        Model.M_SubWayBill subWayBillModel = new M_SubWayBill();
        SQLDAL.T_WayBill wayBillSql = new T_WayBill();
        SQLDAL.T_SubWayBill subWayBillSql = new T_SubWayBill();

        SQLDAL.T_WayBill tWayBill = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();

        public const string strFileds = "wbSerialNum,wbTotalNumber,wbTotalWeight,wbTotalNumber_Customize,wbTotalWeight_Customize,wbActualTotalWeight__Customize,wbCompany,wbStorageDate,wbStatus,wbImportType,swbControlFlag0,swbControlFlag1,swbControlFlag2,swbControlFlag3,wbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/CustomerCheck.rdlc";
        //
        // GET: /CustomerCheck/
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort, string txtBeginDate, string txtEndDate, string ddCompany, string txtCode)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Distinct_WayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wbID";

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

            txtBeginDate = Server.UrlDecode(txtBeginDate.ToString());
            txtEndDate = Server.UrlDecode(txtEndDate.ToString());
            ddCompany = Server.UrlDecode(ddCompany.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());

            string strWhereTemp = " ( (wbStatus = 0) OR (wbStatus = 1)) ";
            if (txtBeginDate != "" && txtEndDate != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (wbStorageDate>='" + Convert.ToDateTime(txtBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(txtEndDate).ToString("yyyyMMdd") + "')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (wbStorageDate>='" + Convert.ToDateTime(txtBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(txtEndDate).ToString("yyyyMMdd") + "')";
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and ( wbSerialNum like'%" + txtCode.ToString() + "%')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " ( wbSerialNum like'%" + txtCode.ToString() + "%')";
                }
            }

            if (ddCompany != "" && ddCompany != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and  (wbCompany like '%" + ddCompany.ToString() + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (wbCompany like '%" + ddCompany.ToString() + "%') ";
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
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbActualTotalWeight__Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbImportType", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbControlFlag0", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbControlFlag1", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbControlFlag2", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbControlFlag3", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wbTotalNumber_Customize":
                            string swbTotalNumber = "0";
                            string swbTotalActualWeight = "0";
                            string swbTotalWeight = "0";

                            DataSet dsSum = tSubWayBill.GetSubWayBillSumInfo(Convert.ToInt32(dt.Rows[i]["wbID"]));
                            if (dsSum != null)
                            {
                                DataTable dtSum = dsSum.Tables[0];
                                if (dtSum != null && dtSum.Rows.Count > 0)
                                {
                                    swbTotalNumber = dtSum.Rows[0]["swbTotalNumber"].ToString();
                                    swbTotalWeight = dtSum.Rows[0]["swbTotalWeight"].ToString();
                                    swbTotalActualWeight = dtSum.Rows[0]["swbTotalActualWeight"].ToString();
                                }
                            }
                            drCustom[strFiledArray[j]] = swbTotalNumber;
                            drCustom["wbTotalWeight_Customize"] = swbTotalWeight;
                            drCustom["wbActualTotalWeight__Customize"] = swbTotalActualWeight;
                            break;
                        case "wbTotalWeight_Customize":
                            break;
                        case "wbActualTotalWeight__Customize":
                            break;
                        case "wbStatus":
                            int needCheck = subWayBillSql.GetNeedCheckNum(Convert.ToInt32(dt.Rows[i]["wbID"]));
                            int status = Convert.ToInt32(dt.Rows[i]["wbStatus"]);
                            drCustom[strFiledArray[j]] = needCheck;
                            break;
                        case "swbControlFlag0":
                            break;
                        case "swbControlFlag1":
                            break;
                        case "swbControlFlag2":
                            break;
                        case "swbControlFlag3":
                            string strSwbControlFlag0 = "0";
                            string strSwbControlFlag1 = "0";
                            string strSwbControlFlag2 = "0";
                            string strSwbControlFlag3 = "0";
                            DataSet dsWayBillControlInfo = null;
                            DataTable dtWayBillControlInfo = null;
                            dsWayBillControlInfo = new T_WayBill().getWayBillControlInfo(dt.Rows[i]["wbID"].ToString());
                            if (dsWayBillControlInfo != null)
                            {
                                dtWayBillControlInfo = dsWayBillControlInfo.Tables[0];
                                if (dtWayBillControlInfo != null && dtWayBillControlInfo.Rows.Count > 0)
                                {
                                    strSwbControlFlag0 = dtWayBillControlInfo.Rows[0]["swbControlFlag0"].ToString();
                                    strSwbControlFlag1 = dtWayBillControlInfo.Rows[0]["swbControlFlag1"].ToString();
                                    strSwbControlFlag2 = dtWayBillControlInfo.Rows[0]["swbControlFlag2"].ToString();
                                    strSwbControlFlag3 = dtWayBillControlInfo.Rows[0]["swbControlFlag3"].ToString();
                                }
                            }
                            drCustom["swbControlFlag0"] = strSwbControlFlag0;
                            drCustom["swbControlFlag1"] = strSwbControlFlag1;
                            drCustom["swbControlFlag2"] = strSwbControlFlag2;
                            drCustom["swbControlFlag3"] = strSwbControlFlag3;
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
            ReportDataSource reportDataSource = new ReportDataSource("CustomerCheck_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtBeginDate, string txtEndDate, string ddCompany, string txtCode, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Distinct_WayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wbID";

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

            txtBeginDate = Server.UrlDecode(txtBeginDate.ToString());
            txtEndDate = Server.UrlDecode(txtEndDate.ToString());
            ddCompany = Server.UrlDecode(ddCompany.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());

            string strWhereTemp = " ( (wbStatus = 0) OR (wbStatus = 1)) ";
            if (txtBeginDate != "" && txtEndDate != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (wbStorageDate>='" + Convert.ToDateTime(txtBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(txtEndDate).ToString("yyyyMMdd") + "')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (wbStorageDate>='" + Convert.ToDateTime(txtBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(txtEndDate).ToString("yyyyMMdd") + "')";
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and ( wbSerialNum like'%" + txtCode.ToString() + "%')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " ( wbSerialNum like'%" + txtCode.ToString() + "%')";
                }
            }

            if (ddCompany != "" && ddCompany != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and  (wbCompany like '%" + ddCompany.ToString() + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (wbCompany like '%" + ddCompany.ToString() + "%') ";
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
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbActualTotalWeight__Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbImportType", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbControlFlag0", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbControlFlag1", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbControlFlag2", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbControlFlag3", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wbTotalNumber_Customize":
                            string swbTotalNumber = "0";
                            string swbTotalActualWeight = "0";
                            string swbTotalWeight = "0";

                            DataSet dsSum = tSubWayBill.GetSubWayBillSumInfo(Convert.ToInt32(dt.Rows[i]["wbID"]));
                            if (dsSum != null)
                            {
                                DataTable dtSum = dsSum.Tables[0];
                                if (dtSum != null && dtSum.Rows.Count > 0)
                                {
                                    swbTotalNumber = dtSum.Rows[0]["swbTotalNumber"].ToString();
                                    swbTotalWeight = dtSum.Rows[0]["swbTotalWeight"].ToString();
                                    swbTotalActualWeight = dtSum.Rows[0]["swbTotalActualWeight"].ToString();
                                }
                            }
                            drCustom[strFiledArray[j]] = swbTotalNumber;
                            drCustom["wbTotalWeight_Customize"] = swbTotalWeight;
                            drCustom["wbActualTotalWeight__Customize"] = swbTotalActualWeight;
                            break;
                        case "wbTotalWeight_Customize":
                            break;
                        case "wbActualTotalWeight__Customize":
                            break;
                        case "wbStatus":
                            int needCheck = subWayBillSql.GetNeedCheckNum(Convert.ToInt32(dt.Rows[i]["wbID"]));
                            int status = Convert.ToInt32(dt.Rows[i]["wbStatus"]);
                            drCustom[strFiledArray[j]] = needCheck;
                            break;
                        case "swbControlFlag0":
                            break;
                        case "swbControlFlag1":
                            break;
                        case "swbControlFlag2":
                            break;
                        case "swbControlFlag3":
                            string strSwbControlFlag0 = "0";
                            string strSwbControlFlag1 = "0";
                            string strSwbControlFlag2 = "0";
                            string strSwbControlFlag3 = "0";
                            DataSet dsWayBillControlInfo = null;
                            DataTable dtWayBillControlInfo = null;
                            dsWayBillControlInfo = new T_WayBill().getWayBillControlInfo(dt.Rows[i]["wbID"].ToString());
                            if (dsWayBillControlInfo != null)
                            {
                                dtWayBillControlInfo = dsWayBillControlInfo.Tables[0];
                                if (dtWayBillControlInfo != null && dtWayBillControlInfo.Rows.Count > 0)
                                {
                                    strSwbControlFlag0 = dtWayBillControlInfo.Rows[0]["swbControlFlag0"].ToString();
                                    strSwbControlFlag1 = dtWayBillControlInfo.Rows[0]["swbControlFlag1"].ToString();
                                    strSwbControlFlag2 = dtWayBillControlInfo.Rows[0]["swbControlFlag2"].ToString();
                                    strSwbControlFlag3 = dtWayBillControlInfo.Rows[0]["swbControlFlag3"].ToString();
                                }
                            }
                            drCustom["swbControlFlag0"] = strSwbControlFlag0;
                            drCustom["swbControlFlag1"] = strSwbControlFlag1;
                            drCustom["swbControlFlag2"] = strSwbControlFlag2;
                            drCustom["swbControlFlag3"] = strSwbControlFlag3;
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
            ReportDataSource reportDataSource = new ReportDataSource("CustomerCheck_DS", dtCustom);

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

            string strOutputFileName = "货物预检信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        /// <summary>
        /// 分页查询类
        /// </summary>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <returns></returns>

        public string GetData(string order, string page, string rows, string sort, string txtBeginDate, string txtEndDate, string ddCompany, string txtCode)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Distinct_WayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wbID";

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

            txtBeginDate = Server.UrlDecode(txtBeginDate.ToString());
            txtEndDate = Server.UrlDecode(txtEndDate.ToString());
            ddCompany = Server.UrlDecode(ddCompany.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());

            string strWhereTemp = " ( (wbStatus = 0) OR (wbStatus = 1)) ";
            if (txtBeginDate != "" && txtEndDate != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (wbStorageDate>='" + Convert.ToDateTime(txtBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(txtEndDate).ToString("yyyyMMdd") + "')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (wbStorageDate>='" + Convert.ToDateTime(txtBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(txtEndDate).ToString("yyyyMMdd") + "')";
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and ( wbSerialNum like'%" + txtCode.ToString() + "%')";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " ( wbSerialNum like'%" + txtCode.ToString() + "%')";
                }
            }

            if (ddCompany != "" && ddCompany != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and  (wbCompany like '%" + ddCompany.ToString() + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (wbCompany like '%" + ddCompany.ToString() + "%') ";
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
                        case "wbTotalNumber_Customize":
                            //string swbTotalNumber = "0";
                            //string swbTotalActualWeight = "0";
                            //string swbTotalWeight = "0";

                            //DataSet dsSum = tSubWayBill.GetSubWayBillSumInfo(Convert.ToInt32(dt.Rows[i]["wbID"]));
                            //if (dsSum != null)
                            //{
                            //    DataTable dtSum = dsSum.Tables[0];
                            //    if (dtSum != null && dtSum.Rows.Count > 0)
                            //    {
                            //        swbTotalNumber = dtSum.Rows[0]["swbTotalNumber"].ToString();
                            //        swbTotalWeight = dtSum.Rows[0]["swbTotalWeight"].ToString();
                            //        swbTotalActualWeight = dtSum.Rows[0]["swbTotalActualWeight"].ToString();
                            //    }
                            //}

                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbTotalNumber);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbTotalNumber);
                            //}

                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}公斤\",", "wbTotalWeight_Customize", swbTotalWeight);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}公斤\"", "wbTotalWeight_Customize", swbTotalWeight);
                            //}

                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}公斤\",", "wbActualTotalWeight__Customize", swbTotalActualWeight);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}公斤\"", "wbActualTotalWeight__Customize", swbTotalActualWeight);
                            //}
                            break;
                        case "wbTotalWeight_Customize":
                            break;
                        case "wbActualTotalWeight__Customize":
                            break;
                        case "wbStatus":
                            //int needCheck = subWayBillSql.GetNeedCheckNum(Convert.ToInt32(dt.Rows[i]["wbID"]));
                            //int status = Convert.ToInt32(dt.Rows[i]["wbStatus"]);

                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}${2}\",", strFiledArray[j], needCheck, status);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}${2}\"", strFiledArray[j], needCheck, status);
                            //}
                            break;
                        case "swbControlFlag0":
                            break;
                        case "swbControlFlag1":
                            break;
                        case "swbControlFlag2":
                            break;
                        case "swbControlFlag3":
                            //string strSwbControlFlag0 = "0";
                            //string strSwbControlFlag1 = "0";
                            //string strSwbControlFlag2 = "0";
                            //string strSwbControlFlag3 = "0";
                            //DataSet dsWayBillControlInfo = null;
                            //DataTable dtWayBillControlInfo = null;
                            //dsWayBillControlInfo = new T_WayBill().getWayBillControlInfo(dt.Rows[i]["wbID"].ToString());
                            //if (dsWayBillControlInfo != null)
                            //{
                            //    dtWayBillControlInfo = dsWayBillControlInfo.Tables[0];
                            //    if (dtWayBillControlInfo != null && dtWayBillControlInfo.Rows.Count > 0)
                            //    {
                            //        strSwbControlFlag0 = dtWayBillControlInfo.Rows[0]["swbControlFlag0"].ToString();
                            //        strSwbControlFlag1 = dtWayBillControlInfo.Rows[0]["swbControlFlag1"].ToString();
                            //        strSwbControlFlag2 = dtWayBillControlInfo.Rows[0]["swbControlFlag2"].ToString();
                            //        strSwbControlFlag3 = dtWayBillControlInfo.Rows[0]["swbControlFlag3"].ToString();
                            //    }
                            //}
                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "swbControlFlag0", strSwbControlFlag0);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "swbControlFlag0", strSwbControlFlag0);
                            //}
                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "swbControlFlag1", strSwbControlFlag1);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "swbControlFlag1", strSwbControlFlag1);
                            //}
                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "swbControlFlag2", strSwbControlFlag2);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "swbControlFlag2", strSwbControlFlag2);
                            //}
                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "swbControlFlag3", strSwbControlFlag3);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "swbControlFlag3", strSwbControlFlag3);
                            //}
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
        public string upDateSwbNeedCheck(int swID, string ids, int swbNeedCheck)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"提交失败，原因未知\"}";
            swID = Convert.ToInt32(Server.UrlDecode(swID.ToString()));
            ids = Server.UrlDecode(ids.ToString());
            try
            {
                if (ids == "")
                {
                    //strRet = "{\"result\":\"error\",\"message\":\"" + "没有进行过新的选择，无需提交" + "\"}";
                    strRet = "{\"result\":\"ok\",\"message\":\"" + "提交成功" + "\"}";
                }
                else
                {
                    if (ids.EndsWith("*"))
                    {
                        ids = ids.Substring(0, ids.Length - 1);
                    }

                    ids = ids.Replace("*", ",");

                    if (subWayBillSql.upDateSwbNeedCheck(swID, ids, 1, swbNeedCheck))
                    {
                        strRet = "{\"result\":\"ok\",\"message\":\"提交成功\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"提交失败，原因未知\"}";
                    }
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }
            return strRet;
        }

        [HttpPost]
        public string getNeedCheckNum(string wbID)
        {
            string strRet = "{\"result\":\"error\",\"wbStatus\":\"\",\"message\":\"获取失败，原因未知\"}";
          
            wbID = Server.UrlDecode(wbID);

            try
            {
                if (wbID == "")
                {
                    strRet = "{\"result\":\"error\",\"wbStatus\":\"\",\"message\":\"" + "获取失败,未给出总运单ID" + "\"}";
                }
                else
                {
                    int needCheck = subWayBillSql.GetNeedCheckNum(Convert.ToInt32(wbID));
                    int status = Convert.ToInt32(new T_WayBill().getWayBillInfo(wbID).Tables[0].Rows[0]["wbStatus"].ToString());
                   
                    strRet = "{\"result\":\"ok\",\"wbStatus\":\""+needCheck.ToString()+"$"+status.ToString()+"\",\"message\":\"获取成功\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"wbStatus\":\"\",\"message\":\"获取失败，原因:" + ex.Message + "\"}";
            }
            return strRet;
        }

        [HttpPost]
        public string getSwbControlFlag(string wbID)
        {
            string strRet = "{\"result\":\"error\",\"swbControlFlag0\":\"0\",\"swbControlFlag1\":\"0\",\"swbControlFlag2\":\"0\",\"swbControlFlag3\":\"0\",\"message\":\"获取失败，原因未知\"}";
            string strSwbControlFlag0 = "0";
            string strSwbControlFlag1 = "0";
            string strSwbControlFlag2 = "0";
            string strSwbControlFlag3 = "0";
            DataSet dsWayBillControlInfo = null;
            DataTable dtWayBillControlInfo = null;
            wbID = Server.UrlDecode(wbID);

            try
            {
                if (wbID == "")
                {
                    strRet = "{\"result\":\"error\",\"swbControlFlag0\":\"0\",\"swbControlFlag1\":\"0\",\"swbControlFlag2\":\"0\",\"swbControlFlag3\":\"0\",\"message\":\"" + "获取失败,未给出总运单ID" + "\"}";
                }
                else
                {
                    
                    dsWayBillControlInfo = new T_WayBill().getWayBillControlInfo(wbID);
                    if (dsWayBillControlInfo != null)
                    {
                        dtWayBillControlInfo = dsWayBillControlInfo.Tables[0];
                        if (dtWayBillControlInfo != null && dtWayBillControlInfo.Rows.Count > 0)
                        {
                            strSwbControlFlag0 = dtWayBillControlInfo.Rows[0]["swbControlFlag0"].ToString();
                            strSwbControlFlag1 = dtWayBillControlInfo.Rows[0]["swbControlFlag1"].ToString();
                            strSwbControlFlag2 = dtWayBillControlInfo.Rows[0]["swbControlFlag2"].ToString();
                            strSwbControlFlag3 = dtWayBillControlInfo.Rows[0]["swbControlFlag3"].ToString();
                        }
                    }
                    strRet = "{\"result\":\"ok\",\"swbControlFlag0\":\"" + strSwbControlFlag0 + "\",\"swbControlFlag1\":\"" + strSwbControlFlag1 + "\",\"swbControlFlag2\":\"" + strSwbControlFlag2 + "\",\"swbControlFlag3\":\"" + strSwbControlFlag3+ "\",\"message\":\"获取成功\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"swbControlFlag0\":\"0\",\"swbControlFlag1\":\"0\",\"swbControlFlag2\":\"0\",\"swbControlFlag3\":\"0\",\"message\":\"获取失败，原因:" + ex.Message + "\"}";
            }
            return strRet;
        }
    }
}
