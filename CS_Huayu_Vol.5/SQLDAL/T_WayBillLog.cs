using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace SQLDAL
{
    public class T_WayBillLog
    {
        /// <summary>
        /// 插入日志记录
        /// </summary>
        /// <param name="wbSerialNum"></param>
        /// <param name="swbSerialNum"></param>
        /// <param name="iStatus"></param>
        /// <param name="strOperator"></param>
        /// <returns></returns>
        public bool InsertLog(M_WayBillLog mWayBillLog)
        {
            Boolean bOK = false;
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@" insert into WayBillLog(Wbl_wbSerialNum,Wbl_swbSerialNum,status,operator)
                                                        values('{0}','{1}',{2},'{3}')", mWayBillLog.Wbl_wbSerialNum, mWayBillLog.Wbl_swbSerialNum, mWayBillLog.status, mWayBillLog.Operator);

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
            {
                bOK = true;
            }
            return bOK;
        }

        /// <summary>
        /// 删除日志记录
        /// </summary>
        /// <param name="wbSerialNum"></param>
        /// <param name="swbSerialNum"></param>
        /// <param name="iStatus"></param>
        /// <param name="strOperator"></param>
        /// <returns></returns>
        public bool DeleteLog(string ids)
        {
            Boolean bOK = false;
            StringBuilder strSql = new StringBuilder();
            if (ids!="")
            {
                strSql.AppendFormat(" delete from WayBillLog where WblID in ({0})", ids);

                if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
                {
                    bOK = true;
                }
            }
            
            return bOK;
        }
    }
}
