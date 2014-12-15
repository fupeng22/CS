<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>出错了!!</title>
    <style type="text/css">
        INPUT
        {
            font-size: 12px;
        }
        TD
        {
            font-size: 12px;
        }
        .p2
        {
            font-size: 12px;
        }
        .p6
        {
            font-size: 12px;
            color: #1b6ad8;
        }
        A
        {
            color: #1b6ad8;
            text-decoration: none;
        }
        A:hover
        {
            color: red;
        }
    </style>
</head>
<body>
    <p align="center">
    </p>
    <p align="center">
    </p>
    <table cellspacing="0" cellpadding="0" width="540" align="center" border="0">
        <tbody>
            <tr>
                <td valign="top" height="270">
                    <div align="center">
                        <br>
                        <img height="211" src="images/Error/error.gif" width="329"><br>
                        <br>
                        <%--<table cellspacing="0" cellpadding="0" width="80%" border="0">
                            <tbody>
                                <tr>
                                    <td>
                                        <font class="p2">&nbsp;&nbsp;&nbsp; <font color="#ff0000">
                                            <img height="13" src="images/Error/emessage.gif" width="12" />&nbsp;无法访问本页的原因是：</font></font>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="8">
                                    </td>
                                    <tr>
                                        <td>
                                            <p>
                                                <font color="#000000">
                                                    <br>
                                                    您所请求的页面不存在</font>!
                                            </p>
                                        </td>
                                    </tr>
                            </tbody>
                        </table>--%>
                    </div>
                </td>
            </tr>
            <tr>
                <td height="5">
                </td>
                <tr>
                    <td align="middle">
                        <center>
                            <table cellspacing="0" cellpadding="0" width="480" border="0">
                                <tbody>
                                    <tr>
                                        <td width="6">
                                            <img height="26" src="images/Error/left.gif" width="7">
                                        </td>
                                        <td background="images/Error/bg.gif">
                                            <div align="center">
                                                <font class="p6"><a href="/Login/Index">返回登录页</a> </font>
                                            </div>
                                        </td>
                                        <td width="7">
                                            <img src="images/Error/right.gif" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </center>
                    </td>
                </tr>
        </tbody>
    </table>
    <p align="center">
    </p>
    <p align="center">
    </p>
</body>
</html>
