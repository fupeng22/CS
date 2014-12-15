using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Util
{
    public class CommonHelper
    {
        public static string ParseSwbNeedCheck(string swbNeedCheck)
        {
            string strRet = "";
            switch (swbNeedCheck)
            {
                case "0":
                    strRet = "放行";
                    break;
                case "1":
                    strRet = "等待预检";
                    break;
                case "2":
                    strRet = "查验放行";
                    break;
                case "3":
                    strRet = "查验扣留";
                    break;
                case "4":
                    strRet = "查验待处理";
                    break;
                case "99":
                    strRet = "退货";
                    break;
                default:
                    strRet = "未知";
                    break;
            }
            return strRet;
        }

        public static string ParseSwbNeedCheck(string swbNeedCheck, string wbImportType, string swbReleaseFlag)
        {
            string strRet = "";
            switch (wbImportType)
            {
                case "0":
                    switch (swbNeedCheck)
                    {
                        case "0":
                            strRet = "放行";
                            break;
                        case "1":
                            strRet = "等待预检";
                            break;
                        case "2":
                            strRet = "查验放行";
                            break;
                        case "3":
                            strRet = "查验扣留";
                            break;
                        case "4":
                            strRet = "查验待处理";
                            break;
                        case "99":
                            strRet = "查验退货";
                            break;
                        default:
                            strRet = "未知";
                            break;
                    }
                    break;
                case "1":
                    switch (swbNeedCheck)
                    {
                        case "99":
                            strRet = "查验退货";
                            break;
                        default:
                            switch (swbReleaseFlag)
                            {
                                case "0":
                                    strRet = "海关检疫都未放行";
                                    break;
                                case "1":
                                    strRet = "海关放行,检疫未放行";
                                    break;
                                case "2":
                                    strRet = "检疫放行,海关未放行";
                                    break;
                                case "3":
                                    strRet = "海关检疫全放行";
                                    break;
                            }
                            break;
                    }
                    break;
                default:
                    strRet = "未知";
                    break;
            }
            return strRet;
        }

        public static string ParseSwbCheckFlag_Custom(string swbCheckFlag_Custom)
        {
            string strRet = "";
            switch (swbCheckFlag_Custom)
            {
                case "0":
                    strRet = "未查验";
                    break;
                case "1":
                    strRet = "通过";
                    break;
                case "2":
                    strRet = "改单重报";
                    break;
                case "3":
                    strRet = "退运";
                    break;
                case "4":
                    strRet = "移交缉私";
                    break;
                case "5":
                    strRet = "移交法规";
                    break;
                case "6":
                    strRet = "收缴";
                    break;
                case "7":
                    strRet = "超期";
                    break;
                case "8":
                    strRet = "放弃";
                    break;
                default:
                    strRet = "未知";
                    break;
            }
            return strRet;
        }

        public static string ParseSwbCheckFlag_Quarantine(string swbCheckFlag_Quarantine)
        {
            string strRet = "";
            switch (swbCheckFlag_Quarantine)
            {
                case "0":
                    strRet = "未查验";
                    break;
                case "1":
                    strRet = "通过";
                    break;
                case "2":
                    strRet = "改单重报";
                    break;
                case "3":
                    strRet = "退运";
                    break;
                case "4":
                    strRet = "留验";
                    break;
                case "5":
                    strRet = "截留销毁";
                    break;
                default:
                    strRet = "未知";
                    break;
            }
            return strRet;
        }

        public static string ParseSwbControlFlag(string swbControlFlag)
        {
            string strRet = "";
            switch (swbControlFlag)
            {
                case "0":
                    strRet = "未布控";
                    break;
                case "1":
                    strRet = "海关布控";
                    break;
                case "2":
                    strRet = "检疫布控";
                    break;
                case "3":
                    strRet = "同时布控";
                    break;
                default:
                    strRet = "未知";
                    break;
            }
            return strRet;
        }

        public static string ParseSwbCustomsCategory(string swbCustomsCategory)
        {
            string strRet = "";
            switch (swbCustomsCategory)
            {
                case "2":
                    strRet = "样品";
                    break;
                case "3":
                    strRet = "KJ-3";
                    break;
                case "4":
                    strRet = "D类";
                    break;
                case "5":
                    strRet = "个人物品";
                    break;
                case "6":
                    strRet = "分运行李";
                    break;
                default:
                    strRet = "未知";
                    break;
            }
            return strRet;
        }

        public static double PerctangleToDecimal(string perc)
        {
            return double.Parse(perc.Replace("%", "")) / 100;
        }

        public static DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone();  // 复制DataRow的表结构
            foreach (DataRow row in rows)
                tmp.Rows.Add(row);  // 将DataRow添加到DataTable中
            return tmp;
        }

        public static string ParseDifferentColor_Main(string strDifferentColode, string strWhereTemp)
        {
            string strRet = "";

            if (strDifferentColode.ToLower().Contains("TaxValueCheck".ToLower()))
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (TaxValueCheck<50) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (TaxValueCheck<50) ";
                }
            }

            if (strDifferentColode.ToLower().Contains("PickGoodsAgain".ToLower()))
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (PickGoodsAgain=1) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (PickGoodsAgain=1) ";
                }
            }

            if (strDifferentColode.ToLower().Contains("IsBlackList".ToLower()))
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (IsBlackList=1) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (IsBlackList=1) ";
                }
            }

            if (strDifferentColode.ToLower().Contains("AboveWayBillLimited".ToLower()))
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (AboveWayBillLimited=1) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (AboveWayBillLimited=1) ";
                }
            }

            strRet = strWhereTemp;

            return strRet;
        }

        public static string ParseDifferentColor_Sub(string strDifferentColode, string strWhereTemp)
        {
            string strRet = "";

            if (strDifferentColode.ToLower().Contains("mismatchCargoName".ToLower()))
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (mismatchCargoName=1) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (mismatchCargoName=1) ";
                }
            }

            if (strDifferentColode.ToLower().Contains("belowFullPrice".ToLower()))
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (belowFullPrice=1) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (belowFullPrice=1) ";
                }
            }

            if (strDifferentColode.ToLower().Contains("above1000".ToLower()))
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + " and (above1000=1) ";
                }
                else
                {
                    strWhereTemp = strWhereTemp + " (above1000=1) ";
                }
            }

            strRet = strWhereTemp;

            return strRet;
        }
    }
}
