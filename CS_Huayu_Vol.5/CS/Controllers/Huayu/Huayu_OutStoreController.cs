using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using DBUtility;
using System.Data;
using System.Data.SqlClient;
using CS.Filter;
using SQLDAL;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_OutStoreController : Controller
    {
        public const string strFileds = "wbStorageDate,OutStoreOperator,OutStoreDate,InOutStoreType,swbDescription_CHN,swbDescription_ENG,wbCompany,wbSerialNum,swbSerialNum,swbWeight,swbNumber,wbID,swbID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/HuayuOutStore.rdlc";
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
        public string GetData(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtVoyage, string txtCode, string txtSubWayBillCode)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBillFlow";

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

            string strWhereTemp = " (InOutStoreType=3) ";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (CONVERT(nvarchar(10),OutStoreDate,120)>='{0}' and CONVERT(nvarchar(10),OutStoreDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (CONVERT(nvarchar(10),OutStoreDate,120)>='{0}' and CONVERT(nvarchar(10),OutStoreDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
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
                        case "OutStoreDate":
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", "")));
                            }
                            break;
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
        public ActionResult Print(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtVoyage, string txtCode, string txtSubWayBillCode)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBillFlow";

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

            string strWhereTemp = " (InOutStoreType=3) ";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (CONVERT(nvarchar(10),OutStoreDate,120)>='{0}' and CONVERT(nvarchar(10),OutStoreDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (CONVERT(nvarchar(10),OutStoreDate,120)>='{0}' and CONVERT(nvarchar(10),OutStoreDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
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
            dtCustom.Columns.Add("OutStoreOperator", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStoreDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("InOutStoreType", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
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
                        case "OutStoreDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
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
            ReportDataSource reportDataSource = new ReportDataSource("HuayuOutStore_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtVoyage, string txtCode, string txtSubWayBillCode, string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_WayBillFlow";

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

            string strWhereTemp = " (InOutStoreType=3) ";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (CONVERT(nvarchar(10),OutStoreDate,120)>='{0}' and CONVERT(nvarchar(10),OutStoreDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (CONVERT(nvarchar(10),OutStoreDate,120)>='{0}' and CONVERT(nvarchar(10),OutStoreDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtVoyage != "" && txtVoyage != "---请选择---")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbCompany like '%{0}%') ", txtVoyage);
                }
            }

            if (txtCode != "")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (wbSerialNum like '%{0}%') ", txtCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp.StartsWith(" and "))
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (swbSerialNum like '%{0}%') ", txtSubWayBillCode);
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
            dtCustom.Columns.Add("OutStoreOperator", Type.GetType("System.String"));
            dtCustom.Columns.Add("OutStoreDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("InOutStoreType", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbCompany", Type.GetType("System.String"));
            dtCustom.Columns.Add("wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbNumber", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbWeight", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_CHN", Type.GetType("System.String"));
            dtCustom.Columns.Add("swbDescription_ENG", Type.GetType("System.String"));
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
                        case "OutStoreDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
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
            ReportDataSource reportDataSource = new ReportDataSource("HuayuOutStore_DS", dtCustom);

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

            string strOutputFileName = "已出库记录_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
    }
}
