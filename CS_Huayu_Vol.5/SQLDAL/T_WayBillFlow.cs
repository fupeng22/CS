using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace SQLDAL
{
    public class T_WayBillFlow
    {

        /// <summary>
        /// 根据总运单号与分运单号来查询是否已经在入库表中(已经入库，并不一定是正常入库)
        /// </summary>
        /// <param name="Wbf_wbSerialNum"></param>
        /// <param name="Wbf_swbSerialNum"></param>
        /// <returns></returns>
        public Boolean ExistInStoreWayBill(string Wbf_wbSerialNum, string Wbf_swbSerialNum)
        {
            Boolean bExist = false;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from V_WayBillFlow_Fast");
            strSql.Append(" where wbSerialNum='" + Wbf_wbSerialNum + "' and swbSerialNum='" + Wbf_swbSerialNum + "'");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());

            if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) != 0)
            {
                bExist = true;
            }

            return bExist;
        }

        /// <summary>
        /// 根据总运单号与分运单号来查询是否已经在预入库表中(即货代公司导入的货物)
        /// </summary>
        /// <param name="Wbf_wbSerialNum"></param>
        /// <param name="Wbf_swbSerialNum"></param>
        /// <returns></returns>
        public Boolean ExistInForeWayBill(string Wbf_wbSerialNum, string Wbf_swbSerialNum)
        {
            Boolean bExist = false;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from V_WayBill_SubWayBill");
            strSql.Append(" where wbSerialNum='" + Wbf_wbSerialNum + "' and swbSerialNum='" + Wbf_swbSerialNum + "'");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());

            if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) != 0)
            {
                bExist = true;
            }

            return bExist;
        }

        /// <summary>
        /// 根据总运单号与分运单号来查询运单流
        /// </summary>
        /// <param name="Wbf_wbSerialNum"></param>
        /// <param name="Wbf_swbSerialNum"></param>
        /// <returns></returns>
        public DataSet getWayBillFlow(string Wbf_wbSerialNum, string Wbf_swbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from V_WayBillFlow");
            strSql.Append(" where wbSerialNum='" + Wbf_wbSerialNum + "' and swbSerialNum='" + Wbf_swbSerialNum + "'");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        /// <summary>
        /// 根据总运单号与分运单号来查询预入库运单信息
        /// </summary>
        /// <param name="Wbf_wbSerialNum"></param>
        /// <param name="Wbf_swbSerialNum"></param>
        /// <returns></returns>
        public DataSet getForeWayBill(string Wbf_wbSerialNum, string Wbf_swbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from V_WayBill_SubWayBill");
            strSql.Append(" where wbSerialNum='" + Wbf_wbSerialNum + "' and swbSerialNum='" + Wbf_swbSerialNum + "'");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        //        /// <summary>
        //        /// 根据总运单号、子运单号来更改运单状态
        //        /// </summary>
        //        /// <param name="wbSerialNum"></param>
        //        /// <param name="swbSerialNum"></param>
        //        /// <param name="iType">0:更改为正常已出库</param>
        //        /// <returns></returns>
        //        public bool UpdateStatus(string wbSerialNum, string swbSerialNum, int iType, string strOperator, string wbImportType, string swbID)
        //        {
        //            StringBuilder strSql = new StringBuilder();

        //            ArrayList arrSQL = new ArrayList();

        //            DataSet ds = null;
        //            DataTable dt = null;

        //            DataSet dsTmp = null;
        //            DataTable dtTmp = null;

        //            switch (iType)
        //            {
        //                case 0://出库
        //                    //strSql.Append("update WayBillFlow");
        //                    //strSql.Append("  set status=3,ExceptionStatus=-1,ExceptionDescription='',operateDate=getDate(),Operator='" + strOperator + "' ");
        //                    //strSql.Append(" from WayBillFlow WBF,V_WayBill_SubWayBill VWBSWB where VWBSWB.swbID=WBF.Wbf_swbID and VWBSWB.wbID=WBF.Wbf_wbID and VWBSWB.wbSerialNum='" + wbSerialNum + "' and VWBSWB.swbSerialNum='" + swbSerialNum + "' ");
        //                    //strSql.Append("update WayBillFlow");
        //                    //strSql.Append("  set IsOutStore=3,ExceptionStatus=-1,ExceptionDescription='',OutStoreDate=getDate(),OutStoreOperator='" + strOperator + "' ");
        //                    //strSql.Append(" from WayBillFlow WBF,V_WayBill_SubWayBill VWBSWB where VWBSWB.swbID=WBF.Wbf_swbID and VWBSWB.wbID=WBF.Wbf_wbID and VWBSWB.wbSerialNum='" + wbSerialNum + "' and VWBSWB.swbSerialNum='" + swbSerialNum + "' ");

        //                    arrSQL.Add(@"update WayBillFlow   set IsOutStore=3,ExceptionStatus=-1,ExceptionDescription='',OutStoreDate=getDate(),OutStoreOperator='" + strOperator + "'  from WayBillFlow WBF,V_WayBill_SubWayBill VWBSWB where VWBSWB.swbID=WBF.Wbf_swbID and VWBSWB.wbID=WBF.Wbf_wbID and VWBSWB.wbSerialNum='" + wbSerialNum + "' and VWBSWB.swbSerialNum='" + swbSerialNum + "' ");

        //                    if (wbImportType == "1")
        //                    {
        //                        dsTmp = new T_SubWayBill().GetSubWayBillInfo(wbSerialNum, swbSerialNum);
        //                        if (dsTmp != null)
        //                        {
        //                            dtTmp = dsTmp.Tables[0];
        //                            if (dtTmp != null && dtTmp.Rows.Count > 0)
        //                            {
        //                                if (new T_SynchronizeInOutStore().ExistSynchronizeInOutStore(dtTmp.Rows[0]["swbID"].ToString()))
        //                                {
        //                                    arrSQL.Add(string.Format(@"UPDATE  dbo.SynchronizeInOutStore
        //                                                                SET      sios_swbID = {2},
        //                                                                        sios_wbSerialNum = '{0}' ,
        //                                                                        sios_swbSerialNum = '{1}' ,
        //                                                                        sios_OutStoreFlag = 1
        //                                                                WHERE  sios_wbSerialNum='{0}' and sios_swbSerialNum='{1}'", wbSerialNum, swbSerialNum, dtTmp.Rows[0]["swbID"].ToString()));
        //                                }
        //                                else
        //                                {
        //                                    arrSQL.Add(string.Format(@"INSERT  INTO dbo.SynchronizeInOutStore
        //                                                                        ( sios_swbID ,
        //                                                                          sios_wbSerialNum ,
        //                                                                          sios_swbSerialNum ,
        //                                                                          sios_InStoreFlag ,
        //                                                                          sios_OutStoreFlag
        //                                                                        )
        //                                                                VALUES  ( {0} ,
        //                                                                          '{1}' ,
        //                                                                          '{2}' ,
        //                                                                          0 ,
        //                                                                          1
        //                                                                        )", dtTmp.Rows[0]["swbID"].ToString(), wbSerialNum, swbSerialNum));
        //                                }
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case 1://入库
        //                    if (ExistInStoreWayBill(wbSerialNum, swbSerialNum))//若已经有入库异常的记录，则直接更改其为入库正常的记录
        //                    {
        //                        //strSql.Append("update WayBillFlow");
        //                        //strSql.Append("  set status=1,ExceptionStatus=-1,ExceptionDescription='',operateDate=getDate(),Operator='" + strOperator + "' ");
        //                        //strSql.Append(" from WayBillFlow WBF,V_WayBill_SubWayBill VWBSWB where VWBSWB.swbID=WBF.Wbf_swbID and VWBSWB.wbID=WBF.Wbf_wbID and VWBSWB.wbSerialNum='" + wbSerialNum + "' and VWBSWB.swbSerialNum='" + swbSerialNum + "' ");

        //                        arrSQL.Add(" update WayBillFlow   set status=1,ExceptionStatus=-1,ExceptionDescription='',operateDate=getDate(),Operator='" + strOperator + "'   from WayBillFlow WBF,V_WayBill_SubWayBill VWBSWB where VWBSWB.swbID=WBF.Wbf_swbID and VWBSWB.wbID=WBF.Wbf_wbID and VWBSWB.wbSerialNum='" + wbSerialNum + "' and VWBSWB.swbSerialNum='" + swbSerialNum + "' ");

        //                        if (wbImportType == "1")
        //                        {
        //                            dsTmp = new T_SubWayBill().GetSubWayBillInfo(wbSerialNum, swbSerialNum);
        //                            if (dsTmp != null)
        //                            {
        //                                dtTmp = dsTmp.Tables[0];
        //                                if (dtTmp != null && dtTmp.Rows.Count > 0)
        //                                {
        //                                    if (new T_SynchronizeInOutStore().ExistSynchronizeInOutStore(dtTmp.Rows[0]["swbID"].ToString()))
        //                                    {
        //                                        arrSQL.Add(string.Format(@"UPDATE  dbo.SynchronizeInOutStore
        //                                                                SET      sios_swbID = {2},
        //                                                                        sios_wbSerialNum = '{0}' ,
        //                                                                        sios_swbSerialNum = '{1}' ,
        //                                                                        sios_InStoreFlag = 1
        //                                                                WHERE  sios_wbSerialNum = '{0}' and sios_swbSerialNum = '{1}' ", wbSerialNum, swbSerialNum, dtTmp.Rows[0]["swbID"].ToString()));
        //                                    }
        //                                    else
        //                                    {
        //                                        arrSQL.Add(string.Format(@"INSERT  INTO dbo.SynchronizeInOutStore
        //                                                                        ( sios_swbID ,
        //                                                                          sios_wbSerialNum ,
        //                                                                          sios_swbSerialNum ,
        //                                                                          sios_InStoreFlag ,
        //                                                                          sios_OutStoreFlag
        //                                                                        )
        //                                                                VALUES  ( {0} ,
        //                                                                          '{1}' ,
        //                                                                          '{2}' ,
        //                                                                          1 ,
        //                                                                          0
        //                                                                        )", dtTmp.Rows[0]["swbID"].ToString(), wbSerialNum, swbSerialNum));
        //                                    }
        //                                }

        //                            }

        //                        }
        //                    }
        //                    else//如果没有入库记录，则新增一条入库记录
        //                    {
        //                        if (ExistInForeWayBill(wbSerialNum, swbSerialNum))//如果在预入库中有此记录,才可进行入库
        //                        {
        //                            ds = getForeWayBill(wbSerialNum, swbSerialNum);
        //                            if (ds != null)
        //                            {
        //                                dt = ds.Tables[0];
        //                                if (dt != null && dt.Rows.Count > 0)
        //                                {
        //                                    arrSQL.Add(string.Format(@" insert into WayBillFlow(Wbf_wbID,Wbf_swbID,Wbf_swbSerialNum,status,Operator,operateDate)
        //                                                        values({0},{1},'{2}',1,'{3}',GETDATE())", dt.Rows[0]["wbID"].ToString(), dt.Rows[0]["swbID"].ToString(), dt.Rows[0]["swbSerialNum"].ToString(), strOperator));

        //                                    if (wbImportType == "1")
        //                                    {
        //                                        if (new T_SynchronizeInOutStore().ExistSynchronizeInOutStore(dt.Rows[0]["swbID"].ToString()))
        //                                        {
        //                                            arrSQL.Add(string.Format(@"UPDATE  dbo.SynchronizeInOutStore
        //                                                                SET     sios_swbID = {2},
        //                                                                        sios_wbSerialNum = '{0}' ,
        //                                                                        sios_swbSerialNum = '{1}' ,
        //                                                                        sios_InStoreFlag = 1
        //                                                                WHERE   sios_wbSerialNum = '{0}' and sios_swbSerialNum = '{1}' ", wbSerialNum, swbSerialNum, dt.Rows[0]["swbID"].ToString()));
        //                                        }
        //                                        else
        //                                        {
        //                                            arrSQL.Add(string.Format(@"INSERT  INTO dbo.SynchronizeInOutStore
        //                                                                        ( sios_swbID ,
        //                                                                          sios_wbSerialNum ,
        //                                                                          sios_swbSerialNum ,
        //                                                                          sios_InStoreFlag ,
        //                                                                          sios_OutStoreFlag
        //                                                                        )
        //                                                                VALUES  ( {0} ,
        //                                                                          '{1}' ,
        //                                                                          '{2}' ,
        //                                                                          1 ,
        //                                                                          0
        //                                                                        )", dt.Rows[0]["swbID"].ToString(), wbSerialNum, swbSerialNum));
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        else//如果在预入库中没有此记录,不可进行入库
        //                        {

        //                        }
        //                    }
        //                    break;
        //                default:
        //                    break;
        //            }

        //            try
        //            {
        //                DBUtility.SqlServerHelper.ExecuteSqlTran(arrSQL);
        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                return false;
        //            }

        //        }

        /// <summary>
        /// 根据总运单号、子运单号来更改运单状态
        /// </summary>
        /// <param name="wbSerialNum"></param>
        /// <param name="swbSerialNum"></param>
        /// <param name="iType">0:更改为正常已出库</param>
        /// <returns></returns>
        public bool UpdateStatus(string wbSerialNum, string swbSerialNum, int iType, string strOperator, string wbImportType, string wbID, string swbID)
        {
            StringBuilder strSql = new StringBuilder();

            ArrayList arrSQL = new ArrayList();

            DataSet ds = null;
            DataTable dt = null;

            DataSet dsTmp = null;
            DataTable dtTmp = null;

            switch (iType)
            {
                case 0://出库
                    //strSql.Append("update WayBillFlow");
                    //strSql.Append("  set status=3,ExceptionStatus=-1,ExceptionDescription='',operateDate=getDate(),Operator='" + strOperator + "' ");
                    //strSql.Append(" from WayBillFlow WBF,V_WayBill_SubWayBill VWBSWB where VWBSWB.swbID=WBF.Wbf_swbID and VWBSWB.wbID=WBF.Wbf_wbID and VWBSWB.wbSerialNum='" + wbSerialNum + "' and VWBSWB.swbSerialNum='" + swbSerialNum + "' ");
                    //strSql.Append("update WayBillFlow");
                    //strSql.Append("  set IsOutStore=3,ExceptionStatus=-1,ExceptionDescription='',OutStoreDate=getDate(),OutStoreOperator='" + strOperator + "' ");
                    //strSql.Append(" from WayBillFlow WBF,V_WayBill_SubWayBill VWBSWB where VWBSWB.swbID=WBF.Wbf_swbID and VWBSWB.wbID=WBF.Wbf_wbID and VWBSWB.wbSerialNum='" + wbSerialNum + "' and VWBSWB.swbSerialNum='" + swbSerialNum + "' ");

                    arrSQL.Add(@"update WayBillFlow   set IsOutStore=3,ExceptionStatus=-1,ExceptionDescription='',OutStoreDate=getDate(),OutStoreOperator='" + strOperator + "'  from WayBillFlow WBF,V_WayBill_SubWayBill VWBSWB where VWBSWB.swbID=WBF.Wbf_swbID and VWBSWB.wbID=WBF.Wbf_wbID and VWBSWB.wbSerialNum='" + wbSerialNum + "' and VWBSWB.swbSerialNum='" + swbSerialNum + "' ");

                    if (wbImportType == "1")
                    {
                        if (new T_SynchronizeInOutStore().ExistSynchronizeInOutStore(swbID))
                        {
                            arrSQL.Add(string.Format(@"UPDATE  dbo.SynchronizeInOutStore
                                                                SET      sios_swbID = {2},
                                                                        sios_wbSerialNum = '{0}' ,
                                                                        sios_swbSerialNum = '{1}' ,
                                                                        sios_OutStoreFlag = 1
                                                                WHERE  sios_wbSerialNum='{0}' and sios_swbSerialNum='{1}'", wbSerialNum, swbSerialNum, swbID));
                        }
                        else
                        {
                            arrSQL.Add(string.Format(@"INSERT  INTO dbo.SynchronizeInOutStore
                                                                        ( sios_swbID ,
                                                                          sios_wbSerialNum ,
                                                                          sios_swbSerialNum ,
                                                                          sios_InStoreFlag ,
                                                                          sios_OutStoreFlag
                                                                        )
                                                                VALUES  ( {0} ,
                                                                          '{1}' ,
                                                                          '{2}' ,
                                                                          0 ,
                                                                          1
                                                                        )", swbID, wbSerialNum, swbSerialNum));
                        }
                    }
                    break;
                case 1://入库
                    if (ExistInStoreWayBill(wbSerialNum, swbSerialNum))//若已经有入库异常的记录，则直接更改其为入库正常的记录
                    {
                        arrSQL.Add(" update WayBillFlow   set status=1,ExceptionStatus=-1,ExceptionDescription='',operateDate=getDate(),Operator='" + strOperator + "'   from WayBillFlow WBF,V_WayBill_SubWayBill VWBSWB where VWBSWB.swbID=WBF.Wbf_swbID and VWBSWB.wbID=WBF.Wbf_wbID and VWBSWB.wbSerialNum='" + wbSerialNum + "' and VWBSWB.swbSerialNum='" + swbSerialNum + "' ");

                        if (wbImportType == "1")
                        {
                            if (new T_SynchronizeInOutStore().ExistSynchronizeInOutStore(swbID))
                            {
                                arrSQL.Add(string.Format(@"UPDATE  dbo.SynchronizeInOutStore
                                                                SET      sios_swbID = {2},
                                                                        sios_wbSerialNum = '{0}' ,
                                                                        sios_swbSerialNum = '{1}' ,
                                                                        sios_InStoreFlag = 1
                                                                WHERE  sios_wbSerialNum = '{0}' and sios_swbSerialNum = '{1}' ", wbSerialNum, swbSerialNum, swbID));
                            }
                            else
                            {
                                arrSQL.Add(string.Format(@"INSERT  INTO dbo.SynchronizeInOutStore
                                                                        ( sios_swbID ,
                                                                          sios_wbSerialNum ,
                                                                          sios_swbSerialNum ,
                                                                          sios_InStoreFlag ,
                                                                          sios_OutStoreFlag
                                                                        )
                                                                VALUES  ( {0} ,
                                                                          '{1}' ,
                                                                          '{2}' ,
                                                                          1 ,
                                                                          0
                                                                        )", swbID, wbSerialNum, swbSerialNum));
                            }
                        }
                    }
                    else//如果没有入库记录，则新增一条入库记录
                    {
                        arrSQL.Add(string.Format(@" insert into WayBillFlow(Wbf_wbID,Wbf_swbID,Wbf_swbSerialNum,status,Operator,operateDate)
                                                        values({0},{1},'{2}',1,'{3}',GETDATE())", wbID, swbID, swbSerialNum, strOperator));

                        if (wbImportType == "1")
                        {
                            if (new T_SynchronizeInOutStore().ExistSynchronizeInOutStore(swbID))
                            {
                                arrSQL.Add(string.Format(@"UPDATE  dbo.SynchronizeInOutStore
                                                                SET     sios_swbID = {2},
                                                                        sios_wbSerialNum = '{0}' ,
                                                                        sios_swbSerialNum = '{1}' ,
                                                                        sios_InStoreFlag = 1
                                                                WHERE   sios_wbSerialNum = '{0}' and sios_swbSerialNum = '{1}' ", wbSerialNum, swbSerialNum, swbID));
                            }
                            else
                            {
                                arrSQL.Add(string.Format(@"INSERT  INTO dbo.SynchronizeInOutStore
                                                                        ( sios_swbID ,
                                                                          sios_wbSerialNum ,
                                                                          sios_swbSerialNum ,
                                                                          sios_InStoreFlag ,
                                                                          sios_OutStoreFlag
                                                                        )
                                                                VALUES  ( {0} ,
                                                                          '{1}' ,
                                                                          '{2}' ,
                                                                          1 ,
                                                                          0
                                                                        )", swbID, wbSerialNum, swbSerialNum));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            try
            {
                DBUtility.SqlServerHelper.ExecuteSqlTran(arrSQL);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public bool PatchInOutStoreByWbId(string wbId, int iType, string strOperator)
        {
            string strSQL = "";
            StringBuilder sbSQL = new StringBuilder();

            ArrayList arrSQL = new ArrayList();

            DataSet ds = null;
            DataTable dt = null;

            DataSet dsTmp = null;
            DataTable dtTmp = null;
            try
            {

                switch (iType)
                {
                    case 0://出库
                        //将本次出库记录更新到中间表
                        strSQL = string.Format(@"UPDATE  dbo.SynchronizeInOutStore
                                                SET     sios_OutStoreFlag = 1
                                                WHERE   sios_swbID IN (
                                                        SELECT  WBF.Wbf_swbID
                                                        FROM    dbo.WayBillFlow WBF
                                                                INNER JOIN dbo.V_WayBill_SubWayBill VWBSWB ON WBF.Wbf_swbID = VWBSWB.swbID
                                                        WHERE   VWBSWB.wbID = {0}
                                                                AND WBF.status = 1
                                                                AND wbf.IsOutStore <> 3
                                                                AND VWBSWB.wbImportType = 1
                                                                AND VWBSWB.swbReleaseFlag = 3 )", wbId);
                        DBUtility.SqlServerHelper.ExecuteSql(strSQL);

                        //正式更新本次出库信息
                        strSQL = string.Format(@"UPDATE  dbo.WayBillFlow
                                                SET     IsOutStore = 3 ,
                                                        ExceptionStatus = -1 ,
                                                        ExceptionDescription = '' ,
                                                        OutStoreDate = GETDATE() ,
                                                        OutStoreOperator = '{1}'
                                                FROM    dbo.WayBillFlow WBF
                                                        INNER JOIN dbo.V_WayBill_SubWayBill VWBSWB ON VWBSWB.swbID = WBF.Wbf_swbID
                                                WHERE   WBF.status = 1
                                                        AND WBF.IsOutStore <> 3
                                                        AND VWBSWB.wbID = {0}
                                                        AND ( VWBSWB.wbImportType = 0
                                                              OR ( VWBSWB.wbImportType = 1
                                                                   AND VWBSWB.swbReleaseFlag = 3
                                                                 )
                                                            )", wbId, strOperator);
                        DBUtility.SqlServerHelper.ExecuteSql(strSQL);
                        break;
                    case 1://入库
                        //删除有异常的入库信息
                        strSQL = string.Format(@"DELETE  FROM dbo.WayBillFlow
                                                FROM    dbo.WayBillFlow WBF
                                                        INNER JOIN dbo.V_WayBill_SubWayBill VWBSWB ON WBF.Wbf_swbSerialNum = VWBSWB.swbSerialNum
                                                WHERE   WBF.status <> 1
                                                        AND WBF.IsOutStore <> 3
                                                        AND VWBSWB.wbID = {0}", wbId);
                        DBUtility.SqlServerHelper.ExecuteSql(strSQL);

                        //将本次入库记录同步到中间表
//                        strSQL = string.Format(@"INSERT  INTO dbo.SynchronizeInOutStore
//                                                ( sios_swbID ,
//                                                  sios_wbSerialNum ,
//                                                  sios_swbSerialNum ,
//                                                  sios_InStoreFlag ,
//                                                  sios_OutStoreFlag
//                                                )
//                                                SELECT  VWBSWB.swbID ,
//                                                        VWBSWB.wbSerialNum ,
//                                                        VWBSWB.swbSerialNum ,
//                                                        1 ,
//                                                        0
//                                                FROM    dbo.V_WayBill_SubWayBill VWBSWB
//                                                WHERE   VWBSWB.wbID = {0}
//                                                        AND VWBSWB.swbID NOT IN ( SELECT    Wbf_swbID
//                                                                                  FROM      dbo.WayBillFlow
//                                                                                  WHERE     Wbf_wbID = {0} )
//                                                        AND VWBSWB.swbID NOT IN ( SELECT    sios_swbID
//                                                                                  FROM      dbo.SynchronizeInOutStore )
//                                                        AND VWBSWB.wbImportType = 1
//        
//                                        ", wbId);
                        strSQL = string.Format(@"INSERT  INTO dbo.SynchronizeInOutStore
                                                ( sios_swbID ,
                                                  sios_wbSerialNum ,
                                                  sios_swbSerialNum ,
                                                  sios_InStoreFlag ,
                                                  sios_OutStoreFlag
                                                )
                                                SELECT  VWBSWB.swbID ,
                                                        VWBSWB.wbSerialNum ,
                                                        VWBSWB.swbSerialNum ,
                                                        1 ,
                                                        0
                                                FROM    ( SELECT    SWB.swbID ,
                                                                    WB.wbID ,
                                                                    swbSerialNum ,
                                                                    wbSerialNum ,
                                                                    wbImportType
                                                          FROM      dbo.Waybill WB
                                                                    INNER JOIN dbo.SubWaybill SWB ON WB.wbID = SWB.swb_wbID
                                                        ) VWBSWB
                                                WHERE   VWBSWB.wbID = {0}
                                                        AND VWBSWB.wbImportType = 1
                                                        AND NOT EXISTS ( SELECT Wbf_swbID
                                                                         FROM   dbo.WayBillFlow
                                                                         WHERE  Wbf_wbID = {0}
                                                                                AND Waybillflow.Wbf_swbID = VWBSWB.swbID )
                                                       ", wbId);
                        DBUtility.SqlServerHelper.ExecuteSql(strSQL);

                        //正式进行本次入库
                        strSQL = string.Format(@"INSERT  INTO dbo.WayBillFlow
                                                ( Wbf_wbID ,
                                                  Wbf_swbID ,
                                                  Wbf_swbSerialNum ,
                                                  status ,
                                                  operateDate ,
                                                  IsOutStore ,
                                                  OutStoreDate ,
                                                  ExceptionStatus ,
                                                  ExceptionDescription ,
                                                  HandleDescription ,
                                                  Operator ,
                                                  OutStoreOperator ,
                                                  Handler ,
                                                  HandleDate ,
                                                  mMemo
                                                )
                                                SELECT  VWBSWB.wbID ,
                                                        VWBSWB.swbID ,
                                                        VWBSWB.swbSerialNum ,
                                                        1 ,
                                                        GETDATE() ,
                                                        0 ,
                                                        NULL ,
                                                        -1 ,
                                                        '' ,
                                                        '' ,
                                                        '{1}' ,
                                                        '' ,
                                                        '' ,
                                                        NULL ,
                                                        ''
                                                FROM    dbo.V_WayBill_SubWayBill VWBSWB
                                                WHERE   VWBSWB.wbID = {0}
                                                        AND VWBSWB.swbID NOT IN ( SELECT    Wbf_swbID
                                                                                  FROM      dbo.WayBillFlow
                                                                                  WHERE     Wbf_wbID = {0} )      
                                          ", wbId, strOperator);
                        DBUtility.SqlServerHelper.ExecuteSql(strSQL);
                        break;
                    default:
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 根据总运单号与分运单号来查询运单流
        /// </summary>
        /// <param name="Wbf_wbSerialNum"></param>
        /// <param name="Wbf_swbSerialNum"></param>
        /// <returns></returns>
        public DataSet getWayBillFlowWithIds(string ids)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from V_WayBillFlow");
            strSql.Append(" where WbfID in (" + ids + ")");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        public bool UpdateHandleStatus(string ids, string ExceptionStatus, string HandleDescription, string strOperator)
        {
            bool bOK = false;
            StringBuilder strSql = new StringBuilder();

            strSql.Append("update WayBillFlow ");
            strSql.Append("  set HandleDate=getDate(),ExceptionStatus=@ExceptionStatus,Handler=@Operator,HandleDescription=@HandleDescription");
            strSql.Append(" where WbfID in (" + ids + ")");
            SqlParameter[] parameters = {
                      
                        new SqlParameter("@ExceptionStatus",SqlDbType.Int),
                        new SqlParameter("@Operator",SqlDbType.NVarChar,255),
                        new SqlParameter("@HandleDescription",SqlDbType.NVarChar,255)                                     };

            parameters[0].Value = ExceptionStatus;
            parameters[1].Value = strOperator;
            parameters[2].Value = HandleDescription;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                bOK = true;
            }
            return bOK;
        }

        /// <summary>
        /// 批量出入仓
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="iType"></param>
        /// <returns></returns>
        public bool PatchInOutStore(string ids, int iType, string strOperator)
        {
            bool bOK = false;
            StringBuilder strSql = new StringBuilder();
            switch (iType)
            {
                case 1:
                    strSql.Append(" update WayBillFlow set status=@status,operateDate=GETDATE(),ExceptionStatus=-1,ExceptionDescription='',HandleDescription='',Operator=@Operator,Handler='',HandleDate=GETDATE() ");
                    strSql.Append(" where WbfID in (" + ids + ")");
                    break;
                case 3:
                    strSql.Append(" update WayBillFlow set IsOutStore=@status,OutStoreDate=GETDATE(),ExceptionStatus=-1,ExceptionDescription='',HandleDescription='',OutStoreOperator=@Operator,Handler='',HandleDate=GETDATE() ");
                    strSql.Append(" where WbfID in (" + ids + ")");
                    break;
                default:
                    break;
            }

            SqlParameter[] parameters = {
                      
                        new SqlParameter("@status",SqlDbType.Int),
                        new SqlParameter("@Operator",SqlDbType.NVarChar,255)            
                                        };
            parameters[0].Value = iType;
            parameters[1].Value = strOperator;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                bOK = true;
            }
            return bOK;
        }


        /// <summary>
        /// 根据条件查询已入库或出库的总运单ID信息（已去重）
        /// </summary>
        /// <param name="InOutStoreBeginDate"></param>
        /// <param name="InOutStoreEndDate"></param>
        /// <param name="wbCompany"></param>
        /// <param name="wbSerialNum"></param>
        /// <param name="swbSerialNum"></param>
        /// <param name="swbNeedCheck"></param>
        /// <param name="order"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public DataSet getWayBillWithCustomCondition(string InOutStoreBeginDate, string InOutStoreEndDate, string wbCompany, string wbSerialNum, string swbSerialNum, string swbNeedCheck, string txtInOutStoreType, string sort, string order)
        {
            StringBuilder sbSQL = new StringBuilder("");
            string strWhereTmp = "";

            //sbSQL.Append(" select distinct wbID from V_WayBillFlow_ID  ");

            //strWhereTmp = strWhereTmp + "  (wbID<>0 and swbID<>0) ";

            sbSQL.Append(" select  wbID from V_WayBillFlow_ID ");

            if (InOutStoreBeginDate != "" && InOutStoreEndDate != "")
            {
                if (strWhereTmp != "")
                {
                    strWhereTmp = strWhereTmp + " and ((CONVERT(nvarchar(10),operateDate,120)>='" + Convert.ToDateTime(InOutStoreBeginDate).ToString("yyyy-MM-dd") + "' and (CONVERT(nvarchar(10),operateDate,120)<='" + Convert.ToDateTime(InOutStoreEndDate).ToString("yyyy-MM-dd") + "'))) ";
                }
                else
                {
                    strWhereTmp = strWhereTmp + " ((CONVERT(nvarchar(10),operateDate,120)>='" + Convert.ToDateTime(InOutStoreBeginDate).ToString("yyyy-MM-dd") + "' and (CONVERT(nvarchar(10),operateDate,120)<='" + Convert.ToDateTime(InOutStoreEndDate).ToString("yyyy-MM-dd") + "'))) ";
                }
            }

            if (wbCompany != "" && wbCompany != "---请选择---")
            {
                if (strWhereTmp != "")
                {
                    strWhereTmp = strWhereTmp + " and (wbCompany like '" + wbCompany + "') ";
                }
                else
                {
                    strWhereTmp = strWhereTmp + " (wbCompany like '" + wbCompany + "') ";
                }
            }

            if (wbSerialNum != "")
            {
                if (strWhereTmp != "")
                {
                    strWhereTmp = strWhereTmp + " and (wbSerialNum like '" + wbSerialNum + "') ";
                }
                else
                {
                    strWhereTmp = strWhereTmp + " (wbSerialNum like '" + wbSerialNum + "') ";
                }
            }

            if (swbSerialNum != "")
            {
                if (strWhereTmp != "")
                {
                    strWhereTmp = strWhereTmp + " and (swbSerialNum like '" + swbSerialNum + "') ";
                }
                else
                {
                    strWhereTmp = strWhereTmp + " (swbSerialNum like '" + swbSerialNum + "') ";
                }
            }

            if (swbNeedCheck != "" && swbNeedCheck != "---请选择---")
            {
                if (strWhereTmp != "")
                {
                    strWhereTmp = strWhereTmp + " and (swbNeedCheck=" + swbNeedCheck + ") ";
                }
                else
                {
                    strWhereTmp = strWhereTmp + " (swbNeedCheck=" + swbNeedCheck + ") ";
                }
            }

            if (txtInOutStoreType != "" && txtInOutStoreType != "-99")
            {
                if (strWhereTmp != "")
                {
                    strWhereTmp = strWhereTmp + " and (InOutStoreType in(" + txtInOutStoreType + ")) ";
                }
                else
                {
                    strWhereTmp = strWhereTmp + " (InOutStoreType in(" + txtInOutStoreType + ")) ";
                }
            }

            if (strWhereTmp != "")
            {
                sbSQL.Append(" where  " + strWhereTmp);
            }

            if (order != "" && sort != "")
            {
                switch (sort.ToLower())
                {
                    case "wbtotalnumbe":
                        break;
                    case "wbtotalweight":
                        break;
                    case "instorecount":
                        break;
                    case "outstorecount":
                        break;
                    default:
                        //sbSQL.Append(" order by  " + sort + " " + order);
                        break;
                }
            }
            sbSQL.Append(" order by wbID desc ");
            DataSet ds = DBUtility.SqlServerHelper.Query(sbSQL.ToString());
            return ds;
        }

        /// <summary>
        /// 根据子运单号查询其总运单信息
        /// </summary>
        /// <param name="swbSerialNum"></param>
        /// <returns></returns>
        public DataSet getWayBill_SubWayBillInfo(string swbSerialNum)
        {
            StringBuilder sbSQL = new StringBuilder("");
            string strWhereTmp = "";

            sbSQL.Append(" select distinct wbID from V_SubWayBill_WayBillFlow  ");

            if (swbSerialNum != "")
            {
                if (strWhereTmp != "")
                {
                    strWhereTmp = strWhereTmp + " and (swbSerialNum='" + swbSerialNum + "') ";
                }
                else
                {
                    strWhereTmp = strWhereTmp + " (swbSerialNum='" + swbSerialNum + "') ";
                }
            }

            if (strWhereTmp != "")
            {
                sbSQL.Append(" where  " + strWhereTmp);
            }

            DataSet ds = DBUtility.SqlServerHelper.Query(sbSQL.ToString());
            return ds;
        }

        /// <summary>
        /// 根据分单号查询此单的出入库信息
        /// </summary>
        /// <param name="InOutStoreBeginDate"></param>
        /// <param name="InOutStoreEndDate"></param>
        /// <param name="wbCompany"></param>
        /// <param name="wbSerialNum"></param>
        /// <param name="swbSerialNum"></param>
        /// <param name="swbNeedCheck"></param>
        /// <param name="order"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public DataSet getSubWayBillInOutStoreInfo(string swbSerialNum)
        {
            StringBuilder sbSQL = new StringBuilder("");

            sbSQL.Append(" select * from V_SubWayBill_WayBillFlow where swbSerialNum='" + swbSerialNum + "'");

            DataSet ds = DBUtility.SqlServerHelper.Query(sbSQL.ToString());
            return ds;
        }

        /// <summary>
        /// 根据分单号查询此单的出入库信息
        /// </summary>
        /// <param name="InOutStoreBeginDate"></param>
        /// <param name="InOutStoreEndDate"></param>
        /// <param name="wbCompany"></param>
        /// <param name="wbSerialNum"></param>
        /// <param name="swbSerialNum"></param>
        /// <param name="swbNeedCheck"></param>
        /// <param name="order"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public DataSet getTop1UnReleaseSubWayBill(string wbID)
        {
            StringBuilder sbSQL = new StringBuilder("");

            sbSQL.Append(" select * from V_WayBillFlow where status =1 and  wbID=" + wbID + " ORDER BY operateDate ");

            DataSet ds = DBUtility.SqlServerHelper.Query(sbSQL.ToString());
            return ds;
        }

        /// <summary>
        /// 根据分单号查询此单的出入库信息
        /// </summary>
        /// <param name="InOutStoreBeginDate"></param>
        /// <param name="InOutStoreEndDate"></param>
        /// <param name="wbCompany"></param>
        /// <param name="wbSerialNum"></param>
        /// <param name="swbSerialNum"></param>
        /// <param name="swbNeedCheck"></param>
        /// <param name="order"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public DataSet getTop1SubWayBill(string wbID)
        {
            StringBuilder sbSQL = new StringBuilder("");

            sbSQL.Append("  select * from V_WayBillFlow where IsOutStore =3 and  wbID=" + wbID + " order by OutStoreDate desc ");

            DataSet ds = DBUtility.SqlServerHelper.Query(sbSQL.ToString());
            return ds;
        }

        public DataSet TestSWBSerialNumInStore(string swbSerialNums)
        {
            StringBuilder sbSQL = new StringBuilder("");
            sbSQL.Append(" select swbSerialNum from V_WayBillFlow where Wbf_wbID=0 and Wbf_swbID=0 and swbSerialNum in (" + swbSerialNums + ")");

            DataSet ds = DBUtility.SqlServerHelper.Query(sbSQL.ToString());
            return ds;
        }

        public bool FillwbIDswbID(string swbSerialNum)
        {
            bool bOK = false;
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append(@"update WayBillFlow set Wbf_wbID=WBSWB.wbID,Wbf_swbID=WBSWB.swbID from WayBillFlow WBF,(select top 1 * from  V_WayBill_SubWayBill where swbSerialNum=@swbSerialNum order by wbStorageDate desc,wbID desc) WBSWB
                            where WBF.Wbf_wbID=0 and WBF.Wbf_swbID=0 and WBF.Wbf_swbSerialNum=@swbSerialNum and WBSWB.swbSerialNum=WBF.Wbf_swbSerialNum");
                SqlParameter[] parameters = {
                      
                        new SqlParameter("@swbSerialNum",SqlDbType.NVarChar,255)      
                                        };

                parameters[0].Value = swbSerialNum;

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

        /// <summary>
        /// 如果出现未预入库时入库或未预入库时出库，则将这次扫描到的单号保存一份在库存表中
        /// </summary>
        /// <param name="swbSerialNum"></param>
        /// <param name="iType"></param>
        /// <returns></returns>
        public bool AddRecordForNoForeStore(string swbSerialNum, string strOperator, int iType)
        {
            bool bOK = false;
            StringBuilder strSql = new StringBuilder();
            try
            {

                DataSet ds = DBUtility.SqlServerHelper.Query("select count(0) from WayBillFlow where Wbf_swbSerialNum='" + swbSerialNum + "' and Wbf_wbID=0 and Wbf_swbID=0");

                if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) != 0)//已经保存过
                {
                    DBUtility.SqlServerHelper.ExecuteSql("delete from  WayBillFlow  where Wbf_swbSerialNum='" + swbSerialNum + "' and Wbf_wbID=0 and Wbf_swbID=0");
                }

                strSql.Append(@"insert into WayBillFlow(Wbf_wbID,Wbf_swbID,Wbf_swbSerialNum,status,operateDate,ExceptionStatus,Operator)
                                values(0,0,@swbSerialNum,@status,GETDATE(),0,@Operator)");
                SqlParameter[] parameters = {
                      
                        new SqlParameter("@swbSerialNum",SqlDbType.NVarChar,255),
                         new SqlParameter("@status",SqlDbType.Int),
                         new SqlParameter("@Operator",SqlDbType.NVarChar,255),
                                        };

                parameters[0].Value = swbSerialNum;
                parameters[1].Value = iType;
                parameters[2].Value = strOperator;

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

        /// <summary>
        /// 根据总运单ID查询其是否已入库
        /// </summary>
        /// <param name="wbID"></param>
        /// <returns></returns>
        public bool IsInStore(string wbID)
        {
            bool bInStore = false;
            DataSet ds = null;
            DataTable dt = null;

            try
            {
                ds = DBUtility.SqlServerHelper.Query(@"  
                                        SELECT  COUNT( SWBF.wbID)
               
                                        FROM    dbo.WayBillFlow AS WBF
                                                INNER JOIN ( SELECT SWBF.wbID ,
                                                                    SWBF.WbfID ,
                                                                    SWBF.InOutStoreType
                                                             FROM   dbo.WayBillFlow AS WBF
                                                                    INNER JOIN ( SELECT VWBSW.wbImportType ,
                                                                                        VWBSW.swbReleaseFlag ,
                                                                                        WBF.status ,
                                                                                        WBF.IsOutStore ,
                                                                                        WBF.Wbf_wbID ,
                                                                                        WBF.Wbf_swbID ,
                                                                                        VWBSW.swbID ,
                                                                                        VWBSW.wbID ,
                                                                                        WBF.WbfID ,
                                                                                        CASE WHEN WBF.IsOutStore IS NULL
                                                                                             THEN CASE WBF.status
                                                                                              WHEN 1 THEN 1
                                                                                              ELSE 0
                                                                                              END
                                                                                             WHEN WBF.IsOutStore = 0
                                                                                             THEN CASE WBF.status
                                                                                              WHEN 1 THEN 1
                                                                                              ELSE 0
                                                                                              END
                                                                                             WHEN WBF.IsOutStore = -1
                                                                                             THEN CASE WBF.status
                                                                                              WHEN 1 THEN 1
                                                                                              ELSE 0
                                                                                              END
                                                                                             WHEN WBF.IsOutStore = 3
                                                                                             THEN 3
                                                                                        END AS InOutStoreType
                                                                                 FROM   dbo.V_WayBill_SubWayBill
                                                                                        AS VWBSW
                                                                                        INNER JOIN dbo.WayBillFlow
                                                                                        AS WBF ON VWBSW.swbID = WBF.Wbf_swbID
                                                                                              AND WBF.Wbf_wbID = VWBSW.wbID
                                                                               ) AS SWBF ON SWBF.WbfID = WBF.WbfID
                                                           ) AS SWBF ON SWBF.WbfID = WBF.WbfID
                                                           WHERE SWBF.InOutStoreType=1 and SWBF.wbID=
                                        " + wbID);
                if (ds != null)
                {
                    dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0][0].ToString()) > 0)
                        {
                            bInStore = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bInStore = false;
            }

            return bInStore;
        }

        /// <summary>
        /// 根据总运单ID获取出入库分单数信息
        /// </summary>
        /// <param name="wbID"></param>
        /// <returns></returns>
        public DataSet getInOutStoreCount(int wbID)
        {
            StringBuilder sbSQL = new StringBuilder("");

            sbSQL.AppendFormat(@"SELECT  COUNT(CASE WHEN InOutStoreType = 1
                                                        AND ( wbImportType = 0
                                                              OR ( wbImportType = 1
                                                                   AND swbReleaseFlag = 3
                                                                 )
                                                            ) THEN 1
                                              END) InOutStoreType1 ,
                                        COUNT(CASE WHEN InOutStoreType = 3 THEN 1
                                              END) InOutStoreType3
                                FROM    dbo.V_Source_WayBillFlow_Fast
                                WHERE   wbid = {0} ", wbID);

            DataSet ds = DBUtility.SqlServerHelper.Query(sbSQL.ToString());
            return ds;
        }

        public DataSet getAllInStoreSubWayBillID(int wbID)
        {
            StringBuilder sbSQL = new StringBuilder("");

            sbSQL.AppendFormat(@"SELECT   swbID
                                   FROM     dbo.V_Source_WayBillFlow_Fast
                                   WHERE    ( InOutStoreType = 1
                                              AND ( wbImportType = 0
                                                    OR ( wbImportType = 1
                                                         AND swbReleaseFlag = 3
                                                       )
                                                  )
                                            )
                                            AND wbid = {0}", wbID);

            DataSet ds = DBUtility.SqlServerHelper.Query(sbSQL.ToString());
            return ds;
        }

        public DataSet getInOutStoreCount()
        {
            StringBuilder sbSQL = new StringBuilder("");

            //            sbSQL.AppendFormat(@"SELECT  wbID ,
            //                                        COUNT(CASE WHEN InOutStoreType = 1
            //                                                        AND ( wbImportType = 0
            //                                                              OR ( wbImportType = 1
            //                                                                   AND swbReleaseFlag = 3
            //                                                                 )
            //                                                            ) THEN 1
            //                                              END) InOutStoreType1 ,
            //                                        COUNT(CASE WHEN InOutStoreType = 3 THEN 1
            //                                              END) InOutStoreType3
            //                                FROM    dbo.V_WayBillFlow
            //                                GROUP BY wbid");
            sbSQL.AppendFormat(@"SELECT  wbID ,
                                        COUNT(CASE WHEN InOutStoreType = 1
                                                        AND ( wbImportType = 0
                                                              OR ( wbImportType = 1
                                                                   AND swbReleaseFlag = 3
                                                                 )
                                                            ) THEN 1
                                              END) InOutStoreType1 ,
                                        COUNT(CASE WHEN InOutStoreType = 3 THEN 1
                                              END) InOutStoreType3
                                FROM    dbo.V_Source_WayBillFlow_Fast
                                GROUP BY wbid");
            DataSet ds = DBUtility.SqlServerHelper.Query(sbSQL.ToString());
            return ds;
        }
    }
}
