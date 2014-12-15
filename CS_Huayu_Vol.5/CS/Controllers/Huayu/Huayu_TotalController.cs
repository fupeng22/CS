using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DBUtility;
using SQLDAL;
using CS.Filter;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_TotalController : Controller
    {
        SQLDAL.T_WayBill tWayBill = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();
        public const string strFileds = "wbTotalNum,wbTotalUnReleased,wbTotalWeight,wbTotalWeight_Category2,wbTotalWeight_Category3,wbTotalWeight_Category4,wbTotalWeight_Category5,wbTotalWeight_Category6,wbTotalWeightWithOutUnit,wbCompany";

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

            string strTemp = "";
            int iMaxCount = 0;
            sort = " " + sort + " " + order;

            strDetailSQL = @"select SWB.swbWeight,SWB.swbActualWeight,SWB.swbNeedCheck,WB.wbCompany
                                from V_Distinct_SubWayBill SWB inner join V_Distinct_WayBill WB on SWB.swb_wbID = WB.wbID 
                                where WB.wbStatus=2 {0} and SWB.swbNeedCheck=3";

            strTotalSQL = @"select * from 
                            (
                            select WB.wbCompany,count(SWB.swb_wbID) wbTotalNum,SUM(case SWB.swbWeight when null then 0 else  cast(round(SWB.swbWeight,2) as  numeric(30,2)) end) as  wbTotalWeight from V_Distinct_SubWayBill SWB 
                            inner join V_Distinct_WayBill  WB on swb.swb_wbID=WB.wbID where WB.wbStatus=2 {0} group by 
                            WB.wbCompany 
                            ) T order by  " + sort;

            //wbTotalWeight_Category2_SQL = @"select SUM(case swbWeight when null then 0 else  cast(round(swbWeight,2) as  numeric(30,2)) end) as  wbTotalWeight from V_WayBill_SubWayBill where swbCustomsCategory='2' and wbStatus=2";
            //wbTotalWeight_Category3_SQL = @"select SUM(case swbWeight when null then 0 else  cast(round(swbWeight,2) as  numeric(30,2)) end) as  wbTotalWeight from V_WayBill_SubWayBill where swbCustomsCategory='3' and wbStatus=2";
            //wbTotalWeight_Category4_SQL = @"select SUM(case swbWeight when null then 0 else  cast(round(swbWeight,2) as  numeric(30,2)) end) as  wbTotalWeight from V_WayBill_SubWayBill where swbCustomsCategory='4' and wbStatus=2";
            //wbTotalWeight_Category5_SQL = @"select SUM(case swbWeight when null then 0 else  cast(round(swbWeight,2) as  numeric(30,2)) end) as  wbTotalWeight from V_WayBill_SubWayBill where swbCustomsCategory='5' and wbStatus=2";
            //wbTotalWeight_Category6_SQL = @"select SUM(case swbWeight when null then 0 else  cast(round(swbWeight,2) as  numeric(30,2)) end) as  wbTotalWeight from V_WayBill_SubWayBill where swbCustomsCategory='6' and wbStatus=2";
            
            ddCompany = Server.UrlDecode(ddCompany.ToString());
            txtStartDate = Server.UrlDecode(txtStartDate.ToString());
            txtEndDate = Server.UrlDecode(txtEndDate.ToString());

            if (txtStartDate != "" && txtEndDate != "")
            {
                txtStartDate = Convert.ToDateTime(txtStartDate).ToString("yyyyMMdd");
                txtEndDate = Convert.ToDateTime(txtEndDate).ToString("yyyyMMdd");
            }


            if (ddCompany != "" && ddCompany != "---请选择---")
            {
                if (txtStartDate != "" && txtEndDate != "")
                {
                    strTemp = " and (WB.wbCompany like '%" + ddCompany + "%')  and (WB.wbStorageDate>='" + txtStartDate + "')" + " and (WB.wbStorageDate<='" + txtEndDate + "')";
                    wbTotalWeight_Temp_SQL = " and  (wbCompany like '%" + ddCompany + "%') and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
                }
                else
                {
                    strTemp = " and (WB.wbCompany like '%" + ddCompany + "%')";
                    wbTotalWeight_Temp_SQL = " and  (wbCompany like '%" + ddCompany + "%') ";
                }
            }
            else
            {
                if (txtStartDate != "" && txtEndDate != "")
                {
                    strTemp = " and (WB.wbStorageDate>='" + txtStartDate + "')" + " and (WB.wbStorageDate<='" + txtEndDate + "')";
                    wbTotalWeight_Temp_SQL = " and (wbStorageDate>='" + txtStartDate + "')" + " and (wbStorageDate<='" + txtEndDate + "')";
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
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], dt_Total.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt_Total.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            break;
                        case "wbTotalWeightWithOutUnit":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt_Total.Rows[i]["wbTotalWeight"] == DBNull.Value ? "" : (dt_Total.Rows[i]["wbTotalWeight"].ToString().Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt_Total.Rows[i]["wbTotalWeight"] == DBNull.Value ? "" : (dt_Total.Rows[i]["wbTotalWeight"].ToString().Replace("\r\n", "")));
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
                            break;
                        case "wbTotalWeight_Category2":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL2 = wbTotalWeight_Temp_SQL + " and wbCompany like '%" + dt_Total.Rows[i]["wbCompany"].ToString() + "%'";
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category2_SQL + wbTotalWeight_Temp_SQL2).Tables[0];
                            //if (dt_wbTotalWeight_Category.Rows.Count > 0)
                            //{
                            //    wbTotalWeight_Category2_SQL = wbTotalWeight_Category2_SQL.Replace("*", "(case swbWeight when null then 0 else  cast(round(swbWeight,2) as  numeric(30,2)) end) as  wbTotalWeight");
                            //    dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category2_SQL + wbTotalWeight_Temp_SQL2).Tables[0];
                            //    for (int k = 0; k < dt_wbTotalWeight_Category.Rows.Count; k++)
                            //    {
                            //        str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[k]["wbTotalWeight"].ToString())).ToString();
                            //    }
                            //}
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString();
                            }
                           
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            break;
                        case "wbTotalWeight_Category3":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL3 = wbTotalWeight_Temp_SQL + " and wbCompany like '%" + dt_Total.Rows[i]["wbCompany"].ToString() + "%'";
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category3_SQL + wbTotalWeight_Temp_SQL3).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString();
                            }

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            break;
                        case "wbTotalWeight_Category4":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL4 = wbTotalWeight_Temp_SQL + " and wbCompany like '%" + dt_Total.Rows[i]["wbCompany"].ToString() + "%'";
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category4_SQL + wbTotalWeight_Temp_SQL4).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString();
                            }

                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            break;
                        case "wbTotalWeight_Category5":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL5 = wbTotalWeight_Temp_SQL + " and wbCompany like '%" + dt_Total.Rows[i]["wbCompany"].ToString() + "%'";
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category5_SQL + wbTotalWeight_Temp_SQL5).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString();
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            break;
                        case "wbTotalWeight_Category6":
                            str_wbTotalWeight_Category = "0.00";
                            string wbTotalWeight_Temp_SQL6 = wbTotalWeight_Temp_SQL + " and wbCompany like '%" + dt_Total.Rows[i]["wbCompany"].ToString() + "%'";
                            dt_wbTotalWeight_Category = SqlServerHelper.Query(wbTotalWeight_Category6_SQL + wbTotalWeight_Temp_SQL6).Tables[0];
                            for (int m = 0; m < dt_wbTotalWeight_Category.Rows.Count; m++)
                            {
                                str_wbTotalWeight_Category = (Convert.ToDouble(str_wbTotalWeight_Category) + Convert.ToDouble(dt_wbTotalWeight_Category.Rows[m]["swbWeight"] == DBNull.Value ? "0" : dt_wbTotalWeight_Category.Rows[m]["swbWeight"].ToString())).ToString();
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\",", strFiledArray[j], str_wbTotalWeight_Category);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}公斤\"", strFiledArray[j], str_wbTotalWeight_Category);
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
            sb.Append("}");
            return sb.ToString();
        }

    }
}
