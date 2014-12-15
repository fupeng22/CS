using System;
using System.Collections.Generic;
using System.Text;
using Model;
using System.Data.SqlClient;
using System.Data;

namespace SQLDAL
{
    public class T_EmailManagement
    {

        public bool InsertEmail(M_EmailManagement m_EmailManagement)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into EmailManagement");
            strSql.Append(" (EmailSMTP,EmailUserName,EmailPwd,FirstPickGoodEmail_Subject,FirstPickGoodEmail_Body,UnReleaseGoodEmail_Subject,UnReleaseGoodEmail_Body,RejectGoodEmail_Subject,RejectGoodEmail_Body,SendDialyReport_Subject,SendDialyReport_Body,mMemo)");
            strSql.Append(" values (");
            strSql.Append("@EmailSMTP,@EmailUserName,@EmailPwd,@FirstPickGoodEmail_Subject,@FirstPickGoodEmail_Body,@UnReleaseGoodEmail_Subject,@UnReleaseGoodEmail_Body,@RejectGoodEmail_Subject,@RejectGoodEmail_Body,@SendDialyReport_Subject,@SendDialyReport_Body,@mMemo)");

            SqlParameter[] parameters = {
                    new SqlParameter("@EmailSMTP",SqlDbType.NVarChar),
                    new SqlParameter("@EmailUserName",SqlDbType.NVarChar ),
                    new SqlParameter("@EmailPwd", SqlDbType.NVarChar),
                    new SqlParameter("@FirstPickGoodEmail_Subject",SqlDbType.NVarChar),
                    new SqlParameter("@FirstPickGoodEmail_Body",SqlDbType.NText),
                    new SqlParameter("@UnReleaseGoodEmail_Subject",SqlDbType.NVarChar),
                    new SqlParameter("@UnReleaseGoodEmail_Body",SqlDbType.NText),
                    new SqlParameter("@RejectGoodEmail_Subject",SqlDbType.NVarChar),
                    new SqlParameter("@RejectGoodEmail_Body",SqlDbType.NText),
                    new SqlParameter("@SendDialyReport_Subject",SqlDbType.NVarChar),
                    new SqlParameter("@SendDialyReport_Body",SqlDbType.NText),
                    new SqlParameter("@mMemo",SqlDbType.NVarChar)
            };
            parameters[0].Value = m_EmailManagement.EmailSMTP;
            parameters[1].Value = m_EmailManagement.EmailUserName;
            parameters[2].Value = m_EmailManagement.EmailPwd;
            parameters[3].Value = m_EmailManagement.FirstPickGoodEmail_Subject;
            parameters[4].Value = m_EmailManagement.FirstPickGoodEmail_Body;
            parameters[5].Value = m_EmailManagement.UnReleaseGoodEmail_Subject;
            parameters[6].Value = m_EmailManagement.UnReleaseGoodEmail_Body;
            parameters[7].Value = m_EmailManagement.RejectGoodEmail_Subject;
            parameters[8].Value = m_EmailManagement.RejectGoodEmail_Body;
            parameters[9].Value = m_EmailManagement.SendDialyReport_Subject;
            parameters[10].Value = m_EmailManagement.SendDialyReport_Body;
            parameters[11].Value = m_EmailManagement.mMemo;

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString(), parameters) >= 1)
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        public bool DeleteAllEmail()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from EmailManagement");

            if (DBUtility.SqlServerHelper.ExecuteSql(strSql.ToString()) >= 1)
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        public DataSet GetAllEmail()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM [EmailManagement]");

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

        public string GetEmailContent(EmailType emailType)
        {
            string strRet = "";
            DataSet ds = new T_EmailManagement().GetAllEmail();
            if (ds!=null)
            {
                DataTable dt = ds.Tables[0];
                if (dt!=null && dt.Rows.Count>0)
                {
                    switch (emailType)
                    {
                        case EmailType.EmailSubject_FirstPickGood:
                            strRet = dt.Rows[0]["FirstPickGoodEmail_Subject"].ToString();
                            break;
                        case EmailType.EmailBody_FirstPickGood:
                            strRet = dt.Rows[0]["FirstPickGoodEmail_Body"].ToString();
                            break;
                        case EmailType.EmailSubject_UnReleaseGoods:
                            strRet = dt.Rows[0]["UnReleaseGoodEmail_Subject"].ToString();
                            break;
                        case EmailType.EmailBody_UnReleaseGoods:
                            strRet = dt.Rows[0]["UnReleaseGoodEmail_Body"].ToString();
                            break;
                        case EmailType.EmailSubject_RejectGoods:
                            strRet = dt.Rows[0]["RejectGoodEmail_Subject"].ToString();
                            break;
                        case EmailType.EmailBody_RejectGoods:
                            strRet = dt.Rows[0]["RejectGoodEmail_Body"].ToString();
                            break;
                        case EmailType.EmailSenderSMTP:
                            strRet = dt.Rows[0]["EmailSMTP"].ToString();
                            break;
                        case EmailType.EmailSenderUserName:
                            strRet = dt.Rows[0]["EmailUserName"].ToString();
                            break;
                        case EmailType.EmailSenderPwd:
                            strRet = dt.Rows[0]["EmailPwd"].ToString();
                            break;
                        case EmailType.EmailSubject_SendDialyReport:
                            strRet = dt.Rows[0]["SendDialyReport_Subject"].ToString();
                            break;
                        case EmailType.EmailBody_SendDialyReport:
                            strRet = dt.Rows[0]["SendDialyReport_Body"].ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            return strRet;
        }
    }

    public enum EmailType
    {
        EmailSubject_FirstPickGood = 1,
        EmailBody_FirstPickGood = 2,
        EmailSubject_UnReleaseGoods = 3,
        EmailBody_UnReleaseGoods = 4,
        EmailSubject_RejectGoods = 5,
        EmailBody_RejectGoods = 6,
        EmailSenderUserName=7,
        EmailSenderSMTP=8,
        EmailSenderPwd=9,
        EmailSubject_SendDialyReport=10,
        EmailBody_SendDialyReport=11
    };

}
