var ViewInOutStoreURL = "/ViewInOutStoreWayBillDetail/Index?Detail_EnableSelectInOutStoreType=0&Detail_wbSerialNum=";
function ViewInOutStore(wbSerialNum, InOutType) {
    var div_DetailDlg = self.parent.$("#dlg_GlobalDetail");
    DetailDlg = div_DetailDlg.dialog({
        buttons: [{
            text: '关 闭',
            iconCls: 'icon-cancel',
            handler: function () {
                DetailDlg.dialog('close');
            }
        }],
        title: '确  认  入  库',
        href: ViewInOutStoreURL + wbSerialNum + "&Detail_InOutStoreType=" + InOutType,
        modal: true,
        resizable: true,
        cache: false,
        left: 0,
        top: 0,
        width: 1020,
        height: 480,
        closed: true
    });
    div_DetailDlg.dialog("open");
}

var ViewForeStoreURL = "/ViewForeStoreWayBillDetail/Index?Detail_wbSerialNum=";
function ViewForeStore(wbSerialNum) {
    var div_DetailDlg = self.parent.$("#dlg_GlobalDetail");
    DetailDlg = div_DetailDlg.dialog({
        buttons: [{
            text: '关 闭',
            iconCls: 'icon-cancel',
            handler: function () {
                DetailDlg.dialog('close');
            }
        }],
        title: '确  认  入  库',
        href: ViewForeStoreURL + wbSerialNum,
        modal: true,
        resizable: true,
        cache: false,
        left: 0,
        top: 0,
        width: 1020,
        height: 480,
        closed: true
    });
    div_DetailDlg.dialog("open");
}

var AllSaveStoreURL = "/Huayu_SaveInStore/AllSaveStore?wbID=";
function AllSaveStore(wbID) {
    $.messager.progress();
    $.ajax({
        type: "POST",
        url: AllSaveStoreURL + encodeURI(wbID),
        data: "",
        async: true,
        cache: false,
        beforeSend: function (XMLHttpRequest) {

        },
        success: function (msg) {
            $.messager.progress('close');
            var JSONMsg = eval("(" + msg + ")");
            if (JSONMsg.result.toLowerCase() == 'ok') {
                reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
            } else {
                reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
            }
        },
        complete: function (XMLHttpRequest, textStatus) {
            $.messager.progress('close');
        },
        error: function () {
            $.messager.progress('close');
        }
    });
}

$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddCompany = $('#ddCompany');
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    var DetailURL = "/ViewSubWayBillDetail/Index?Detail_wbSerialNum=";
    var CheckURL = "/Huayu_SaveInStore/upDateSwbNeedCheck";

    var GetForeStoreCountURL = "/Huayu_SaveInStore/getForeStoreCount?wbID=";
    var GetInOutStoreCountURL = "/Huayu_SaveInStore/getInOutStoreCount?wbID=";

    var getSubWayBillSumInfoURL = "/ViewSubWayBillDetail/getSubWayBillSumInfo?wbID=";

    var DetailDlg = null;
    var DetailDlgForm = null;

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL ="";// "/Huayu_SaveInStore/GetData?txtBeginDate=" + encodeURI($("#txtBeginDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val()) + "&ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val());

    $("#btnQuery").click(function () {
        QueryURL = "/Huayu_SaveInStore/GetData?txtBeginDate=" + encodeURI($("#txtBeginDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val()) + "&ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnPrint").click(function () {
        PrintURL = "/Huayu_SaveInStore/Print?txtBeginDate=" + encodeURI($("#txtBeginDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val()) + "&ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            var div_PrintDlg = self.parent.$("#dlg_GlobalPrint");
            div_PrintDlg.show();
            var PrintDlg = null;
            div_PrintDlg.find("#frmPrintURL").attr("src", PrintURL);
            PrintDlg = div_PrintDlg.window({
                title: '打印',
                href: "",
                modal: true,
                resizable: true,
                minimizable: false,
                collapsible: false,
                cache: false,
                closed: true,
                width: 900,
                height: 500
            });
            div_PrintDlg.window("open");

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可打印", "error");
            return false;
        }

    });

    $("#btnExcel").click(function () {
        var browserType = "";
        if ($.browser.msie) {
            browserType = "msie";
        }
        else if ($.browser.safari) {
            browserType = "safari";
        }
        else if ($.browser.mozilla) {
            browserType = "mozilla";
        }
        else if ($.browser.opera) {
            browserType = "opera";
        }
        else {
            browserType = "unknown";
        }

        PrintURL = "/Huayu_SaveInStore/Excel?txtBeginDate=" + encodeURI($("#txtBeginDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val()) + "&ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    });

    _$_datagrid.datagrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        singleSelect: true,
        url: QueryURL,
        sortName: 'wbID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'wbID',
        columns: [[
					{ field: 'wbStorageDate', title: '导入日期', width: 60, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'wbCompany', title: '货代公司', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'wbSerialNum', title: '总运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalNumber_Customize', title: '子运单总件数', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetSubWayBillSumInfo' type='swbTotalNumber' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'wbTotalWeight_Customize', title: '子运单总重量(公斤)', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetSubWayBillSumInfo' type='swbTotalWeight'  wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'ForeStore_Count', title: '预入库分运单数(<font style="color:red;font-weight:bold">可入库</font>)', width: 150, sortable: true,
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='clsForeStoreCountSpan' wbID='" + rec.wbID + "' wbSerialNum='" + rec.wbSerialNum + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'InStore_Count', title: '已入库分运单数(<font style="color:red;font-weight:bold">可出库</font>)', width: 150, sortable: true,
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='clsInOutStoreCountSpan' wbID='" + rec.wbID + "' InOutStoreType='1'></span>";
                            return strRet;
                        }
                    },
                    { field: 'OutStore_Count', title: '已出库分运单数', width: 120, sortable: true,
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='clsInOutStoreCountSpan'  wbID='" + rec.wbID + "' InOutStoreType='3'></span>";
                            return strRet;
                        }
                    },
                    { field: 'hiddenFields', title: '', hidden: true, width: 0, sortable: true,
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='clsInOutStoreCountSpan_Hid'  wbSerialNum='" + rec.wbSerialNum + "' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'wbImportType', title: '数据来源', width: 80, sortable: true,
                        formatter: function (value, rec, index) {
                            var s = "";
                            if (rec.wbImportType == 0) {
                                s = "内部数据";
                            } else {
                                s = "海关同步";
                            }
                            return s;
                        }
                    }
				]],
        pagination: true,
        pageSize: 20,
        pageList: [20, 25, 30, 35, 40, 45, 50],
        toolbar: "#toolBar",
        onRowContextMenu: function (e, rowIndex, rowData) {
            e.preventDefault();
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);

            var cmenu = $('<div id="cmenu" style="width:100px;"></div>').appendTo('body');
            $('<div  id="mnuDetail" iconCls="icon-search"/>').html("查看子运单明细").appendTo(cmenu);
            cmenu.menu({
                onClick: function (item) {
                    cmenu.remove();
                    switch (item.id.toLowerCase()) {
                        case "mnudetail":
                            Detail();
                            break;
                    }
                }
            });

            $('#cmenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        },
        onHeaderContextMenu: function (e, field) {
            e.preventDefault();
            if (!$('#tmenu').length) {
                createColumnMenu();
            }
            $('#tmenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        },
        onSortColumn: function (sort, order) {
            //_$_datagrid.datagrid("reload");
        },
        onClickRow: function (rowIndex, rowData) {
            Detail(rowData.wbSerialNum);
        },
        onLoadSuccess: function (data) {
            var allInOutStoreCountSpan_Hid = $(".clsInOutStoreCountSpan_Hid");
            var allForeStoreCountSpan = $(".clsForeStoreCountSpan");
            
            $.each(allInOutStoreCountSpan_Hid, function (i, item) {
                var strRet = "";
                var wbID = $(item).attr("wbID");
                var wbSerialNum = $(item).attr("wbSerialNum");
                $.ajax({
                    type: "POST",
                    url: GetInOutStoreCountURL + encodeURI(wbID),
                    data: "",
                    async: true,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var msg = eval("(" + msg + ")");
                        if (msg.result == 'ok') {
                            strRet = msg.message;

                        } else {
                            strRet = "获取出错";
                        }
                        $(".clsInOutStoreCountSpan[wbID=" + wbID + "][InOutStoreType=1]").html(msg.InStoreCount.toString());
                        //                        if (msg.InStoreCount != "0") {
                        //                            $(".clsInOutStoreCountSpan[wbID=" + wbID + "][InOutStoreType=1]").html(msg.InStoreCount.toString() + "<a href='#' onclick=\"ViewInOutStore('" + encodeURI(wbSerialNum) + "',1);\">点击</a>");
                        //                        } else {
                        //                            $(".clsInOutStoreCountSpan[wbID=" + wbID + "][InOutStoreType=1]").html(msg.InStoreCount.toString());
                        //                        }
                        //                        if (msg.OutStoreCount != "0") {
                        //                            $(".clsInOutStoreCountSpan[wbID=" + wbID + "][InOutStoreType=3]").html(msg.OutStoreCount.toString() + "<a href='#' onclick=\"ViewInOutStore('" + encodeURI(wbSerialNum) + "',3);\">点击</a>");
                        //                        } else {
                        //                            $(".clsInOutStoreCountSpan[wbID=" + wbID + "][InOutStoreType=3]").html(msg.OutStoreCount.toString());
                        //                        }
                        $(".clsInOutStoreCountSpan[wbID=" + wbID + "][InOutStoreType=3]").html(msg.OutStoreCount.toString());
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });

                $.ajax({
                    type: "POST",
                    url: GetForeStoreCountURL + encodeURI(wbID),
                    data: "",
                    async: true,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var msg = eval("(" + msg + ")");
                        if (msg.result == 'ok') {
                            strRet = msg.message;
                        } else {
                            strRet = "获取出错";
                        }
                        if (strRet != "0") {
                            $(".clsForeStoreCountSpan[wbID=" + wbID + "]").html(strRet.toString() + "<a href='#' onclick=\"AllSaveStore('" + encodeURI(wbID) + "');\">全部入库</a>&nbsp;&nbsp;&nbsp;<a href='#' onclick=\"ViewForeStore('" + encodeURI(wbSerialNum) + "');\">指定入库</a>");
                        } else {
                            $(".clsForeStoreCountSpan[wbID=" + wbID + "]").html(strRet.toString());
                        }
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });

                $.ajax({
                    type: "POST",
                    url: getSubWayBillSumInfoURL + encodeURI(wbID),
                    data: "",
                    async: true,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var msg = eval("(" + msg + ")");
                        if (msg.result == 'ok') {
                            strRet = msg.message;

                        } else {
                            strRet = "获取出错";
                        }
                        $(".cls_SynchorizeGetSubWayBillSumInfo[wbID=" + wbID + "][type='swbTotalNumber']").html(msg.swbTotalNumber.toString());
                        $(".cls_SynchorizeGetSubWayBillSumInfo[wbID=" + wbID + "][type='swbTotalWeight']").html(msg.swbTotalWeight.toString());
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });

            });
        }
    });

    function Detail(wbSerialNum) {
        var div_DetailDlg = self.parent.$("#dlg_GlobalDetail");
        if (wbSerialNum) {
            DetailDlg = div_DetailDlg.dialog({
                buttons: [{
                    text: '关 闭',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        DetailDlg.dialog('close');
                    }
                }],
                title: '查看子运单明细',
                href: DetailURL + wbSerialNum,
                modal: true,
                resizable: true,
                cache: false,
                left: 0,
                top: 0,
                width: 1020,
                height: 480,
                closed: true
            });
            _$_datagrid.datagrid("unselectAll");
        } else {
            var selects = _$_datagrid.datagrid("getSelections");
            if (selects.length != 1) {
                reWriteMessagerAlert("提示", "<center>请选择数据进行查看(<font style='color:red'>只可查看一行</font>)</center>", "error");
                return false;
            } else {
                var wbSerialNum = selects[0].wbSerialNum;
                DetailDlg = div_DetailDlg.dialog({
                    buttons: [{
                        text: '关 闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            DetailDlg.dialog('close');
                        }
                    }],
                    title: '查看子运单明细',
                    href: DetailURL + wbSerialNum,
                    modal: true,
                    resizable: true,
                    cache: false,
                    left: 0,
                    top: 0,
                    width: 1020,
                    height: 480,
                    closed: true
                });
                _$_datagrid.datagrid("unselectAll");
            }
        }

        div_DetailDlg.dialog("open");
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:150px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "wbserialNum":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "wbSerialNum") {

                } else {
                    if (item.iconCls == 'icon-ok') {
                        _$_datagrid.datagrid('hideColumn', $(item.text).attr("id"));
                        tmenu.menu('setIcon', {
                            target: item.target,
                            iconCls: 'icon-empty'
                        });
                    } else {
                        _$_datagrid.datagrid('showColumn', $(item.text).attr("id"));
                        tmenu.menu('setIcon', {
                            target: item.target,
                            iconCls: 'icon-ok'
                        });
                    }
                }
            }
        });
    }
});
