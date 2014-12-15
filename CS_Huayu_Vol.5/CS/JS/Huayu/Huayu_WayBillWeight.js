$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddCompany = $('#txtVoyage');
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    var getSubWayBillSumInfoURL = "/ViewSubWayBillDetail/getSubWayBillSumInfo?wbID=";

    var DetailURL = "/ViewSubWayBillDetail/Index?Detail_wbSerialNum=";
    var UpdateWieghtURL = "/Huayu_WayBillWeight/UpdateWayBillWeight?wbID=";
    var UpdateWBSRportURL = "/Huayu_WayBillWeight/UpdateWBSRport?wbID=";
    var UpdateWBArrivalDateURL = "/Huayu_WayBillWeight/UpdateWBArrivalDate?wbID=";
    var UpdateDlg = null;

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL ="";// "/Huayu_WayBillWeight/GetData?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue"));

    $("#GbtnQuery").click(function () {
        QueryURL = "/Huayu_WayBillWeight/GetData?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue"));
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#GbtnReset").click(function () {
        $("#inputBeginDate").val("");
        $("#inputEndDate").val("");
        $("#txtGCode").val("");
        _$_ddCompany.combobox("setValue", "---请选择---");
        $("#GbtnQuery").click();
    });

    $("#btnPrint").click(function () {
        PrintURL = "/Huayu_WayBillWeight/Print?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Huayu_WayBillWeight/Excel?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        url: QueryURL,
        sortName: 'wbID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'wbID',
        columns: [[
					{ field: 'wbStorageDate', title: '报关日期', width: 60, sortable: true,
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
                    { field: 'swbTotalNumber', title: '分运单总件数', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetSubWayBillSumInfo' type='swbTotalNumber' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'swbTotalWeight', title: '分运单总重量（公斤）', width: 130, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetSubWayBillSumInfo' type='swbTotalWeight'  wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'ActualWeight', title: '计费重量（公斤）', width: 100, sortable: true, align: "right",
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            return rowData.ActualWeight + "&nbsp;&nbsp;&nbsp;" + "<a href='#' class='btnUpdateWayBillWeight' wbwId='" + rowData.wbwId + "' ActualWeight='" + rowData.ActualWeight + "' wbID='" + rowData.wbID + "' wbSerialNum='" + rowData.wbSerialNum + "'>修改</a>";
                        }
                    },
                    { field: 'wbSRport_Customize', title: '起运地/目的地', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            return rowData.wbSRport + "&nbsp;&nbsp;&nbsp;" + "<a href='#' class='btnUpdateWBSRport' wbwId='" + rowData.wbwId + "' wbSRport='" + rowData.wbSRport + "' wbID='" + rowData.wbID + "' wbSerialNum='" + rowData.wbSerialNum + "'>修改</a>";
                        }
                    }, { field: 'wbArrivalDate_Customize', title: '到货日期', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            return rowData.wbArrivalDate + "&nbsp;" + "<a href='#' class='btnUpdateWBArrivalDate' wbwId='" + rowData.wbwId + "' wbArrivalDate='" + rowData.wbArrivalDate + "' wbID='" + rowData.wbID + "' wbSerialNum='" + rowData.wbSerialNum + "'>修改</a>";
                        }
                    },
                    { field: 'wbStatus', title: '状态', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
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
                    },
                    { field: 'hiddenFields', title: '', hidden: true, width: 0, sortable: true,
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_getSubWayBillSumInfo' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    }
				]],
        pagination: true,
        pageSize: 15,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
        toolbar: "#toolBar",
        onRowContextMenu: function (e, rowIndex, rowData) {
            e.preventDefault();
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);

            var cmenu = $('<div id="cmenu" style="width:100px;"></div>').appendTo('body');
            $('<div  id="mnuViewSubWayBillDetail" iconCls="icon-infomation"/>').html("查看子运单明细").appendTo(cmenu);
            cmenu.menu({
                onClick: function (item) {
                    cmenu.remove();
                    switch (item.id.toLowerCase()) {
                        case "mnuviewsubwaybilldetail":
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
            _$_datagrid.datagrid("reload");
        },
        onClickRow: function (rowIndex, rowData) {
            Detail(rowData.wbSerialNum);
        },
        onLoadSuccess: function (data) {
            var allUpdateBtn = $(".btnUpdateWayBillWeight");
            var allUpdateWBSRportBtn = $(".btnUpdateWBSRport");
            var allUpdateWBArrivalDateBtn = $(".btnUpdateWBArrivalDate");
            var allSynchorizeGetSubWayBillSumInfoSpan = $(".cls_SynchorizeGetSubWayBillSumInfo[type='swbTotalNumber']");
            $.each(allUpdateBtn, function (i, item) {
                var wbID = $(item).attr("wbID");
                var wbSerialNum = $(item).attr("wbSerialNum");
                var wbActualWeight = $(item).attr("ActualWeight");
                $(item).click(function () {
                    Update(wbID, wbSerialNum, wbActualWeight);
                });
            });
            $.each(allUpdateWBSRportBtn, function (i, item) {
                var wbID = $(item).attr("wbID");
                var wbSerialNum = $(item).attr("wbSerialNum");
                var wbSRport = $(item).attr("wbSRport");
                $(item).click(function () {
                    UpdateWBSRport(wbID, wbSerialNum, wbSRport);
                });
            });
            $.each(allUpdateWBArrivalDateBtn, function (i, item) {
                var wbID = $(item).attr("wbID");
                var wbSerialNum = $(item).attr("wbSerialNum");
                var wbArrivalDate = $(item).attr("wbArrivalDate");
                $(item).click(function () {
                    UpdateWBArrivalDate(wbID, wbSerialNum, wbArrivalDate);
                });
            });
            $.each(allSynchorizeGetSubWayBillSumInfoSpan, function (i, item) {
                var strRet = "";
                var wbID = $(item).attr("wbID");
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

    function Update(wbID, wbSerialNum, wbWeight) {
        $("#hid_wbID").val(wbID);
        $("#txtWbSerialNum").val(wbSerialNum);
        $("#txtWbWeight").numberbox("setValue", wbWeight);
        UpdateDlg = $('#dlg_UpdateWayBillWeight').dialog({
            buttons: [{
                text: '保 存',
                iconCls: 'icon-ok',
                handler: function () {
                    var hid_wbID = $("#hid_wbID").val();
                    var txtWbSerialNum = $("#txtWbSerialNum").val();
                    var txtWbWeight = $('#txtWbWeight').numberbox('getValue');
                    if (hid_wbID == "" || txtWbSerialNum == "" || txtWbWeight == "") {
                        reWriteMessagerAlert('操作提示', '请填写完整信息<br/>(总运单号、计费重量)', "error");
                        return false;
                    }

                    var bOK = false;
                    $.ajax({
                        type: "POST",
                        url: UpdateWieghtURL + encodeURI(hid_wbID) + "&wayBillWeight=" + encodeURI(txtWbWeight),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            var JSONMsg = eval("(" + msg + ")");
                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                bOK = true;
                            } else {
                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                            }
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });
                    if (bOK) {
                        UpdateDlg.dialog('close');
                        $("#GbtnQuery").click();
                    }
                }
            }, {
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    UpdateDlg.dialog('close');
                }
            }],
            title: '修改计费重量',
            modal: true,
            resizable: true,
            cache: false,
            closed: true,
            left: 50,
            top: 30,
            width: 400,
            height: 150
        });

        $('#dlg_UpdateWayBillWeight').dialog("open");
    }

    function UpdateWBSRport(wbID, wbSerialNum, wbSRport) {
        $("#hid_wbID_WBSRport").val(wbID);
        $("#txtWbSerialNum_WBSRport").val(wbSerialNum);
        $("#txtWBSRport").val(wbSRport);
        UpdateDlg = $('#dlg_UpdateWBSRport').dialog({
            buttons: [{
                text: '保 存',
                iconCls: 'icon-ok',
                handler: function () {
                    var hid_wbID_WBSRport = $("#hid_wbID_WBSRport").val();
                    var txtWbSerialNum_WBSRport = $("#txtWbSerialNum_WBSRport").val();
                    var txtWBSRport = $('#txtWBSRport').val();
                    if (hid_wbID_WBSRport == "" || txtWbSerialNum_WBSRport == "") {
                        reWriteMessagerAlert('操作提示', '请填写完整信息<br/>(总运单号)', "error");
                        return false;
                    }

                    var bOK = false;
                    $.ajax({
                        type: "POST",
                        url: UpdateWBSRportURL + encodeURI(hid_wbID_WBSRport) + "&WBSRport=" + encodeURI(txtWBSRport),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            var JSONMsg = eval("(" + msg + ")");
                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                bOK = true;
                            } else {
                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                            }
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });
                    if (bOK) {
                        UpdateDlg.dialog('close');
                        $("#GbtnQuery").click();
                    }
                }
            }, {
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    UpdateDlg.dialog('close');
                }
            }],
            title: '修改起运地/目的地',
            modal: true,
            resizable: true,
            cache: false,
            closed: true,
            left: 50,
            top: 30,
            width: 400,
            height: 150
        });

        $('#dlg_UpdateWBSRport').dialog("open");
    }

    function UpdateWBArrivalDate(wbID, wbSerialNum, wbArrivalDate) {
        $("#hid_wbID_WBArrivalDate").val(wbID);
        $("#txtWbSerialNum_WBArrivalDate").val(wbSerialNum);
        $("#txtWBArrivalDate").val(wbArrivalDate);
        UpdateDlg = $('#dlg_UpdateWBArrivalDate').dialog({
            buttons: [{
                text: '保 存',
                iconCls: 'icon-ok',
                handler: function () {
                    var hid_wbID_WBArrivalDate = $("#hid_wbID_WBArrivalDate").val();
                    var txtWbSerialNum_WBArrivalDate = $("#txtWbSerialNum_WBArrivalDate").val();
                    var txtWBArrivalDate = $('#txtWBArrivalDate').val();
                    if (hid_wbID_WBArrivalDate == "" || txtWbSerialNum_WBArrivalDate == "" || txtWBArrivalDate == "") {
                        reWriteMessagerAlert('操作提示', '请填写完整信息<br/>(总运单号、到货日期)', "error");
                        return false;
                    }

                    var bOK = false;
                    $.ajax({
                        type: "POST",
                        url: UpdateWBArrivalDateURL + encodeURI(hid_wbID_WBArrivalDate) + "&WBArrivalDate=" + encodeURI(txtWBArrivalDate),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            var JSONMsg = eval("(" + msg + ")");
                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                bOK = true;
                            } else {
                                reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                            }
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });
                    if (bOK) {
                        UpdateDlg.dialog('close');
                        $("#GbtnQuery").click();
                    }
                }
            }, {
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    UpdateDlg.dialog('close');
                }
            }],
            title: '修改到货日期',
            modal: true,
            resizable: true,
            cache: false,
            closed: true,
            left: 50,
            top: 30,
            width: 400,
            height: 150
        });

        $('#dlg_UpdateWBArrivalDate').dialog("open");
    }

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
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "wbserialnum":
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
