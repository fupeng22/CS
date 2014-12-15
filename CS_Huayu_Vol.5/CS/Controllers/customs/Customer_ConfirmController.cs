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
    public class Customer_ConfirmController : Controller
    {
        Model.M_WayBill wayBillModel = new M_WayBill();
        Model.M_SubWayBill subWayBillModel = new M_SubWayBill();
        SQLDAL.T_WayBill wayBillSql = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();

        public const string strFileds = "wbStorageDate,wbCompany,wbSerialNum,wbSubNumber_Custom,wbReleaseCount_Custom,wbUnReleaseCount_Custom,wbNotProCount_Custom,wbImportType,wbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/CustomerConfirm.rdlc";
        //
        // GET: /Customer_Confirm/
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
        public string GetData1(string order, string page, string rows, string sort, string ddStatus, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage)
        {
            string strSQLQuery = "";
            string strTemp = "";
            int iRowsCount = 0;
            int iMaxCount = 0;

            strSQLQuery = "select * from V_Distinct_WayBill {0} ";

            ddStatus = Server.UrlDecode(ddStatus.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate.ToString());
            txtGCode = Server.UrlDecode(txtGCode.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());

            if (ddStatus == "-1" || ddStatus == "1")//查看已预检
            {
                strTemp = " where ( wbStatus=1 or wbStatus=2 ) ";
            }
            else if (ddStatus == "0")//查看未预检
            {
                strTemp = " where (  wbStatus=0 ) ";
            }

            if (inputBeginDate != "" && inputEndDate != "")
            {
                strTemp = strTemp + " and (  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "' ) ";
            }

            if (txtGCode != "")
            {
                strTemp = strTemp + " and (  wbSerialNum like '%" + txtGCode + "%') ";
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                strTemp = strTemp + " and (  wbCompany like '%" + txtVoyage + "%') ";
            }

            strTemp = strTemp + " order by " + sort + " " + order;

            DataSet ds = SqlServerHelper.Query(string.Format(strSQLQuery, strTemp));
            DataTable dt = ds.Tables[0];
            Boolean bNeedCheck = false;

            Dictionary<string, int> dic_detainNum = null;
            Dictionary<string, Struct_SumInfo> dic_SumInfo = null;

            DataSet dsSubWayBillInfo = null;
            DataTable dtSubWayBillInfo = null;
            dsSubWayBillInfo = tSubWayBill.GetAllSubWayBillInfo();
            if (dsSubWayBillInfo != null)
            {
                dtSubWayBillInfo = dsSubWayBillInfo.Tables[0];
            }

            //if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
            //{
            //    for (int m = 0; m < dtSubWayBillInfo.Rows.Count; m++)
            //    {
            //        if (dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString() == dt.Rows[i]["wbID"].ToString())
            //        {
            //            if (Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"].ToString()) == 3)
            //            {
            //                detainNum = detainNum + 1;
            //                break;
            //            }
            //        }
            //    }
            //}
            dic_detainNum = new Dictionary<string, int>();
            dic_SumInfo = new Dictionary<string, Struct_SumInfo>();
            if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
            {
                for (int m = 0; m < dtSubWayBillInfo.Rows.Count; m++)
                {
                    if (Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"].ToString()) == 3)
                    {
                        if (dic_detainNum.ContainsKey(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()))
                        {
                            dic_detainNum[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()] = dic_detainNum[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()] + 1;
                        }
                        else
                        {
                            dic_detainNum.Add(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString(), 1);
                        }
                    }

                    if (dic_SumInfo.ContainsKey(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()))
                    {
                        if (((Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"]) == 0) && (dtSubWayBillInfo.Rows[m]["swbSortingTime"] != DBNull.Value)) || ((Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"]) == 2) && (dtSubWayBillInfo.Rows[m]["swbSortingTime"] != DBNull.Value)))//(swbNeedCheck=0 or swbNeedCheck=2) and swbSortingTime is not null
                        {
                            dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].releseNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].releseNum + 1;
                        }
                        if (Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"]) == 3)//swbNeedCheck=3
                        {
                            dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notReleseNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notReleseNum + 1;
                        }
                        if (dtSubWayBillInfo.Rows[m]["swbSortingTime"] == DBNull.Value)// swbSortingTime is null 
                        {
                            dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notProNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notProNum + 1;
                        }

                        dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].subNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].subNum + 1;
                    }
                    else
                    {
                        dic_SumInfo.Add(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString(), new Struct_SumInfo()
                        {
                            releseNum = 0,
                            notReleseNum = 0,
                            notProNum = 0,
                            subNum = 0,
                        });
                    }
                }
            }

            DataTable dt_Actual = new DataTable();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt_Actual.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bNeedCheck = false;
                if ("2" == dt.Rows[i]["wbStatus"].ToString())
                {
                    int detainNum = 0;
                    // swb_wbID=" + wbID + " and  swbNeedCheck=3 "
                    if (dic_detainNum.ContainsKey(dt.Rows[i]["wbID"].ToString()))
                    {
                        detainNum = dic_detainNum[dt.Rows[i]["wbID"].ToString()];
                    }
                    //detainNum= tSubWayBill.GetActualNotReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));
                    if (detainNum != 0)
                    {
                        bNeedCheck = true;
                    }
                }
                else
                {
                    bNeedCheck = true;
                }

                if (bNeedCheck)
                {
                    DataRow row = dt_Actual.NewRow();
                    for (int k = 0; k < row.ItemArray.Length; k++)
                    {
                        row[k] = dt.Rows[i][k];
                    }
                    dt_Actual.Rows.Add(row);

                    iRowsCount = iRowsCount + 1;
                }
            }

            if (Convert.ToInt32(page) > iRowsCount / Convert.ToInt32(rows) && Convert.ToInt32(page) <= iRowsCount / Convert.ToInt32(rows) + 1)
            {
                iMaxCount = iRowsCount;
            }
            else
            {
                iMaxCount = Convert.ToInt32(rows) * Convert.ToInt32(page);
            }

            StringBuilder sb = new StringBuilder("");
            sb.Append("{");
            sb.AppendFormat("\"total\":{0}", iRowsCount);
            sb.Append(",\"rows\":[");
            for (int i = Convert.ToInt32(rows) * (Convert.ToInt32(page) - 1); i < iMaxCount; i++)
            {
                sb.Append("{");

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    //获取称重总重量
                    int releseNum = 0;
                    int notReleseNum = 0;
                    int notProNum = 0;
                    int subNum = 0;

                    //releseNum = tSubWayBill.GetActualReleseNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    //notReleseNum = tSubWayBill.GetActualNotReleseNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    //notProNum = tSubWayBill.GetActualNotProNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    //subNum = tSubWayBill.GetActualSubNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    if (dic_SumInfo.ContainsKey(dt_Actual.Rows[i]["wbID"].ToString()))
                    {
                        releseNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].releseNum;
                        notReleseNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].notReleseNum;
                        notProNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].notProNum;
                        subNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].subNum;
                    }

                    //if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
                    //{
                    //    for (int k = 0; k < dtSubWayBillInfo.Rows.Count; k++)
                    //    {
                    //        if (dtSubWayBillInfo.Rows[k]["swb_wbID"].ToString() == dt_Actual.Rows[i]["wbID"].ToString())
                    //        {
                    //            if (((Convert.ToInt32(dtSubWayBillInfo.Rows[k]["swbNeedCheck"]) == 0) && (dtSubWayBillInfo.Rows[k]["swbSortingTime"] != DBNull.Value)) || ((Convert.ToInt32(dtSubWayBillInfo.Rows[k]["swbNeedCheck"]) == 2) && (dtSubWayBillInfo.Rows[k]["swbSortingTime"] != DBNull.Value)))//(swbNeedCheck=0 or swbNeedCheck=2) and swbSortingTime is not null
                    //            {
                    //                releseNum = releseNum + 1;
                    //            }
                    //            if (Convert.ToInt32(dtSubWayBillInfo.Rows[k]["swbNeedCheck"]) == 3)//swbNeedCheck=3
                    //            {
                    //                notReleseNum = notReleseNum + 1;
                    //            }
                    //            if (dtSubWayBillInfo.Rows[k]["swbSortingTime"] == DBNull.Value)// swbSortingTime is null 
                    //            {
                    //                notProNum = notProNum + 1;
                    //            }

                    //            subNum = subNum + 1;

                    //        }

                    //    }

                    //}

                    switch (strFiledArray[j])
                    {
                        case "wbCompany"://格式化公司(保存的是用户名，取出公司名)
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString());
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString());
                            }
                            break;
                        case "wbSubNumber_Custom":
                            if (subNum != -1)
                            {
                                if (j != strFiledArray.Length - 1)
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], subNum);
                                }
                                else
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], subNum);
                                }
                            }
                            else
                            {
                                if (j != strFiledArray.Length - 1)
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "无");
                                }
                                else
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "无");
                                }
                            }
                            break;
                        case "wbReleaseCount_Custom":
                            if (releseNum != -1)
                            {
                                if (j != strFiledArray.Length - 1)
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], releseNum);
                                }
                                else
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], releseNum);
                                }
                            }
                            else
                            {
                                if (j != strFiledArray.Length - 1)
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "无");
                                }
                                else
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "无");
                                }
                            }
                            break;
                        case "wbUnReleaseCount_Custom":
                            if (notReleseNum != -1)
                            {
                                if (j != strFiledArray.Length - 1)
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], notReleseNum);
                                }
                                else
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], notReleseNum);
                                }
                            }
                            else
                            {
                                if (j != strFiledArray.Length - 1)
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "无");
                                }
                                else
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "无");
                                }
                            }
                            break;
                        case "wbNotProCount_Custom":
                            if (notProNum != -1)
                            {
                                if (j != strFiledArray.Length - 1)
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], notProNum);
                                }
                                else
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], notProNum);
                                }
                            }
                            else
                            {
                                if (j != strFiledArray.Length - 1)
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], "无");
                                }
                                else
                                {
                                    sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], "无");
                                }
                            }
                            break;
                        default:
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt_Actual.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Actual.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt_Actual.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Actual.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            break;
                    }
                }

                if (i == dt_Actual.Rows.Count - 1)
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
        public ActionResult Print1(string order, string page, string rows, string sort, string ddStatus, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage)
        {
            string strSQLQuery = "";
            string strTemp = "";
            int iRowsCount = 0;
            int iMaxCount = 0;

            strSQLQuery = "select * from V_Checking_WayBill {0} ";

            ddStatus = Server.UrlDecode(ddStatus.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate.ToString());
            txtGCode = Server.UrlDecode(txtGCode.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());

            if (ddStatus == "-1" || ddStatus == "1")//查看已预检
            {
                strTemp = " where ( wbStatus=1 or wbStatus=2 ) ";
            }
            else if (ddStatus == "0")//查看未预检
            {
                strTemp = " where (  wbStatus=0 ) ";
            }

            if (inputBeginDate != "" && inputEndDate != "")
            {
                strTemp = strTemp + " and (  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "' ) ";
            }

            if (txtGCode != "")
            {
                strTemp = strTemp + " and (  wbSerialNum like '%" + txtGCode + "%') ";
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                strTemp = strTemp + " and (  wbCompany like '%" + txtVoyage + "%') ";
            }

            strTemp = strTemp + " order by " + sort + " " + order;

            DataSet ds = SqlServerHelper.Query(string.Format(strSQLQuery, strTemp));
            DataTable dt = ds.Tables[0];
            Boolean bNeedCheck = false;

            Dictionary<string, int> dic_detainNum = null;
            Dictionary<string, Struct_SumInfo> dic_SumInfo = null;

            DataSet dsSubWayBillInfo = null;
            DataTable dtSubWayBillInfo = null;
            dsSubWayBillInfo = tSubWayBill.GetAllSubWayBillInfo();
            if (dsSubWayBillInfo != null)
            {
                dtSubWayBillInfo = dsSubWayBillInfo.Tables[0];
            }

            //if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
            //{
            //    for (int m = 0; m < dtSubWayBillInfo.Rows.Count; m++)
            //    {
            //        if (dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString() == dt.Rows[i]["wbID"].ToString())
            //        {
            //            if (Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"].ToString()) == 3)
            //            {
            //                detainNum = detainNum + 1;
            //                break;
            //            }
            //        }
            //    }
            //}
            dic_detainNum = new Dictionary<string, int>();
            dic_SumInfo = new Dictionary<string, Struct_SumInfo>();
            if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
            {
                for (int m = 0; m < dtSubWayBillInfo.Rows.Count; m++)
                {
                    if (Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"].ToString()) == 3)
                    {
                        if (dic_detainNum.ContainsKey(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()))
                        {
                            dic_detainNum[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()] = dic_detainNum[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()] + 1;
                        }
                        else
                        {
                            dic_detainNum.Add(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString(), 1);
                        }
                    }

                    if (dic_SumInfo.ContainsKey(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()))
                    {
                        if (((Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"]) == 0) && (dtSubWayBillInfo.Rows[m]["swbSortingTime"] != DBNull.Value)) || ((Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"]) == 2) && (dtSubWayBillInfo.Rows[m]["swbSortingTime"] != DBNull.Value)))//(swbNeedCheck=0 or swbNeedCheck=2) and swbSortingTime is not null
                        {
                            dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].releseNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].releseNum + 1;
                        }
                        if (Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"]) == 3)//swbNeedCheck=3
                        {
                            dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notReleseNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notReleseNum + 1;
                        }
                        if (dtSubWayBillInfo.Rows[m]["swbSortingTime"] == DBNull.Value)// swbSortingTime is null 
                        {
                            dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notProNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notProNum + 1;
                        }

                        dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].subNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].subNum + 1;
                    }
                    else
                    {
                        dic_SumInfo.Add(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString(), new Struct_SumInfo()
                        {
                            releseNum = 0,
                            notReleseNum = 0,
                            notProNum = 0,
                            subNum = 0,
                        });
                    }
                }
            }

            DataTable dt_Actual = new DataTable();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt_Actual.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bNeedCheck = false;
                if ("2" == dt.Rows[i]["wbStatus"].ToString())
                {
                    int detainNum = 0;
                    // swb_wbID=" + wbID + " and  swbNeedCheck=3 "
                    if (dic_detainNum.ContainsKey(dt.Rows[i]["wbID"].ToString()))
                    {
                        detainNum = dic_detainNum[dt.Rows[i]["wbID"].ToString()];
                    }
                    //detainNum= tSubWayBill.GetActualNotReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));
                    if (detainNum != 0)
                    {
                        bNeedCheck = true;
                    }
                }
                else
                {
                    bNeedCheck = true;
                }

                if (bNeedCheck)
                {
                    DataRow row = dt_Actual.NewRow();
                    for (int k = 0; k < row.ItemArray.Length; k++)
                    {
                        row[k] = dt.Rows[i][k];
                    }
                    dt_Actual.Rows.Add(row);

                    iRowsCount = iRowsCount + 1;
                }
            }

            if (Convert.ToInt32(page) > iRowsCount / Convert.ToInt32(rows) && Convert.ToInt32(page) <= iRowsCount / Convert.ToInt32(rows) + 1)
            {
                iMaxCount = iRowsCount;
            }
            else
            {
                iMaxCount = Convert.ToInt32(rows) * Convert.ToInt32(page);
            }
            //,,,,,,,
            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSubNumber_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbReleaseCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbUnReleaseCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbNotProCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = Convert.ToInt32(rows) * (Convert.ToInt32(page) - 1); i < iMaxCount; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    //获取称重总重量
                    int releseNum = -1;
                    int notReleseNum = -1;
                    int notProNum = -1;
                    int subNum = -1;

                    //releseNum = tSubWayBill.GetActualReleseNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    //notReleseNum = tSubWayBill.GetActualNotReleseNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    //notProNum = tSubWayBill.GetActualNotProNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    //subNum = tSubWayBill.GetActualSubNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    if (dic_SumInfo.ContainsKey(dt_Actual.Rows[i]["wbID"].ToString()))
                    {
                        releseNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].releseNum;
                        notReleseNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].notReleseNum;
                        notProNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].notProNum;
                        subNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].subNum;
                    }

                    //if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
                    //{
                    //    for (int k = 0; k < dtSubWayBillInfo.Rows.Count; k++)
                    //    {
                    //        if (dtSubWayBillInfo.Rows[k]["swb_wbID"].ToString() == dt_Actual.Rows[i]["wbID"].ToString())
                    //        {
                    //            if (((Convert.ToInt32(dtSubWayBillInfo.Rows[k]["swbNeedCheck"]) == 0) && (dtSubWayBillInfo.Rows[k]["swbSortingTime"] != DBNull.Value)) || ((Convert.ToInt32(dtSubWayBillInfo.Rows[k]["swbNeedCheck"]) == 2) && (dtSubWayBillInfo.Rows[k]["swbSortingTime"] != DBNull.Value)))//(swbNeedCheck=0 or swbNeedCheck=2) and swbSortingTime is not null
                    //            {
                    //                releseNum = releseNum + 1;
                    //            }
                    //            if (Convert.ToInt32(dtSubWayBillInfo.Rows[k]["swbNeedCheck"]) == 3)//swbNeedCheck=3
                    //            {
                    //                notReleseNum = notReleseNum + 1;
                    //            }
                    //            if (dtSubWayBillInfo.Rows[k]["swbSortingTime"] == DBNull.Value)// swbSortingTime is null 
                    //            {
                    //                notProNum = notProNum + 1;
                    //            }

                    //            subNum = subNum + 1;

                    //        }

                    //    }

                    //}

                    switch (strFiledArray[j])
                    {
                        case "wbCompany"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                            break;
                        case "wbSubNumber_Custom":
                            if (subNum != -1)
                            {
                                drCustom[strFiledArray[j]] = subNum;
                            }
                            else
                            {
                                drCustom[strFiledArray[j]] = "无";
                            }
                            break;
                        case "wbReleaseCount_Custom":
                            if (releseNum != -1)
                            {
                                drCustom[strFiledArray[j]] = releseNum;
                            }
                            else
                            {
                                drCustom[strFiledArray[j]] = "无";
                            }
                            break;
                        case "wbUnReleaseCount_Custom":
                            if (notReleseNum != -1)
                            {
                                drCustom[strFiledArray[j]] = notReleseNum;
                            }
                            else
                            {
                                drCustom[strFiledArray[j]] = "无";
                            }
                            break;
                        case "wbNotProCount_Custom":
                            if (notProNum != -1)
                            {
                                drCustom[strFiledArray[j]] = notProNum;
                            }
                            else
                            {
                                drCustom[strFiledArray[j]] = "无";
                            }
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
            ReportDataSource reportDataSource = new ReportDataSource("CustomerConfirm_DS", dtCustom);

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
        public ActionResult Excel1(string order, string page, string rows, string sort, string ddStatus, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage, string browserType)
        {
            string strSQLQuery = "";
            string strTemp = "";
            int iRowsCount = 0;
            int iMaxCount = 0;

            strSQLQuery = "select * from V_Checking_WayBill {0} ";

            ddStatus = Server.UrlDecode(ddStatus.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate.ToString());
            txtGCode = Server.UrlDecode(txtGCode.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());

            if (ddStatus == "-1" || ddStatus == "1")//查看已预检
            {
                strTemp = " where ( wbStatus=1 or wbStatus=2 ) ";
            }
            else if (ddStatus == "0")//查看未预检
            {
                strTemp = " where (  wbStatus=0 ) ";
            }

            if (inputBeginDate != "" && inputEndDate != "")
            {
                strTemp = strTemp + " and (  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "' ) ";
            }

            if (txtGCode != "")
            {
                strTemp = strTemp + " and (  wbSerialNum like '%" + txtGCode + "%') ";
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                strTemp = strTemp + " and (  wbCompany like '%" + txtVoyage + "%') ";
            }

            strTemp = strTemp + " order by " + sort + " " + order;

            DataSet ds = SqlServerHelper.Query(string.Format(strSQLQuery, strTemp));
            DataTable dt = ds.Tables[0];
            Boolean bNeedCheck = false;

            Dictionary<string, int> dic_detainNum = null;
            Dictionary<string, Struct_SumInfo> dic_SumInfo = null;

            DataSet dsSubWayBillInfo = null;
            DataTable dtSubWayBillInfo = null;
            dsSubWayBillInfo = tSubWayBill.GetAllSubWayBillInfo();
            if (dsSubWayBillInfo != null)
            {
                dtSubWayBillInfo = dsSubWayBillInfo.Tables[0];
            }

            //if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
            //{
            //    for (int m = 0; m < dtSubWayBillInfo.Rows.Count; m++)
            //    {
            //        if (dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString() == dt.Rows[i]["wbID"].ToString())
            //        {
            //            if (Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"].ToString()) == 3)
            //            {
            //                detainNum = detainNum + 1;
            //                break;
            //            }
            //        }
            //    }
            //}
            dic_detainNum = new Dictionary<string, int>();
            dic_SumInfo = new Dictionary<string, Struct_SumInfo>();
            if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
            {
                for (int m = 0; m < dtSubWayBillInfo.Rows.Count; m++)
                {
                    if (Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"].ToString()) == 3)
                    {
                        if (dic_detainNum.ContainsKey(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()))
                        {
                            dic_detainNum[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()] = dic_detainNum[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()] + 1;
                        }
                        else
                        {
                            dic_detainNum.Add(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString(), 1);
                        }
                    }

                    if (dic_SumInfo.ContainsKey(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()))
                    {
                        if (((Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"]) == 0) && (dtSubWayBillInfo.Rows[m]["swbSortingTime"] != DBNull.Value)) || ((Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"]) == 2) && (dtSubWayBillInfo.Rows[m]["swbSortingTime"] != DBNull.Value)))//(swbNeedCheck=0 or swbNeedCheck=2) and swbSortingTime is not null
                        {
                            dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].releseNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].releseNum + 1;
                        }
                        if (Convert.ToInt32(dtSubWayBillInfo.Rows[m]["swbNeedCheck"]) == 3)//swbNeedCheck=3
                        {
                            dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notReleseNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notReleseNum + 1;
                        }
                        if (dtSubWayBillInfo.Rows[m]["swbSortingTime"] == DBNull.Value)// swbSortingTime is null 
                        {
                            dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notProNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].notProNum + 1;
                        }

                        dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].subNum = dic_SumInfo[dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString()].subNum + 1;
                    }
                    else
                    {
                        dic_SumInfo.Add(dtSubWayBillInfo.Rows[m]["swb_wbID"].ToString(), new Struct_SumInfo()
                        {
                            releseNum = 0,
                            notReleseNum = 0,
                            notProNum = 0,
                            subNum = 0,
                        });
                    }
                }
            }

            DataTable dt_Actual = new DataTable();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt_Actual.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bNeedCheck = false;
                if ("2" == dt.Rows[i]["wbStatus"].ToString())
                {
                    int detainNum = 0;
                    // swb_wbID=" + wbID + " and  swbNeedCheck=3 "
                    if (dic_detainNum.ContainsKey(dt.Rows[i]["wbID"].ToString()))
                    {
                        detainNum = dic_detainNum[dt.Rows[i]["wbID"].ToString()];
                    }
                    //detainNum= tSubWayBill.GetActualNotReleseNum(int.Parse(dt.Rows[i]["wbID"].ToString()));
                    if (detainNum != 0)
                    {
                        bNeedCheck = true;
                    }
                }
                else
                {
                    bNeedCheck = true;
                }

                if (bNeedCheck)
                {
                    DataRow row = dt_Actual.NewRow();
                    for (int k = 0; k < row.ItemArray.Length; k++)
                    {
                        row[k] = dt.Rows[i][k];
                    }
                    dt_Actual.Rows.Add(row);

                    iRowsCount = iRowsCount + 1;
                }
            }

            if (Convert.ToInt32(page) > iRowsCount / Convert.ToInt32(rows) && Convert.ToInt32(page) <= iRowsCount / Convert.ToInt32(rows) + 1)
            {
                iMaxCount = iRowsCount;
            }
            else
            {
                iMaxCount = Convert.ToInt32(rows) * Convert.ToInt32(page);
            }
            //,,,,,,,
            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSubNumber_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbReleaseCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbUnReleaseCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbNotProCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = Convert.ToInt32(rows) * (Convert.ToInt32(page) - 1); i < iMaxCount; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    //获取称重总重量
                    int releseNum = -1;
                    int notReleseNum = -1;
                    int notProNum = -1;
                    int subNum = -1;

                    //releseNum = tSubWayBill.GetActualReleseNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    //notReleseNum = tSubWayBill.GetActualNotReleseNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    //notProNum = tSubWayBill.GetActualNotProNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    //subNum = tSubWayBill.GetActualSubNum(int.Parse(dt_Actual.Rows[i]["wbID"].ToString()));
                    if (dic_SumInfo.ContainsKey(dt_Actual.Rows[i]["wbID"].ToString()))
                    {
                        releseNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].releseNum;
                        notReleseNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].notReleseNum;
                        notProNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].notProNum;
                        subNum = dic_SumInfo[dt_Actual.Rows[i]["wbID"].ToString()].subNum;
                    }

                    //if (dtSubWayBillInfo != null && dtSubWayBillInfo.Rows.Count > 0)
                    //{
                    //    for (int k = 0; k < dtSubWayBillInfo.Rows.Count; k++)
                    //    {
                    //        if (dtSubWayBillInfo.Rows[k]["swb_wbID"].ToString() == dt_Actual.Rows[i]["wbID"].ToString())
                    //        {
                    //            if (((Convert.ToInt32(dtSubWayBillInfo.Rows[k]["swbNeedCheck"]) == 0) && (dtSubWayBillInfo.Rows[k]["swbSortingTime"] != DBNull.Value)) || ((Convert.ToInt32(dtSubWayBillInfo.Rows[k]["swbNeedCheck"]) == 2) && (dtSubWayBillInfo.Rows[k]["swbSortingTime"] != DBNull.Value)))//(swbNeedCheck=0 or swbNeedCheck=2) and swbSortingTime is not null
                    //            {
                    //                releseNum = releseNum + 1;
                    //            }
                    //            if (Convert.ToInt32(dtSubWayBillInfo.Rows[k]["swbNeedCheck"]) == 3)//swbNeedCheck=3
                    //            {
                    //                notReleseNum = notReleseNum + 1;
                    //            }
                    //            if (dtSubWayBillInfo.Rows[k]["swbSortingTime"] == DBNull.Value)// swbSortingTime is null 
                    //            {
                    //                notProNum = notProNum + 1;
                    //            }

                    //            subNum = subNum + 1;

                    //        }

                    //    }

                    //}

                    switch (strFiledArray[j])
                    {
                        case "wbCompany"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                            break;
                        case "wbSubNumber_Custom":
                            if (subNum != -1)
                            {
                                drCustom[strFiledArray[j]] = subNum;
                            }
                            else
                            {
                                drCustom[strFiledArray[j]] = "无";
                            }
                            break;
                        case "wbReleaseCount_Custom":
                            if (releseNum != -1)
                            {
                                drCustom[strFiledArray[j]] = releseNum;
                            }
                            else
                            {
                                drCustom[strFiledArray[j]] = "无";
                            }
                            break;
                        case "wbUnReleaseCount_Custom":
                            if (notReleseNum != -1)
                            {
                                drCustom[strFiledArray[j]] = notReleseNum;
                            }
                            else
                            {
                                drCustom[strFiledArray[j]] = "无";
                            }
                            break;
                        case "wbNotProCount_Custom":
                            if (notProNum != -1)
                            {
                                drCustom[strFiledArray[j]] = notProNum;
                            }
                            else
                            {
                                drCustom[strFiledArray[j]] = "无";
                            }
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
            ReportDataSource reportDataSource = new ReportDataSource("CustomerConfirm_DS", dtCustom);

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

            string strOutputFileName = "放行记录_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        public string GetData(string order, string page, string rows, string sort, string ddStatus, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage)
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

            ddStatus = Server.UrlDecode(ddStatus.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate.ToString());
            txtGCode = Server.UrlDecode(txtGCode.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());

            string strWhereTemp = "";
            if (ddStatus != "")
            {
                if (ddStatus == "-1" || ddStatus == "1")//查看已预检
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + @" and ( wbStatus = 1
                                                          OR ( wbStatus = 2 AND wbImportType=0
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                    swbNeedCheck
                                                                             HAVING swbNeedCheck = 3
                                                                                    AND COUNT(swbNeedCheck) > 0 )
                                                             )
                                                             OR
                                                             ( wbStatus = 2 AND wbImportType=1
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                     swbReleaseFlag
                                                                             HAVING swbReleaseFlag <>3
                                                                                    AND COUNT(swbReleaseFlag) > 0 )
                                                             )
                                                        )";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + @"  ( wbStatus = 1
                                                          OR ( wbStatus = 2 AND wbImportType=0
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                    swbNeedCheck
                                                                             HAVING swbNeedCheck = 3
                                                                                    AND COUNT(swbNeedCheck) > 0 )
                                                             )
                                                             OR
                                                             ( wbStatus = 2 AND wbImportType=1
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                     swbReleaseFlag
                                                                             HAVING swbReleaseFlag <> 3
                                                                                    AND COUNT(swbReleaseFlag) > 0 )
                                                             )
                                                        )";
                    }
                }
                else if (ddStatus == "0")//查看未预检
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and (  wbStatus=0 )";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (  wbStatus=0 )";
                    }
                }

            }


            if (inputBeginDate != "" && inputEndDate != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "' ) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "' ) ";
                }
            }

            if (txtGCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (  wbSerialNum like '%" + txtGCode + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (  wbSerialNum like '%" + txtGCode + "%') ";
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (  wbCompany like '%" + txtVoyage + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (  wbCompany like '%" + txtVoyage + "%') ";
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
                        case "wbCompany"://格式化公司(保存的是用户名，取出公司名)
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString());
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString());
                            }
                            break;
                        case "wbSubNumber_Custom":
                            break;
                        case "wbReleaseCount_Custom":
                            break;
                        case "wbUnReleaseCount_Custom":
                            break;
                        case "wbNotProCount_Custom":
                            //DataSet dsWayBillInfo = null;
                            //DataTable dtWayBillInfo = null;
                            //string subNum = "-1";
                            //string releseNum = "-1";
                            //string notReleseNum = "-1";
                            //string notProNum = "-1";
                            //dsWayBillInfo = new T_SubWayBill().getWayBillSumInfoBywbID(dt.Rows[i]["wbID"].ToString());
                            //if (dsWayBillInfo != null)
                            //{
                            //    dtWayBillInfo = dsWayBillInfo.Tables[0];
                            //    if (dtWayBillInfo != null && dtWayBillInfo.Rows.Count > 0)
                            //    {
                            //        subNum = dtWayBillInfo.Rows[0]["wbSubNumber_Custom"].ToString();
                            //        releseNum = dtWayBillInfo.Rows[0]["wbReleaseCount_Custom"].ToString();
                            //        notReleseNum = dtWayBillInfo.Rows[0]["wbUnReleaseCount_Custom"].ToString();
                            //        notProNum = dtWayBillInfo.Rows[0]["wbNotProCount_Custom"].ToString();
                            //    }
                            //}

                            //if (subNum == "-1")
                            //{
                            //    subNum = "无";
                            //}

                            //if (releseNum == "-1")
                            //{
                            //    releseNum = "无";
                            //}

                            //if (notReleseNum == "-1")
                            //{
                            //    notReleseNum = "无";
                            //}

                            //if (notProNum == "-1")
                            //{
                            //    notProNum = "无";
                            //}

                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "wbSubNumber_Custom", subNum);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "wbSubNumber_Custom", subNum);
                            //}

                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "wbReleaseCount_Custom", releseNum);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "wbReleaseCount_Custom", releseNum);
                            //}

                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "wbUnReleaseCount_Custom", notReleseNum);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "wbUnReleaseCount_Custom", notReleseNum);
                            //}

                            //if (j != strFiledArray.Length - 1)
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\",", "wbNotProCount_Custom", notProNum);
                            //}
                            //else
                            //{
                            //    sb.AppendFormat("\"{0}\":\"{1}\"", "wbNotProCount_Custom", notProNum);
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
        public ActionResult Print(string order, string page, string rows, string sort, string ddStatus, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage)
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

            ddStatus = Server.UrlDecode(ddStatus.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate.ToString());
            txtGCode = Server.UrlDecode(txtGCode.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());

            string strWhereTemp = "";
            if (ddStatus != "")
            {
                if (ddStatus == "-1" || ddStatus == "1")//查看已预检
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + @" and ( wbStatus = 1
                                                          OR ( wbStatus = 2 AND wbImportType=0
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                    swbNeedCheck
                                                                             HAVING swbNeedCheck = 3
                                                                                    AND COUNT(swbNeedCheck) > 0 )
                                                             )
                                                             OR
                                                             ( wbStatus = 2 AND wbImportType=1
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                     swbReleaseFlag
                                                                             HAVING swbReleaseFlag<>3
                                                                                    AND COUNT(swbReleaseFlag) > 0 )
                                                             )
                                                        )";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + @"  ( wbStatus = 1
                                                          OR ( wbStatus = 2 AND wbImportType=0
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                    swbNeedCheck
                                                                             HAVING swbNeedCheck = 3
                                                                                    AND COUNT(swbNeedCheck) > 0 )
                                                             )
                                                             OR
                                                             ( wbStatus = 2 AND wbImportType=1
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                     swbReleaseFlag
                                                                             HAVING swbReleaseFlag <>3
                                                                                    AND COUNT(swbReleaseFlag) > 0 )
                                                             )
                                                        )";
                    }
                }
                else if (ddStatus == "0")//查看未预检
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and (  wbStatus=0 )";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (  wbStatus=0 )";
                    }
                }

            }


            if (inputBeginDate != "" && inputEndDate != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "' ) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "' ) ";
                }
            }

            if (txtGCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (  wbSerialNum like '%" + txtGCode + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (  wbSerialNum like '%" + txtGCode + "%') ";
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (  wbCompany like '%" + txtVoyage + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (  wbCompany like '%" + txtVoyage + "%') ";
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
            dtCustom.Columns.Add("wbSubNumber_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbReleaseCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbUnReleaseCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbNotProCount_Custom", Type.GetType("System.String"));
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
                        case "wbCompany"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                            break;
                        case "wbSubNumber_Custom":
                            break;
                        case "wbReleaseCount_Custom":
                            break;
                        case "wbUnReleaseCount_Custom":
                            break;
                        case "wbNotProCount_Custom":
                            DataSet dsWayBillInfo = null;
                            DataTable dtWayBillInfo = null;
                            string subNum = "-1";
                            string releseNum = "-1";
                            string notReleseNum = "-1";
                            string notProNum = "-1";
                            dsWayBillInfo = new T_SubWayBill().getWayBillSumInfoBywbID(dt.Rows[i]["wbID"].ToString());
                            if (dsWayBillInfo != null)
                            {
                                dtWayBillInfo = dsWayBillInfo.Tables[0];
                                if (dtWayBillInfo != null && dtWayBillInfo.Rows.Count > 0)
                                {
                                    subNum = dtWayBillInfo.Rows[0]["wbSubNumber_Custom"].ToString();
                                    releseNum = dtWayBillInfo.Rows[0]["wbReleaseCount_Custom"].ToString();
                                    notReleseNum = dtWayBillInfo.Rows[0]["wbUnReleaseCount_Custom"].ToString();
                                    notProNum = dtWayBillInfo.Rows[0]["wbNotProCount_Custom"].ToString();
                                }
                            }

                            if (subNum == "-1")
                            {
                                subNum = "无";
                            }

                            if (releseNum == "-1")
                            {
                                releseNum = "无";
                            }

                            if (notReleseNum == "-1")
                            {
                                notReleseNum = "无";
                            }

                            if (notProNum == "-1")
                            {
                                notProNum = "无";
                            }
                            drCustom["wbSubNumber_Custom"] = subNum;
                            drCustom["wbReleaseCount_Custom"] = releseNum;
                            drCustom["wbUnReleaseCount_Custom"] = notReleseNum;
                            drCustom["wbNotProCount_Custom"] = notProNum;
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
            ReportDataSource reportDataSource = new ReportDataSource("CustomerConfirm_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string ddStatus, string inputBeginDate, string inputEndDate, string txtGCode, string txtVoyage, string browserType)
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

            ddStatus = Server.UrlDecode(ddStatus.ToString());
            inputBeginDate = Server.UrlDecode(inputBeginDate.ToString());
            inputEndDate = Server.UrlDecode(inputEndDate.ToString());
            txtGCode = Server.UrlDecode(txtGCode.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());

            string strWhereTemp = "";
            if (ddStatus != "")
            {
                if (ddStatus == "-1" || ddStatus == "1")//查看已预检
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + @" and ( wbStatus = 1
                                                          OR ( wbStatus = 2 AND wbImportType=0
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                    swbNeedCheck
                                                                             HAVING swbNeedCheck = 3
                                                                                    AND COUNT(swbNeedCheck) > 0 )
                                                             )
                                                             OR
                                                             ( wbStatus = 2 AND wbImportType=1
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                     swbReleaseFlag
                                                                             HAVING swbReleaseFlag <>3
                                                                                    AND COUNT(swbReleaseFlag) > 0 )
                                                             )
                                                        )";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + @"  ( wbStatus = 1
                                                          OR ( wbStatus = 2 AND wbImportType=0
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                    swbNeedCheck
                                                                             HAVING swbNeedCheck = 3
                                                                                    AND COUNT(swbNeedCheck) > 0 )
                                                             )
                                                             OR
                                                             ( wbStatus = 2 AND wbImportType=1
                                                               AND wbID IN ( SELECT swb_wbID
                                                                             FROM   dbo.V_Distinct_SubWayBill
                                                                             GROUP BY swb_wbID ,
                                                                                     swbReleaseFlag
                                                                             HAVING swbReleaseFlag <>3
                                                                                    AND COUNT(swbReleaseFlag) > 0 )
                                                             )
                                                        )";
                    }
                }
                else if (ddStatus == "0")//查看未预检
                {
                    if (strWhereTemp != "")
                    {
                        strWhereTemp = strWhereTemp + " and (  wbStatus=0 )";
                    }
                    else
                    {
                        strWhereTemp = strWhereTemp + "  (  wbStatus=0 )";
                    }
                }

            }


            if (inputBeginDate != "" && inputEndDate != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "' ) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (  wbStorageDate>='" + Convert.ToDateTime(inputBeginDate).ToString("yyyyMMdd") + "' and wbStorageDate<='" + Convert.ToDateTime(inputEndDate).ToString("yyyyMMdd") + "' ) ";
                }
            }

            if (txtGCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (  wbSerialNum like '%" + txtGCode + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (  wbSerialNum like '%" + txtGCode + "%') ";
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (  wbCompany like '%" + txtVoyage + "%') ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (  wbCompany like '%" + txtVoyage + "%') ";
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
            dtCustom.Columns.Add("wbSubNumber_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbReleaseCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbUnReleaseCount_Custom", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbNotProCount_Custom", Type.GetType("System.String"));
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
                        case "wbCompany"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : dt.Rows[i][strFiledArray[j]].ToString();
                            break;
                        case "wbSubNumber_Custom":
                            break;
                        case "wbReleaseCount_Custom":
                            break;
                        case "wbUnReleaseCount_Custom":
                            break;
                        case "wbNotProCount_Custom":
                            DataSet dsWayBillInfo = null;
                            DataTable dtWayBillInfo = null;
                            string subNum = "-1";
                            string releseNum = "-1";
                            string notReleseNum = "-1";
                            string notProNum = "-1";
                            dsWayBillInfo = new T_SubWayBill().getWayBillSumInfoBywbID(dt.Rows[i]["wbID"].ToString());
                            if (dsWayBillInfo != null)
                            {
                                dtWayBillInfo = dsWayBillInfo.Tables[0];
                                if (dtWayBillInfo != null && dtWayBillInfo.Rows.Count > 0)
                                {
                                    subNum = dtWayBillInfo.Rows[0]["wbSubNumber_Custom"].ToString();
                                    releseNum = dtWayBillInfo.Rows[0]["wbReleaseCount_Custom"].ToString();
                                    notReleseNum = dtWayBillInfo.Rows[0]["wbUnReleaseCount_Custom"].ToString();
                                    notProNum = dtWayBillInfo.Rows[0]["wbNotProCount_Custom"].ToString();
                                }
                            }

                            if (subNum == "-1")
                            {
                                subNum = "无";
                            }

                            if (releseNum == "-1")
                            {
                                releseNum = "无";
                            }

                            if (notReleseNum == "-1")
                            {
                                notReleseNum = "无";
                            }

                            if (notProNum == "-1")
                            {
                                notProNum = "无";
                            }
                            drCustom["wbSubNumber_Custom"] = subNum;
                            drCustom["wbReleaseCount_Custom"] = releseNum;
                            drCustom["wbUnReleaseCount_Custom"] = notReleseNum;
                            drCustom["wbNotProCount_Custom"] = notProNum;
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
            ReportDataSource reportDataSource = new ReportDataSource("CustomerConfirm_DS", dtCustom);

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

            string strOutputFileName = "放行记录_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
        public string UpdateReleaseStatus(string ids)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"放行失败,原因未知\"}";
            ids = Server.UrlDecode(ids);
            if (ids != "")
            {
                try
                {
                    if (ids.EndsWith("*"))
                    {
                        ids = ids.Substring(0, ids.Length - 1);
                        ids = ids.Replace("*", ",");
                    }

                    if (wayBillSql.updateReleaStatus(ids))
                    {
                        strRet = "{\"result\":\"ok\",\"message\":\"已经成功将所选择的运单放行\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"放行失败,原因未知\"}";
                    }
                }
                catch (Exception ex)
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
                }

            }
            else
            {
                strRet = "{\"result\":\"error\",\"message\":\"未选择需要放行的运单号，无需放行\"}";
            }

            return strRet;
        }

        [HttpPost]
        public string getWayBillSumInfoBywbID(string wbID)
        {
            string strRet = "{\"result\":\"error\",wbSubNumber_Custom:\"0\",wbReleaseCount_Custom:\"0\",\"wbUnReleaseCount_Custom\":\"0\",\"wbNotProCount_Custom\":\"0\",\"message\":\"获取失败，原因未知\"}";
            DataSet dsWayBillInfo = null;
            DataTable dtWayBillInfo = null;
            string subNum = "-1";
            string releseNum = "-1";
            string notReleseNum = "-1";
            string notProNum = "-1";

            wbID = Server.UrlDecode(wbID);

            try
            {
                if (wbID == "")
                {
                    strRet = "{\"result\":\"error\",wbSubNumber_Custom:\"0\",wbReleaseCount_Custom:\"0\",\"wbUnReleaseCount_Custom\":\"0\",\"wbNotProCount_Custom\":\"0\",\"message\":\"" + "获取失败,未给出总运单ID" + "\"}";
                }
                else
                {
                    dsWayBillInfo = new T_SubWayBill().getWayBillSumInfoBywbID(wbID);
                    if (dsWayBillInfo != null)
                    {
                        dtWayBillInfo = dsWayBillInfo.Tables[0];
                        if (dtWayBillInfo != null && dtWayBillInfo.Rows.Count > 0)
                        {
                            subNum = dtWayBillInfo.Rows[0]["wbSubNumber_Custom"].ToString();
                            releseNum = dtWayBillInfo.Rows[0]["wbReleaseCount_Custom"].ToString();
                            notReleseNum = dtWayBillInfo.Rows[0]["wbUnReleaseCount_Custom"].ToString();
                            notProNum = dtWayBillInfo.Rows[0]["wbNotProCount_Custom"].ToString();
                        }
                    }

                    if (subNum == "-1")
                    {
                        subNum = "无";
                    }

                    if (releseNum == "-1")
                    {
                        releseNum = "无";
                    }

                    if (notReleseNum == "-1")
                    {
                        notReleseNum = "无";
                    }

                    if (notProNum == "-1")
                    {
                        notProNum = "无";
                    }

                    strRet = "{\"result\":\"ok\",wbSubNumber_Custom:\"" + subNum + "\",wbReleaseCount_Custom:\"" + releseNum + "\",\"wbUnReleaseCount_Custom\":\"" + notReleseNum + "\",\"wbNotProCount_Custom\":\"" + notProNum + "\",\"message\":\"获取成功\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",wbSubNumber_Custom:\"0\",wbReleaseCount_Custom:\"0\",\"wbUnReleaseCount_Custom\":\"0\",\"wbNotProCount_Custom\":\"0\",\"message\":\"获取失败，原因:" + ex.Message + "\"}";
            }
            return strRet;
        }
    }

    public class Struct_SumInfo
    {
        public int releseNum
        {
            get;
            set;
        }
        public int notReleseNum
        {
            get;
            set;
        }
        public int notProNum
        {
            get;
            set;
        }
        public int subNum
        {
            get;
            set;
        }

    }
}
