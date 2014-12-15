using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Model;
using System.Data.SqlClient;

namespace SQLDAL
{
    public class T_WayBillWeight
    {
        /// <summary>
        /// 查询指定总运单号是否已经在计费重量表中
        /// </summary>
        /// <param name="wbID"></param>
        /// <returns></returns>
        public Boolean ExistInWayBillWeight(string wbID)
        {
            Boolean bExist = false;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from WayBillWeight");
            strSql.Append(" where wbw_wbID=" + wbID);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());

            if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) > 0)
            {
                bExist = true;
            }

            return bExist;
        }

        /// <summary>
        /// 添加计费重量
        /// </summary>
        /// <param name="m_WayBillWeight"></param>
        /// <returns></returns>
        public Boolean AddWayBillWeight(M_WayBillWeight m_WayBillWeight)
        {
            Boolean bOK = false;
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@" insert into WayBillWeight(wbw_wbID,ActualWeight)
                                                        values({0},{1})", m_WayBillWeight.wbw_wbID, m_WayBillWeight.ActualWeight);

                if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
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
        /// 修改计费重量
        /// </summary>
        /// <param name="m_WayBillWeight"></param>
        /// <returns></returns>
        public Boolean UpdateWayBillWeight(M_WayBillWeight m_WayBillWeight)
        {
            Boolean bOK = false;
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@" update WayBillWeight set ActualWeight={1} where wbw_wbID={0}", m_WayBillWeight.wbw_wbID, m_WayBillWeight.ActualWeight);

                if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
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
        /// 获取计费重量
        /// </summary>
        /// <returns></returns>
        public string getWeightForCompute(string wbID)
        {
            string strRet = "0.00";

            DataSet ds = null;
            DataTable dt = null;

            try
            {
                ds = DBUtility.SqlServerHelper.Query("select top 1 * from WayBillWeight where wbw_wbID=" + wbID);
                if (ds != null)
                {
                    dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        strRet = dt.Rows[0]["ActualWeight"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                strRet = "0.00";
            }

            if (strRet == "0.00")
            {
                try
                {
                    ds = new T_WayBill().getWayBillInfo(wbID);
                    if (ds != null)
                    {
                        dt = ds.Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            strRet = dt.Rows[0]["wbTotalWeight"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    strRet = "0.00";
                }
            }
            return strRet;
        }

        public DataSet GetWayBillWeightInfo(string wbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from V_WayBill_WayBillWeight where wbID="+wbID);
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
        /// 修改计费重量
        /// </summary>
        /// <param name="m_WayBillWeight"></param>
        /// <returns></returns>
        public Boolean UpdateWayBillWeightInfo(M_WayBillWeight m_WayBillWeight)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"UPDATE WayBillWeight
                           SET ddlReceiptMode_ForSetting=@ddlReceiptMode_ForSetting,
                           OperateFee_ForSetting=@OperateFee_ForSetting,
                           PickGoodsFee_ForSetting=@PickGoodsFee_ForSetting,
                           ShiftGoodsFee_ForSetting=@ShiftGoodsFee_ForSetting,
                           CollectionFee_ForSetting=@CollectionFee_ForSetting,
                           ddlPayMode_ForSetting=@ddlPayMode_ForSetting,
                           ShouldPayUnit_ForSetting=@ShouldPayUnit_ForSetting,
                           shouldPay_ForSetting=@shouldPay_ForSetting,
                           wbCompany_ForSetting=@wbCompany_ForSetting,
                           Receipt_ForSetting=@Receipt_ForSetting,
                           ReportSystem_ForSetting=@ReportSystem_ForSetting,
                           QuarantineCheckFee_ForSetting=@QuarantineCheckFee_ForSetting,
                           QuarantinePacketFee_ForSetting=@QuarantinePacketFee_ForSetting
                           where wbwId=@wbwId ");

            SqlParameter[] parameters = {
                    new SqlParameter("@ddlReceiptMode_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@OperateFee_ForSetting",SqlDbType.NVarChar ),
                    new SqlParameter("@PickGoodsFee_ForSetting", SqlDbType.NVarChar),
                    new SqlParameter("@ShiftGoodsFee_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@CollectionFee_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@ddlPayMode_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@ShouldPayUnit_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@shouldPay_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@wbCompany_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@Receipt_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@ReportSystem_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@QuarantineCheckFee_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@QuarantinePacketFee_ForSetting",SqlDbType.NVarChar),
                    new SqlParameter("@wbwId",SqlDbType.Int)
            };
            parameters[0].Value = m_WayBillWeight.ddlReceiptMode_ForSetting;
            parameters[1].Value = m_WayBillWeight.OperateFee_ForSetting;
            parameters[2].Value = m_WayBillWeight.PickGoodsFee_ForSetting;
            parameters[3].Value = m_WayBillWeight.ShiftGoodsFee_ForSetting;
            parameters[4].Value = m_WayBillWeight.CollectionFee_ForSetting;
            parameters[5].Value = m_WayBillWeight.ddlPayMode_ForSetting;
            parameters[6].Value = m_WayBillWeight.ShouldPayUnit_ForSetting;
            parameters[7].Value = m_WayBillWeight.shouldPay_ForSetting;
            parameters[8].Value = m_WayBillWeight.wbCompany_ForSetting;
            parameters[9].Value = m_WayBillWeight.Receipt_ForSetting;
            parameters[10].Value = m_WayBillWeight.ReportSystem_ForSetting;
            parameters[11].Value = m_WayBillWeight.QuarantineCheckFee_ForSetting;
            parameters[12].Value = m_WayBillWeight.QuarantinePacketFee_ForSetting;
            parameters[13].Value = m_WayBillWeight.wbwId;

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
