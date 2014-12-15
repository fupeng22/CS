using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SQLDAL
{
    public class T_RejectSubWayBill
    {
        //获取退运信息
        public DataSet getRejectSubWayBillInfo(string wbID,string beginD,string endD)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from V_Distinct_RejectSubWayBill");
            strSql.Append(" WHERE swb_wbID=" + wbID + "");

            if (beginD!="" && endD!="")
            {
                strSql.AppendFormat(" and (convert(nvarchar(10),RejectDate,120)>='{0}' and convert(nvarchar(10),RejectDate,120)<='{1}') ", Convert.ToDateTime(beginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(endD).ToString("yyyy-MM-dd"));
            }

            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }
    }
}
