$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddCompany = $('#txtVoyage');
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    var DetailURL = "/ViewSubWayBillDetail/Index?Detail_wbSerialNum=";

    var InOutStoreWayBillDetailURL = "/ViewInOutStoreWayBillDetail/Index?Detail_wbSerialNum=";

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL = "/Huayu_AnanalyseStatistic/GetData?txtvStorageBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&txtvStorageEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtvSerialNum=" + encodeURI($("#txtGCode").val()) + "&txtvCompany=" + encodeURI(_$_ddCompany.combobox("getValue"));

    $("#btnQuery").click(function () {
        QueryURL = "/Huayu_AnanalyseStatistic/GetData?txtvStorageBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&txtvStorageEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtvSerialNum=" + encodeURI($("#txtGCode").val()) + "&txtvCompany=" + encodeURI(_$_ddCompany.combobox("getValue"));
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnReset").click(function () {
        $("#inputBeginDate").val("");
        $("#inputEndDate").val("");
        $("#txtGCode").val("");
        _$_ddCompany.combobox("setValue", "---请选择---");
        $("#btnQuery").click();
    });

    $("#btnPrint").click(function () {
        PrintURL = "/Huayu_AnanalyseStatistic/Print?txtvStorageBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&txtvStorageEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtvSerialNum=" + encodeURI($("#txtGCode").val()) + "&txtvCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Huayu_AnanalyseStatistic/Excel?txtvStorageBeginDate=" + encodeURI($("#inputBeginDate").val()) + "&txtvStorageEndDate=" + encodeURI($("#inputEndDate").val()) + "&txtvSerialNum=" + encodeURI($("#txtGCode").val()) + "&txtvCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        sortName: 'dStorageDate',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'dStorageDate',
        columns: [[
					{ field: 'dStorageDate', title: '报关日期', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'vCompany', title: '货代公司', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'vSerialNum', title: '总运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'iTotalNum', title: '子运单总件数', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'dTotalWeight', title: '子运单总重量(公斤)', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'InStoreNum', title: '入库件数', width: 120, sortable: true, align: "center",
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            return "<a href='#' class='load_InOutStore_WayBills' wbSerialNum='" + rowData.vSerialNum + "' InOutType='1'>" + rowData.InStoreNum + "</a>";
                        }
                    },
                    { field: 'OutStoreNum', title: '出库件数', width: 120, sortable: true, align: "center",
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            return "<a href='#' class='load_InOutStore_WayBills' wbSerialNum='" + rowData.vSerialNum + "' InOutType='3'>" + rowData.OutStoreNum + "</a>";
                        }
                    },
                    { field: 'ReleaseNum', title: '放行件数', width: 120, sortable: true, hidden: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'NotReleaseNum', title: '扣留件数', width: 120, sortable: true, hidden: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbID', title: '', hidden: true

                    }
				]],
        pagination: true,
        toolbar: "#toolBar",
        onRowContextMenu: function (e, rowIndex, rowData) {
            e.preventDefault();
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);

            var cmenu = $('<div id="cmenu" style="width:100px;"></div>').appendTo('body');
            $('<div  id="mnuViewSubWayBillDetail"  iconCls="icon-infomation"/>').html("查看子运单明细").appendTo(cmenu);
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
            //_$_datagrid.datagrid("reload");
        },
        onClickRow: function (rowIndex, rowData) {
            Detail(rowData.vSerialNum);
        },
        onLoadSuccess: function (data) {
            var allViewInOutStoreBtn = $(".load_InOutStore_WayBills");
            $.each(allViewInOutStoreBtn, function (i, item) {
                $(item).click(function () {
                    var ViewInOutStoreDlg = $('#dlg_InOutStoreDetail').dialog({
                        buttons: [{
                            text: '关 闭',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                ViewInOutStoreDlg.dialog('close');
                            }
                        }],
                        title: '根据条件查询出入库明细记录',
                        href: InOutStoreWayBillDetailURL + encodeURI($(item).attr("wbSerialNum")) + "&Detail_InOutStoreType=" + encodeURI($(item).attr("InOutType")),
                        modal: true,
                        resizable: true,
                        cache: false,
                        left: 50,
                        top: 30,
                        width: 970,
                        height: 480,
                        closed: true
                    });

                    $('#dlg_InOutStoreDetail').dialog("open");
                });
            });
        }
    });

    //设置分页控件   
    var p = _$_datagrid.datagrid('getPager');
    $(p).pagination({
        pageSize: 15,
        pageList: [15, 30, 45, 60, 75]
    });

    $("#btnQuery").click();

    function Detail(wbSerialNum) {
        if (wbSerialNum) {
            DetailDlg = $('#dlg_Detail').dialog({
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
                left: 50,
                top: 10,
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
                var wbSerialNum = selects[0].vSerialNum;
                DetailDlg = $('#dlg_Detail').dialog({
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
                    left: 50,
                    top: 10,
                    width: 1020,
                    height: 480,
                    closed: true
                });
                _$_datagrid.datagrid("unselectAll");
            }
        }

        $('#dlg_Detail').dialog("open");
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "vserialnum":
                    break;
                case "wbid":
                    break;
                case "releasenum":
                    break;
                case "notreleasenum":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "vSerialNum") {

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
