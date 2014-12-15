$(function () {
    var _$_datagrid = $("#DG_InOutStoreDetail");
    var _$_ddlInOutStoreType = $('#ddlInOutStoreType');

    var PatchOutStoreURL = "/ViewInOutStoreWayBillDetail/PatchRemoveStore?strSwbIds=";

    _$_ddlInOutStoreType.combotree({
        data: [{ "text": "---请选择---", "id": "-99" }, { "text": "正常已入库", "id": "1" }, { "text": "正常已出库", "id": "3"}],
        valueField: 'id',
        textField: 'text',
        panelHeight: null,
        editable: false,
        readonly: $("#Detail_EnableSelectInOutStoreType").val() == "0" ? true : false,
        onLoadSuccess: function (node, data) {
            switch ($("#hid_InOutStoreType").val()) {
                case "":
                    _$_ddlInOutStoreType.combotree("setValues", ["-99"]);
                    break;
                default:
                    _$_ddlInOutStoreType.combotree("setValues", eval("[" + $("#hid_InOutStoreType").val() + "]"));
                    break;
            }
        }
    });

    var PrintURL = "";
    var QueryURL = "/ViewInOutStoreWayBillDetail/GetData?Detail_wbSerialNum=" + encodeURI($("#txtCode_InOutStoreDetail").val()) + "&Detail_InOutStoreType=" + encodeURI(_$_ddlInOutStoreType.combotree("getValues").join(',')) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_InOutStoreDetail").val());

    $("#btnInOutStoreDetailQuery").click(function () {
        QueryURL = "/ViewInOutStoreWayBillDetail/GetData?Detail_wbSerialNum=" + encodeURI($("#txtCode_InOutStoreDetail").val()) + "&Detail_InOutStoreType=" + encodeURI(_$_ddlInOutStoreType.combotree("getValues").join(',')) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_InOutStoreDetail").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnPrint_InOutStoreDetaildlg").click(function () {
        PrintURL = "/ViewInOutStoreWayBillDetail/Print?Detail_wbSerialNum=" + encodeURI($("#txtCode_InOutStoreDetail").val()) + "&Detail_InOutStoreType=" + encodeURI(_$_ddlInOutStoreType.combotree("getValues").join(',')) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_InOutStoreDetail").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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


    $("#btnExcel_InOutStoreDetaildlg").click(function () {
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

        PrintURL = "/ViewInOutStoreWayBillDetail/Excel?Detail_wbSerialNum=" + encodeURI($("#txtCode_InOutStoreDetail").val()) + "&Detail_InOutStoreType=" + encodeURI(_$_ddlInOutStoreType.combotree("getValues").join(',')) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_InOutStoreDetail").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'operateDate',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'WbfID',
        columns: [[
                    { field: 'cb', title: '', checkbox: true
                    },
                    { field: 'wbSerialNum', title: '总运单号', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbSerialNum', title: '分运单号', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'InOutStoreTypeDescription', title: '出入库类型', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'operateDate', title: '入库日期', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'OutStoreDate', title: '出库日期', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
					{ field: 'wbCompany', title: '货代公司', width: 100, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'swbDescription_CHN', title: '货物中文名', width: 160, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbDescription_ENG', title: '货物英文名', width: 160, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNumber', title: '件数', width: 50, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbWeight', title: '重量(公斤)', width: 70, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
					{ field: 'Wbf_swbID', title: '主键', hidden: true, width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					}
				]],
        pagination: true,
        pageSize: 20,
        pageList: [20, 25, 30, 35, 40, 45, 50],
        toolbar: "#toolBarInOutStoreDetail",
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

        }
    });

    $("#btnSeleAll").click(function () {
        SeleAll();
    });

    $("#btnInverseSele").click(function () {
        InverseSele();
    });

    $("#btnConfirmRemoveStore").click(function () {
        ConfirmRemoveStore();
    });

    function SeleAll() {
        var rows = _$_datagrid.datagrid("getRows");
        for (var i = 0; i < rows.length; i++) {
            var m = _$_datagrid.datagrid("getRowIndex", rows[i]);
            if (rows[i].wbID == "0" && rows[i].swbID == "0") {
                _$_datagrid.datagrid("uncheckRow", i);
                _$_datagrid.datagrid("unselectRow", i);
            } else {
                _$_datagrid.datagrid("selectRow", m)
            }
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
                if (rows[i].wbID == "0" && rows[i].swbID == "0") {
                    _$_datagrid.datagrid("uncheckRow", i);
                    _$_datagrid.datagrid("unselectRow", i);
                } else {
                    _$_datagrid.datagrid("selectRow", m)
                }

            }
        }
    }

    function ConfirmRemoveStore() {
        reWriteMessagerConfirm("提示", "您确定需要将所选的分运单进行出库吗？",
                    function (ok) {
                        if (ok) {
                            var selects = _$_datagrid.datagrid("getSelections");
                            var ids = [];
                            for (var i = 0; i < selects.length; i++) {
                                ids.push(selects[i].Wbf_swbID);
                            }
                            if (selects.length == 0) {
                                $.messager.alert("提示", "<center>请选择需要进行出库的分运单</center>", "error");
                                return false;
                            }
                            $.ajax({
                                type: "POST",
                                url: PatchOutStoreURL + encodeURI(ids.join(',')),
                                data: "",
                                async: false,
                                cache: false,
                                beforeSend: function (XMLHttpRequest) {

                                },
                                success: function (msg) {
                                    var JSONMsg = eval("(" + msg + ")");
                                    if (JSONMsg.result.toLowerCase() == 'ok') {
                                        $.messager.alert('操作提示', JSONMsg.message, 'info');
                                        $("#btnInOutStoreDetailQuery").click();
                                    } else {
                                        $.messager.alert('操作提示', JSONMsg.message, 'error');
                                    }
                                },
                                complete: function (XMLHttpRequest, textStatus) {

                                },
                                error: function () {

                                }
                            });
                        } else {

                        }
                    }
                );
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:150px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "swbserialnum":
                    break;
                case "wbf_swbid":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "Wbf_swbID") {

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
