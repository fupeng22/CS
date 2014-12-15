using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using System.Data;
using SQLDAL;
using Util;
using CS.Filter;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_ViewFeeForeSettingInfoController : Controller
    {
        //
        // GET: /Huayu_ViewFeeForeSettingInfo/
        [HuayuRequiresLoginAttribute]
        public ActionResult Index()
        {
            M_WayBillWeight m_WayBillWeight = new M_WayBillWeight()
            {
                wbwId = -1,
                wbw_wbID = -1,
                ActualWeight = 0.0,
                ddlReceiptMode_ForSetting = "",
                OperateFee_ForSetting = "error",
                PickGoodsFee_ForSetting = "error",
                ShiftGoodsFee_ForSetting = "0.00",
                CollectionFee_ForSetting = "0.00",
                ddlPayMode_ForSetting = "",
                ShouldPayUnit_ForSetting = "",
                shouldPay_ForSetting = "0.00",
                wbCompany_ForSetting = "",
                Receipt_ForSetting = "",
                ReportSystem_ForSetting = "0.00",
                QuarantineCheckFee_ForSetting = "0.00",
                QuarantinePacketFee_ForSetting = "0.00"
            };

            string strWBID = Request.QueryString["wbID"] == null ? "-1" : Request.QueryString["wbID"].ToString();
            DataSet dsWayBillWeightInfo = null;
            DataTable dtWayBillWeightInfo = null;
            DataSet ds_SubWayBill = null;
            DataTable dt_SubWayBill = null;
            string CustomCategory_ForSetting = "";
            string CustomCategory_ForSetting_Des = "";

            dsWayBillWeightInfo = new T_WayBillWeight().GetWayBillWeightInfo(strWBID);
            if (dsWayBillWeightInfo != null)
            {
                dtWayBillWeightInfo = dsWayBillWeightInfo.Tables[0];
                if (dtWayBillWeightInfo != null && dtWayBillWeightInfo.Rows.Count > 0)
                {
                    if (dtWayBillWeightInfo.Rows[0]["wbwId"] != null)
                    {
                        m_WayBillWeight.wbwId = Convert.ToInt32(dtWayBillWeightInfo.Rows[0]["wbwId"].ToString());
                    }
                    if (dtWayBillWeightInfo.Rows[0]["wbID"] != null)
                    {
                        m_WayBillWeight.wbw_wbID = Convert.ToInt32(dtWayBillWeightInfo.Rows[0]["wbID"].ToString());
                    }
                    if (dtWayBillWeightInfo.Rows[0]["ActualWeight"] != null)
                    {
                        m_WayBillWeight.ActualWeight = Convert.ToDouble(dtWayBillWeightInfo.Rows[0]["ActualWeight"].ToString());
                    }
                    if (dtWayBillWeightInfo.Rows[0]["ddlReceiptMode_ForSetting"] != null)
                    {
                        m_WayBillWeight.ddlReceiptMode_ForSetting = dtWayBillWeightInfo.Rows[0]["ddlReceiptMode_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["OperateFee_ForSetting"] != null)
                    {
                        m_WayBillWeight.OperateFee_ForSetting = dtWayBillWeightInfo.Rows[0]["OperateFee_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["PickGoodsFee_ForSetting"] != null)
                    {
                        m_WayBillWeight.PickGoodsFee_ForSetting = dtWayBillWeightInfo.Rows[0]["PickGoodsFee_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["ShiftGoodsFee_ForSetting"] != null)
                    {
                        m_WayBillWeight.ShiftGoodsFee_ForSetting = dtWayBillWeightInfo.Rows[0]["ShiftGoodsFee_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["CollectionFee_ForSetting"] != null)
                    {
                        m_WayBillWeight.CollectionFee_ForSetting = dtWayBillWeightInfo.Rows[0]["CollectionFee_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["ddlPayMode_ForSetting"] != null)
                    {
                        m_WayBillWeight.ddlPayMode_ForSetting = dtWayBillWeightInfo.Rows[0]["ddlPayMode_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["ShouldPayUnit_ForSetting"] != null)
                    {
                        m_WayBillWeight.ShouldPayUnit_ForSetting = dtWayBillWeightInfo.Rows[0]["ShouldPayUnit_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["shouldPay_ForSetting"] != null)
                    {
                        m_WayBillWeight.shouldPay_ForSetting = dtWayBillWeightInfo.Rows[0]["shouldPay_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["wbCompany_ForSetting"] != null)
                    {
                        m_WayBillWeight.wbCompany_ForSetting = dtWayBillWeightInfo.Rows[0]["wbCompany_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["Receipt_ForSetting"] != null)
                    {
                        m_WayBillWeight.Receipt_ForSetting = dtWayBillWeightInfo.Rows[0]["Receipt_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["ReportSystem_ForSetting"] != null)
                    {
                        m_WayBillWeight.ReportSystem_ForSetting = dtWayBillWeightInfo.Rows[0]["ReportSystem_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["QuarantineCheckFee_ForSetting"] != null)
                    {
                        m_WayBillWeight.QuarantineCheckFee_ForSetting = dtWayBillWeightInfo.Rows[0]["QuarantineCheckFee_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["QuarantinePacketFee_ForSetting"] != null)
                    {
                        m_WayBillWeight.QuarantinePacketFee_ForSetting = dtWayBillWeightInfo.Rows[0]["QuarantinePacketFee_ForSetting"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["wbID"] != null)
                    {
                        ViewData["WBID"] = dtWayBillWeightInfo.Rows[0]["wbID"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["wbSerialNum"] != null)
                    {
                        ViewData["WBSerialNum"] = dtWayBillWeightInfo.Rows[0]["wbSerialNum"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["wbCompany"] != null)
                    {
                        ViewData["WBCompany"] = dtWayBillWeightInfo.Rows[0]["wbCompany"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["wbSRport"] != null)
                    {
                        ViewData["WBSRport"] = dtWayBillWeightInfo.Rows[0]["wbSRport"].ToString();
                    }
                    if (dtWayBillWeightInfo.Rows[0]["wbArrivalDate"] != null)
                    {
                        ViewData["WBArrivalDate"] = Convert.ToDateTime(dtWayBillWeightInfo.Rows[0]["wbArrivalDate"].ToString()).ToString("yyyy-MM-dd");
                    }

                    ds_SubWayBill = new T_SubWayBill().GetSubWayBillInfoBywbID(Convert.ToInt32(ViewData["WBID"].ToString()));
                    if (ds_SubWayBill != null)
                    {
                        dt_SubWayBill = ds_SubWayBill.Tables[0];
                        if (dt_SubWayBill != null && dt_SubWayBill.Rows.Count > 0)
                        {
                            CustomCategory_ForSetting = dt_SubWayBill.Rows[0]["swbCustomsCategory"].ToString();
                            switch (CustomCategory_ForSetting)
                            {
                                case "2":
                                    CustomCategory_ForSetting_Des = "样品";
                                    break;
                                case "3":
                                    CustomCategory_ForSetting_Des = "KJ-3";
                                    break;
                                case "4":
                                    CustomCategory_ForSetting_Des = "D类";
                                    break;
                                case "5":
                                    CustomCategory_ForSetting_Des = "个人物品";
                                    break;
                                case "6":
                                    CustomCategory_ForSetting_Des = "分运行李";
                                    break;
                                default:
                                    break;
                            }
                            ViewData["txtCustomCategory_Des"] = CustomCategory_ForSetting_Des;
                            ViewData["txtCustomCategory"] = CustomCategory_ForSetting;
                        }
                    }
                }
            }

            if (m_WayBillWeight.OperateFee_ForSetting == "error" || m_WayBillWeight.PickGoodsFee_ForSetting == "error" || m_WayBillWeight.OperateFee_ForSetting == "" || m_WayBillWeight.PickGoodsFee_ForSetting == "")
            {
                struct_CumputeInfo m_Struct_CumputeInfo = ComputeSubWayBillInfo(ViewData["txtCustomCategory"].ToString(), m_WayBillWeight.ActualWeight.ToString());
                m_WayBillWeight.OperateFee_ForSetting = m_Struct_CumputeInfo.OperateFee_ForSetting;
                m_WayBillWeight.PickGoodsFee_ForSetting = m_Struct_CumputeInfo.PickGoodsFee_ForSetting;
            }

            return View(m_WayBillWeight);
        }

        public struct_CumputeInfo ComputeSubWayBillInfo(string hid_CustomCategory_ForSetting, string wbActualWeight_ForPrint)
        {
            string OperateFee_ForSetting = "0.00";
            string PickGoodsFee_ForSetting = "0.00";
            struct_CumputeInfo m_Struct_CumputeInfo = null;
            //计算费用
            switch (hid_CustomCategory_ForSetting)
            {
                case "2"://样品
                    //操作费
                    OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                    //提货费
                    PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-2"))).ToString("0.00");

                    break;
                case "3"://KJ-3
                    //操作费
                    OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                    //提货费
                    PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-2"))).ToString("0.00");

                    break;
                case "4"://D类
                    //操作费
                    OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-1"))).ToString("0.00");
                    //提货费
                    PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("2-1-2"))).ToString("0.00");

                    break;
                case "5"://个人物品
                    //操作费
                    OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("5-1-1"))).ToString("0.00");
                    //提货费
                    PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("5-1-2"))).ToString("0.00");

                    break;
                case "6"://分运行李
                    //操作费
                    OperateFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("6-1-1"))).ToString("0.00");
                    //提货费
                    PickGoodsFee_ForSetting = (Convert.ToDouble(wbActualWeight_ForPrint) * Convert.ToDouble(new T_FeeRate().GetFeeRateValue("6-1-2"))).ToString("0.00");

                    break;
                default:
                    break;
            }
            m_Struct_CumputeInfo = new struct_CumputeInfo()
            {
                OperateFee_ForSetting = OperateFee_ForSetting,
                PickGoodsFee_ForSetting = PickGoodsFee_ForSetting
            };
            return m_Struct_CumputeInfo;
        }

        [HttpPost]
        public string updateFeeForeSettingInfo(string wbwId, string ddlReceiptMode_ForSetting, string OperateFee_ForSetting, string PickGoodsFee_ForSetting, string ShiftGoodsFee_ForSetting, string CollectionFee_ForSetting, string ddlPayMode_ForSetting, string ShouldPayUnit_ForSetting, string shouldPay_ForSetting, string wbCompany_ForSetting, string Receipt_ForSetting, string ReportSystem_ForSetting, string QuarantineCheckFee_ForSetting, string QuarantinePacketFee_ForSetting)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"修改失败，原因未知\"}";
            wbwId = Server.UrlDecode(wbwId.ToString());
            ddlReceiptMode_ForSetting = Server.UrlDecode(ddlReceiptMode_ForSetting.ToString());
            OperateFee_ForSetting = Server.UrlDecode(OperateFee_ForSetting.ToString());
            PickGoodsFee_ForSetting = Server.UrlDecode(PickGoodsFee_ForSetting.ToString());
            ShiftGoodsFee_ForSetting = Server.UrlDecode(ShiftGoodsFee_ForSetting.ToString());
            CollectionFee_ForSetting = Server.UrlDecode(CollectionFee_ForSetting.ToString());
            ddlPayMode_ForSetting = Server.UrlDecode(ddlPayMode_ForSetting.ToString());
            ShouldPayUnit_ForSetting = Server.UrlDecode(ShouldPayUnit_ForSetting.ToString());
            shouldPay_ForSetting = Server.UrlDecode(shouldPay_ForSetting.ToString());
            wbCompany_ForSetting = Server.UrlDecode(wbCompany_ForSetting.ToString());
            Receipt_ForSetting = Server.UrlDecode(Receipt_ForSetting.ToString());
            ReportSystem_ForSetting = Server.UrlDecode(ReportSystem_ForSetting.ToString());
            QuarantineCheckFee_ForSetting = Server.UrlDecode(QuarantineCheckFee_ForSetting.ToString());
            QuarantinePacketFee_ForSetting = Server.UrlDecode(QuarantinePacketFee_ForSetting.ToString());
            M_WayBillWeight m_WayBillWeight = new M_WayBillWeight();

            try
            {
                if (wbwId == "")
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "修改失败,修改数据的ID指定有误" + "\"}";
                }
                else
                {
                    m_WayBillWeight.wbwId = Convert.ToInt32(wbwId);
                    m_WayBillWeight.ddlReceiptMode_ForSetting = ddlReceiptMode_ForSetting;
                    m_WayBillWeight.OperateFee_ForSetting = OperateFee_ForSetting;
                    m_WayBillWeight.PickGoodsFee_ForSetting = PickGoodsFee_ForSetting;
                    m_WayBillWeight.ShiftGoodsFee_ForSetting = ShiftGoodsFee_ForSetting;
                    m_WayBillWeight.CollectionFee_ForSetting = CollectionFee_ForSetting;
                    m_WayBillWeight.ddlPayMode_ForSetting = ddlPayMode_ForSetting;
                    m_WayBillWeight.ShouldPayUnit_ForSetting = ShouldPayUnit_ForSetting;
                    m_WayBillWeight.shouldPay_ForSetting = shouldPay_ForSetting;
                    m_WayBillWeight.wbCompany_ForSetting = wbCompany_ForSetting;
                    m_WayBillWeight.Receipt_ForSetting = Receipt_ForSetting;
                    m_WayBillWeight.ReportSystem_ForSetting = ReportSystem_ForSetting;
                    m_WayBillWeight.QuarantineCheckFee_ForSetting = QuarantineCheckFee_ForSetting;
                    m_WayBillWeight.QuarantinePacketFee_ForSetting = QuarantinePacketFee_ForSetting;
                    if (new T_WayBillWeight().UpdateWayBillWeightInfo(m_WayBillWeight))
                    {
                        strRet = "{\"result\":\"ok\",\"message\":\"修改成功\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"修改失败，原因未知\"}";
                    }
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"修改失败,原因:" + ex.Message + "\"}";
            }
            return strRet;
        }

    }

    public class struct_CumputeInfo
    {
        public string OperateFee_ForSetting
        {
            get;
            set;
        }

        public string PickGoodsFee_ForSetting
        {
            get;
            set;
        }
    }
}
