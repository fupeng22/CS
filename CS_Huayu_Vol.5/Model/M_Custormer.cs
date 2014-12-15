using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class M_Custormer
    {
        public M_Custormer()
        {
        }

        #region
        private int _CustormerID;

        public int CustormerID
        {
            get { return _CustormerID; }
            set { _CustormerID = value; }
        }
        private string _CustormerName;

        public string CustormerName
        {
            get { return _CustormerName; }
            set { _CustormerName = value; }
        }
        private int _CategoryID;

        public int CategoryID
        {
            get { return _CategoryID; }
            set { _CategoryID = value; }
        }
        private string _CategoryName;

        public string CategoryName
        {
            get { return _CategoryName; }
            set { _CategoryName = value; }
        }



        #endregion
    }
}
