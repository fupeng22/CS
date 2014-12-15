using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Model;
using System.Data.SqlClient;

namespace SQLDAL
{
    public class T_WayBillFlow_Exception
    {
        public bool Insert(M_WayBillFlow_Exception mWayBillFlow_Exception)
        {
            bool bOK = false;
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append(@"insert into WayBillFlow_Exception([Wbf_wbID]
           ,[Wbf_swbID]
           ,[Wbf_swbSerialNum]
           ,[status]
           ,[operateDate]
           ,[ExceptionStatus]
           ,[ExceptionDescription]
           ,[HandleDescription]
           ,[Operator]
           ,[Handler]
           ,[HandleDate]
           ,[IsOutStore]
           ,[OutStoreDate]
           ,[OutStoreOperator]
           ,[mMemo]) values(@Wbf_wbID,@Wbf_swbID,@Wbf_swbSerialNum,@status,@operateDate,@ExceptionStatus,
           @ExceptionDescription,@HandleDescription,@Operator,@Handler,@HandleDate,@IsOutStore,@OutStoreDate,@OutStoreDate,@mMemo)");

                SqlParameter[] parameters = {
                    new SqlParameter("@Wbf_wbID",SqlDbType.Int),
                    new SqlParameter("@Wbf_swbID",SqlDbType.Int ),
                    new SqlParameter("@Wbf_swbSerialNum", SqlDbType.NVarChar,255),
                    new SqlParameter("@status",SqlDbType.Int),
                    new SqlParameter("@operateDate",SqlDbType.DateTime),
                    new SqlParameter("@ExceptionStatus",SqlDbType.Int),
                    new SqlParameter("@ExceptionDescription",SqlDbType.NVarChar,4000),
                    new SqlParameter("@HandleDescription",SqlDbType.NVarChar,4000),
                    new SqlParameter("@Operator",SqlDbType.NVarChar,255),
                    new SqlParameter("@Handler",SqlDbType.NVarChar,255),
                    new SqlParameter("@HandleDate",SqlDbType.DateTime),
                     new SqlParameter("@IsOutStore",SqlDbType.Int),
                    new SqlParameter("@OutStoreDate",SqlDbType.DateTime),
                     new SqlParameter("@OutStoreOperator",SqlDbType.NVarChar,4000),
                    new SqlParameter("@mMemo",SqlDbType.NVarChar,4000)
            };
                parameters[0].Value = mWayBillFlow_Exception.Wbf_wbID;
                parameters[1].Value = mWayBillFlow_Exception.Wbf_swbID;
                parameters[2].Value = mWayBillFlow_Exception.Wbf_swbSerialNum;
                parameters[3].Value = mWayBillFlow_Exception.status;
                parameters[4].Value = mWayBillFlow_Exception.operateDate;
                parameters[5].Value = mWayBillFlow_Exception.ExceptionStatus;
                parameters[6].Value = mWayBillFlow_Exception.ExceptionDescription;
                parameters[7].Value = mWayBillFlow_Exception.HandleDescription;
                parameters[8].Value = mWayBillFlow_Exception.Operator;
                parameters[9].Value = mWayBillFlow_Exception.Handler;
                parameters[10].Value = mWayBillFlow_Exception.HandleDate;
                parameters[11].Value = mWayBillFlow_Exception.IsOutStore;
                parameters[12].Value = mWayBillFlow_Exception.OutStoreDate;
                parameters[13].Value = mWayBillFlow_Exception.OutStoreOperator;
                parameters[14].Value = mWayBillFlow_Exception.mMemo;


                if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
                {
                    bOK = true;
                }

            }
            catch (Exception ex)
            {
                bOK = false;
            }

            return bOK;
        }

        public bool AddExceptionRecord(string ids)
        {
            bool bOK = false;
            DataSet ds = new T_WayBillFlow().getWayBillFlowWithIds(ids);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        M_WayBillFlow_Exception modelWayBillFlow_Exception = new M_WayBillFlow_Exception();
                        modelWayBillFlow_Exception.Wbf_wbID = Convert.ToInt32(dt.Rows[i]["Wbf_wbID"] == DBNull.Value ? "0" : dt.Rows[i]["Wbf_wbID"].ToString());
                        modelWayBillFlow_Exception.Wbf_swbID = Convert.ToInt32(dt.Rows[i]["Wbf_swbID"] == DBNull.Value ? "0" : dt.Rows[i]["Wbf_swbID"].ToString());
                        modelWayBillFlow_Exception.Wbf_swbSerialNum = dt.Rows[i]["swbSerialNum"] == DBNull.Value ? "-1" : dt.Rows[i]["swbSerialNum"].ToString();
                        modelWayBillFlow_Exception.status = Convert.ToInt32(dt.Rows[i]["status"] == DBNull.Value ? "-1" : dt.Rows[i]["status"].ToString());
                        modelWayBillFlow_Exception.operateDate = Convert.ToDateTime(dt.Rows[i]["operateDate"] == DBNull.Value ? DateTime.Now.ToString("yyyy-MM-dd") : dt.Rows[i]["operateDate"].ToString());
                        modelWayBillFlow_Exception.ExceptionStatus = Convert.ToInt32(dt.Rows[i]["ExceptionStatus"] == DBNull.Value ? "-1" : dt.Rows[i]["ExceptionStatus"].ToString());
                        modelWayBillFlow_Exception.ExceptionDescription = dt.Rows[i]["ExceptionDescription"] == DBNull.Value ? "" : dt.Rows[i]["ExceptionDescription"].ToString();
                        modelWayBillFlow_Exception.HandleDescription = dt.Rows[i]["HandleDescription"] == DBNull.Value ? "" : dt.Rows[i]["HandleDescription"].ToString();
                        modelWayBillFlow_Exception.Operator = dt.Rows[i]["Operator"] == DBNull.Value ? "" : dt.Rows[i]["Operator"].ToString();
                        modelWayBillFlow_Exception.Handler = dt.Rows[i]["Handler"] == DBNull.Value ? "" : dt.Rows[i]["Handler"].ToString();
                        modelWayBillFlow_Exception.IsOutStore = Convert.ToInt32(dt.Rows[i]["IsOutStore"] == DBNull.Value ? "-1" : dt.Rows[i]["IsOutStore"].ToString());
                        modelWayBillFlow_Exception.OutStoreDate = Convert.ToDateTime(dt.Rows[i]["OutStoreDate"].ToString()=="" ? DateTime.Now.ToString("yyyy-MM-dd") : dt.Rows[i]["OutStoreDate"].ToString());
                        modelWayBillFlow_Exception.OutStoreOperator = dt.Rows[i]["OutStoreOperator"] == DBNull.Value ? "" : dt.Rows[i]["OutStoreOperator"].ToString();
                        modelWayBillFlow_Exception.HandleDate = Convert.ToDateTime(dt.Rows[i]["HandleDate"] == DBNull.Value ? DateTime.Now.ToString("yyyy-MM-dd") : dt.Rows[i]["HandleDate"].ToString());
                        modelWayBillFlow_Exception.mMemo = dt.Rows[i]["mMemo"] == DBNull.Value ? "" : dt.Rows[i]["mMemo"].ToString();

                        Insert(modelWayBillFlow_Exception);
                    }
                }
            }
            return bOK;
        }

        /// <summary>
        /// 根据ID删除记录
        /// </summary>
        /// <param name="Wbf_wbSerialNum"></param>
        /// <param name="Wbf_swbSerialNum"></param>
        /// <returns></returns>
        public Boolean Delete(string ids)
        {
            bool bOK = false;
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("delete from WayBillFlow_Exception");
                strSql.Append(" where WbfID in (" + ids + ")");
                if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) > 0)
                {
                    bOK = true;
                }
            }
            catch (Exception ex)
            {
                bOK = false;
            }
            
            return bOK;
        }
    }
}
