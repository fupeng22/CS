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

namespace CS.Controllers.customs
{
    [ErrorAttribute]
    public class Customer_QueryController : Controller
    {
        SQLDAL.T_WayBill tWayBill = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();
        public const string strFileds = "wbStorageDate,wbCompany,wbSerialNum,wbTotalNumber_Customize,wbTotalWeight_Customize,wbActualTotalWeight__Customize,wbTotalNumber,wbTotalWeight,wbStatus,releseNum,notReleseNum,wbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/CustomerQuery.rdlc";
        //
        // GET: /Forwarder_QueryCompany/
        [CustomerRequiresLoginAttribute]
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
        public string GetData(string order, string page, string rows, string sort, string txtCode, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage, string hidSearchType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Checking_WayBill";

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

            string strWhereTemp = "";
            switch (hidSearchType.ToString())
            {
                case "-1"://没有选择条件,则返回空记录
                    strWhereTemp = " 1=2 ";
                    break;
                case "1"://选择普通选择
                    if (txtCode.ToString() != "")
                    {
                        strWhereTemp = " (wbSerialNum like '%" + txtCode.ToString() + "%')";
                    }
                    else
                    {
                        strWhereTemp = " (wbSerialNum like '%%')";
                    }
                    break;
                case "0"://选择高级查询
                    if (txtGCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (wbSerialNum like '%" + txtGCode.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (wbSerialNum like '%" + txtGCode.ToString() + "%') ";
                        }
                    }

                    if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'";
                        }
                    }

                    if (txtVoyage.ToString() != "" && txtVoyage.ToString() != "---请选择---")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (wbCompany like '%" + txtVoyage.ToString() + "%')";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (wbCompany like '%" + txtVoyage.ToString() + "%')";
                        }
                    }

                    break;
                default:
                    strWhereTemp = " 1=2 ";
                    break;
            }

            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            int releseNum = -1;
            int notReleseNum = -1;

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
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
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

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], swbTotalNumber);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], swbTotalNumber);
                            }

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", "wbTotalWeight_Customize", swbTotalWeight);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", "wbTotalWeight_Customize", swbTotalWeight);
                            }

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", "wbActualTotalWeight__Customize", swbTotalActualWeight);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", "wbActualTotalWeight__Customize", swbTotalActualWeight);
                            }
                            break;
                        case "wbTotalWeight_Customize":
                            break;
                        case "wbActualTotalWeight__Customize":
                            break;
                        case "wbStatus":
                            releseNum = -1;
                            notReleseNum = -1;
                            if (j != strFiledArray.Length - 1)
                            {
                                if (dt.Rows[i][strFiledArray[j]].ToString() != "")
                                {
                                    switch (int.Parse(dt.Rows[i][strFiledArray[j]].ToString()))
                                    {
                                        case 0:
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "等待预检");
                                            break;
                                        case 1:
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "查验中");
                                            break;
                                        case 2:
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "已放行");
                                            break;
                                        default:
                                            sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "未知");
                                            break;
                                    }

                                    releseNum = tSubWayBill.GetActualReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));
                                    notReleseNum = tSubWayBill.GetActualNotReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));

                                    if (releseNum != -1)
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\",", "releseNum", releseNum.ToString());
                                    }
                                    else
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\",", "releseNum", "无");

                                    }
                                    if (notReleseNum != -1)
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\",", "notReleseNum", notReleseNum.ToString());

                                    }
                                    else
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\",", "notReleseNum", "无");
                                    }

                                }

                            }
                            else
                            {
                                if (dt.Rows[i][strFiledArray[j]].ToString() != "")
                                {
                                    switch (int.Parse(dt.Rows[i][strFiledArray[j]].ToString()))
                                    {
                                        case 0:
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "等待预检");
                                            break;
                                        case 1:
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "查验中");
                                            break;
                                        case 2:
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "已放行");
                                            break;
                                        default:
                                            sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "未知");
                                            break;
                                    }
                                    releseNum = tSubWayBill.GetActualReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));
                                    notReleseNum = tSubWayBill.GetActualNotReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));

                                    if (releseNum != -1)
                                    {

                                        sb.AppendFormat("\"{0}\":\"{1}\"", "releseNum", releseNum.ToString());
                                    }
                                    else
                                    {

                                        sb.AppendFormat("\"{0}\":\"{1}\"", "releseNum", "无");

                                    }
                                    if (notReleseNum != -1)
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\"", "notReleseNum", notReleseNum.ToString());

                                    }
                                    else
                                    {
                                        sb.AppendFormat("\"{0}\":\"{1}\"", "notReleseNum", "无");
                                    }

                                }
                            }
                            break;
                        case "releseNum":
                            break;
                        case "notReleseNum":
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
        public ActionResult Print(string order, string page, string rows, string sort, string txtCode, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage, string hidSearchType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Checking_WayBill";

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

            string strWhereTemp = "";
            switch (hidSearchType.ToString())
            {
                case "-1"://没有选择条件,则返回空记录
                    strWhereTemp = " 1=2 ";
                    break;
                case "1"://选择普通选择
                    if (txtCode.ToString() != "")
                    {
                        strWhereTemp = " (wbSerialNum like '%" + txtCode.ToString() + "%')";
                    }
                    else
                    {
                        strWhereTemp = " (wbSerialNum like '%%')";
                    }
                    break;
                case "0"://选择高级查询
                    if (txtGCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (wbSerialNum like '%" + txtGCode.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (wbSerialNum like '%" + txtGCode.ToString() + "%') ";
                        }
                    }

                    if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'";
                        }
                    }

                    if (txtVoyage.ToString() != "" && txtVoyage.ToString() != "---请选择---")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (wbCompany like '%" + txtVoyage.ToString() + "%')";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (wbCompany like '%" + txtVoyage.ToString() + "%')";
                        }
                    }

                    break;
                default:
                    strWhereTemp = " 1=2 ";
                    break;
            }

            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            int releseNum = -1;
            int notReleseNum = -1;

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbActualTotalWeight__Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("releseNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("notReleseNum", Type.GetType("System.String"));
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
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")) + "公斤";
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
                        case "wbStatus":
                            releseNum = -1;
                            notReleseNum = -1;
                            if (j != strFiledArray.Length - 1)
                            {
                                if (dt.Rows[i][strFiledArray[j]].ToString() != "")
                                {
                                    switch (int.Parse(dt.Rows[i][strFiledArray[j]].ToString()))
                                    {
                                        case 0:
                                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "等待预检";
                                            break;
                                        case 1:
                                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "查验中";
                                            break;
                                        case 2:
                                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "已放行";
                                            break;
                                        default:
                                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "未知";
                                            break;
                                    }

                                    releseNum = tSubWayBill.GetActualReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));
                                    notReleseNum = tSubWayBill.GetActualNotReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));

                                    if (releseNum != -1)
                                    {
                                        drCustom["releseNum"] = releseNum.ToString();
                                    }
                                    else
                                    {
                                        drCustom["releseNum"] = "无";

                                    }
                                    if (notReleseNum != -1)
                                    {
                                        drCustom["notReleseNum"] = releseNum.ToString();
                                    }
                                    else
                                    {
                                        drCustom["notReleseNum"] = "无";
                                    }

                                }

                            }
                            else
                            {
                                if (dt.Rows[i][strFiledArray[j]].ToString() != "")
                                {
                                    switch (int.Parse(dt.Rows[i][strFiledArray[j]].ToString()))
                                    {
                                        case 0:
                                            drCustom[strFiledArray[j]] =  "等待预检";
                                            break;
                                        case 1:
                                            drCustom[strFiledArray[j]] = "查验中";
                                            break;
                                        case 2:
                                            drCustom[strFiledArray[j]] = "已放行";
                                            break;
                                        default:
                                            drCustom[strFiledArray[j]] = "未知";
                                            break;
                                    }
                                    releseNum = tSubWayBill.GetActualReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));
                                    notReleseNum = tSubWayBill.GetActualNotReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));

                                    if (releseNum != -1)
                                    {
                                        drCustom["releseNum"] = releseNum.ToString();
                                    }
                                    else
                                    {
                                        drCustom["releseNum"] = "无";

                                    }
                                    if (notReleseNum != -1)
                                    {
                                        drCustom["notReleseNum"] = releseNum.ToString();
                                    }
                                    else
                                    {
                                        drCustom["notReleseNum"] = "无";
                                    }

                                }
                            }
                            break;
                        case "releseNum":
                            break;
                        case "notReleseNum":
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
            ReportDataSource reportDataSource = new ReportDataSource("CustomerQuery_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtCode, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage, string hidSearchType, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Checking_WayBill";

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

            string strWhereTemp = "";
            switch (hidSearchType.ToString())
            {
                case "-1"://没有选择条件,则返回空记录
                    strWhereTemp = " 1=2 ";
                    break;
                case "1"://选择普通选择
                    if (txtCode.ToString() != "")
                    {
                        strWhereTemp = " (wbSerialNum like '%" + txtCode.ToString() + "%')";
                    }
                    else
                    {
                        strWhereTemp = " (wbSerialNum like '%%')";
                    }
                    break;
                case "0"://选择高级查询
                    if (txtGCode.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (wbSerialNum like '%" + txtGCode.ToString() + "%') ";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (wbSerialNum like '%" + txtGCode.ToString() + "%') ";
                        }
                    }

                    if (inputBeginDate.ToString() != "" && inputEndDate.ToString() != "")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "'";
                        }
                    }

                    if (txtVoyage.ToString() != "" && txtVoyage.ToString() != "---请选择---")
                    {
                        if (strWhereTemp != "")
                        {
                            strWhereTemp = strWhereTemp + " and (wbCompany like '%" + txtVoyage.ToString() + "%')";
                        }
                        else
                        {
                            strWhereTemp = strWhereTemp + " (wbCompany like '%" + txtVoyage.ToString() + "%')";
                        }
                    }

                    break;
                default:
                    strWhereTemp = " 1=2 ";
                    break;
            }

            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            int releseNum = -1;
            int notReleseNum = -1;

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbActualTotalWeight__Customize", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbStatus", Type.GetType("System.String"));
            dtCustom.Columns.Add("releseNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("notReleseNum", Type.GetType("System.String"));
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
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")) + "公斤";
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
                        case "wbStatus":
                            releseNum = -1;
                            notReleseNum = -1;
                            if (j != strFiledArray.Length - 1)
                            {
                                if (dt.Rows[i][strFiledArray[j]].ToString() != "")
                                {
                                    switch (int.Parse(dt.Rows[i][strFiledArray[j]].ToString()))
                                    {
                                        case 0:
                                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "等待预检";
                                            break;
                                        case 1:
                                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "查验中";
                                            break;
                                        case 2:
                                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "已放行";
                                            break;
                                        default:
                                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : "未知";
                                            break;
                                    }

                                    releseNum = tSubWayBill.GetActualReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));
                                    notReleseNum = tSubWayBill.GetActualNotReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));

                                    if (releseNum != -1)
                                    {
                                        drCustom["releseNum"] = releseNum.ToString();
                                    }
                                    else
                                    {
                                        drCustom["releseNum"] = "无";

                                    }
                                    if (notReleseNum != -1)
                                    {
                                        drCustom["notReleseNum"] = releseNum.ToString();
                                    }
                                    else
                                    {
                                        drCustom["notReleseNum"] = "无";
                                    }

                                }

                            }
                            else
                            {
                                if (dt.Rows[i][strFiledArray[j]].ToString() != "")
                                {
                                    switch (int.Parse(dt.Rows[i][strFiledArray[j]].ToString()))
                                    {
                                        case 0:
                                            drCustom[strFiledArray[j]] = "等待预检";
                                            break;
                                        case 1:
                                            drCustom[strFiledArray[j]] = "查验中";
                                            break;
                                        case 2:
                                            drCustom[strFiledArray[j]] = "已放行";
                                            break;
                                        default:
                                            drCustom[strFiledArray[j]] = "未知";
                                            break;
                                    }
                                    releseNum = tSubWayBill.GetActualReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));
                                    notReleseNum = tSubWayBill.GetActualNotReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));

                                    if (releseNum != -1)
                                    {
                                        drCustom["releseNum"] = releseNum.ToString();
                                    }
                                    else
                                    {
                                        drCustom["releseNum"] = "无";

                                    }
                                    if (notReleseNum != -1)
                                    {
                                        drCustom["notReleseNum"] = releseNum.ToString();
                                    }
                                    else
                                    {
                                        drCustom["notReleseNum"] = "无";
                                    }

                                }
                            }
                            break;
                        case "releseNum":
                            break;
                        case "notReleseNum":
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
            ReportDataSource reportDataSource = new ReportDataSource("CustomerQuery_DS", dtCustom);

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

            string strOutputFileName = "统计信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
