$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddCompany = $('#txtVoyage');
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";
    var DetailURL = "/ViewSubWayBillDetail/Index?Detail_bEnableReject=1&Detail_wbSerialNum=";
    //var DetailURL = "/ViewSubWayBillDetail/Index?Detail_bEnableReject=1&Detail_wbSerialNum=";

    var getSubWayBillSumInfoURL = "/ViewSubWayBillDetail/getSubWayBillSumInfo?wbID=";

    var DetailDlg = null;
    var Detail_RejectSubWayBillDlg = null;
    var Detail_RejectSubWayURL = "/ViewRejectSubWayBillDetail/Index?Detail_wbSerialNum=";

    var PrintRejectWayBillURL = "/RejectSheetSetting/Index?wbID=";
    var PrintRejectWayBill_Dlg = null;

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL = "";// "/Huayu_RejectWayBillHandler/GetData?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue"));

    $("#GbtnQuery").click(function () {
        QueryURL = "/Huayu_RejectWayBillHandler/GetData?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue"));
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
        PrintURL = "/Huayu_RejectWayBillHandler/Print?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Huayu_RejectWayBillHandler/Excel?inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
					{ field: 'wbStorageDate', title: '报关日期', width: 90, sortable: true,
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
                    { field: 'wbStatus', title: '状态', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'releseNum', title: '放行件数', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'notReleseNum', title: '扣留件数', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'TotalRejectSubWayBill', title: '退货件数', width: 80, sortable: true, align: "center",
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            switch (rowData.TotalRejectSubWayBill) {
                                case "0":
                                    return rowData.TotalRejectSubWayBill;
                                    break;
                                case "无":
                                    return rowData.TotalRejectSubWayBill;
                                    break;
                                default:
                                    return "<a href='#' class='ViewRejectSubWayBill' wbSerialNum='" + rowData.wbSerialNum + "'>" + rowData.TotalRejectSubWayBill + "</a>";
                                    break;
                            }

                        }
                    },
                    { field: 'printRejectWayBill', title: '操作', width: 120,
                        formatter: function (value, rowData, rowIndex) {
                            if (rowData.TotalRejectSubWayBill == "0" || rowData.TotalRejectSubWayBill == "无") {

                            } else {
                                if (rowData.anyOutStore != "0") {
                                    return "<a href='#' class='printRejectWayBill_cls' wbID='" + rowData.wbID + "'>打印退货单</a>";
                                } else {
                                    return "<span style='color:red;font-weight:bold'>该总单从未进行出库</span>";
                                }

                            }
                        }
                    },
                    { field: 'wbImportType', title: '数据来源', width: 100, sortable: true,
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
            Detail(rowData.wbSerialNum, "-99");
        },
        onLoadSuccess: function (data) {
            var allViewRejectSubWayBillBtn = $(".ViewRejectSubWayBill");
            var printRejectWayBillBtn = $(".printRejectWayBill_cls");
            var allSynchorizeGetSubWayBillSumInfoSpan = $(".cls_SynchorizeGetSubWayBillSumInfo[type='swbTotalNumber']");
            $.each(allViewRejectSubWayBillBtn, function (i, item) {
                $(item).click(function () {
                    var wbSerialNum = $(item).attr("wbSerialNum");
                    Detail(wbSerialNum, "99");
                });
            });
            $.each(printRejectWayBillBtn, function (i, item) {
                $(item).click(function () {
                    var wbID = $(item).attr("wbID");
                    PrintRejectSubWayBill(wbID);
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

    function PrintRejectSubWayBill(wbID) {
        var div_GlobalPrintSheetDlg = self.parent.$("#dlg_GlobalPrintSheet");
        PrintRejectWayBill_Dlg = div_GlobalPrintSheetDlg.dialog({
            buttons: [{
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    PrintRejectWayBill_Dlg.dialog('close');
                }
            }],
            title: '退货单打印设置',
            href: PrintRejectWayBillURL + encodeURI(wbID),
            modal: true,
            resizable: true,
            cache: false,
            left: 0,
            top: 0,
            width: 850,
            height: 490,
            closed: true
        });
        _$_datagrid.datagrid("unselectAll");

        div_GlobalPrintSheetDlg.dialog("open");
    }

    //    function Detail(wbSerialNum) {
    //        var div_DetailDlg = self.parent.$("#dlg_GlobalDetail");
    //        if (wbSerialNum) {
    //            DetailDlg = div_DetailDlg.dialog({
    //                buttons: [{
    //                    text: '关 闭',
    //                    iconCls: 'icon-cancel',
    //                    handler: function () {
    //                        DetailDlg.dialog('close');
    //                    }
    //                }],
    //                title: '查看子运单明细',
    //                href: DetailURL + wbSerialNum,
    //                modal: true,
    //                resizable: true,
    //                cache: false,
    //                left: 0,
    //                top: 0,
    //                width: 1020,
    //                height: 480,
    //                closed: true
    //            });
    //            _$_datagrid.datagrid("unselectAll");
    //        } else {
    //            var selects = _$_datagrid.datagrid("getSelections");
    //            if (selects.length != 1) {
    //                reWriteMessagerAlert("提示", "<center>请选择数据进行查看(<font style='color:red'>只可查看一行</font>)</center>", "error");
    //                return false;
    //            } else {
    //                var wbSerialNum = selects[0].wbSerialNum;
    //                DetailDlg = div_DetailDlg.dialog({
    //                    buttons: [{
    //                        text: '关 闭',
    //                        iconCls: 'icon-cancel',
    //                        handler: function () {
    //                            DetailDlg.dialog('close');
    //                        }
    //                    }],
    //                    title: '查看子运单明细',
    //                    href: DetailURL + wbSerialNum,
    //                    modal: true,
    //                    resizable: true,
    //                    cache: false,
    //                    left: 0,
    //                    top: 0,
    //                    width: 1020,
    //                    height: 480,
    //                    closed: true
    //                });
    //                _$_datagrid.datagrid("unselectAll");
    //            }
    //        }

    //        div_DetailDlg.dialog("open");
    //    }

    function Detail(wbSerialNum, defaultStatus) {
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
                href: DetailURL + wbSerialNum + "&Detail_swbStatus=" + defaultStatus,
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
                    href: DetailURL + wbSerialNum + "&Detail_swbStatus=" + defaultStatus,
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

    //    function Detail_RejectSubWayBill(wbSerialNum) {
    //       var div_DetailDlg = self.parent.$("#dlg_GlobalDetail");
    //        if (wbSerialNum) {
    //            DG_ViewRejectSubWayBillDetailResult = div_DetailDlg.dialog({
    //                buttons: [{
    //                    text: '关 闭',
    //                    iconCls: 'icon-cancel',
    //                    handler: function () {
    //                        DG_ViewRejectSubWayBillDetailResult.dialog('close');
    //                    }
    //                }],
    //                title: '查看退货子运单明细',
    //                href: Detail_RejectSubWayURL + wbSerialNum,
    //                modal: true,
    //                resizable: true,
    //                cache: false,
    //                left: 0,
    //                top: 0,
    //                width: 1020,
    //                height: 480,
    //                closed: true
    //            });
    //            _$_datagrid.datagrid("unselectAll");
    //        } else {
    //            var selects = _$_datagrid.datagrid("getSelections");
    //            if (selects.length != 1) {
    //                reWriteMessagerAlert("提示", "<center>请选择数据进行查看(<font style='color:red'>只可查看一行</font>)</center>", "error");
    //                return false;
    //            } else {
    //                var wbSerialNum = selects[0].wbSerialNum;
    //                DG_ViewRejectSubWayBillDetailResult = div_DetailDlg.dialog({
    //                    buttons: [{
    //                        text: '关 闭',
    //                        iconCls: 'icon-cancel',
    //                        handler: function () {
    //                            DG_ViewRejectSubWayBillDetailResult.dialog('close');
    //                        }
    //                    }],
    //                    title: '查看退货子运单明细',
    //                    href: Detail_RejectSubWayURL + wbSerialNum,
    //                    modal: true,
    //                    resizable: true,
    //                    cache: false,
    //                    left: 0,
    //                    top: 0,
    //                    width: 1020,
    //                    height: 480,
    //                    closed: true
    //                });
    //                _$_datagrid.datagrid("unselectAll");
    //            }
    //        }

    //        div_DetailDlg.dialog("open");
    //    }

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
