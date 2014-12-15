using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DBUtility;
using Model;

namespace SQLDAL
{
    public class T_User
    {
        /// <summary>
        /// 根据用户账号判断该用户账号是否已被使用
        /// </summary>
        /// <param name="UserID">用户编号</param>
        /// <returns>存在返回true反之返回false</returns>

        public bool UserExists(string userName, int comment)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT *");
            strSql.Append(" FROM [User]");
            strSql.Append(" WHERE (userName = '" + userName + "') and (comment = " + comment + ")");

            if (DBUtility.SqlServerHelper.Query(strSql.ToString()).Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UserExists(string userName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT *");
            strSql.Append(" FROM [User]");
            strSql.Append(" WHERE (userName = '" + userName + "')");

            if (DBUtility.SqlServerHelper.Query(strSql.ToString()).Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UserExists(int userID, string userName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT *");
            strSql.Append(" FROM [User]");
            strSql.Append(" WHERE (userName = '" + userName + "') and (userID<>" + userID+")");

            if (DBUtility.SqlServerHelper.Query(strSql.ToString()).Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 用户登录检测(false时不区分大小写)
        /// </summary>
        /// <param name="UserID">用户标识</param>
        /// <param name="pwd">密码</param>
        /// <returns>通过返回true反之返回false</returns>
        /// 


        public DataSet CheckLogin(string userName, string pwd, int comment)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM [User]");
            strSql.Append("  WHERE (userName = '" + userName + "') AND (userPassword = '" + pwd + "') and (comment = " + comment + ")");

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


        ///<summary>
        ///获取权限
        ///<param name="userName">用户名</param>
        ///<param name="userPassword">密码</param>
        ///<return>返回数据集DataSet</return>
        ///
        public int GetAuthority(string userName, string userPassord)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM [User]");
            strSql.Append("  WHERE (userName = '" + userName + "') AND (userPassword = '" + userPassord + "')");
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return int.Parse(ds.Tables[0].Rows[0][3].ToString());
            }
            else
            {
                return 0;
            }
        }

        ///<summary>
        ///获取全部用户
        ///<param name="userName">用户名</param>
        ///<param name="userPassword">密码</param>
        ///<return>返回数据集DataSet</return>
        ///
        public DataSet GetUsers()
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM [User]");

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


        //注册用户

        public bool addUser(Model.M_User mUser)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" insert into [User]");
            strSql.Append(" (userName,userPassword,authority,comment,company,CompanyFullName,LinkPerson,IdentityCode,LinkTel,CompanyPhone,CompanyAddr,LinkMail,iSendFirstPickGoodsEmail,iSendUnReleaseGoodsEmail,iSendRejectGoodsEmail)");
            strSql.Append(" values(");
            strSql.Append("@userName,@userPassword,@authority,@comment,@company,@CompanyFullName,@LinkPerson,@IdentityCode,@LinkTel,@CompanyPhone,@CompanyAddr,@LinkMail,@iSendFirstPickGoodsEmail,@iSendUnReleaseGoodsEmail,@iSendRejectGoodsEmail)");

            SqlParameter[] parameters = {
                      
                        new SqlParameter("@userName",SqlDbType.VarChar),
                        new SqlParameter("@userPassword",SqlDbType.VarChar),
                        new SqlParameter("@authority",SqlDbType.Int),
                        new SqlParameter("@comment",SqlDbType.Int),
                         new SqlParameter("@company",SqlDbType.VarChar),
                         new SqlParameter("@CompanyFullName",SqlDbType.NVarChar),
                        new SqlParameter("@LinkPerson",SqlDbType.NVarChar),
                        new SqlParameter("@IdentityCode",SqlDbType.NVarChar),
                        new SqlParameter("@LinkTel",SqlDbType.NVarChar),
                        new SqlParameter("@CompanyPhone",SqlDbType.NVarChar),
                        new SqlParameter("@CompanyAddr",SqlDbType.NVarChar),
                        new SqlParameter("@LinkMail",SqlDbType.NVarChar),
                        new SqlParameter("@iSendFirstPickGoodsEmail",SqlDbType.Int),
                        new SqlParameter("@iSendUnReleaseGoodsEmail",SqlDbType.Int),
                        new SqlParameter("@iSendRejectGoodsEmail",SqlDbType.Int)
                                                        };

            parameters[0].Value = mUser.UserName;
            parameters[1].Value = mUser.UserPassword;
            parameters[2].Value = mUser.Authority;
            parameters[3].Value = mUser.Comment;
            parameters[4].Value = mUser.Company;
            parameters[5].Value = mUser.CompanyFullName;
            parameters[6].Value = mUser.LinkPerson;
            parameters[7].Value = mUser.IdentityCode;
            parameters[8].Value = mUser.LinkTel;
            parameters[9].Value = mUser.CompanyPhone;
            parameters[10].Value = mUser.CompanyAddr;
            parameters[11].Value = mUser.LinkMail;
            parameters[12].Value = mUser.iSendFirstPickGoodsEmail;
            parameters[13].Value = mUser.iSendUnReleaseGoodsEmail;
            parameters[14].Value = mUser.iSendRejectGoodsEmail;


            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //删除用户


        public bool deleteUser(int userID)
        {

            StringBuilder strSql = new StringBuilder();

            strSql.Append("delete from [User] where userID=" + userID + "");
            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //更新用户
        public bool UpdateUser(Model.M_User mUser)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [User]");
            strSql.Append("  set userName=@userName,userPassword=@userPassword,authority=@authority,comment=@comment,company=@company,CompanyFullName=@CompanyFullName,LinkPerson=@LinkPerson,IdentityCode=@IdentityCode,LinkTel=@LinkTel,CompanyPhone=@CompanyPhone,CompanyAddr=@CompanyAddr,LinkMail=@LinkMail,iSendFirstPickGoodsEmail=@iSendFirstPickGoodsEmail,iSendUnReleaseGoodsEmail=@iSendUnReleaseGoodsEmail,iSendRejectGoodsEmail=@iSendRejectGoodsEmail");
            strSql.Append(" where userID=" + mUser.UserID + "");
            SqlParameter[] parameters = {
                       
                        new SqlParameter("@userName",SqlDbType.NVarChar),
                        new SqlParameter("@userPassword",SqlDbType.NVarChar),
                        new SqlParameter("@authority",SqlDbType.Int),
                        new SqlParameter("@comment",SqlDbType.Int),
                        new SqlParameter("@company",SqlDbType.NVarChar),
                         new SqlParameter("@CompanyFullName",SqlDbType.NVarChar),
                        new SqlParameter("@LinkPerson",SqlDbType.NVarChar),
                        new SqlParameter("@IdentityCode",SqlDbType.NVarChar),
                        new SqlParameter("@LinkTel",SqlDbType.NVarChar),
                        new SqlParameter("@CompanyPhone",SqlDbType.NVarChar),
                        new SqlParameter("@CompanyAddr",SqlDbType.NVarChar),
                         new SqlParameter("@LinkMail",SqlDbType.NVarChar),
                        new SqlParameter("@iSendFirstPickGoodsEmail",SqlDbType.Int),
                        new SqlParameter("@iSendUnReleaseGoodsEmail",SqlDbType.Int),
                        new SqlParameter("@iSendRejectGoodsEmail",SqlDbType.Int)
                                                        };

            parameters[0].Value = mUser.UserName;
            parameters[1].Value = mUser.UserPassword;
            parameters[2].Value = mUser.Authority;
            parameters[3].Value = mUser.Comment;
            parameters[4].Value = mUser.Company;
            parameters[5].Value = mUser.CompanyFullName;
            parameters[6].Value = mUser.LinkPerson;
            parameters[7].Value = mUser.IdentityCode;
            parameters[8].Value = mUser.LinkTel;
            parameters[9].Value = mUser.CompanyPhone;
            parameters[10].Value = mUser.CompanyAddr;
            parameters[11].Value = mUser.LinkMail;
            parameters[12].Value = mUser.iSendFirstPickGoodsEmail;
            parameters[13].Value = mUser.iSendUnReleaseGoodsEmail;
            parameters[14].Value = mUser.iSendRejectGoodsEmail;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //修改密码


        public bool changePassword(Model.M_User mUser)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update  [User] ");
            strSql.Append("  set userPassword=@userPassword");
            strSql.Append(" where userName='" + mUser.UserName + "'");
            SqlParameter[] parameters = {
                      
                        new SqlParameter("@userPassword",SqlDbType.VarChar)
                                                        };

            parameters[0].Value = mUser.UserPassword;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //获取货代公司名称

        public DataSet GetCompany()
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT company FROM [User]");
            strSql.Append("  WHERE (comment = 2) ");
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

        //获取货代公司名称

        public DataSet GetAllCompany()
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT userName,company FROM [User]");
            strSql.Append("  WHERE (comment = 2) ");
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
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataSet GetUser(string userID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM [User] where userID=" + userID);

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
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataSet GetUserByCompany(string company)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM [User] where company='" + company + "'");

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
        /// 根据用户名获取用户公司
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetUserByUserName(string userName)
        {
            string strCompany = "";
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT top 1 * FROM [User] where userName='" + userName + "'");

            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds!=null)
            {
                DataTable dt = ds.Tables[0];
                if (dt!=null && dt.Rows.Count>0)
                {
                    strCompany=dt.Rows[0]["company"].ToString();
                }
            }

            return strCompany;
        }

//        public Boolean LoginValidate()
//        {
//            Boolean bOK = false;

//            StringBuilder strSql = new StringBuilder();
//            strSql.Append(@"select * from LoginValidate where 
//                        CONVERT(nvarchar(10),GETDATE(),120)>=CONVERT(nvarchar(10),dBeginD,120) and 
//                        CONVERT(nvarchar(10),GETDATE(),120)<=CONVERT(nvarchar(10),dEndD,120)
//                         and isCurrent=1");

//            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
//            if (ds != null)
//            {
//                DataTable dt = ds.Tables[0];
//                if (dt != null && dt.Rows.Count > 0)
//                {
//                    bOK = true;
//                }
//            }

//            return bOK;
//        }

        public Boolean LoginValidate()
        {
            Boolean bOK = false;
            //Util.CryptographyTool.Encrypt();
            StringBuilder strSql = new StringBuilder();
            DateTime dBeginD = DateTime.Now;
            DateTime dEndD = DateTime.Now;
            strSql.Append(@"select * from LoginValidate where isCurrent=1");

            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    try
                    {
                        dBeginD = Convert.ToDateTime(Util.CryptographyTool.Decrypt(dt.Rows[0]["dBeginD"].ToString(),"HuayuTAT"));
                        dEndD = Convert.ToDateTime(Util.CryptographyTool.Decrypt(dt.Rows[0]["dEndD"].ToString(), "HuayuTAT"));
                        if (dBeginD<=DateTime.Now&& dEndD>=DateTime.Now)
                        {
                            bOK = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }

            return bOK;
        }
    }
}
