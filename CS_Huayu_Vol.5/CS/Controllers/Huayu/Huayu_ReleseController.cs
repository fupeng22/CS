using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SQLDAL;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using System.Text;
using CS.Filter;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_ReleseController : Controller
    {
        SQLDAL.T_WayBill tWayBill = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();
        public const string strFileds = "wbStorageDate,wbCompany,wbVoyage,wbSerialNum,wbTotalNumber,wbTotalNumber_Customize,wbTotalWeight_Customize,wbActualTotalWeight__Customize,wbTotalWeight,wbMoney_Custom,subNum,anyOutStore,value1,wbImportType,wbArrivalDate,wbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/HuayuRelese.rdlc";
        public const string STR_RELEASE_REPORT_URL = "~/Content/Reports/Huayu_Print_Release.rdlc";
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
        public string GetData(string order, string page, string rows, string sort, string txtCode, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage, string hidSearchType, string txtCompany)
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

            txtCode = Server.UrlDecode(txtCode.ToString());
            txtGCode = Server.UrlDecode(txtGCode.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());
            txtCompany = Server.UrlDecode(txtCompany.ToString());

            string strWhereTemp = "  (wbStatus = 2) ";

            switch (hidSearchType.ToString())
            {
                case "-1"://没有选择条件,则返回空记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + "  and  (1=2) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "   (1=2) ";
                    }
                    break;
                case "1"://选择普通选择
                    if (txtCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and  (wbSerialNum like '%" + txtCode.ToString() + "%')";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "   (wbSerialNum like '%" + txtCode.ToString() + "%')";
                        }
                    }
                    break;
                case "0"://选择高级查询
                    if (txtGCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (wbSerialNum like '%" + txtGCode.ToString() + "%')  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbSerialNum like '%" + txtGCode.ToString() + "%')  ";
                        }
                    }

                    if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'   ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'   ";
                        }
                    }

                    if (txtVoyage.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and wbVoyage like '%" + txtVoyage.ToString() + "%'  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  wbVoyage like '%" + txtVoyage.ToString() + "%'  ";
                        }
                    }

                    if (txtCompany.ToString() != "" && txtCompany.ToString() != "---请选择---")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and wbCompany like '%" + txtCompany.ToString() + "%'  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  wbCompany like '%" + txtCompany.ToString() + "%'  ";
                        }
                    }
                    break;
                default:
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + "  and  (1=2) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "   (1=2) ";
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

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wbTotalWeight":
                            string strActualWeight = "0.00";
                            //double TotalActualWeight = tSubWayBill.GetTotalActualWeight(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));

                            //if (TotalActualWeight > double.Parse(dt.Rows[i]["wbTotalWeight"].ToString()))
                            //{
                            //    strActualWeight = TotalActualWeight.ToString();
                            //}
                            //else
                            //{
                            //    strActualWeight = double.Parse(dt.Rows[i]["wbTotalWeight"].ToString()).ToString();
                            //}
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], strActualWeight);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], strActualWeight);
                            }
                            break;
                        case "subNum":
                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], tSubWayBill.GetActualSubNum(Convert.ToInt32(dt.Rows[i]["wbID"].ToString())).ToString().Replace("\r\n", ""));
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], tSubWayBill.GetActualSubNum(Convert.ToInt32(dt.Rows[i]["wbID"].ToString())).ToString().Replace("\r\n", ""));
                            //}
                            break;
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
                        case "value1":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "");
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "");
                            }
                            break;
                        case "anyOutStore":
                            string strAnyOutStore = "0";
                            //DataSet dsWayBillFlow = null;
                            //DataTable dtWayBillFlow = null;
                            //dsWayBillFlow = new T_WayBillFlow().getInOutStoreCount(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                            //if (dsWayBillFlow!=null)
                            //{
                            //    dtWayBillFlow=dsWayBillFlow.Tables[0];
                            //    if (dtWayBillFlow!=null && dtWayBillFlow.Rows.Count>0)
                            //    {
                            //        strAnyOutStore=dtWayBillFlow.Rows[0]["InOutStoreType3"].ToString();
                            //    }
                            //}
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], strAnyOutStore);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], strAnyOutStore);
                            }
                            break;
                        case "wbMoney_Custom":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "免费");
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "免费");
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
        public ActionResult Print(string order, string page, string rows, string sort, string txtCode, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage, string hidSearchType, string txtCompany)
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

            txtCode = Server.UrlDecode(txtCode.ToString());
            txtGCode = Server.UrlDecode(txtGCode.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());
            txtCompany = Server.UrlDecode(txtCompany.ToString());

            string strWhereTemp = "  (wbStatus = 2) ";

            switch (hidSearchType.ToString())
            {
                case "-1"://没有选择条件,则返回空记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + "  and  (1=2) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "   (1=2) ";
                    }
                    break;
                case "1"://选择普通选择
                    if (txtCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and  (wbSerialNum like '%" + txtCode.ToString() + "%')";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "   (wbSerialNum like '%" + txtCode.ToString() + "%')";
                        }
                    }
                    break;
                case "0"://选择高级查询
                    if (txtGCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (wbSerialNum like '%" + txtGCode.ToString() + "%')  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbSerialNum like '%" + txtGCode.ToString() + "%')  ";
                        }
                    }

                    if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'   ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'   ";
                        }
                    }

                    if (txtVoyage.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and wbVoyage like '%" + txtVoyage.ToString() + "%'  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  wbVoyage like '%" + txtVoyage.ToString() + "%'  ";
                        }
                    }

                    if (txtCompany.ToString() != "" && txtCompany.ToString() != "---请选择---")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and wbCompany like '%" + txtCompany.ToString() + "%'  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  wbCompany like '%" + txtCompany.ToString() + "%'  ";
                        }
                    }
                    break;
                default:
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + "  and  (1=2) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "   (1=2) ";
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
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbVoyage", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbActualTotalWeight__Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbMoney_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("subNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("value1", Type.GetType("System.String"));
            dtCustom.Columns.Add("anyOutStore", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbImportType", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbArrivalDate", Type.GetType("System.String"));
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
                        case "wbTotalWeight":
                            string strActualWeight = "0.00";
                            //double TotalActualWeight = tSubWayBill.GetTotalActualWeight(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));

                            //if (TotalActualWeight > double.Parse(dt.Rows[i]["wbTotalWeight"].ToString()))
                            //{
                            //    strActualWeight = TotalActualWeight.ToString();
                            //}
                            //else
                            //{
                            //    strActualWeight = double.Parse(dt.Rows[i]["wbTotalWeight"].ToString()).ToString();
                            //}

                            drCustom[strFiledArray[j]] = strActualWeight + "公斤";
                            break;
                        case "subNum":
                            drCustom[strFiledArray[j]] = tSubWayBill.GetActualSubNum(Convert.ToInt32(dt.Rows[i]["wbID"].ToString())).ToString().Replace("\r\n", "");
                            break;
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
                            drCustom["wbTotalWeight_Customize"] = swbTotalWeight + "公斤";
                            drCustom["wbActualTotalWeight__Customize"] = swbTotalActualWeight + "公斤";
                            break;
                        case "wbTotalWeight_Customize":
                            break;
                        case "wbActualTotalWeight__Customize":
                            break;
                        case "value1":
                            drCustom[strFiledArray[j]] = "";
                            break;
                        case "anyOutStore":
                            string strAnyOutStore = "0";
                            //DataSet dsWayBillFlow = null;
                            //DataTable dtWayBillFlow = null;
                            //dsWayBillFlow = new T_WayBillFlow().getInOutStoreCount(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                            //if (dsWayBillFlow != null)
                            //{
                            //    dtWayBillFlow = dsWayBillFlow.Tables[0];
                            //    if (dtWayBillFlow != null && dtWayBillFlow.Rows.Count > 0)
                            //    {
                            //        strAnyOutStore = dtWayBillFlow.Rows[0]["InOutStoreType3"].ToString();
                            //    }
                            //}
                            drCustom[strFiledArray[j]] = strAnyOutStore;
                            break;
                        case "wbMoney_Custom":
                            drCustom[strFiledArray[j]] = "免费";
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
            ReportDataSource reportDataSource = new ReportDataSource("HuayuRelese_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtCode, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage, string hidSearchType, string txtCompany, string browserType)
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

            txtCode = Server.UrlDecode(txtCode.ToString());
            txtGCode = Server.UrlDecode(txtGCode.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());
            txtCompany = Server.UrlDecode(txtCompany.ToString());

            string strWhereTemp = "  (wbStatus = 2) ";

            switch (hidSearchType.ToString())
            {
                case "-1"://没有选择条件,则返回空记录
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + "  and  (1=2) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "   (1=2) ";
                    }
                    break;
                case "1"://选择普通选择
                    if (txtCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and  (wbSerialNum like '%" + txtCode.ToString() + "%')";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "   (wbSerialNum like '%" + txtCode.ToString() + "%')";
                        }
                    }
                    break;
                case "0"://选择高级查询
                    if (txtGCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (wbSerialNum like '%" + txtGCode.ToString() + "%')  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  (wbSerialNum like '%" + txtGCode.ToString() + "%')  ";
                        }
                    }

                    if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'   ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'   ";
                        }
                    }

                    if (txtVoyage.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and wbVoyage like '%" + txtVoyage.ToString() + "%'  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  wbVoyage like '%" + txtVoyage.ToString() + "%'  ";
                        }
                    }

                    if (txtCompany.ToString() != "" && txtCompany.ToString() != "---请选择---")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + "  and wbCompany like '%" + txtCompany.ToString() + "%'  ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + "  wbCompany like '%" + txtCompany.ToString() + "%'  ";
                        }
                    }
                    break;
                default:
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + "  and  (1=2) ";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "   (1=2) ";
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
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbVoyage", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbActualTotalWeight__Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbMoney_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("subNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("value1", Type.GetType("System.String"));
            dtCustom.Columns.Add("anyOutStore", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbImportType", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbArrivalDate", Type.GetType("System.String"));
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
                        case "wbTotalWeight":
                            string strActualWeight = "0.00";
                            //double TotalActualWeight = tSubWayBill.GetTotalActualWeight(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));

                            //if (TotalActualWeight > double.Parse(dt.Rows[i]["wbTotalWeight"].ToString()))
                            //{
                            //    strActualWeight = TotalActualWeight.ToString();
                            //}
                            //else
                            //{
                            //    strActualWeight = double.Parse(dt.Rows[i]["wbTotalWeight"].ToString()).ToString();
                            //}

                            drCustom[strFiledArray[j]] = strActualWeight + "公斤";
                            break;
                        case "subNum":
                            drCustom[strFiledArray[j]] = tSubWayBill.GetActualSubNum(Convert.ToInt32(dt.Rows[i]["wbID"].ToString())).ToString().Replace("\r\n", "");
                            break;
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
                            drCustom["wbTotalWeight_Customize"] = swbTotalWeight + "公斤";
                            drCustom["wbActualTotalWeight__Customize"] = swbTotalActualWeight + "公斤";
                            break;
                        case "wbTotalWeight_Customize":
                            break;
                        case "wbActualTotalWeight__Customize":
                            break;
                        case "value1":
                            drCustom[strFiledArray[j]] = "";
                            break;
                        case "anyOutStore":
                            string strAnyOutStore = "0";
                            //DataSet dsWayBillFlow = null;
                            //DataTable dtWayBillFlow = null;
                            //dsWayBillFlow = new T_WayBillFlow().getInOutStoreCount(Convert.ToInt32(dt.Rows[i]["wbID"].ToString()));
                            //if (dsWayBillFlow != null)
                            //{
                            //    dtWayBillFlow = dsWayBillFlow.Tables[0];
                            //    if (dtWayBillFlow != null && dtWayBillFlow.Rows.Count > 0)
                            //    {
                            //        strAnyOutStore = dtWayBillFlow.Rows[0]["InOutStoreType3"].ToString();
                            //    }
                            //}
                            drCustom[strFiledArray[j]] = strAnyOutStore;
                            break;
                        case "wbMoney_Custom":
                            drCustom[strFiledArray[j]] = "免费";
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
            ReportDataSource reportDataSource = new ReportDataSource("HuayuRelese_DS", dtCustom);

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

            string strOutputFileName = "提货单信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        [HttpGet]
        public ActionResult PrintRelese(string wbSerialNum, string TotalNum, string SubNum, string value1, string TotalWeight)
        {
            string serialNum = Server.UrlDecode(wbSerialNum);
            int totalNum = int.Parse(Server.UrlDecode(TotalNum));
            int subNum = int.Parse(Server.UrlDecode(SubNum));
            string value = Server.UrlDecode(value1);
            string totalWeight = Server.UrlDecode(TotalWeight);

            int wbID = tWayBill.GetWayBillID(serialNum);
            int pirntTimes = tWayBill.getPrintStatus(wbID);
            int releseCount = tSubWayBill.GetReleseNum(wbID);
            int saveCount = tSubWayBill.GetSaveNum(wbID);

            DataTable saveDT = new DataTable();
            DataSet saveDs = tSubWayBill.GetSave(wbID); 
            StringBuilder saveStr = new StringBuilder();
            saveStr.Append("扣留货物分运单号： ");
            string strCode = "";
            if (saveDs != null)
            {
                saveDT = saveDs.Tables[0];
                for (int i = 0; i < saveDs.Tables[0].Rows.Count; i++)
                {
                    strCode += "[" + saveDs.Tables[0].Rows[i][1].ToString() + "]  ";
                    if (i % 6 == 0 && i != 0)
                    {
                        strCode += "\r\n";
                    }
                }

            }
            else
            {
                strCode = "无扣留货物";
            }
            saveStr.Append(strCode);


            StringBuilder momeyStr = new StringBuilder();
            momeyStr.Append("收费明细： ");
            momeyStr.Append("运单号：" + serialNum + "，总重量：" + totalWeight.Substring(0, totalWeight.Length - 2) + "公斤 ，操作费：" + value + "元。");


            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_RELEASE_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("Huayu_Print_Release_DS", saveDT);

            ReportParameter var_lbTimes = new ReportParameter("lbTimes", "第" + pirntTimes.ToString() + "批");
            ReportParameter var_lbReleseCode = new ReportParameter("lbReleseCode", DateTime.Now.ToString("yyyyMMdd") + serialNum);
            ReportParameter var_lbKACode = new ReportParameter("lbKACode", "TSN");
            ReportParameter var_lbSerialNum = new ReportParameter("lbSerialNum", serialNum);
            ReportParameter var_lbTotalNum = new ReportParameter("lbTotalNum", totalNum.ToString());
            ReportParameter var_lbSubNum = new ReportParameter("lbSubNum", subNum.ToString());
            ReportParameter var_lbReleseNum = new ReportParameter("lbReleseNum", releseCount.ToString());
            ReportParameter var_lbSaveNum = new ReportParameter("lbSaveNum", saveCount.ToString());
            ReportParameter var_txtSaveInfo = new ReportParameter("txtSaveInfo", saveStr.ToString());
            ReportParameter var_txtMoney = new ReportParameter("txtMoney", momeyStr.ToString());

            localReport.SetParameters(new ReportParameter[] { var_lbTimes });
            localReport.SetParameters(new ReportParameter[] { var_lbReleseCode });
            localReport.SetParameters(new ReportParameter[] { var_lbKACode });
            localReport.SetParameters(new ReportParameter[] { var_lbSerialNum });
            localReport.SetParameters(new ReportParameter[] { var_lbTotalNum });
            localReport.SetParameters(new ReportParameter[] { var_lbSubNum });
            localReport.SetParameters(new ReportParameter[] { var_lbReleseNum });
            localReport.SetParameters(new ReportParameter[] { var_lbSubNum });
            localReport.SetParameters(new ReportParameter[] { var_lbSaveNum });
            localReport.SetParameters(new ReportParameter[] { var_txtSaveInfo });
            localReport.SetParameters(new ReportParameter[] { var_txtMoney });

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
        public ActionResult ExcelRelese(string wbSerialNum, string TotalNum, string SubNum, string value1, string TotalWeight, string browserType)
        {
            string serialNum = Server.UrlDecode(wbSerialNum);
            int totalNum = int.Parse(Server.UrlDecode(TotalNum));
            int subNum = int.Parse(Server.UrlDecode(SubNum));
            string value = Server.UrlDecode(value1);
            string totalWeight = Server.UrlDecode(TotalWeight);

            int wbID = tWayBill.GetWayBillID(serialNum);
            int pirntTimes = tWayBill.getPrintStatus(wbID);
            int releseCount = tSubWayBill.GetReleseNum(wbID);
            int saveCount = tSubWayBill.GetSaveNum(wbID);

            DataTable saveDT = new DataTable();
            DataSet saveDs = tSubWayBill.GetSave(wbID);
            StringBuilder saveStr = new StringBuilder();
            saveStr.Append("扣留货物分运单号： ");
            string strCode = "";
            if (saveDs != null)
            {
                saveDT = saveDs.Tables[0];
                for (int i = 0; i < saveDs.Tables[0].Rows.Count; i++)
                {
                    strCode += "[" + saveDs.Tables[0].Rows[i][1].ToString() + "]  ";
                    if (i % 6 == 0 && i != 0)
                    {
                        strCode += "\r\n";
                    }
                }

            }
            else
            {
                strCode = "无扣留货物";
            }
            saveStr.Append(strCode);


            StringBuilder momeyStr = new StringBuilder();
            momeyStr.Append("收费明细： ");
            momeyStr.Append("运单号：" + serialNum + "，总重量：" + totalWeight.Substring(0, totalWeight.Length - 2) + "公斤 ，操作费：" + value + "元。");


            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_RELEASE_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("Huayu_Print_Release_DS", saveDT);

            ReportParameter var_lbTimes = new ReportParameter("lbTimes", "第" + pirntTimes.ToString() + "批");
            ReportParameter var_lbReleseCode = new ReportParameter("lbReleseCode", DateTime.Now.ToString("yyyyMMdd") + serialNum);
            ReportParameter var_lbKACode = new ReportParameter("lbKACode", "TSN");
            ReportParameter var_lbSerialNum = new ReportParameter("lbSerialNum", serialNum);
            ReportParameter var_lbTotalNum = new ReportParameter("lbTotalNum", totalNum.ToString());
            ReportParameter var_lbSubNum = new ReportParameter("lbSubNum", subNum.ToString());
            ReportParameter var_lbReleseNum = new ReportParameter("lbReleseNum", releseCount.ToString());
            ReportParameter var_lbSaveNum = new ReportParameter("lbSaveNum", saveCount.ToString());
            ReportParameter var_txtSaveInfo = new ReportParameter("txtSaveInfo", saveStr.ToString());
            ReportParameter var_txtMoney = new ReportParameter("txtMoney", momeyStr.ToString());

            //ReportParameter var_lbTimes = new ReportParameter("lbTimes", "");
            //ReportParameter var_lbReleseCode = new ReportParameter("lbReleseCode", "");
            //ReportParameter var_lbKACode = new ReportParameter("lbKACode", "TSN");
            //ReportParameter var_lbSerialNum = new ReportParameter("lbSerialNum", "");
            //ReportParameter var_lbTotalNum = new ReportParameter("lbTotalNum","");
            //ReportParameter var_lbSubNum = new ReportParameter("lbSubNum", "");
            //ReportParameter var_lbReleseNum = new ReportParameter("lbReleseNum", "");
            //ReportParameter var_lbSaveNum = new ReportParameter("lbSaveNum", "");
            //ReportParameter var_txtSaveInfo = new ReportParameter("txtSaveInfo","");
            //ReportParameter var_txtMoney = new ReportParameter("txtMoney", "");

            localReport.SetParameters(new ReportParameter[] { var_lbTimes });
            localReport.SetParameters(new ReportParameter[] { var_lbReleseCode });
            localReport.SetParameters(new ReportParameter[] { var_lbKACode });
            localReport.SetParameters(new ReportParameter[] { var_lbSerialNum });
            localReport.SetParameters(new ReportParameter[] { var_lbTotalNum });
            localReport.SetParameters(new ReportParameter[] { var_lbSubNum });
            localReport.SetParameters(new ReportParameter[] { var_lbReleseNum });
            localReport.SetParameters(new ReportParameter[] { var_lbSubNum });
            localReport.SetParameters(new ReportParameter[] { var_lbSaveNum });
            localReport.SetParameters(new ReportParameter[] { var_txtSaveInfo });
            localReport.SetParameters(new ReportParameter[] { var_txtMoney });

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

            string strOutputFileName = "提货单信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
