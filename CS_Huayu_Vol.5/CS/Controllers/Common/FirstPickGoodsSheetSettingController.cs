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
    public class FirstPickGoodsSheetSettingController : Controller
    {
        public const string strFileds = "wbCompany,wbStorageDate,wbSerialNum,swbSerialNum,DetainDate,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,swbNeedCheck,wbID,swbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/FirstPickGoodsSheetSetting.rdlc";

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

        public string GetData(string order, string page, string rows, string sort, string wbID, string swbNeedCheck)
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

            string strWhereTemp = @" ( ( wbImportType = 0
                                        AND swbNeedCheck = 3
                                      )
                                      OR ( wbImportType = 1
                                           AND swbReleaseFlag <> 3
                                         )
                                    ) ";
            string strSwbNeedCheck = Server.UrlDecode(swbNeedCheck.ToString());
            string strWBID = Server.UrlDecode(wbID.ToString());

            //if (strSwbNeedCheck != "")
            //{
            //    if (strWhereTemp != "")
            //    {
            //        strWhereTemp = strWhereTemp + " and (swbNeedCheck =" + strSwbNeedCheck + ") ";
            //    }
            //    else
            //    {
            //        strWhereTemp = strWhereTemp + "  (swbNeedCheck =" + strSwbNeedCheck + ") ";
            //    }
            //}

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
                        case "DetainDate":
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
            ds = (new T_SubWayBill()).getWayBill_SubWayBill_UnRelease(wbID);
            string strAllUnReleaseSubwayBill = "";
            string wbSerialNum = "";

            string wbSerialNumber_ForPrint = "0";//总件数
            string InStoreDate_ForSetting = "";//入库日期
            string wbCompany_ForSetting = "";//客户名（即导入运单的单位）

            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strAllUnReleaseSubwayBill = strAllUnReleaseSubwayBill + dt.Rows[i]["swbSerialNum"].ToString() + ",";
                    }
                }
            }
            if (strAllUnReleaseSubwayBill.EndsWith(","))
            {
                strAllUnReleaseSubwayBill = strAllUnReleaseSubwayBill.Substring(0, strAllUnReleaseSubwayBill.Length - 1);
            }

            ds = (new T_WayBill()).getWayBillInfo(wbID);
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    wbSerialNum = dt.Rows[0]["wbSerialNum"].ToString();
                    InStoreDate_ForSetting = Convert.ToDateTime(dt.Rows[0]["wbArrivalDate"].ToString()).ToString("yyyy-MM-dd");
                }
            }

            ViewData["wbSerialNum_ForSetting"] = wbSerialNum;
            ViewData["wbID_ForSetting"] = wbID;
            ViewData["OldUnReleaseSubWayBill"] = strAllUnReleaseSubwayBill;
            ViewData["FlowNum_ForSetting"] = DateTime.Now.ToString("yyyyMMddHHmmss") + wbSerialNum;

            //得到其入库日期，每票总运单的分运单入库日期都是一天，没有例外
            //DataSet dsWayBillFlow_InStoreDate = new T_WayBillFlow().getTop1UnReleaseSubWayBill(wbID);
            //if (dsWayBillFlow_InStoreDate != null)
            //{
            //    DataTable dtWayBillFlow_InStoreDate = dsWayBillFlow_InStoreDate.Tables[0];
            //    if (dtWayBillFlow_InStoreDate != null && dtWayBillFlow_InStoreDate.Rows.Count > 0)
            //    {
            //        InStoreDate_ForSetting = Convert.ToDateTime(dtWayBillFlow_InStoreDate.Rows[0]["operateDate"].ToString()).ToString("yyyy-MM-dd");
            //    }
            //}
            ViewData["InStoreDate_ForSetting"] = InStoreDate_ForSetting;

            //得到总运单表中的总件数，总重量
            ds = new T_WayBill().GetWayBill(wbSerialNum);
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    wbSerialNumber_ForPrint = dt.Rows[0]["wbTotalNumber"].ToString();
                    wbCompany_ForSetting = dt.Rows[0]["wbCompany"].ToString();
                }
            }

            ViewData["wbSerialNumber_ForPrint"] = wbSerialNumber_ForPrint;
            ViewData["wbCompany_ForSetting"] = wbCompany_ForSetting;

            //计算分运单统计信息
            ComputeSubWayBillInfo(wbID);
        }

        public string ComputeSubWayBillInfo(string wbID)
        {
            string resulr = "error";
            string message = "计算失败，原因未知";
            string swbTotalNumber_ForPrint = "0";//分运单件数
            string ReleaseNum_ForSetting = "0";//放行件数
            string UnReleaseNum_ForSetting = "0";//扣留件数
            string wbActualWeight_ForPrint = "0.00";//重量
            string CustomCategory_ForSetting = "";//业务类型
            string hid_CustomCategory_ForSetting = "";//隐藏业务类型
            string OperateFee_ForSetting = "0.00";//操作费
            string PickGoodsFee_ForSetting = "0.00";//提货费
            string KeepGoodsFee_ForSetting = "0.00";//仓储费
            string ShiftGoodsFee_ForSetting = "0.00";//移库费
            string ReportSystem_ForSetting = "0.00";//系统申报费
            string QuarantineCheckFee_ForSetting = "0.00";//检验检疫查验费
            string QuarantinePacketFee_ForSetting = "0.00";//检验检疫拆包费
            string strRet = "";
            DataSet dsWayBillWeight = null;
            DataTable dtWayBillWeight = null;

            DataSet ds_SubWayBill = null;
            DataTable dt = null;
            if (wbID != "")
            {
                ds_SubWayBill = new T_SubWayBill().GetSubWayBillSumInfo(Convert.ToInt32(wbID));
                if (ds_SubWayBill != null)
                {
                    dt = ds_SubWayBill.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        swbTotalNumber_ForPrint = dt.Rows[0]["swbTotalNumber"].ToString();

                    }
                }

                ds_SubWayBill = new T_SubWayBill().getWayBill_SubWayBill_UnRelease(wbID);
                if (ds_SubWayBill != null)
                {
                    dt = ds_SubWayBill.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        UnReleaseNum_ForSetting = dt.Rows.Count.ToString();

                    }
                }

                ds_SubWayBill = new T_SubWayBill().getWayBill_SubWayBill_Release(wbID);
                if (ds_SubWayBill != null)
                {
                    dt = ds_SubWayBill.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        ReleaseNum_ForSetting = (Convert.ToInt32(ReleaseNum_ForSetting) + dt.Rows.Count).ToString(); ;
                    }
                }

                //ds_SubWayBill = new T_SubWayBill().getWayBill_SubWayBill(wbID, 2);
                //if (ds_SubWayBill != null)
                //{
                //    dt = ds_SubWayBill.Tables[0];
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        ReleaseNum_ForSetting = (Convert.ToInt32(ReleaseNum_ForSetting) + dt.Rows.Count).ToString(); ;
                //    }
                //}

                resulr = "ok";
            }

            wbActualWeight_ForPrint = new T_WayBillWeight().getWeightForCompute(wbID);

            ds_SubWayBill = new T_SubWayBill().GetSubWayBillInfoBywbID(Convert.ToInt32(wbID));
            if (ds_SubWayBill != null)
            {
                dt = ds_SubWayBill.Tables[0];
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

            ViewData["swbTotalNumber_ForPrint"] = swbTotalNumber_ForPrint;
            ViewData["UnReleaseNum_ForSetting"] = UnReleaseNum_ForSetting;
            ViewData["ReleaseNum_ForSetting"] = ReleaseNum_ForSetting;
            ViewData["CustomCategory_ForSetting"] = CustomCategory_ForSetting;
            ViewData["hid_CustomCategory_ForSetting"] = hid_CustomCategory_ForSetting;
            ViewData["wbActualWeight_ForPrint"] = wbActualWeight_ForPrint;

            //计算费用
            switch (hid_CustomCategory_ForSetting)
            {
                case "2"://样品
                    //操作费
                    //OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                    //提货费
                    //PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-2"))).ToString("0.00");
                    //仓储费
                    KeepGoodsFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3) * Convert.ToDouble(ReleaseNum_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-3"))).ToString("0.00");

                    break;
                case "3"://KJ-3
                    //操作费
                    //OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                    //提货费
                    //PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-2"))).ToString("0.00");
                    //仓储费
                    KeepGoodsFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3) * Convert.ToDouble(ReleaseNum_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-3"))).ToString("0.00");

                    break;
                case "4"://D类
                    //操作费
                    //OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                    //提货费
                    //PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-2"))).ToString("0.00");
                    //仓储费
                    KeepGoodsFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 3) * Convert.ToDouble(ReleaseNum_ForSetting) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-3"))).ToString("0.00");

                    break;
                case "5"://个人物品
                    //操作费
                    //OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("5-1-1"))).ToString("0.00");
                    //提货费
                    //PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("5-1-2"))).ToString("0.00");
                    //仓储费
                    KeepGoodsFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 7 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 7) * Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("5-1-5"))).ToString("0.00");

                    break;
                case "6"://分运行李
                    //操作费
                    //OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("6-1-1"))).ToString("0.00");
                    //提货费
                    //PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("6-1-2"))).ToString("0.00");
                    //仓储费
                    KeepGoodsFee_ForSetting = ((DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 7 <= 0 ? 0 : DateTimeHelper.DateDiff(EnumDateCompare.day, Convert.ToDateTime(ViewData["InStoreDate_ForSetting"].ToString()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) - 7) * Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("6-1-5"))).ToString("0.00");

                    break;
                default:
                    break;
            }

            dsWayBillWeight = new T_WayBillWeight().GetWayBillWeightInfo(wbID);
            if (dsWayBillWeight != null)
            {
                dtWayBillWeight = dsWayBillWeight.Tables[0];
                if (dtWayBillWeight != null && dtWayBillWeight.Rows.Count > 0)
                {
                    OperateFee_ForSetting = dtWayBillWeight.Rows[0]["OperateFee_ForSetting"].ToString();
                    PickGoodsFee_ForSetting = dtWayBillWeight.Rows[0]["PickGoodsFee_ForSetting"].ToString();
                    ShiftGoodsFee_ForSetting = dtWayBillWeight.Rows[0]["ShiftGoodsFee_ForSetting"].ToString();
                    ViewData["hid_ddlReceiptMode_ForSetting"] = dtWayBillWeight.Rows[0]["ddlReceiptMode_ForSetting"].ToString();
                    ViewData["CollectionFee_ForSetting"] = dtWayBillWeight.Rows[0]["CollectionFee_ForSetting"].ToString();
                    ViewData["hid_ddlPayMode_ForSetting"] = dtWayBillWeight.Rows[0]["ddlPayMode_ForSetting"].ToString();
                    ViewData["hid_Receipt_ForSetting"] = dtWayBillWeight.Rows[0]["Receipt_ForSetting"].ToString();
                    ViewData["ShouldPayUnit_ForSetting"] = dtWayBillWeight.Rows[0]["ShouldPayUnit_ForSetting"].ToString();
                    ViewData["shouldPay_ForSetting"] = dtWayBillWeight.Rows[0]["shouldPay_ForSetting"].ToString();
                    ViewData["wbCompany_ForSetting"] = dtWayBillWeight.Rows[0]["wbCompany_ForSetting"].ToString();
                    ViewData["ReportSystem_ForSetting"] = dtWayBillWeight.Rows[0]["ReportSystem_ForSetting"].ToString();
                    ViewData["QuarantineCheckFee_ForSetting"] = dtWayBillWeight.Rows[0]["QuarantineCheckFee_ForSetting"].ToString();
                    ViewData["QuarantinePacketFee_ForSetting"] = dtWayBillWeight.Rows[0]["QuarantinePacketFee_ForSetting"].ToString();
                }
            }

            ViewData["OperateFee_ForSetting"] = OperateFee_ForSetting;
            ViewData["PickGoodsFee_ForSetting"] = PickGoodsFee_ForSetting;
            ViewData["KeepGoodsFee_ForSetting"] = KeepGoodsFee_ForSetting;
            ViewData["ShiftGoodsFee_ForSetting"] = ShiftGoodsFee_ForSetting;

            strRet = "{\"result\":\"" + resulr + "\",\"message\":\"" + message + "\",\"row\":[{\"swbTotalNumber_ForPrint\":\"" + swbTotalNumber_ForPrint + "\",\"UnReleaseNum_ForSetting\":\"" + UnReleaseNum_ForSetting + "\",\"ReleaseNum_ForSetting\":\"" + ReleaseNum_ForSetting + "\",\"wbActualWeight_ForPrint\":\"" + wbActualWeight_ForPrint + "\",\"OperateFee_ForSetting\":\"" + OperateFee_ForSetting + "\",\"CustomCategory_ForSetting\":\"" + CustomCategory_ForSetting + "\",\"hid_CustomCategory_ForSetting\":\"" + hid_CustomCategory_ForSetting + "\",\"PickGoodsFee_ForSetting\":\"" + PickGoodsFee_ForSetting + "\",\"KeepGoodsFee_ForSetting\":\"" + KeepGoodsFee_ForSetting + "\",\"ShiftGoodsFee_ForSetting\":\"" + ShiftGoodsFee_ForSetting + "\"}]}";
            return strRet;
        }

        /// <summary>
        /// 打印提货单
        /// </summary>
        /// <param name="strCurrentReleaseSubWayBill">本次放行的分运单信息(单号1,单号2……)</param>
        /// <param name="strWBID"></param>
        /// <param name="iPrintType">0:打印预览   1:确认打印</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Print(string strWBID, int iPrintType, string wbSerialNum_ForPrint, string FlowNum_ForPrint, string wbSerialNumber_ForPrint, string swbTotalNumber_ForPrint, string ReleaseNum_ForSetting, string UnReleaseNum_ForSetting, string wbActualWeight_ForPrint, string InStoreDate_ForSetting, string PickGoodsDate_ForSetting, string OperateFee_ForSetting, string PickGoodsFee_ForSetting, string KeepGoodsFee_ForSetting, string ShiftGoodsFee_ForSetting, string CollectionFee_ForSetting, string TotalFee_ForSetting, string ddlPayMode_ForSetting, string ReportSystem_ForSetting, string QuarantineCheckFee_ForSetting, string QuarantinePacketFee_ForSetting)
        {
            DataSet dsAllUnReleaseSubWayBill = new DataSet();

            dsAllUnReleaseSubWayBill = SqlServerHelper.Query(string.Format(@"SELECT  swbSerialNum ,
                                                                                swbDescription_CHN ,
                                                                                swbDescription_ENG ,
                                                                                swbNumber ,
                                                                                swbWeight ,
                                                                                CONVERT(NVARCHAR(10), DetainDate, 120) DetainDate
                                                                        FROM    V_WayBill_SubWayBill
                                                                        WHERE   wbID = {0}
                                                                                AND ( ( wbImportType = 0
                                                                                        AND swbNeedCheck = 3
                                                                                      )
                                                                                      OR ( wbImportType = 1
                                                                                           AND swbReleaseFlag <>3
                                                                                         )
                                                                                    )", strWBID));

            string str_wbSerialNum_ForPrint = wbSerialNum_ForPrint == null ? "" : Server.UrlDecode(wbSerialNum_ForPrint);
            string str_FlowNum_ForPrint = FlowNum_ForPrint == null ? "" : Server.UrlDecode(FlowNum_ForPrint);
            string str_wbSerialNumber_ForPrint = wbSerialNumber_ForPrint == null ? "" : Server.UrlDecode(wbSerialNumber_ForPrint);
            string str_swbTotalNumber_ForPrint = swbTotalNumber_ForPrint == null ? "" : Server.UrlDecode(swbTotalNumber_ForPrint);
            string str_ReleaseNum_ForSetting = ReleaseNum_ForSetting == null ? "" : Server.UrlDecode(ReleaseNum_ForSetting);
            string str_UnReleaseNum_ForSetting = UnReleaseNum_ForSetting == null ? "" : Server.UrlDecode(UnReleaseNum_ForSetting);
            string str_wbActualWeight_ForPrint = wbActualWeight_ForPrint == null ? "" : Server.UrlDecode(wbActualWeight_ForPrint);
            string str_InStoreDate_ForSetting = InStoreDate_ForSetting == null ? "" : Server.UrlDecode(InStoreDate_ForSetting);
            string str_PickGoodsDate_ForSetting = PickGoodsDate_ForSetting == null ? "" : Server.UrlDecode(PickGoodsDate_ForSetting);
            string str_OperateFee_ForSetting = OperateFee_ForSetting == null ? "" : Server.UrlDecode(OperateFee_ForSetting);
            string str_PickGoodsFee_ForSetting = PickGoodsFee_ForSetting == null ? "" : Server.UrlDecode(PickGoodsFee_ForSetting);
            string str_KeepGoodsFee_ForSetting = KeepGoodsFee_ForSetting == null ? "" : Server.UrlDecode(KeepGoodsFee_ForSetting);
            string str_ShiftGoodsFee_ForSetting = ShiftGoodsFee_ForSetting == null ? "" : Server.UrlDecode(ShiftGoodsFee_ForSetting);
            string str_CollectionFee_ForSetting = CollectionFee_ForSetting == null ? "" : Server.UrlDecode(CollectionFee_ForSetting);
            string str_TotalFee_ForSetting = TotalFee_ForSetting == null ? "" : Server.UrlDecode(TotalFee_ForSetting);
            string str_ddlPayMode_ForSetting = ddlPayMode_ForSetting == null ? "" : Server.UrlDecode(ddlPayMode_ForSetting);
            string str_ReportSystem_ForSetting = ReportSystem_ForSetting == null ? "" : Server.UrlDecode(ReportSystem_ForSetting);
            string str_QuarantineCheckFee_ForSetting = QuarantineCheckFee_ForSetting == null ? "" : Server.UrlDecode(QuarantineCheckFee_ForSetting);
            string str_QuarantinePacketFee_ForSetting = QuarantinePacketFee_ForSetting == null ? "" : Server.UrlDecode(QuarantinePacketFee_ForSetting);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource dtAllUnReleaseSubWayBill = new ReportDataSource("FirstPickGoodsSheetSetting_DS", dsAllUnReleaseSubWayBill.Tables[0]);

            ReportParameter var_wbSerialNum_ForPrint = new ReportParameter("wbSerialNum_ForPrint", str_wbSerialNum_ForPrint.ToString());
            ReportParameter var_FlowNum_ForPrint = new ReportParameter("FlowNum_ForPrint", str_FlowNum_ForPrint.ToString());
            ReportParameter var_wbSerialNumber_ForPrint = new ReportParameter("wbSerialNumber_ForPrint", str_wbSerialNumber_ForPrint.ToString());
            ReportParameter var_swbTotalNumber_ForPrint = new ReportParameter("swbTotalNumber_ForPrint", str_swbTotalNumber_ForPrint.ToString());
            ReportParameter var_ReleaseNum_ForSetting = new ReportParameter("ReleaseNum_ForSetting", str_ReleaseNum_ForSetting.ToString());
            ReportParameter var_UnReleaseNum_ForSetting = new ReportParameter("UnReleaseNum_ForSetting", str_UnReleaseNum_ForSetting.ToString());
            ReportParameter var_wbActualWeight_ForPrint = new ReportParameter("wbActualWeight_ForPrint", str_wbActualWeight_ForPrint.ToString());
            ReportParameter var_InStoreDate_ForSetting = new ReportParameter("InStoreDate_ForSetting", str_InStoreDate_ForSetting.ToString());
            ReportParameter var_PickGoodsDate_ForSetting = new ReportParameter("PickGoodsDate_ForSetting", str_PickGoodsDate_ForSetting.ToString());
            ReportParameter var_OperateFee_ForSetting = new ReportParameter("OperateFee_ForSetting", str_OperateFee_ForSetting.ToString());
            ReportParameter var_PickGoodsFee_ForSetting = new ReportParameter("PickGoodsFee_ForSetting", str_PickGoodsFee_ForSetting.ToString());
            ReportParameter var_KeepGoodsFee_ForSetting = new ReportParameter("KeepGoodsFee_ForSetting", str_KeepGoodsFee_ForSetting.ToString());
            ReportParameter var_ShiftGoodsFee_ForSetting = new ReportParameter("ShiftGoodsFee_ForSetting", str_ShiftGoodsFee_ForSetting.ToString());
            ReportParameter var_CollectionFee_ForSetting = new ReportParameter("CollectionFee_ForSetting", str_CollectionFee_ForSetting.ToString());
            ReportParameter var_TotalFee_ForSetting = new ReportParameter("TotalFee_ForSetting", str_TotalFee_ForSetting.ToString());
            ReportParameter var_ddlPayMode_ForSetting = new ReportParameter("ddlPayMode_ForSetting", str_ddlPayMode_ForSetting.ToString());
            ReportParameter var_ReportSystem_ForSetting = new ReportParameter("ReportSystem_ForSetting", str_ReportSystem_ForSetting.ToString());
            ReportParameter var_QuarantineCheckFee_ForSetting = new ReportParameter("QuarantineCheckFee_ForSetting", str_QuarantineCheckFee_ForSetting.ToString());
            ReportParameter var_QuarantinePacketFee_ForSetting = new ReportParameter("QuarantinePacketFee_ForSetting", str_QuarantinePacketFee_ForSetting.ToString());

            localReport.SetParameters(new ReportParameter[] { var_wbSerialNum_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_FlowNum_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_wbSerialNumber_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_swbTotalNumber_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_ReleaseNum_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_UnReleaseNum_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_wbActualWeight_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_InStoreDate_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_PickGoodsDate_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_OperateFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_PickGoodsFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_KeepGoodsFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_ShiftGoodsFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_CollectionFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_TotalFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_ddlPayMode_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_ReportSystem_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_QuarantineCheckFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_QuarantinePacketFee_ForSetting });

            localReport.DataSources.Add(dtAllUnReleaseSubWayBill);
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
        public string SendMail_PDF(string strWBID, int iPrintType, string wbSerialNum_ForPrint, string FlowNum_ForPrint, string wbSerialNumber_ForPrint, string swbTotalNumber_ForPrint, string ReleaseNum_ForSetting, string UnReleaseNum_ForSetting, string wbActualWeight_ForPrint, string InStoreDate_ForSetting, string PickGoodsDate_ForSetting, string OperateFee_ForSetting, string PickGoodsFee_ForSetting, string KeepGoodsFee_ForSetting, string ShiftGoodsFee_ForSetting, string CollectionFee_ForSetting, string TotalFee_ForSetting, string ddlPayMode_ForSetting, string ReportSystem_ForSetting, string QuarantineCheckFee_ForSetting, string QuarantinePacketFee_ForSetting)
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
                            if (dsUser.Tables[0].Rows[0]["iSendFirstPickGoodsEmail"].ToString() == "1")
                            {
                                DataSet dsAllUnReleaseSubWayBill = new DataSet();

                                //dsAllUnReleaseSubWayBill = SqlServerHelper.Query(string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),DetainDate,120) DetainDate from  V_WayBill_SubWayBill where wbID={0} and swbNeedCheck=3 ", strWBID));
                                dsAllUnReleaseSubWayBill = SqlServerHelper.Query(string.Format(@"SELECT  swbSerialNum ,
                                                                                swbDescription_CHN ,
                                                                                swbDescription_ENG ,
                                                                                swbNumber ,
                                                                                swbWeight ,
                                                                                CONVERT(NVARCHAR(10), DetainDate, 120) DetainDate
                                                                        FROM    V_WayBill_SubWayBill
                                                                        WHERE   wbID = {0}
                                                                                AND ( ( wbImportType = 0
                                                                                        AND swbNeedCheck = 3
                                                                                      )
                                                                                      OR ( wbImportType = 1
                                                                                           AND swbReleaseFlag<>3
                                                                                         )
                                                                                    )", strWBID));
                                string str_wbSerialNum_ForPrint = wbSerialNum_ForPrint == null ? "" : Server.UrlDecode(wbSerialNum_ForPrint);
                                string str_FlowNum_ForPrint = FlowNum_ForPrint == null ? "" : Server.UrlDecode(FlowNum_ForPrint);
                                string str_wbSerialNumber_ForPrint = wbSerialNumber_ForPrint == null ? "" : Server.UrlDecode(wbSerialNumber_ForPrint);
                                string str_swbTotalNumber_ForPrint = swbTotalNumber_ForPrint == null ? "" : Server.UrlDecode(swbTotalNumber_ForPrint);
                                string str_ReleaseNum_ForSetting = ReleaseNum_ForSetting == null ? "" : Server.UrlDecode(ReleaseNum_ForSetting);
                                string str_UnReleaseNum_ForSetting = UnReleaseNum_ForSetting == null ? "" : Server.UrlDecode(UnReleaseNum_ForSetting);
                                string str_wbActualWeight_ForPrint = wbActualWeight_ForPrint == null ? "" : Server.UrlDecode(wbActualWeight_ForPrint);
                                string str_InStoreDate_ForSetting = InStoreDate_ForSetting == null ? "" : Server.UrlDecode(InStoreDate_ForSetting);
                                string str_PickGoodsDate_ForSetting = PickGoodsDate_ForSetting == null ? "" : Server.UrlDecode(PickGoodsDate_ForSetting);
                                string str_OperateFee_ForSetting = OperateFee_ForSetting == null ? "" : Server.UrlDecode(OperateFee_ForSetting);
                                string str_PickGoodsFee_ForSetting = PickGoodsFee_ForSetting == null ? "" : Server.UrlDecode(PickGoodsFee_ForSetting);
                                string str_KeepGoodsFee_ForSetting = KeepGoodsFee_ForSetting == null ? "" : Server.UrlDecode(KeepGoodsFee_ForSetting);
                                string str_ShiftGoodsFee_ForSetting = ShiftGoodsFee_ForSetting == null ? "" : Server.UrlDecode(ShiftGoodsFee_ForSetting);
                                string str_CollectionFee_ForSetting = CollectionFee_ForSetting == null ? "" : Server.UrlDecode(CollectionFee_ForSetting);
                                string str_TotalFee_ForSetting = TotalFee_ForSetting == null ? "" : Server.UrlDecode(TotalFee_ForSetting);
                                string str_ddlPayMode_ForSetting = ddlPayMode_ForSetting == null ? "" : Server.UrlDecode(ddlPayMode_ForSetting);
                                string str_ReportSystem_ForSetting = ReportSystem_ForSetting == null ? "" : Server.UrlDecode(ReportSystem_ForSetting);
                                string str_QuarantineCheckFee_ForSetting = QuarantineCheckFee_ForSetting == null ? "" : Server.UrlDecode(QuarantineCheckFee_ForSetting);
                                string str_QuarantinePacketFee_ForSetting = QuarantinePacketFee_ForSetting == null ? "" : Server.UrlDecode(QuarantinePacketFee_ForSetting);

                                LocalReport localReport = new LocalReport();
                                localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
                                ReportDataSource dtAllUnReleaseSubWayBill = new ReportDataSource("FirstPickGoodsSheetSetting_DS", dsAllUnReleaseSubWayBill.Tables[0]);

                                ReportParameter var_wbSerialNum_ForPrint = new ReportParameter("wbSerialNum_ForPrint", str_wbSerialNum_ForPrint.ToString());
                                ReportParameter var_FlowNum_ForPrint = new ReportParameter("FlowNum_ForPrint", str_FlowNum_ForPrint.ToString());
                                ReportParameter var_wbSerialNumber_ForPrint = new ReportParameter("wbSerialNumber_ForPrint", str_wbSerialNumber_ForPrint.ToString());
                                ReportParameter var_swbTotalNumber_ForPrint = new ReportParameter("swbTotalNumber_ForPrint", str_swbTotalNumber_ForPrint.ToString());
                                ReportParameter var_ReleaseNum_ForSetting = new ReportParameter("ReleaseNum_ForSetting", str_ReleaseNum_ForSetting.ToString());
                                ReportParameter var_UnReleaseNum_ForSetting = new ReportParameter("UnReleaseNum_ForSetting", str_UnReleaseNum_ForSetting.ToString());
                                ReportParameter var_wbActualWeight_ForPrint = new ReportParameter("wbActualWeight_ForPrint", str_wbActualWeight_ForPrint.ToString());
                                ReportParameter var_InStoreDate_ForSetting = new ReportParameter("InStoreDate_ForSetting", str_InStoreDate_ForSetting.ToString());
                                ReportParameter var_PickGoodsDate_ForSetting = new ReportParameter("PickGoodsDate_ForSetting", str_PickGoodsDate_ForSetting.ToString());
                                ReportParameter var_OperateFee_ForSetting = new ReportParameter("OperateFee_ForSetting", str_OperateFee_ForSetting.ToString());
                                ReportParameter var_PickGoodsFee_ForSetting = new ReportParameter("PickGoodsFee_ForSetting", str_PickGoodsFee_ForSetting.ToString());
                                ReportParameter var_KeepGoodsFee_ForSetting = new ReportParameter("KeepGoodsFee_ForSetting", str_KeepGoodsFee_ForSetting.ToString());
                                ReportParameter var_ShiftGoodsFee_ForSetting = new ReportParameter("ShiftGoodsFee_ForSetting", str_ShiftGoodsFee_ForSetting.ToString());
                                ReportParameter var_CollectionFee_ForSetting = new ReportParameter("CollectionFee_ForSetting", str_CollectionFee_ForSetting.ToString());
                                ReportParameter var_TotalFee_ForSetting = new ReportParameter("TotalFee_ForSetting", str_TotalFee_ForSetting.ToString());
                                ReportParameter var_ddlPayMode_ForSetting = new ReportParameter("ddlPayMode_ForSetting", str_ddlPayMode_ForSetting.ToString());
                                ReportParameter var_ReportSystem_ForSetting = new ReportParameter("ReportSystem_ForSetting", str_ReportSystem_ForSetting.ToString());
                                ReportParameter var_QuarantineCheckFee_ForSetting = new ReportParameter("QuarantineCheckFee_ForSetting", str_QuarantineCheckFee_ForSetting.ToString());
                                ReportParameter var_QuarantinePacketFee_ForSetting = new ReportParameter("QuarantinePacketFee_ForSetting", str_QuarantinePacketFee_ForSetting.ToString());

                                localReport.SetParameters(new ReportParameter[] { var_wbSerialNum_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_FlowNum_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_wbSerialNumber_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_swbTotalNumber_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_ReleaseNum_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_UnReleaseNum_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_wbActualWeight_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_InStoreDate_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_PickGoodsDate_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_OperateFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_PickGoodsFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_KeepGoodsFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_ShiftGoodsFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_CollectionFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_TotalFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_ddlPayMode_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_ReportSystem_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_QuarantineCheckFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_QuarantinePacketFee_ForSetting });

                                localReport.DataSources.Add(dtAllUnReleaseSubWayBill);
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
                                            string FileName = Server.MapPath("~/Temp/PDF/") + "快件提货单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
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
        /// 导出扣货单
        /// </summary>
        /// <param name="strCurrentReleaseSubWayBill">本次放行的分运单信息(单号1,单号2……)</param>
        /// <param name="strWBID"></param>
        /// <param name="iPrintType">0:导出预览   1:确认导出</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Excel(string strWBID, int iPrintType, string wbSerialNum_ForPrint, string FlowNum_ForPrint, string wbSerialNumber_ForPrint, string swbTotalNumber_ForPrint, string ReleaseNum_ForSetting, string UnReleaseNum_ForSetting, string wbActualWeight_ForPrint, string InStoreDate_ForSetting, string PickGoodsDate_ForSetting, string OperateFee_ForSetting, string PickGoodsFee_ForSetting, string KeepGoodsFee_ForSetting, string ShiftGoodsFee_ForSetting, string CollectionFee_ForSetting, string TotalFee_ForSetting, string ddlPayMode_ForSetting,string ReportSystem_ForSetting, string QuarantineCheckFee_ForSetting, string QuarantinePacketFee_ForSetting, string browserType)
        {
            DataSet dsAllUnReleaseSubWayBill = new DataSet();

            //dsAllUnReleaseSubWayBill = SqlServerHelper.Query(string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),DetainDate,120) DetainDate from  V_WayBill_SubWayBill where wbID={0} and swbNeedCheck=3 ", strWBID));
            dsAllUnReleaseSubWayBill = SqlServerHelper.Query(string.Format(@"SELECT  swbSerialNum ,
                                                                                swbDescription_CHN ,
                                                                                swbDescription_ENG ,
                                                                                swbNumber ,
                                                                                swbWeight ,
                                                                                CONVERT(NVARCHAR(10), DetainDate, 120) DetainDate
                                                                        FROM    V_WayBill_SubWayBill
                                                                        WHERE   wbID = {0}
                                                                                AND ( ( wbImportType = 0
                                                                                        AND swbNeedCheck = 3
                                                                                      )
                                                                                      OR ( wbImportType = 1
                                                                                           AND swbReleaseFlag <>3
                                                                                         )
                                                                                    )", strWBID));


            string str_wbSerialNum_ForPrint = wbSerialNum_ForPrint == null ? "" : Server.UrlDecode(wbSerialNum_ForPrint);
            string str_FlowNum_ForPrint = FlowNum_ForPrint == null ? "" : Server.UrlDecode(FlowNum_ForPrint);
            string str_wbSerialNumber_ForPrint = wbSerialNumber_ForPrint == null ? "" : Server.UrlDecode(wbSerialNumber_ForPrint);
            string str_swbTotalNumber_ForPrint = swbTotalNumber_ForPrint == null ? "" : Server.UrlDecode(swbTotalNumber_ForPrint);
            string str_ReleaseNum_ForSetting = ReleaseNum_ForSetting == null ? "" : Server.UrlDecode(ReleaseNum_ForSetting);
            string str_UnReleaseNum_ForSetting = UnReleaseNum_ForSetting == null ? "" : Server.UrlDecode(UnReleaseNum_ForSetting);
            string str_wbActualWeight_ForPrint = wbActualWeight_ForPrint == null ? "" : Server.UrlDecode(wbActualWeight_ForPrint);
            string str_InStoreDate_ForSetting = InStoreDate_ForSetting == null ? "" : Server.UrlDecode(InStoreDate_ForSetting);
            string str_PickGoodsDate_ForSetting = PickGoodsDate_ForSetting == null ? "" : Server.UrlDecode(PickGoodsDate_ForSetting);
            string str_OperateFee_ForSetting = OperateFee_ForSetting == null ? "" : Server.UrlDecode(OperateFee_ForSetting);
            string str_PickGoodsFee_ForSetting = PickGoodsFee_ForSetting == null ? "" : Server.UrlDecode(PickGoodsFee_ForSetting);
            string str_KeepGoodsFee_ForSetting = KeepGoodsFee_ForSetting == null ? "" : Server.UrlDecode(KeepGoodsFee_ForSetting);
            string str_ShiftGoodsFee_ForSetting = ShiftGoodsFee_ForSetting == null ? "" : Server.UrlDecode(ShiftGoodsFee_ForSetting);
            string str_CollectionFee_ForSetting = CollectionFee_ForSetting == null ? "" : Server.UrlDecode(CollectionFee_ForSetting);
            string str_TotalFee_ForSetting = TotalFee_ForSetting == null ? "" : Server.UrlDecode(TotalFee_ForSetting);
            string str_ddlPayMode_ForSetting = ddlPayMode_ForSetting == null ? "" : Server.UrlDecode(ddlPayMode_ForSetting);
            string str_ReportSystem_ForSetting = ReportSystem_ForSetting == null ? "" : Server.UrlDecode(ReportSystem_ForSetting);
            string str_QuarantineCheckFee_ForSetting = QuarantineCheckFee_ForSetting == null ? "" : Server.UrlDecode(QuarantineCheckFee_ForSetting);
            string str_QuarantinePacketFee_ForSetting = QuarantinePacketFee_ForSetting == null ? "" : Server.UrlDecode(QuarantinePacketFee_ForSetting);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource dtAllUnReleaseSubWayBill = new ReportDataSource("FirstPickGoodsSheetSetting_DS", dsAllUnReleaseSubWayBill.Tables[0]);

            ReportParameter var_wbSerialNum_ForPrint = new ReportParameter("wbSerialNum_ForPrint", str_wbSerialNum_ForPrint.ToString());
            ReportParameter var_FlowNum_ForPrint = new ReportParameter("FlowNum_ForPrint", str_FlowNum_ForPrint.ToString());
            ReportParameter var_wbSerialNumber_ForPrint = new ReportParameter("wbSerialNumber_ForPrint", str_wbSerialNumber_ForPrint.ToString());
            ReportParameter var_swbTotalNumber_ForPrint = new ReportParameter("swbTotalNumber_ForPrint", str_swbTotalNumber_ForPrint.ToString());
            ReportParameter var_ReleaseNum_ForSetting = new ReportParameter("ReleaseNum_ForSetting", str_ReleaseNum_ForSetting.ToString());
            ReportParameter var_UnReleaseNum_ForSetting = new ReportParameter("UnReleaseNum_ForSetting", str_UnReleaseNum_ForSetting.ToString());
            ReportParameter var_wbActualWeight_ForPrint = new ReportParameter("wbActualWeight_ForPrint", str_wbActualWeight_ForPrint.ToString());
            ReportParameter var_InStoreDate_ForSetting = new ReportParameter("InStoreDate_ForSetting", str_InStoreDate_ForSetting.ToString());
            ReportParameter var_PickGoodsDate_ForSetting = new ReportParameter("PickGoodsDate_ForSetting", str_PickGoodsDate_ForSetting.ToString());
            ReportParameter var_OperateFee_ForSetting = new ReportParameter("OperateFee_ForSetting", str_OperateFee_ForSetting.ToString());
            ReportParameter var_PickGoodsFee_ForSetting = new ReportParameter("PickGoodsFee_ForSetting", str_PickGoodsFee_ForSetting.ToString());
            ReportParameter var_KeepGoodsFee_ForSetting = new ReportParameter("KeepGoodsFee_ForSetting", str_KeepGoodsFee_ForSetting.ToString());
            ReportParameter var_ShiftGoodsFee_ForSetting = new ReportParameter("ShiftGoodsFee_ForSetting", str_ShiftGoodsFee_ForSetting.ToString());
            ReportParameter var_CollectionFee_ForSetting = new ReportParameter("CollectionFee_ForSetting", str_CollectionFee_ForSetting.ToString());
            ReportParameter var_TotalFee_ForSetting = new ReportParameter("TotalFee_ForSetting", str_TotalFee_ForSetting.ToString());
            ReportParameter var_ddlPayMode_ForSetting = new ReportParameter("ddlPayMode_ForSetting", str_ddlPayMode_ForSetting.ToString());
            ReportParameter var_ReportSystem_ForSetting = new ReportParameter("ReportSystem_ForSetting", str_ReportSystem_ForSetting.ToString());
            ReportParameter var_QuarantineCheckFee_ForSetting = new ReportParameter("QuarantineCheckFee_ForSetting", str_QuarantineCheckFee_ForSetting.ToString());
            ReportParameter var_QuarantinePacketFee_ForSetting = new ReportParameter("QuarantinePacketFee_ForSetting", str_QuarantinePacketFee_ForSetting.ToString());

            localReport.SetParameters(new ReportParameter[] { var_wbSerialNum_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_FlowNum_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_wbSerialNumber_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_swbTotalNumber_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_ReleaseNum_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_UnReleaseNum_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_wbActualWeight_ForPrint });
            localReport.SetParameters(new ReportParameter[] { var_InStoreDate_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_PickGoodsDate_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_OperateFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_PickGoodsFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_KeepGoodsFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_ShiftGoodsFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_CollectionFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_TotalFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_ddlPayMode_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_ReportSystem_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_QuarantineCheckFee_ForSetting });
            localReport.SetParameters(new ReportParameter[] { var_QuarantinePacketFee_ForSetting });

            localReport.DataSources.Add(dtAllUnReleaseSubWayBill);
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

            string strOutputFileName = "快件提货单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
        public string SendEmail_Excel(string strWBID, int iPrintType, string wbSerialNum_ForPrint, string FlowNum_ForPrint, string wbSerialNumber_ForPrint, string swbTotalNumber_ForPrint, string ReleaseNum_ForSetting, string UnReleaseNum_ForSetting, string wbActualWeight_ForPrint, string InStoreDate_ForSetting, string PickGoodsDate_ForSetting, string OperateFee_ForSetting, string PickGoodsFee_ForSetting, string KeepGoodsFee_ForSetting, string ShiftGoodsFee_ForSetting, string CollectionFee_ForSetting, string TotalFee_ForSetting, string ddlPayMode_ForSetting,string ReportSystem_ForSetting, string QuarantineCheckFee_ForSetting, string QuarantinePacketFee_ForSetting, string browserType)
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
                            if (dsUser.Tables[0].Rows[0]["iSendFirstPickGoodsEmail"].ToString() == "1")
                            {
                                DataSet dsAllUnReleaseSubWayBill = new DataSet();

                                //dsAllUnReleaseSubWayBill = SqlServerHelper.Query(string.Format(" select swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,convert(nvarchar(10),DetainDate,120) DetainDate from  V_WayBill_SubWayBill where wbID={0} and swbNeedCheck=3 ", strWBID));
                                dsAllUnReleaseSubWayBill = SqlServerHelper.Query(string.Format(@"SELECT  swbSerialNum ,
                                                                                swbDescription_CHN ,
                                                                                swbDescription_ENG ,
                                                                                swbNumber ,
                                                                                swbWeight ,
                                                                                CONVERT(NVARCHAR(10), DetainDate, 120) DetainDate
                                                                        FROM    V_WayBill_SubWayBill
                                                                        WHERE   wbID = {0}
                                                                                AND ( ( wbImportType = 0
                                                                                        AND swbNeedCheck = 3
                                                                                      )
                                                                                      OR ( wbImportType = 1
                                                                                           AND swbReleaseFlag <>3
                                                                                         )
                                                                                    )", strWBID));
                                string str_wbSerialNum_ForPrint = wbSerialNum_ForPrint == null ? "" : Server.UrlDecode(wbSerialNum_ForPrint);
                                string str_FlowNum_ForPrint = FlowNum_ForPrint == null ? "" : Server.UrlDecode(FlowNum_ForPrint);
                                string str_wbSerialNumber_ForPrint = wbSerialNumber_ForPrint == null ? "" : Server.UrlDecode(wbSerialNumber_ForPrint);
                                string str_swbTotalNumber_ForPrint = swbTotalNumber_ForPrint == null ? "" : Server.UrlDecode(swbTotalNumber_ForPrint);
                                string str_ReleaseNum_ForSetting = ReleaseNum_ForSetting == null ? "" : Server.UrlDecode(ReleaseNum_ForSetting);
                                string str_UnReleaseNum_ForSetting = UnReleaseNum_ForSetting == null ? "" : Server.UrlDecode(UnReleaseNum_ForSetting);
                                string str_wbActualWeight_ForPrint = wbActualWeight_ForPrint == null ? "" : Server.UrlDecode(wbActualWeight_ForPrint);
                                string str_InStoreDate_ForSetting = InStoreDate_ForSetting == null ? "" : Server.UrlDecode(InStoreDate_ForSetting);
                                string str_PickGoodsDate_ForSetting = PickGoodsDate_ForSetting == null ? "" : Server.UrlDecode(PickGoodsDate_ForSetting);
                                string str_OperateFee_ForSetting = OperateFee_ForSetting == null ? "" : Server.UrlDecode(OperateFee_ForSetting);
                                string str_PickGoodsFee_ForSetting = PickGoodsFee_ForSetting == null ? "" : Server.UrlDecode(PickGoodsFee_ForSetting);
                                string str_KeepGoodsFee_ForSetting = KeepGoodsFee_ForSetting == null ? "" : Server.UrlDecode(KeepGoodsFee_ForSetting);
                                string str_ShiftGoodsFee_ForSetting = ShiftGoodsFee_ForSetting == null ? "" : Server.UrlDecode(ShiftGoodsFee_ForSetting);
                                string str_CollectionFee_ForSetting = CollectionFee_ForSetting == null ? "" : Server.UrlDecode(CollectionFee_ForSetting);
                                string str_TotalFee_ForSetting = TotalFee_ForSetting == null ? "" : Server.UrlDecode(TotalFee_ForSetting);
                                string str_ddlPayMode_ForSetting = ddlPayMode_ForSetting == null ? "" : Server.UrlDecode(ddlPayMode_ForSetting);
                                string str_ReportSystem_ForSetting = ReportSystem_ForSetting == null ? "" : Server.UrlDecode(ReportSystem_ForSetting);
                                string str_QuarantineCheckFee_ForSetting = QuarantineCheckFee_ForSetting == null ? "" : Server.UrlDecode(QuarantineCheckFee_ForSetting);
                                string str_QuarantinePacketFee_ForSetting = QuarantinePacketFee_ForSetting == null ? "" : Server.UrlDecode(QuarantinePacketFee_ForSetting);

                                LocalReport localReport = new LocalReport();
                                localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
                                ReportDataSource dtAllUnReleaseSubWayBill = new ReportDataSource("FirstPickGoodsSheetSetting_DS", dsAllUnReleaseSubWayBill.Tables[0]);

                                ReportParameter var_wbSerialNum_ForPrint = new ReportParameter("wbSerialNum_ForPrint", str_wbSerialNum_ForPrint.ToString());
                                ReportParameter var_FlowNum_ForPrint = new ReportParameter("FlowNum_ForPrint", str_FlowNum_ForPrint.ToString());
                                ReportParameter var_wbSerialNumber_ForPrint = new ReportParameter("wbSerialNumber_ForPrint", str_wbSerialNumber_ForPrint.ToString());
                                ReportParameter var_swbTotalNumber_ForPrint = new ReportParameter("swbTotalNumber_ForPrint", str_swbTotalNumber_ForPrint.ToString());
                                ReportParameter var_ReleaseNum_ForSetting = new ReportParameter("ReleaseNum_ForSetting", str_ReleaseNum_ForSetting.ToString());
                                ReportParameter var_UnReleaseNum_ForSetting = new ReportParameter("UnReleaseNum_ForSetting", str_UnReleaseNum_ForSetting.ToString());
                                ReportParameter var_wbActualWeight_ForPrint = new ReportParameter("wbActualWeight_ForPrint", str_wbActualWeight_ForPrint.ToString());
                                ReportParameter var_InStoreDate_ForSetting = new ReportParameter("InStoreDate_ForSetting", str_InStoreDate_ForSetting.ToString());
                                ReportParameter var_PickGoodsDate_ForSetting = new ReportParameter("PickGoodsDate_ForSetting", str_PickGoodsDate_ForSetting.ToString());
                                ReportParameter var_OperateFee_ForSetting = new ReportParameter("OperateFee_ForSetting", str_OperateFee_ForSetting.ToString());
                                ReportParameter var_PickGoodsFee_ForSetting = new ReportParameter("PickGoodsFee_ForSetting", str_PickGoodsFee_ForSetting.ToString());
                                ReportParameter var_KeepGoodsFee_ForSetting = new ReportParameter("KeepGoodsFee_ForSetting", str_KeepGoodsFee_ForSetting.ToString());
                                ReportParameter var_ShiftGoodsFee_ForSetting = new ReportParameter("ShiftGoodsFee_ForSetting", str_ShiftGoodsFee_ForSetting.ToString());
                                ReportParameter var_CollectionFee_ForSetting = new ReportParameter("CollectionFee_ForSetting", str_CollectionFee_ForSetting.ToString());
                                ReportParameter var_TotalFee_ForSetting = new ReportParameter("TotalFee_ForSetting", str_TotalFee_ForSetting.ToString());
                                ReportParameter var_ddlPayMode_ForSetting = new ReportParameter("ddlPayMode_ForSetting", str_ddlPayMode_ForSetting.ToString());
                                ReportParameter var_ReportSystem_ForSetting = new ReportParameter("ReportSystem_ForSetting", str_ReportSystem_ForSetting.ToString());
                                ReportParameter var_QuarantineCheckFee_ForSetting = new ReportParameter("QuarantineCheckFee_ForSetting", str_QuarantineCheckFee_ForSetting.ToString());
                                ReportParameter var_QuarantinePacketFee_ForSetting = new ReportParameter("QuarantinePacketFee_ForSetting", str_QuarantinePacketFee_ForSetting.ToString());

                                localReport.SetParameters(new ReportParameter[] { var_wbSerialNum_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_FlowNum_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_wbSerialNumber_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_swbTotalNumber_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_ReleaseNum_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_UnReleaseNum_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_wbActualWeight_ForPrint });
                                localReport.SetParameters(new ReportParameter[] { var_InStoreDate_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_PickGoodsDate_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_OperateFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_PickGoodsFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_KeepGoodsFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_ShiftGoodsFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_CollectionFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_TotalFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_ddlPayMode_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_ReportSystem_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_QuarantineCheckFee_ForSetting });
                                localReport.SetParameters(new ReportParameter[] { var_QuarantinePacketFee_ForSetting });

                                localReport.DataSources.Add(dtAllUnReleaseSubWayBill);
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

                                string strOutputFileName = "快件提货单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
        public string SaveSaleInfo(string FlowNum_ForPrint, string hid_CustomCategory_ForSetting, string wbID_ForPrint, string InStoreDate_ForSetting, string PickGoodsDate_ForSetting, string wbActualWeight_ForPrint, string OperateFee_ForSetting, string PickGoodsFee_ForSetting, string KeepGoodsFee_ForSetting, string ShiftGoodsFee_ForSetting, string CollectionFee_ForSetting, string ddlPayMode_ForSetting, string ShouldPayUnit_ForSetting, string shouldPay_ForSetting, string TotalFee_ForSetting, string ddlReceiptMode_ForSetting, string Receipt_ForSetting)
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
                    RejectGoodsFee = "0.00",
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
                        if (dsUser.Tables[0].Rows[0]["iSendFirstPickGoodsEmail"].ToString() == "1")
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

                                    strSubJect = new T_EmailManagement().GetEmailContent(EmailType.EmailSubject_FirstPickGood).Replace("[Date]", DateTime.Now.ToString("yyyyMMddHHmmss")).Replace("【Date]", DateTime.Now.ToString("yyyyMMddHHmmss")).Replace("[Date】", DateTime.Now.ToString("yyyyMMddHHmmss")).Replace("【Date】", DateTime.Now.ToString("yyyyMMddHHmmss"));
                                    strBody = new T_EmailManagement().GetEmailContent(EmailType.EmailBody_FirstPickGood);

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

        [HttpPost]
        public string CumputeFeeForTCS(string strActualWeight)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"计算失败，原因未知\"}";
            string strOperateFee = "0.00";
            string strPickGoodsFee = "0.00";
            string strShouldPayUnit = "0.00";
            string strShouldPayValue = "0.00";
            try
            {
                strOperateFee = (Convert.ToDouble(new T_FeeRate().GetFeeRateValue("97-1")) * Convert.ToDouble(strActualWeight)).ToString("0.00");
                strPickGoodsFee = (Convert.ToDouble(new T_FeeRate().GetFeeRateValue("97-2")) * Convert.ToDouble(strActualWeight)).ToString("0.00");
                strShouldPayUnit = new T_FeeRate().GetFeeRateValue("97-3");
                strShouldPayValue = (Convert.ToDouble(new T_FeeRate().GetFeeRateValue("97-3")) * Convert.ToDouble(strActualWeight)).ToString("0.00");
                strRet = "{\"result\":\"ok\",\"message\":\"计算成功\",\"data\":[{\"strOperateFee\":\"" + strOperateFee + "\",\"strPickGoodsFee\":\"" + strPickGoodsFee + "\",\"strShouldPayUnit\":\"" + strShouldPayUnit + "\",\"strShouldPayValue\":\"" + strShouldPayValue + "\"}]}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"计算失败，原因:" + ex.Message + "\"}";
            }

            return strRet;
        }
    }
}
