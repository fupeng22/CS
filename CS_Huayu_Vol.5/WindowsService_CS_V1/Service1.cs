using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.IO;
using System.Configuration;
using SQLDAL;

namespace WindowsService_CS_V1
{
    public partial class Service1 : ServiceBase
    {
        private string STR_TIMEINTERVAL = ConfigurationManager.AppSettings["TimeInterval"].ToString();

        System.Timers.Timer timer = new System.Timers.Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Interval = Convert.ToDouble(STR_TIMEINTERVAL);
            timer.Start();
        }

        protected void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.LogMessage("Service Started");
            Run();
            this.LogMessage("Service Stopped");
        }

        protected override void OnStop()
        {
            timer.Stop();
        }

        private void LogMessage(string xMsg)
        {
            string strFileName = "";
            try
            {
                strFileName = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                File.AppendAllText(strFileName, "【"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "】----" + xMsg + "\r\n");
            }
            catch
            {
                //不做任何操作
            }
        }

        protected void Run()
        {
            lock (this)
            {
                
            }
        }
    }
}
