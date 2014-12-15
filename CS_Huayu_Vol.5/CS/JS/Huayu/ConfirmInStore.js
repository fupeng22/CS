$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddCompany = $('#txtVoyage');
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    var ValidateInfoURL = "/Huayu_ConfirmInStore/ValidateInfo?wbSerialNum=";

    var DeleForeStoreInfo = "/Huayu_ConfirmInStore/DeleForeStore?swbIds=";

    var PatchInStoreURL = "/Huayu_ConfirmInStore/PatchInOutStore?iType=1&ids_wbSerialNum=";
    var PatchOutStoreURL = "/Huayu_ConfirmInStore/PatchInOutStore?iType=3&ids_wbSerialNum=";

    var CreateURL = "/Huayu_ConfirmInStore/Create";
    var CreateDlg = null;
    var CreateDlgForm = null;

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    $('#txtNeedCheckStatus').combotree('loadData', [
    {
        id: -99,
        text: '---请选择(可多选)---'
    },
    {
        id: 0,
        text: '放行'
    },
    {
        id: 1,
        text: '等待预检'
    },
    {
        id: 2,
        text: '查验放行'
    },
    {
        id: 3,
        text: '查验扣留'
    },
    {
        id: 4,
        text: '查验待处理'
    },
    {
        id: 99,
        text: '退货'
    }]);

    $('#txtNeedCheckStatus').combotree("setValue", "-99");
    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL = "/Huayu_ConfirmInStore/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&NeedCheck=" + encodeURI($('#txtNeedCheckStatus').combotree("getValues").join(','));

    $("#btnQuery").click(function () {
        QueryURL = "/Huayu_ConfirmInStore/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&NeedCheck=" + encodeURI($('#txtNeedCheckStatus').combotree("getValues").join(','));
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 20); //延迟100毫秒执行，时间可以更短
    });

    $("#btnReset").click(function () {
        $("#txtBeginD").val("");
        $("#txtEndD").val("");
        $("#txtCode").val("");
        $("#txtSubWayBillCode").val("");
        $('#txtNeedCheckStatus').combotree("setValue", "-99");
        _$_ddCompany.combobox("setValue", "---请选择---");
        $("#btnQuery").click();
    });

    $("#btnConfirmInStore").click(function () {
        ConfirmPatchInOutStore("1");
    });

    $("#btnAddForeStoreInfo").click(function () {
        Add();
    });

    $("#btnDeleForeStoreInfo").click(function () {
        Delete();
    });

    $("#btnPrint").click(function () {
        PrintURL = "/Huayu_ConfirmInStore/Print?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&NeedCheck=" + encodeURI($('#txtNeedCheckStatus').combotree("getValues").join(',')) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Huayu_ConfirmInStore/Excel?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&NeedCheck=" + encodeURI($('#txtNeedCheckStatus').combotree("getValues").join(',')) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        sortName: 'wbStorageDate',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        columns: [[
                    { field: 'cb', title: '', width: 100, checkbox: true
                    },
					{ field: 'wbStorageDate', title: '报关日期', width: 120, sortable: true,
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
                    { field: 'swbSerialNum', title: '分运单号', width: 120, sortable: true,
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
                    { field: 'swbNumber', title: '件数', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbWeight', title: '重量', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNeedCheckDescription', title: '预检状态', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
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

    function Add() {
        _$_datagrid.datagrid("unselectAll");
        CreateDlg = $('#dlg_Create').dialog({
            buttons: [{
                text: '保 存',
                iconCls: 'icon-ok',
                handler: function () {
                    var ddlwbSerialNum = CreateDlg.find('#ddlwbSerialNum').combobox("getValue");
                    var txtSwbSerialNum = CreateDlg.find('#txtSwbSerialNum').val();
                    var ddlSwbCustomsCategory = CreateDlg.find('#ddlSwbCustomsCategory').combogrid("getValue");

                    if (ddlwbSerialNum == "" || ddlwbSerialNum == "" || ddlSwbCustomsCategory == "---请选择---") {
                        reWriteMessagerAlert('操作提示', '请填写完整信息<br/>(总运单号、分运单号、报关类别必填)', "error");
                        return false;
                    }

                    var bOK = false;
                    $.ajax({
                        type: "POST",
                        url: ValidateInfoURL + encodeURI(ddlwbSerialNum) + "&swbSerialNum=" + encodeURI(txtSwbSerialNum),
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

                    if (bOK == false) {
                        return false;
                    } else {
                        CreateDlgForm = CreateDlg.find('form');
                        CreateDlgForm.form('submit', {
                            url: CreateDlgForm.url,
                            onSubmit: function () {
                                return $(this).form('validate');
                            },
                            success: function () {
                                reWriteMessagerAlert('提示', '成功', "info");
                                CreateDlg.dialog('close');
                                _$_datagrid.datagrid("reload");
                                _$_datagrid.datagrid("unselectAll");
                            }
                        });
                    }
                }
            }, {
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    CreateDlg.dialog('close');
                }
            }],
            title: '补录预入库单信息',
            href: CreateURL,
            modal: true,
            resizable: true,
            cache: false,
            closed: true,
            left: 50,
            top: 30,
            width: 800,
            height: 260
        });

        $('#dlg_Create').dialog("open");
    }

    function ConfirmPatchInOutStore(iType) {
        var selects = _$_datagrid.datagrid("getSelections");
        var ids_wbSerialNum = [];
        var ids_swbSerialNum = [];
        for (var i = 0; i < selects.length; i++) {
            ids_wbSerialNum.push(selects[i].wbSerialNum);
            ids_swbSerialNum.push(selects[i].swbSerialNum);
        }
        if (selects.length == 0) {
            reWriteMessagerAlert("提示", "<center>请您先选择数据</center>", "error");
            return false;
        } else {
            var strIds_wbSerialNum = ids_wbSerialNum.join(',');
            var strIds_swbSerialNum = ids_swbSerialNum.join(',');
            switch (iType) {
                case "1":
                    reWriteMessagerConfirm("操作提示", "您确定需要将所选记录进行入仓吗？", function (ok) {
                        if (ok) {
                            PatchInOutStore(strIds_wbSerialNum, strIds_swbSerialNum, iType);
                        }
                    });
                    break;
                case "3":
                    reWriteMessagerConfirm("操作提示", "您确定需要将所选记录进行出仓吗？", function (ok) {
                        if (ok) {
                            PatchInOutStore(strIds_wbSerialNum, strIds_swbSerialNum, iType);
                        }
                    });
                    break;
                default:

            }
        }

    }

    function PatchInOutStore(ids_wbSerialNum, ids_swbSerialNum, iType) {
        var PatchURL = "";
        switch (iType) {
            case "1":
                PatchURL = PatchInStoreURL + encodeURI(ids_wbSerialNum) + "&ids_swbSerialNum=" + encodeURI(ids_swbSerialNum);
                break;
            case "3":
                PatchURL = PatchOutStoreURL + encodeURI(ids_wbSerialNum) + "&ids_swbSerialNum=" + encodeURI(ids_swbSerialNum);
                break;
            default:

        }
        if (ids_wbSerialNum == undefined || ids_swbSerialNum == undefined || ids_wbSerialNum == "" || ids_swbSerialNum == "") {

        } else {
            _$_datagrid.datagrid("unselectAll");
            $.ajax({
                type: "POST",
                url: PatchURL,
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

    function Delete() {
        reWriteMessagerConfirm("提示", "您确定需要删除所选的预入库信息吗？</br><font style='color:red;font-weight:bold'>(删除后无法恢复,请谨慎选择)</font>？",
                    function (ok) {
                        if (ok) {
                            var selects = _$_datagrid.datagrid("getSelections");
                            var ids_swbID = [];
                            for (var i = 0; i < selects.length; i++) {
                                ids_swbID.push(selects[i].swbID);
                            }

                            if (selects.length == 0) {
                                reWriteMessagerAlert("提示", "<center>请选择需要删除的预入库信息</center>", "error");
                                //reWriteMessagerAlert("提示", "<center>请您先选择数据</center>", "error");
                                return false;
                            }
                            var strIds_swbID = ids_swbID.join(',');
                            $.ajax({
                                type: "POST",
                                url: DeleForeStoreInfo + encodeURI(strIds_swbID),
                                data: "",
                                async: false,
                                cache: false,
                                beforeSend: function (XMLHttpRequest) {

                                },
                                success: function (msg) {
                                    var JSONMsg = eval("(" + msg + ")");
                                    if (JSONMsg.result.toLowerCase() == 'ok') {
                                        reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
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

                            _$_datagrid.datagrid("reload");
                            _$_datagrid.datagrid("unselectAll");
                        } else {

                        }
                    }
                );
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
