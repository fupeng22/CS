using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DBUtility;
using Model;

namespace SQLDAL
{
  public  class T_Custormer
    {

        public T_Custormer()
        {
        }



        // 获取CardName
        public DataSet GetCustomerName()
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select CustormerID,CustormerName from Custormer");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }
        //获取所有客户
        public DataSet GetAllCustomer()
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from Custormer");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }
        //获取所有客户对于价格
        public DataSet GetAllCustomerValue()
        {
       
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT dbo.Category.CategoryValue, dbo.Custormer.CustormerName");
            strSql.Append(" FROM dbo.Custormer INNER JOIN");
            strSql.Append("  dbo.Category ON dbo.Custormer.CategoryID = dbo.Category.CategoryID");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }


        //新增
        public bool AddCustormer(Model.M_Custormer mCustormer)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Custormer");
            strSql.Append(" (CustormerName,CategoryID,CategoryName)");
            strSql.Append(" values (");
            strSql.Append(" @CustormerName,@CategoryID,@CategoryName)");
            SqlParameter[] parameters = {
                      
                        new SqlParameter("@CustormerName",SqlDbType.VarChar),
                        new SqlParameter("@CategoryID",SqlDbType.Int),
                        new SqlParameter("@CategoryName",SqlDbType.VarChar)
                        
                                                        };
           
            parameters[0].Value = mCustormer.CustormerName;
            parameters[1].Value = mCustormer.CategoryID;
            parameters[2].Value = mCustormer.CategoryName;


            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //更新
        public bool UpdateCustormer(Model.M_Custormer mCustormer)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Custormer");
            strSql.Append("  set CustormerName=@CustormerName,CategoryID=@CategoryID,CategoryName=@CategoryName");
            strSql.Append(" where CustormerID=" + mCustormer.CustormerID + "");
            SqlParameter[] parameters = {
                      
                        new SqlParameter("@CustormerName",SqlDbType.VarChar),
                        new SqlParameter("@CategoryID",SqlDbType.Int),
                        new SqlParameter("@CategoryName",SqlDbType.VarChar)
                       
                                                        };
         
            parameters[0].Value = mCustormer.CustormerName;
            parameters[1].Value = mCustormer.CategoryID;
            parameters[2].Value = mCustormer.CategoryName;



            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //删除
        public bool deleteCustormer(int CustormerID)
        {

            StringBuilder strSql = new StringBuilder();

            strSql.Append("delete from Custormer where CustormerID=" + CustormerID + "");
            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
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
