using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Filter;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using DBUtility;
using Microsoft.Reporting.WebForms;
using System.IO;
using SQLDAL;
using Model;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_ConfirmInStoreController : Controller
    {
        SQLDAL.T_WayBillFlow tWayBillFlow = new T_WayBillFlow();
        public const string strFileds = "wbStorageDate,wbCompany,wbSerialNum,swbSerialNum,swbDescription_CHN,swbDescription_ENG,swbNumber,swbWeight,swbNeedCheck,swbNeedCheckDescription,wbID,swbID";
        public const string strFileds_AllWayBill = "wbStorageDate,wbCompany,wbSerialNum,wbTotalNumber,wbSubNumber,wbTotalWeight,wbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/HuayuForeStore.rdlc";
        //
        // GET: /Forwarder_QueryCompany/

        [HuayuRequiresLoginAttribute]
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 分页查询类
        /// </summary>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public string GetData(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtVoyage, string txtCode, string txtSubWayBillCode, string NeedCheck)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Fore_WayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "swbID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = sort + " " + order;

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(page);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = "";

            txtBeginD = Server.UrlDecode(txtBeginD.ToString());
            txtEndD = Server.UrlDecode(txtEndD.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());
            txtSubWayBillCode = Server.UrlDecode(txtSubWayBillCode.ToString());
            NeedCheck = Server.UrlDecode(NeedCheck.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbStorageDate>='{0}' and wbStorageDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbStorageDate>='{0}' and wbStorageDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbCompany like '%{0}%') ", txtVoyage);
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
            }

            if (NeedCheck != "" && NeedCheck != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbNeedCheck in ({0})) ", NeedCheck);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (swbNeedCheck in ({0})) ", NeedCheck);
                }
            }

            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            StringBuilder sb = new StringBuilder("");
            sb.Append("{");
            sb.AppendFormat("\"total\":{0}", Convert.ToInt32(param[7].Value.ToString()));
            sb.Append(",\"rows\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("{");

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        default:
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
                            }
                            break;
                    }



                }

                if (i == dt.Rows.Count - 1)
                {
                    sb.Append("}");
                }
                else
                {
                    sb.Append("},");
                }
            }
            dt = null;
            if (sb.ToString().EndsWith(","))
            {
                sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }


        [HttpGet]
        public ActionResult Print(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtVoyage, string txtCode, string txtSubWayBillCode, string NeedCheck)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Fore_WayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "swbID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = sort + " " + order;

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(page);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = "";

            txtBeginD = Server.UrlDecode(txtBeginD.ToString());
            txtEndD = Server.UrlDecode(txtEndD.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());
            txtSubWayBillCode = Server.UrlDecode(txtSubWayBillCode.ToString());
            NeedCheck = Server.UrlDecode(NeedCheck.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbStorageDate>='{0}' and wbStorageDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbStorageDate>='{0}' and wbStorageDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbCompany like '%{0}%') ", txtVoyage);
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
            }

            if (NeedCheck != "" && NeedCheck != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbNeedCheck in ({0})) ", NeedCheck);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (swbNeedCheck in ({0})) ", NeedCheck);
                }
            }
            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNeedCheck", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNeedCheckDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbID", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }
                if (drCustom["swbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("HuayuForeStore_DS", dtCustom);

            localReport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding = "UTF-8";
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
                " <OutputFormat>PDF</OutputFormat>" +
                " <PageWidth>12in</PageWidth>" +
                " <PageHeigth>11in</PageHeigth>" +
                " <MarginTop>0.5in</MarginTop>" +
                " <MarginLeft>1in</MarginLeft>" +
                " <MarginRight>1in</MarginRight>" +
                " <MarginBottom>0.5in</MarginBottom>" +
                " </DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            return File(renderedBytes, mimeType);
        }


        [HttpGet]
        public ActionResult Excel(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtVoyage, string txtCode, string txtSubWayBillCode, string NeedCheck, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Fore_WayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "swbID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = sort + " " + order;

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(page);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = "";

            txtBeginD = Server.UrlDecode(txtBeginD.ToString());
            txtEndD = Server.UrlDecode(txtEndD.ToString());
            txtVoyage = Server.UrlDecode(txtVoyage.ToString());
            txtCode = Server.UrlDecode(txtCode.ToString());
            txtSubWayBillCode = Server.UrlDecode(txtSubWayBillCode.ToString());
            NeedCheck = Server.UrlDecode(NeedCheck.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbStorageDate>='{0}' and wbStorageDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbStorageDate>='{0}' and wbStorageDate<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyyMMdd"), Convert.ToDateTime(txtEndD).ToString("yyyyMMdd"));
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbCompany like '%{0}%') ", txtVoyage);
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
            }

            if (NeedCheck != "" && NeedCheck != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbNeedCheck in ({0})) ", NeedCheck);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (swbNeedCheck in ({0})) ", NeedCheck);
                }
            }
            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            DataTable dtCustom = new DataTable();
            dtCustom.Columns.Add("wbStorageDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNeedCheck", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNeedCheckDescription", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbID", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbID", Type.GetType("System.String"));

            DataRow drCustom = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();

                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }
                }
                if (drCustom["swbID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("HuayuForeStore_DS", dtCustom);

            localReport.DataSources.Add(reportDataSource);

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = localReport.Render(
               "Excel", null, out mimeType, out encoding, out extension,
               out streamids, out warnings);
            string strFileName = Server.MapPath(STR_TEMPLATE_EXCEL);
            FileStream fs = new FileStream(strFileName, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            string strOutputFileName = "预入库信息_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            switch (browserType.ToLower())
            {
                case "safari":
                    break;
                case "mozilla":
                    break;
                default:
                    strOutputFileName = HttpUtility.UrlEncode(strOutputFileName);
                    break;
            }

            return File(strFileName, "application/vnd.ms-excel", strOutputFileName);
        }

        [HttpPost]
        public string PatchInOutStore(string ids_wbSerialNum, string ids_swbSerialNum, string iType)
        {
            string strDesc = "";
            string[] arrWBSerialNum = null;
            string[] arrSWBSerialNum = null;
            arrWBSerialNum = Server.UrlDecode(ids_wbSerialNum).Split(',');
            arrSWBSerialNum = Server.UrlDecode(ids_swbSerialNum).Split(',');
            switch (iType)
            {
                case "1":
                    strDesc = "入仓";
                    break;
                case "3":
                    strDesc = "出仓";
                    break;
                default:
                    break;
            }
            string strRet = "{\"result\":\"error\",\"message\":\"" + strDesc + "失败，原因未知\"}";

            try
            {
                if (arrWBSerialNum.Length == arrSWBSerialNum.Length)
                {
                    for (int i = 0; i < arrWBSerialNum.Length; i++)
                    {
                        if (arrWBSerialNum[i] != "" && arrSWBSerialNum[i] != "")
                        {
                            tWayBillFlow.UpdateStatus(arrWBSerialNum[i], arrSWBSerialNum[i], Convert.ToInt32(iType), Session["Global_Huayu_UserName"] == null ? "" : Session["Global_Huayu_UserName"].ToString(),"0","0","0");
                        }
                    }

                }

                strRet = "{\"result\":\"ok\",\"message\":\"" + strDesc + "完成\"}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string ddlwbSerialNum = collection["ddlwbSerialNum"].ToString();
                string txtSwbSerialNum = collection["txtSwbSerialNum"].ToString();
                string txtSwbDescription_CHN = collection["txtSwbDescription_CHN"].ToString();
                string txtSwbDescription_ENG = collection["txtSwbDescription_ENG"].ToString();
                string txtSwbNumber = collection["txtSwbNumber"].ToString();
                string txtSwbWeight = collection["txtSwbWeight"].ToString();
                string txtValue = collection["txtValue"].ToString();
                string txtSwbMonetary = collection["txtSwbMonetary"].ToString();
                string ddlSwbCustomsCategory = collection["ddlSwbCustomsCategory"].ToString();
                string txtSwbRecipients = collection["txtSwbRecipients"].ToString();

                string wbID = "";

                DataSet ds = (new T_WayBill()).GetWayBill(ddlwbSerialNum);
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        wbID = dt.Rows[0]["wbID"].ToString();

                        M_SubWayBill subWayBillModel = new M_SubWayBill();

                        subWayBillModel.Swb_wbID = Convert.ToInt32(wbID);
                        subWayBillModel.SwbSerialNum = txtSwbSerialNum;
                        subWayBillModel.SwbDescription_CHN = txtSwbDescription_CHN;
                        subWayBillModel.SwbDescription_ENG = txtSwbDescription_ENG;
                        subWayBillModel.SwbNumber = int.Parse(txtSwbNumber == "" ? "0" : txtSwbNumber);
                        subWayBillModel.SwbWeight = double.Parse(txtSwbWeight == "" ? "0" : txtSwbWeight);
                        subWayBillModel.SwbValue = double.Parse(txtValue == "" ? "0" : txtValue);
                        subWayBillModel.SwbMonetary = txtSwbMonetary;
                        subWayBillModel.SwbRecipients = txtSwbRecipients;
                        subWayBillModel.SwbCustomsCategory = ddlSwbCustomsCategory;
                        new T_SubWayBill().addSubWayBill(subWayBillModel);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        ///// <summary>
        ///// 分页查询类
        ///// </summary>
        ///// <param name="order"></param>
        ///// <param name="page"></param>
        ///// <param name="rows"></param>
        ///// <param name="sort"></param>
        ///// <returns></returns>
        //public string GetAllWayBillData(string order, string page, string rows, string sort)
        //{
        //    SqlParameter[] param = new SqlParameter[8];
        //    param[0] = new SqlParameter();
        //    param[0].SqlDbType = SqlDbType.VarChar;
        //    param[0].ParameterName = "@TableName";
        //    param[0].Direction = ParameterDirection.Input;
        //    param[0].Value = "V_Distinct_WayBill";

        //    param[1] = new SqlParameter();
        //    param[1].SqlDbType = SqlDbType.VarChar;
        //    param[1].ParameterName = "@FieldKey";
        //    param[1].Direction = ParameterDirection.Input;
        //    param[1].Value = "wbID";

        //    param[2] = new SqlParameter();
        //    param[2].SqlDbType = SqlDbType.VarChar;
        //    param[2].ParameterName = "@FieldShow";
        //    param[2].Direction = ParameterDirection.Input;
        //    param[2].Value = "*";

        //    param[3] = new SqlParameter();
        //    param[3].SqlDbType = SqlDbType.VarChar;
        //    param[3].ParameterName = "@FieldOrder";
        //    param[3].Direction = ParameterDirection.Input;
        //    param[3].Value = sort + " " + order;

        //    param[4] = new SqlParameter();
        //    param[4].SqlDbType = SqlDbType.Int;
        //    param[4].ParameterName = "@PageSize";
        //    param[4].Direction = ParameterDirection.Input;
        //    param[4].Value = 10000000;//Convert.ToInt32(rows);

        //    param[5] = new SqlParameter();
        //    param[5].SqlDbType = SqlDbType.Int;
        //    param[5].ParameterName = "@PageCurrent";
        //    param[5].Direction = ParameterDirection.Input;
        //    param[5].Value = Convert.ToInt32(page);

        //    param[6] = new SqlParameter();
        //    param[6].SqlDbType = SqlDbType.VarChar;
        //    param[6].ParameterName = "@Where";
        //    param[6].Direction = ParameterDirection.Input;
        //    string strWhereTemp = "";
        //    param[6].Value = strWhereTemp;

        //    param[7] = new SqlParameter();
        //    param[7].SqlDbType = SqlDbType.Int;
        //    param[7].ParameterName = "@RecordCount";
        //    param[7].Direction = ParameterDirection.Output;

        //    DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
        //    DataTable dt = ds.Tables["result"];

        //    StringBuilder sb = new StringBuilder("");
        //    sb.Append("{");
        //    sb.AppendFormat("\"total\":{0}", Convert.ToInt32(param[7].Value.ToString()));
        //    sb.Append(",\"rows\":[");
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        sb.Append("{");

        //        string[] strFiledArray = strFileds_AllWayBill.Split(',');
        //        for (int j = 0; j < strFiledArray.Length; j++)
        //        {
        //            switch (strFiledArray[j])
        //            {
        //                default:
        //                    if (j != strFiledArray.Length - 1)
        //                    {
        //                        sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
        //                    }
        //                    else
        //                    {
        //                        sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", "")));
        //                    }
        //                    break;
        //            }



        //        }

        //        if (i == dt.Rows.Count - 1)
        //        {
        //            sb.Append("}");
        //        }
        //        else
        //        {
        //            sb.Append("},");
        //        }
        //    }
        //    dt = null;
        //    if (sb.ToString().EndsWith(","))
        //    {
        //        sb = new StringBuilder(sb.ToString().Substring(0, sb.ToString().Length - 1));
        //    }
        //    sb.Append("]");
        //    sb.Append("}");
        //    return sb.ToString();
        //}

        /// <summary>
        /// 分页查询类
        /// </summary>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public string GetAllWayBillData(string order, string page, string rows, string sort)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_Distinct_WayBill";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "wbID";

            param[2] = new SqlParameter();
            param[2].SqlDbType = SqlDbType.VarChar;
            param[2].ParameterName = "@FieldShow";
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = "*";

            param[3] = new SqlParameter();
            param[3].SqlDbType = SqlDbType.VarChar;
            param[3].ParameterName = "@FieldOrder";
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = " wbSerialNum desc ";

            param[4] = new SqlParameter();
            param[4].SqlDbType = SqlDbType.Int;
            param[4].ParameterName = "@PageSize";
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = 10000000;//Convert.ToInt32(rows);

            param[5] = new SqlParameter();
            param[5].SqlDbType = SqlDbType.Int;
            param[5].ParameterName = "@PageCurrent";
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = Convert.ToInt32(1);

            param[6] = new SqlParameter();
            param[6].SqlDbType = SqlDbType.VarChar;
            param[6].ParameterName = "@Where";
            param[6].Direction = ParameterDirection.Input;
            string strWhereTemp = "";
            param[6].Value = strWhereTemp;

            param[7] = new SqlParameter();
            param[7].SqlDbType = SqlDbType.Int;
            param[7].ParameterName = "@RecordCount";
            param[7].Direction = ParameterDirection.Output;

            DataSet ds = SqlServerHelper.RunProcedure("spPageViewByStr", param, "result");
            DataTable dt = ds.Tables["result"];

            StringBuilder sb = new StringBuilder("");
            sb.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("{");
                sb.AppendFormat("\"id\":\"{0}\",\"text\":\"{1}\"", dt.Rows[i]["wbSerialNum"].ToString(), dt.Rows[i]["wbSerialNum"].ToString());

                if (i == dt.Rows.Count - 1)
                {
                    sb.Append("}");
                }
                else
                {
                    sb.Append("},");
                }
            }
            dt = null;
            
            sb.Append("]");
            return sb.ToString();
        }

        [HttpPost]
        public string ValidateInfo(string wbSerialNum, string swbSerialNum)
        {
            DataSet ds = null;
            DataTable dt = null;
            int iInOutStoreType = -1;
            string strRet = "{\"result\":\"error\",\"message\":\"原因未知\"}";
            wbSerialNum = Server.UrlDecode(wbSerialNum);
            swbSerialNum = Server.UrlDecode(swbSerialNum);
            try
            {
                if ((new T_WayBill()).ExistWbSerialNum(wbSerialNum) == false)
                {
                    if (!(new T_WayBill()).IsInReStore(wbSerialNum, swbSerialNum))
                    {
                        ds = (new T_WayBillFlow()).getWayBillFlow(wbSerialNum, swbSerialNum);
                        if (ds != null)
                        {
                            dt = ds.Tables[0];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                iInOutStoreType = Convert.ToInt32(dt.Rows[0]["InOutStoreType"].ToString());
                                switch (iInOutStoreType)
                                {
                                    case 1:
                                        strRet = "{\"result\":\"error\",\"message\":\"" + "此分单已经入库" + "\"}";
                                        break;
                                    case 3:
                                        strRet = "{\"result\":\"error\",\"message\":\"" + "此分单已经出库" + "\"}";
                                        break;
                                    default:
                                        strRet = "{\"result\":\"error\",\"message\":\"" + "" + "\"}";
                                        break;
                                }
                            }
                            else
                            {
                                strRet = "{\"result\":\"ok\",\"message\":\"" + "" + "\"}";
                            }
                        }
                        else
                        {
                            strRet = "{\"result\":\"ok\",\"message\":\"" + "" + "\"}";
                        }
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"" + "所填写的【总运单号、分运单号】已经在预入库表中，请重新填写" + "\"}";
                    }
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "所填写的【总运单号】在系统中不存在，请重新填写" + "\"}";
                }

            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }

        [HttpPost]
        public string DeleForeStore(string swbIds)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"原因未知\"}";
            try
            {
                if (swbIds != "")
                {
                    if ((new T_SubWayBill()).DeleteSubWayBillByID(swbIds))
                    {
                        strRet = "{\"result\":\"ok\",\"message\":\"" + "成功删除了所选择的的预入库信息" + "\"}";
                    }
                    else
                    {
                        strRet = "{\"result\":\"error\",\"message\":\"" + "删除过程中出现错误" + "\"}";
                    }
                    //string[] arrSwbIds = swbIds.Split(',');
                    //for (int i = 0; i < arrSwbIds.Length; i++)
                    //{
                    //    if (arrSwbIds[i].Trim() != "")
                    //    {
                    //        (new T_SubWayBill()).DeleteSubWayBillByID(swbIds);
                    //    }
                    //}
                }
                else
                {
                    strRet = "{\"result\":\"error\",\"message\":\"" + "未选择需要删除的预入库信息" + "\"}";
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
