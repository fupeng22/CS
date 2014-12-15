using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Util;
using System.Net;
using System.IO;
using System.Text;

namespace CS.Controllers.Common
{
    public class Test1Controller : Controller
    {

        protected const string STR_SAVE_TXT_FILE = "~/Temp/xls/";

        //
        // GET: /Test1/

        public ActionResult Index()
        {
            return View();
        }

        public string EncodeString(string str)
        {
            return Util.CryptographyTool.Encrypt(str, "HuayuTAT");
        }

        public string DecodeString(string str)
        {
            return Util.CryptographyTool.Decrypt(str, "HuayuTAT");
        }

        /// <summary>  
        /// 获取FTP文件列表包括文件夹  
        /// </summary>  
        /// <returns></returns>  
        private string[] GetAllList(string url, string userid, string password)
        {
            List<string> list = new List<string>();
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(new Uri(url));
            req.Credentials = new NetworkCredential(userid, password);
            req.Method = WebRequestMethods.Ftp.ListDirectory;
            req.UseBinary = true;
            req.UsePassive = true;
            try
            {
                using (FtpWebResponse res = (FtpWebResponse)req.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(res.GetResponseStream(),Encoding.UTF8))
                    {
                        string s;
                        while ((s = sr.ReadLine()) != null)
                        {
                            list.Add(s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return list.ToArray();
        }

        [HttpPost]
        public string getFTPFileList()
        {
            string[] fileList = GetAllList("ftp://192.168.1.149:6666", "MyFTP", "nabon16891689");
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < fileList.Length; i++)
            {
                sb.AppendFormat("{0}</br>",fileList[i]);
            }
            return sb.ToString();
        }

        public void UploadFileToFTP(FormCollection form)
        {
            HttpPostedFileBase excelFile = Request.Files["MyFile"];
            string strFileName = "";
            string strFullFilePath = "";

            if (excelFile == null)
            {
                
            }
            else
            {
                if (excelFile.ContentLength == 0)
                {
                   
                }
                else
                {
                    strFileName = "[" + DateTime.Now.ToString("yyyyMMddHHmmss") + (new Random()).Next(10).ToString("00") + "]";

                    string strSourceFileNameWithExtension = excelFile.FileName.Substring(excelFile.FileName.LastIndexOf("\\") + 1);
                    string strSourceFileNameWithOutExtension = strSourceFileNameWithExtension.Substring(0, strSourceFileNameWithExtension.LastIndexOf("."));
                    string strSourceFileNameExtensionName = strSourceFileNameWithExtension.Substring(strSourceFileNameWithExtension.LastIndexOf(".") + 1);

                    strFullFilePath = Server.MapPath(STR_SAVE_TXT_FILE + strSourceFileNameWithOutExtension + strFileName + "." + strSourceFileNameExtensionName);

                    try
                    {
                        excelFile.SaveAs(strFullFilePath);

                        FTP ftp = new FTP();
                        ftp.RemoteHost = "192.168.1.149";
                        ftp.RemotePort = 6666;
                        ftp.RemoteUser = "MyFTP";
                        ftp.RemotePass = "nabon16891689";
                        ftp.Connect();
                        ftp.Put(strFullFilePath);
                        ftp.DisConnect();
                    }
                    catch (Exception ex)
                    {
                        
                    }

                }
            }

        }
    }
}
