using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SQLDAL;
using System.Text;
using System.Data;
using CS.Filter;

namespace CS.Controllers.Huayu
{
    [ErrorAttribute]
    public class Huayu_PrintReleaseController : Controller
    {
        SQLDAL.T_WayBill tWayBill = new T_WayBill();
        SQLDAL.T_SubWayBill tSubWayBill = new T_SubWayBill();
        //
        // GET: /Huayu_PrintRelease/
        [HuayuRequiresLoginAttribute]
        public ActionResult Index()
        {
            string serialNum =Server.UrlDecode( Request.QueryString["wbSerialNum"].ToString());
            int totalNum = int.Parse(Server.UrlDecode(Request.QueryString["TotalNum"].ToString()));
            int subNum = int.Parse(Server.UrlDecode(Request.QueryString["SubNum"].ToString()));
            string value = Server.UrlDecode(Request.QueryString["value1"]);
            string totalWeight = Server.UrlDecode(Request.QueryString["TotalWeight"]);

            int wbID = tWayBill.GetWayBillID(serialNum);
            int pirntTimes = tWayBill.getPrintStatus(wbID);
            int releseCount = tSubWayBill.GetReleseNum(wbID);
            int saveCount = tSubWayBill.GetSaveNum(wbID);

            ViewData["wbID"] = wbID;
            ViewData["rul"] = Request.Url;
            ViewData["lbTimes"] = "第" + pirntTimes.ToString() + "批";
            ViewData["lbReleseCode"] = DateTime.Now.ToString("yyyyMMdd") + serialNum;
            ViewData["lbKACode"] = "TSN";
            ViewData["lbSerialNum"] = serialNum;
            ViewData["lbTotalNum"] = totalNum;
            ViewData["lbSubNum"] = subNum;
            ViewData["lbReleseNum"] = releseCount;
            ViewData["lbSaveNum"] = saveCount;

            DataSet saveDs = tSubWayBill.GetSave(wbID);
            StringBuilder saveStr = new StringBuilder();
            saveStr.Append("扣留货物分运单号： ");
            string strCode = "";
            if (saveDs != null)
            {
                for (int i = 0; i < saveDs.Tables[0].Rows.Count; i++)
                {
                    strCode += "[" + saveDs.Tables[0].Rows[i][1].ToString() + "]  ";
                    if (i % 6 == 0 && i != 0)
                    {
                        strCode += "\r\n";
                    }
                }

            }
            else
            {
                strCode = "无扣留货物";
            }
            saveStr.Append(strCode);
            ViewData["txtSaveInfo"] = saveStr.ToString();

            StringBuilder momeyStr = new StringBuilder();
            momeyStr.Append("收费明细： ");
            momeyStr.Append("运单号：" + serialNum + "，总重量：" + totalWeight.Substring(0,totalWeight.Length-2) + "公斤 ，操作费：" + value + "元。");
            ViewData["txtMoney"] = momeyStr.ToString();

            return View();
        }

        
        [HttpPost]
        public string produceSubWayBillUnReleased(string wbID)
        {
            string strRet = "none";
            StringBuilder sb = new StringBuilder("");

            wbID = Server.UrlDecode(wbID);
            DataSet saveDs = tSubWayBill.GetSave(Convert.ToInt32(wbID));

            if (saveDs != null)
            {
                DataTable dt = saveDs.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    sb.Append(@"<table border='1' style='width: 90%'>
                    <thead style='background-color: #EEEEEE;'>
                        扣留分运单明细
                    </thead>
                    <tr style='background-color: #EEEEEE;'>
                        <th>
                            序号
                        </th>
                        <th>
                            分运单号
                        </th>
                        <th>
                            货物名称
                        </th>
                        <th>
                            数量
                        </th>
                        <th>
                            报关重量
                        </th>
                        <th>
                            实际重量
                        </th>
                        <th>
                            价值
                        </th>
                        <th>
                            收件人
                        </th>
                    </tr>");


                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            sb.AppendFormat(@"<tr  style='background-color: #EEEEEE;'>
                        <td>
                            {0}
                        </td>
                        <td>
                            {1}
                        </td>
                        <td>
                            {2}
                        </td>
                        <td>
                            {3}
                        </td>
                        <td>
                            {4}
                        </td>
                        <td>
                           {5}
                        </td>
                        <td>
                           {6}
                        </td>
                        <td>
                            {7}
                        </td>
                    </tr>", dt.Rows[i]["swbID"].ToString(), dt.Rows[i]["swbSerialNum"].ToString(), dt.Rows[i]["swbDescription_CHN"].ToString(), dt.Rows[i]["swbNumber"].ToString(), dt.Rows[i]["swbWeight"].ToString(), dt.Rows[i]["swbActualWeight"].ToString(), dt.Rows[i]["swbValue"].ToString(), dt.Rows[i]["swbRecipients"].ToString());
                        }
                        else
                        {
                            sb.AppendFormat(@"<tr>
                        <td>
                            {0}
                        </td>
                        <td>
                            {1}
                        </td>
                        <td>
                            {2}
                        </td>
                        <td>
                            {3}
                        </td>
                        <td>
                            {4}
                        </td>
                        <td>
                           {5}
                        </td>
                        <td>
                           {6}
                        </td>
                        <td>
                            {7}
                        </td>
                    </tr>", dt.Rows[i]["swbID"].ToString(), dt.Rows[i]["swbSerialNum"].ToString(), dt.Rows[i]["swbDescription_CHN"].ToString(), dt.Rows[i]["swbNumber"].ToString(), dt.Rows[i]["swbWeight"].ToString(), dt.Rows[i]["swbActualWeight"].ToString(), dt.Rows[i]["swbValue"].ToString(), dt.Rows[i]["swbRecipients"].ToString());
                        }
                    }

                    sb.Append(@"</table>");
                }
            }

            strRet = sb.ToString();
            return strRet;
        }

    }
}
