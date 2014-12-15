using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using DBUtility;
using Model;

namespace SQLDAL
{


    public class T_WayBill
    {
        //是否已经存在该总运单
        public bool ExistWbSerialNum(string wbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(wbSerialNum) from V_Distinct_WayBill");
            strSql.Append(" where wbSerialNum='" + wbSerialNum + "'");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());

            if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //获取ID根据wbSerialNum
        public int GetWayBillID(string wbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select wbID from V_Distinct_WayBill");
            strSql.Append(" where wbSerialNum='" + wbSerialNum + "'");
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

        //获取WayBill (wbStatus = 0 or wbStatus=1)
        public DataSet GetWayBill()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT wbSerialNum, wbTotalNumber, wbTotalWeight, wbCompany, wbStorageDate, wbID,wbStatus");
            strSql.Append(" FROM V_Distinct_WayBill WHERE (wbStatus = 0 or wbStatus=1) ORDER BY wbID DESC");

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

        public DataSet GetWayBill(string wbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM V_Distinct_WayBill WHERE wbSerialNum='" + wbSerialNum + "'");

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

        //获取WayBill (wbStatus=1)  正在查验货物
        public DataSet GetChekingWayBill()
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT wbSerialNum, wbSubNumber, wbTotalWeight, wbCompany, wbStorageDate, wbID,wbStatus");
            strSql.Append(" FROM V_Distinct_WayBill WHERE  wbStatus=1 or wbStatus=2 ORDER BY wbID DESC");

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

        public DataSet GetChekWayBill()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT wbSerialNum, wbSubNumber, wbTotalWeight, wbCompany, wbStorageDate, wbID,wbStatus");
            strSql.Append(" FROM V_Distinct_WayBill WHERE  wbStatus=0 ORDER BY wbID DESC");

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

        //新增主运单
        public bool addWayBill(Model.M_WayBill model, string company)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Waybill");
            strSql.Append(" (wbSerialNum,wbTotalNumber,wbTotalWeight,wbVoyage,wbIOmark,wbChinese,wbEnglish,wbSubNumber,wbTransportMode,wbEntryDate,wbSRport,wbPortCode,wbStorageDate,wbCompany,wbStatus,wbImportType)");
            strSql.Append(" values (");
            strSql.Append("@wbSerialNum,@wbTotalNumber,@wbTotalWeight,@wbVoyage,@wbIOmark,@wbChinese,@wbEnglish,@wbSubNumber,@wbTransportMode,@wbEntryDate,@wbSRport,@wbPortCode,@wbStorageDate,@wbCompany,@wbStatus,@wbImportType)");

            SqlParameter[] parameters = {
                    new SqlParameter("@wbSerialNum",SqlDbType.VarChar),
                    new SqlParameter("@wbTotalNumber",SqlDbType.Int ),
                    new SqlParameter("@wbTotalWeight", SqlDbType.Real),
                    new SqlParameter("@wbVoyage",SqlDbType.VarChar),
                    new SqlParameter("@wbIOmark",SqlDbType.VarChar),
                    new SqlParameter("@wbChinese",SqlDbType.VarChar),
                    new SqlParameter("@wbEnglish",SqlDbType.VarChar),
                    new SqlParameter("@wbSubNumber",SqlDbType.Int),
                    new SqlParameter("@wbTransportMode",SqlDbType.VarChar),
                    new SqlParameter("@wbEntryDate",SqlDbType.VarChar),
                    new SqlParameter("@wbSRport",SqlDbType.VarChar),
                    new SqlParameter("@wbPortCode",SqlDbType.VarChar),
                    new SqlParameter("@wbStorageDate",SqlDbType.VarChar),
                    new SqlParameter("@wbCompany",SqlDbType.VarChar),
                    new SqlParameter("@wbStatus",SqlDbType.Int),
                    new SqlParameter("@wbImportType",SqlDbType.Int)
            };
            parameters[0].Value = model.WbSerialNum;
            parameters[1].Value = model.WbTotalNumber;
            parameters[2].Value = model.WbTotalWeight;
            parameters[3].Value = model.WbVoyage;
            parameters[4].Value = model.WbIOmark;
            parameters[5].Value = model.WbChinese;
            parameters[6].Value = model.WbEnglish;
            parameters[7].Value = model.WbSubNumber;
            parameters[8].Value = model.WbTransportMode;
            parameters[9].Value = model.WbEntryDate;
            parameters[10].Value = model.WbSRPort;
            parameters[11].Value = model.WbPortCode;
            parameters[12].Value = model.StorageDate;
            parameters[13].Value = company == "" ? "" : (new T_User()).GetUserByUserName(company);
            parameters[14].Value = 0;
            parameters[15].Value = model.wbImportType == null ? 0 : model.wbImportType;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;

            }

        }

        //更新主运单
        public bool updateWayBill(Model.M_WayBill model, string company)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"update WayBill set 
                            wbTotalNumber=@wbTotalNumber,
                            wbTotalWeight=@wbTotalWeight,
                            wbVoyage=@wbVoyage,
                            wbIOmark=@wbIOmark,
                            wbChinese=@wbChinese,
                            wbEnglish=@wbEnglish,
                            wbSubNumber=@wbSubNumber,
                            wbTransportMode=@wbTransportMode,
                            wbEntryDate=@wbEntryDate,
                            wbSRport=@wbSRport,
                            wbPortCode=@wbPortCode,
                            wbStorageDate=@wbStorageDate,
                            wbCompany=@wbCompany,
                            wbStatus=@wbStatus,
                            wbImportType=@wbImportType
                            from WayBill WB inner join V_Distinct_WayBill V_WB on V_WB.wbID=WB.wbID where V_WB.wbSerialNum=@wbSerialNum");

            SqlParameter[] parameters = {
                    new SqlParameter("@wbSerialNum",SqlDbType.VarChar),
                    new SqlParameter("@wbTotalNumber",SqlDbType.Int ),
                    new SqlParameter("@wbTotalWeight", SqlDbType.Real),
                    new SqlParameter("@wbVoyage",SqlDbType.VarChar),
                    new SqlParameter("@wbIOmark",SqlDbType.VarChar),
                    new SqlParameter("@wbChinese",SqlDbType.VarChar),
                    new SqlParameter("@wbEnglish",SqlDbType.VarChar),
                    new SqlParameter("@wbSubNumber",SqlDbType.Int),
                    new SqlParameter("@wbTransportMode",SqlDbType.VarChar),
                    new SqlParameter("@wbEntryDate",SqlDbType.VarChar),
                    new SqlParameter("@wbSRport",SqlDbType.VarChar),
                    new SqlParameter("@wbPortCode",SqlDbType.VarChar),
                    new SqlParameter("@wbStorageDate",SqlDbType.VarChar),
                    new SqlParameter("@wbCompany",SqlDbType.VarChar),
                    new SqlParameter("@wbStatus",SqlDbType.Int),
                    new SqlParameter("@wbImportType",SqlDbType.Int)

            };
            parameters[0].Value = model.WbSerialNum;
            parameters[1].Value = model.WbTotalNumber;
            parameters[2].Value = model.WbTotalWeight;
            parameters[3].Value = model.WbVoyage;
            parameters[4].Value = model.WbIOmark;
            parameters[5].Value = model.WbChinese;
            parameters[6].Value = model.WbEnglish;
            parameters[7].Value = model.WbSubNumber;
            parameters[8].Value = model.WbTransportMode;
            parameters[9].Value = model.WbEntryDate;
            parameters[10].Value = model.WbSRPort;
            parameters[11].Value = model.WbPortCode;
            parameters[12].Value = model.StorageDate;
            parameters[13].Value = company == "" ? "" : (new T_User()).GetUserByUserName(company);
            parameters[14].Value = 0;
            parameters[15].Value = model.wbImportType == null ? 0 : model.wbImportType;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;

            }

        }

        //根据wbID 更新主运单状态为已预检
        public bool updateStatus(int wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Waybill");
            strSql.Append("  set wbStatus=@wbStatus");
            strSql.Append(" where wbID=" + wbID + "");
            SqlParameter[] parameters = {
                    new SqlParameter("@wbStatus",SqlDbType.Int)
                
            };
            parameters[0].Value = 1;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;

            }
        }



        //根据wbID 更新主运单状态为已放行
        public bool updateReleaStatus(int wbID, int printTimes)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Waybill");
            strSql.Append("  set wbStatus=@wbStatus,wbPrintTimes=@wbPrintTimes");
            strSql.Append(" where wbID=" + wbID + "");
            SqlParameter[] parameters = {
                    new SqlParameter("@wbStatus",SqlDbType.Int),
                      new SqlParameter("@wbPrintTimes",SqlDbType.Int)
                
            };
            parameters[0].Value = 2;
            parameters[1].Value = printTimes + 1;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        //根据wbID 更新主运单状态为已放行
        public bool updateReleaStatus(string wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Waybill");
            strSql.Append("  set wbStatus=@wbStatus,wbPrintTimes=wbPrintTimes+1");
            strSql.Append(" where wbID in(" + wbID + ")");
            SqlParameter[] parameters = {
                    new SqlParameter("@wbStatus",SqlDbType.Int)
                
            };
            parameters[0].Value = 2;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;

            }
        }



        //根据wbID 更新主运单状态为已放行
        public int getPrintStatus(int wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select wbPrintTimes from V_Distinct_WayBill");

            strSql.Append(" where wbID=" + wbID + "");


            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    return int.Parse(ds.Tables[0].Rows[0][0].ToString());
                }
                return 0;
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// 收费普通查询
        /// </summary>
        /// <param name="wbSerialNum"></param>
        /// <returns></returns>



        public DataSet GetModelByWbSerialNum(string wbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  wbStorageDate,wbVoyage,wbSerialNum,wbTotalNumber,wbTotalWeight,wbSubNumber");
            strSql.Append(" from  V_Distinct_WayBill where wbSerialNum='" + wbSerialNum + "' and wbStatus=2");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        /// <summary>
        /// 高级查询
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        /// 

        public DataSet GetModelByDateVoyageCode(string strDate, string strVoyage, string strSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  wbStorageDate,wbVoyage,wbSerialNum,wbTotalNumber,wbTotalWeight,wbSubNumber");
            strSql.Append(" from  V_Distinct_WayBill where");

            if (strDate.Trim() != "")
            {
                strSql.Append(" wbStorageDate='" + strDate + "'");
            }
            else
            {
                if (strVoyage.Trim() != "")
                {
                    strSql.Append(" wbVoyage='" + strVoyage + "'");
                }
                else
                {
                    if (strSerialNum.Trim() != "")
                    {
                        strSql.Append(" wbSerialNum='" + strSerialNum + "'");
                    }
                }
            }



            if (strVoyage.Trim() != "")
            {
                strSql.Append(" and wbVoyage='" + strVoyage + "'");
            }

            if (strSerialNum.Trim() != "")
            {
                strSql.Append(" and wbSerialNum='" + strSerialNum + "'");
            }

            strSql.Append(" and wbStatus=2");

            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }



        /// <summary>
        /// 状态普通查询
        /// </summary>
        /// <param name="wbSerialNum"></param>
        /// <returns></returns>



        public DataSet GetSatusByWbSerialNum(string wbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT  dbo.V_Distinct_WayBill.wbStorageDate, dbo.V_Distinct_WayBill.wbVoyage,");
            strSql.Append("dbo.V_Distinct_WayBill.wbSerialNum, dbo.V_Distinct_SubWayBill.swbSerialNum,dbo.V_Distinct_SubWayBill.swbDescription_CHN, dbo.V_Distinct_SubWayBill.swbNumber,dbo.V_Distinct_SubWayBill.swbWeight, dbo.V_Distinct_SubWayBill.swbActualWeight, dbo.V_Distinct_SubWayBill.swbNeedCheck");
            strSql.Append(" FROM dbo.V_Distinct_WayBill INNER JOIN dbo.V_Distinct_SubWayBill ON dbo.V_Distinct_WayBill.wbID = dbo.V_Distinct_SubWayBill.swb_wbID");
            strSql.Append(" WHERE (dbo.V_Distinct_WayBill.wbSerialNum = '" + wbSerialNum + "')");

            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;

        }

        /// <summary>
        /// 高级查询
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        /// 

        public DataSet GetStatusByDateVoyageCode(string strDate, string strVoyage, string strSerialNum)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append(" SELECT dbo.V_Distinct_WayBill.wbStorageDate, dbo.V_Distinct_WayBill.wbVoyage,");
            strSql.Append("dbo.V_Distinct_WayBill.wbSerialNum, dbo.V_Distinct_SubWayBill.swbSerialNum,dbo.V_Distinct_SubWayBill.swbDescription_CHN, dbo.V_Distinct_SubWayBill.swbNumber,dbo.V_Distinct_SubWayBill.swbWeight, dbo.V_Distinct_SubWayBill.swbActualWeight, dbo.V_Distinct_SubWayBill.swbNeedCheck");
            strSql.Append(" FROM dbo.V_Distinct_WayBill INNER JOIN dbo.V_Distinct_SubWayBill ON dbo.V_Distinct_WayBill.wbID = dbo.V_Distinct_SubWayBill.swb_wbID");
            strSql.Append(" WHERE ");


            if (strDate.Trim() != "")
            {
                strSql.Append(" wbStorageDate='" + strDate + "'");
            }
            else
            {
                if (strVoyage.Trim() != "")
                {
                    strSql.Append(" wbVoyage='" + strVoyage + "'");
                }
                else
                {
                    if (strSerialNum.Trim() != "")
                    {
                        strSql.Append(" wbSerialNum='" + strSerialNum + "'");
                    }
                }
            }



            if (strVoyage.Trim() != "")
            {
                strSql.Append(" and wbVoyage='" + strVoyage + "'");
            }

            if (strSerialNum.Trim() != "")
            {
                strSql.Append(" and wbSerialNum='" + strSerialNum + "'");
            }



            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }




        /// <summary>
        /// 货物状态普通查询
        /// </summary>
        /// <param name="wbSerialNum"></param>
        /// <returns></returns>



        public DataSet CommonQuery(string wbSerialNum)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT wbID,wbStorageDate, wbCompany, wbSerialNum, wbTotalNumber, wbTotalWeight, wbStatus FROM V_Distinct_WayBill");
            strSql.Append(" WHERE wbSerialNum like'%" + wbSerialNum + "%'");
            strSql.Append(" ORDER BY wbID DESC");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;

        }


        public DataSet CommonQuery(string wbSerialNum, string company)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT wbID,wbStorageDate, wbCompany, wbSerialNum, wbTotalNumber, wbTotalWeight, wbStatus FROM V_Distinct_WayBill");
            strSql.Append(" WHERE wbSerialNum like'%" + wbSerialNum + "%' and wbCompany='" + company + "'");
            strSql.Append(" ORDER BY wbID DESC");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;

        }

        /// <summary>
        /// 高级查询
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        /// 

        public DataSet SuperQuery(string strDate, string strVoyage, string strSerialNum)
        {


            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT wbID,wbStorageDate, wbCompany, wbSerialNum, wbTotalNumber, wbTotalWeight, wbStatus FROM V_Distinct_WayBill");
            strSql.Append(" WHERE ");


            if (strDate.Trim() != "")
            {
                strSql.Append(" wbStorageDate='" + strDate + "'");
            }
            else
            {
                if (strVoyage.Trim() != "")
                {
                    strSql.Append(" wbCompany like'%" + strVoyage + "%'");
                }
                else
                {
                    if (strSerialNum.Trim() != "")
                    {
                        strSql.Append(" wbSerialNum like'%" + strSerialNum + "%'");
                    }
                }
            }



            if (strVoyage.Trim() != "")
            {
                strSql.Append(" and wbCompany like'%" + strVoyage + "%'");
            }

            if (strSerialNum.Trim() != "")
            {
                strSql.Append(" and wbSerialNum like'%" + strSerialNum + "%'");
            }


            strSql.Append(" ORDER BY wbID DESC");

            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }

        /// <summary>
        /// 预检查询
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        /// 

        public DataSet CheckQuery(string strDate, string strCompany)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT wbSerialNum, wbTotalNumber, wbTotalWeight, wbCompany, wbStorageDate, wbID,wbStatus");
            strSql.Append(" FROM V_Distinct_WayBill WHERE (wbStatus = 0 or wbStatus=1)");

            if (strDate != "")
            {
                strSql.Append(" and wbStorageDate='" + strDate + "'");
            }


            if (strCompany != "")
            {
                strSql.Append(" and wbCompany='" + strCompany + "'");
            }


            strSql.Append(" ORDER BY wbID DESC");

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
        /// 华宇统计
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        /// 

        public DataSet totalByDate(string startDate, string endDate, string company)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT dbo.V_Distinct_SubWayBill.swbWeight, dbo.V_Distinct_SubWayBill.swbActualWeight, dbo.V_Distinct_SubWayBill.swbNeedCheck, dbo.V_Distinct_WayBill.wbCompany");
            strSql.Append(" FROM dbo.V_Distinct_SubWayBill INNER JOIN dbo.V_Distinct_WayBill ON dbo.V_Distinct_SubWayBill.swb_wbID = dbo.V_Distinct_WayBill.wbID");

            if (company == "")
            {
                strSql.Append(" WHERE (dbo.V_Distinct_WayBill.wbStatus = 2) and (dbo.V_Distinct_WayBill.wbStorageDate>='" + startDate + "') and (dbo.V_Distinct_WayBill.wbStorageDate<='" + endDate + "')");
            }
            else
            {
                strSql.Append(" WHERE (dbo.V_Distinct_WayBill.wbStatus = 2) and (dbo.V_Distinct_WayBill.wbStorageDate>='" + startDate + "') and (dbo.V_Distinct_WayBill.wbStorageDate<='" + endDate + "') and (dbo.V_Distinct_WayBill.wbCompany='" + company + "')");
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


        public DataSet gruopByCompany(string startDate, string endDate, string company)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT TOP 100 PERCENT COUNT(swbWeight) AS Expr1, wbCompany FROM (");
            strSql.Append("SELECT dbo.V_Distinct_SubWayBill.swbWeight, dbo.V_Distinct_SubWayBill.swbActualWeight, dbo.V_Distinct_SubWayBill.swbNeedCheck, dbo.V_Distinct_WayBill.wbCompany");
            strSql.Append(" FROM dbo.V_Distinct_SubWayBill INNER JOIN dbo.V_Distinct_WayBill ON dbo.V_Distinct_SubWayBill.swb_wbID = dbo.V_Distinct_WayBill.wbID");
            if (company == "")
            {
                strSql.Append(" WHERE (dbo.V_Distinct_WayBill.wbStatus = 2) and (dbo.V_Distinct_WayBill.wbStorageDate>='" + startDate + "') and (dbo.V_Distinct_WayBill.wbStorageDate<='" + endDate + "')) DERIVEDTBL");
            }
            else
            {
                strSql.Append(" WHERE (dbo.V_Distinct_WayBill.wbStatus = 2) and (dbo.V_Distinct_WayBill.wbCompany='" + company + "') and (dbo.V_Distinct_WayBill.wbStorageDate>='" + startDate + "') and (dbo.V_Distinct_WayBill.wbStorageDate<='" + endDate + "')) DERIVEDTBL");
            }
            strSql.Append(" GROUP BY wbCompany");
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


        public DataSet getWayBillInfo(string wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from V_Distinct_WayBill where wbID=" + wbID);

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
        /// 检查总运单、分运单是否在预入库记录中
        /// </summary>
        /// <param name="wbSerialNum"></param>
        /// <returns></returns>
        public Boolean IsInReStore(string wbSerialNum, string swbSerialNum)
        {
            Boolean bExist = false;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  count(0) from V_Fore_WayBill where wbSerialNum='" + wbSerialNum + "' and swbSerialNum='" + swbSerialNum + "'");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());

            if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) > 0)
            {
                bExist = true;
            }
            return bExist;
        }

        public DataSet getWayBillControlInfo(string wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT  swb_wbID ,
                                        COUNT(CASE WHEN swbControlFlag = 0 THEN 1
                                              END) swbControlFlag0 ,
                                        COUNT(CASE WHEN swbControlFlag = 1 THEN 1
                                              END) swbControlFlag1 ,
                                        COUNT(CASE WHEN swbControlFlag = 2 THEN 1
                                              END) swbControlFlag2 ,
                                        COUNT(CASE WHEN swbControlFlag = 3 THEN 1
                                              END) swbControlFlag3
                                FROM    V_Distinct_SubWayBill
                                GROUP BY swb_wbID
                                HAVING  swb_wbID = {0}", wbID);

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

        public bool updateWBSRport(string wbID, string wbSRport)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Waybill");
            strSql.Append("  set wbSRport=@wbSRport ");
            strSql.Append(" where wbID in(" + wbID + ")");
            SqlParameter[] parameters = {
                    new SqlParameter("@wbSRport",SqlDbType.NVarChar)
            };
            parameters[0].Value = wbSRport;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool updateWBArrivalDate(string wbID, string WBArrivalDate)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Waybill");
            strSql.Append("  set wbArrivalDate=@wbArrivalDate ");
            strSql.Append(" where wbID in(" + wbID + ")");
            SqlParameter[] parameters = {
                    new SqlParameter("@wbArrivalDate",SqlDbType.DateTime)
            };
            parameters[0].Value = WBArrivalDate;

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
