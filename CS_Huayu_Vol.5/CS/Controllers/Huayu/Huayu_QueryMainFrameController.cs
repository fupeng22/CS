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
using Microsoft.Reporting.WebForms;
using System.IO;
using CS.Filter;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_QueryMainFrameController : Controller
    {
        SQLDAL.T_WayBillFlow tWayBillFlow = new T_WayBillFlow();
        SQLDAL.T_WayBill tWayBill = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();

        public const string strFileds = "wbCompany,wbSerialNum,wbTotalNumbe,wbTotalWeight,InStoreCount,OutStoreCount,NotInStoreCount,RejectNum,StoreCount,wbID";
        public const string strFiledsSubWayBill = "swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,InOutStoreDate,FinalStatusDecription,InOutStoreOperator,swbNeedCheckDescription,swbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/ShowSubWayBillDetail.rdlc";
        public const string STR_REPORT_WAYBIILL_URL = "~/Content/Reports/ShowWayBillInfo.rdlc";

        [HuayuRequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult PrintSubWayBillInfo(string order, string page, string rows, string sort, string txtWBID, string strSwbSerialNum, string InOutType)
        {
            string strWBSerialNum = "";
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_SubWayBill_WayBillFlow";

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

            txtWBID = Server.UrlDecode(txtWBID.ToString());
            strSwbSerialNum = Server.UrlDecode(strSwbSerialNum.ToString());
            InOutType = Server.UrlDecode(InOutType.ToString());

            string strWhereTemp = "";
            switch (InOutType)
            {
                case "1"://查看已入库记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (status=1) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (status=1) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                case "3"://查看已出库记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (IsOutStore=3) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (IsOutStore=3) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                case "-1"://查看总库存记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (InOutStoreType=1) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (InOutStoreType=1) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                case "99"://查看未入库记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (InOutStoreType is null) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (InOutStoreType is null) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                default://查看所有记录
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }

                    if (strSwbSerialNum.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (swbSerialNum='" + strSwbSerialNum.ToString() + "') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (swbSerialNum='" + strSwbSerialNum.ToString() + "') ";
                        }
                    }
                    break;
            }
            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("InOutStoreDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("FinalStatusDecription", Type.GetType("System.String"));
            dtCustom.Columns.Add("InOutStoreOperator", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNeedCheckDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                strWBSerialNum = dt.Rows[i]["wbSerialNum"].ToString();

                string[] strFiledArray = strFiledsSubWayBill.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "InOutStoreDate":
                            string InOutStoreDate = "";
                            switch (InOutType)
                            {
                                case "1":
                                    InOutStoreDate = dt.Rows[i]["operateDate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i]["operateDate"].ToString()).ToString("yyyy-MM-dd");
                                    break;
                                case "3":
                                    InOutStoreDate = dt.Rows[i]["OutStoreDate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i]["OutStoreDate"].ToString()).ToString("yyyy-MM-dd");
                                    break;
                                case "-1":
                                    InOutStoreDate = dt.Rows[i]["InOutStoreDate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i]["InOutStoreDate"].ToString()).ToString("yyyy-MM-dd");
                                    break;
                                default:
                                    InOutStoreDate = dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd");
                                    break;
                            }
                            drCustom[strFiledArray[j]] = InOutStoreDate;
                            break;
                        case "FinalStatusDecription":
                            string FinalStatusDecription = "";
                            switch (InOutType)
                            {
                                case "1":
                                    FinalStatusDecription = dt.Rows[i]["StatusDecription"] == DBNull.Value ? "" : dt.Rows[i]["StatusDecription"].ToString();
                                    break;
                                case "3":
                                    FinalStatusDecription = dt.Rows[i]["IsOutStoreStatusDecription"] == DBNull.Value ? "" : dt.Rows[i]["IsOutStoreStatusDecription"].ToString();
                                    break;
                                case "-1":
                                    FinalStatusDecription = dt.Rows[i]["FinalStatusDecription"] == DBNull.Value ? "" : dt.Rows[i]["FinalStatusDecription"].ToString();
                                    break;
                                default:
                                    FinalStatusDecription = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                                    break;
                            }
                            drCustom[strFiledArray[j]] = FinalStatusDecription;
                            break;
                        case "InOutStoreOperator":
                            string InOutStoreOperator = "";
                            switch (InOutType)
                            {
                                case "1":
                                    InOutStoreOperator = dt.Rows[i]["Operator"] == DBNull.Value ? "" : dt.Rows[i]["Operator"].ToString();
                                    break;
                                case "3":
                                    InOutStoreOperator = dt.Rows[i]["OutStoreOperator"] == DBNull.Value ? "" : dt.Rows[i]["OutStoreOperator"].ToString();
                                    break;
                                case "-1":
                                    InOutStoreOperator = dt.Rows[i]["InOutStoreOperator"] == DBNull.Value ? "" : dt.Rows[i]["InOutStoreOperator"].ToString();
                                    break;
                                default:
                                    InOutStoreOperator = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                                    break;
                            }
                            drCustom[strFiledArray[j]] = InOutStoreOperator;
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }
                if (drCustom["swbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ShowSubWayBillDetail_DS", dtCustom);
            ReportParameter rptParaReportTitle = new ReportParameter("ReportTitle", strWBSerialNum + "子运单明细信息");
            localReport.SetParameters(new ReportParameter[] { rptParaReportTitle });
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
        public ActionResult ExcelSubWayBillInfo(string order, string page, string rows, string sort, string txtWBID, string strSwbSerialNum, string browserType, string InOutType)
        {
            string strWBSerialNum = "";
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_SubWayBill_WayBillFlow";

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

            txtWBID = Server.UrlDecode(txtWBID.ToString());
            strSwbSerialNum = Server.UrlDecode(strSwbSerialNum.ToString());
            InOutType = Server.UrlDecode(InOutType.ToString());

            string strWhereTemp = "";
            switch (InOutType)
            {
                case "1"://查看已入库记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (status=1) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (status=1) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                case "3"://查看已出库记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (IsOutStore=3) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (IsOutStore=3) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                case "-1"://查看总库存记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (InOutStoreType=1) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (InOutStoreType=1) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                case "99"://查看未入库记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (InOutStoreType is null) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (InOutStoreType is null) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                default://查看所有记录
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }

                    if (strSwbSerialNum.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (swbSerialNum='" + strSwbSerialNum.ToString() + "') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (swbSerialNum='" + strSwbSerialNum.ToString() + "') ";
                        }
                    }
                    break;
            }
            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("InOutStoreDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("FinalStatusDecription", Type.GetType("System.String"));
            dtCustom.Columns.Add("InOutStoreOperator", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNeedCheckDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                strWBSerialNum = dt.Rows[i]["wbSerialNum"].ToString();

                string[] strFiledArray = strFiledsSubWayBill.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "InOutStoreDate":
                            string InOutStoreDate = "";
                            switch (InOutType)
                            {
                                case "1":
                                    InOutStoreDate = dt.Rows[i]["operateDate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i]["operateDate"].ToString()).ToString("yyyy-MM-dd");
                                    break;
                                case "3":
                                    InOutStoreDate = dt.Rows[i]["OutStoreDate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i]["OutStoreDate"].ToString()).ToString("yyyy-MM-dd");
                                    break;
                                case "-1":
                                    InOutStoreDate = dt.Rows[i]["InOutStoreDate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i]["InOutStoreDate"].ToString()).ToString("yyyy-MM-dd");
                                    break;
                                default:
                                    InOutStoreDate = dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd");
                                    break;
                            }
                            drCustom[strFiledArray[j]] = InOutStoreDate;
                            break;
                        case "FinalStatusDecription":
                            string FinalStatusDecription = "";
                            switch (InOutType)
                            {
                                case "1":
                                    FinalStatusDecription = dt.Rows[i]["StatusDecription"] == DBNull.Value ? "" : dt.Rows[i]["StatusDecription"].ToString();
                                    break;
                                case "3":
                                    FinalStatusDecription = dt.Rows[i]["IsOutStoreStatusDecription"] == DBNull.Value ? "" : dt.Rows[i]["IsOutStoreStatusDecription"].ToString();
                                    break;
                                case "-1":
                                    FinalStatusDecription = dt.Rows[i]["FinalStatusDecription"] == DBNull.Value ? "" : dt.Rows[i]["FinalStatusDecription"].ToString();
                                    break;
                                default:
                                    FinalStatusDecription = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                                    break;
                            }
                            drCustom[strFiledArray[j]] = FinalStatusDecription;
                            break;
                        case "InOutStoreOperator":
                            string InOutStoreOperator = "";
                            switch (InOutType)
                            {
                                case "1":
                                    InOutStoreOperator = dt.Rows[i]["Operator"] == DBNull.Value ? "" : dt.Rows[i]["Operator"].ToString();
                                    break;
                                case "3":
                                    InOutStoreOperator = dt.Rows[i]["OutStoreOperator"] == DBNull.Value ? "" : dt.Rows[i]["OutStoreOperator"].ToString();
                                    break;
                                case "-1":
                                    InOutStoreOperator = dt.Rows[i]["InOutStoreOperator"] == DBNull.Value ? "" : dt.Rows[i]["InOutStoreOperator"].ToString();
                                    break;
                                default:
                                    InOutStoreOperator = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                                    break;
                            }
                            drCustom[strFiledArray[j]] = InOutStoreOperator;
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }
                if (drCustom["swbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ShowSubWayBillDetail_DS", dtCustom);
            ReportParameter rptParaReportTitle = new ReportParameter("ReportTitle", strWBSerialNum + "子运单明细信息");
            localReport.SetParameters(new ReportParameter[] { rptParaReportTitle });
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

            string strOutputFileName = strWBSerialNum + "子运单明细信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
        public string GetData(string order, string page, string rows, string sort, string inputBeginDate, string inputEndDate, string txtCompany, string txtWBSerialNum, string txtSWBSerialNum, string txtSWBNeedCheck, string InOutStoreType)
        {
            double sumTotalNumber = 0;
            double sumTotalWeight = 0;
            double sumInStoreCount = 0;
            double sumOutStoreCount = 0;
            double sumStoreCount = 0;
            double sumNotInStore = 0;
            double sumRejectNum = 0;

            StringBuilder sb = new StringBuilder("");
            inputBeginDate = Server.UrlDecode(inputBeginDate);
            inputEndDate = Server.UrlDecode(inputEndDate);
            txtCompany = Server.UrlDecode(txtCompany);
            txtWBSerialNum = Server.UrlDecode(txtWBSerialNum);
            txtSWBSerialNum = Server.UrlDecode(txtSWBSerialNum);
            txtSWBNeedCheck = Server.UrlDecode(txtSWBNeedCheck);
            InOutStoreType = Server.UrlDecode(InOutStoreType);
            if (txtSWBSerialNum != "")
            {
                DataSet ds = tWayBillFlow.getWayBill_SubWayBillInfo(txtSWBSerialNum);
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"total\":{0}", dt.Rows.Count);
                        sb.Append(",\"rows\":[");

                        int maxCount = -1;

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

                            DataSet dsWayBillInfo = tWayBill.getWayBillInfo(dt.Rows[i]["wbID"].ToString());
                            string wbSerialNum = "";
                            string wbCompany = "";
                            string swbTotalNum = "";
                            string swbTotalWeight = "";
                            string swbInStoreCount = "";
                            string swbOutStoreCount = "";
                            string swbStoreCount = "";
                            string swbNotInStoreCount = "";
                            string swbRejectNum = "";

                            if (dsWayBillInfo != null)
                            {
                                DataTable dtWayBillInfo = dsWayBillInfo.Tables[0];
                                if (dtWayBillInfo != null && dtWayBillInfo.Rows.Count > 0)
                                {
                                    wbSerialNum = dtWayBillInfo.Rows[0]["wbSerialNum"].ToString();
                                    wbCompany = dtWayBillInfo.Rows[0]["wbCompany"].ToString();
                                }
                            }

                            SqlParameter[] parameters = {
                      
                                new SqlParameter("@vCompany_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageBeginDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageEndDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vSerialNum_input",SqlDbType.NVarChar)
                                                        };

                            parameters[0].Value = "";
                            parameters[1].Value = "";
                            parameters[2].Value = "";
                            parameters[3].Value = wbSerialNum;
                            try
                            {
                                DataSet dsAnaysic = SqlServerHelper.RunProcedure("sp_WayBill_AnanalyseStatistic", parameters, "Default");
                                DataTable dtAnaysic = dsAnaysic.Tables["Default"];
                                swbInStoreCount = dtAnaysic.Rows[0]["InStoreNum"].ToString();
                                swbOutStoreCount = dtAnaysic.Rows[0]["OutStoreNum"].ToString();
                                swbStoreCount = dtAnaysic.Rows[0]["StoreNum"].ToString();
                                swbNotInStoreCount = dtAnaysic.Rows[0]["NotInStoreNum"].ToString();
                                swbRejectNum = dtAnaysic.Rows[0]["RejectNum"].ToString();

                                sumInStoreCount = sumInStoreCount + Convert.ToDouble(swbInStoreCount);
                                sumOutStoreCount = sumOutStoreCount + Convert.ToDouble(swbOutStoreCount);
                                sumStoreCount = sumStoreCount + Convert.ToDouble(swbStoreCount);
                                sumNotInStore = sumNotInStore + Convert.ToDouble(swbNotInStoreCount);
                                sumRejectNum = sumRejectNum + Convert.ToDouble(swbRejectNum);
                            }
                            catch (Exception ex)
                            {

                            }

                            DataSet dsSubWayBillInfo = tSubWayBill.GetSubWayBillSumInfo(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                            if (dsSubWayBillInfo != null)
                            {
                                DataTable dtSubWayBillInfo = dsSubWayBillInfo.Tables[0];
                                if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
                                {
                                    swbTotalNum = dtSubWayBillInfo.Rows[0]["swbTotalNumber"].ToString();
                                    swbTotalWeight = dtSubWayBillInfo.Rows[0]["swbTotalWeight"].ToString();

                                    sumTotalNumber = sumTotalNumber + Convert.ToDouble(swbTotalNum);
                                    sumTotalWeight = sumTotalWeight + Convert.ToDouble(swbTotalWeight);
                                }
                            }

                            //"wbCompany,wbSerialNum,wbTotalNumbe,wbTotalWeight,InStoreCount,OutStoreCount,wbID";
                            string[] strFiledArray = strFileds.Split(',');
                            for (int j = 0; j < strFiledArray.Length; j++)
                            {
                                switch (strFiledArray[j])
                                {
                                    case "wbCompany":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], wbCompany);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], wbCompany);
                                        }
                                        break;
                                    case "wbSerialNum"://格式化公司(保存的是用户名，取出公司名)
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], wbSerialNum);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], wbSerialNum);
                                        }
                                        break;
                                    case "wbTotalNumbe":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbTotalNum);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbTotalNum);
                                        }
                                        break;
                                    case "InStoreCount":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbInStoreCount);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbInStoreCount);
                                        }
                                        break;
                                    case "OutStoreCount":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbOutStoreCount);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbOutStoreCount);
                                        }
                                        break;
                                    case "StoreCount":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbStoreCount);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbStoreCount);
                                        }
                                        break;
                                    case "NotInStoreCount":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbNotInStoreCount);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbNotInStoreCount);
                                        }
                                        break;
                                    case "RejectNum":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbRejectNum);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbRejectNum);
                                        }
                                        break;
                                    case "wbTotalWeight":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbTotalWeight);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbTotalWeight);
                                        }
                                        break;
                                    case "wbID":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i]["wbID"].ToString());
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i]["wbID"].ToString());
                                        }
                                        break;
                                    default:
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "");
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "");
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
                    }
                    else
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"total\":{0}", dt.Rows.Count);
                        sb.Append(",\"rows\":[");
                    }
                }

                if (sb.ToString().EndsWith(","))
                {
                    sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
                }
                sb.Append("]");
                sb.Append(",\"footer\":[{\"wbCompany\":\"<font style='color:red;font-weight:bold'>当前页合计</font>\",\"wbTotalNumbe\":\"wbTotalNumbe总运单件数:<font style='color:red;font-weight:bold'>" + sumTotalNumber.ToString() + "</font>\",\"InStoreCount\":" + "\"InStoreCount已入库分单数:<font style='color:red;font-weight:bold'>" + sumInStoreCount.ToString() + "</font>\",\"wbTotalWeight\":" + "\"总运单重量:<font style='color:red;font-weight:bold'>" + sumTotalWeight.ToString() + "</font>\",\"OutStoreCount\":" + "\"OutStoreCount已出库分单数:<font style='color:red;font-weight:bold'>" + sumOutStoreCount.ToString() + "</font>\",\"StoreCount\":" + "\"StoreCount库存件数:<font style='color:red;font-weight:bold'>" + sumStoreCount.ToString() + "</font>\",\"NotInStoreCount\":" + "\"NotInStoreCount未入库件数:<font style='color:red;font-weight:bold'>" + sumNotInStore.ToString() + "</font>\",\"RejectNum\":" + "\"RejectNum退运件数:<font style='color:red;font-weight:bold'>" + sumRejectNum.ToString() + "</font>\"}]");
                //sb.Append(",\"footer\":[{\"wbCompany\":\"<font style='color:red;font-weight:bold'>当前页合计</font>\",\"wbTotalNumbe\":\"wbTotalNumbe总运单件数:<font style='color:red;font-weight:bold'>" + sumTotalNumber.ToString() + "</font>\",\"InStoreCount\":" + "\"InStoreCount已入库分单数:<font style='color:red;font-weight:bold'>" + sumInStoreCount.ToString() + "</font>\",\"wbTotalWeight\":" + "\"总运单重量:<font style='color:red;font-weight:bold'>" + sumTotalWeight.ToString() + "</font>\",\"OutStoreCount\":" + "\"OutStoreCount已出库分单数:<font style='color:red;font-weight:bold'>" + sumOutStoreCount.ToString() + "</font>\",\"StoreCount\":" + "\"StoreCount库存件数:<font style='color:red;font-weight:bold'>" + sumStoreCount.ToString() + "</font>\",\"NotInStoreCount\":" + "\"NotInStoreCount未入库件数:<font style='color:red;font-weight:bold'>" + sumNotInStore.ToString() + "</font>\"}]");
                sb.Append("}");
            }
            else
            {
                DataSet ds = tWayBillFlow.getWayBillWithCustomCondition(inputBeginDate, inputEndDate, txtCompany, txtWBSerialNum, txtSWBSerialNum, txtSWBNeedCheck, InOutStoreType, sort, order);
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    //dt = dtSort(dt, sort, order);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dt=DistinctDT(dt);

                        sb.Append("{");
                        sb.AppendFormat("\"total\":{0}", dt.Rows.Count);
                        sb.Append(",\"rows\":[");

                        int maxCount = -1;

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

                            DataSet dsWayBillInfo = tWayBill.getWayBillInfo(dt.Rows[i]["wbID"].ToString());
                            string wbSerialNum = "";
                            string wbCompany = "";
                            string swbTotalNum = "";
                            string swbTotalWeight = "";
                            string swbInStoreCount = "";
                            string swbOutStoreCount = "";
                            string swbStoreCount = "";
                            string swbNotInStoreCount = "";
                            string swbRejectNum = "";

                            if (dsWayBillInfo != null)
                            {
                                DataTable dtWayBillInfo = dsWayBillInfo.Tables[0];
                                if (dtWayBillInfo != null && dtWayBillInfo.Rows.Count > 0)
                                {
                                    wbSerialNum = dtWayBillInfo.Rows[0]["wbSerialNum"].ToString();
                                    wbCompany = dtWayBillInfo.Rows[0]["wbCompany"].ToString();
                                }
                            }

                            SqlParameter[] parameters = {
                      
                                new SqlParameter("@vCompany_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageBeginDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageEndDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vSerialNum_input",SqlDbType.NVarChar)
                                                        };

                            parameters[0].Value = "";
                            parameters[1].Value = "";
                            parameters[2].Value = "";
                            parameters[3].Value = wbSerialNum;
                            try
                            {
                                DataSet dsAnaysic = SqlServerHelper.RunProcedure("sp_WayBill_AnanalyseStatistic", parameters, "Default");
                                DataTable dtAnaysic = dsAnaysic.Tables["Default"];
                                swbInStoreCount = dtAnaysic.Rows[0]["InStoreNum"].ToString();
                                swbOutStoreCount = dtAnaysic.Rows[0]["OutStoreNum"].ToString();
                                swbStoreCount = dtAnaysic.Rows[0]["StoreNum"].ToString();
                                swbNotInStoreCount = dtAnaysic.Rows[0]["NotInStoreNum"].ToString();
                                swbRejectNum = dtAnaysic.Rows[0]["RejectNum"].ToString();

                                sumInStoreCount = sumInStoreCount + Convert.ToDouble(swbInStoreCount);
                                sumOutStoreCount = sumOutStoreCount + Convert.ToDouble(swbOutStoreCount);
                                sumStoreCount = sumStoreCount + Convert.ToDouble(swbStoreCount);
                                sumNotInStore = sumNotInStore + Convert.ToDouble(swbNotInStoreCount);
                                sumRejectNum = sumRejectNum + Convert.ToDouble(swbRejectNum);
                            }
                            catch (Exception ex)
                            {

                            }

                            DataSet dsSubWayBillInfo = tSubWayBill.GetSubWayBillSumInfo(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                            if (dsSubWayBillInfo != null)
                            {
                                DataTable dtSubWayBillInfo = dsSubWayBillInfo.Tables[0];
                                if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
                                {
                                    swbTotalNum = dtSubWayBillInfo.Rows[0]["swbTotalNumber"].ToString();
                                    swbTotalWeight = dtSubWayBillInfo.Rows[0]["swbTotalWeight"].ToString();

                                    sumTotalNumber = sumTotalNumber + Convert.ToDouble(swbTotalNum);
                                    sumTotalWeight = sumTotalWeight + Convert.ToDouble(swbTotalWeight);
                                }
                            }

                            //"wbCompany,wbSerialNum,wbTotalNumbe,wbTotalWeight,InStoreCount,OutStoreCount,wbID";
                            string[] strFiledArray = strFileds.Split(',');
                            for (int j = 0; j < strFiledArray.Length; j++)
                            {
                                switch (strFiledArray[j])
                                {
                                    case "wbCompany":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], wbCompany);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], wbCompany);
                                        }
                                        break;
                                    case "wbSerialNum"://格式化公司(保存的是用户名，取出公司名)
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], wbSerialNum);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], wbSerialNum);
                                        }
                                        break;
                                    case "wbTotalNumbe":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbTotalNum);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbTotalNum);
                                        }
                                        break;
                                    case "InStoreCount":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbInStoreCount);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbInStoreCount);
                                        }
                                        break;
                                    case "OutStoreCount":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbOutStoreCount);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbOutStoreCount);
                                        }
                                        break;
                                    case "StoreCount":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbStoreCount);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbStoreCount);
                                        }
                                        break;
                                    case "NotInStoreCount":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbNotInStoreCount);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbNotInStoreCount);
                                        }

                                        break;
                                    case "RejectNum":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbRejectNum);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbRejectNum);
                                        }
                                        break;
                                    case "wbTotalWeight":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbTotalWeight);
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbTotalWeight);
                                        }
                                        break;
                                    case "wbID":
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i]["wbID"].ToString());
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i]["wbID"].ToString());
                                        }
                                        break;
                                    default:
                                        if (j != strFiledArray.Length - 1)
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "");
                                        }
                                        else
                                        {
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "");
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
                    }
                    else
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"total\":{0}", dt.Rows.Count);
                        sb.Append(",\"rows\":[");
                    }
                }

                if (sb.ToString().EndsWith(","))
                {
                    sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
                }
                sb.Append("]");
                sb.Append(",\"footer\":[{\"wbCompany\":\"<font style='color:red;font-weight:bold'>当前页合计</font>\",\"wbTotalNumbe\":\"wbTotalNumbe总运单件数:<font style='color:red;font-weight:bold'>" + sumTotalNumber.ToString() + "</font>\",\"InStoreCount\":" + "\"InStoreCount已入库分单数:<font style='color:red;font-weight:bold'>" + sumInStoreCount.ToString() + "</font>\",\"wbTotalWeight\":" + "\"总运单重量:<font style='color:red;font-weight:bold'>" + sumTotalWeight.ToString() + "</font>\",\"OutStoreCount\":" + "\"OutStoreCount已出库分单数:<font style='color:red;font-weight:bold'>" + sumOutStoreCount.ToString() + "</font>\",\"StoreCount\":" + "\"StoreCount库存件数:<font style='color:red;font-weight:bold'>" + sumStoreCount.ToString() + "</font>\",\"NotInStoreCount\":" + "\"NotInStoreCount未入库件数:<font style='color:red;font-weight:bold'>" + sumNotInStore.ToString() + "</font>\",\"RejectNum\":" + "\"RejectNum退运件数:<font style='color:red;font-weight:bold'>" + sumRejectNum.ToString() + "</font>\"}]");
                sb.Append("}");

            }
            return sb.ToString();
        }


        protected DataTable dtSort(DataTable dtDistinctwbID, string sort, string order)
        {
            double sumTotalNumber = 0;
            double sumTotalWeight = 0;
            double sumInStoreCount = 0;
            double sumOutStoreCount = 0;
            double sumStoreCount = 0;
            double sumNotInStore = 0;

            DataTable dt = dtDistinctwbID;

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumbe", Type.GetType("System.Double"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.Double"));
            dtCustom.Columns.Add("InStoreCount", Type.GetType("System.Double"));
            dtCustom.Columns.Add("OutStoreCount", Type.GetType("System.Double"));
            dtCustom.Columns.Add("StoreCount", Type.GetType("System.Double"));
            dtCustom.Columns.Add("NotInStoreCount", Type.GetType("System.Double"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.Int32"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                DataSet dsWayBillInfo = tWayBill.getWayBillInfo(dt.Rows[i]["wbID"].ToString());
                string wbSerialNum = "";
                string wbCompany = "";
                string swbTotalNum = "";
                string swbTotalWeight = "";
                string swbInStoreCount = "";
                string swbOutStoreCount = "";
                string swbStoreCount = "";
                string swbNotInStoreCount = "";

                if (dsWayBillInfo != null)
                {
                    DataTable dtWayBillInfo = dsWayBillInfo.Tables[0];
                    if (dtWayBillInfo != null && dtWayBillInfo.Rows.Count > 0)
                    {
                        wbSerialNum = dtWayBillInfo.Rows[0]["wbSerialNum"].ToString();
                        wbCompany = dtWayBillInfo.Rows[0]["wbCompany"].ToString();
                    }
                }

                SqlParameter[] parameters = {
                      
                                new SqlParameter("@vCompany_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageBeginDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageEndDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vSerialNum_input",SqlDbType.NVarChar)
                                                        };

                parameters[0].Value = "";
                parameters[1].Value = "";
                parameters[2].Value = "";
                parameters[3].Value = wbSerialNum;
                try
                {
                    DataSet dsAnaysic = SqlServerHelper.RunProcedure("sp_WayBill_AnanalyseStatistic", parameters, "Default");
                    DataTable dtAnaysic = dsAnaysic.Tables["Default"];
                    swbInStoreCount = dtAnaysic.Rows[0]["InStoreNum"].ToString();
                    swbOutStoreCount = dtAnaysic.Rows[0]["OutStoreNum"].ToString();
                    swbStoreCount = dtAnaysic.Rows[0]["StoreNum"].ToString();
                    swbNotInStoreCount = dtAnaysic.Rows[0]["NotInStoreNum"].ToString();

                    sumInStoreCount = sumInStoreCount + Convert.ToDouble(swbInStoreCount);
                    sumOutStoreCount = sumOutStoreCount + Convert.ToDouble(swbOutStoreCount);
                    sumStoreCount = sumStoreCount + Convert.ToDouble(swbStoreCount);
                    sumNotInStore = sumNotInStore + Convert.ToDouble(swbNotInStoreCount);
                }
                catch (Exception ex)
                {

                }

                DataSet dsSubWayBillInfo = tSubWayBill.GetSubWayBillSumInfo(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                if (dsSubWayBillInfo != null)
                {
                    DataTable dtSubWayBillInfo = dsSubWayBillInfo.Tables[0];
                    if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
                    {
                        swbTotalNum = dtSubWayBillInfo.Rows[0]["swbTotalNumber"].ToString();
                        swbTotalWeight = dtSubWayBillInfo.Rows[0]["swbTotalWeight"].ToString();

                        sumTotalNumber = sumTotalNumber + Convert.ToDouble(swbTotalNum);
                        sumTotalWeight = sumTotalWeight + Convert.ToDouble(swbTotalWeight);
                    }
                }

                //"wbCompany,wbSerialNum,wbTotalNumbe,wbTotalWeight,InStoreCount,OutStoreCount,wbID";
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wbSerialNum"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = wbSerialNum;
                            break;
                        case "wbCompany"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = (new T_User()).GetUserByUserName(wbCompany);
                            break;
                        case "wbTotalNumbe":
                            drCustom[strFiledArray[j]] = swbTotalNum;
                            break;
                        case "InStoreCount":
                            drCustom[strFiledArray[j]] = swbInStoreCount;
                            break;
                        case "OutStoreCount":
                            drCustom[strFiledArray[j]] = swbOutStoreCount;
                            break;
                        case "StoreCount":
                            drCustom[strFiledArray[j]] = swbStoreCount;
                            break;
                        case "NotInStoreCount":
                            drCustom[strFiledArray[j]] = swbNotInStoreCount;
                            break;
                        case "wbTotalWeight":
                            drCustom[strFiledArray[j]] = swbTotalWeight;
                            break;
                        case "wbID":
                            drCustom[strFiledArray[j]] = dt.Rows[i]["wbID"].ToString();
                            break;
                        default:
                            drCustom[strFiledArray[j]] = "";
                            break;
                    }
                }
                if (drCustom["wbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }

            dtCustom.DefaultView.Sort = sort + " " + order;
            dtCustom = dtCustom.DefaultView.ToTable();

            return dtCustom;
        }


        [HttpGet]
        public ActionResult PrintWayBillInfo(string order, string page, string rows, string sort, string inputBeginDate, string inputEndDate, string txtCompany, string txtWBSerialNum, string txtSWBSerialNum, string txtSWBNeedCheck, string InOutStoreType)
        {
            double sumTotalNumber = 0;
            double sumTotalWeight = 0;
            double sumInStoreCount = 0;
            double sumOutStoreCount = 0;
            double sumStoreCount = 0;
            double sumNotInStore = 0;
            double sumRejectNum = 0;

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumbe", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("InStoreCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStoreCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("StoreCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("NotInStoreCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("RejectNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));

            DataRow drCustom = null;

            inputBeginDate = Server.UrlDecode(inputBeginDate);
            inputEndDate = Server.UrlDecode(inputEndDate);
            txtCompany = Server.UrlDecode(txtCompany);
            txtWBSerialNum = Server.UrlDecode(txtWBSerialNum);
            txtSWBSerialNum = Server.UrlDecode(txtSWBSerialNum);
            txtSWBNeedCheck = Server.UrlDecode(txtSWBNeedCheck);
            InOutStoreType = Server.UrlDecode(InOutStoreType);

            DataSet ds = tWayBillFlow.getWayBillWithCustomCondition(inputBeginDate, inputEndDate, txtCompany, txtWBSerialNum, txtSWBSerialNum, txtSWBNeedCheck, InOutStoreType, sort, order);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                //dt = dtSort(dt, sort, order);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dt = DistinctDT(dt);
                    int maxCount = -1;

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

                        DataSet dsWayBillInfo = tWayBill.getWayBillInfo(dt.Rows[i]["wbID"].ToString());
                        string wbSerialNum = "";
                        string wbCompany = "";
                        string swbTotalNum = "";
                        string swbTotalWeight = "";
                        string swbInStoreCount = "";
                        string swbOutStoreCount = "";
                        string swbStoreCount = "";
                        string swbNotInStoreCount = "";
                        string swbRejectNum = "";

                        if (dsWayBillInfo != null)
                        {
                            DataTable dtWayBillInfo = dsWayBillInfo.Tables[0];
                            if (dtWayBillInfo != null && dtWayBillInfo.Rows.Count > 0)
                            {
                                wbSerialNum = dtWayBillInfo.Rows[0]["wbSerialNum"].ToString();
                                wbCompany = dtWayBillInfo.Rows[0]["wbCompany"].ToString();
                            }
                        }

                        SqlParameter[] parameters = {
                      
                                new SqlParameter("@vCompany_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageBeginDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageEndDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vSerialNum_input",SqlDbType.NVarChar)
                                                        };

                        parameters[0].Value = "";
                        parameters[1].Value = "";
                        parameters[2].Value = "";
                        parameters[3].Value = wbSerialNum;
                        try
                        {
                            DataSet dsAnaysic = SqlServerHelper.RunProcedure("sp_WayBill_AnanalyseStatistic", parameters, "Default");
                            DataTable dtAnaysic = dsAnaysic.Tables["Default"];
                            swbInStoreCount = dtAnaysic.Rows[0]["InStoreNum"].ToString();
                            swbOutStoreCount = dtAnaysic.Rows[0]["OutStoreNum"].ToString();
                            swbStoreCount = dtAnaysic.Rows[0]["StoreNum"].ToString();
                            swbNotInStoreCount = dtAnaysic.Rows[0]["NotInStoreNum"].ToString();
                            swbRejectNum = dtAnaysic.Rows[0]["RejectNum"].ToString();

                            sumInStoreCount = sumInStoreCount + Convert.ToDouble(swbInStoreCount);
                            sumOutStoreCount = sumOutStoreCount + Convert.ToDouble(swbOutStoreCount);
                            sumStoreCount = sumStoreCount + Convert.ToDouble(swbStoreCount);
                            sumNotInStore = sumNotInStore + Convert.ToDouble(swbNotInStoreCount);
                            sumRejectNum = sumRejectNum + Convert.ToDouble(swbRejectNum);
                        }
                        catch (Exception ex)
                        {

                        }

                        DataSet dsSubWayBillInfo = tSubWayBill.GetSubWayBillSumInfo(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                        if (dsSubWayBillInfo != null)
                        {
                            DataTable dtSubWayBillInfo = dsSubWayBillInfo.Tables[0];
                            if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
                            {
                                swbTotalNum = dtSubWayBillInfo.Rows[0]["swbTotalNumber"].ToString();
                                swbTotalWeight = dtSubWayBillInfo.Rows[0]["swbTotalWeight"].ToString();

                                sumTotalNumber = sumTotalNumber + Convert.ToDouble(swbTotalNum);
                                sumTotalWeight = sumTotalWeight + Convert.ToDouble(swbTotalWeight);
                            }
                        }

                        string[] strFiledArray = strFileds.Split(',');
                        for (int j = 0; j < strFiledArray.Length; j++)
                        {
                            switch (strFiledArray[j])
                            {
                                case "wbCompany":
                                    drCustom[strFiledArray[j]] = wbCompany;
                                    break;
                                case "wbSerialNum"://格式化公司(保存的是用户名，取出公司名)
                                    drCustom[strFiledArray[j]] = wbSerialNum;
                                    break;
                                case "wbTotalNumbe":
                                    drCustom[strFiledArray[j]] = swbTotalNum;
                                    break;
                                case "InStoreCount":
                                    drCustom[strFiledArray[j]] = swbInStoreCount;
                                    break;
                                case "OutStoreCount":
                                    drCustom[strFiledArray[j]] = swbOutStoreCount;
                                    break;
                                case "StoreCount":
                                    drCustom[strFiledArray[j]] = swbStoreCount;
                                    break;
                                case "NotInStoreCount":
                                    drCustom[strFiledArray[j]] = swbNotInStoreCount;
                                    break;
                                case "RejectNum":
                                    drCustom[strFiledArray[j]] = swbRejectNum;
                                    break;
                                case "wbTotalWeight":
                                    drCustom[strFiledArray[j]] = swbTotalWeight;
                                    break;
                                case "wbID":
                                    drCustom[strFiledArray[j]] = dt.Rows[i]["wbID"].ToString();
                                    break;
                                default:
                                    drCustom[strFiledArray[j]] = "";
                                    break;
                            }
                        }
                        if (drCustom["wbID"].ToString() != "")
                        {
                            dtCustom.Rows.Add(drCustom);
                        }
                    }
                    dt = null;
                }
            }
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_WAYBIILL_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ShowWayBillInfo_DS", dtCustom);

            ReportParameter var_sumTotalNumber = new ReportParameter("sumTotalNumber", sumTotalNumber.ToString());
            ReportParameter var_sumTotalWeight = new ReportParameter("sumTotalWeight", sumTotalWeight.ToString());
            ReportParameter var_sumInStoreCount = new ReportParameter("sumInStoreCount", sumInStoreCount.ToString());
            ReportParameter var_sumOutStoreCount = new ReportParameter("sumOutStoreCount", sumOutStoreCount.ToString());
            ReportParameter var_sumStoreCount = new ReportParameter("sumStoreCount", sumStoreCount.ToString());
            ReportParameter var_sumNotInStoreCount = new ReportParameter("SumNotInStore", sumNotInStore.ToString());
            ReportParameter var_sumRejectNum = new ReportParameter("SumRejectNum", sumRejectNum.ToString());

            localReport.SetParameters(new ReportParameter[] { var_sumTotalNumber });
            localReport.SetParameters(new ReportParameter[] { var_sumTotalWeight });
            localReport.SetParameters(new ReportParameter[] { var_sumInStoreCount });
            localReport.SetParameters(new ReportParameter[] { var_sumOutStoreCount });
            localReport.SetParameters(new ReportParameter[] { var_sumStoreCount });
            localReport.SetParameters(new ReportParameter[] { var_sumNotInStoreCount });
            localReport.SetParameters(new ReportParameter[] { var_sumRejectNum });

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
        public ActionResult ExcelWayBillInfo(string order, string page, string rows, string sort, string inputBeginDate, string inputEndDate, string txtCompany, string txtWBSerialNum, string txtSWBSerialNum, string txtSWBNeedCheck, string InOutStoreType, string browserType)
        {
            double sumTotalNumber = 0;
            double sumTotalWeight = 0;
            double sumInStoreCount = 0;
            double sumOutStoreCount = 0;
            double sumStoreCount = 0;
            double sumNotInStore = 0;
            double sumRejectNum = 0;

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumbe", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("InStoreCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStoreCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("StoreCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("NotInStoreCount", Type.GetType("System.String"));
            dtCustom.Columns.Add("RejectNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));

            DataRow drCustom = null;

            inputBeginDate = Server.UrlDecode(inputBeginDate);
            inputEndDate = Server.UrlDecode(inputEndDate);
            txtCompany = Server.UrlDecode(txtCompany);
            txtWBSerialNum = Server.UrlDecode(txtWBSerialNum);
            txtSWBSerialNum = Server.UrlDecode(txtSWBSerialNum);
            txtSWBNeedCheck = Server.UrlDecode(txtSWBNeedCheck);
            InOutStoreType = Server.UrlDecode(InOutStoreType);

            DataSet ds = tWayBillFlow.getWayBillWithCustomCondition(inputBeginDate, inputEndDate, txtCompany, txtWBSerialNum, txtSWBSerialNum, txtSWBNeedCheck, InOutStoreType, sort, order);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                //dt = dtSort(dt, sort, order);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dt = DistinctDT(dt);
                    int maxCount = -1;

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

                        DataSet dsWayBillInfo = tWayBill.getWayBillInfo(dt.Rows[i]["wbID"].ToString());
                        string wbSerialNum = "";
                        string wbCompany = "";
                        string swbTotalNum = "";
                        string swbTotalWeight = "";
                        string swbInStoreCount = "";
                        string swbOutStoreCount = "";
                        string swbStoreCount = "";
                        string swbNotInStoreCount = "";
                        string swbRejectNum = "";

                        if (dsWayBillInfo != null)
                        {
                            DataTable dtWayBillInfo = dsWayBillInfo.Tables[0];
                            if (dtWayBillInfo != null && dtWayBillInfo.Rows.Count > 0)
                            {
                                wbSerialNum = dtWayBillInfo.Rows[0]["wbSerialNum"].ToString();
                                wbCompany = dtWayBillInfo.Rows[0]["wbCompany"].ToString();
                            }
                        }

                        SqlParameter[] parameters = {
                      
                                new SqlParameter("@vCompany_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageBeginDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vStorageEndDate_input",SqlDbType.NVarChar),
                                new SqlParameter("@vSerialNum_input",SqlDbType.NVarChar)
                                                        };

                        parameters[0].Value = "";
                        parameters[1].Value = "";
                        parameters[2].Value = "";
                        parameters[3].Value = wbSerialNum;
                        try
                        {
                            DataSet dsAnaysic = SqlServerHelper.RunProcedure("sp_WayBill_AnanalyseStatistic", parameters, "Default");
                            DataTable dtAnaysic = dsAnaysic.Tables["Default"];
                            swbInStoreCount = dtAnaysic.Rows[0]["InStoreNum"].ToString();
                            swbOutStoreCount = dtAnaysic.Rows[0]["OutStoreNum"].ToString();
                            swbStoreCount = dtAnaysic.Rows[0]["StoreNum"].ToString();
                            swbNotInStoreCount = dtAnaysic.Rows[0]["NotInStoreNum"].ToString();
                            swbRejectNum = dtAnaysic.Rows[0]["RejectNum"].ToString();

                            sumInStoreCount = sumInStoreCount + Convert.ToDouble(swbInStoreCount);
                            sumOutStoreCount = sumOutStoreCount + Convert.ToDouble(swbOutStoreCount);
                            sumStoreCount = sumStoreCount + Convert.ToDouble(swbStoreCount);
                            sumNotInStore = sumNotInStore + Convert.ToDouble(swbNotInStoreCount);
                            sumRejectNum = sumRejectNum + Convert.ToDouble(swbRejectNum);
                        }
                        catch (Exception ex)
                        {

                        }

                        DataSet dsSubWayBillInfo = tSubWayBill.GetSubWayBillSumInfo(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                        if (dsSubWayBillInfo != null)
                        {
                            DataTable dtSubWayBillInfo = dsSubWayBillInfo.Tables[0];
                            if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
                            {
                                swbTotalNum = dtSubWayBillInfo.Rows[0]["swbTotalNumber"].ToString();
                                swbTotalWeight = dtSubWayBillInfo.Rows[0]["swbTotalWeight"].ToString();

                                sumTotalNumber = sumTotalNumber + Convert.ToDouble(swbTotalNum);
                                sumTotalWeight = sumTotalWeight + Convert.ToDouble(swbTotalWeight);
                            }
                        }

                        string[] strFiledArray = strFileds.Split(',');
                        for (int j = 0; j < strFiledArray.Length; j++)
                        {
                            switch (strFiledArray[j])
                            {
                                case "wbCompany":
                                    drCustom[strFiledArray[j]] = wbCompany;
                                    break;
                                case "wbSerialNum"://格式化公司(保存的是用户名，取出公司名)
                                    drCustom[strFiledArray[j]] = wbSerialNum;
                                    break;
                                case "wbTotalNumbe":
                                    drCustom[strFiledArray[j]] = swbTotalNum;
                                    break;
                                case "InStoreCount":
                                    drCustom[strFiledArray[j]] = swbInStoreCount;
                                    break;
                                case "OutStoreCount":
                                    drCustom[strFiledArray[j]] = swbOutStoreCount;
                                    break;
                                case "StoreCount":
                                    drCustom[strFiledArray[j]] = swbStoreCount;
                                    break;
                                case "NotInStoreCount":
                                    drCustom[strFiledArray[j]] = swbNotInStoreCount;
                                    break;
                                case "RejectNum":
                                    drCustom[strFiledArray[j]] = swbRejectNum;
                                    break;
                                case "wbTotalWeight":
                                    drCustom[strFiledArray[j]] = swbTotalWeight;
                                    break;
                                case "wbID":
                                    drCustom[strFiledArray[j]] = dt.Rows[i]["wbID"].ToString();
                                    break;
                                default:
                                    drCustom[strFiledArray[j]] = "";
                                    break;
                            }
                        }
                        if (drCustom["wbID"].ToString() != "")
                        {
                            dtCustom.Rows.Add(drCustom);
                        }
                    }
                    dt = null;
                }
            }
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_WAYBIILL_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ShowWayBillInfo_DS", dtCustom);

            ReportParameter var_sumTotalNumber = new ReportParameter("sumTotalNumber", sumTotalNumber.ToString());
            ReportParameter var_sumTotalWeight = new ReportParameter("sumTotalWeight", sumTotalWeight.ToString());
            ReportParameter var_sumInStoreCount = new ReportParameter("sumInStoreCount", sumInStoreCount.ToString());
            ReportParameter var_sumOutStoreCount = new ReportParameter("sumOutStoreCount", sumOutStoreCount.ToString());
            ReportParameter var_sumStoreCount = new ReportParameter("sumStoreCount", sumStoreCount.ToString());
            ReportParameter var_sumNotInStoreCount = new ReportParameter("SumNotInStore", sumNotInStore.ToString());
            ReportParameter var_sumRejectNum = new ReportParameter("SumRejectNum", sumRejectNum.ToString());

            localReport.SetParameters(new ReportParameter[] { var_sumTotalNumber });
            localReport.SetParameters(new ReportParameter[] { var_sumTotalWeight });
            localReport.SetParameters(new ReportParameter[] { var_sumInStoreCount });
            localReport.SetParameters(new ReportParameter[] { var_sumOutStoreCount });
            localReport.SetParameters(new ReportParameter[] { var_sumStoreCount });
            localReport.SetParameters(new ReportParameter[] { var_sumNotInStoreCount });
            localReport.SetParameters(new ReportParameter[] { var_sumRejectNum });

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

            string strOutputFileName = "总运单信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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


        public string GetSubWayBill(string order, string page, string rows, string sort, string txtWBID, string strSwbSerialNum, string InOutType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_SubWayBill_WayBillFlow";

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

            txtWBID = Server.UrlDecode(txtWBID.ToString());
            strSwbSerialNum = Server.UrlDecode(strSwbSerialNum.ToString());
            InOutType = Server.UrlDecode(InOutType.ToString());

            string strWhereTemp = "";
            switch (InOutType)
            {
                case "1"://查看已入库记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (status=1) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (status=1) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                case "3"://查看已出库记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (IsOutStore=3) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (IsOutStore=3) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                case "-1"://查看总库存记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (InOutStoreType=1) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (InOutStoreType=1) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                case "99"://查看未入库记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and   (InOutStoreType is null) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (InOutStoreType is null) ";
                    }
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }
                    break;
                default://查看所有记录
                    if (txtWBID.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (wbID=" + txtWBID.ToString() + ") ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbID=" + txtWBID.ToString() + ") ";
                        }
                    }

                    if (strSwbSerialNum.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and   (swbSerialNum='" + strSwbSerialNum.ToString() + "') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (swbSerialNum='" + strSwbSerialNum.ToString() + "') ";
                        }
                    }
                    break;
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

                string[] strFiledArray = strFiledsSubWayBill.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "InOutStoreDate":
                            string InOutStoreDate = "";
                            switch (InOutType)
                            {
                                case "1":
                                    InOutStoreDate = dt.Rows[i]["operateDate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i]["operateDate"].ToString()).ToString("yyyy-MM-dd");
                                    break;
                                case "3":
                                    InOutStoreDate = dt.Rows[i]["OutStoreDate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i]["OutStoreDate"].ToString()).ToString("yyyy-MM-dd");
                                    break;
                                case "-1":
                                    InOutStoreDate = dt.Rows[i]["InOutStoreDate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i]["InOutStoreDate"].ToString()).ToString("yyyy-MM-dd");
                                    break;
                                default:
                                    InOutStoreDate = dt.Rows[i][strFiledArray[j]].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[i][strFiledArray[j]].ToString()).ToString("yyyy-MM-dd");
                                    break;
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], InOutStoreDate);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], InOutStoreDate);
                            }
                            break;
                        case "FinalStatusDecription":
                            string FinalStatusDecription = "";
                            switch (InOutType)
                            {
                                case "1":
                                    FinalStatusDecription = dt.Rows[i]["StatusDecription"] == DBNull.Value ? "" : dt.Rows[i]["StatusDecription"].ToString();
                                    break;
                                case "3":
                                    FinalStatusDecription = dt.Rows[i]["IsOutStoreStatusDecription"] == DBNull.Value ? "" : dt.Rows[i]["IsOutStoreStatusDecription"].ToString();
                                    break;
                                case "-1":
                                    FinalStatusDecription = dt.Rows[i]["FinalStatusDecription"] == DBNull.Value ? "" : dt.Rows[i]["FinalStatusDecription"].ToString();
                                    break;
                                default:
                                    FinalStatusDecription = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                                    break;
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], FinalStatusDecription);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], FinalStatusDecription);
                            }
                            break;
                        case "InOutStoreOperator":
                            string InOutStoreOperator = "";
                            switch (InOutType)
                            {
                                case "1":
                                    InOutStoreOperator = dt.Rows[i]["Operator"] == DBNull.Value ? "" : dt.Rows[i]["Operator"].ToString();
                                    break;
                                case "3":
                                    InOutStoreOperator = dt.Rows[i]["OutStoreOperator"] == DBNull.Value ? "" : dt.Rows[i]["OutStoreOperator"].ToString();
                                    break;
                                case "-1":
                                    InOutStoreOperator = dt.Rows[i]["InOutStoreOperator"] == DBNull.Value ? "" : dt.Rows[i]["InOutStoreOperator"].ToString();
                                    break;
                                default:
                                    InOutStoreOperator = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                                    break;
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], InOutStoreOperator);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], InOutStoreOperator);
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

        protected DataTable DistinctDT(DataTable dt)
        {
            DataTable dtTmp = null;
            Dictionary<Int32, string> dicDistinct = new Dictionary<int, string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!dicDistinct.ContainsKey(Convert.ToInt32(dt.Rows[i]["wbID"].ToString())))
                {
                    dicDistinct.Add(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()),"");
                }
            }

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbID", Type.GetType("System.Int32"));

            DataRow drCustom = null;
            for (int i = 0; i < dicDistinct.Keys.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                drCustom["wbID"] = dicDistinct.ElementAt(i).Key;
                if (drCustom["wbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
           
            dtTmp = dtCustom;
            return dtTmp;
        }
    }
}
