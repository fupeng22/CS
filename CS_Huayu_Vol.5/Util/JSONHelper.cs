using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public class JSONHelper
    {
        public static string EncodeInValidStr(string strSource)
        {
            string strRet = "";
            strRet = strSource.Replace('"','”').Replace("'","’").Replace("\\","").Replace('<','《').Replace('>','》');
            return strRet;
        }
    }
}
