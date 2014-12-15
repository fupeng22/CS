using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class M_WayBillFlow
    {
        public M_WayBillFlow()
        {
        }

        #region 属性
        public int WbfID
        {
            get;
            set;
        }

        public int Wbf_wbID
        {
            get;
            set;
        }

        public int Wbf_swbID
        {
            get;
            set;
        }

        public string Wbf_swbSerialNum
        {
            get;
            set;
        }

        public int status
        {
            get;
            set;
        }

        public DateTime operateDate
        {
            get;
            set;
        }

        public Int32 IsOutStore
        {
            get;
            set;
        }

        public DateTime OutStoreDate
        {
            get;
            set;
        }

        public int ExceptionStatus
        {
            get;
            set;
        }

        public string OutStoreOperator
        {
            get;
            set;
        }

        public string ExceptionDescription
        {
            get;
            set;
        }

        public string HandleDescription
        {
            get;
            set;
        }

        public string Operator
        {
            get;
            set;
        }

        public string Handler
        {
            get;
            set;
        }

        public DateTime HandleDate
        {
            get;
            set;
        }

        public string mMemo
        {
            get;
            set;
        }

        #endregion
    }
}
