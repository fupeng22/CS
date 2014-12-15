$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddCompany = $('#txtVoyage');
    var _$_ddlExceptionType = $("#ddlExceptionType");
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    var ExceptionHandleURL = "/Huayu_ExceptionStore/ExceptionHandle?ids=";

    var SubmitExceptionHandleURL = "/Huayu_ExceptionStore/UpdateHandleStatus?ids=";

    var PatchInStoreURL = "/Huayu_ExceptionStore/PatchInOutStore?iType=1&ids=";
    var PatchOutStoreURL = "/Huayu_ExceptionStore/PatchInOutStore?iType=3&ids=";

    var ExceptionHandleDlg = null;
    var ExceptionHandleDlgForm = null;

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
    var QueryURL = "";// "/Huayu_ExceptionStore/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtExceptionBeginD=" + encodeURI($("#txtExceptionBeginD").val()) + "&txtExceptionEndD=" + encodeURI($("#txtExceptionEndD").val()) + "&txtExceptionType=" + encodeURI(_$_ddlExceptionType.combotree("getValues").join(',')) + "&txtExceptionStatus=" + encodeURI($("#txtExceptionStatus").combobox("getValue"));

    //    $('#txtExceptionType').combobox({
    //        data: [{ "value": "---请选择---", "id": "---请选择---" }, { "text": "异常入库", "id": "2" }, { "text": "异常出库", "id": "4"}],
    //        valueField: 'id',
    //        textField: 'text',
    //        editable: false,
    //        panelHeight: null
    //    });

    //    $('#txtExceptionStatus').combobox({
    //        data: [{ "value": "---请选择---", "id": "---请选择---" }, { "text": "待处理", "id": "0" }, { "text": "正在处理", "id": "1" }, { "text": "已处理", "id": "2"}],
    //        valueField: 'id',
    //        textField: 'text',
    //        editable: false,
    //        panelHeight: null
    //    });

    $("#btnQuery").click(function () {
        QueryURL = "/Huayu_ExceptionStore/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtExceptionBeginD=" + encodeURI($("#txtExceptionBeginD").val()) + "&txtExceptionEndD=" + encodeURI($("#txtExceptionEndD").val()) + "&txtExceptionType=" + encodeURI(_$_ddlExceptionType.combotree("getValues").join(',')) + "&txtExceptionStatus=" + encodeURI($("#txtExceptionStatus").combobox("getValue"));
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnHandleExceptionForSele").click(function () {
        ExceptionHandle();
    });

    $("#btnHandleExceptionForImport").click(function () {
        ExceptionHandle();
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

    $("#btnSeleAll").click(function () {
        SeleAll();
    });

    $("#btnInverseSele").click(function () {
        InverseSele();
    });

    $("#btnInStore").click(function () {
        ConfirmPatchInOutStore("1");
    });

    $("#btnOutStore").click(function () {
        ConfirmPatchInOutStore("3");
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
        sortName: 'WbfID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        columns: [[
                    { field: 'cb', title: '', width: 100, checkbox: true
                    },
                    { field: 'HandleException', title: '操作', width: 80, sortable: true, align: "center",
                        formatter: function (value, rowData, rowIndex) {
                            //return "<input type='button' class='handle_ReInStore' value='入库' wbfID='" + rowData.WbfID + "'/>" + "<input type='button' class='handle_ReOutStore' value='出库' wbfID='" + rowData.WbfID + "'/>";
                            return "<a href='#' class='handle_ReInStore' Wbf_wbID='" + rowData.wbID + "' Wbf_swbID='" + rowData.swbID + "' wbfID='" + rowData.WbfID + "'>入仓</a>" + "&nbsp;&nbsp;" + "<a href='#' class='handle_ReOutStore'  Wbf_wbID='" + rowData.wbID + "' Wbf_swbID='" + rowData.swbID + "'  wbfID='" + rowData.WbfID + "'>出仓</a>";
                        }
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
        onCheck: function (rowIndex, rowData) {
            if (rowData.wbID == "0" && rowData.swbID == "0") {
                _$_datagrid.datagrid("uncheckRow", rowIndex);
                _$_datagrid.datagrid("unselectRow", rowIndex);
            }
        },
        onSelect: function (rowIndex, rowData) {
            if (rowData.wbID == "0" && rowData.swbID == "0") {
                _$_datagrid.datagrid("uncheckRow", rowIndex);
                _$_datagrid.datagrid("unselectRow", rowIndex);
            }
        },
        onCheckAll: function (rows) {
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].wbID == "0" && rows[i].swbID == "0") {
                    _$_datagrid.datagrid("uncheckRow", i);
                    _$_datagrid.datagrid("unselectRow", i);
                }
            }
        },
        onSelectAll: function (rows) {
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].wbID == "0" && rows[i].swbID == "0") {
                    _$_datagrid.datagrid("uncheckRow", i);
                    _$_datagrid.datagrid("unselectRow", i);
                }
            }
        },
        onRowContextMenu: function (e, rowIndex, rowData) {
            e.preventDefault();
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);

            var cmenu = $('<div id="cmenu" style="width:200px;"></div>').appendTo('body');
            //            $('<div  id="mnuHandleExceptionForImport" iconCls="icon-exception_handle"/>').html("处理异常记录(通过导入)").appendTo(cmenu);
            $('<div  id="mnuSeleAll" iconCls="icon-seleall"/>').html("全选").appendTo(cmenu);
            $('<div  id="mnuInverseSele" iconCls="icon-inversesele"/>').html("反选").appendTo(cmenu);
            $('<div  id="mnuInStore" iconCls="icon-instore"/>').html("入仓").appendTo(cmenu);
            $('<div  id="mnuOutStore" iconCls="icon-outstore"/>').html("出仓").appendTo(cmenu);
            $('<div  id="mnuHandleExceptionForSingleSele" iconCls="icon-exception_handle"/>').html("处理此行异常记录").appendTo(cmenu);
            $('<div  id="mnuPrint" iconCls="icon-print"/>').html("打印").appendTo(cmenu);
            $('<div  id="mnuExcel" iconCls="icon-excel"/>').html("导出").appendTo(cmenu);
            cmenu.menu({
                onClick: function (item) {
                    cmenu.remove();
                    switch (item.id.toLowerCase()) {
                        case "mnuhandleexceptionforimport":
                            ExceptionHandle();
                            break;
                        case "mnuhandleexceptionforsinglesele":
                            ExceptionHandle();
                            break;
                        case "mnuseleall":
                            SeleAll();
                            break;
                        case "mnuinversesele":
                            InverseSele();
                            break;
                        case "mnuinstore":
                            ConfirmPatchInOutStore("1");
                            break;
                        case "mnuoutstore":
                            ConfirmPatchInOutStore("3");
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
        onClickRow: function (rowIndex, rowData) {
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);
            ExceptionHandle(rowData.WbfID);
        },
        onLoadSuccess: function (data) {

            var allHandleReInStoreBtn = $(".handle_ReInStore");
            var allHandleReOutStoreBtn = $(".handle_ReOutStore");
            $.each(allHandleReInStoreBtn, function (i, item) {
                var wbID = $(item).attr("Wbf_wbID");
                var swbID = $(item).attr("Wbf_swbID");
                if (wbID == "0" && swbID == "0") {
                    $(item).hide();
                } else {
                    $(item).show();
                    $(item).click(function () {
                        reWriteMessagerConfirm("操作提示", "您确定需要将此异常记录进行入仓操作吗?<br/>(入仓后只可在【历史异常件处理记录】中查看此数据之前的异常信息)", function (ok) {
                            if (ok) {
                                PatchInOutStore($(item).attr("wbfID"), "1");
                            }
                        });

                    });
                }

            });

            $.each(allHandleReOutStoreBtn, function (i, item) {
                var wbID = $(item).attr("Wbf_wbID");
                var swbID = $(item).attr("Wbf_swbID");
                if (wbID == "0" && swbID == "0") {
                    $(item).hide();
                } else {
                    $(item).show();
                    $(item).click(function () {
                        reWriteMessagerConfirm("操作提示", "您确定需要将此异常记录进行出仓操作吗?<br/>(出仓后只可在【历史异常件处理记录】中查看此数据之前的异常信息)", function (ok) {
                            if (ok) {
                                PatchInOutStore($(item).attr("wbfID"), "3");
                            }
                        });
                    });
                }

            });
        }
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

    function ExceptionHandle(ids) {
        if (ids == undefined || ids == "") {
            var selects = _$_datagrid.datagrid("getSelections");
            var idsArr = [];
            for (var i = 0; i < selects.length; i++) {
                idsArr.push(selects[i].WbfID);
            }
            if (selects.length == 0) {
                //$.messager.alert('操作提示', '请您先选择需要处理的异常记录!', 'error');
                reWriteMessagerAlert('操作提示', '请您先选择需要处理的异常记录!', 'error');
                return false;
            }
            var ids = idsArr.join(',');
            if (ids == undefined || ids == "") {

            } else {
                _$_datagrid.datagrid("unselectAll");
                ExceptionHandleDlg = $('#dlg_ExceptionHandler').dialog({
                    buttons: [{
                        text: '提 交',
                        iconCls: 'icon-ok',
                        handler: function () {
                            var idsForHandle = ExceptionHandleDlg.find('form #hidIds').val();
                            var ExceptionStatus = ExceptionHandleDlg.find('form #ddlExceptionStatus').combobox("getValue");
                            var HandleDescription = ExceptionHandleDlg.find('form #txtExceptionDescription').val();
                            var strOperator = $("#hidSessionUserName").val();
                            if (ExceptionStatus == "---请选择---") {
                                //$.messager.alert('操作提示', '请您选择处理状态!', 'error');
                                reWriteMessagerAlert('操作提示', '请您选择处理状态!', 'error');
                                return false;
                            } else {
                                $.ajax({
                                    type: "GET",
                                    url: SubmitExceptionHandleURL + encodeURI(idsForHandle) + "&ExceptionStatus=" + encodeURI(ExceptionStatus) + "&HandleDescription=" + encodeURI(HandleDescription) + "&strOperator=" + encodeURI(strOperator),
                                    data: "",
                                    async: false,
                                    cache: false,
                                    beforeSend: function (XMLHttpRequest) {

                                    },
                                    success: function (msg) {
                                        var JSONMsg = eval("(" + msg + ")");
                                        if (JSONMsg.result.toLowerCase() == 'ok') {
                                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                            ExceptionHandleDlg.dialog('close');
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
                    }, {
                        text: '关 闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            ExceptionHandleDlg.dialog('close');
                        }
                    }],
                    title: '处理异常记录',
                    href: ExceptionHandleURL + ids,
                    modal: true,
                    resizable: true,
                    cache: false,
                    closed: false,
                    left: 50,
                    top: 30,
                    width: 1000,
                    height: 320,
                    closed: true
                });

                $('#dlg_ExceptionHandler').dialog("open");
            }
        } else {
            if (ids == undefined || ids == "") {
                //$.messager.alert('操作提示', '请您先选择需要处理的异常记录!', 'error');
                reWriteMessagerAlert('操作提示', '请您先选择需要处理的异常记录!', 'error');
                return false;
            } else {
                _$_datagrid.datagrid("unselectAll");
                ExceptionHandleDlg = $('#dlg_ExceptionHandler').dialog({
                    buttons: [{
                        text: '提 交',
                        iconCls: 'icon-ok',
                        handler: function () {
                            var idsForHandle = ExceptionHandleDlg.find('form #hidIds').val();
                            var ExceptionStatus = ExceptionHandleDlg.find('form #ddlExceptionStatus').combobox("getValue");
                            var HandleDescription = ExceptionHandleDlg.find('form #txtExceptionDescription').val();
                            var strOperator = $("#hidSessionUserName").val();
                            if (ExceptionStatus == "---请选择---") {
                                //$.messager.alert('操作提示', '请您选择处理状态!', 'error');
                                reWriteMessagerAlert('操作提示', '请您选择处理状态!', 'error');
                                return false;
                            } else {
                                $.ajax({
                                    type: "GET",
                                    url: SubmitExceptionHandleURL + encodeURI(idsForHandle) + "&ExceptionStatus=" + encodeURI(ExceptionStatus) + "&HandleDescription=" + encodeURI(HandleDescription) + "&strOperator=" + encodeURI(strOperator),
                                    data: "",
                                    async: false,
                                    cache: false,
                                    beforeSend: function (XMLHttpRequest) {

                                    },
                                    success: function (msg) {
                                        var JSONMsg = eval("(" + msg + ")");
                                        if (JSONMsg.result.toLowerCase() == 'ok') {
                                            //$.messager.alert('操作提示', JSONMsg.message, 'info');
                                            reWriteMessagerAlert('操作提示', JSONMsg.message, 'info');
                                            ExceptionHandleDlg.dialog('close');
                                            _$_datagrid.datagrid("reload");
                                            _$_datagrid.datagrid("unselectAll");
                                        } else {
                                            //$.messager.alert('操作提示', JSONMsg.message, 'error');
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
                    }, {
                        text: '关 闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            ExceptionHandleDlg.dialog('close');
                        }
                    }],
                    title: '处理异常记录',
                    href: ExceptionHandleURL + ids,
                    modal: true,
                    resizable: true,
                    cache: false,
                    closed: false,
                    left: 50,
                    top: 30,
                    width: 1000,
                    height: 320,
                    closed: true
                });

                $('#dlg_ExceptionHandler').dialog("open");
            }
        }

    }

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
                    reWriteMessagerConfirm("操作提示", "您确定需要将此异常记录进行入仓操作吗?<br/>(入仓后只可在【历史异常件处理记录】中查看此数据之前的异常信息)", function (ok) {
                        if (ok) {
                            PatchInOutStore(strIds, iType);
                        }
                    });
                    break;
                case "3":
                    reWriteMessagerConfirm("操作提示", "您确定需要将此异常记录进行出仓操作吗?<br/>(出仓后只可在【历史异常件处理记录】中查看此数据之前的异常信息)", function (ok) {
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
                reWriteMessagerAlert('操作提示', '请您先选择需要处理的记录!', 'error');
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
                reWriteMessagerAlert('操作提示', '请您先选择需要处理的记录!', 'error');
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

    function Print() {
        PrintURL = "/Huayu_ExceptionStore/Print?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtExceptionBeginD=" + encodeURI($("#txtExceptionBeginD").val()) + "&txtExceptionEndD=" + encodeURI($("#txtExceptionEndD").val()) + "&txtExceptionType=" + encodeURI(_$_ddlExceptionType.combotree("getValues").join(',')) + "&txtExceptionStatus=" + encodeURI($("#txtExceptionStatus").combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Huayu_ExceptionStore/Excel?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtVoyage=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtSubWayBillCode=" + encodeURI($("#txtSubWayBillCode").val()) + "&txtExceptionBeginD=" + encodeURI($("#txtExceptionBeginD").val()) + "&txtExceptionEndD=" + encodeURI($("#txtExceptionEndD").val()) + "&txtExceptionType=" + encodeURI(_$_ddlExceptionType.combotree("getValues").join(',')) + "&txtExceptionStatus=" + encodeURI($("#txtExceptionStatus").combobox("getValue")) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    }
});
