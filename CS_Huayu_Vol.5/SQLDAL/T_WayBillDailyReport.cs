using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SQLDAL
{
    public class T_WayBillDailyReport
    {
        /// <summary>
        /// 新增日报表信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool addWayBillDailyReport(Model.M_WayBillDailyReport model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into WayBillDailyReport");
            strSql.Append(@" ([wbrCode]
           ,[CustomsCategory]
           ,[wbr_wbID]
           ,[InStoreDate]
           ,[OutStoreDate]
           ,[WayBillActualWeight]
           ,[OperateFee]
           ,[PickGoodsFee]
           ,[KeepGoodsFee]
           ,[ShiftGoodsFee]
           ,[RejectGoodsFee]
           ,[CollectionKeepGoodsFee]
           ,[ActualPay]
           ,[PayMethod]
           ,[Receipt]
           ,[ShouldPayUnit]
           ,[shouldPay]
           ,[ReceptMethod]
           ,[SalesMan]
           ,[mMemo])");
            strSql.Append(" values (");
            strSql.Append(@"@wbrCode
           ,@CustomsCategory
           ,@wbr_wbID
           ,@InStoreDate
           ,@OutStoreDate
           ,@WayBillActualWeight
           ,@OperateFee
           ,@PickGoodsFee
           ,@KeepGoodsFee
           ,@ShiftGoodsFee
           ,@RejectGoodsFee
           ,@CollectionKeepGoodsFee
           ,@ActualPay
           ,@PayMethod
           ,@Receipt
           ,@ShouldPayUnit
           ,@shouldPay
           ,@ReceptMethod
           ,@SalesMan
           ,@mMemo)");

            SqlParameter[] parameters = {
                   
                    new SqlParameter("@wbrCode",SqlDbType.NVarChar),
                    new SqlParameter("@CustomsCategory", SqlDbType.NVarChar),
                    new SqlParameter("@wbr_wbID", SqlDbType.Int), 
                    new SqlParameter("@InStoreDate", SqlDbType.NVarChar),
                    new SqlParameter("@OutStoreDate", SqlDbType.NVarChar),
                    new SqlParameter("@WayBillActualWeight", SqlDbType.NVarChar),
                    new SqlParameter("@OperateFee", SqlDbType.NVarChar),
                    new SqlParameter("@PickGoodsFee", SqlDbType.NVarChar),
                    new SqlParameter("@KeepGoodsFee", SqlDbType.NVarChar),
                    new SqlParameter("@ShiftGoodsFee", SqlDbType.NVarChar),
                    new SqlParameter("@RejectGoodsFee", SqlDbType.NVarChar),
                    new SqlParameter("@CollectionKeepGoodsFee", SqlDbType.NVarChar),
                    new SqlParameter("@ActualPay", SqlDbType.NVarChar),
                    new SqlParameter("@PayMethod", SqlDbType.NVarChar),
                    new SqlParameter("@Receipt", SqlDbType.NVarChar),
                    new SqlParameter("@ShouldPayUnit", SqlDbType.NVarChar),
                    new SqlParameter("@shouldPay", SqlDbType.NVarChar),
                    new SqlParameter("@ReceptMethod", SqlDbType.NVarChar),
                    new SqlParameter("@SalesMan", SqlDbType.NVarChar),
                    new SqlParameter("@mMemo", SqlDbType.NText)
            };

            parameters[0].Value = model.wbrCode == null ? "" : model.wbrCode;
            parameters[1].Value = model.CustomsCategory == null ? "" : model.CustomsCategory;
            parameters[2].Value = model.wbr_wbID == null ? -1 : model.wbr_wbID;
            parameters[3].Value = model.InStoreDate == null ? "" : model.InStoreDate;
            parameters[4].Value = model.OutStoreDate == null ? "" : model.OutStoreDate;
            parameters[5].Value = model.WayBillActualWeight == null ? "0.00" : model.WayBillActualWeight;
            parameters[6].Value = model.OperateFee == null ? "0.00" : model.OperateFee;
            parameters[7].Value = model.PickGoodsFee == null ? "0.00" : model.PickGoodsFee;
            parameters[8].Value = model.KeepGoodsFee == null ? "0.00" : model.KeepGoodsFee;
            parameters[9].Value = model.ShiftGoodsFee == null ? "0.00" : model.ShiftGoodsFee;
            parameters[10].Value = model.RejectGoodsFee == null ? "0.00" : model.RejectGoodsFee;
            parameters[11].Value = model.CollectionKeepGoodsFee == null ? "0.00" : model.CollectionKeepGoodsFee;
            parameters[12].Value = model.ActualPay == null ? "0.00" : model.ActualPay;
            parameters[13].Value = model.PayMethod == null ? "" : model.PayMethod;
            parameters[14].Value = model.Receipt == null ? "" : model.Receipt;
            parameters[15].Value = model.ShouldPayUnit == null ? "" : model.ShouldPayUnit;
            parameters[16].Value = model.shouldPay == null ? "" : model.shouldPay;
            parameters[17].Value = model.ReceptMethod == null ? "" : model.ReceptMethod;
            parameters[18].Value = model.SalesMan == null ? "" : model.SalesMan;
            parameters[19].Value = model.mMemo == null ? "" : model.mMemo;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
