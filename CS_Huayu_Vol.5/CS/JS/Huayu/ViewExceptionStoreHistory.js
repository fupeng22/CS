$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddCompany = $('#txtVoyage');
    var _$_ddlExceptionType = $("#ddlExceptionType");
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    var DeleteURL = "/Huayu_ViewExceptionStoreHistory/DeleteInfo?ids=";

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddlExceptionType.combotree('loadData', [
    {
        id: -99,
        text: '---请选择(可多选)---'
    },
    {
        id: 2,
        text: '异常入库'
    },
    {
        id: 4,
        text: '异常出库'
    }]);

    _$_ddlExceptionType.combotree("setValue", "-99");

    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL = "/Huayu_ViewExceptionStoreHistory/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtExceptionBeginD=" + encodeURI($("#txtExceptionBeginD").val()) + "&txtExceptionEndD=" + encodeURI($("#txtExceptionEndD").val()) + "&txtExceptionType=" + encodeURI(_$_ddlExceptionType.combotree("getValues").join(',')) + "&txtExceptionStatus=" + encodeURI($("#txtExceptionStatus").combobox("getValue"));

    $("#btnQuery").click(function () {
        QueryURL = "/Huayu_ViewExceptionStoreHistory/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtExceptionBeginD=" + encodeURI($("#txtExceptionBeginD").val()) + "&txtExceptionEndD=" + encodeURI($("#txtExceptionEndD").val()) + "&txtExceptionType=" + encodeURI(_$_ddlExceptionType.combotree("getValues").join(',')) + "&txtExceptionStatus=" + encodeURI($("#txtExceptionStatus").combobox("getValue"));
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnReset").click(function () {
        $("#txtBeginD").val("");
        $("#txtEndD").val("");
        $("#txtExceptionBeginD").val("");
        $("#txtExceptionEndD").val("");
        $("#txtCode").val("");
        $("#txtSubWayBillCode").val("");
        _$_ddlExceptionType.combotree("setValue", "-99");
        $("#txtExceptionStatus").combobox("setValue", "---请选择---");
        _$_ddCompany.combobox("setValue", "---请选择---");
        $("#btnQuery").click();
    });


    $("#btnDele").click(function () {
        Delete();
    });

    $("#btnSeleAll").click(function () {
        SeleAll();
    });

    $("#btnInverseSele").click(function () {
        InverseSele();
    });

    $("#btnPrint").click(function () {
        Print();
    });

    $("#btnExcel").click(function () {
        Excel();
    });


    _$_datagrid.datagrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'wbStorageDate',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        columns: [[
                    { field: 'cb', title: '', width: 100, checkbox: true
                    },
					{ field: 'wbStorageDate', title: '报关日期', width: 80, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'ExceptionDate', title: '异常日期', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },

					{ field: 'wbCompany', title: '货代公司', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'wbSerialNum', title: '异常总运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbSerialNum', title: '异常分运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ExceptionTypeDescription', title: '异常类型', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                     { field: 'ExceptionOperator', title: '异常操作员', width: 100, sortable: true,
                         sorter: function (a, b) {
                             return (a > b ? 1 : -1);
                         }
                     },
                    { field: 'ExceptionStatusDescription', title: '异常状态', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ExceptionDescription', title: '异常描述', width: 250, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'HandleDescription', title: '处理意见', width: 250, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'HandleDate', title: '处理时间', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'Handler', title: '处理人', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'WbfID', title: '', hidden: true
                    }
				]],
        pagination: true,
        toolbar: "#toolBar",
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
        onRowContextMenu: function (e, rowIndex, rowData) {
            e.preventDefault();
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);

            var cmenu = $('<div id="cmenu" style="width:100px;"></div>').appendTo('body');
            $('<div  id="mnuDelete" iconCls="icon-remove"/>').html("删除").appendTo(cmenu);
            $('<div  id="mnuSeleAll" iconCls="icon-seleall"/>').html("全选").appendTo(cmenu);
            $('<div  id="mnuInverseSele" iconCls="icon-inversesele"/>').html("反选").appendTo(cmenu);
            $('<div  id="mnuPrint" iconCls="icon-print"/>').html("打印").appendTo(cmenu);
            $('<div  id="mnuExcel" iconCls="icon-excel"/>').html("导出").appendTo(cmenu);
            cmenu.menu({
                onClick: function (item) {
                    cmenu.remove();
                    switch (item.id.toLowerCase()) {
                        case "mnudelete":
                            Delete();
                            break;
                        case "mnuseleall":
                            SeleAll();
                            break;
                        case "mnuinversesele":
                            InverseSele();
                            break;
                        case "mnuprint":
                            Print();
                            break;
                        case "mnuexcel":
                            Excel();
                            break;
                    }
                }
            });

            $('#cmenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        },
        onSortColumn: function (sort, order) {
            //_$_datagrid.datagrid("reload");
        }
    });

    //设置分页控件   
    var p = _$_datagrid.datagrid('getPager');
    $(p).pagination({
        pageSize: 15,
        pageList: [10, 15, 20, 25, 30]
    });

    $("#btnQuery").click();

    function Delete() {
        reWriteMessagerConfirm("提示", "您确定需要删除所选的历史异常件处理信息吗？",
            function (ok) {
                if (ok) {
                    var selects = _$_datagrid.datagrid("getSelections");
                    var ids = [];
                    for (var i = 0; i < selects.length; i++) {
                        ids.push(selects[i].WbfID);
                    }
                    if (selects.length == 0) {
                        reWriteMessagerAlert("提示", "<center>请选择需要删除的数据</center>", "error");
                        return false;
                    }
                    $.ajax({
                        type: "GET",
                        url: DeleteURL + ids.join(','),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            var JSONMsg = eval("(" + msg + ")");
                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                reWriteMessagerAlert('提示', JSONMsg.message, "info");
                            } else {
                                reWriteMessagerAlert('提示', JSONMsg.message, "error");
                            }
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });

                    _$_datagrid.datagrid("reload");
                    _$_datagrid.datagrid("unselectAll");
                } else {

                }
            }
          );
    }

    function SeleAll() {
        var rows = _$_datagrid.datagrid("getRows");
        for (var i = 0; i < rows.length; i++) {
            var m = _$_datagrid.datagrid("getRowIndex", rows[i]);
            _$_datagrid.datagrid("selectRow", m)
        }
    }

    function InverseSele() {
        var rows = _$_datagrid.datagrid("getRows");
        var selects = _$_datagrid.datagrid("getSelections");
        for (var i = 0; i < rows.length; i++) {
            var bSele = false;
            var m = _$_datagrid.datagrid("getRowIndex", rows[i]);
            for (var j = 0; j < selects.length; j++) {
                var n = _$_datagrid.datagrid("getRowIndex", selects[j]);
                if (m == n) {
                    bSele = true;
                }
            }
            if (bSele) {
                _$_datagrid.datagrid("unselectRow", m)
            } else {
                _$_datagrid.datagrid("selectRow", m)
            }
        }
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "wbserialnum":
                    break;
                case "cb":
                    break;
                case "wbfid":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "Wbf_wbSerialNum") {

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

    function Print() {
        PrintURL = "/Huayu_ViewExceptionStoreHistory/Print?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtExceptionBeginD=" + encodeURI($("#txtExceptionBeginD").val()) + "&txtExceptionEndD=" + encodeURI($("#txtExceptionEndD").val()) + "&txtExceptionType=" + encodeURI(_$_ddlExceptionType.combotree("getValues").join(',')) + "&txtExceptionStatus=" + encodeURI($("#txtExceptionStatus").combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

    }

    function Excel() {
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

        PrintURL = "/Huayu_ViewExceptionStoreHistory/Excel?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtExceptionBeginD=" + encodeURI($("#txtExceptionBeginD").val()) + "&txtExceptionEndD=" + encodeURI($("#txtExceptionEndD").val()) + "&txtExceptionType=" + encodeURI(_$_ddlExceptionType.combotree("getValues").join(',')) + "&txtExceptionStatus=" + encodeURI($("#txtExceptionStatus").combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    }
});
