using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SQLDAL
{
    public class T_SynchronizeInOutStore
    {
        public bool ExistSynchronizeInOutStore(string sios_swbID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from SynchronizeInOutStore where sios_swbID="+sios_swbID);
            DataSet ds = DBUtility.SqlServerHelper.Query(strSql.ToString());
            return int.Parse(ds.Tables[0].Rows[0][0].ToString()) > 0;
        }
    }
}
