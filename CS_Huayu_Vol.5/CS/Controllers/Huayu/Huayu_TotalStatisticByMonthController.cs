using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SQLDAL;
using CS.Filter;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Text;

namespace CS.Controllers.Huayu
{
    public class Huayu_TotalStatisticByMonthController : Controller
    {
        SQLDAL.T_WayBill tWayBill = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();
        public const string strFileds = "wbTotalNum,custom_wbStorageDate,wbTotalUnReleased,wbTotalWeight,wbTotalWeight_Category2,wbTotalWeight_Category3,wbTotalWeight_Category4,wbTotalWeight_Category5,wbTotalWeight_Category6,wbTotalWeightWithOutUnit,wbCompany";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/Huayu_TotalStatisticByMonth.rdlc";

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
        public string GetData(string order, string page, string rows, string sort, string ddCompany, string txtStartDate, string txtEndDate)
        {
            string strTotalSQL = "";
            string strDetailSQL = "";

            string wbTotalWeight_Category2_SQL = "";
            string wbTotalWeight_Category3_SQL = "";
            string wbTotalWeight_Category4_SQL = "";
            string wbTotalWeight_Category5_SQL = "";
            string wbTotalWeight_Category6_SQL = "";
            string wbTotalWeight_Temp_SQL = "";
            DataTable dt_wbTotalWeight_Category = null;
            string str_wbTotalWeight_Category = "";

            Int32 i_wbTotalNum = 0;
            Int32 i_wbTotalUnReleased = 0;
            double i_wbTotalWeight = 0;
            double d_wbTotalWeight_Category2 = 0;
            double d_wbTotalWeight_Category3 = 0;
            double d_wbTotalWeight_Category4 = 0;
            double d_wbTotalWeight_Category5 = 0;
            double d_wbTotalWeight_Category6 = 0;

            string strTemp = "";
            int iMaxCount = 0;
            sort = " " + sort + " " + order;

//            strDetailSQL = @"select T.wbCompany,count(T.swbID) as iCountNum,sum(T.swbWeight) as swbWeight,
//                            sum(T.swbActualWeight) as swbActualWeight,T.custom_wbStorageDate from
//                            (
//	                            select sum(swbWeight) as swbWeight,sum(swbActualWeight) as swbActualWeight,swbID,wbCompany,SUBSTRING(wbStorageDate,1,6) AS custom_wbStorageDate
//	                            from V_WayBill_SubWayBill 
//	                            where wbStatus=2  
//	                            {0}
//	                            and swbNeedCheck=3
//	                            group by SUBSTRING(wbStorageDate,1,6),wbCompany,swbID
//                            ) T group by T.wbCompany,T.custom_wbStorageDate";

//            strTotalSQL = @"select T1.wbCompany,count(T1.swbID) as wbTotalNum,sum(T1.wbTotalWeight) as  wbTotalWeight,T1.custom_wbStorageDate from
//                            (
//	                            select T.wbCompany,T.swbID,sum(T.swbWeight) as wbTotalWeight,SUBSTRING(T.wbStorageDate,1,6) AS custom_wbStorageDate from
//	                            (
//		                            select wbCompany,swbID,swbWeight,wbStorageDate from V_WayBill_SubWayBill where 
//			                            wbStatus=2  {0} 
//	                            ) T group by T.wbCompany,SUBSTRING(T.wbStorageDate,1,6),T.swbID
//                            ) T1 group by T1.wbCompany,T1.custom_wbStorageDate order by " + sort;

            strDetailSQL = @"select T.wbCompany,count(T.swbID) as iCountNum,sum(T.swbWeight) as swbWeight,
                            sum(T.swbActualWeight) as swbActualWeight,T.custom_wbStorageDate from
                            (
	                            select sum(swbWeight) as swbWeight,sum(swbActualWeight) as swbActualWeight,swbID,wbCompany,SUBSTRING(wbStorageDate,1,6) AS custom_wbStorageDate
	                            from V_WayBill_SubWayBill 
	                            where wbStatus in (0,1,2)  
	                            {0}
                                and swbNeedCheck=3
	                            group by SUBSTRING(wbStorageDate,1,6),wbCompany,swbID
                            ) T group by T.wbCompany,T.custom_wbStorageDate";

            strTotalSQL = @"select T1.wbCompany,count(T1.swbID) as wbTotalNum,sum(T1.wbTotalWeight) as  wbTotalWeight,T1.custom_wbStorageDate from
                            (
	                            select T.wbCompany,T.swbID,sum(T.swbWeight) as wbTotalWeight,SUBSTRING(T.wbStorageDate,1,6) AS custom_wbStorageDate from
	                            (
		                            select wbCompany,swbID,swbWeight,wbStorageDate from V_WayBill_SubWayBill where 
			                            wbStatus in (0,1,2)  {0} 
	                            ) T group by T.wbCompany,SUBSTRING(T.wbStorageDate,1,6),T.swbID
                            ) T1 group by T1.wbCompany,T1.custom_wbStorageDate order by " + sort;

            ddCompany = Server.UrlDecode(ddCompany.ToString());
            txtStartDate = Server.UrlDecode(txtStartDate.ToString());
            txtEndDate = Server.UrlDecode(txtEndDate.ToString());

            if (txtStartDate != "" && txtEndDate != "")
            {
                txtStartDate = Convert.ToDateTime(txtStartDate).ToString("yyyyMM01");
                txtEndDate = Convert.ToDateTime(txtEndDate).AddMonths(1).AddDays(-1).ToString("yyyyMMdd");
            }

            if (ddCompany != "" && ddCompany != "---请选择---")
            {
                if (txtStartDate != "" && txtEndDate != "")
                {
                    strTemp = " and (wbCompany like '" + ddCompany + "')  and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                    wbTotalWeight_Temp_SQL = " and  (wbCompany like '" + ddCompany + "') ";//and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                }
                else
                {
                    strTemp = " and (wbCompany like '" + ddCompany + "')";
                    wbTotalWeight_Temp_SQL = " and  (wbCompany like '" + ddCompany + "') ";
                }
            }
            else
            {
                if (txtStartDate != "" && txtEndDate != "")
                {
                    strTemp = " and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                    wbTotalWeight_Temp_SQL = "";// " and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                }
                else
                {

                }
            }

            strDetailSQL = string.Format(strDetailSQL, strTemp);
            strTotalSQL = string.Format(strTotalSQL, strTemp);

            DataTable dt_Detail = SqlServerHelper.Query(strDetailSQL).Tables[0];
            DataTable dt_Total = SqlServerHelper.Query(strTotalSQL).Tables[0];

            StringBuilder sb = new StringBuilder("");
            sb.Append("{");
            sb.AppendFormat("\"total\":{0}", dt_Total.Rows.Count);
            sb.Append(",\"rows\":[");

            if (Convert.ToInt32(page) > dt_Total.Rows.Count / Convert.ToInt32(rows) && Convert.ToInt32(page) <= dt_Total.Rows.Count / Convert.ToInt32(rows) + 1)
            {
                iMaxCount = dt_Total.Rows.Count;
            }
            else
            {
                iMaxCount = Convert.ToInt32(page) * Convert.ToInt32(rows);
            }

            for (int i = (Convert.ToInt32(page) - 1) * Convert.ToInt32(rows); i < iMaxCount; i++)
            {
                sb.Append("{");

                wbTotalWeight_Category2_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='2' and wbStatus in (0,1,2)";
                wbTotalWeight_Category3_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='3' and wbStatus in (0,1,2)";
                wbTotalWeight_Category4_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='4' and wbStatus in (0,1,2)";
                wbTotalWeight_Category5_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='5' and wbStatus in (0,1,2)";
                wbTotalWeight_Category6_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='6' and wbStatus in (0,1,2)";

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wbTotalWeight":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : Convert.ToDouble(dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")).ToString("0.00"));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : Convert.ToDouble(dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")).ToString("0.00"));
                            }
                            i_wbTotalWeight = i_wbTotalWeight + Convert.ToDouble(dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "0" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            break;
                        case "wbTotalWeightWithOutUnit":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt_Total.Rows[i]["wbTotalWeight"] == DBNull.Value ? "" : Convert.ToDouble(dt_Total.Rows[i]["wbTotalWeight"].ToString().Replace("\r\n", "")).ToString("0.00"));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt_Total.Rows[i]["wbTotalWeight"] == DBNull.Value ? "" : Convert.ToDouble(dt_Total.Rows[i]["wbTotalWeight"].ToString().Replace("\r\n", "")).ToString("0.00"));
                            }
                            break;
                        case "wbTotalUnReleased":
                            int iUnReleased = 0;
                            for (int k = 0; k < dt_Detail.Rows.Count; k++)
                            {
                                if (dt_Detail.Rows[k]["wbCompany"].ToString() == dt_Total.Rows[i]["wbCompany"].ToString())
                                {
                                    iUnReleased = iUnReleased + 1;
                                }
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], iUnReleased);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], iUnReleased);
                            }
                            i_wbTotalUnReleased = i_wbTotalUnReleased + Convert.ToInt32(iUnReleased);
                            break;
                        case "wbTotalWeight_Category2":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL2 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL2 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL2 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }

                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category2_SQL + wbTotalWeight_Temp_SQL2).Tables[0];

                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            d_wbTotalWeight_Category2 = d_wbTotalWeight_Category2 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category3":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL3 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL3 = wbTotalWeight_Temp_SQL + "  and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL3 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category3_SQL + wbTotalWeight_Temp_SQL3).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            d_wbTotalWeight_Category3 = d_wbTotalWeight_Category3 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category4":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL4 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL4 = wbTotalWeight_Temp_SQL + "  and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL4 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category4_SQL + wbTotalWeight_Temp_SQL4).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            d_wbTotalWeight_Category4 = d_wbTotalWeight_Category4 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category5":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL5 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL5 = wbTotalWeight_Temp_SQL + "  and  (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL5 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category5_SQL + wbTotalWeight_Temp_SQL5).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            d_wbTotalWeight_Category5 = d_wbTotalWeight_Category5 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category6":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL6 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL6 = wbTotalWeight_Temp_SQL + "  and  (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL6 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category6_SQL + wbTotalWeight_Temp_SQL6).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            d_wbTotalWeight_Category6 = d_wbTotalWeight_Category6 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalNum":
                            i_wbTotalNum = i_wbTotalNum + Convert.ToInt32(dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "0" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            break;
                        default:
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            break;
                    }

                }

                if (i == dt_Total.Rows.Count - 1)
                {
                    sb.Append("}");
                }
                else
                {
                    sb.Append("},");
                }
            }
            dt_Total = null;
            if (sb.ToString().EndsWith(","))
            {
                sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
            }
            sb.Append("]");
            sb.Append(",\"footer\":[{\"custom_wbStorageDate\":\"<font style='color:red;font-weight:bold'>统计</font>\",\"wbTotalNum\":\"" + i_wbTotalNum.ToString() + "\",\"wbTotalUnReleased\":\"" + i_wbTotalUnReleased.ToString() + "\",\"wbTotalWeight\":\"" + i_wbTotalWeight.ToString("0.00") + "\",\"wbTotalWeight_Category2\":\"" + d_wbTotalWeight_Category2.ToString("0.00") + "\",\"wbTotalWeight_Category3\":\"" + d_wbTotalWeight_Category3.ToString("0.00") + "\",\"wbTotalWeight_Category4\":\"" + d_wbTotalWeight_Category4.ToString("0.00") + "\",\"wbTotalWeight_Category5\":\"" + d_wbTotalWeight_Category5.ToString("0.00") + "\",\"wbTotalWeight_Category6\":\"" + d_wbTotalWeight_Category6.ToString("0.00") + "\",\"iconCls\":\"icon-sum\"}]");
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 分页查询类
        /// </summary>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public ActionResult Print(string order, string page, string rows, string sort, string ddCompany, string txtStartDate, string txtEndDate)
        {
            string strTotalSQL = "";
            string strDetailSQL = "";

            string wbTotalWeight_Category2_SQL = "";
            string wbTotalWeight_Category3_SQL = "";
            string wbTotalWeight_Category4_SQL = "";
            string wbTotalWeight_Category5_SQL = "";
            string wbTotalWeight_Category6_SQL = "";
            string wbTotalWeight_Temp_SQL = "";
            DataTable dt_wbTotalWeight_Category = null;
            string str_wbTotalWeight_Category = "";

            Int32 i_wbTotalNum = 0;
            Int32 i_wbTotalUnReleased = 0;
            double i_wbTotalWeight = 0;
            double d_wbTotalWeight_Category2 = 0;
            double d_wbTotalWeight_Category3 = 0;
            double d_wbTotalWeight_Category4 = 0;
            double d_wbTotalWeight_Category5 = 0;
            double d_wbTotalWeight_Category6 = 0;

            string strTemp = "";
            int iMaxCount = 0;
            sort = " " + sort + " " + order;

            strDetailSQL = @"select T.wbCompany,count(T.swbID) as iCountNum,sum(T.swbWeight) as swbWeight,
                            sum(T.swbActualWeight) as swbActualWeight,T.custom_wbStorageDate from
                            (
	                            select sum(swbWeight) as swbWeight,sum(swbActualWeight) as swbActualWeight,swbID,wbCompany,SUBSTRING(wbStorageDate,1,6) AS custom_wbStorageDate
	                            from V_WayBill_SubWayBill 
	                            where wbStatus=2  
	                            {0}
	                            and swbNeedCheck=3
	                            group by SUBSTRING(wbStorageDate,1,6),wbCompany,swbID
                            ) T group by T.wbCompany,T.custom_wbStorageDate";

            strTotalSQL = @"select T1.wbCompany,count(T1.swbID) as wbTotalNum,sum(T1.wbTotalWeight) as  wbTotalWeight,T1.custom_wbStorageDate from
                            (
	                            select T.wbCompany,T.swbID,sum(T.swbWeight) as wbTotalWeight,SUBSTRING(T.wbStorageDate,1,6) AS custom_wbStorageDate from
	                            (
		                            select wbCompany,swbID,swbWeight,wbStorageDate from V_WayBill_SubWayBill where 
			                            wbStatus=2  {0} 
	                            ) T group by T.wbCompany,SUBSTRING(T.wbStorageDate,1,6),T.swbID
                            ) T1 group by T1.wbCompany,T1.custom_wbStorageDate order by " + sort;

            ddCompany = Server.UrlDecode(ddCompany.ToString());
            txtStartDate = Server.UrlDecode(txtStartDate.ToString());
            txtEndDate = Server.UrlDecode(txtEndDate.ToString());

            if (txtStartDate != "" && txtEndDate != "")
            {
                txtStartDate = Convert.ToDateTime(txtStartDate).ToString("yyyyMM01");
                txtEndDate = Convert.ToDateTime(txtEndDate).AddMonths(1).AddDays(-1).ToString("yyyyMMdd");
            }

            if (ddCompany != "" && ddCompany != "---请选择---")
            {
                if (txtStartDate != "" && txtEndDate != "")
                {
                    strTemp = " and (wbCompany like '" + ddCompany + "')  and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                    wbTotalWeight_Temp_SQL = " and  (wbCompany like '" + ddCompany + "') ";//and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                }
                else
                {
                    strTemp = " and (wbCompany like '" + ddCompany + "')";
                    wbTotalWeight_Temp_SQL = " and  (wbCompany like '" + ddCompany + "') ";
                }
            }
            else
            {
                if (txtStartDate != "" && txtEndDate != "")
                {
                    strTemp = " and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                    wbTotalWeight_Temp_SQL = "";// " and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                }
                else
                {

                }
            }

            strDetailSQL = string.Format(strDetailSQL, strTemp);
            strTotalSQL = string.Format(strTotalSQL, strTemp);

            DataTable dt_Detail = SqlServerHelper.Query(strDetailSQL).Tables[0];
            DataTable dt_Total = SqlServerHelper.Query(strTotalSQL).Tables[0];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbTotalNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("custom_wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalUnReleased", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category2", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category3", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category4", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category5", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category6", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeightWithOutUnit", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));

            DataRow drCustom = null;

            if (Convert.ToInt32(page) > dt_Total.Rows.Count / Convert.ToInt32(rows) && Convert.ToInt32(page) <= dt_Total.Rows.Count / Convert.ToInt32(rows) + 1)
            {
                iMaxCount = dt_Total.Rows.Count;
            }
            else
            {
                iMaxCount = Convert.ToInt32(page) * Convert.ToInt32(rows);
            }

            for (int i = (Convert.ToInt32(page) - 1) * Convert.ToInt32(rows); i < iMaxCount; i++)
            {
                drCustom = dtCustom.NewRow();

                wbTotalWeight_Category2_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='2' and wbStatus=2";
                wbTotalWeight_Category3_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='3' and wbStatus=2";
                wbTotalWeight_Category4_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='4' and wbStatus=2";
                wbTotalWeight_Category5_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='5' and wbStatus=2";
                wbTotalWeight_Category6_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='6' and wbStatus=2";

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wbTotalWeight":
                            drCustom[strFiledArray[j]] = dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : Convert.ToDouble(dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")).ToString("0.00");
                            i_wbTotalWeight = i_wbTotalWeight + Convert.ToDouble(dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "0" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            break;
                        case "wbTotalWeightWithOutUnit":
                            drCustom[strFiledArray[j]] = dt_Total.Rows[i]["wbTotalWeight"] == DBNull.Value ? "" : Convert.ToDouble(dt_Total.Rows[i]["wbTotalWeight"].ToString().Replace("\r\n", "")).ToString("0.00");
                            break;
                        case "wbTotalUnReleased":
                            int iUnReleased = 0;
                            for (int k = 0; k < dt_Detail.Rows.Count; k++)
                            {
                                if (dt_Detail.Rows[k]["wbCompany"].ToString() == dt_Total.Rows[i]["wbCompany"].ToString())
                                {
                                    iUnReleased = iUnReleased + 1;
                                }
                            }
                            drCustom[strFiledArray[j]] = iUnReleased;
                            i_wbTotalUnReleased = i_wbTotalUnReleased + Convert.ToInt32(iUnReleased);
                            break;
                        case "wbTotalWeight_Category2":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL2 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL2 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL2 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }

                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category2_SQL + wbTotalWeight_Temp_SQL2).Tables[0];

                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category2 = d_wbTotalWeight_Category2 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category3":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL3 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL3 = wbTotalWeight_Temp_SQL + "  and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL3 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category3_SQL + wbTotalWeight_Temp_SQL3).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category3 = d_wbTotalWeight_Category3 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category4":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL4 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL4 = wbTotalWeight_Temp_SQL + "  and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL4 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category4_SQL + wbTotalWeight_Temp_SQL4).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category4 = d_wbTotalWeight_Category4 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category5":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL5 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL5 = wbTotalWeight_Temp_SQL + "  and  (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL5 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category5_SQL + wbTotalWeight_Temp_SQL5).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category5 = d_wbTotalWeight_Category5 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category6":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL6 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL6 = wbTotalWeight_Temp_SQL + "  and  (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL6 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category6_SQL + wbTotalWeight_Temp_SQL6).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category6 = d_wbTotalWeight_Category6 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalNum":
                            i_wbTotalNum = i_wbTotalNum + Convert.ToInt32(dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "0" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            drCustom[strFiledArray[j]] = dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }

                }
                if (drCustom["custom_wbStorageDate"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt_Total = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);

            ReportParameter var_i_wbTotalNum = new ReportParameter("i_wbTotalNum", i_wbTotalNum.ToString());
            ReportParameter var_i_wbTotalUnReleased = new ReportParameter("i_wbTotalUnReleased", i_wbTotalUnReleased.ToString());
            ReportParameter var_i_wbTotalWeight = new ReportParameter("i_wbTotalWeight", i_wbTotalWeight.ToString());
            ReportParameter var_d_wbTotalWeight_Category2 = new ReportParameter("d_wbTotalWeight_Category2", d_wbTotalWeight_Category2.ToString());
            ReportParameter var_d_wbTotalWeight_Category3 = new ReportParameter("d_wbTotalWeight_Category3", d_wbTotalWeight_Category3.ToString());
            ReportParameter var_d_wbTotalWeight_Category4 = new ReportParameter("d_wbTotalWeight_Category4", d_wbTotalWeight_Category4.ToString());
            ReportParameter var_d_wbTotalWeight_Category5 = new ReportParameter("d_wbTotalWeight_Category5", d_wbTotalWeight_Category5.ToString());
            ReportParameter var_d_wbTotalWeight_Category6 = new ReportParameter("d_wbTotalWeight_Category6", d_wbTotalWeight_Category6.ToString());

            localReport.SetParameters(new ReportParameter[] { var_i_wbTotalNum });
            localReport.SetParameters(new ReportParameter[] { var_i_wbTotalUnReleased });
            localReport.SetParameters(new ReportParameter[] { var_i_wbTotalWeight });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category2 });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category3 });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category4 });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category5 });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category6 });

            ReportDataSource reportDataSource = new ReportDataSource("Huayu_TotalStatisticByMonth_DS", dtCustom);

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

        /// <summary>
        /// 分页查询类
        /// </summary>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public ActionResult Excel(string order, string page, string rows, string sort, string ddCompany, string txtStartDate, string txtEndDate, string browserType)
        {
            string strTotalSQL = "";
            string strDetailSQL = "";

            string wbTotalWeight_Category2_SQL = "";
            string wbTotalWeight_Category3_SQL = "";
            string wbTotalWeight_Category4_SQL = "";
            string wbTotalWeight_Category5_SQL = "";
            string wbTotalWeight_Category6_SQL = "";
            string wbTotalWeight_Temp_SQL = "";
            DataTable dt_wbTotalWeight_Category = null;
            string str_wbTotalWeight_Category = "";

            Int32 i_wbTotalNum = 0;
            Int32 i_wbTotalUnReleased = 0;
            double i_wbTotalWeight = 0;
            double d_wbTotalWeight_Category2 = 0;
            double d_wbTotalWeight_Category3 = 0;
            double d_wbTotalWeight_Category4 = 0;
            double d_wbTotalWeight_Category5 = 0;
            double d_wbTotalWeight_Category6 = 0;

            string strTemp = "";
            int iMaxCount = 0;
            sort = " " + sort + " " + order;

            strDetailSQL = @"select T.wbCompany,count(T.swbID) as iCountNum,sum(T.swbWeight) as swbWeight,
                            sum(T.swbActualWeight) as swbActualWeight,T.custom_wbStorageDate from
                            (
	                            select sum(swbWeight) as swbWeight,sum(swbActualWeight) as swbActualWeight,swbID,wbCompany,SUBSTRING(wbStorageDate,1,6) AS custom_wbStorageDate
	                            from V_WayBill_SubWayBill 
	                            where wbStatus=2  
	                            {0}
	                            and swbNeedCheck=3
	                            group by SUBSTRING(wbStorageDate,1,6),wbCompany,swbID
                            ) T group by T.wbCompany,T.custom_wbStorageDate";

            strTotalSQL = @"select T1.wbCompany,count(T1.swbID) as wbTotalNum,sum(T1.wbTotalWeight) as  wbTotalWeight,T1.custom_wbStorageDate from
                            (
	                            select T.wbCompany,T.swbID,sum(T.swbWeight) as wbTotalWeight,SUBSTRING(T.wbStorageDate,1,6) AS custom_wbStorageDate from
	                            (
		                            select wbCompany,swbID,swbWeight,wbStorageDate from V_WayBill_SubWayBill where 
			                            wbStatus=2  {0} 
	                            ) T group by T.wbCompany,SUBSTRING(T.wbStorageDate,1,6),T.swbID
                            ) T1 group by T1.wbCompany,T1.custom_wbStorageDate order by " + sort;

            ddCompany = Server.UrlDecode(ddCompany.ToString());
            txtStartDate = Server.UrlDecode(txtStartDate.ToString());
            txtEndDate = Server.UrlDecode(txtEndDate.ToString());

            if (txtStartDate != "" && txtEndDate != "")
            {
                txtStartDate = Convert.ToDateTime(txtStartDate).ToString("yyyyMM01");
                txtEndDate = Convert.ToDateTime(txtEndDate).AddMonths(1).AddDays(-1).ToString("yyyyMMdd");
            }

            if (ddCompany != "" && ddCompany != "---请选择---")
            {
                if (txtStartDate != "" && txtEndDate != "")
                {
                    strTemp = " and (wbCompany like '" + ddCompany + "')  and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                    wbTotalWeight_Temp_SQL = " and  (wbCompany like '" + ddCompany + "') ";//and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                }
                else
                {
                    strTemp = " and (wbCompany like '" + ddCompany + "')";
                    wbTotalWeight_Temp_SQL = " and  (wbCompany like '" + ddCompany + "') ";
                }
            }
            else
            {
                if (txtStartDate != "" && txtEndDate != "")
                {
                    strTemp = " and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                    wbTotalWeight_Temp_SQL = "";// " and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                }
                else
                {

                }
            }

            strDetailSQL = string.Format(strDetailSQL, strTemp);
            strTotalSQL = string.Format(strTotalSQL, strTemp);

            DataTable dt_Detail = SqlServerHelper.Query(strDetailSQL).Tables[0];
            DataTable dt_Total = SqlServerHelper.Query(strTotalSQL).Tables[0];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbTotalNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("custom_wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalUnReleased", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category2", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category3", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category4", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category5", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeight_Category6", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbTotalWeightWithOutUnit", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));

            DataRow drCustom = null;

            if (Convert.ToInt32(page) > dt_Total.Rows.Count / Convert.ToInt32(rows) && Convert.ToInt32(page) <= dt_Total.Rows.Count / Convert.ToInt32(rows) + 1)
            {
                iMaxCount = dt_Total.Rows.Count;
            }
            else
            {
                iMaxCount = Convert.ToInt32(page) * Convert.ToInt32(rows);
            }

            for (int i = (Convert.ToInt32(page) - 1) * Convert.ToInt32(rows); i < iMaxCount; i++)
            {
                drCustom = dtCustom.NewRow();

                wbTotalWeight_Category2_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='2' and wbStatus=2";
                wbTotalWeight_Category3_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='3' and wbStatus=2";
                wbTotalWeight_Category4_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='4' and wbStatus=2";
                wbTotalWeight_Category5_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='5' and wbStatus=2";
                wbTotalWeight_Category6_SQL = @"select * from V_WayBill_SubWayBill where swbCustomsCategory='6' and wbStatus=2";

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wbTotalWeight":
                            drCustom[strFiledArray[j]] = dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : Convert.ToDouble(dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")).ToString("0.00");
                            i_wbTotalWeight = i_wbTotalWeight + Convert.ToDouble(dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "0" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            break;
                        case "wbTotalWeightWithOutUnit":
                            drCustom[strFiledArray[j]] = dt_Total.Rows[i]["wbTotalWeight"] == DBNull.Value ? "" : Convert.ToDouble(dt_Total.Rows[i]["wbTotalWeight"].ToString().Replace("\r\n", "")).ToString("0.00");
                            break;
                        case "wbTotalUnReleased":
                            int iUnReleased = 0;
                            for (int k = 0; k < dt_Detail.Rows.Count; k++)
                            {
                                if (dt_Detail.Rows[k]["wbCompany"].ToString() == dt_Total.Rows[i]["wbCompany"].ToString())
                                {
                                    iUnReleased = iUnReleased + 1;
                                }
                            }
                            drCustom[strFiledArray[j]] = iUnReleased;
                            i_wbTotalUnReleased = i_wbTotalUnReleased + Convert.ToInt32(iUnReleased);
                            break;
                        case "wbTotalWeight_Category2":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL2 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL2 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL2 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }

                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category2_SQL + wbTotalWeight_Temp_SQL2).Tables[0];

                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category2 = d_wbTotalWeight_Category2 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category3":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL3 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL3 = wbTotalWeight_Temp_SQL + "  and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL3 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category3_SQL + wbTotalWeight_Temp_SQL3).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category3 = d_wbTotalWeight_Category3 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category4":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL4 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL4 = wbTotalWeight_Temp_SQL + "  and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL4 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category4_SQL + wbTotalWeight_Temp_SQL4).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category4 = d_wbTotalWeight_Category4 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category5":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL5 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL5 = wbTotalWeight_Temp_SQL + "  and  (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL5 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category5_SQL + wbTotalWeight_Temp_SQL5).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category5 = d_wbTotalWeight_Category5 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalWeight_Category6":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL6 = "";
                            if (string.IsNullOrEmpty(wbTotalWeight_Temp_SQL))
                            {
                                wbTotalWeight_Temp_SQL6 = wbTotalWeight_Temp_SQL + "  and  (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            else
                            {
                                wbTotalWeight_Temp_SQL6 = wbTotalWeight_Temp_SQL + " and (SUBSTRING(wbStorageDate,1,6)='" + dt_Total.Rows[i]["custom_wbStorageDate"].ToString() + "') " + " and wbCompany like '" + dt_Total.Rows[i]["wbCompany"].ToString() + "'";
                            }
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category6_SQL + wbTotalWeight_Temp_SQL6).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString("0.00");
                            }
                            drCustom[strFiledArray[j]] = str_wbTotalWeight_Category;
                            d_wbTotalWeight_Category6 = d_wbTotalWeight_Category6 + Convert.ToDouble(str_wbTotalWeight_Category);
                            break;
                        case "wbTotalNum":
                            i_wbTotalNum = i_wbTotalNum + Convert.ToInt32(dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "0" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            drCustom[strFiledArray[j]] = dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }

                }
                if (drCustom["custom_wbStorageDate"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt_Total = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);

            ReportParameter var_i_wbTotalNum = new ReportParameter("i_wbTotalNum", i_wbTotalNum.ToString());
            ReportParameter var_i_wbTotalUnReleased = new ReportParameter("i_wbTotalUnReleased", i_wbTotalUnReleased.ToString());
            ReportParameter var_i_wbTotalWeight = new ReportParameter("i_wbTotalWeight", i_wbTotalWeight.ToString());
            ReportParameter var_d_wbTotalWeight_Category2 = new ReportParameter("d_wbTotalWeight_Category2", d_wbTotalWeight_Category2.ToString());
            ReportParameter var_d_wbTotalWeight_Category3 = new ReportParameter("d_wbTotalWeight_Category3", d_wbTotalWeight_Category3.ToString());
            ReportParameter var_d_wbTotalWeight_Category4 = new ReportParameter("d_wbTotalWeight_Category4", d_wbTotalWeight_Category4.ToString());
            ReportParameter var_d_wbTotalWeight_Category5 = new ReportParameter("d_wbTotalWeight_Category5", d_wbTotalWeight_Category5.ToString());
            ReportParameter var_d_wbTotalWeight_Category6 = new ReportParameter("d_wbTotalWeight_Category6", d_wbTotalWeight_Category6.ToString());

            localReport.SetParameters(new ReportParameter[] { var_i_wbTotalNum });
            localReport.SetParameters(new ReportParameter[] { var_i_wbTotalUnReleased });
            localReport.SetParameters(new ReportParameter[] { var_i_wbTotalWeight });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category2 });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category3 });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category4 });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category5 });
            localReport.SetParameters(new ReportParameter[] { var_d_wbTotalWeight_Category6 });

            ReportDataSource reportDataSource = new ReportDataSource("Huayu_TotalStatisticByMonth_DS", dtCustom);

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

            string strOutputFileName = "货物重量统计信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
