$(function () {
    var _$_datagrid = $("#DG_InOutStoreExceptionLog");
    var _$_InOutStoreExceptionEventType = $('#txtInOutStoreExceptionEventType');

    _$_InOutStoreExceptionEventType.combotree('loadData', [
    {
        id: -99,
        text: '---请选择(可多选)---'
    },
    {
        id: 1,
        text: '未预入库时入库'
    },
    {
        id: 2,
        text: '重复入库'
    },
    {
        id: 3,
        text: '已出库时入库'
    },
    {
        id: 4,
        text: '入库异常时入库'
    },
    {
        id: 5,
        text: '出库异常时入库'
    },
    {
        id: 6,
        text: '未预入库时出库'
    },
    {
        id: 7,
        text: '未入库时出库'
    },
    {
        id: 8,
        text: '重复出库'
    },
    {
        id: 9,
        text: '入库异常时出库'
    },
    {
        id: 10,
        text: '出库异常时出库'
    },
    {
        id: 11,
        text: '数据格式不正确'
    }, 
    {
        id: 12,
        text: '未放行却出库'
    },
    {
        id: 99,
        text: '未知'
    }]);

    _$_InOutStoreExceptionEventType.combotree("setValue", "-99");

    var PrintURL = "";
    var QueryURL = "/Huayu_InOutStoreExceptionEvent/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtInOutStoreExceptionEventType=" + encodeURI(_$_InOutStoreExceptionEventType.combotree("getValues").join(',')) + "&txtWayBillCode=" + encodeURI($("#txtWayBillCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtOperator=" + encodeURI($("#txtOperator").val());
    var DeleURL = "/Huayu_InOutStoreExceptionEvent/Delete?ids=";

    $("#btnQuery").click(function () {
        QueryURL = "/Huayu_InOutStoreExceptionEvent/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtInOutStoreExceptionEventType=" + encodeURI(_$_InOutStoreExceptionEventType.combotree("getValues").join(',')) + "&txtWayBillCode=" + encodeURI($("#txtWayBillCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtOperator=" + encodeURI($("#txtOperator").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnDelete").click(function () {
        Delete();
    });

    $("#btnSeleAll").click(function () {
        SeleAll();
    });

    $("#btnInverseSele").click(function () {
        InverseSele();
    });

    $("#btnReset").click(function () {
        $("#txtBeginD").val("");
        $("#txtEndD").val("");
        $("#txtInOutStoreExceptionEventType").combotree("setValue", "-99");
        $("#txtWayBillCode").val("");
        $("#txtSubWayBillCode").val("");
        $("#txtOperator").val("");
        $("#btnQuery").click();
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
        sortName: 'operateDate',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'WblID',
        columns: [[
                    { field: 'cb', title: '', width: 100, checkbox: true
                    },
					{ field: 'operateDate', title: '发生日期', width: 100, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'Wbl_wbSerialNum', title: '总运单号', width: 150, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
					{ field: 'Wbl_swbSerialNum', title: '子运单号(扫描得到的单号)', width: 200, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'status', title: '事件类型', width: 150, sortable: true
                    },
                    { field: 'operator', title: '操作员', width: 150, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'WblID', title: '', hidden: true
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

            var cmenu = $('<div id="cmenu" style="width:200px;"></div>').appendTo('body');
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
        },
        onDblClickRow: function (rowIndex, rowData) {
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);
            ExceptionHandle(rowData.WbfID);
        }
    });

    //设置分页控件   
    var p = _$_datagrid.datagrid('getPager');
    $(p).pagination({
        pageSize: 15,
        pageList: [10, 15, 20, 25, 30]
    });

    $("#btnQuery").click();

    function Print() {
        PrintURL = "/Huayu_InOutStoreExceptionEvent/Print?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtInOutStoreExceptionEventType=" + encodeURI(_$_InOutStoreExceptionEventType.combotree("getValues").join(',')) + "&txtWayBillCode=" + encodeURI($("#txtWayBillCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtOperator=" + encodeURI($("#txtOperator").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Huayu_InOutStoreExceptionEvent/Excel?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtInOutStoreExceptionEventType=" + encodeURI(_$_InOutStoreExceptionEventType.combotree("getValues").join(',')) + "&txtWayBillCode=" + encodeURI($("#txtWayBillCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtOperator=" + encodeURI($("#txtOperator").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    }

    function Delete() {
        reWriteMessagerConfirm("提示", "您确定需要删除所选的异常事件吗？",
                    function (ok) {
                        if (ok) {
                            var selects = _$_datagrid.datagrid("getSelections");
                            var ids = [];
                            for (var i = 0; i < selects.length; i++) {
                                ids.push(selects[i].WblID);
                            }
                            if (selects.length == 0) {
                                reWriteMessagerAlert("提示", "<center>请选择需要删除的数据</center>", "error");
                                return false;
                            }

                            var bOK = false;
                            $.ajax({
                                type: "POST",
                                url: DeleURL + ids.join(','),
                                data: "",
                                async: false,
                                cache: false,
                                beforeSend: function (XMLHttpRequest) {

                                },
                                success: function (msg) {
                                    var JSONMsg = eval("(" + msg + ")");
                                    if (JSONMsg.result.toLowerCase() == 'ok') {
                                        reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                        bOK = true;
                                    } else {
                                        reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                                        bOK = false;
                                    }
                                },
                                complete: function (XMLHttpRequest, textStatus) {

                                },
                                error: function () {

                                }
                            });
                            if (bOK) {
                                _$_datagrid.datagrid("reload");
                                _$_datagrid.datagrid("unselectAll");
                            }

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
                case "wblid":
                    break;
                case "cb":
                    break;
                case "wbl_swbserialnum":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "Wbl_swbSerialNum") {

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
