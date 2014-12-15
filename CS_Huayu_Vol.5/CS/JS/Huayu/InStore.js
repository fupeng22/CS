$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddCompany = $('#txtVoyage');
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    var PatchInStoreURL = "/Huayu_ExceptionStore/PatchInOutStore?iType=1&ids=";
    var PatchOutStoreURL = "/Huayu_ExceptionStore/PatchInOutStore?iType=3&ids=";

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL = "/Huayu_InStore/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val());

    $("#btnQuery").click(function () {
        QueryURL = "/Huayu_InStore/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val());
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
        $("#txtCode").val("");
        $("#txtSubWayBillCode").val("");
        _$_ddCompany.combobox("setValue", "---请选择---");
        $("#btnQuery").click();
    });

    $("#btnConfirmOutStore").click(function () {
        ConfirmPatchInOutStore("3");
    });

    $("#btnPrint").click(function () {
        PrintURL = "/Huayu_InStore/Print?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Huayu_InStore/Excel?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        sortName: 'operateDate',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        columns: [[
                    { field: 'cb', title: '', width: 100, checkbox: true
                    },
                    { field: 'wbSerialNum', title: '总运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbSerialNum', title: '分运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbCompany', title: '货代公司', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
					{ field: 'operateDate', title: '入库日期', width: 90, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbDescription_CHN', title: '货物中文名称', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbDescription_ENG', title: '货物英文名称', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNumber', title: '件数', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbWeight', title: '重量', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'Operator', title: '操作员', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'WbfID', title: '', hidden: true
                    }
				]],
        pagination: true,
        pageSize: 15,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
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
        onSortColumn: function (sort, order) {
            //_$_datagrid.datagrid("reload");
        }
    });

    function ConfirmPatchInOutStore(iType) {
        var selects = _$_datagrid.datagrid("getSelections");
        var ids = [];
        for (var i = 0; i < selects.length; i++) {
            ids.push(selects[i].WbfID);
        }
        if (selects.length == 0) {
            reWriteMessagerAlert("提示", "<center>请您先选择数据</center>", "error");
            return false;
        } else {
            var strIds = ids.join(',');
            switch (iType) {
                case "1":
                    reWriteMessagerConfirm("操作提示", "您确定需要将所选记录进行入仓吗？", function (ok) {
                        if (ok) {
                            PatchInOutStore(strIds, iType);
                        }
                    });
                    break;
                case "3":
                    reWriteMessagerConfirm("操作提示", "您确定需要将所选记录进行出仓吗？", function (ok) {
                        if (ok) {
                            PatchInOutStore(strIds, iType);
                        }
                    });
                    break;
                default:

            }
        }

    }

    function PatchInOutStore(ids, iType) {
        var PatchURL = "";
        switch (iType) {
            case "1":
                PatchURL = PatchInStoreURL;
                break;
            case "3":
                PatchURL = PatchOutStoreURL;
                break;
            default:
        }
        if (ids == undefined || ids == "") {
            var selects = _$_datagrid.datagrid("getSelections");
            var idsArr = [];
            for (var i = 0; i < selects.length; i++) {
                idsArr.push(selects[i].WbfID);
            }
            if (selects.length == 0) {
                reWriteMessagerAlert('操作提示', '请您先选择记录!', 'error');
                return false;
            }
            var ids = idsArr.join(',');
            if (ids == undefined || ids == "") {

            } else {
                _$_datagrid.datagrid("unselectAll");
                $.ajax({
                    type: "POST",
                    url: PatchURL + encodeURI(ids),
                    data: "",
                    async: false,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var JSONMsg = eval("(" + msg + ")");
                        if (JSONMsg.result.toLowerCase() == 'ok') {
                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');

                            _$_datagrid.datagrid("reload");
                            _$_datagrid.datagrid("unselectAll");
                        } else {
                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                            return false;
                        }
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });
            }
        } else {
            if (ids == undefined || ids == "") {
                reWriteMessagerAlert('操作提示', '请您先选择记录!', 'error');
                return false;
            } else {
                _$_datagrid.datagrid("unselectAll");
                $.ajax({
                    type: "POST",
                    url: PatchURL + encodeURI(ids),
                    data: "",
                    async: false,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var JSONMsg = eval("(" + msg + ")");
                        if (JSONMsg.result.toLowerCase() == 'ok') {
                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');

                            _$_datagrid.datagrid("reload");
                            _$_datagrid.datagrid("unselectAll");
                        } else {
                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                            return false;
                        }
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });
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
