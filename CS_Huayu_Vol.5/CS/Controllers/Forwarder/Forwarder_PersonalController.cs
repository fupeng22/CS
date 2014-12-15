using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using System.Data;
using System.Text;
using System.IO;
using SQLDAL;
using System.Data.OleDb;
using CS.Filter;
using Util;

namespace CS.Controllers.Forwarder
{
    [ErrorAttribute]
    public class Forwarder_PersonalController : Controller
    {
        protected const string STR_SAVE_TXT_FILE = "~/Temp/xls/";

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

            HttpPostedFileBase excelFile = Request.Files["MyFile"];
            if (excelFile == null)
            {
                strRet = "{\"result\":\"error\",\"data\":\"null\",\"message\":\"为选择文件\"}";
            }
            else
            {
                if (excelFile.ContentLength == 0)
                {
                    strRet = "{\"result\":\"error\",\"data\":\"null\",\"message\":\"文件大小为0\"}";
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

            //int gg = 0;

            try
            {
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + url + ";" + "Extended Properties=\"Excel 8.0;HDR=No;IMEX=1\"";
                string strExcel = string.Format("select * from [{0}$]", "sheet1");
                DataSet ds = new DataSet();

                using (OleDbConnection conn = new OleDbConnection(strConn))
                {
                    conn.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(strExcel, strConn);
                    adapter.Fill(ds);
                    conn.Close();
                }

                DataTable dtBill = ds.Tables[0];
                if (dtBill.Rows.Count > 0)
                {
                    if (dtBill.Rows[0].ItemArray.Length >= 8)
                    {
                        sbMainWayBill.Append("\"MainWayBill\":{\"result\":\"ok\",\"message\":\"总运单数据正确\",\"data\":");
                        sbMainWayBill.Append("{\"txtSeriaNum\":\"" + JSONHelper.EncodeInValidStr(dtBill.Rows[1][1].ToString()) + "\",");
                        sbMainWayBill.Append("\"txtWbVoyage\":\"" + JSONHelper.EncodeInValidStr(dtBill.Rows[1][7].ToString()) + "\",");
                        sbMainWayBill.Append("\"txtWbIOmark\":\"" + "" + "\",");
                        sbMainWayBill.Append("\"txtWbChinese\":\"" + "" + "\",");
                        sbMainWayBill.Append("\"txtWbEnglish\":\"" + "" + "\",");
                        sbMainWayBill.Append("\"txtWbTotalWeight\":\"" + JSONHelper.EncodeInValidStr(dtBill.Rows[1][11].ToString()) + "\",");
                        sbMainWayBill.Append("\"txtWbTotalNumber\":\"" + JSONHelper.EncodeInValidStr(dtBill.Rows[1][9].ToString()) + "\",");
                        sbMainWayBill.Append("\"txtWbTransportMode\":\"" + "" + "\",");
                        sbMainWayBill.Append("\"txtWbEntryDate\":\"" + JSONHelper.EncodeInValidStr(dtBill.Rows[1][3].ToString()) + "\",");
                        sbMainWayBill.Append("\"txtWbSRport\":\"" + "" + "\",");
                        sbMainWayBill.Append("\"txtWbPortCode\":\"" + JSONHelper.EncodeInValidStr(dtBill.Rows[1][5].ToString()) + "\",");
                        sbMainWayBill.Append("\"txtSubNumber\":\"" + JSONHelper.EncodeInValidStr(dtBill.Rows[1][13].ToString()) + "\"}}");

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
                        dtSub.Columns.Add("swbValueDetail", Type.GetType("System.String"));

                        int iCount = 0;
                        int iInterval = 0;

                        for (int i = 3; i < dtBill.Rows.Count; i = i + iInterval)
                        {
                            if (dtBill.Rows[i][0].ToString() != "")
                            {
                                DataRow row = dtSub.NewRow();
                                iCount++;
                                int n = 0;
                                string str_swbDescription_CHN = "";
                                string str_swbValue = "";
                                double i_swbValue = 0;
                                double temp_swbValue = 0;
                                if (dtBill.Rows[i].ItemArray.Length >= 13)
                                {
                                    row[0] = iCount;
                                    row[1] = JSONHelper.EncodeInValidStr(dtBill.Rows[1][1].ToString());
                                    row[2] = JSONHelper.EncodeInValidStr(dtBill.Rows[i][1].ToString());
                                    row[4] = "";
                                    row[5] = JSONHelper.EncodeInValidStr(dtBill.Rows[i][8].ToString());
                                    row[6] = JSONHelper.EncodeInValidStr(dtBill.Rows[i][9].ToString());
                                    row[8] = "";
                                    row[9] = JSONHelper.EncodeInValidStr(dtBill.Rows[i][5].ToString());
                                    row[10] = JSONHelper.EncodeInValidStr(dtBill.Rows[i][12].ToString());

                                    str_swbDescription_CHN = str_swbDescription_CHN + JSONHelper.EncodeInValidStr(dtBill.Rows[i][6].ToString()) + ",";
                                    str_swbValue = str_swbValue + JSONHelper.EncodeInValidStr(dtBill.Rows[i][7].ToString()) + ",";
                                    try
                                    {
                                        temp_swbValue = Convert.ToDouble(JSONHelper.EncodeInValidStr(dtBill.Rows[i][7].ToString()));
                                    }
                                    catch (Exception ex)
                                    {
                                        temp_swbValue = 0;
                                    }
                                    i_swbValue = i_swbValue + temp_swbValue;
                                }
                                if (i != dtBill.Rows.Count - 1)
                                {
                                    int iMergeRowsCount = 0;
                                    for (int j = i + 1; j < dtBill.Rows.Count; j++)
                                    {
                                        iMergeRowsCount = iMergeRowsCount + 1;
                                        if (j != dtBill.Rows.Count - 1)
                                        {
                                            if (dtBill.Rows[j].ItemArray.Length >= 13)
                                            {
                                                if (JSONHelper.EncodeInValidStr(dtBill.Rows[j][1].ToString()) != "")
                                                {
                                                    iInterval = iMergeRowsCount;
                                                    break;
                                                }
                                                else
                                                {
                                                    str_swbDescription_CHN = str_swbDescription_CHN + JSONHelper.EncodeInValidStr(dtBill.Rows[j][6].ToString()) + ",";
                                                    str_swbValue = str_swbValue + JSONHelper.EncodeInValidStr(dtBill.Rows[j][7].ToString()) + ",";
                                                    try
                                                    {
                                                        temp_swbValue = Convert.ToDouble(JSONHelper.EncodeInValidStr(dtBill.Rows[j][7].ToString()));
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        temp_swbValue = 0;
                                                    }
                                                    i_swbValue = i_swbValue + temp_swbValue;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (dtBill.Rows[j].ItemArray.Length >= 13)
                                            {
                                                str_swbDescription_CHN = str_swbDescription_CHN + JSONHelper.EncodeInValidStr(dtBill.Rows[j][6].ToString()) + ",";
                                                str_swbValue = str_swbValue + JSONHelper.EncodeInValidStr(dtBill.Rows[j][7].ToString()) + ",";
                                                try
                                                {
                                                    temp_swbValue = Convert.ToDouble(JSONHelper.EncodeInValidStr(dtBill.Rows[j][7].ToString()));
                                                }
                                                catch (Exception ex)
                                                {
                                                    temp_swbValue = 0;
                                                }
                                                i_swbValue = i_swbValue + temp_swbValue;
                                            }
                                        }

                                    }
                                }
                                if (str_swbDescription_CHN.EndsWith(","))
                                {
                                    str_swbDescription_CHN = str_swbDescription_CHN.Substring(0, str_swbDescription_CHN.Length - 1);
                                }
                                if (str_swbValue.EndsWith(","))
                                {
                                    str_swbValue = str_swbValue.Substring(0, str_swbValue.Length - 1);
                                }
                                row[3] = str_swbDescription_CHN;
                                row[7] = i_swbValue;
                                row[11] = str_swbValue;
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
                }
                else
                {
                    sbMainWayBill.Append("\"MainWayBill\":{\"result\":\"error\",\"data\":[],\"message\":\"总运单信息有问题.请检查\"}");
                    strRet = "{" + sbMainWayBill.ToString() + sbSubWayBill.ToString() + "}";
                }

            }
            catch (Exception ex)
            {
                //sbMainWayBill.Append("\"MainWayBill\":{\"result\":\"error\",\"data\":[],\"message\":\"" + ex.Message + "\"}");
                //strRet = "{" + sbMainWayBill.ToString() + sbSubWayBill.ToString() + "}";

                sbMainWayBill.Append("\"MainWayBill\":{\"result\":\"error\",\"data\":[],\"message\":\"" + JSONHelper.EncodeInValidStr(ex.Message) + "\"}");
                strRet = "{\"MainWayBill\":{\"result\":\"error\",\"data\":[],\"message\":\"" + JSONHelper.EncodeInValidStr(ex.Message) + "\"},\"SubWayBill\":{\"result\":\"error\",\"message\":\"" + JSONHelper.EncodeInValidStr(ex.Message) + "\",\"data\":[]}}";
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
                wayBillModel.WbTotalWeight = double.Parse(collection["txtWbTotalWeight"].ToString() == "" ? "0" : collection["txtWbTotalWeight"].ToString());
                wayBillModel.WbTotalNumber = int.Parse(collection["txtWbTotalNumber"].ToString() == "" ? "0" : collection["txtWbTotalNumber"].ToString());
                wayBillModel.WbSubNumber = int.Parse(collection["txtSubNumber"].ToString() == "" ? "0" : collection["txtSubNumber"].ToString());
                wayBillModel.StorageDate = DateTime.Now.ToString("yyyyMMdd");

                if (wayBillSql.ExistWbSerialNum(wayBillModel.WbSerialNum))
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
                                subWayBillModel.SwbNumber = int.Parse(dtSubWayBill.Rows[i][5].ToString() == "" ? "0" : dtSubWayBill.Rows[i][5].ToString());
                                subWayBillModel.SwbWeight = double.Parse(dtSubWayBill.Rows[i][6].ToString() == "" ? "0" : dtSubWayBill.Rows[i][6].ToString());
                                subWayBillModel.SwbValue = double.Parse(dtSubWayBill.Rows[i][7].ToString() == "" ? "0" : dtSubWayBill.Rows[i][7].ToString());
                                subWayBillModel.SwbMonetary = dtSubWayBill.Rows[i][8].ToString();
                                subWayBillModel.SwbRecipients = dtSubWayBill.Rows[i][9].ToString();
                                subWayBillModel.SwbCustomsCategory = dtSubWayBill.Rows[i][10].ToString();
                                subWayBillModel.swbValueDetail = dtSubWayBill.Rows[i][11].ToString();
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
                else
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
                                //if (i==dtSubWayBill.Rows.Count-2)
                                //{
                                //    string ggg = "";
                                //}
                                subWayBillModel = new M_SubWayBill();
                                subWayBillModel.Swb_wbID = swID;
                                subWayBillModel.SwbSerialNum = dtSubWayBill.Rows[i][2].ToString();
                                subWayBillModel.SwbDescription_CHN = dtSubWayBill.Rows[i][3].ToString();
                                subWayBillModel.SwbDescription_ENG = dtSubWayBill.Rows[i][4].ToString();
                                subWayBillModel.SwbNumber = int.Parse(dtSubWayBill.Rows[i][5].ToString() == "" ? "0" : dtSubWayBill.Rows[i][5].ToString());
                                subWayBillModel.SwbWeight = double.Parse(dtSubWayBill.Rows[i][6].ToString() == "" ? "0" : dtSubWayBill.Rows[i][6].ToString());
                                subWayBillModel.SwbValue = double.Parse(dtSubWayBill.Rows[i][7].ToString() == "" ? "0" : dtSubWayBill.Rows[i][7].ToString());
                                subWayBillModel.SwbMonetary = dtSubWayBill.Rows[i][8].ToString();
                                subWayBillModel.SwbRecipients = dtSubWayBill.Rows[i][9].ToString();
                                subWayBillModel.SwbCustomsCategory = dtSubWayBill.Rows[i][10].ToString();
                                subWayBillModel.swbValueDetail = dtSubWayBill.Rows[i][11].ToString();
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
