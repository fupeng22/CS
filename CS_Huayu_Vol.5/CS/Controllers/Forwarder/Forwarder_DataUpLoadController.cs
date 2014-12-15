using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using Model;
using SQLDAL;
using System.Text;
using CS.Filter;

namespace CS.Controllers.Forwarder
{
    [ErrorAttribute]
    public class Forwarder_DataUpLoadController : Controller
    {
        protected const string STR_SAVE_TXT_FILE = "~/Temp/txt/";

        Model.M_WayBill wayBillModel = new Model.M_WayBill();
        Model.M_SubWayBill subWayBillModel = new M_SubWayBill();

        SQLDAL.T_WayBill wayBillSql = new T_WayBill();
        SQLDAL.T_SubWayBill subWayBillSql = new T_SubWayBill();

        //
        // GET: /DataUpLoad/
        [ForwarderRequiresLoginAttribute]
        public ActionResult Index()
        {
            Session["Forwarder_DataUpLoad_SubWayBill"] = null;
            return View();
        }


        [HttpPost]
        public string UploadFile(FormCollection collection)
        {
            string strRet = "";
            string strFileName = "";
            string strFullFilePath = "";

            HttpPostedFileBase txtFile = Request.Files["MyFile"];
            if (txtFile == null)
            {
                strRet = "{\"result\":\"error\",\"data\":\"null\",\"message\":\"未选择文件\"}";
            }
            else
            {
                if (txtFile.ContentLength == 0)
                {
                    strRet = "{\"result\":\"error\",\"data\":\"null\",\"message\":\"文件大小为0\"}";
                }
                else
                {
                    strFileName = "[" + DateTime.Now.ToString("yyyyMMddHHmmss") + (new Random()).Next(10).ToString("00") + "]";

                    string strSourceFileNameWithExtension = txtFile.FileName.Substring(txtFile.FileName.LastIndexOf("\\") + 1);
                    string strSourceFileNameWithOutExtension = strSourceFileNameWithExtension.Substring(0, strSourceFileNameWithExtension.LastIndexOf("."));
                    string strSourceFileNameExtensionName = strSourceFileNameWithExtension.Substring(strSourceFileNameWithExtension.LastIndexOf(".") + 1);

                    strFullFilePath = Server.MapPath(STR_SAVE_TXT_FILE + strSourceFileNameWithOutExtension + strFileName + "." + strSourceFileNameExtensionName);

                    try
                    {
                        txtFile.SaveAs(strFullFilePath);

                        strRet = txtTOJSON(strFullFilePath);
                    }
                    catch (Exception ex)
                    {
                        strRet = "{\"result\":\"error\",\"data\":\"null\",\"message\":\"" + ex.Message + "\"}";
                    }

                }
            }

            return strRet;
        }


        public string txtTOJSON(string url)
        {
            DataTable dt = new DataTable();
            string strRet = "";
            StringBuilder sb = new StringBuilder("");
            StringBuilder sbMainWayBill = new StringBuilder("");
            StringBuilder sbSubWayBill = new StringBuilder("");

            try
            {
                using (StreamReader sr = new StreamReader(url, System.Text.Encoding.GetEncoding("gb2312")))
                {
                    String line = sr.ReadLine();

                    string[] seperator = { "||" };

                    string[] array = line.Split(seperator, StringSplitOptions.None);

                    if (array.Length >= 12)
                    {
                        sbMainWayBill.Append("\"MainWayBill\":{\"result\":\"ok\",\"message\":\"总运单数据正确\",\"data\":");
                        sbMainWayBill.Append("{\"txtSeriaNum\":\"" + array[0].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbVoyage\":\"" + array[1].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbIOmark\":\"" + array[2].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbChinese\":\"" + array[3].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbEnglish\":\"" + array[4].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbTotalWeight\":\"" + array[5].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbTotalNumber\":\"" + array[6].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbTransportMode\":\"" + array[8].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbEntryDate\":\"" + array[9].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbSRport\":\"" + array[10].ToString() + "\",");
                        sbMainWayBill.Append("\"txtWbPortCode\":\"" + array[11].ToString() + "\",");
                        sbMainWayBill.Append("\"txtSubNumber\":\"" + array[7].ToString() + "\"}}");

                        DataTable dtSub = new DataTable();
                        dtSub.Columns.Add("swbID", Type.GetType("System.Int32"));
                        dtSub.Columns.Add("wbSerialNum", Type.GetType("System.String"));
                        dtSub.Columns.Add("swbSerialNum", Type.GetType("System.String"));
                        dtSub.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
                        dtSub.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
                        dtSub.Columns.Add("swbNumber", Type.GetType("System.Int32"));
                        dtSub.Columns.Add("swbWeight", Type.GetType("System.Double"));
                        dtSub.Columns.Add("swbValue", Type.GetType("System.Double"));
                        dtSub.Columns.Add("swbMonetary", Type.GetType("System.String"));
                        dtSub.Columns.Add("swbRecipients", Type.GetType("System.String"));
                        dtSub.Columns.Add("swbCustomsCategory", Type.GetType("System.String"));
                        int iCount = 0;
                        while ((line = sr.ReadLine()) != null)
                        {
                            DataRow row = dtSub.NewRow();
                            iCount++;
                            string[] subArray = line.Split(seperator, StringSplitOptions.None);
                            if (subArray.Length >= 11)
                            {
                                row[0] = iCount;
                                row[1] = array[0].ToString();
                                row[2] = subArray[0];
                                row[3] = subArray[2];
                                row[4] = subArray[1];
                                row[5] = subArray[3];
                                row[6] = subArray[4];
                                row[7] = subArray[5];
                                row[8] = subArray[6];
                                row[9] = subArray[7];
                                row[10] = subArray[12];

                                dtSub.Rows.Add(row);
                            }
                        }

                        if (iCount > 0)
                        {
                            Session["Forwarder_DataUpLoad_SubWayBill"] = dtSub;
                            sbSubWayBill.Append("\"SubWayBill\":{\"result\":\"ok\",\"message\":\"子运单获取正常\",\"data\":{\"total\":" + dtSub.Rows.Count + ",\"rows\":[");
                            for (int i = 0; i < dtSub.Rows.Count; i++)
                            {
                                sbSubWayBill.Append("{");

                                sbSubWayBill.AppendFormat("\"swbID\":\"{0}\",\"wbSerialNum\":\"{1}\",\"swbSerialNum\":\"{2}\",\"swbDescription_CHN\":\"{3}\",\"swbDescription_ENG\":\"{4}\",\"swbNumber\":\"{5}\",\"swbWeight\":\"{6}\",\"swbValue\":\"{7}\",\"swbMonetary\":\"{8}\",\"swbRecipients\":\"{9}\",\"swbCustomsCategory\":\"{10}\"", dtSub.Rows[i]["swbID"].ToString(), dtSub.Rows[i]["wbSerialNum"].ToString(), dtSub.Rows[i]["swbSerialNum"].ToString(), dtSub.Rows[i]["swbDescription_CHN"].ToString(), dtSub.Rows[i]["swbDescription_ENG"].ToString(), dtSub.Rows[i]["swbNumber"].ToString(), dtSub.Rows[i]["swbWeight"].ToString(), dtSub.Rows[i]["swbValue"].ToString(), dtSub.Rows[i]["swbMonetary"].ToString(), dtSub.Rows[i]["swbRecipients"].ToString(), dtSub.Rows[i]["swbCustomsCategory"].ToString());

                                if (i == dtSub.Rows.Count - 1)
                                {
                                    sbSubWayBill.Append("}");
                                }
                                else
                                {
                                    sbSubWayBill.Append("},");
                                }
                            }
                            sbSubWayBill.Append("]}}");
                        }
                        else
                        {
                            sbSubWayBill.Append("\"SubWayBill\":{\"result\":\"error\",\"data\":[],\"message\":\"该总运单没有子运单\"}");
                        }
                        strRet = "{" + sbMainWayBill.ToString() + "," + sbSubWayBill.ToString() + "}";
                    }
                    else
                    {
                        sbMainWayBill.Append("\"MainWayBill\":{\"result\":\"error\",\"data\":[],\"message\":\"总运单信息有问题.请检查\"}");
                        strRet = "{" + sbMainWayBill.ToString() + sbSubWayBill.ToString() + "}";
                    }

                    sr.Close();

                }
            }
            catch (Exception ex)
            {
                sbMainWayBill.Append("\"MainWayBill\":{\"result\":\"error\",\"data\":[],\"message\":\"" + ex.Message + "\"}");
                strRet = "{" + sbMainWayBill.ToString() + sbSubWayBill.ToString() + "}";
            }

            return strRet;
        }


        //page=2&rows=10&sort=swbID&order=asc
        [HttpPost]
        public string GetSubWayBill(int page, int rows, string sort, string order)
        {
            string strRet = "";
            DataTable dtSub = null;
            StringBuilder sbSubWayBill = new StringBuilder("");
            int maxCount = -1;

            if (Session["Forwarder_DataUpLoad_SubWayBill"] != null)
            {
                dtSub = Session["Forwarder_DataUpLoad_SubWayBill"] as DataTable;
                dtSub.DefaultView.Sort = sort + " " + order;
                dtSub = dtSub.DefaultView.ToTable();
                Session["Forwarder_DataUpLoad_SubWayBill"] = dtSub;
                sbSubWayBill.Append("{\"total\":" + dtSub.Rows.Count + ",\"rows\":[");

                if (page > dtSub.Rows.Count / rows && page <= dtSub.Rows.Count / rows + 1)
                {
                    maxCount = dtSub.Rows.Count;
                }
                else
                {
                    maxCount = rows * page;
                }

                for (int i = rows * (page - 1); i < maxCount; i++)
                {
                    sbSubWayBill.Append("{");

                    sbSubWayBill.AppendFormat("\"swbID\":\"{0}\",\"wbSerialNum\":\"{1}\",\"swbSerialNum\":\"{2}\",\"swbDescription_CHN\":\"{3}\",\"swbDescription_ENG\":\"{4}\",\"swbNumber\":\"{5}\",\"swbWeight\":\"{6}\",\"swbValue\":\"{7}\",\"swbMonetary\":\"{8}\",\"swbRecipients\":\"{9}\",\"swbCustomsCategory\":\"{10}\"", dtSub.Rows[i]["swbID"].ToString(), dtSub.Rows[i]["wbSerialNum"].ToString(), dtSub.Rows[i]["swbSerialNum"].ToString(), dtSub.Rows[i]["swbDescription_CHN"].ToString(), dtSub.Rows[i]["swbDescription_ENG"].ToString(), dtSub.Rows[i]["swbNumber"].ToString(), dtSub.Rows[i]["swbWeight"].ToString(), dtSub.Rows[i]["swbValue"].ToString(), dtSub.Rows[i]["swbMonetary"].ToString(), dtSub.Rows[i]["swbRecipients"].ToString(), dtSub.Rows[i]["swbCustomsCategory"].ToString());

                    if (i == maxCount - 1)
                    {
                        sbSubWayBill.Append("}");
                    }
                    else
                    {
                        sbSubWayBill.Append("},");
                    }
                }
                sbSubWayBill.Append("]}");
            }
            else
            {
                sbSubWayBill.Append("{\"total\":" + 0 + ",\"rows\":[");

                sbSubWayBill.Append("]}");
            }

            strRet = sbSubWayBill.ToString();

            return strRet;
        }


        [HttpPost]
        public string SaveData(FormCollection collection)
        {
            string strRet = "";
            bool wbflag = false;
            bool subflag = false;
            int swID = 0;
            StringBuilder sbSWBSerialNum = new StringBuilder("");
            DataTable dtSubWayBill = null;

            wayBillModel = new M_WayBill();

            try
            {
                wayBillModel.WbSerialNum = collection["txtSeriaNum"].ToString();
                wayBillModel.WbVoyage = collection["txtWbVoyage"].ToString();
                wayBillModel.WbChinese = collection["txtWbIOmark"].ToString();
                wayBillModel.WbChinese = collection["txtWbChinese"].ToString();
                wayBillModel.WbEnglish = collection["txtWbEnglish"].ToString();
                wayBillModel.WbTransportMode = collection["txtWbTransportMode"].ToString();
                wayBillModel.WbEntryDate = collection["txtWbEntryDate"].ToString();
                wayBillModel.WbSRPort = collection["txtWbSRport"].ToString();
                wayBillModel.WbPortCode = collection["txtWbPortCode"].ToString();
                wayBillModel.WbTotalWeight = double.Parse(collection["txtWbTotalWeight"].ToString());
                wayBillModel.WbTotalNumber = int.Parse(collection["txtWbTotalNumber"].ToString());
                wayBillModel.WbSubNumber = int.Parse(collection["txtSubNumber"].ToString());
                wayBillModel.StorageDate = DateTime.Now.ToString("yyyyMMdd");

                if (wayBillSql.ExistWbSerialNum(wayBillModel.WbSerialNum))//不存在总运单号时则新增
                {
                    wbflag = false;
                    wbflag = wayBillSql.addWayBill(wayBillModel, Session["Global_Forwarder_UserName"] == null ? "" : Session["Global_Forwarder_UserName"].ToString());
                    if (wbflag)
                    {
                        swID = wayBillSql.GetWayBillID(wayBillModel.WbSerialNum);
                    }

                    if (swID != 0)
                    {
                        if (Session["Forwarder_DataUpLoad_SubWayBill"] != null)
                        {
                            dtSubWayBill = Session["Forwarder_DataUpLoad_SubWayBill"] as DataTable;
                            for (int i = 0; i < dtSubWayBill.Rows.Count; i++)
                            {
                                subWayBillModel = new M_SubWayBill();
                                subWayBillModel.Swb_wbID = swID;
                                subWayBillModel.SwbSerialNum = dtSubWayBill.Rows[i][2].ToString();
                                subWayBillModel.SwbDescription_CHN = dtSubWayBill.Rows[i][3].ToString();
                                subWayBillModel.SwbDescription_ENG = dtSubWayBill.Rows[i][4].ToString();
                                subWayBillModel.SwbNumber = int.Parse(dtSubWayBill.Rows[i][5].ToString());
                                subWayBillModel.SwbWeight = double.Parse(dtSubWayBill.Rows[i][6].ToString());
                                subWayBillModel.SwbValue = double.Parse(dtSubWayBill.Rows[i][7].ToString());
                                subWayBillModel.SwbMonetary = dtSubWayBill.Rows[i][8].ToString();
                                subWayBillModel.SwbRecipients = dtSubWayBill.Rows[i][9].ToString();
                                subWayBillModel.SwbCustomsCategory = dtSubWayBill.Rows[i][10].ToString();
                                subflag = subWayBillSql.addSubWayBill(subWayBillModel);

                                sbSWBSerialNum.AppendFormat("'{0}',", dtSubWayBill.Rows[i][2].ToString());
                            }

                            //更新库存异常表中信息
                            if (sbSWBSerialNum.ToString() != "")
                            {
                                if (sbSWBSerialNum.ToString().EndsWith(","))
                                {
                                    sbSWBSerialNum = new StringBuilder(sbSWBSerialNum.ToString().Substring(0, sbSWBSerialNum.ToString().Length - 1));
                                }
                                DataSet dsSwbSerialNum = (new T_WayBillFlow()).TestSWBSerialNumInStore(sbSWBSerialNum.ToString());
                                if (dsSwbSerialNum != null)
                                {
                                    DataTable dtSerialNum = dsSwbSerialNum.Tables[0];
                                    if (dtSerialNum != null && dtSerialNum.Rows.Count > 0)
                                    {
                                        for (int k = 0; k < dtSerialNum.Rows.Count; k++)
                                        {
                                            new T_WayBillFlow().FillwbIDswbID(dtSerialNum.Rows[k]["swbSerialNum"].ToString());
                                        }
                                    }
                                }
                            }
                        }
                        strRet = "{\"result\":\"ok\",\"message\":\"总运单为[" + wayBillModel.WbSerialNum + "]的总运单已经保存成功\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"总运单为[" + wayBillModel.WbSerialNum + "]的总运单已经保存失败\"}";
                    }
                }
                else//总运单已存在时则直接更新此总运单
                {
                    wbflag = false;
                    wbflag = wayBillSql.updateWayBill(wayBillModel, Session["Global_Forwarder_UserName"] == null ? "" : Session["Global_Forwarder_UserName"].ToString());
                    if (wbflag)
                    {
                        swID = wayBillSql.GetWayBillID(wayBillModel.WbSerialNum);
                    }

                    if (swID != 0)
                    {
                        subWayBillSql.DeleteSubWayBillByWBID(swID.ToString());

                        if (Session["Forwarder_DataUpLoad_SubWayBill"] != null)
                        {
                            dtSubWayBill = Session["Forwarder_DataUpLoad_SubWayBill"] as DataTable;
                            for (int i = 0; i < dtSubWayBill.Rows.Count; i++)
                            {
                                subWayBillModel = new M_SubWayBill();
                                subWayBillModel.Swb_wbID = swID;
                                subWayBillModel.SwbSerialNum = dtSubWayBill.Rows[i][2].ToString();
                                subWayBillModel.SwbDescription_CHN = dtSubWayBill.Rows[i][3].ToString();
                                subWayBillModel.SwbDescription_ENG = dtSubWayBill.Rows[i][4].ToString();
                                subWayBillModel.SwbNumber = int.Parse(dtSubWayBill.Rows[i][5].ToString());
                                subWayBillModel.SwbWeight = double.Parse(dtSubWayBill.Rows[i][6].ToString());
                                subWayBillModel.SwbValue = double.Parse(dtSubWayBill.Rows[i][7].ToString());
                                subWayBillModel.SwbMonetary = dtSubWayBill.Rows[i][8].ToString();
                                subWayBillModel.SwbRecipients = dtSubWayBill.Rows[i][9].ToString();
                                subWayBillModel.SwbCustomsCategory = dtSubWayBill.Rows[i][10].ToString();
                                subflag = subWayBillSql.addSubWayBill(subWayBillModel);

                                sbSWBSerialNum.AppendFormat("'{0}',", dtSubWayBill.Rows[i][2].ToString());
                            }
                            //更新库存异常表中信息
                            if (sbSWBSerialNum.ToString() != "")
                            {
                                if (sbSWBSerialNum.ToString().EndsWith(","))
                                {
                                    sbSWBSerialNum = new StringBuilder(sbSWBSerialNum.ToString().Substring(0, sbSWBSerialNum.ToString().Length - 1));
                                }
                                DataSet dsSwbSerialNum = (new T_WayBillFlow()).TestSWBSerialNumInStore(sbSWBSerialNum.ToString());
                                if (dsSwbSerialNum != null)
                                {
                                    DataTable dtSerialNum = dsSwbSerialNum.Tables[0];
                                    if (dtSerialNum != null && dtSerialNum.Rows.Count > 0)
                                    {
                                        for (int k = 0; k < dtSerialNum.Rows.Count; k++)
                                        {
                                            new T_WayBillFlow().FillwbIDswbID(dtSerialNum.Rows[k]["swbSerialNum"].ToString());
                                        }
                                    }
                                }
                            }
                        }
                        strRet = "{\"result\":\"ok\",\"message\":\"总运单为[" + wayBillModel.WbSerialNum + "]的总运单已经保存成功\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"总运单为[" + wayBillModel.WbSerialNum + "]的总运单已经保存失败\"}";
                    }
                    //strRet = "{\"result\":\"error\",\"message\":\"系统中已经存在编号为[" + wayBillModel.WbSerialNum + "]的总运单号\"}";
                }
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }
            Session["Forwarder_DataUpLoad_SubWayBill"] = null;
            return strRet;
        }


        [HttpGet]
        public string TestExistWBSerialNum(string strWBSerialNum)
        {
            string strRet = "{\"result\":\"false\",\"message\":\"\"}";
            strWBSerialNum = Server.UrlDecode(strWBSerialNum);
            if (!wayBillSql.ExistWbSerialNum(strWBSerialNum))
            {
                strRet = "{\"result\":\"true\",\"message\":\"\"}";
            }
            return strRet;
        }


        [HttpGet]
        public string TestExistSWBSerialNumInOtherWayBill(string wbSerialNum)
        {
            string strRet = "{\"result\":\"ok\",\"message\":\"\"}";
            string strSWBSerialNum = "";
            DataTable dtSubWayBill = null;
            if (Session["Forwarder_DataUpLoad_SubWayBill"] != null)
            {
                dtSubWayBill = Session["Forwarder_DataUpLoad_SubWayBill"] as DataTable;
                if (dtSubWayBill.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSubWayBill.Rows.Count; i++)
                    {
                        if (new T_SubWayBill().TestExistSWBSerialNumInOtherWayBill(wbSerialNum, dtSubWayBill.Rows[i]["swbSerialNum"].ToString()))
                        {
                            strSWBSerialNum = strSWBSerialNum + dtSubWayBill.Rows[i]["swbSerialNum"].ToString() + ",<br />";
                        }
                    }
                    if (strSWBSerialNum.EndsWith(",<br />"))
                    {
                        strSWBSerialNum = strSWBSerialNum.Substring(0, strSWBSerialNum.Length - ",<br />".Length);
                    }
                    if (strSWBSerialNum != "")
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"以下分单号已经在其他总运单中存在,是否继续保存?<br /><font style='color:red;font-weight:bold'>请谨慎选择</font><br/>" + strSWBSerialNum + "\"}";
                    }
                }
            }
            return strRet;
        }


        public string TestHasUploadData()
        {
            string strRet = "{\"result\":\"no\",\"message\":\"无上传数据或数据已过期，请重新上传\"}";

            if (Session["Forwarder_DataUpLoad_SubWayBill"] != null)
            {
                strRet = "{\"result\":\"yes\",\"message\":\"\"}";
            }
            return strRet;
        }
    }
}
