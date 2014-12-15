using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class M_WayBill
    {
        public M_WayBill()
        {
        }

        #region 属性
        private int _wbID;

        public int WbID
        {
            get { return _wbID; }
            set { _wbID = value; }
        }
        private string _wbSerialNum;

        public string WbSerialNum
        {
            get { return _wbSerialNum; }
            set { _wbSerialNum = value; }
        }
        private int _wbTotalNumber;

        public int WbTotalNumber
        {
            get { return _wbTotalNumber; }
            set { _wbTotalNumber = value; }
        }
        private double _wbTotalWeight;

        public double WbTotalWeight
        {
            get { return _wbTotalWeight; }
            set { _wbTotalWeight = value; }
        }
        private int _DelFlag;

        public int DelFlag
        {
            get { return _DelFlag; }
            set { _DelFlag = value; }
        }

        private string _wbVoyage;

        public string WbVoyage
        {
            get { return _wbVoyage; }
            set { _wbVoyage = value; }
        }
        private string _wbIOmark;

        public string WbIOmark
        {
            get { return _wbIOmark; }
            set { _wbIOmark = value; }
        }
        private string _wbChinese;

        public string WbChinese
        {
            get { return _wbChinese; }
            set { _wbChinese = value; }
        }
        private string _wbEnglish;

        public string WbEnglish
        {
            get { return _wbEnglish; }
            set { _wbEnglish = value; }
        }
        private int _wbSubNumber;

        public int WbSubNumber
        {
            get { return _wbSubNumber; }
            set { _wbSubNumber = value; }
        }
        private string _wbTransportMode;

        public string WbTransportMode
        {
            get { return _wbTransportMode; }
            set { _wbTransportMode = value; }
        }


        private string _wbEntryDate;

        public string WbEntryDate
        {
            get { return _wbEntryDate; }
            set { _wbEntryDate = value; }
        }


        private string _wbSRPort;

        public string WbSRPort
        {
            get { return _wbSRPort; }
            set { _wbSRPort = value; }
        }
        private string _wbPortCode;

        public string WbPortCode
        {
            get { return _wbPortCode; }
            set { _wbPortCode = value; }
        }
        private string _StorageDate;

        public string StorageDate
        {
            get { return _StorageDate; }
            set { _StorageDate = value; }
        }

        public int wbImportType
        {
            get;
            set;
        }

        #endregion


    }
}
