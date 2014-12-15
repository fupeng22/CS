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
using System.Text;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace CS.Controllers.Common
{
    public class ViewForeStoreWayBillDetailController : Controller
    {
        Model.M_WayBill wayBillModel = new M_WayBill();
        Model.M_SubWayBill subWayBillModel = new M_SubWayBill();
        SQLDAL.T_WayBill wayBillSql = new T_WayBill();
        SQLDAL.T_SubWayBill subWayBillSql = new T_SubWayBill();
        public const string strFileds = "swbNeedCheck,swbNeedCheckDescription,wbStorageDate,wbCompany,wbSerialNum,wbStatus,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,swbSerialNum,wbID,swbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/ViewForeStoreWayBillDetail.rdlc";
        //
        // GET: /Customer_Detail/

        public ActionResult Index()
        {
            ViewData["Detail_wbSerialNum"] = Request.QueryString["Detail_wbSerialNum"] == null ? "" : Request.QueryString["Detail_wbSerialNum"];
            ViewData["Detail_swbSerialNum"] = Request.QueryString["Detail_swbSerialNum"] == null ? "" : Request.QueryString["Detail_swbSerialNum"];
            ViewData["Detail_swbStatus"] = Request.QueryString["Detail_swbStatus"] == null ? "" : Request.QueryString["Detail_swbStatus"];

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

        public string GetData(string order, string page, string rows, string sort, string Detail_wbSerialNum, string Detail_swbSerialNum, string txtswbDescription_CHN, string txtswbDescription_ENG, string txtSwbStatus)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Fore_WayBill";

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

            //string strWhereTemp = " ((swbInspectionFlag=3 and wbImportType=1) or (wbImportType is null or wbImportType=0)) ";
            string strWhereTemp = "";
            string wbSerialNum = Server.UrlDecode(Detail_wbSerialNum.ToString());
            string swbSerialNum = Server.UrlDecode(Detail_swbSerialNum.ToString());
            string swbDescription_CHN = Server.UrlDecode(txtswbDescription_CHN.ToString());
            string swbDescription_ENG = Server.UrlDecode(txtswbDescription_ENG.ToString());
            string SwbStatus = Server.UrlDecode(txtSwbStatus.ToString());

            if (wbSerialNum != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (wbSerialNum ='" + wbSerialNum + "') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (wbSerialNum ='" + wbSerialNum + "') ";
                }
            }

            if (swbSerialNum != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (swbSerialNum like '%" + swbSerialNum + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (swbSerialNum like '%" + swbSerialNum + "%') ";
                }
            }

            if (swbDescription_CHN != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (swbDescription_CHN like '%" + swbDescription_CHN + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (swbDescription_CHN like '%" + swbDescription_CHN + "%') ";
                }
            }

            if (swbDescription_ENG != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (swbDescription_ENG like '%" + swbDescription_ENG + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (swbDescription_ENG like '%" + swbDescription_ENG + "%') ";
                }
            }

            if (SwbStatus != "" && SwbStatus != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbNeedCheck in ({0})) ", SwbStatus);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (swbNeedCheck in ({0})) ", SwbStatus);
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
        public ActionResult Print(string order, string page, string rows, string sort, string Detail_wbSerialNum, string Detail_swbSerialNum, string txtswbDescription_CHN, string txtswbDescription_ENG, string txtSwbStatus)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Fore_WayBill";

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

            //string strWhereTemp = " ((swbInspectionFlag=3 and wbImportType=1) or (wbImportType is null or wbImportType=0)) ";
            string strWhereTemp = "";
            string wbSerialNum = Server.UrlDecode(Detail_wbSerialNum.ToString());
            string swbSerialNum = Server.UrlDecode(Detail_swbSerialNum.ToString());
            string swbDescription_CHN = Server.UrlDecode(txtswbDescription_CHN.ToString());
            string swbDescription_ENG = Server.UrlDecode(txtswbDescription_ENG.ToString());
            string SwbStatus = Server.UrlDecode(txtSwbStatus.ToString());

            if (wbSerialNum != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (wbSerialNum ='" + wbSerialNum + "') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (wbSerialNum ='" + wbSerialNum + "') ";
                }
            }

            if (swbSerialNum != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (swbSerialNum like '%" + swbSerialNum + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (swbSerialNum like '%" + swbSerialNum + "%') ";
                }
            }

            if (swbDescription_CHN != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (swbDescription_CHN like '%" + swbDescription_CHN + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (swbDescription_CHN like '%" + swbDescription_CHN + "%') ";
                }
            }

            if (swbDescription_ENG != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (swbDescription_ENG like '%" + swbDescription_ENG + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (swbDescription_ENG like '%" + swbDescription_ENG + "%') ";
                }
            }

            if (SwbStatus != "" && SwbStatus != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbNeedCheck in ({0})) ", SwbStatus);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (swbNeedCheck in ({0})) ", SwbStatus);
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
            dtCustom.Columns.Add("swbNeedCheck", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNeedCheckDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbID", Type.GetType("System.String"));

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
                if (drCustom["swbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ViewForeStoreWayBillDetail_DS", dtCustom);

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

        public ActionResult Excel(string order, string page, string rows, string sort, string Detail_wbSerialNum, string Detail_swbSerialNum, string txtswbDescription_CHN, string txtswbDescription_ENG, string txtSwbStatus, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Fore_WayBill";

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

            //string strWhereTemp = " ((swbInspectionFlag=3 and wbImportType=1) or (wbImportType is null or wbImportType=0)) ";
            string strWhereTemp = "";
            string wbSerialNum = Server.UrlDecode(Detail_wbSerialNum.ToString());
            string swbSerialNum = Server.UrlDecode(Detail_swbSerialNum.ToString());
            string swbDescription_CHN = Server.UrlDecode(txtswbDescription_CHN.ToString());
            string swbDescription_ENG = Server.UrlDecode(txtswbDescription_ENG.ToString());
            string SwbStatus = Server.UrlDecode(txtSwbStatus.ToString());

            if (wbSerialNum != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (wbSerialNum ='" + wbSerialNum + "') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (wbSerialNum ='" + wbSerialNum + "') ";
                }
            }

            if (swbSerialNum != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (swbSerialNum like '%" + swbSerialNum + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (swbSerialNum like '%" + swbSerialNum + "%') ";
                }
            }

            if (swbDescription_CHN != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (swbDescription_CHN like '%" + swbDescription_CHN + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (swbDescription_CHN like '%" + swbDescription_CHN + "%') ";
                }
            }

            if (swbDescription_ENG != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (swbDescription_ENG like '%" + swbDescription_ENG + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (swbDescription_ENG like '%" + swbDescription_ENG + "%') ";
                }
            }

            if (SwbStatus != "" && SwbStatus != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbNeedCheck in ({0})) ", SwbStatus);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (swbNeedCheck in ({0})) ", SwbStatus);
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
            dtCustom.Columns.Add("swbNeedCheck", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNeedCheckDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbID", Type.GetType("System.String"));

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
                if (drCustom["swbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("ViewForeStoreWayBillDetail_DS", dtCustom);

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

            string strOutputFileName = "预入库信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
        public string PatchSaveInStore(string strSwbIds)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"入库失败，原因未知\"}";
            DataSet dsSubWayBill = null;
            DataTable dtSubWayBill = null;

            try
            {
                strSwbIds = Server.UrlDecode(strSwbIds);
                if (!string.IsNullOrEmpty(strSwbIds))
                {
                    dsSubWayBill = new T_SubWayBill().getWayBillSubWayBill(strSwbIds);
                    if (dsSubWayBill != null)
                    {
                        dtSubWayBill = dsSubWayBill.Tables[0];
                        if (dtSubWayBill != null && dtSubWayBill.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtSubWayBill.Rows.Count; i++)
                            {
                                new T_WayBillFlow().UpdateStatus(dtSubWayBill.Rows[i]["wbSerialNum"].ToString(), dtSubWayBill.Rows[i]["swbSerialNum"].ToString(), 1, Session["Global_Huayu_UserName"] == null ? "" : Session["Global_Huayu_UserName"].ToString(), dtSubWayBill.Rows[i]["wbImportType"].ToString(), dtSubWayBill.Rows[i]["wbID"].ToString(), dtSubWayBill.Rows[i]["swbID"].ToString());
                            }
                            strRet = "{\"result\":\"ok\",\"message\":\"" + "入库成功" + "\"}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"入库失败,原因:" + ex.Message + "\"}";
            }

            return strRet;
        }
    }
}
