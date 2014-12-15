using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class M_WayBillWeight
    {
        public Int32 wbwId
        {
            get;
            set;
        }

        public Int32 wbw_wbID
        {
            get;
            set;
        }

        public double ActualWeight
        {
            get;
            set;
        }

        public string ddlReceiptMode_ForSetting
        {
            get;
            set;
        }

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

        public string ShiftGoodsFee_ForSetting
        {
            get;
            set;
        }

        public string CollectionFee_ForSetting
        {
            get;
            set;
        }

        public string ddlPayMode_ForSetting
        {
            get;
            set;
        }

        public string ShouldPayUnit_ForSetting
        {
            get;
            set;
        }

        public string shouldPay_ForSetting
        {
            get;
            set;
        }

        public string wbCompany_ForSetting
        {
            get;
            set;
        }

        public string Receipt_ForSetting
        {
            get;
            set;
        }

        public string ReportSystem_ForSetting
        {
            get;
            set;
        }

        public string QuarantineCheckFee_ForSetting
        {
            get;
            set;
        }

        public string QuarantinePacketFee_ForSetting
        {
            get;
            set;
        }
    }
}
