using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class M_WayBillDailyReport
    {
        public Int32 wbrID
        {
            get;
            set;
        }

        public string wbrCode
        {
            get;
            set;
        }

        public string CustomsCategory
        {
            get;
            set;
        }

        public Int32 wbr_wbID
        {
            get;
            set;
        }

        public string InStoreDate
        {
            get;
            set;
        }

        public string OutStoreDate
        {
            get;
            set;
        }

        public string WayBillActualWeight
        {
            get;
            set;
        }

        public string OperateFee
        {
            get;
            set;
        }

        public string PickGoodsFee
        {
            get;
            set;
        }

        public string KeepGoodsFee
        {
            get;
            set;
        }

        public string ShiftGoodsFee
        {
            get;
            set;
        }

        public string RejectGoodsFee
        {
            get;
            set;
        }
        public string CollectionKeepGoodsFee
        {
            get;
            set;
        }


        public string ActualPay
        {
            get;
            set;
        }

        public string PayMethod
        {
            get;
            set;
        }
        public string Receipt
        {
            get;
            set;
        }

        public string ShouldPayUnit
        {
            get;
            set;
        }

        public string shouldPay
        {
            get;
            set;
        }
        public string ReceptMethod
        {
            get;
            set;
        }

        public string SalesMan
        {
            get;
            set;
        }
        public string mMemo
        {
            get;
            set;
        }
    }
}
