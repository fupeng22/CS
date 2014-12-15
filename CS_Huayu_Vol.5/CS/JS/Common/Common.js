//找出所有行中一样的列数据, 返回最后一列
//支持各个字段排序后合并, 如果合并字段不想变动, 可在sql排序时, 把该字段加到第一个排序
var get_row_to_col = function (rows, sn, col, c) {
    var t = 0;
    if (rows != undefined) {
        var is_break = true;
        for (var i = 0; i < rows.length; i++) {
            var d = c + i;
            if (rows[d] == undefined) {
                return t;
            }
            var _value = eval("rows[" + d + "]." + col + "");
            if (_value == sn) {
                t++;
            } else {
                is_break = false;
            }
            if (is_break == false) {
                return t;
            }
        }
    }
    return t;
}

/**
* datagrid 中合并行合并
* @create chh 
* @time   2013/5/6 9:32:18
* @params col   string  合并字段名
* @params dom   string  datagrid ID名
* @params data  object  datagrid当前页所有数据
* return
**/
var row_merge = function (col, dom, data) {
    for (var k in col) {
        var i = 0;
        while (i < data.length) {
            if (typeof (data[i]) == "object") {
                var p = k > 0 ? k - 1 : k;
                var g_val = eval("data[i]." + col[p] + ";");
                var last_i = get_row_to_col(data, g_val, col[p], i);
                if (last_i > 1) {
                    $('#' + dom).datagrid('mergeCells', {
                        index: i,
                        field: col[k],
                        rowspan: last_i
                    });
                    i += last_i;
                } else {
                    i++;
                }
            }
        }
    }
    return;
}

var reWriteMessagerAlert = function (title, content, icon) {
    var dlgIcon = "";
    switch (icon) {
        case "error":
            dlgIcon = "icon-no";
            break;
        case "info":
            dlgIcon = "icon-ok";
            break;
        default:

    }

    //    var div_MsgAlertDlg = self.parent.$("#dlg_MsgAlert");
    //    var msgAlertDlg = null;
    //    div_MsgAlertDlg.find("#msgContent").html(content);
    //    msgAlertDlg = div_MsgAlertDlg.window({
    //        title: title,
    //        href: "",
    //        modal: true,
    //        resizable: true,
    //        minimizable: false,
    //        maximizable: false,
    //        collapsible: false,
    //        cache: false,
    //        closed: true,
    //        width: 300,
    //        height: 200,
    //        iconCls: dlgIcon
    //    });
    //    msgAlertDlg.window("open");

    var div_MsgAlertDlg = self.parent.$("#dlg_MsgAlert");
    div_MsgAlertDlg.show();
    var msgAlertDlg = null;
    div_MsgAlertDlg.find("#msgContent").html(content);
    msgAlertDlg = div_MsgAlertDlg.dialog({
        buttons: [{
            text: '关 闭',
            iconCls: 'icon-cancel',
            handler: function () {
                div_MsgAlertDlg.dialog('close');
                if ($("#txtSubWayBillCode")) {
                    $("#txtSubWayBillCode").focus();
                }
            }
        }],
        title: title,
        href: "",
        modal: true,
        resizable: true,
        cache: false,
        closed: false,
        width: 300,
        height: 130,
        closed: true,
        iconCls: dlgIcon
    });

    div_MsgAlertDlg.dialog("open");

    div_MsgAlertDlg.find("#txtGlobalTextInput").focus();

    // ESC\Enter方式关闭dialog
    msgAlertDlg.bind('keydown', function (event) {
        //if (event.keyCode == '27' || event.keyCode == '13') {
        if (event.keyCode == '27') {
            msgAlertDlg.dialog('close');
            if ($("#txtSubWayBillCode")) {
                $("#txtSubWayBillCode").focus();
            }
        }
    });
}

var reWriteMessagerConfirm = function (title, content, fun) {
    var dlgIcon = "icon-infomation";

    var div_MsgAlertDlg = self.parent.$("#dlg_MsgAlert");
    div_MsgAlertDlg.show();
    var msgAlertDlg = null;
    div_MsgAlertDlg.find("#msgContent").html(content);
    msgAlertDlg = div_MsgAlertDlg.dialog({
        buttons: [{
            text: '确  定',
            iconCls: 'icon-ok',
            handler: function () {
                fun(true);
                div_MsgAlertDlg.dialog('close');
            }
        }, {
            text: '取  消',
            iconCls: 'icon-cancel',
            handler: function () {
                div_MsgAlertDlg.dialog('close');
            }
        }],
        title: title,
        href: "",
        modal: true,
        resizable: true,
        cache: false,
        closed: false,
        width: 300,
        height: 130,
        closed: true,
        iconCls: dlgIcon
    });

    div_MsgAlertDlg.dialog("open");
}

function dateDiff(d1, d2, type) {
    var obj = {};
    obj.Y = d2.getFullYear() - d1.getFullYear() + ((d2.getMonth() + d2.getDate() * 0.01) > (d1.getMonth() + d1.getDate() * 0.01) ? 0 : -1); //直接年份相减，如果月日不大于开始日期应该减少一年  
    obj.M = (d2.getFullYear() - d1.getFullYear()) * 12 + d2.getMonth() - d1.getMonth() + (d2.getDate() > d1.getDate() ? 0 : -1); //年差*12+月份差 如果号数不大于开始日期应该减少一个月  
    obj.Q = Math.floor(obj.M / 3); //三个月为一个季度  
    obj.D = Math.floor((d2 - d1) / (1000 * 60 * 60 * 24)); //差几天  
    obj.h = Math.floor((d2 - d1) / (1000 * 60 * 60)); //差几小时  
    obj.m = Math.floor((d2 - d1) / (1000 * 60)); //差几分钟  
    obj.s = Math.floor((d2 - d1) / 1000); //差几秒  
    if (obj[type]) {
        return obj[type];
    } else {
        return 0;
    }
}

function dateCon(d, num) {
    var d = new Date(d.substring(0, 4),
    d.substring(5, 7) - 1,
    d.substring(8, 10),
    d.substring(11, 13),
    d.substring(14, 16),
    d.substring(17, 19));
    d.setTime(d.getTime() + num * 1000);
    //alert(d.toLocaleString());
    return d.getFullYear() + "-"
    + (d.getMonth() + 1)
    + "-" + d.getDate()
    + " " + d.getHours()
    + ":" + d.getMinutes()
    + ":" + d.getSeconds();
}

function DateFormat(date, format) {
    if (!date) return;
    if (!format) format = "yyyy-MM-dd";
    switch (typeof date) {
        case "string":
            date = new Date(date.replace(/-/, "/"));
            break;
        case "number":
            date = new Date(date);
            break;
    }
    if (!date instanceof Date) return;
    var dict = {
        "yyyy": date.getFullYear(),
        "M": date.getMonth() + 1,
        "d": date.getDate(),
        "H": date.getHours(),
        "m": date.getMinutes(),
        "s": date.getSeconds(),
        "MM": ("" + (date.getMonth() + 101)).substr(1),
        "dd": ("" + (date.getDate() + 100)).substr(1),
        "HH": ("" + (date.getHours() + 100)).substr(1),
        "mm": ("" + (date.getMinutes() + 100)).substr(1),
        "ss": ("" + (date.getSeconds() + 100)).substr(1)
    };
    return format.replace(/(yyyy|MM?|dd?|HH?|ss?|mm?)/g, function () {
        return dict[arguments[0]];
    });
}

//cookie
function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}
function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    createCookie(name, "", -1);
}

function switchStylestyle(styleName) {
    $('link[rel=stylesheet][title]').each(function (i) {
        this.disabled = true;
        //console.info(this.getAttribute('title')+"_____"+styleName);
        if (this.getAttribute('title') == styleName) this.disabled = false;
    });

    $("iframe").contents().find('link[rel=stylesheet][title]').each(function (i) {
        this.disabled = true;
        if (this.getAttribute('title') == styleName) this.disabled = false;
    });

    createCookie('style', styleName, 365);
}

$(function () {
    $('.styleswitch').click(function () {
        switchStylestyle(this.getAttribute("rel"));
        return false;
    });
    var c = readCookie('style');
    if (c) {
        switchStylestyle(c);
    } else {
        switchStylestyle("default");
    }
});