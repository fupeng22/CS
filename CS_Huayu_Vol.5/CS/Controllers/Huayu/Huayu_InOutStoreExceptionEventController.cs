using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Filter;
using SQLDAL;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using DBUtility;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_InOutStoreExceptionEventController : Controller
    {
        SQLDAL.T_WayBillLog tWayBillLog = new T_WayBillLog();
        public const string strFileds = "operateDate,operator,Wbl_wbSerialNum,Wbl_swbSerialNum,status,WblID";

        public const string STR_TEMPLATE_EXCEL = "~/Temp/Template/template.xls";
        public const string STR_REPORT_URL = "~/Content/Reports/HuayuInOutStoreExceptionEvent.rdlc";
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
        public string GetData(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtInOutStoreExceptionEventType, string txtWayBillCode, string txtSubWayBillCode, string txtOperator)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_InOutStoreExceptionEvent";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "WblID";

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
            txtInOutStoreExceptionEventType = Server.UrlDecode(txtInOutStoreExceptionEventType.ToString());
            txtWayBillCode = Server.UrlDecode(txtWayBillCode.ToString());
            txtSubWayBillCode = Server.UrlDecode(txtSubWayBillCode.ToString());
            txtOperator = Server.UrlDecode(txtOperator.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (CONVERT(nvarchar(10),operateDate,120)>='{0}' and CONVERT(nvarchar(10),operateDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (CONVERT(nvarchar(10),operateDate,120)>='{0}' and CONVERT(nvarchar(10),operateDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtInOutStoreExceptionEventType != "" && txtInOutStoreExceptionEventType != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (status in ({0})) ", txtInOutStoreExceptionEventType);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (status in ({0})) ", txtInOutStoreExceptionEventType);
                }
            }

            if (txtWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (Wbl_wbSerialNum like '%{0}%') ", txtWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (Wbl_wbSerialNum like '%{0}%') ", txtWayBillCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (Wbl_swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (Wbl_swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
            }

            if (txtOperator != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (company like '%{0}%') ", txtOperator);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (company like '%{0}%') ", txtOperator);
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
                        case "operator"://格式化公司(保存的是用户名，取出公司名)
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : ((new T_User()).GetUserByUserName(dt.Rows[i][strFiledArray[j]].ToString())));
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : ((new T_User()).GetUserByUserName(dt.Rows[i][strFiledArray[j]].ToString())));
                            }
                            break;
                        case "status":
                            string s = "";
                            switch (dt.Rows[i][strFiledArray[j]].ToString())
                            {
                                case "1":
                                    s = "未预入库时入库";
                                    break;
                                case "2":
                                    s = "重复入库";
                                    break;
                                case "3":
                                    s = "已出库时入库";
                                    break;
                                case "4":
                                    s = "入库异常时入库";
                                    break;
                                case "5":
                                    s = "出库异常时入库";
                                    break;
                                case "6":
                                    s = "未预入库时出库";
                                    break;
                                case "7":
                                    s = "未入库时出库";
                                    break;
                                case "8":
                                    s = "重复出库";
                                    break;
                                case "9":
                                    s = "入库异常时出库";
                                    break;
                                case "10":
                                    s = "出库异常时出库";
                                    break;
                                case "11":
                                    s = "数据格式不正确";
                                    break;
                                case "12":
                                    s = "未放行却出库";
                                    break;
                                case "99":
                                    s = "未知";
                                    break;
                            }
                            if (j != strFiledArray.Length - 1)
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\",", strFiledArray[j], s);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\":\"{1}\"", strFiledArray[j], s);
                            }
                            break;
                        case "operateDate":
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
        public ActionResult Print(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtInOutStoreExceptionEventType, string txtWayBillCode, string txtSubWayBillCode, string txtOperator)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_InOutStoreExceptionEvent";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "WblID";

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
            txtInOutStoreExceptionEventType = Server.UrlDecode(txtInOutStoreExceptionEventType.ToString());
            txtWayBillCode = Server.UrlDecode(txtWayBillCode.ToString());
            txtSubWayBillCode = Server.UrlDecode(txtSubWayBillCode.ToString());
            txtOperator = Server.UrlDecode(txtOperator.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (CONVERT(nvarchar(10),operateDate,120)>='{0}' and CONVERT(nvarchar(10),operateDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (CONVERT(nvarchar(10),operateDate,120)>='{0}' and CONVERT(nvarchar(10),operateDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtInOutStoreExceptionEventType != "" && txtInOutStoreExceptionEventType != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (status in ({0})) ", txtInOutStoreExceptionEventType);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (status in ({0})) ", txtInOutStoreExceptionEventType);
                }
            }

            if (txtWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (Wbl_wbSerialNum like '%{0}%') ", txtWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (Wbl_wbSerialNum like '%{0}%') ", txtWayBillCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (Wbl_swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (Wbl_swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
            }

            if (txtOperator != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (company like '%{0}%') ", txtOperator);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (company like '%{0}%') ", txtOperator);
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
            dtCustom.Columns.Add("operateDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("operator", Type.GetType("System.String"));
            dtCustom.Columns.Add("Wbl_wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("Wbl_swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("status", Type.GetType("System.String"));
            dtCustom.Columns.Add("WblID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wbCompany"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : ((new T_User()).GetUserByUserName(dt.Rows[i][strFiledArray[j]].ToString()));
                            break;
                        case "status":
                            string s = "";
                            switch (dt.Rows[i][strFiledArray[j]].ToString())
                            {
                                case "1":
                                    s = "未预入库时入库";
                                    break;
                                case "2":
                                    s = "重复入库";
                                    break;
                                case "3":
                                    s = "已出库时入库";
                                    break;
                                case "4":
                                    s = "入库异常时入库";
                                    break;
                                case "5":
                                    s = "出库异常时入库";
                                    break;
                                case "6":
                                    s = "未预入库时出库";
                                    break;
                                case "7":
                                    s = "未入库时出库";
                                    break;
                                case "8":
                                    s = "重复出库";
                                    break;
                                case "9":
                                    s = "入库异常时出库";
                                    break;
                                case "10":
                                    s = "出库异常时出库";
                                    break;
                                case "11":
                                    s = "数据格式不正确";
                                    break;
                                case "12":
                                    s = "未放行却出库";
                                    break;
                                case "99":
                                    s = "未知";
                                    break;
                            }
                            drCustom[strFiledArray[j]] = s;
                            break;
                        case "operateDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }

                }
                if (drCustom["WblID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("HuayuInOutStoreExceptionEvent_DS", dtCustom);

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
        public ActionResult Excel(string order, string page, string rows, string sort, string txtBeginD, string txtEndD, string txtInOutStoreExceptionEventType, string txtWayBillCode, string txtSubWayBillCode, string txtOperator,string browserType)
        {
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter();
            param[0].SqlDbType = SqlDbType.VarChar;
            param[0].ParameterName = "@TableName";
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = "V_InOutStoreExceptionEvent";

            param[1] = new SqlParameter();
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].ParameterName = "@FieldKey";
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = "WblID";

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
            txtInOutStoreExceptionEventType = Server.UrlDecode(txtInOutStoreExceptionEventType.ToString());
            txtWayBillCode = Server.UrlDecode(txtWayBillCode.ToString());
            txtSubWayBillCode = Server.UrlDecode(txtSubWayBillCode.ToString());
            txtOperator = Server.UrlDecode(txtOperator.ToString());

            string strWhereTemp = "";
            if (txtBeginD != "" && txtEndD != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (CONVERT(nvarchar(10),operateDate,120)>='{0}' and CONVERT(nvarchar(10),operateDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (CONVERT(nvarchar(10),operateDate,120)>='{0}' and CONVERT(nvarchar(10),operateDate,120)<='{1}') ", Convert.ToDateTime(txtBeginD).ToString("yyyy-MM-dd"), Convert.ToDateTime(txtEndD).ToString("yyyy-MM-dd"));
                }
            }

            if (txtInOutStoreExceptionEventType != "" && txtInOutStoreExceptionEventType != "-99")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (status in ({0})) ", txtInOutStoreExceptionEventType);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format(" (status in ({0})) ", txtInOutStoreExceptionEventType);
                }
            }

            if (txtWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (Wbl_wbSerialNum like '%{0}%') ", txtWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (Wbl_wbSerialNum like '%{0}%') ", txtWayBillCode);
                }
            }

            if (txtSubWayBillCode != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (Wbl_swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (Wbl_swbSerialNum like '%{0}%') ", txtSubWayBillCode);
                }
            }

            if (txtOperator != "")
            {
                if (strWhereTemp != "")
                {
                    strWhereTemp = strWhereTemp + string.Format(" and (company like '%{0}%') ", txtOperator);
                }
                else
                {
                    strWhereTemp = strWhereTemp + string.Format("  (company like '%{0}%') ", txtOperator);
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
            dtCustom.Columns.Add("operateDate", Type.GetType("System.String"));
            dtCustom.Columns.Add("operator", Type.GetType("System.String"));
            dtCustom.Columns.Add("Wbl_wbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("Wbl_swbSerialNum", Type.GetType("System.String"));
            dtCustom.Columns.Add("status", Type.GetType("System.String"));
            dtCustom.Columns.Add("WblID", Type.GetType("System.String"));

            DataRow drCustom = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                drCustom = dtCustom.NewRow();
                string[] strFiledArray = strFileds.Split(',');
                for (int j = 0; j < strFiledArray.Length; j++)
                {
                    switch (strFiledArray[j])
                    {
                        case "wbCompany"://格式化公司(保存的是用户名，取出公司名)
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : ((new T_User()).GetUserByUserName(dt.Rows[i][strFiledArray[j]].ToString()));
                            break;
                        case "status":
                            string s = "";
                            switch (dt.Rows[i][strFiledArray[j]].ToString())
                            {
                                case "1":
                                    s = "未预入库时入库";
                                    break;
                                case "2":
                                    s = "重复入库";
                                    break;
                                case "3":
                                    s = "已出库时入库";
                                    break;
                                case "4":
                                    s = "入库异常时入库";
                                    break;
                                case "5":
                                    s = "出库异常时入库";
                                    break;
                                case "6":
                                    s = "未预入库时出库";
                                    break;
                                case "7":
                                    s = "未入库时出库";
                                    break;
                                case "8":
                                    s = "重复出库";
                                    break;
                                case "9":
                                    s = "入库异常时出库";
                                    break;
                                case "10":
                                    s = "出库异常时出库";
                                    break;
                                case "11":
                                    s = "数据格式不正确";
                                    break;
                                case "12":
                                    s = "未放行却出库";
                                    break;
                                case "99":
                                    s = "未知";
                                    break;
                            }
                            drCustom[strFiledArray[j]] = s;
                            break;
                        case "operateDate":
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (Convert.ToDateTime(dt.Rows[i][strFiledArray[j]]).ToString("yyyy-MM-dd").Replace("\r\n", ""));
                            break;
                        default:
                            drCustom[strFiledArray[j]] = dt.Rows[i][strFiledArray[j]] == DBNull.Value ? "" : (dt.Rows[i][strFiledArray[j]].ToString().Replace("\r\n", ""));
                            break;
                    }

                }
                if (drCustom["WblID"].ToString() != "")
                {
                    dtCustom.Rows.Add(drCustom);
                }
            }
            dt = null;

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath(STR_REPORT_URL);
            ReportDataSource reportDataSource = new ReportDataSource("HuayuInOutStoreExceptionEvent_DS", dtCustom);

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

            string strOutputFileName = "出入仓异常事件_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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
        public string Delete(string ids)
        {
            string strRet = "{\"result\":\"error\",\"message\":\"删除失败，原因未知\"}";

            try
            {
                tWayBillLog.DeleteLog(ids);
                strRet = "{\"result\":\"ok\",\"message\":\"删除成功\"}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }

    }
}
