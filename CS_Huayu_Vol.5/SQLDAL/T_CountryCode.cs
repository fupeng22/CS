using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace SQLDAL
{
  public  class T_CountryCode
    {
        //新增ProvinceIDData
        public bool addProvinceData(Model.M_CountryCode mdb)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CountryCode");
            strSql.Append(" (id,chineseName,englishName,parentID,levels)");
            strSql.Append(" values (");
            strSql.Append(" @id,@chineseName,@englishName,@parentID,@levels)");

            SqlParameter[] parameters = {
                        new SqlParameter("@id",SqlDbType.Int),
                        new SqlParameter("@chineseName",SqlDbType.NVarChar),
                        new SqlParameter("@englishName",SqlDbType.NVarChar),
                        new SqlParameter("@parentID",SqlDbType.Int),
                        new SqlParameter("@levels",SqlDbType.Int)
                        };

            parameters[0].Value = mdb.Id;
            parameters[1].Value = mdb.ChinesName;
            parameters[2].Value = mdb.EnglishName;
            parameters[3].Value = 0;
            parameters[4].Value = 1;



            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //******************页面获取函数***************************//
        // 获取ProviceID
        public DataSet GetProvinceID()
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from CountryCode");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return ds;
        }
    }
}
