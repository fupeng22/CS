using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DBUtility;
using Model;

namespace SQLDAL
{
   public class T_Category
    {

       public T_Category()
       {
       }


       // 获取CardName
       public DataSet GetCategoryName()
       {

           StringBuilder strSql = new StringBuilder();
           strSql.Append("select CategoryID,CategoryName from Category");
           DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
           return ds;
       }
       //获取所有客户
       public DataSet GetAllCategory()
       {

           StringBuilder strSql = new StringBuilder();
           strSql.Append("select * from Category");
           DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
           return ds;
       }


       //新增
       public bool AddCategory(Model.M_Category mCategory)
       {
           StringBuilder strSql = new StringBuilder();
           strSql.Append("insert into Category");
           strSql.Append(" (CategoryName,CategoryValue)");
           strSql.Append(" values (");
           strSql.Append(" @CategoryName,@CategoryValue)");
           SqlParameter[] parameters = {
                     
                        new SqlParameter("@CategoryName",SqlDbType.VarChar),
                        new SqlParameter("@CategoryValue",SqlDbType.SmallMoney)
                      
                        
                                                        };
         
           parameters[0].Value = mCategory.CategoryName;
           parameters[1].Value = mCategory.CategoryValue;
       


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
       public bool UpdateCategory(Model.M_Category mCategory)
       {
           StringBuilder strSql = new StringBuilder();
           strSql.Append("update Category");
           strSql.Append("  set CategoryName=@CustormerName,CategoryValue=@CategoryValue");
           strSql.Append(" where CategoryID=" + mCategory.CategoryID + "");
           SqlParameter[] parameters = {
                       
                        new SqlParameter("@CustormerName",SqlDbType.VarChar),
                        new SqlParameter("@CategoryValue",SqlDbType.SmallMoney)
                      
                       
                                                        };
          
           parameters[0].Value = mCategory.CategoryName;
           parameters[1].Value = mCategory.CategoryValue;
      
    


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
       public bool deleteCategory(int CategoryID)
       {

           StringBuilder strSql = new StringBuilder();

           strSql.Append("delete from Category where CategoryID=" + CategoryID + "");
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
