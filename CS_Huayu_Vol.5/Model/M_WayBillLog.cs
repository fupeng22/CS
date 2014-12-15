using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class M_WayBillLog
    {
        public M_WayBillLog()
        {
        }

        #region 属性
        public int WblID
        {
            get;
            set;
        }

        public string Wbl_wbSerialNum
        {
            get;
            set;
        }

        public string Wbl_swbSerialNum
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

        public string Operator
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
