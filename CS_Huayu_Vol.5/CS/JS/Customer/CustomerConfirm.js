$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddStatus = $("#ddStatus");
    var _$_ddCompany = $('#txtVoyage');
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    var getWayBillSumInfoURL = "/Customer_Confirm/getWayBillSumInfoBywbID?wbID=";

    var DetailURL = "/ViewSubWayBillDetail/Index?Detail_wbSerialNum=";

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL = "";// "/Customer_Confirm/GetData?ddStatus=" + encodeURI(_$_ddStatus.combobox("getValue")) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue"));
    var UpdateReleaseStatus = "/Customer_Confirm/UpdateReleaseStatus?ids=";

    $("#GbtnQuery").click(function () {
        QueryURL = "/Customer_Confirm/GetData?ddStatus=" + encodeURI(_$_ddStatus.combobox("getValue")) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue"));
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
        _$_ddStatus.combobox("setValue", "-1");
        $("#GbtnQuery").click();
    });

    $("#btnPrint").click(function () {
        PrintURL = "/Customer_Confirm/Print?ddStatus=" + encodeURI(_$_ddStatus.combobox("getValue")) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Customer_Confirm/Excel?ddStatus=" + encodeURI(_$_ddStatus.combobox("getValue")) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    });

    function ConfirmRelease() {
        var ids = "";

        var selectRows = _$_datagrid.datagrid("getSelections");
        for (var i = 0; i < selectRows.length; i++) {
            ids = ids + selectRows[i].wbID + "*";
        }
        if (ids == "") {
            reWriteMessagerAlert('操作提示', "请先选择需要放行的运单号", 'error');
            return false;
        }
        $.ajax({
            type: "GET",
            url: UpdateReleaseStatus + ids,
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                var msg = eval("(" + msg + ")");
                if (msg.result == 'ok') {
                    reWriteMessagerAlert('操作提示', msg.message, 'info');
                    _$_datagrid.datagrid("reload");
                    _$_datagrid.datagrid("unselectAll");

                } else {
                    reWriteMessagerAlert('操作提示', msg.message, 'error');
                    return false;
                }
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
    }

    _$_ddStatus.combobox({
        onChange: function (d1, d2) {
            QueryURL = "/Customer_Confirm/GetData?ddStatus=" + encodeURI(d1) + "&inputBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&inputEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtGCode=" + encodeURI($("#txtGCode").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue"));
            window.setTimeout(function () {
                $.extend(_$_datagrid.datagrid("options"), {
                    url: QueryURL
                });
                _$_datagrid.datagrid("reload");
            }, 10); //延迟100毫秒执行，时间可以更短
        }
    });

    $("#btnConfirmRelese").click(function () {
        ConfirmRelease();
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
                     { field: 'cb', width: 120, checkbox: true },
					{ field: 'wbStorageDate', title: '导入日期', width: 120, sortable: true,
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
                    { field: 'wbSubNumber_Custom', title: '分运单数', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetWayBillSumInfo' type='wbSubNumber_Custom' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'wbReleaseCount_Custom', title: '放行件数', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetWayBillSumInfo' type='wbReleaseCount_Custom' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'wbUnReleaseCount_Custom', title: '扣留件数', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetWayBillSumInfo' type='wbUnReleaseCount_Custom' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'wbNotProCount_Custom', title: '未上机件数', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetWayBillSumInfo' type='wbNotProCount_Custom' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'wbImportType', title: '数据来源', width: 120, sortable: true,
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
        pageSize: 15,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
        toolbar: "#toolBar",
        onRowContextMenu: function (e, rowIndex, rowData) {
            e.preventDefault();
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);

            var cmenu = $('<div id="cmenu" style="width:100px;"></div>').appendTo('body');
            $('<div  id="mnuConfirmRelease" iconCls="icon-people-accept"/>').html("确认放行").appendTo(cmenu);
            $('<div  id="mnuViewSubWayBillDetail" iconCls="icon-infomation"/>').html("查看子运单明细").appendTo(cmenu);
            cmenu.menu({
                onClick: function (item) {
                    cmenu.remove();
                    switch (item.id.toLowerCase()) {
                        case "mnuconfirmrelease":
                            ConfirmRelease();
                            break;
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
            //_$_datagrid.datagrid("reload");
        },
        onClickRow: function (rowIndex, rowData) {
            Detail(rowData.wbSerialNum);
        },
        onLoadSuccess: function (data) {
            var allSynchorizeGetSubWayBillSumInfoSpan = $(".cls_SynchorizeGetWayBillSumInfo[type='wbNotProCount_Custom']");
            $.each(allSynchorizeGetSubWayBillSumInfoSpan, function (i, item) {
                var strRet = "";
                var wbID = $(item).attr("wbID");
                $.ajax({
                    type: "POST",
                    url: getWayBillSumInfoURL + encodeURI(wbID),
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
                        $(".cls_SynchorizeGetWayBillSumInfo[wbID=" + wbID + "][type='wbSubNumber_Custom']").html(msg.wbSubNumber_Custom.toString());
                        $(".cls_SynchorizeGetWayBillSumInfo[wbID=" + wbID + "][type='wbReleaseCount_Custom']").html(msg.wbReleaseCount_Custom.toString());
                        var s = "";
                        if (parseInt(msg.wbUnReleaseCount_Custom) > 0) {
                            s = "<div style='background-color:red;width:100%'>" + msg.wbUnReleaseCount_Custom + "</div>";
                        } else {
                            s = msg.wbUnReleaseCount_Custom;
                        }
                        $(".cls_SynchorizeGetWayBillSumInfo[wbID=" + wbID + "][type='wbUnReleaseCount_Custom']").html(s);
                        $(".cls_SynchorizeGetWayBillSumInfo[wbID=" + wbID + "][type='wbNotProCount_Custom']").html(msg.wbNotProCount_Custom.toString());
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
                case "cb":
                    break;
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
