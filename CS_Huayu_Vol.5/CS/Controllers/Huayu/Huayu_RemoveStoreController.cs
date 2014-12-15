using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using SQLDAL;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Text;
using CS.Filter;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_RemoveStoreController : Controller
    {
        Model.M_WayBill wayBillModel = new M_WayBill();
        Model.M_SubWayBill subWayBillModel = new M_SubWayBill();
        SQLDAL.T_WayBill wayBillSql = new T_WayBill();
        SQLDAL.T_SubWayBill subWayBillSql = new T_SubWayBill();

        SQLDAL.T_WayBill tWayBill = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();

        public const string strFileds = "wbSerialNum,wbTotalNumber,wbTotalWeight,wbTotalNumber_Customize,wbTotalWeight_Customize,wbActualTotalWeight__Customize,wbCompany,wbStorageDate,wbStatus,ForeStore_Count,InStore_Count,OutStore_Count,wbImportType,wbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/InOutStoreCount.rdlc";

        [HuayuRequiresLogin]
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

            string strWhereTemp = " (wbStatus=2)  ";
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

            DataSet dsInOutStoreCount = null;
            DataTable dtInOutStoreCount = null;

            dsInOutStoreCount = new T_WayBillFlow().getInOutStoreCount();
            if (dsInOutStoreCount != null)
            {
                dtInOutStoreCount = dsInOutStoreCount.Tables[0];
            }

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
            dtCustom.Columns.Add("ForeStore_Count", Type.GetType("System.String"));
            dtCustom.Columns.Add("InStore_Count", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStore_Count", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbImportType", Type.GetType("System.String"));
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
                        case "ForeStore_Count":
                            int strForeStore_Count = 0;
                            DataSet dsForeSubWayBill = null;
                            DataTable dtForeSubWayBill = null;
                            dsForeSubWayBill = new T_SubWayBill().getForeSubWayBill(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                            if (dsForeSubWayBill != null)
                            {
                                dtForeSubWayBill = dsForeSubWayBill.Tables[0];
                                if (dtForeSubWayBill != null && dtForeSubWayBill.Rows.Count > 0)
                                {
                                    strForeStore_Count = dtForeSubWayBill.Rows.Count;
                                }
                            }
                            drCustom[strFiledArray[j]] = strForeStore_Count;
                            break;
                        case "InStore_Count":
                            break;
                        case "OutStore_Count":
                            int InStore_Count = 0;
                            int OutStore_Count = 0;
                            if (dtInOutStoreCount != null && dtInOutStoreCount.Rows.Count > 0)
                            {
                                for (int k = 0; k < dtInOutStoreCount.Rows.Count; k++)
                                {
                                    if (Convert.ToInt32(dt.Rows[i]["wbID"].ToString()) == Convert.ToInt32(dtInOutStoreCount.Rows[k]["wbID"].ToString()))
                                    {
                                        InStore_Count = dtInOutStoreCount.Rows[k]["InOutStoreType1"] == DBNull.Value ? 0 : Convert.ToInt32(dtInOutStoreCount.Rows[k]["InOutStoreType1"].ToString());
                                        OutStore_Count = dtInOutStoreCount.Rows[k]["InOutStoreType3"] == DBNull.Value ? 0 : Convert.ToInt32(dtInOutStoreCount.Rows[k]["InOutStoreType3"].ToString());
                                        break;
                                    }
                                }
                            }
                            drCustom["InStore_Count"] = InStore_Count;
                            drCustom["OutStore_Count"] = OutStore_Count;
                            break;
                        case "wbStatus":
                            int needCheck = subWayBillSql.GetNeedCheckNum(Convert.ToInt32(dt.Rows[i]["wbID"]));
                            int status = Convert.ToInt32(dt.Rows[i]["wbStatus"]);
                            drCustom[strFiledArray[j]] = needCheck;
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
            ReportDataSource reportDataSource = new ReportDataSource("InOutStoreCount_DS", dtCustom);

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

            string strWhereTemp = " (wbStatus=2)  ";
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

            DataSet dsInOutStoreCount = null;
            DataTable dtInOutStoreCount = null;

            dsInOutStoreCount = new T_WayBillFlow().getInOutStoreCount();
            if (dsInOutStoreCount != null)
            {
                dtInOutStoreCount = dsInOutStoreCount.Tables[0];
            }

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
            dtCustom.Columns.Add("ForeStore_Count", Type.GetType("System.String"));
            dtCustom.Columns.Add("InStore_Count", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStore_Count", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbImportType", Type.GetType("System.String"));
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
                        case "ForeStore_Count":
                            int strForeStore_Count = 0;
                            DataSet dsForeSubWayBill = null;
                            DataTable dtForeSubWayBill = null;
                            dsForeSubWayBill = new T_SubWayBill().getForeSubWayBill(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                            if (dsForeSubWayBill != null)
                            {
                                dtForeSubWayBill = dsForeSubWayBill.Tables[0];
                                if (dtForeSubWayBill != null && dtForeSubWayBill.Rows.Count > 0)
                                {
                                    strForeStore_Count = dtForeSubWayBill.Rows.Count;
                                }
                            }
                            drCustom[strFiledArray[j]] = strForeStore_Count;
                            break;
                        case "InStore_Count":
                            break;
                        case "OutStore_Count":
                            int InStore_Count = 0;
                            int OutStore_Count = 0;
                            if (dtInOutStoreCount != null && dtInOutStoreCount.Rows.Count > 0)
                            {
                                for (int k = 0; k < dtInOutStoreCount.Rows.Count; k++)
                                {
                                    if (Convert.ToInt32(dt.Rows[i]["wbID"].ToString()) == Convert.ToInt32(dtInOutStoreCount.Rows[k]["wbID"].ToString()))
                                    {
                                        InStore_Count = dtInOutStoreCount.Rows[k]["InOutStoreType1"] == DBNull.Value ? 0 : Convert.ToInt32(dtInOutStoreCount.Rows[k]["InOutStoreType1"].ToString());
                                        OutStore_Count = dtInOutStoreCount.Rows[k]["InOutStoreType3"] == DBNull.Value ? 0 : Convert.ToInt32(dtInOutStoreCount.Rows[k]["InOutStoreType3"].ToString());
                                        break;
                                    }
                                }
                            }
                            drCustom["InStore_Count"] = InStore_Count;
                            drCustom["OutStore_Count"] = OutStore_Count;
                            break;
                        case "wbStatus":
                            int needCheck = subWayBillSql.GetNeedCheckNum(Convert.ToInt32(dt.Rows[i]["wbID"]));
                            int status = Convert.ToInt32(dt.Rows[i]["wbStatus"]);
                            drCustom[strFiledArray[j]] = needCheck;
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
            ReportDataSource reportDataSource = new ReportDataSource("InOutStoreCount_DS", dtCustom);

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

            string strWhereTemp = " (wbStatus=2)  ";
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

            //DataSet dsInOutStoreCount = null;
            //DataTable dtInOutStoreCount = null;

            //dsInOutStoreCount = new T_WayBillFlow().getInOutStoreCount();
            //if (dsInOutStoreCount!=null)
            //{
            //    dtInOutStoreCount=dsInOutStoreCount.Tables[0];
            //}

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
                        case "ForeStore_Count":
                            //int strForeStore_Count = 0;
                            //DataSet dsForeSubWayBill = null;
                            //DataTable dtForeSubWayBill = null;
                            //dsForeSubWayBill = new T_SubWayBill().getForeSubWayBill(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                            //if (dsForeSubWayBill != null)
                            //{
                            //    dtForeSubWayBill = dsForeSubWayBill.Tables[0];
                            //    if (dtForeSubWayBill != null && dtForeSubWayBill.Rows.Count > 0)
                            //    {
                            //        strForeStore_Count = dtForeSubWayBill.Rows.Count;
                            //    }
                            //}
                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], strForeStore_Count);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], strForeStore_Count);
                            //}
                            break;
                        case "InStore_Count":
                            break;
                        case "OutStore_Count":
                            //int InStore_Count = 0;
                            //int OutStore_Count = 0;
                            //if (dtInOutStoreCount != null && dtInOutStoreCount.Rows.Count > 0)
                            //{
                            //    for (int k = 0; k < dtInOutStoreCount.Rows.Count; k++)
                            //    {
                            //        if (Convert.ToInt32(dt.Rows[i]["wbID"].ToString()) == Convert.ToInt32(dtInOutStoreCount.Rows[k]["wbID"].ToString()))
                            //        {
                            //            InStore_Count = dtInOutStoreCount.Rows[k]["InOutStoreType1"] == DBNull.Value ? 0 : Convert.ToInt32(dtInOutStoreCount.Rows[k]["InOutStoreType1"].ToString());
                            //            OutStore_Count = dtInOutStoreCount.Rows[k]["InOutStoreType3"] == DBNull.Value ? 0 : Convert.ToInt32(dtInOutStoreCount.Rows[k]["InOutStoreType3"].ToString());
                            //            break;
                            //        }
                            //    }
                            //}

                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "InStore_Count", InStore_Count);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "InStore_Count", InStore_Count);
                            //}
                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "OutStore_Count", OutStore_Count);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "OutStore_Count", OutStore_Count);
                            //}
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
        public string getInOutStoreCount(string wbID)
        {
            string strRet = "{\"result\":\"error\",InStoreCount:\"0\",OutStoreCount:\"0\",\"message\":\"获取失败，原因未知\"}";
            string InStoreCount = "0";
            string OutStoreCount = "0";
            wbID = Server.UrlDecode(wbID.ToString());
            try
            {
                if (wbID == "")
                {
                    strRet = "{\"result\":\"error\",InStoreCount:\"0\",OutStoreCount:\"0\",\"message\":\"" + "获取失败,未给出总运单ID" + "\"}";
                }
                else
                {
                    DataSet dsInOutStoreCount = null;
                    DataTable dtInOutStoreCount = null;

                    dsInOutStoreCount = new T_WayBillFlow().getInOutStoreCount(Convert.ToInt32(wbID));
                    if (dsInOutStoreCount != null)
                    {
                        dtInOutStoreCount = dsInOutStoreCount.Tables[0];
                        if (dtInOutStoreCount != null && dtInOutStoreCount.Rows.Count > 0)
                        {
                            InStoreCount = dtInOutStoreCount.Rows[0]["InOutStoreType1"].ToString();
                            OutStoreCount = dtInOutStoreCount.Rows[0]["InOutStoreType3"].ToString();
                        }
                    }
                    strRet = "{\"result\":\"ok\",InStoreCount:\"" + InStoreCount + "\",OutStoreCount:\"" + OutStoreCount + "\",\"message\":\"获取成功\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",InStoreCount:\"0\",OutStoreCount:\"0\",\"message\":\"获取失败，原因:" + ex.Message + "\"}";
            }
            return strRet;
        }

        [HttpPost]
        public string getForeStoreCount(string wbID)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"获取失败，原因未知\"}";
            string ForeStoreCount = "0";
            wbID = Server.UrlDecode(wbID.ToString());
            try
            {
                if (wbID == "")
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "获取失败,未给出总运单ID" + "\"}";
                }
                else
                {
                    DataSet dsForeSubWayBill = null;
                    DataTable dtForeSubWayBill = null;

                    dsForeSubWayBill = new T_SubWayBill().getForeSubWayBill(Convert.ToInt32(wbID));
                    if (dsForeSubWayBill != null)
                    {
                        dtForeSubWayBill = dsForeSubWayBill.Tables[0];
                        if (dtForeSubWayBill != null && dtForeSubWayBill.Rows.Count > 0)
                        {
                            ForeStoreCount = dtForeSubWayBill.Rows.Count.ToString();
                        }
                    }
                    strRet = "{\"result\":\"ok\",\"message\":\"" + ForeStoreCount + "\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"获取失败，原因:" + ex.Message + "\"}";
            }
            return strRet;
        }

        [HttpPost]
        public string AllRemoveStore(string wbID)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"全部出库失败，原因未知\"}";

            wbID = Server.UrlDecode(wbID.ToString());
            try
            {
                if (wbID == "")
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "全部出库失败,未给出总运单ID" + "\"}";
                }
                else
                {
                    //DataSet dsSubWauBillID = null;
                    //DataTable dtSubWayBillID = null;

                    //DataSet dsSubWauBill = null;
                    //DataTable dtSubWayBill = null;

                    //dsSubWauBillID = new T_WayBillFlow().getAllInStoreSubWayBillID(Convert.ToInt32(wbID));
                    //if (dsSubWauBillID != null)
                    //{
                    //    dtSubWayBillID = dsSubWauBillID.Tables[0];
                    //    if (dtSubWayBillID != null && dtSubWayBillID.Rows.Count > 0)
                    //    {
                    //        for (int i = 0; i < dtSubWayBillID.Rows.Count; i++)
                    //        {
                    //            dsSubWauBill = new T_SubWayBill().getWayBillSubWayBill(dtSubWayBillID.Rows[i]["swbID"].ToString());
                    //            if (dsSubWauBill!=null)
                    //            {
                    //                dtSubWayBill=dsSubWauBill.Tables[0];
                    //                if (dtSubWayBill!=null && dtSubWayBill.Rows.Count>0)
                    //                {
                    //                    new T_WayBillFlow().UpdateStatus(dtSubWayBill.Rows[0]["wbSerialNum"].ToString(), dtSubWayBill.Rows[0]["swbSerialNum"].ToString(), 0, Session["Global_Huayu_UserName"] == null ? "" : Session["Global_Huayu_UserName"].ToString(), dtSubWayBill.Rows[0]["wbImportType"].ToString(), dtSubWayBill.Rows[0]["wbID"].ToString(), dtSubWayBill.Rows[0]["swbID"].ToString());
                    //                }
                    //            }
                                
                    //        }

                    //    }
                    //}
                    if (new T_WayBillFlow().PatchInOutStoreByWbId(wbID, 0, Session["Global_Huayu_UserName"] == null ? "" : Session["Global_Huayu_UserName"].ToString()))
                    {
                        strRet = "{\"result\":\"ok\",\"message\":\"全部出库已成功完成\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"全部出库失败\"}";
                    }
                    
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"全部出库失败，原因:" + ex.Message + "\"}";
            }
            return strRet;
        }
    }
}
