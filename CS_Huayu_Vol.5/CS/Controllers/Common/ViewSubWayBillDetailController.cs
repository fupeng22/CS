using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using SQLDAL;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using DBUtility;
using Microsoft.Reporting.WebForms;
using System.IO;
using CS.Filter;
using Util;

namespace CS.Controllers.Common
{
    [ErrorAttribute]
    public class ViewSubWayBillDetailController : Controller
    {
        Model.M_WayBill wayBillModel = new M_WayBill();
        Model.M_SubWayBill subWayBillModel = new M_SubWayBill();
        SQLDAL.T_WayBill wayBillSql = new T_WayBill();
        SQLDAL.T_SubWayBill subWayBillSql = new T_SubWayBill();
        public const string strFileds = "swbNeedCheck,wbStorageDate,wbCompany,wbSerialNum,swbActualNumber,wbStatus,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,swbSerialNum,wbID,swbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/ViewSubWayBillDetail.rdlc";
        //
        // GET: /Customer_Detail/

        public ActionResult Index()
        {
            ViewData["Detail_wbSerialNum"] = Request.QueryString["Detail_wbSerialNum"] == null ? "" : Request.QueryString["Detail_wbSerialNum"];
            ViewData["Detail_swbSerialNum"] = Request.QueryString["Detail_swbSerialNum"] == null ? "" : Request.QueryString["Detail_swbSerialNum"];
            ViewData["Detail_swbStatus"] = Request.QueryString["Detail_swbStatus"] == null ? "" : Request.QueryString["Detail_swbStatus"];
            ViewData["Detail_swbReleaseFlag"] = Request.QueryString["Detail_swbReleaseFlag"] == null ? "" : Request.QueryString["Detail_swbReleaseFlag"];

            ViewData["Detail_bEnableReject"] = Request.QueryString["Detail_bEnableReject"] == null ? "" : Request.QueryString["Detail_bEnableReject"];
            ViewData["Detail_bEnableUnRelease"] = Request.QueryString["Detail_bEnableUnRelease"] == null ? "" : Request.QueryString["Detail_bEnableUnRelease"];
            ViewData["Detail_bEnableRelease"] = Request.QueryString["Detail_bEnableRelease"] == null ? "" : Request.QueryString["Detail_bEnableRelease"];

            DataSet dsWayBill = null;
            DataTable dtWayBill = null;
            dsWayBill = new T_WayBill().GetWayBill(ViewData["Detail_wbSerialNum"].ToString());
            if (dsWayBill!=null)
            {
                dtWayBill=dsWayBill.Tables[0];
                if (dtWayBill!=null && dtWayBill.Rows.Count>0)
                {
                    ViewData["Detail_wbImportType"] = dtWayBill.Rows[0]["wbImportType"].ToString();
                }
            }
            
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

        public string GetData(string order, string page, string rows, string sort, string Detail_wbSerialNum, string Detail_swbSerialNum, string txtswbDescription_CHN, string txtswbDescription_ENG, string txtSwbStatus, string ddlSortingTimes, string txtSwbReleaseFlag, string Detail_wbImportType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBill_SubWayBill";

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

            string strWhereTemp = "";
            string wbSerialNum = Server.UrlDecode(Detail_wbSerialNum.ToString());
            string swbSerialNum = Server.UrlDecode(Detail_swbSerialNum.ToString());
            string swbDescription_CHN = Server.UrlDecode(txtswbDescription_CHN.ToString());
            string swbDescription_ENG = Server.UrlDecode(txtswbDescription_ENG.ToString());
            string SwbStatus = Server.UrlDecode(txtSwbStatus.ToString());
            string strddlSortingTimes = Server.UrlDecode(ddlSortingTimes.ToString());
            string strWBImportType = Server.UrlDecode(Detail_wbImportType.ToString());
            string strSwbReleaseFlag = Server.UrlDecode(txtSwbReleaseFlag.ToString());

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

            switch (strWBImportType)
            {
                case "0":
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
                    break;
                case "1":
                    if (SwbStatus == "99")
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
                    else
                    {
                        if (strSwbReleaseFlag != "" && strSwbReleaseFlag != "-99")
                        {
                            if (txtSwbReleaseFlag.IndexOf("99") != -1)
                            {
                                if (strWhereTemp != "")
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" and ((swbNeedCheck=99)  or  (swbReleaseFlag in ({0}))) ", txtSwbReleaseFlag);
                                }
                                else
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" ((swbNeedCheck=99)  or  (swbReleaseFlag in ({0}))) ", txtSwbReleaseFlag);
                                }
                            }
                            else
                            {
                                if (strWhereTemp != "")
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" and (swbReleaseFlag in ({0})) ", txtSwbReleaseFlag);
                                }
                                else
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" (swbReleaseFlag in ({0})) ", txtSwbReleaseFlag);
                                }
                            }
                          
                        }
                    }
                    break;
                default:
                    break;
            }

            if (strddlSortingTimes != "---请选择---")
            {
                switch (strddlSortingTimes)
                {
                    case "0":
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (swbActualNumber is null or swbActualNumber=0) ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (swbActualNumber is null or swbActualNumber=0) ";
                        }
                        break;
                    case "1":
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (swbActualNumber is not null and swbActualNumber<>0) ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (swbActualNumber is not null and swbActualNumber<>0) ";
                        }
                        break;
                    default:
                        break;
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
                        case "swbNeedCheck":
                            string strSubWayBillStatus = "";
                            strSubWayBillStatus = CommonHelper.ParseSwbNeedCheck(dt.Rows[i][strFiledArray[j]].ToString(), dt.Rows[i]["wbImportType"].ToString(), dt.Rows[i]["swbReleaseFlag"].ToString());
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], strSubWayBillStatus);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], strSubWayBillStatus);
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

        public ActionResult Print(string order, string page, string rows, string sort, string Detail_wbSerialNum, string Detail_swbSerialNum, string txtswbDescription_CHN, string txtswbDescription_ENG, string txtSwbStatus, string ddlSortingTimes, string txtSwbReleaseFlag, string Detail_wbImportType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBill_SubWayBill";

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

            string strWhereTemp = "";
            string wbSerialNum = Server.UrlDecode(Detail_wbSerialNum.ToString());
            string swbSerialNum = Server.UrlDecode(Detail_swbSerialNum.ToString());
            string swbDescription_CHN = Server.UrlDecode(txtswbDescription_CHN.ToString());
            string swbDescription_ENG = Server.UrlDecode(txtswbDescription_ENG.ToString());
            string SwbStatus = Server.UrlDecode(txtSwbStatus.ToString());
            string strddlSortingTimes = Server.UrlDecode(ddlSortingTimes.ToString());
            string strWBImportType = Server.UrlDecode(Detail_wbImportType.ToString());
            string strSwbReleaseFlag = Server.UrlDecode(txtSwbReleaseFlag.ToString());

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

            switch (strWBImportType)
            {
                case "0":
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
                    break;
                case "1":
                    if (SwbStatus == "99")
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
                    else
                    {
                        if (strSwbReleaseFlag != "" && strSwbReleaseFlag != "-99")
                        {
                            if (txtSwbReleaseFlag.IndexOf("99") != -1)
                            {
                                if (strWhereTemp != "")
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" and ((swbNeedCheck=99)  or  (swbReleaseFlag in ({0}))) ", txtSwbReleaseFlag);
                                }
                                else
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" ((swbNeedCheck=99)  or  (swbReleaseFlag in ({0}))) ", txtSwbReleaseFlag);
                                }
                            }
                            else
                            {
                                if (strWhereTemp != "")
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" and (swbReleaseFlag in ({0})) ", txtSwbReleaseFlag);
                                }
                                else
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" (swbReleaseFlag in ({0})) ", txtSwbReleaseFlag);
                                }
                            }

                        }
                    }
                    break;
                default:
                    break;
            }


            if (strddlSortingTimes != "---请选择---")
            {
                switch (strddlSortingTimes)
                {
                    case "0":
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (swbActualNumber is null or swbActualNumber=0) ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (swbActualNumber is null or swbActualNumber=0) ";
                        }
                        break;
                    case "1":
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (swbActualNumber is not null and swbActualNumber<>0) ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (swbActualNumber is not null and swbActualNumber<>0) ";
                        }
                        break;
                    default:
                        break;
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
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbActualNumber", Type.GetType("System.String"));
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
                        case "swbNeedCheck":
                            string strSubWayBillStatus = "";
                            strSubWayBillStatus = CommonHelper.ParseSwbNeedCheck(dt.Rows[i][strFiledArray[j]].ToString(), dt.Rows[i]["wbImportType"].ToString(), dt.Rows[i]["swbReleaseFlag"].ToString());
                            drCustom[strFiledArray[j]] = strSubWayBillStatus;
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
            ReportDataSource reportDataSource = new ReportDataSource("ViewSubWayBillDetail_DS", dtCustom);

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

        public ActionResult Excel(string order, string page, string rows, string sort, string Detail_wbSerialNum, string Detail_swbSerialNum, string txtswbDescription_CHN, string txtswbDescription_ENG, string txtSwbStatus, string ddlSortingTimes, string txtSwbReleaseFlag, string Detail_wbImportType, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBill_SubWayBill";

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

            string strWhereTemp = "";
            string wbSerialNum = Server.UrlDecode(Detail_wbSerialNum.ToString());
            string swbSerialNum = Server.UrlDecode(Detail_swbSerialNum.ToString());
            string swbDescription_CHN = Server.UrlDecode(txtswbDescription_CHN.ToString());
            string swbDescription_ENG = Server.UrlDecode(txtswbDescription_ENG.ToString());
            string SwbStatus = Server.UrlDecode(txtSwbStatus.ToString());
            string strddlSortingTimes = Server.UrlDecode(ddlSortingTimes.ToString());
            string strWBImportType = Server.UrlDecode(Detail_wbImportType.ToString());
            string strSwbReleaseFlag = Server.UrlDecode(txtSwbReleaseFlag.ToString());

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

            switch (strWBImportType)
            {
                case "0":
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
                    break;
                case "1":
                    if (SwbStatus == "99")
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
                    else
                    {
                        if (strSwbReleaseFlag != "" && strSwbReleaseFlag != "-99")
                        {
                            if (txtSwbReleaseFlag.IndexOf("99") != -1)
                            {
                                if (strWhereTemp != "")
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" and ((swbNeedCheck=99)  or  (swbReleaseFlag in ({0}))) ", txtSwbReleaseFlag);
                                }
                                else
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" ((swbNeedCheck=99)  or  (swbReleaseFlag in ({0}))) ", txtSwbReleaseFlag);
                                }
                            }
                            else
                            {
                                if (strWhereTemp != "")
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" and (swbReleaseFlag in ({0})) ", txtSwbReleaseFlag);
                                }
                                else
                                {
                                    strWhereTemp = strWhereTemp + string.Format(" (swbReleaseFlag in ({0})) ", txtSwbReleaseFlag);
                                }
                            }

                        }
                    }
                    break;
                default:
                    break;
            }


            if (strddlSortingTimes != "---请选择---")
            {
                switch (strddlSortingTimes)
                {
                    case "0":
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (swbActualNumber is null or swbActualNumber=0) ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (swbActualNumber is null or swbActualNumber=0) ";
                        }
                        break;
                    case "1":
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (swbActualNumber is not null and swbActualNumber<>0) ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (swbActualNumber is not null and swbActualNumber<>0) ";
                        }
                        break;
                    default:
                        break;
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
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbActualNumber", Type.GetType("System.String"));
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
                        case "swbNeedCheck":
                            string strSubWayBillStatus = "";
                            strSubWayBillStatus = CommonHelper.ParseSwbNeedCheck(dt.Rows[i][strFiledArray[j]].ToString(), dt.Rows[i]["wbImportType"].ToString(), dt.Rows[i]["swbReleaseFlag"].ToString());
                            drCustom[strFiledArray[j]] = strSubWayBillStatus;
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
            ReportDataSource reportDataSource = new ReportDataSource("ViewSubWayBillDetail_DS", dtCustom);

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

            string strOutputFileName = "子运单信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
        public string PatchUpdateSwbNeedCheck(string strSwbIds, string iNeedCheck)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"更改失败，原因未知\"}";
            strSwbIds = Server.UrlDecode(strSwbIds);
            iNeedCheck = Server.UrlDecode(iNeedCheck);
            try
            {
                if (strSwbIds != "")
                {
                    if ((new T_SubWayBill()).upDateSwbNeedCheck(strSwbIds, iNeedCheck))
                    {
                        strRet = "{\"result\":\"ok\",\"message\":\"" + "更改成功" + "\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"" + "更改失败" + "\"}";
                    }
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "请先选择数据" + "\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }

        [HttpPost]
        public string PatchRejectSubWayBill(string strSwbIds)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"退货失败，原因未知\"}";
            strSwbIds = Server.UrlDecode(strSwbIds);
            try
            {
                if (strSwbIds != "")
                {
                    if ((new T_SubWayBill()).RejectSubWayBill(strSwbIds, Session["Global_Huayu_UserName"] == null ? "" : Session["Global_Huayu_UserName"].ToString()))
                    {
                        strRet = "{\"result\":\"ok\",\"message\":\"" + "退货成功" + "\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"" + "退货失败" + "\"}";
                    }
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "请先选择数据" + "\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }

        [HttpPost]
        public string getSubWayBillSumInfo(string wbID)
        {
            string strRet = "{\"result\":\"error\",swbTotalWeight:\"0.00\",swbTotalNumber:\"0\",\"swbTotalActualNumber\":\"0\",\"swbTotalActualWeight\":\"0.00\",\"message\":\"获取失败，原因未知\"}";
            string swbTotalNumber = "0";
            string swbTotalActualWeight = "0";
            string swbTotalWeight = "0";
            string swbTotalActualNumber = "0";

            wbID = Server.UrlDecode(wbID);

            try
            {
                if (wbID == "")
                {
                    strRet = "{\"result\":\"error\",swbTotalWeight:\"0.00\",swbTotalNumber:\"0\",\"swbTotalActualNumber\":\"0\",\"swbTotalActualWeight\":\"0.00\",\"message\":\"" + "获取失败,未给出总运单ID" + "\"}";
                }
                else
                {
                    DataSet dsSum = new T_SubWayBill().GetSubWayBillSumInfo(Convert.ToInt32(wbID));
                    if (dsSum != null)
                    {
                        DataTable dtSum = dsSum.Tables[0];
                        if (dtSum != null && dtSum.Rows.Count > 0)
                        {
                            swbTotalNumber = dtSum.Rows[0]["swbTotalNumber"].ToString();
                            swbTotalWeight = dtSum.Rows[0]["swbTotalWeight"].ToString();
                            swbTotalActualNumber = dtSum.Rows[0]["swbTotalActualNumber"].ToString();
                            swbTotalActualWeight = dtSum.Rows[0]["swbTotalActualWeight"].ToString();
                        }
                    }
                    strRet = "{\"result\":\"ok\",swbTotalWeight:\"" + swbTotalWeight + "\",swbTotalNumber:\"" + swbTotalNumber + "\",\"swbTotalActualNumber\":\""+swbTotalActualNumber+"\",\"swbTotalActualWeight\":\"" + swbTotalActualWeight + "\",\"message\":\"获取成功\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",swbTotalWeight:\"0.00\",swbTotalNumber:\"0\",\"swbTotalActualNumber\":\"0\",\"swbTotalActualWeight\":\"0.00\",\"message\":\"获取失败，原因:" + ex.Message + "\"}";
            }
            return strRet;
        }

        [HttpPost]
        public string getReleaseNum_UnReleaseNumByWbID(string wbID)
        {
            string strRet = "{\"result\":\"error\",wbStatus:\"\",releseNum:\"0\",\"notReleseNum\":\"0\",\"message\":\"获取失败，原因未知\"}";
            int releseNum = -1;
            int notReleseNum = -1;
            string strReleseNum = "-1";
            string strNotReleseNum = "-1";
            string wbStatus = "";
            DataSet dsReleaseNum_UnReleaseNum = null;
            DataTable dtReleaseNum_UnReleaseNum = null;

            wbID = Server.UrlDecode(wbID);

            try
            {
                if (wbID == "")
                {
                    strRet = "{\"result\":\"error\",wbStatus:\"\",releseNum:\"0\",\"notReleseNum\":\"0\",\"message\":\"" + "获取失败,未给出总运单ID" + "\"}";
                }
                else
                {
                    releseNum = -1;
                    notReleseNum = -1;

                    switch (int.Parse(new T_WayBill().getWayBillInfo(wbID).Tables[0].Rows[0]["wbStatus"].ToString()))
                    {
                        case 0:
                            wbStatus="等待预检";
                            break;
                        case 1:
                            wbStatus="查验中";
                            break;
                        case 2:
                            wbStatus="已放行";
                            dsReleaseNum_UnReleaseNum = new T_SubWayBill().GetReleaseNum_UnReleaseNumByWbID(int.Parse(wbID));
                            if (dsReleaseNum_UnReleaseNum != null)
                            {
                                dtReleaseNum_UnReleaseNum = dsReleaseNum_UnReleaseNum.Tables[0];
                                if (dtReleaseNum_UnReleaseNum != null && dtReleaseNum_UnReleaseNum.Rows.Count > 0)
                                {
                                    releseNum = int.Parse(dtReleaseNum_UnReleaseNum.Rows[0]["ReleaseNum"].ToString());
                                    notReleseNum = int.Parse(dtReleaseNum_UnReleaseNum.Rows[0]["unReleaseNum"].ToString());
                                }
                            }
                            break;
                        default:
                            wbStatus="未知";
                            break;
                    }


                    if (releseNum != -1)
                    {
                        strReleseNum = releseNum.ToString();
                    }
                    else
                    {
                        strReleseNum = "无";

                    }
                    if (notReleseNum != -1)
                    {
                        strNotReleseNum = notReleseNum.ToString();
                    }
                    else
                    {
                        strNotReleseNum = "无";
                    }

                    strRet = "{\"result\":\"ok\",wbStatus:\""+wbStatus+"\",releseNum:\""+strReleseNum+"\",\"notReleseNum\":\""+strNotReleseNum+"\",\"message\":\"获取成功\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",wbStatus:\"\",releseNum:\"0\",\"notReleseNum\":\"0\",\"message\":\"获取失败，原因:" + ex.Message + "\"}";
            }
            return strRet;
        }
    }
}
