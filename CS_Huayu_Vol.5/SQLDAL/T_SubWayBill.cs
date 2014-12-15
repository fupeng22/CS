using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using DBUtility;
using Model;



namespace SQLDAL
{
    public class T_SubWayBill
    {
        public T_SubWayBill()
        {

        }
        //获取ID根据wbSerialNum
        public int GetSubWayBillID()
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Max(wbID) from Waybill");

            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }
        //获取放行件数
        public int GetReleseNum(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(swbID) from V_Distinct_SubWayBill");
            strSql.Append(" WHERE (swbNeedCheck != 3) and swb_wbID=" + wbID + "");
            //strSql.Append(" WHERE (swbNeedCheck = 0 or swbNeedCheck=2) and swb_wbID=" + wbID + "");


            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }
        //获取扣留件数
        public int GetSaveNum(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(swbID) from V_Distinct_SubWayBill");
            strSql.Append(" WHERE (swbNeedCheck = 3) and swb_wbID=" + wbID + "");


            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        //获取扣留件数
        public DataSet GetSave(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select swbID,swbSerialNum,swbDescription_CHN,swbNumber,swbWeight,swbActualWeight,swbValue,swbRecipients from V_Distinct_SubWayBill");
            strSql.Append(" WHERE (swbNeedCheck = 3) and swb_wbID=" + wbID + "");


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
        //新增
        public bool addSubWayBill(Model.M_SubWayBill model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into SubWaybill");
            strSql.Append(" (swb_wbID,swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,swbValue,swbMonetary,swbRecipients,swbCustomsCategory,swbValueDetail)");
            strSql.Append(" values (");
            strSql.Append("@swb_wbID,@swbSerialNum,@swbDescription_CHN,@swbDescription_ENG,@swbNumber,@swbWeight,@swbValue,@swbMonetary,@swbRecipients,@swbCustomsCategory,@swbValueDetail)");

            SqlParameter[] parameters = {
                   
                    new SqlParameter("@swb_wbID",SqlDbType.Int),
                    new SqlParameter("@swbSerialNum", SqlDbType.VarChar),
                    new SqlParameter("@swbDescription_CHN", SqlDbType.VarChar), 
                    new SqlParameter("@swbDescription_ENG", SqlDbType.VarChar),
                    new SqlParameter("@swbNumber", SqlDbType.Int),
                    new SqlParameter("@swbWeight", SqlDbType.Real),
                    new SqlParameter("@swbValue", SqlDbType.Real),
                    new SqlParameter("@swbMonetary", SqlDbType.VarChar),
                    new SqlParameter("@swbRecipients", SqlDbType.VarChar),
                    new SqlParameter("@swbCustomsCategory", SqlDbType.VarChar),
                    new SqlParameter("@swbValueDetail", SqlDbType.VarChar)
            };

            parameters[0].Value = model.Swb_wbID;
            parameters[1].Value = model.SwbSerialNum;
            parameters[2].Value = model.SwbDescription_CHN;
            parameters[3].Value = model.SwbDescription_ENG;
            parameters[4].Value = model.SwbNumber;
            parameters[5].Value = model.SwbWeight;
            parameters[6].Value = model.SwbValue;
            parameters[7].Value = model.SwbMonetary;
            parameters[8].Value = model.SwbRecipients;
            parameters[9].Value = model.SwbCustomsCategory;
            parameters[10].Value = model.swbValueDetail == null ? "" : model.swbValueDetail;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                //更新库存表中先前插入的未预入库记录信息
                //new T_WayBillFlow().FillwbIDswbID(model.SwbSerialNum);

                return true;
            }
            else
            {
                return false;

            }

        }

        //根据swSeralNum获取WayBill
        public DataSet GetSubWayBill(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from V_Distinct_SubWayBill");
            strSql.Append(" where swb_wbID=" + wbID + " and (swbNeedCheck=0 or swbNeedCheck=1)");
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


        //获取放行件数
        public int GetNeedCheckNum(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(swbID) from V_Distinct_SubWayBill");
            strSql.Append(" WHERE (swbNeedCheck = 1) and swb_wbID=" + wbID + "");

            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }


        //更新

        public bool upDateSwbNeedCheck(int swID, string strParm, int index, int swbNeedCheck)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update SubWaybill ");
            strSql.Append("set swbNeedCheck=@swbNeedCheck");
            //全部更新
            switch (index)
            {
                case 0:
                    strSql.Append(" where swb_wbID=" + swID + " ");
                    break;
                case -1:
                    strSql.Append(" where swb_wbID=" + swID + " ");
                    strSql.Append(" and swbID not in (" + strParm + ")");
                    break;
                case 1:
                    strSql.Append(" where swb_wbID=" + swID + " ");
                    strSql.Append(" and swbID  in (" + strParm + ")");
                    break;
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@swbNeedCheck", SqlDbType.Int)
                                      };

            parameters[0].Value = swbNeedCheck;
            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }



        }


        //获取称重总重量
        public double GetTotalActualWeight(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select SUM(swbActualWeight) from V_Distinct_SubWayBill");
            strSql.Append(" where swb_wbID=" + wbID + "");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                return double.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }


        //获取称重总重量
        public int GetActualSubNum(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(swbID) from V_Distinct_SubWayBill");
            strSql.Append(" where swb_wbID=" + wbID + "");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }


        //获取实际放行数量
        public int GetActualReleseNum(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            //strSql.Append("select count(swbNeedCheck) from V_Distinct_SubWayBill");
            //strSql.Append(" where swb_wbID=" + wbID + " and (swbNeedCheck=0 or swbNeedCheck=2) and swbSortingTime is not null");
//            strSql.AppendFormat(@"SELECT  
//                                          COUNT(CASE WHEN ( ( wbImportType = 0
//                                                        AND (swbNeedCheck = 0 OR swbNeedCheck=2)
//                                                      )
//                                                      OR ( wbImportType = 1
//                                                           AND swbReleaseFlag = 3
//                                                         )
//                                                    ) THEN 1
//                                          END) AS ReleaseNum
//                            FROM    dbo.V_WayBill_SubWayBill
//                            WHERE   wbID = {0}",wbID);
            strSql.AppendFormat(@"SELECT  COUNT(CASE WHEN ( ( T.wbImportType = 0
                                                            AND ( T.swbNeedCheck = 0
                                                                  OR T.swbNeedCheck = 2
                                                                )
                                                          )
                                                          OR ( T.wbImportType = 1
                                                               AND T.swbReleaseFlag = 3
                                                             )
                                                        ) THEN 1
                                              END) AS ReleaseNum
                                FROM    ( SELECT DISTINCT
                                                    WB.wbID ,
                                                    WB.wbImportType ,
                                                    SWB.swbNeedCheck ,
                                                    SWB.swbReleaseFlag
                                          FROM      ( SELECT    wbID ,
                                                                wbImportType
                                                      FROM      dbo.Waybill
                                                      WHERE     ( wbID IN ( SELECT  MAX(wbID) AS wbID
                                                                            FROM    dbo.Waybill AS WB
                                                                            GROUP BY wbSerialNum ) )
                                                    ) AS WB
                                                    INNER JOIN ( SELECT swbID ,
                                                                        swb_wbID ,
                                                                        swbNeedCheck ,
                                                                        swbReleaseFlag
                                                                 FROM   dbo.SubWaybill
                                                                 WHERE  ( swbID IN (
                                                                          SELECT    MAX(swbID) AS swbID
                                                                          FROM      dbo.SubWaybill AS SWB
                                                                          GROUP BY  swb_wbID ,
                                                                                    swbSerialNum ) )
                                                               ) AS SWB ON SWB.swb_wbID = WB.wbID
                                        ) T
                                WHERE   T.wbID = {0}", wbID);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public int GetActualRejectNum(int wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(swbNeedCheck) from V_Distinct_SubWayBill");
            strSql.Append(" where swb_wbID=" + wbID + " and  swbNeedCheck=99 ");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        //获取实际扣留数量
        public int GetActualNotReleseNum(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            //strSql.Append("select count(swbNeedCheck) from V_Distinct_SubWayBill");
            //strSql.Append(" where swb_wbID=" + wbID + " and  swbNeedCheck=3 ");
//            strSql.AppendFormat(@"SELECT  COUNT(CASE WHEN ( ( wbImportType = 0
//                                                        AND swbNeedCheck = 3
//                                                      )
//                                                      OR ( wbImportType = 1
//                                                           AND swbReleaseFlag <>3
//                                                         )
//                                                    ) THEN 1
//                                          END) AS unReleaseNum
//                            FROM    dbo.V_WayBill_SubWayBill
//                            WHERE   wbID = {0}", wbID);
            strSql.AppendFormat(@"SELECT  COUNT(CASE WHEN ( ( T.wbImportType = 0
                                                            AND T.swbNeedCheck = 3
                                                          )
                                                          OR ( T.wbImportType = 1
                                                               AND T.swbReleaseFlag <> 3
                                                             )
                                                        ) THEN 1
                                              END) AS unReleaseNum
                                FROM    ( SELECT DISTINCT
                                                    WB.wbID ,
                                                    WB.wbImportType ,
                                                    SWB.swbNeedCheck ,
                                                    SWB.swbReleaseFlag
                                          FROM      ( SELECT    wbID ,
                                                                wbImportType
                                                      FROM      dbo.Waybill
                                                      WHERE     ( wbID IN ( SELECT  MAX(wbID) AS wbID
                                                                            FROM    dbo.Waybill AS WB
                                                                            GROUP BY wbSerialNum ) )
                                                    ) AS WB
                                                    INNER JOIN ( SELECT swbID ,
                                                                        swb_wbID ,
                                                                        swbNeedCheck ,
                                                                        swbReleaseFlag
                                                                 FROM   dbo.SubWaybill
                                                                 WHERE  ( swbID IN (
                                                                          SELECT    MAX(swbID) AS swbID
                                                                          FROM      dbo.SubWaybill AS SWB
                                                                          GROUP BY  swb_wbID ,
                                                                                    swbSerialNum ) )
                                                               ) AS SWB ON SWB.swb_wbID = WB.wbID
                                        ) T
                                WHERE   T.wbID = {0}", wbID);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public DataSet GetReleaseNum_UnReleaseNumByWbID(int wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT  wbID ,
                                    COUNT(CASE WHEN ( ( wbImportType = 0
                                                        AND ( swbNeedCheck = 0
                                                              OR swbNeedCheck = 2
                                                            )
                                                      )
                                                      OR ( wbImportType = 1
                                                           AND swbReleaseFlag = 3
                                                         )
                                                    ) THEN 1
                                          END) AS ReleaseNum ,
                                    COUNT(CASE WHEN ( ( wbImportType = 0
                                                        AND swbNeedCheck = 3
                                                      )
                                                      OR ( wbImportType = 1
                                                           AND swbReleaseFlag <> 3
                                                         )
                                                    ) THEN 1
                                          END) AS unReleaseNum ,
                                    COUNT(CASE WHEN swbNeedCheck = 99 THEN 1
                                          END) AS RejectNum
                            FROM    dbo.V_WayBill_SubWayBill
                            GROUP BY wbID  HAVING wbID={0}", wbID);
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

        //获取上机数量

        public int GetActualNotProNum(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(swb_wbID) from V_Distinct_SubWayBill");
            strSql.Append(" where swbSortingTime is null and swb_wbID=" + wbID + "");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 根据总运单ID获取其子运单信息
        /// </summary>
        /// <param name="wbID"></param>
        /// <returns></returns>
        public DataSet GetSubWayBillInfoBywbID(int wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from V_Distinct_SubWayBill where swb_wbID={0}", wbID);
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

        /// <summary>
        /// 获取所有子运单信息
        /// </summary>
        /// <param name="wbID"></param>
        /// <returns></returns>
        public DataSet GetAllSubWayBillInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from V_Distinct_SubWayBill");
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

        /// <summary>
        /// 根据总运单ID得到该总运单下的所有子运单的汇总信息，如：总重量、总数量、实际总重量
        /// </summary>
        /// <param name="wbID"></param>
        /// <returns></returns>
        public DataSet GetSubWayBillSumInfo(int wbID)
        {

            StringBuilder strSql = new StringBuilder();
//            strSql.AppendFormat(@"select  swb_wbID,
//                                SUM(case swbActualNumber when null then 0 else  swbActualNumber end) as swbTotalActualNumber,
//                                COUNT(swbID) as swbTotalNumber,
//                                SUM(case swbWeight when null then 0 else  cast(round(swbWeight,2) as  numeric(30,2)) end) as swbTotalWeight, 
//                                SUM(case swbActualWeight when null then 0 else  cast(round(swbActualWeight,2) as  numeric(30,2)) end) as swbTotalActualWeight
//                                from V_Distinct_SubWayBill  group by swb_wbID  having swb_wbID={0}", wbID);
            strSql.AppendFormat(@"SELECT  T.swb_wbID ,
                                            SUM(CASE T.swbActualNumber
                                                  WHEN NULL THEN 0
                                                  ELSE T.swbActualNumber
                                                END) AS swbTotalActualNumber ,
                                            COUNT(T.swbID) AS swbTotalNumber ,
                                            SUM(CASE T.swbWeight
                                                  WHEN NULL THEN 0
                                                  ELSE CAST(ROUND(T.swbWeight, 2) AS NUMERIC(30, 2))
                                                END) AS swbTotalWeight ,
                                            SUM(CASE T.swbActualWeight
                                                  WHEN NULL THEN 0
                                                  ELSE CAST(ROUND(T.swbActualWeight, 2) AS NUMERIC(30, 2))
                                                END) AS swbTotalActualWeight
                                    FROM    ( SELECT    swbID ,
                                                        swb_wbID ,
                                                        swbWeight ,
                                                        swbActualWeight ,
                                                        swbActualNumber
                                              FROM      dbo.SubWaybill
                                              WHERE     ( swbID IN ( SELECT MAX(swbID) AS swbID
                                                                     FROM   dbo.SubWaybill AS SWB
                                                                     GROUP BY swb_wbID ,
                                                                            swbSerialNum ) )
                                            ) T
                                    GROUP BY T.swb_wbID
                                    HAVING  T.swb_wbID = {0}", wbID);
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

        public Boolean TestExistSWBSerialNumInOtherWayBill(string wbSerialNum, string swbSerialNum)
        {
            Boolean bExist = false;
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select count(0) from V_WayBill_SubWayBill where swbSerialNum='{0}' and wbSerialNum<>'{1}'", swbSerialNum, wbSerialNum);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) > 0)
            {
                bExist = true;
            }
            return bExist;
        }

        //更新

        /// <summary>
        /// 删除指定ID的分运单信息
        /// </summary>
        /// <param name="swID"></param>
        /// <param name="strParm"></param>
        /// <param name="index"></param>
        /// <param name="swbNeedCheck"></param>
        /// <returns></returns>
        public bool DeleteSubWayBillByID(string swbIDs)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from SubWaybill  where swbID in (" + swbIDs + ")");

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public DataSet GetSubWayBillInfo(string wbSerialNum,string swbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from V_WayBill_SubWayBill where wbSerialNum='" +wbSerialNum+ "' and  swbSerialNum='"+swbSerialNum+"'");
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

        /// <summary>
        /// 删除指定总运单ID的分运单信息
        /// </summary>
        /// <param name="swID"></param>
        /// <param name="strParm"></param>
        /// <param name="index"></param>
        /// <param name="swbNeedCheck"></param>
        /// <returns></returns>
        public bool DeleteSubWayBillByWBID(string wbIDs)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from SubWaybill  where swb_wbID in (" + wbIDs + ")");

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool upDateSwbNeedCheck(string swbIds, string iNeedCheck)
        {
            StringBuilder strSql = new StringBuilder();
            switch (iNeedCheck)
            {
                case "2":
                    strSql.Append("update SubWaybill ");
                    strSql.Append("set ReleaseDate=getdate(),swbReleaseFlag=3,swbNeedCheck=" + iNeedCheck + " where swbId in (" + swbIds + ")");
                    break;
                case "3":
                    strSql.Append("update SubWaybill ");
                    strSql.Append("set DetainDate=getdate(),swbReleaseFlag=0,swbNeedCheck=" + iNeedCheck + " where swbId in (" + swbIds + ")");
                    break;
                case "4":
                    strSql.Append("update SubWaybill ");
                    strSql.Append("set InHandleDate=getdate(),swbNeedCheck=" + iNeedCheck + " where swbId in (" + swbIds + ")");
                    break;
                case "99":
                    strSql.Append("update SubWaybill ");
                    strSql.Append("set RejectDate=getdate(),swbNeedCheck=" + iNeedCheck + " where swbId in (" + swbIds + ")");
                    break;
                default:
                    break;
            }
            

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RejectSubWayBill(string swbIds, string Operator)
        {
            bool bOK = false;
            ArrayList arrSQLList=new ArrayList();
            DataSet ds = DBUtility.SqlServerHelper.Query("select * from subwaybill where swbId in(" + swbIds + ")");
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        arrSQLList.Add(string.Format(@"INSERT INTO RejectSubWayBill
                            ([swbID]
                            ,[swb_wbID]
                            ,[swbSerialNum]
                            ,[swbDescription_CHN]
                            ,[swbDescription_ENG]
                            ,[swbNumber]
                            ,[swbWeight]
                            ,[swbActualWeight]
                            ,[swbSortingTime]
                            ,[swbNeedCheck]
                            ,[swbXRayWebSiteIPAndPort]
                            ,[swbImageLocalPath]
                            ,[swbCheckDescription]
                            ,[swbDelFlag]
                            ,[swbValue]
                            ,[swbMonetary]
                            ,[swbRecipients]
                            ,[swbCustomsCategory]
                            ,[swbValueDetail]
                            ,[DetainDate]
                            ,[ReleaseDate]
                            ,[InHandleDate]
                            ,[swbActualNumber]
                            ,[RejectDate]
                            ,[Operator]
                            ,[mMemo])
                        VALUES
                            ({0}
                            ,{1}
                            ,'{2}'
                            ,'{3}'
                            ,'{4}'
                            ,{5}
                            ,{6}
                            ,{7}
                            ,'{8}'
                            ,{9}
                            ,'{10}'
                            ,'{11}'
                            ,'{12}'
                            ,{13}
                            ,{14}
                            ,'{15}'
                            ,'{16}'
                            ,'{17}'
                            ,'{18}'
                            ,'{19}'
                            ,'{20}'
                            ,'{21}'
                            ,{22}
                            ,getDate()
                            ,'{23}'
                            ,'')", dt.Rows[i]["swbID"] == DBNull.Value ? "-1" : dt.Rows[i]["swbID"].ToString()
                             , dt.Rows[i]["swb_wbID"] == DBNull.Value ? "-1" : dt.Rows[i]["swb_wbID"].ToString()
                             , dt.Rows[i]["swbSerialNum"] == DBNull.Value ? "" : dt.Rows[i]["swbSerialNum"].ToString()
                             , dt.Rows[i]["swbDescription_CHN"] == DBNull.Value ? "" : dt.Rows[i]["swbDescription_CHN"].ToString()
                             , dt.Rows[i]["swbDescription_ENG"] == DBNull.Value ? "" : dt.Rows[i]["swbDescription_ENG"].ToString()
                             , dt.Rows[i]["swbNumber"] == DBNull.Value ? "0" : dt.Rows[i]["swbNumber"].ToString()
                             , dt.Rows[i]["swbWeight"] == DBNull.Value ? "0" : dt.Rows[i]["swbWeight"].ToString()
                             , dt.Rows[i]["swbActualWeight"] == DBNull.Value ? "0" : dt.Rows[i]["swbActualWeight"].ToString()
                             , dt.Rows[i]["swbSortingTime"] == null ? "" : dt.Rows[i]["swbSortingTime"].ToString()
                             , dt.Rows[i]["swbNeedCheck"] == DBNull.Value ? "-1" : dt.Rows[i]["swbNeedCheck"].ToString()
                             , dt.Rows[i]["swbXRayWebSiteIPAndPort"] == DBNull.Value ? "" : dt.Rows[i]["swbXRayWebSiteIPAndPort"].ToString()
                             , dt.Rows[i]["swbImageLocalPath"] == DBNull.Value ? "" : dt.Rows[i]["swbImageLocalPath"].ToString()
                             , dt.Rows[i]["swbCheckDescription"] == DBNull.Value ? "" : dt.Rows[i]["swbCheckDescription"].ToString()
                             , dt.Rows[i]["swbDelFlag"] == DBNull.Value ? "-1" : dt.Rows[i]["swbDelFlag"].ToString()
                             , dt.Rows[i]["swbValue"] == DBNull.Value ? "0" : dt.Rows[i]["swbValue"].ToString()
                             , dt.Rows[i]["swbMonetary"] == DBNull.Value ? "" : dt.Rows[i]["swbMonetary"].ToString()
                             , dt.Rows[i]["swbRecipients"] == DBNull.Value ? "" : dt.Rows[i]["swbRecipients"].ToString()
                             , dt.Rows[i]["swbCustomsCategory"] == DBNull.Value ? "" : dt.Rows[i]["swbCustomsCategory"].ToString()
                             , dt.Rows[i]["swbValueDetail"] == DBNull.Value ? "" : dt.Rows[i]["swbValueDetail"].ToString()
                             , dt.Rows[i]["DetainDate"] == null ? "" : dt.Rows[i]["DetainDate"].ToString()
                             , dt.Rows[i]["ReleaseDate"] == null ? "" : dt.Rows[i]["ReleaseDate"].ToString()
                             , dt.Rows[i]["InHandleDate"] == null ? "" : dt.Rows[i]["InHandleDate"].ToString()
                             , dt.Rows[i]["swbActualNumber"] == DBNull.Value ? "0" : dt.Rows[i]["swbActualNumber"].ToString()
                             , Operator
                             ));
                    }
                    arrSQLList.Add("delete from subwaybill where swbId in(" + swbIds + ")");
                    SqlServerHelper.ExecuteSqlTran(arrSQLList);
                    bOK = true;
                }
            }
            return bOK;
        }

        public bool SaveInStore(string swbIds, string Operator)
        {
            bool bOK = false;
            ArrayList arrSQLList = new ArrayList();
            DataSet ds = DBUtility.SqlServerHelper.Query("select * from subwaybill where swbId in(" + swbIds + ")");
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        arrSQLList.Add(string.Format(@"INSERT INTO RejectSubWayBill
                            ([swbID]
                            ,[swb_wbID]
                            ,[swbSerialNum]
                            ,[swbDescription_CHN]
                            ,[swbDescription_ENG]
                            ,[swbNumber]
                            ,[swbWeight]
                            ,[swbActualWeight]
                            ,[swbSortingTime]
                            ,[swbNeedCheck]
                            ,[swbXRayWebSiteIPAndPort]
                            ,[swbImageLocalPath]
                            ,[swbCheckDescription]
                            ,[swbDelFlag]
                            ,[swbValue]
                            ,[swbMonetary]
                            ,[swbRecipients]
                            ,[swbCustomsCategory]
                            ,[swbValueDetail]
                            ,[DetainDate]
                            ,[ReleaseDate]
                            ,[InHandleDate]
                            ,[swbActualNumber]
                            ,[RejectDate]
                            ,[Operator]
                            ,[mMemo])
                        VALUES
                            ({0}
                            ,{1}
                            ,'{2}'
                            ,'{3}'
                            ,'{4}'
                            ,{5}
                            ,{6}
                            ,{7}
                            ,'{8}'
                            ,{9}
                            ,'{10}'
                            ,'{11}'
                            ,'{12}'
                            ,{13}
                            ,{14}
                            ,'{15}'
                            ,'{16}'
                            ,'{17}'
                            ,'{18}'
                            ,'{19}'
                            ,'{20}'
                            ,'{21}'
                            ,{22}
                            ,getDate()
                            ,'{23}'
                            ,'')", dt.Rows[i]["swbID"] == DBNull.Value ? "-1" : dt.Rows[i]["swbID"].ToString()
                             , dt.Rows[i]["swb_wbID"] == DBNull.Value ? "-1" : dt.Rows[i]["swb_wbID"].ToString()
                             , dt.Rows[i]["swbSerialNum"] == DBNull.Value ? "" : dt.Rows[i]["swbSerialNum"].ToString()
                             , dt.Rows[i]["swbDescription_CHN"] == DBNull.Value ? "" : dt.Rows[i]["swbDescription_CHN"].ToString()
                             , dt.Rows[i]["swbDescription_ENG"] == DBNull.Value ? "" : dt.Rows[i]["swbDescription_ENG"].ToString()
                             , dt.Rows[i]["swbNumber"] == DBNull.Value ? "0" : dt.Rows[i]["swbNumber"].ToString()
                             , dt.Rows[i]["swbWeight"] == DBNull.Value ? "0" : dt.Rows[i]["swbWeight"].ToString()
                             , dt.Rows[i]["swbActualWeight"] == DBNull.Value ? "0" : dt.Rows[i]["swbActualWeight"].ToString()
                             , dt.Rows[i]["swbSortingTime"] == null ? "" : dt.Rows[i]["swbSortingTime"].ToString()
                             , dt.Rows[i]["swbNeedCheck"] == DBNull.Value ? "-1" : dt.Rows[i]["swbNeedCheck"].ToString()
                             , dt.Rows[i]["swbXRayWebSiteIPAndPort"] == DBNull.Value ? "" : dt.Rows[i]["swbXRayWebSiteIPAndPort"].ToString()
                             , dt.Rows[i]["swbImageLocalPath"] == DBNull.Value ? "" : dt.Rows[i]["swbImageLocalPath"].ToString()
                             , dt.Rows[i]["swbCheckDescription"] == DBNull.Value ? "" : dt.Rows[i]["swbCheckDescription"].ToString()
                             , dt.Rows[i]["swbDelFlag"] == DBNull.Value ? "-1" : dt.Rows[i]["swbDelFlag"].ToString()
                             , dt.Rows[i]["swbValue"] == DBNull.Value ? "0" : dt.Rows[i]["swbValue"].ToString()
                             , dt.Rows[i]["swbMonetary"] == DBNull.Value ? "" : dt.Rows[i]["swbMonetary"].ToString()
                             , dt.Rows[i]["swbRecipients"] == DBNull.Value ? "" : dt.Rows[i]["swbRecipients"].ToString()
                             , dt.Rows[i]["swbCustomsCategory"] == DBNull.Value ? "" : dt.Rows[i]["swbCustomsCategory"].ToString()
                             , dt.Rows[i]["swbValueDetail"] == DBNull.Value ? "" : dt.Rows[i]["swbValueDetail"].ToString()
                             , dt.Rows[i]["DetainDate"] == null ? "" : dt.Rows[i]["DetainDate"].ToString()
                             , dt.Rows[i]["ReleaseDate"] == null ? "" : dt.Rows[i]["ReleaseDate"].ToString()
                             , dt.Rows[i]["InHandleDate"] == null ? "" : dt.Rows[i]["InHandleDate"].ToString()
                             , dt.Rows[i]["swbActualNumber"] == DBNull.Value ? "0" : dt.Rows[i]["swbActualNumber"].ToString()
                             , Operator
                             ));
                    }
                    arrSQLList.Add("delete from subwaybill where swbId in(" + swbIds + ")");
                    SqlServerHelper.ExecuteSqlTran(arrSQLList);
                    bOK = true;
                }
            }
            return bOK;
        }

        /// <summary>
        /// 根据总运单号查询其所有退货分运单信息
        /// </summary>
        /// <param name="wbSerialNum"></param>
        /// <returns></returns>
        public DataSet getRejectSubWayBillInfo(string wbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from V_WayBill_RejectSubWayBill where wbSerialNum='{0}'", wbSerialNum);
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

        /// <summary>
        /// 根据总运单ID查询总运单子运单信息
        /// </summary>
        /// <param name="wbID"></param>
        /// <returns></returns>
        public DataSet getWayBill_SubWayBill(string wbID,int iNeedCheck)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from V_WayBill_SubWayBill where wbID={0}", wbID);
            switch (iNeedCheck)
            {
                case -1:
                    break;
                default:
                    strSql.Append(" and swbNeedCheck in ("+iNeedCheck.ToString()+")");
                    break;
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

        /// <summary>
        /// 根据总运单ID查询总运单子运单信息
        /// </summary>
        /// <param name="wbID"></param>
        /// <returns></returns>
        public DataSet getWayBill_SubWayBill_UnRelease(string wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT  *
                                FROM    dbo.V_WayBill_SubWayBill
                                WHERE   (( wbImportType = 0
                                          AND swbNeedCheck = 3
                                        )
                                        OR ( wbImportType = 1
                                             AND swbReleaseFlag <>3
                                           ))
                                 and wbID={0}", wbID);
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

        /// <summary>
        /// 根据总运单ID查询总运单子运单信息
        /// </summary>
        /// <param name="wbID"></param>
        /// <returns></returns>
        public DataSet getWayBill_SubWayBill_Release(string wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT  *
                            FROM    dbo.V_WayBill_SubWayBill
                            WHERE   ( ( wbImportType = 0
                                        AND ( swbNeedCheck = 0
                                              OR swbNeedCheck = 2
                                            )
                                      )
                                      OR ( wbImportType = 1
                                           AND swbReleaseFlag = 3
                                         )
                                    )
                                 and wbID={0}", wbID);
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

        /// <summary>
        /// 根据分运单单号信息查询分运单信息
        /// </summary>
        /// <param name="strSerialNums"></param>
        /// <returns></returns>
        public DataSet getWayBill_SubWayBill(string wbID,string strSerialNums)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from V_WayBill_SubWayBill where  wbID={0} and swbSerialNum in ({1})",wbID, strSerialNums);
           
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

        public DataSet getForeSubWayBill(int wbId)
        {
            StringBuilder strSql = new StringBuilder();
            //strSql.AppendFormat(@"select * from V_Fore_WayBill_Fast where ((swbInspectionFlag=3 and wbImportType=1) or (wbImportType is null or wbImportType=0)) and wbID={0}", wbId);
            //strSql.AppendFormat(@"select * from V_Fore_WayBill_Fast where  wbID={0}", wbId);
//            strSql.AppendFormat(@"SELECT  wbImportType ,
//                                            swbID ,
//                                            wbID ,
//                                            swbInspectionFlag
//                                    FROM    ( SELECT DISTINCT
//                                                        SWB.swbID ,
//                                                        WB.wbID ,
//                                                        wbImportType ,
//                                                        swbInspectionFlag
//                                              FROM      WayBill AS WB
//                                                        INNER JOIN ( SELECT swbID ,
//                                                                            swb_wbID ,
//                                                                            swbInspectionFlag
//                                                                     FROM   dbo.SubWaybill
//                                                                     WHERE  ( swbID IN (
//                                                                              SELECT    MAX(swbID) AS swbID
//                                                                              FROM      dbo.SubWaybill AS SWB WHERE swb_wbID={0}
//                                                                              GROUP BY  swb_wbID ,
//                                                                                        swbSerialNum ) )
//                                                                   ) AS SWB ON SWB.swb_wbID = WB.wbID
//                                            ) AS VWBSWB
//                                    WHERE   ( swbID NOT IN (
//                                              SELECT    SWB.swbID
//                                              FROM      ( SELECT DISTINCT
//                                                                    SWB.swbID ,
//                                                                    WB.wbID ,
//                                                                    wbImportType ,
//                                                                    swbInspectionFlag
//                                                          FROM      WayBill AS WB
//                                                                    INNER JOIN ( SELECT swbID ,
//                                                                                        swb_wbID ,
//                                                                                        swbInspectionFlag
//                                                                                 FROM   dbo.SubWaybill
//                                                                                 WHERE  ( swbID IN (
//                                                                                          SELECT  MAX(swbID) AS swbID
//                                                                                          FROM    dbo.SubWaybill
//                                                                                                  AS SWB WHERE swb_wbID={0}
//                                                                                          GROUP BY swb_wbID ,
//                                                                                                  swbSerialNum ) )
//                                                                               ) AS SWB ON SWB.swb_wbID = WB.wbID
//                                                        ) AS SWB
//                                                        INNER JOIN dbo.WayBillFlow AS WBF ON WBF.Wbf_swbID = SWB.swbID ) )
//                                    ",wbId);
            strSql.AppendFormat(@"SELECT  wbImportType ,
                                            swbID ,
                                            wbID ,
                                            swbInspectionFlag
                                    FROM    ( SELECT DISTINCT
                                                        SWB.swbID ,
                                                        WB.wbID ,
                                                        wbImportType ,
                                                        swbInspectionFlag
                                              FROM      WayBill AS WB
                                                        INNER JOIN ( SELECT swbID ,
                                                                            swb_wbID ,
                                                                            swbInspectionFlag
                                                                     FROM   dbo.SubWaybill T1
                                                                     WHERE  EXISTS ( SELECT MAX(swbID) AS swbID
                                                                                     FROM   dbo.SubWaybill AS SWB
                                                                                     WHERE  swb_wbID = {0}
                                                                                            AND SWB.swbID = T1.swbID
                                                                                     GROUP BY swb_wbID ,
                                                                                            swbSerialNum )
                                                                   ) AS SWB ON SWB.swb_wbID = WB.wbID
                                            ) AS VWBSWB
                                    WHERE   NOT EXISTS ( SELECT SWB.swbID
                                                         FROM   ( SELECT DISTINCT
                                                                            SWB.swbID ,
                                                                            WB.wbID ,
                                                                            wbImportType ,
                                                                            swbInspectionFlag
                                                                  FROM      WayBill AS WB
                                                                            INNER JOIN ( SELECT swbID ,
                                                                                                swb_wbID ,
                                                                                                swbInspectionFlag
                                                                                         FROM   dbo.SubWaybill T2
                                                                                         WHERE  EXISTS ( SELECT
                                                                                                  MAX(swbID) AS swbID
                                                                                                  FROM
                                                                                                  dbo.SubWaybill
                                                                                                  AS SWB
                                                                                                  WHERE
                                                                                                  swb_wbID = {0}
                                                                                                  AND SWB.swbID = T2.swbID
                                                                                                  GROUP BY swb_wbID ,
                                                                                                  swbSerialNum )
                                                                                       ) AS SWB ON SWB.swb_wbID = WB.wbID
                                                                ) AS SWB
                                                                INNER JOIN dbo.WayBillFlow AS WBF ON WBF.Wbf_swbID = SWB.swbID
                                                         WHERE  VWBSWB.swbID = SWB.swbID ) ", wbId);
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

        public DataSet getWayBillSubWayBill(string swbIds)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from V_WayBill_SubWayBill where  swbID in ({0})", swbIds);

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

        /// <summary>
        /// 根据主运单得其相关信息
        /// </summary>
        /// <param name="strSerialNums"></param>
        /// <returns></returns>
        public DataSet getWayBillSumInfoBywbID(string wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT  COUNT(0) AS wbSubNumber_Custom ,
                                        COUNT(CASE WHEN ( ( wbImportType = 0
                                                            AND swbNeedCheck = 3
                                                          )
                                                          OR ( wbImportType = 1
                                                               AND swbReleaseFlag <>3
                                                             )
                                                        ) THEN 1
                                              END) AS wbUnReleaseCount_Custom ,
                                        COUNT(CASE WHEN ( ( ( wbImportType = 0
                                                              AND ( swbNeedCheck = 0
                                                                    OR swbNeedCheck = 2
                                                                  )
                                                            )
                                                            OR ( wbImportType = 1
                                                                 AND swbReleaseFlag = 3
                                                               )
                                                          )
                                                          AND swbSortingTime IS NOT NULL
                                                        ) THEN 1
                                              END) AS wbReleaseCount_Custom ,
                                        COUNT(CASE WHEN swbSortingTime IS NULL THEN 1
                                              END) AS wbNotProCount_Custom 
                                FROM    dbo.V_WayBill_SubWayBill
                                GROUP BY   wbID  Having wbID={0}", wbID);

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
