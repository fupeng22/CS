using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Filter;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
using System.Text;
using SQLDAL;
using Microsoft.Reporting.WebForms;
using System.IO;
using Util;
using Model;
using System.Configuration;
using System.Net.Mail;

namespace CS.Controllers.Common
{
    [ErrorAttribute]
    public class RejectSheetSettingController : Controller
    {
        public const string strFileds = "wbCompany,wbStorageDate,wbSerialNum,swbSerialNum,RejectDate,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,swbNeedCheck,wbID,swbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/Huayu_RejectSeet.rdlc";

        //public string STR_SENDER_SMTP = ConfigurationManager.AppSettings["SenderSMTP"];
        //public string STR_SENDER_USERNAME = ConfigurationManager.AppSettings["SenderUserName"];
        //public string STR_SENDER_USERMAIL = ConfigurationManager.AppSettings["SenderUserMail"];
        //public string STR_SENDER_USERPWD = ConfigurationManager.AppSettings["SenderPwd"];
        //public string STR_SENDER_RECIEVEREMAIL = ConfigurationManager.AppSettings["RecieverEmail"];
        //public string STR_SENDER_RECIEVERUSERNAME = ConfigurationManager.AppSettings["RecieverName"];
        //public string STR_CARBONCODY = ConfigurationManager.AppSettings["CarbonCopy"];
        public string STR_TIMEOUT = ConfigurationManager.AppSettings["MaxTimeOut"];
        //public string STR_MAILSUBJECT = ConfigurationManager.AppSettings["MailSubject"];
        //public string STR_MAILBODY = ConfigurationManager.AppSettings["MailBody"];
        //
        // GET: /UnReleaseSheetSettion/

        public ActionResult Index()
        {
            string strWBID = Request.QueryString["wbID"] == null ? "" : Request.QueryString["wbID"].ToString();
            SetViewData(Server.UrlDecode(strWBID));
            return View();
        }

        public string GetData(string order, string page, string rows, string sort, string wbID, string txtBeginD, string txtEndD)
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

            string strWhereTemp = " (swbNeedCheck=99) ";
            string strBeginD = Server.UrlDecode(txtBeginD.ToString());
            string strEndD = Server.UrlDecode(txtEndD.ToString());
            string strWBID = Server.UrlDecode(wbID.ToString());

            if (strWBID != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (wbID =" + strWBID + ") ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + "  (wbID =" + strWBID + ") ";
                }
            }

            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (convert(nvarchar(10),RejectDate,120)>='{0}' and convert(nvarchar(10),RejectDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (convert(nvarchar(10),RejectDate,120)>='{0}' and convert(nvarchar(10),RejectDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
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
                        case "RejectDate":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
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

        protected void SetViewData(string wbID)
        {
            DataTable dt = null;
            DataSet ds = null;
            string wbSerialNum = "";
            string InStoreDate_ForSetting = "";//入库日期
            string IntervalDays_ForSetting = "0";//入库到提货的天数

            ds = (new T_WayBill()).getWayBillInfo(wbID);
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    wbSerialNum = dt.Rows[0]["wbSerialNum"].ToString();
                }
            }

            ViewData["wbSerialNum_ForSetting"] = wbSerialNum;
            ViewData["wbID_ForSetting"] = wbID;
            ViewData["FlowNum_ForSetting"] = DateTime.Now.ToString("yyyyMMddHHmmss") + wbSerialNum;

            //得到其入库日期，每票总运单的分运单入库日期都是一天，没有例外
            DataSet dsWayBillFlow_InStoreDate = new T_WayBillFlow().getTop1SubWayBill(wbID);
            if (dsWayBillFlow_InStoreDate != null)
            {
                DataTable dtWayBillFlow_InStoreDate = dsWayBillFlow_InStoreDate.Tables[0];
                if (dtWayBillFlow_InStoreDate != null && dtWayBillFlow_InStoreDate.Rows.Count > 0)
                {
                    InStoreDate_ForSetting = Convert.ToDateTime(dtWayBillFlow_InStoreDate.Rows[0]["OutStoreDate"].ToString()).ToString("yyyy-MM-dd");
                    IntervalDays_ForSetting = DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(Convert.ToDateTime(InStoreDate_ForSetting).ToString("yyyy-MM-dd")), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))).ToString();
                    //switch (dtWayBillFlow_InStoreDate.Rows[0]["InOutStoreType"].ToString())
                    //{
                    //    case "3":
                    //        try
                    //        {
                    //            InStoreDate_ForSetting = Convert.ToDateTime(dtWayBillFlow_InStoreDate.Rows[0]["InOutStoreDate"].ToString()).ToString("yyyy-MM-dd");
                    //            IntervalDays_ForSetting = DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(Convert.ToDateTime(InStoreDate_ForSetting).ToString("yyyy-MM-dd")), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))).ToString();
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            IntervalDays_ForSetting = "0";
                    //        }
                    //        break;
                    //    default:
                    //        InStoreDate_ForSetting = "未入库或已出库";
                    //        break;
                    //}
                }
            }
            ViewData["InStoreDate_ForSetting"] = InStoreDate_ForSetting;
            ViewData["IntervalDays_ForSetting"] = IntervalDays_ForSetting;

            //生成默认的扣货统计信息
            ComputeRejectInfo(wbID, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"));
        }

        [HttpPost]
        public string ManualComputeFee(string txtBeginD, string txtEndD, string RejectNum, string RejectWeight, string CustomCategory)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"计算失败.原因未知\"}";
            string KeepFee_ForSetting = "0.00";//仓储费
            string RejectFee_ForSetting = "0.00";//退运费

            txtBeginD = Server.UrlDecode(txtBeginD);
            txtEndD = Server.UrlDecode(txtEndD);
            RejectNum = Server.UrlDecode(RejectNum);
            RejectWeight = Server.UrlDecode(RejectWeight);
            CustomCategory = Server.UrlDecode(CustomCategory);

            try
            {
                double iDiffDays = Util.DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(txtBeginD), Convert.ToDateTime(txtEndD));
                //计算费用
                switch (CustomCategory)
                {
                    case "2"://样品
                        //仓储费
                        KeepFee_ForSetting = (Convert.ToDouble(RejectNum) * (iDiffDays - 3 < 0 ? 0 : (iDiffDays - 3)) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                        //退运费
                        RejectFee_ForSetting = (Convert.ToDouble(RejectWeight) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                        break;
                    case "3"://KJ-3
                        //仓储费
                        KeepFee_ForSetting = (Convert.ToDouble(RejectNum) * (iDiffDays - 3 < 0 ? 0 : (iDiffDays - 3)) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                        //退运费
                        RejectFee_ForSetting = (Convert.ToDouble(RejectWeight) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                        break;
                    case "4"://D类
                        //仓储费
                        KeepFee_ForSetting = (Convert.ToDouble(RejectNum) * (iDiffDays - 3 < 0 ? 0 : (iDiffDays - 3)) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                        //退运费
                        RejectFee_ForSetting = (Convert.ToDouble(RejectWeight) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                        break;
                    case "5"://个人物品
                        //扣货费
                        KeepFee_ForSetting = ((iDiffDays - 7 < 0 ? 0 : (iDiffDays - 7)) * Convert.ToDouble(RejectWeight) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("5-1-5"))).ToString("0.00");
                        //退运费
                        RejectFee_ForSetting = (Convert.ToDouble(RejectWeight) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                        break;
                    case "6"://分运行李
                        //扣货费
                        KeepFee_ForSetting = ((iDiffDays - 7 < 0 ? 0 : (iDiffDays - 7)) * Convert.ToDouble(RejectWeight) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("5-1-5"))).ToString("0.00");
                        //退运费
                        RejectFee_ForSetting = (Convert.ToDouble(RejectWeight) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                        break;
                    default:
                        break;
                }
                strRet = "{\"result\":\"" + "ok" + "\",\"message\":\"" + "" + "\",\"row\":[{\"KeepFee_ForSetting\":\"" + KeepFee_ForSetting + "\",\"RejectFee_ForSetting\":\"" + RejectFee_ForSetting + "\"}]}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"计算失败.原因:" + ex.Message + "\"}";
            }
            return strRet;
        }

        [HttpPost]
        public string ComputeRejectInfo(string wbID, string beginD, string endD)
        {
            string resulr = "error";
            string message = "计算失败，原因未知";
            string RejectNum_ForSetting = "0";//退运总件数
            string RejectWeight_ForSetting = "0.00";//退运总重量
            string CustomCategory_ForSetting = "";//业务类型
            string hid_CustomCategory_ForSetting = "";//隐藏业务类型
            string KeepFee_ForSetting = "0.00";//仓储费
            string RejectFee_ForSetting = "0.00";//退运费

            string strRet = "";
            beginD = Server.UrlDecode(beginD);
            endD = Server.UrlDecode(endD);

            DataSet ds_RejectWayBill = null;
            DataTable dt = null;
            ds_RejectWayBill = (new T_RejectSubWayBill()).getRejectSubWayBillInfo(wbID, beginD, endD);
            if (ds_RejectWayBill != null)
            {
                dt = ds_RejectWayBill.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    //得到曾经扣留的件数，以及扣留的货物总重量:
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            //RejectNum_ForSetting = (Convert.ToInt32(RejectNum_ForSetting) + Convert.ToInt32(dt.Rows[i]["swbNumber"].ToString())).ToString();
                            RejectNum_ForSetting = (Convert.ToInt32(RejectNum_ForSetting) + 1).ToString();
                            RejectWeight_ForSetting = (Convert.ToDouble(RejectWeight_ForSetting) + Convert.ToDouble(dt.Rows[i]["swbWeight"].ToString())).ToString();
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    ViewData["RejectNum_ForSetting"] = RejectNum_ForSetting;
                    ViewData["RejectWeight_ForSetting"] = RejectWeight_ForSetting;
                    resulr = "ok";
                }
            }

            ds_RejectWayBill = new T_SubWayBill().GetSubWayBillInfoBywbID(Convert.ToInt32(wbID));
            if (ds_RejectWayBill != null)
            {
                dt = ds_RejectWayBill.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    CustomCategory_ForSetting = dt.Rows[0]["swbCustomsCategory"].ToString();
                    switch (CustomCategory_ForSetting)
                    {
                        case "2":
                            CustomCategory_ForSetting = "样品";
                            break;
                        case "3":
                            CustomCategory_ForSetting = "KJ-3";
                            break;
                        case "4":
                            CustomCategory_ForSetting = "D类";
                            break;
                        case "5":
                            CustomCategory_ForSetting = "个人物品";
                            break;
                        case "6":
                            CustomCategory_ForSetting = "分运行李";
                            break;
                        default:
                            break;
                    }
                    hid_CustomCategory_ForSetting = dt.Rows[0]["swbCustomsCategory"].ToString();
                }
            }

            ViewData["CustomCategory_ForSetting"] = CustomCategory_ForSetting;
            ViewData["hid_CustomCategory_ForSetting"] = hid_CustomCategory_ForSetting;

            //计算费用
            switch (hid_CustomCategory_ForSetting)
            {
                case "2"://样品
                    //仓储费
                    KeepFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3) * Convert.ToInt32(RejectNum_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                    //退运费
                    RejectFee_ForSetting = (Convert.ToDouble(RejectWeight_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                    break;
                case "3"://KJ-3
                    //仓储费
                    KeepFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3) * Convert.ToInt32(RejectNum_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                    //退运费
                    RejectFee_ForSetting = (Convert.ToDouble(RejectWeight_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                    break;
                case "4"://D类
                    //仓储费
                    KeepFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3) * Convert.ToInt32(RejectNum_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                    //退运费
                    RejectFee_ForSetting = (Convert.ToDouble(RejectWeight_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                    break;
                case "5"://个人物品
                    //扣货费
                    KeepFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 7 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 7) * Convert.ToDouble(RejectWeight_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("5-1-5"))).ToString("0.00");
                    //退运费
                    RejectFee_ForSetting = (Convert.ToDouble(RejectWeight_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                    break;
                case "6"://分运行李
                    //扣货费
                    KeepFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 7 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 7) * Convert.ToDouble(RejectWeight_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("6-1-5"))).ToString("0.00");
                    //退运费
                    RejectFee_ForSetting = (Convert.ToDouble(RejectWeight_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("98-1-1"))).ToString("0.00");

                    break;
                default:
                    break;
            }
            ViewData["KeepFee_ForSetting"] = KeepFee_ForSetting;
            ViewData["RejectFee_ForSetting"] = RejectFee_ForSetting;

            strRet = "{\"result\":\"" + resulr + "\",\"message\":\"" + message + "\",\"row\":[{\"RejectNum_ForSetting\":\"" + RejectNum_ForSetting + "\",\"RejectWeight_ForSetting\":\"" + RejectWeight_ForSetting + "\",\"CustomCategory_ForSetting\":\"" + CustomCategory_ForSetting + "\",\"hid_CustomCategory_ForSetting\":\"" + hid_CustomCategory_ForSetting + "\",\"KeepFee_ForSetting\":\"" + KeepFee_ForSetting + "\",\"RejectFee_ForSetting\":\"" + RejectFee_ForSetting + "\"}]}";
            return strRet;
        }

        /// <summary>
        /// 打印扣货单
        /// </summary>
        /// <param name="strCurrentReleaseSubWayBill">本次放行的分运单信息(单号1,单号2……)</param>
        /// <param name="strWBID"></param>
        /// <param name="iPrintType">0:打印预览   1:确认打印</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Print(string strCurrentReleaseSubWayBill, string strWBID, string txtBeginD, string txtEndD, int iPrintType, string FlowNum_ForSetting, string wbSerialNum_ForPrint, string InStoreDate_ForSetting, string RejectDate_ForSetting, string RejectNum_ForSetting, string RejectWeight_ForSetting, string RejectFee_ForSetting, string KeepFee_ForSetting, string ddlPayMode_ForSetting, string TotalFee_ForSetting)
        {
            string CurrentReleaseSubWayBill = Server.UrlDecode(strCurrentReleaseSubWayBill).Replace("，", ",");
            DataSet dsRejectSubWayBill = new DataSet();
            string strSQL = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                strSQL = string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),RejectDate,120) RejectDate from  V_WayBill_SubWayBill where wbID={0}  and (convert(nvarchar(10),RejectDate,120)>='{1}' and convert(nvarchar(10),RejectDate,120)<='{2}') ", strWBID, Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
            }
            else
            {
                strSQL = string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),RejectDate,120) RejectDate from  V_WayBill_SubWayBill where wbID={0} ", strWBID);
            }
            if (strSQL.ToString() != "")
            {
                dsRejectSubWayBill = SqlServerHelper.Query(strSQL);
            }

            string str_FlowNum_ForSetting = FlowNum_ForSetting == null ? "" : Server.UrlDecode(FlowNum_ForSetting);
            string str_wbSerialNum_ForPrint = wbSerialNum_ForPrint == null ? "" : Server.UrlDecode(wbSerialNum_ForPrint);
            string str_InStoreDate_ForSetting = InStoreDate_ForSetting == null ? "" : Server.UrlDecode(InStoreDate_ForSetting);
            string str_RejectDate_ForSetting = RejectDate_ForSetting == null ? "" : Server.UrlDecode(RejectDate_ForSetting);
            string str_RejectNum_ForSetting = RejectNum_ForSetting == null ? "" : Server.UrlDecode(RejectNum_ForSetting);
            string str_RejectWeight_ForSetting = RejectWeight_ForSetting == null ? "" : Server.UrlDecode(RejectWeight_ForSetting);
            string str_RejectFee_ForSetting = RejectFee_ForSetting == null ? "" : Server.UrlDecode(RejectFee_ForSetting);
            string str_KeepFee_ForSetting = KeepFee_ForSetting == null ? "" : Server.UrlDecode(KeepFee_ForSetting);
            string str_TotalFee_ForSetting = TotalFee_ForSetting == null ? "" : Server.UrlDecode(TotalFee_ForSetting);
            string str_ddlPayMode_ForSetting = ddlPayMode_ForSetting == null ? "" : Server.UrlDecode(ddlPayMode_ForSetting);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource dtRejectSubWayBill = new ReportDataSource("PrintRejectSubWayBill_DS", dsRejectSubWayBill.Tables[0]);

            ReportParameter var_FlowNum_ForPrint = new ReportParameter("FlowNum_ForSetting", str_FlowNum_ForSetting.ToString());
            ReportParameter var_wbSerialNum_ForPrint = new ReportParameter("wbSerialNum_ForPrint", str_wbSerialNum_ForPrint.ToString());
            ReportParameter var_InStoreDate_ForSetting = new ReportParameter("InStoreDate_ForSetting", str_InStoreDate_ForSetting.ToString());
            ReportParameter var_RejectDate_ForSetting = new ReportParameter("RejectDate_ForSetting", str_RejectDate_ForSetting.ToString());
            ReportParameter var_RejectNum_ForSetting = new ReportParameter("RejectNum_ForSetting", str_RejectNum_ForSetting.ToString());
            ReportParameter var_RejectWeight_ForSetting = new ReportParameter("RejectWeight_ForSetting", str_RejectWeight_ForSetting.ToString());
            ReportParameter var_RejectFee_ForSetting = new ReportParameter("RejectFee_ForSetting", str_RejectFee_ForSetting.ToString());
            ReportParameter var_KeepFee_ForSetting = new ReportParameter("KeepFee_ForSetting", str_KeepFee_ForSetting.ToString());
            ReportParameter var_ddlPayMode_ForSetting = new ReportParameter("ddlPayMode_ForSetting", str_ddlPayMode_ForSetting.ToString());
            ReportParameter var_TotalFee_ForSetting = new ReportParameter("TotalFee_ForSetting", str_TotalFee_ForSetting.ToString());

            localReport.SetParameters(new ReportParameter[] { var_FlowNum_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_wbSerialNum_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_InStoreDate_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_RejectDate_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_RejectNum_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_RejectWeight_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_RejectFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_KeepFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_ddlPayMode_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_TotalFee_ForSetting });

            localReport.DataSources.Add(dtRejectSubWayBill);
            string reportType = "PDF";
            string mimeType;
            string encoding = "UTF-8";
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
                " <OutputFormat>PDF</OutputFormat>" +
                " <PageWidth>12in</PageWidth>" +
                " <PageHeigth>6in</PageHeigth>" +
                " <MarginTop>0.2in</MarginTop>" +
                " <MarginLeft>1in</MarginLeft>" +
                " <MarginRight>1in</MarginRight>" +
                " <MarginBottom>0.2in</MarginBottom>" +
                " </DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            return File(renderedBytes, mimeType);
        }

        [HttpPost]
        public string SendMail_PDF(string strCurrentReleaseSubWayBill, string strWBID, string txtBeginD, string txtEndD, int iPrintType, string FlowNum_ForSetting, string wbSerialNum_ForPrint, string InStoreDate_ForSetting, string RejectDate_ForSetting, string RejectNum_ForSetting, string RejectWeight_ForSetting, string RejectFee_ForSetting, string KeepFee_ForSetting, string ddlPayMode_ForSetting, string TotalFee_ForSetting)
        {
            string strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因未知\"}";
            string wbCompany = "";
            try
            {
                DataSet dsWayBill = new T_WayBill().getWayBillInfo(strWBID);
                if (dsWayBill != null)
                {
                    if (dsWayBill.Tables[0] != null && dsWayBill.Tables[0].Rows.Count > 0)
                    {
                        wbCompany = dsWayBill.Tables[0].Rows[0]["wbCompany"].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(wbCompany))
                {
                    DataSet dsUser = new T_User().GetUserByCompany(wbCompany);
                    if (dsUser != null)
                    {
                        if (dsUser.Tables[0] != null && dsUser.Tables[0].Rows.Count > 0)
                        {
                            if (dsUser.Tables[0].Rows[0]["iSendRejectGoodsEmail"].ToString() == "1")
                            {
                                string CurrentReleaseSubWayBill = Server.UrlDecode(strCurrentReleaseSubWayBill).Replace("，", ",");
                                DataSet dsRejectSubWayBill = new DataSet();
                                string strSQL = "";
                                if (txtBeginD != "" && txtEndD != "")
                                {
                                    strSQL = string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),RejectDate,120) RejectDate from  V_WayBill_SubWayBill where wbID={0}  and (convert(nvarchar(10),RejectDate,120)>='{1}' and convert(nvarchar(10),RejectDate,120)<='{2}') ", strWBID, Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    strSQL = string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),RejectDate,120) RejectDate from  V_WayBill_SubWayBill where wbID={0} ", strWBID);
                                }
                                if (strSQL.ToString() != "")
                                {
                                    dsRejectSubWayBill = SqlServerHelper.Query(strSQL);
                                }

                                string str_FlowNum_ForSetting = FlowNum_ForSetting == null ? "" : Server.UrlDecode(FlowNum_ForSetting);
                                string str_wbSerialNum_ForPrint = wbSerialNum_ForPrint == null ? "" : Server.UrlDecode(wbSerialNum_ForPrint);
                                string str_InStoreDate_ForSetting = InStoreDate_ForSetting == null ? "" : Server.UrlDecode(InStoreDate_ForSetting);
                                string str_RejectDate_ForSetting = RejectDate_ForSetting == null ? "" : Server.UrlDecode(RejectDate_ForSetting);
                                string str_RejectNum_ForSetting = RejectNum_ForSetting == null ? "" : Server.UrlDecode(RejectNum_ForSetting);
                                string str_RejectWeight_ForSetting = RejectWeight_ForSetting == null ? "" : Server.UrlDecode(RejectWeight_ForSetting);
                                string str_RejectFee_ForSetting = RejectFee_ForSetting == null ? "" : Server.UrlDecode(RejectFee_ForSetting);
                                string str_KeepFee_ForSetting = KeepFee_ForSetting == null ? "" : Server.UrlDecode(KeepFee_ForSetting);
                                string str_TotalFee_ForSetting = TotalFee_ForSetting == null ? "" : Server.UrlDecode(TotalFee_ForSetting);
                                string str_ddlPayMode_ForSetting = ddlPayMode_ForSetting == null ? "" : Server.UrlDecode(ddlPayMode_ForSetting);

                                LocalReport localReport = new LocalReport();
                                localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
                                ReportDataSource dtRejectSubWayBill = new ReportDataSource("PrintRejectSubWayBill_DS", dsRejectSubWayBill.Tables[0]);

                                ReportParameter var_FlowNum_ForPrint = new ReportParameter("FlowNum_ForSetting", str_FlowNum_ForSetting.ToString());
                                ReportParameter var_wbSerialNum_ForPrint = new ReportParameter("wbSerialNum_ForPrint", str_wbSerialNum_ForPrint.ToString());
                                ReportParameter var_InStoreDate_ForSetting = new ReportParameter("InStoreDate_ForSetting", str_InStoreDate_ForSetting.ToString());
                                ReportParameter var_RejectDate_ForSetting = new ReportParameter("RejectDate_ForSetting", str_RejectDate_ForSetting.ToString());
                                ReportParameter var_RejectNum_ForSetting = new ReportParameter("RejectNum_ForSetting", str_RejectNum_ForSetting.ToString());
                                ReportParameter var_RejectWeight_ForSetting = new ReportParameter("RejectWeight_ForSetting", str_RejectWeight_ForSetting.ToString());
                                ReportParameter var_RejectFee_ForSetting = new ReportParameter("RejectFee_ForSetting", str_RejectFee_ForSetting.ToString());
                                ReportParameter var_KeepFee_ForSetting = new ReportParameter("KeepFee_ForSetting", str_KeepFee_ForSetting.ToString());
                                ReportParameter var_ddlPayMode_ForSetting = new ReportParameter("ddlPayMode_ForSetting", str_ddlPayMode_ForSetting.ToString());
                                ReportParameter var_TotalFee_ForSetting = new ReportParameter("TotalFee_ForSetting", str_TotalFee_ForSetting.ToString());

                                localReport.SetParameters(new ReportParameter[] { var_FlowNum_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_wbSerialNum_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_InStoreDate_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_RejectDate_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_RejectNum_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_RejectWeight_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_RejectFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_KeepFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_ddlPayMode_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_TotalFee_ForSetting });

                                localReport.DataSources.Add(dtRejectSubWayBill);
                                string reportType = "PDF";
                                string mimeType;
                                string encoding = "UTF-8";
                                string fileNameExtension;

                                string deviceInfo = "<DeviceInfo>" +
                                    " <OutputFormat>PDF</OutputFormat>" +
                                    " <PageWidth>12in</PageWidth>" +
                                    " <PageHeigth>6in</PageHeigth>" +
                                    " <MarginTop>0.2in</MarginTop>" +
                                    " <MarginLeft>1in</MarginLeft>" +
                                    " <MarginRight>1in</MarginRight>" +
                                    " <MarginBottom>0.2in</MarginBottom>" +
                                    " </DeviceInfo>";

                                Warning[] warnings;
                                string[] streams;
                                byte[] renderedBytes;

                                renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

                                switch (iPrintType)
                                {
                                    case 1:
                                        try
                                        {
                                            string FileName = Server.MapPath("~/Temp/PDF/") + "快件退运单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                                            using (FileStream wf = new FileStream(FileName, FileMode.Create))
                                            {
                                                wf.Write(renderedBytes, 0, renderedBytes.Length);
                                                wf.Flush();
                                                wf.Close();

                                                if (NetMail_SendMail(FileName, wbCompany, "0"))
                                                {
                                                    strResult = "{\"result\":\"ok\",\"message\":\"发送邮件成功\"}";
                                                }
                                                else
                                                {
                                                    strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败\"}";
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因:" + ex.Message + "\"}";
                                        }

                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                strResult = "{\"result\":\"ok\",\"message\":\"无需邮件推送\"}";
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因:" + ex.Message + "\"}";
            }
            return strResult;
        }

        /// <summary>
        /// 导出退运单
        /// </summary>
        /// <param name="strCurrentReleaseSubWayBill">本次放行的分运单信息(单号1,单号2……)</param>
        /// <param name="strWBID"></param>
        /// <param name="iPrintType">0:导出预览   1:确认导出</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Excel(string strCurrentReleaseSubWayBill, string strWBID, string txtBeginD, string txtEndD, int iPrintType, string FlowNum_ForSetting, string wbSerialNum_ForPrint, string InStoreDate_ForSetting, string RejectDate_ForSetting, string RejectNum_ForSetting, string RejectWeight_ForSetting, string RejectFee_ForSetting, string KeepFee_ForSetting, string ddlPayMode_ForSetting, string TotalFee_ForSetting, string browserType)
        {
            string CurrentReleaseSubWayBill = Server.UrlDecode(strCurrentReleaseSubWayBill).Replace("，", ",");
            DataSet dsRejectSubWayBill = new DataSet();
            string strSQL = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                strSQL = string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),RejectDate,120) RejectDate from  V_WayBill_SubWayBill where wbID={0}  and (convert(nvarchar(10),RejectDate,120)>='{1}' and convert(nvarchar(10),RejectDate,120)<='{2}') ", strWBID, Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
            }
            else
            {
                strSQL = string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),RejectDate,120) RejectDate from  V_WayBill_SubWayBill where wbID={0} ", strWBID);
            }
            if (strSQL.ToString() != "")
            {
                dsRejectSubWayBill = SqlServerHelper.Query(strSQL);
            }

            string str_FlowNum_ForSetting = FlowNum_ForSetting == null ? "" : Server.UrlDecode(FlowNum_ForSetting);
            string str_wbSerialNum_ForPrint = wbSerialNum_ForPrint == null ? "" : Server.UrlDecode(wbSerialNum_ForPrint);
            string str_InStoreDate_ForSetting = InStoreDate_ForSetting == null ? "" : Server.UrlDecode(InStoreDate_ForSetting);
            string str_RejectDate_ForSetting = RejectDate_ForSetting == null ? "" : Server.UrlDecode(RejectDate_ForSetting);
            string str_RejectNum_ForSetting = RejectNum_ForSetting == null ? "" : Server.UrlDecode(RejectNum_ForSetting);
            string str_RejectWeight_ForSetting = RejectWeight_ForSetting == null ? "" : Server.UrlDecode(RejectWeight_ForSetting);
            string str_RejectFee_ForSetting = RejectFee_ForSetting == null ? "" : Server.UrlDecode(RejectFee_ForSetting);
            string str_KeepFee_ForSetting = KeepFee_ForSetting == null ? "" : Server.UrlDecode(KeepFee_ForSetting);
            string str_TotalFee_ForSetting = TotalFee_ForSetting == null ? "" : Server.UrlDecode(TotalFee_ForSetting);
            string str_ddlPayMode_ForSetting = ddlPayMode_ForSetting == null ? "" : Server.UrlDecode(ddlPayMode_ForSetting);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource dtRejectSubWayBill = new ReportDataSource("PrintRejectSubWayBill_DS", dsRejectSubWayBill.Tables[0]);

            ReportParameter var_FlowNum_ForPrint = new ReportParameter("FlowNum_ForSetting", str_FlowNum_ForSetting.ToString());
            ReportParameter var_wbSerialNum_ForPrint = new ReportParameter("wbSerialNum_ForPrint", str_wbSerialNum_ForPrint.ToString());
            ReportParameter var_InStoreDate_ForSetting = new ReportParameter("InStoreDate_ForSetting", str_InStoreDate_ForSetting.ToString());
            ReportParameter var_RejectDate_ForSetting = new ReportParameter("RejectDate_ForSetting", str_RejectDate_ForSetting.ToString());
            ReportParameter var_RejectNum_ForSetting = new ReportParameter("RejectNum_ForSetting", str_RejectNum_ForSetting.ToString());
            ReportParameter var_RejectWeight_ForSetting = new ReportParameter("RejectWeight_ForSetting", str_RejectWeight_ForSetting.ToString());
            ReportParameter var_RejectFee_ForSetting = new ReportParameter("RejectFee_ForSetting", str_RejectFee_ForSetting.ToString());
            ReportParameter var_KeepFee_ForSetting = new ReportParameter("KeepFee_ForSetting", str_KeepFee_ForSetting.ToString());
            ReportParameter var_ddlPayMode_ForSetting = new ReportParameter("ddlPayMode_ForSetting", str_ddlPayMode_ForSetting.ToString());
            ReportParameter var_TotalFee_ForSetting = new ReportParameter("TotalFee_ForSetting", str_TotalFee_ForSetting.ToString());

            localReport.SetParameters(new ReportParameter[] { var_FlowNum_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_wbSerialNum_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_InStoreDate_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_RejectDate_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_RejectNum_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_RejectWeight_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_RejectFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_KeepFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_ddlPayMode_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_TotalFee_ForSetting });

            localReport.DataSources.Add(dtRejectSubWayBill);
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

            string strOutputFileName = "快件退运单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
        public string SendEmail_Excel(string strCurrentReleaseSubWayBill, string strWBID, string txtBeginD, string txtEndD, int iPrintType, string FlowNum_ForSetting, string wbSerialNum_ForPrint, string InStoreDate_ForSetting, string RejectDate_ForSetting, string RejectNum_ForSetting, string RejectWeight_ForSetting, string RejectFee_ForSetting, string KeepFee_ForSetting, string ddlPayMode_ForSetting, string TotalFee_ForSetting, string browserType)
        {
            string strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因未知\"}";
            string wbCompany = "";
            try
            {
                DataSet dsWayBill = new T_WayBill().getWayBillInfo(strWBID);
                if (dsWayBill != null)
                {
                    if (dsWayBill.Tables[0] != null && dsWayBill.Tables[0].Rows.Count > 0)
                    {
                        wbCompany = dsWayBill.Tables[0].Rows[0]["wbCompany"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(wbCompany))
                {
                    DataSet dsUser = new T_User().GetUserByCompany(wbCompany);
                    if (dsUser != null)
                    {
                        if (dsUser.Tables[0] != null && dsUser.Tables[0].Rows.Count > 0)
                        {
                            if (dsUser.Tables[0].Rows[0]["iSendRejectGoodsEmail"].ToString() == "1")
                            {
                                string CurrentReleaseSubWayBill = Server.UrlDecode(strCurrentReleaseSubWayBill).Replace("，", ",");
                                DataSet dsRejectSubWayBill = new DataSet();
                                string strSQL = "";
                                if (txtBeginD != "" && txtEndD != "")
                                {
                                    strSQL = string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),RejectDate,120) RejectDate from  V_WayBill_SubWayBill where wbID={0}  and (convert(nvarchar(10),RejectDate,120)>='{1}' and convert(nvarchar(10),RejectDate,120)<='{2}') ", strWBID, Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    strSQL = string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),RejectDate,120) RejectDate from  V_WayBill_SubWayBill where wbID={0} ", strWBID);
                                }
                                if (strSQL.ToString() != "")
                                {
                                    dsRejectSubWayBill = SqlServerHelper.Query(strSQL);
                                }

                                string str_FlowNum_ForSetting = FlowNum_ForSetting == null ? "" : Server.UrlDecode(FlowNum_ForSetting);
                                string str_wbSerialNum_ForPrint = wbSerialNum_ForPrint == null ? "" : Server.UrlDecode(wbSerialNum_ForPrint);
                                string str_InStoreDate_ForSetting = InStoreDate_ForSetting == null ? "" : Server.UrlDecode(InStoreDate_ForSetting);
                                string str_RejectDate_ForSetting = RejectDate_ForSetting == null ? "" : Server.UrlDecode(RejectDate_ForSetting);
                                string str_RejectNum_ForSetting = RejectNum_ForSetting == null ? "" : Server.UrlDecode(RejectNum_ForSetting);
                                string str_RejectWeight_ForSetting = RejectWeight_ForSetting == null ? "" : Server.UrlDecode(RejectWeight_ForSetting);
                                string str_RejectFee_ForSetting = RejectFee_ForSetting == null ? "" : Server.UrlDecode(RejectFee_ForSetting);
                                string str_KeepFee_ForSetting = KeepFee_ForSetting == null ? "" : Server.UrlDecode(KeepFee_ForSetting);
                                string str_TotalFee_ForSetting = TotalFee_ForSetting == null ? "" : Server.UrlDecode(TotalFee_ForSetting);
                                string str_ddlPayMode_ForSetting = ddlPayMode_ForSetting == null ? "" : Server.UrlDecode(ddlPayMode_ForSetting);

                                LocalReport localReport = new LocalReport();
                                localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
                                ReportDataSource dtRejectSubWayBill = new ReportDataSource("PrintRejectSubWayBill_DS", dsRejectSubWayBill.Tables[0]);

                                ReportParameter var_FlowNum_ForPrint = new ReportParameter("FlowNum_ForSetting", str_FlowNum_ForSetting.ToString());
                                ReportParameter var_wbSerialNum_ForPrint = new ReportParameter("wbSerialNum_ForPrint", str_wbSerialNum_ForPrint.ToString());
                                ReportParameter var_InStoreDate_ForSetting = new ReportParameter("InStoreDate_ForSetting", str_InStoreDate_ForSetting.ToString());
                                ReportParameter var_RejectDate_ForSetting = new ReportParameter("RejectDate_ForSetting", str_RejectDate_ForSetting.ToString());
                                ReportParameter var_RejectNum_ForSetting = new ReportParameter("RejectNum_ForSetting", str_RejectNum_ForSetting.ToString());
                                ReportParameter var_RejectWeight_ForSetting = new ReportParameter("RejectWeight_ForSetting", str_RejectWeight_ForSetting.ToString());
                                ReportParameter var_RejectFee_ForSetting = new ReportParameter("RejectFee_ForSetting", str_RejectFee_ForSetting.ToString());
                                ReportParameter var_KeepFee_ForSetting = new ReportParameter("KeepFee_ForSetting", str_KeepFee_ForSetting.ToString());
                                ReportParameter var_ddlPayMode_ForSetting = new ReportParameter("ddlPayMode_ForSetting", str_ddlPayMode_ForSetting.ToString());
                                ReportParameter var_TotalFee_ForSetting = new ReportParameter("TotalFee_ForSetting", str_TotalFee_ForSetting.ToString());

                                localReport.SetParameters(new ReportParameter[] { var_FlowNum_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_wbSerialNum_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_InStoreDate_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_RejectDate_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_RejectNum_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_RejectWeight_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_RejectFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_KeepFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_ddlPayMode_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_TotalFee_ForSetting });

                                localReport.DataSources.Add(dtRejectSubWayBill);
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

                                string strOutputFileName = "快件退运单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

                                switch (iPrintType)
                                {
                                    case 1:
                                        try
                                        {
                                            string FileName = Server.MapPath("~/Temp/PDF/") + strOutputFileName;
                                            using (FileStream wf = new FileStream(FileName, FileMode.Create))
                                            {
                                                wf.Write(bytes, 0, bytes.Length);
                                                wf.Flush();
                                                wf.Close();
                                                if (NetMail_SendMail(FileName, wbCompany, "1"))
                                                {
                                                    strResult = "{\"result\":\"ok\",\"message\":\"发送邮件成功\"}";
                                                }
                                                else
                                                {
                                                    strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败\"}";
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因:" + ex.Message + "\"}";
                                        }

                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                strResult = "{\"result\":\"ok\",\"message\":\"无需邮件推送\"}";
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                strResult = "{\"result\":\"error\",\"message\":\"发送邮件失败，原因:" + ex.Message + "\"}";
            }
            return strResult;
        }

        [HttpPost]
        public string SaveSaleInfo(string FlowNum_ForPrint, string hid_CustomCategory_ForSetting, string wbID_ForPrint, string InStoreDate_ForSetting, string PickGoodsDate_ForSetting, string wbActualWeight_ForPrint, string OperateFee_ForSetting, string PickGoodsFee_ForSetting, string KeepGoodsFee_ForSetting, string ShiftGoodsFee_ForSetting, string CollectionFee_ForSetting, string ddlPayMode_ForSetting, string ShouldPayUnit_ForSetting, string shouldPay_ForSetting, string TotalFee_ForSetting, string ddlReceiptMode_ForSetting, string Receipt_ForSetting, string RejectFee_ForSetting)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"销售报表保存失败,不允许继续打印或导出,原因未知\"}";
            M_WayBillDailyReport m_WayBillDailyReport = null;
            FlowNum_ForPrint = Server.UrlDecode(FlowNum_ForPrint);
            hid_CustomCategory_ForSetting = Server.UrlDecode(hid_CustomCategory_ForSetting);
            wbID_ForPrint = Server.UrlDecode(wbID_ForPrint);
            InStoreDate_ForSetting = Server.UrlDecode(InStoreDate_ForSetting);
            PickGoodsDate_ForSetting = Server.UrlDecode(PickGoodsDate_ForSetting);
            wbActualWeight_ForPrint = Server.UrlDecode(wbActualWeight_ForPrint);
            OperateFee_ForSetting = Server.UrlDecode(OperateFee_ForSetting);
            PickGoodsFee_ForSetting = Server.UrlDecode(PickGoodsFee_ForSetting);
            KeepGoodsFee_ForSetting = Server.UrlDecode(KeepGoodsFee_ForSetting);
            ShiftGoodsFee_ForSetting = Server.UrlDecode(ShiftGoodsFee_ForSetting);
            CollectionFee_ForSetting = Server.UrlDecode(CollectionFee_ForSetting);
            ddlPayMode_ForSetting = Server.UrlDecode(ddlPayMode_ForSetting);
            ShouldPayUnit_ForSetting = Server.UrlDecode(ShouldPayUnit_ForSetting);
            shouldPay_ForSetting = Server.UrlDecode(shouldPay_ForSetting);
            TotalFee_ForSetting = Server.UrlDecode(TotalFee_ForSetting);
            ddlReceiptMode_ForSetting = Server.UrlDecode(ddlReceiptMode_ForSetting);
            Receipt_ForSetting = Server.UrlDecode(Receipt_ForSetting);
            RejectFee_ForSetting = Server.UrlDecode(RejectFee_ForSetting);

            try
            {
                m_WayBillDailyReport = new M_WayBillDailyReport()
                {
                    wbrCode = FlowNum_ForPrint,
                    CustomsCategory = hid_CustomCategory_ForSetting,
                    wbr_wbID = Convert.ToInt32(wbID_ForPrint),
                    InStoreDate = InStoreDate_ForSetting,
                    OutStoreDate = PickGoodsDate_ForSetting,
                    WayBillActualWeight = wbActualWeight_ForPrint,
                    OperateFee = OperateFee_ForSetting,
                    PickGoodsFee = PickGoodsFee_ForSetting,
                    KeepGoodsFee = KeepGoodsFee_ForSetting,
                    ShiftGoodsFee = ShiftGoodsFee_ForSetting,
                    CollectionKeepGoodsFee = CollectionFee_ForSetting,
                    PayMethod = ddlPayMode_ForSetting,
                    ShouldPayUnit = ShouldPayUnit_ForSetting,
                    shouldPay = shouldPay_ForSetting,
                    ActualPay = TotalFee_ForSetting,
                    ReceptMethod = ddlReceiptMode_ForSetting,
                    Receipt = Receipt_ForSetting,
                    RejectGoodsFee = RejectFee_ForSetting,
                    SalesMan = Session["Global_Huayu_UserName"] == null ? "" : Session["Global_Huayu_UserName"].ToString(),
                    mMemo = ""

                };

                new T_WayBillDailyReport().addWayBillDailyReport(m_WayBillDailyReport);

                strRet = "{\"result\":\"ok\",\"message\":\"销售报表保存成功,开始打印或导出\"}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"销售报表保存失败,不允许继续打印或导出,原因:" + ex.Message + "\"}";
            }

            return strRet;
        }

        private Boolean NetMail_SendMail(string AttachmentFileName, string company, string FileType)
        {
            Boolean bOK = false;
            SmtpClient client = new SmtpClient();
            MailAddress mailTo = null;
            MailMessage mail = null;
            string strSubJect = "";// string.Format(STR_MAILSUBJECT, DateTime.Now.ToString("yyyyMMddHHmmss"));
            string strBody = "";// string.Format(STR_MAILBODY, DateTime.Now.ToString("yyyyMMddHHmmss"));

            string[] arrEmails = null;

            string STR_SENDER_SMTP = "";
            string STR_SENDER_USERMAIL = "";
            string STR_SENDER_USERPWD = "";
            string STR_SENDER_USERNAME = "";

            STR_SENDER_SMTP = new T_EmailManagement().GetEmailContent(EmailType.EmailSenderSMTP);
            STR_SENDER_USERMAIL = new T_EmailManagement().GetEmailContent(EmailType.EmailSenderUserName);
            STR_SENDER_USERPWD = Util.CryptographyTool.Decrypt(new T_EmailManagement().GetEmailContent(EmailType.EmailSenderPwd), "HuayuTAT");
            STR_SENDER_USERNAME = STR_SENDER_USERMAIL;

            client.Host = STR_SENDER_SMTP;
            client.Credentials = new System.Net.NetworkCredential(STR_SENDER_USERMAIL, STR_SENDER_USERPWD);

            DataSet dsUser = new T_User().GetUserByCompany(company);
            if (dsUser != null)
            {
                if (dsUser.Tables[0] != null && dsUser.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        if (dsUser.Tables[0].Rows[0]["iSendRejectGoodsEmail"].ToString() == "1")
                        {
                            arrEmails = dsUser.Tables[0].Rows[0]["LinkMail"].ToString().Replace('，', ',').Split(',');
                            for (int i = 0; i < arrEmails.Length; i++)
                            {
                                if (arrEmails[i].Trim() != "")
                                {
                                    mail = new MailMessage();
                                    mail.From = new MailAddress(STR_SENDER_USERMAIL, STR_SENDER_USERNAME, Encoding.GetEncoding(936));
                                    mailTo = new MailAddress(arrEmails[i].Trim(), company, Encoding.GetEncoding(936));
                                    mail.To.Add(mailTo);
                                    //mail.CC.Add(STR_CARBONCODY);抄送
                                    switch (FileType)
                                    {
                                        case "0":
                                            mail.Attachments.Add(new Attachment(AttachmentFileName, System.Net.Mime.MediaTypeNames.Application.Pdf));
                                            break;
                                        case "1":
                                            mail.Attachments.Add(new Attachment(AttachmentFileName, System.Net.Mime.MediaTypeNames.Application.Octet));
                                            break;
                                        default:
                                            break;
                                    }
                                    strSubJect = new T_EmailManagement().GetEmailContent(EmailType.EmailSubject_RejectGoods).Replace("[Date]", DateTime.Now.ToString("yyyyMMddHHmmss")).Replace("【Date]", DateTime.Now.ToString("yyyyMMddHHmmss")).Replace("[Date】", DateTime.Now.ToString("yyyyMMddHHmmss")).Replace("【Date】", DateTime.Now.ToString("yyyyMMddHHmmss"));
                                    strBody = new T_EmailManagement().GetEmailContent(EmailType.EmailBody_RejectGoods);

                                    mail.Subject = strSubJect;
                                    mail.Body = strBody;
                                    mail.SubjectEncoding = Encoding.UTF8;
                                    mail.IsBodyHtml = true;

                                    client.Timeout = Convert.ToInt32(STR_TIMEOUT);
                                    client.Send(mail);
                                }
                            }


                        }

                        bOK = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }


            return bOK;
        }
    }
}
