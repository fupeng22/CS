using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using SQLDAL;
using System.Data;
using System.Text;
using System.IO;
using CS.Filter;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_WayBillPatchInController : Controller
    {
        protected const string STR_SAVE_TXT_FILE = "~/Temp/txt/";

        Model.M_WayBill wayBillModel = new Model.M_WayBill();
        Model.M_SubWayBill subWayBillModel = new M_SubWayBill();
        Model.M_WayBillFlow wayBillFlowModel = new M_WayBillFlow();
        Model.M_WayBillLog wayBillLogModel = new M_WayBillLog();

        SQLDAL.T_WayBill wayBillSql = new T_WayBill();
        SQLDAL.T_SubWayBill subWayBillSql = new T_SubWayBill();
        SQLDAL.T_WayBillFlow wayBillFlowSql = new T_WayBillFlow();
        SQLDAL.T_WayBillLog wayBillLogSql = new T_WayBillLog();

        //
        // GET: /DataUpLoad/
        [HuayuRequiresLoginAttribute]
        public ActionResult Index()
        {
            TempData["Forwarder_DataUpLoad_SubWayBill"] = null;
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
                    if (strSourceFileNameExtensionName.ToLower() == "txt")
                    {
                        strFullFilePath = Server.MapPath(STR_SAVE_TXT_FILE + strSourceFileNameWithOutExtension + strFileName + "." + strSourceFileNameExtensionName);

                        try
                        {
                            txtFile.SaveAs(strFullFilePath);

                            txtToDatatable(strFullFilePath);

                            strRet = "{\"result\":\"ok\",\"data\":\"null\",\"message\":\"" + "" + "\"}";
                        }
                        catch (Exception ex)
                        {
                            strRet = "{\"result\":\"error\",\"data\":\"null\",\"message\":\"" + ex.Message + "\"}";
                        }
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"data\":\"null\",\"message\":\"" + "请选择txt格式文件" + "\"}";
                    }

                }
            }

            return strRet;
        }

        
        public void txtToDatatable(string url)
        {
            DataTable dt = new DataTable();

            string str_wbSerialNum = "";
            string str_swbSerialNum = "";

            DataRow row = null;

            try
            {
                using (StreamReader sr = new StreamReader(url, System.Text.Encoding.GetEncoding("gb2312")))
                {
                    String line = "";
                    string[] seperator = { "||" };

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
                    dtSub.Columns.Add("singal", Type.GetType("System.Int32"));
                    //singal:
                    //0：未找到预入库记录
                    //1:入库异常(待处理)  
                    //2:入库异常(处理中)  
                    //3:入库异常(已处理)  
                    //4：已入库
                    //5：出库异常(待处理)
                    //6：出库异常(处理中)
                    //7：出库异常(已处理)
                    //8：已出库
                    //9:未知
                    //10:数据格式不正确
                    //99:正常，可以入库，即预入库中存在，并且以前并没有入库正常过
                    dtSub.Columns.Add("status", Type.GetType("System.Int32"));
                    dtSub.Columns.Add("StatusDecription", Type.GetType("System.String"));
                    dtSub.Columns.Add("ExceptionStatus", Type.GetType("System.Int32"));
                    dtSub.Columns.Add("ExceptionStatusDescription", Type.GetType("System.String"));
                    dtSub.Columns.Add("Wbf_wbID", Type.GetType("System.Int32"));
                    dtSub.Columns.Add("Wbf_swbID", Type.GetType("System.Int32"));

                    int iCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] array = line.Split(seperator, StringSplitOptions.None);
                        if (array.Length >= 2)
                        {
                            str_wbSerialNum = array[0];
                            str_swbSerialNum = array[1];

                            if (str_wbSerialNum == "" || str_swbSerialNum == "")
                            {
                                if (str_wbSerialNum == "" && str_swbSerialNum == "")
                                {

                                }
                                else
                                {
                                    row = dtSub.NewRow();

                                    row[1] = str_wbSerialNum;
                                    row[2] = str_swbSerialNum;
                                    row[3] = "";
                                    row[4] = "";
                                    row[5] = 0;
                                    row[6] = 0;
                                    row[7] = 0;
                                    row[8] = "";
                                    row[9] = "";
                                    row[10] = "";
                                    row[11] = 10;
                                    row[12] = 0;
                                    row[13] = "数据格式不正确(单号不完整)";
                                    row[14] = -1;
                                    row[15] = "数据格式不正确(单号不完整)";
                                    row[16] = -1;
                                    row[17] = -1;
                                    if (row[1] != null)
                                    {
                                        iCount = iCount + 1;
                                        row[0] = iCount;
                                        dtSub.Rows.Add(row);
                                    }
                                }

                            }
                            else
                            {
                                row = dtSub.NewRow();
                                if (wayBillFlowSql.ExistInForeWayBill(str_wbSerialNum, str_swbSerialNum))
                                {
                                    DataSet ds = wayBillFlowSql.getForeWayBill(str_wbSerialNum, str_swbSerialNum);
                                    if (ds != null)
                                    {
                                        DataTable dtTemp = ds.Tables[0];
                                        if (dtTemp != null && dtTemp.Rows.Count > 0)
                                        {
                                            row[1] = str_wbSerialNum;
                                            row[2] = str_swbSerialNum;
                                            row[3] = dtTemp.Rows[0]["swbDescription_CHN"] == DBNull.Value ? "" : dtTemp.Rows[0]["swbDescription_CHN"].ToString();
                                            row[4] = dtTemp.Rows[0]["swbDescription_ENG"] == DBNull.Value ? "" : dtTemp.Rows[0]["swbDescription_ENG"].ToString();
                                            row[5] = dtTemp.Rows[0]["swbNumber"] == DBNull.Value ? "" : dtTemp.Rows[0]["swbNumber"].ToString();
                                            row[6] = dtTemp.Rows[0]["swbWeight"] == DBNull.Value ? "" : dtTemp.Rows[0]["swbWeight"].ToString();
                                            row[7] = dtTemp.Rows[0]["swbValue"] == DBNull.Value ? 0 : Convert.ToDouble(dtTemp.Rows[0]["swbValue"]);
                                            row[8] = dtTemp.Rows[0]["swbMonetary"] == DBNull.Value ? "" : dtTemp.Rows[0]["swbMonetary"].ToString();
                                            row[9] = dtTemp.Rows[0]["swbRecipients"] == DBNull.Value ? "" : dtTemp.Rows[0]["swbRecipients"].ToString();
                                            row[10] = dtTemp.Rows[0]["swbCustomsCategory"] == DBNull.Value ? "" : dtTemp.Rows[0]["swbCustomsCategory"].ToString();
                                            row[11] = 9;
                                            row[12] = 0;
                                            row[13] = "";
                                            row[14] = -1;
                                            row[15] = "";
                                            row[16] = dtTemp.Rows[0]["wbID"] == DBNull.Value ? -1 : Convert.ToInt32(dtTemp.Rows[0]["wbID"].ToString());
                                            row[17] = dtTemp.Rows[0]["swbID"] == DBNull.Value ? -1 : Convert.ToInt32(dtTemp.Rows[0]["swbID"].ToString());

                                            int iStatus = -99;
                                            int iExceptionStatus = -99;
                                            if (wayBillFlowSql.ExistInStoreWayBill(str_wbSerialNum, str_swbSerialNum))
                                            {
                                                DataSet dsWayBillFlow = wayBillFlowSql.getWayBillFlow(str_wbSerialNum, str_swbSerialNum);
                                                if (dsWayBillFlow != null)
                                                {
                                                    DataTable dtWayBillFlow = dsWayBillFlow.Tables[0];
                                                    if (dtWayBillFlow != null && dtWayBillFlow.Rows.Count > 0)
                                                    {
                                                        iStatus = Int32.Parse(dtWayBillFlow.Rows[0]["status"].ToString());
                                                        iExceptionStatus = Int32.Parse(dtWayBillFlow.Rows[0]["ExceptionStatus"].ToString());

                                                        switch (iStatus)
                                                        {
                                                            case 1://正常入库
                                                                row[11] = 4;
                                                                break;
                                                            case 2://异常入库
                                                                switch (iExceptionStatus)
                                                                {
                                                                    case 0://未处理
                                                                        row[11] = 1;
                                                                        break;
                                                                    case 1://正在处理
                                                                        row[11] = 2;
                                                                        break;
                                                                    case 2://已处理
                                                                        row[11] = 3;
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                                break;
                                                            case 3://正常已出库
                                                                row[11] = 8;
                                                                break;
                                                            case 4://异常出库
                                                                switch (iExceptionStatus)
                                                                {
                                                                    case 0://未处理
                                                                        row[11] = 5;
                                                                        break;
                                                                    case 1://正在处理
                                                                        row[11] = 6;
                                                                        break;
                                                                    case 2://已处理
                                                                        row[11] = 7;
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                                break;
                                                            default:
                                                                break;
                                                        }

                                                        row[12] = dtWayBillFlow.Rows[0]["status"] == DBNull.Value ? "" : dtWayBillFlow.Rows[0]["status"].ToString();
                                                        row[13] = dtWayBillFlow.Rows[0]["StatusDecription"] == DBNull.Value ? "" : dtWayBillFlow.Rows[0]["StatusDecription"].ToString();
                                                        row[14] = dtWayBillFlow.Rows[0]["ExceptionStatus"] == DBNull.Value ? "" : dtWayBillFlow.Rows[0]["ExceptionStatus"].ToString();
                                                        row[15] = dtWayBillFlow.Rows[0]["ExceptionStatusDescription"] == DBNull.Value ? "" : dtWayBillFlow.Rows[0]["ExceptionStatusDescription"].ToString();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                row[11] = 99;
                                                row[12] = 0;
                                                row[13] = "可以入库";
                                                row[14] = -1;
                                                row[15] = "可以入库";
                                            }
                                        }
                                    }


                                }
                                else//不在预入库表中
                                {
                                    row[1] = str_wbSerialNum;
                                    row[2] = str_swbSerialNum;
                                    row[3] = "";
                                    row[4] = "";
                                    row[5] = 0;
                                    row[6] = 0;
                                    row[7] = 0;
                                    row[8] = "";
                                    row[9] = "";
                                    row[10] = "";
                                    row[11] = 0;
                                    row[12] = 0;
                                    row[13] = "未预入库";
                                    row[14] = -1;
                                    row[15] = "未预入库";
                                    row[16] = -1;
                                    row[17] = -1;
                                }

                                if (row[1] != null)
                                {
                                    iCount = iCount + 1;
                                    row[0] = iCount;
                                    dtSub.Rows.Add(row);
                                }
                            }
                        }
                        else//此行记录格式不正确
                        {
                            row = dtSub.NewRow();
                            str_wbSerialNum = "";
                            str_swbSerialNum = "";
                            switch (array.Length)
                            {
                                case 1:
                                    str_wbSerialNum = array[0];
                                    break;
                                default:
                                    break;
                            }
                            if (str_wbSerialNum == "" && str_swbSerialNum == "")
                            {

                            }
                            else
                            {
                                row[1] = str_wbSerialNum;
                                row[2] = str_swbSerialNum;
                                row[3] = "";
                                row[4] = "";
                                row[5] = 0;
                                row[6] = 0;
                                row[7] = 0;
                                row[8] = "";
                                row[9] = "";
                                row[10] = "";
                                row[11] = 10;
                                row[12] = 0;
                                row[13] = "数据格式不正确(单号不完整)";
                                row[14] = -1;
                                row[15] = "数据格式不正确(单号不完整)";
                                row[16] = -1;
                                row[17] = -1;
                                if (row[1] != null)
                                {
                                    iCount = iCount + 1;
                                    row[0] = iCount;
                                    dtSub.Rows.Add(row);
                                }
                            }

                        }

                    }
                    sr.Close();

                    if (iCount > 0)
                    {
                        TempData["Forwarder_DataUpLoad_SubWayBill"] = dtSub;
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        
        //page=2&rows=10&sort=swbID&order=asc
        [HttpPost]
        public string GetSubWayBill(int page, int rows, string sort, string order)
        {
            string strRet = "";
            DataTable dtSub = null;
            StringBuilder sbSubWayBill = new StringBuilder("");
            int maxCount = -1;

            if (TempData["Forwarder_DataUpLoad_SubWayBill"] != null)
            {
                dtSub = TempData["Forwarder_DataUpLoad_SubWayBill"] as DataTable;
                dtSub.DefaultView.Sort = sort + " " + order;
                dtSub = dtSub.DefaultView.ToTable();
                TempData["Forwarder_DataUpLoad_SubWayBill"] = dtSub;
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

                    sbSubWayBill.AppendFormat("\"swbID\":\"{0}\",\"wbSerialNum\":\"{1}\",\"swbSerialNum\":\"{2}\",\"swbDescription_CHN\":\"{3}\",\"swbDescription_ENG\":\"{4}\",\"swbNumber\":\"{5}\",\"swbWeight\":\"{6}\",\"swbValue\":\"{7}\",\"swbMonetary\":\"{8}\",\"swbRecipients\":\"{9}\",\"swbCustomsCategory\":\"{10}\",\"singal\":\"{11}\",\"status\":\"{12}\",\"StatusDecription\":\"{13}\",\"ExceptionStatus\":\"{14}\",\"ExceptionStatusDescription\":\"{15}\"", dtSub.Rows[i]["swbID"].ToString(), dtSub.Rows[i]["wbSerialNum"].ToString(), dtSub.Rows[i]["swbSerialNum"].ToString(), dtSub.Rows[i]["swbDescription_CHN"].ToString(), dtSub.Rows[i]["swbDescription_ENG"].ToString(), dtSub.Rows[i]["swbNumber"].ToString(), dtSub.Rows[i]["swbWeight"].ToString(), dtSub.Rows[i]["swbValue"].ToString(), dtSub.Rows[i]["swbMonetary"].ToString(), dtSub.Rows[i]["swbRecipients"].ToString(), dtSub.Rows[i]["swbCustomsCategory"].ToString(), dtSub.Rows[i]["singal"].ToString(), dtSub.Rows[i]["status"].ToString(), dtSub.Rows[i]["StatusDecription"].ToString(), dtSub.Rows[i]["ExceptionStatus"].ToString(), dtSub.Rows[i]["ExceptionStatusDescription"].ToString());

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
        public string SaveData()
        {
            string strRet = "";

            DataTable dtSubWayBill = null;

            wayBillFlowSql = new T_WayBillFlow();

            try
            {
                if (TempData["Forwarder_DataUpLoad_SubWayBill"] != null)
                {
                    dtSubWayBill = TempData["Forwarder_DataUpLoad_SubWayBill"] as DataTable;
                    for (int i = 0; i < dtSubWayBill.Rows.Count; i++)
                    {
                        string str_wbSerialNum = "";
                        string str_swbSerialNum = "";
                        string str_userName = "";

                        str_wbSerialNum = dtSubWayBill.Rows[i]["wbSerialNum"].ToString();
                        str_swbSerialNum = dtSubWayBill.Rows[i]["swbSerialNum"].ToString();
                        str_userName = Session["Global_Huayu_UserName"] == null ? "" : Session["Global_Huayu_UserName"].ToString();

                        wayBillLogModel = new M_WayBillLog();
                        wayBillLogModel.Wbl_wbSerialNum = str_wbSerialNum;
                        wayBillLogModel.Wbl_swbSerialNum = str_swbSerialNum;
                        wayBillLogModel.Operator = str_userName;

                        switch (dtSubWayBill.Rows[i]["singal"].ToString())
                        {
                            case "99":
                                wayBillFlowSql.UpdateStatus(str_wbSerialNum, str_swbSerialNum, 1, str_userName,"0","0","0");
                                break;
                            case "0":
                                wayBillLogModel.status = 1;
                                new T_WayBillFlow().AddRecordForNoForeStore(str_swbSerialNum, Session["Global_Huayu_UserName"] == null ? "" : Session["Global_Huayu_UserName"].ToString(), 2);
                                break;
                            case "1":
                                wayBillLogModel.status = 4;
                                break;
                            case "2":
                                wayBillLogModel.status = 4;
                                break;
                            case "3":
                                wayBillLogModel.status = 4;
                                break;
                            case "4":
                                wayBillLogModel.status = 2;
                                break;
                            case "8":
                                wayBillLogModel.status = 3;
                                break;
                            case "5":
                                wayBillLogModel.status = 5;
                                break;
                            case "6":
                                wayBillLogModel.status = 5;
                                break;
                            case "7":
                                wayBillLogModel.status = 5;
                                break;
                            case "10":
                                wayBillLogModel.status = 11;
                                break;
                            case "9":
                                wayBillLogModel.status = 99;
                                break;
                            default:
                                break;
                        }

                        switch (dtSubWayBill.Rows[i]["singal"].ToString())
                        {
                            case "99":
                                break;
                            default:
                                wayBillLogSql.InsertLog(wayBillLogModel);
                                break;
                        }
                    }
                    strRet = "{\"result\":\"ok\",\"message\":\"所导入的数据已经完成入仓了\"}";
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "您还没有上传需要进行入仓的运单数据" + "\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }


            return strRet;
        }

    }
}
