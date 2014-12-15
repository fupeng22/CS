using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Threading;
using System.Web.Helpers;
using System.Net.Mail;
using System.Text;

namespace CS.Controllers.Common
{
    public class EmailController : Controller
    {

        public string STR_SENDER_SMTP = ConfigurationManager.AppSettings["SenderSMTP"];
        public string STR_SENDER_USERNAME = ConfigurationManager.AppSettings["SenderUserName"];
        public string STR_SENDER_USERMAIL = ConfigurationManager.AppSettings["SenderUserMail"];
        public string STR_SENDER_USERPWD = ConfigurationManager.AppSettings["SenderPwd"];
        public string STR_SENDER_RECIEVEREMAIL = ConfigurationManager.AppSettings["RecieverEmail"];
        public string STR_SENDER_RECIEVERUSERNAME = ConfigurationManager.AppSettings["RecieverName"];
        public string STR_CARBONCODY = ConfigurationManager.AppSettings["CarbonCopy"];
        public string STR_TIMEOUT = ConfigurationManager.AppSettings["MaxTimeOut"];
        //
        // GET: /Email/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EmailRequest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProcessRequest(string customerName, string customerRequest)
        {
            Thread threadSendMail;
            //threadSendMail = new Thread(new ThreadStart(WebMail_SendEmail));
            //threadSendMail.Name = "WebMail_SendEmail";
            //threadSendMail.Start();

            threadSendMail = new Thread(new ThreadStart(NetMail_SendMail));
            threadSendMail.Name = "NetMail_SendMail";
            threadSendMail.Start();

            return View();
        }

        public void NetMail_SendMail()
        {
            string customerName = "";
            string customerRequest = "";
            //MailAddress mailFrom = new MailAddress(STR_SENDER_USERMAIL);
            SmtpClient client = new SmtpClient();
            MailAddress mailTo = null;
            MailMessage mail = null;
            string strSubJect = "Subject";
            string strBody = "Body";

            client.Host = STR_SENDER_SMTP;
            client.Credentials = new System.Net.NetworkCredential(STR_SENDER_USERMAIL, STR_SENDER_USERPWD);

            string[] arrRecieverEmail = STR_SENDER_RECIEVEREMAIL.Split('*');
            try
            {
                for (int i = 0; i < arrRecieverEmail.Length; i++)
                {
                    if (arrRecieverEmail[i].ToString() != "")
                    {
                        mail = new MailMessage();
                        mail.From = new MailAddress(STR_SENDER_USERMAIL, STR_SENDER_USERNAME, Encoding.GetEncoding(936));
                        mailTo = new MailAddress(arrRecieverEmail[i], STR_SENDER_RECIEVERUSERNAME, Encoding.GetEncoding(936));
                        mail.To.Add(mailTo);
                        mail.CC.Add(STR_CARBONCODY);
                        mail.Attachments.Add(new Attachment(Server.MapPath("~/Temp/xls/") + "1.png", System.Net.Mime.MediaTypeNames.Image.Jpeg));
                        mail.Attachments.Add(new Attachment(Server.MapPath("~/Temp/xls/") + "1.xls", System.Net.Mime.MediaTypeNames.Application.Octet));
                        mail.Subject = strSubJect;
                        mail.Body = strBody;
                        mail.SubjectEncoding = Encoding.UTF8;
                        mail.IsBodyHtml = true;

                        client.Timeout = Convert.ToInt32(STR_TIMEOUT);
                        client.Send(mail);
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        public void WebMail_SendEmail()
        {
            string customerName = "";
            string customerRequest = "";
            string[] arrRecieverEmail = STR_SENDER_RECIEVEREMAIL.Split('*');
            try
            {
                for (int i = 0; i < arrRecieverEmail.Length; i++)
                {
                    if (arrRecieverEmail[i].ToString() != "")
                    {
                        System.Threading.Thread.Sleep(5000);
                        // 初始化   
                        WebMail.SmtpServer = STR_SENDER_SMTP;
                        WebMail.SmtpPort = 25;
                        WebMail.EnableSsl = false;
                        WebMail.UserName = STR_SENDER_USERNAME;
                        WebMail.From = STR_SENDER_USERMAIL;
                        WebMail.Password = STR_SENDER_USERPWD;
                        // 发送邮件       
                        WebMail.Send(to: arrRecieverEmail[i],
                        subject: "来自 - " + customerName + "的求助",
                        body: customerRequest
                        );
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
