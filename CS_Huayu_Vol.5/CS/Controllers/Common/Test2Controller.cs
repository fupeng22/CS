using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Util;
using System.Security.Cryptography;
using System.Text;

namespace CS.Controllers.Common
{
    public class Test2Controller : Controller
    {
        //
        // GET: /Test2/

        public ActionResult Index()
        {
            MD5 m = new MD5CryptoServiceProvider();
            byte[] s = m.ComputeHash(UnicodeEncoding.UTF8.GetBytes("<RequestOrder></RequestOrder>123456"));

            ViewData["s1"] = System.Convert.ToBase64String(s);
            return View();
        }

        public string GetCode(string type,string str,int width,int height)
        {
            string strRet = "";
            switch (type)
            {
                case "0":
                    strRet = Util.BarCodeToHTML.get39(str,width,height);
                    break;
                case "1":
                    strRet = Util.BarCodeToHTML.getEAN13(str, width, height);
                    break;
                default:
                    break;
            }
            return strRet;
        }
    }
}
