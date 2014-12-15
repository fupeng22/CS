using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SQLDAL
{
    public class T_FeeRate
    {
        /// <summary>
        /// 编辑指定ID费率信息
        /// </summary>
        /// <param name="frID"></param>
        /// <param name="CategoryValue"></param>
        /// <param name="CategoryUnit"></param>
        /// <param name="mMemo"></param>
        /// <returns></returns>
        public bool UpdateFeeSetting(string frID,string CategoryValue,string CategoryUnit,string mMemo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" update FeeRate set CategoryValue='{0}',CategoryUnit='{1}',mMemo='{2}' where frID={3}",CategoryValue,CategoryUnit,mMemo,frID);

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有汇率信息
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllFeeRate()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  * from FeeRate");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        /// <summary>
        /// 根据条件获取费率信息
        /// </summary>
        /// <returns></returns>
        public DataSet GetFeeRateInfo(string CategoryID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  * from FeeRate where CategoryID='" + CategoryID + "'");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        /// <summary>
        /// 根据条件获取费率信息
        /// </summary>
        /// <returns></returns>
        public string GetFeeRateValue(string CategoryID)
        {
            string strRet = "0.00";
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  * from FeeRate where CategoryID='" + CategoryID + "'");
            DataSet ds = null;
            DataTable dt = null;
            ds=DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds!=null)
            {
                dt = ds.Tables[0];
                if (dt!=null && dt.Rows.Count>0)
                {
                    strRet = dt.Rows[0]["CategoryValue"].ToString();
                }
            }
            return strRet;
        }
    }
}
