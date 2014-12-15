$(function () {
    var _$_datagrid = $("#DG_SubWayBillDetail");
    var PrintURL = "";
    var _$_ddlSwbStatus = $("#ddlSwbStatus");
    var _$_ddlSwbReleaseFlag = $("#ddlSwbReleaseFlag");
    var _$_ddlSortingTimes = $("#ddlSortingTimes");
    var LoadStatusJSONUrl = "/CustomerMain/CreateStatusJSON";
    var LoadReleaseFlagURL = "/CustomerMain/CreateReleaseFlagJSON";
    var LoadSortJSONUrl = "/CustomerMain/CreateSortJSON";

    var RejectSubWayBillURL = "/ViewSubWayBillDetail/PatchRejectSubWayBill?strSwbIds=";
    var UnReleaseSubWayBillURL = "/ViewSubWayBillDetail/PatchUpdateSwbNeedCheck?strSwbIds=";

    _$_ddlSwbStatus.combotree({
        url: LoadStatusJSONUrl,
        valueField: 'id',
        textField: 'text',
        //readonly: $("#Detail_EnableSelectInOutStoreType").val() == "0" ? true : false,
        onLoadSuccess: function () {
            switch ($("#hid_swbStatus").val()) {
                case "":
                    _$_ddlSwbStatus.combotree("setValues", ["-99"]);
                    break;
                default:
                    _$_ddlSwbStatus.combotree("setValues", eval("[" + $("#hid_swbStatus").val() + "]"));
                    break;
            }
        }
    });

    _$_ddlSwbReleaseFlag.combotree({
        url: LoadReleaseFlagURL,
        valueField: 'id',
        textField: 'text',
        onLoadSuccess: function () {
            switch ($("#hid_swbReleaseFlag").val()) {
                case "":
                    _$_ddlSwbReleaseFlag.combotree("setValues", ["-99"]);
                    break;
                default:
                    _$_ddlSwbReleaseFlag.combotree("setValues", eval("[" + $("#hid_swbReleaseFlag").val() + "]"));
                    break;
            }

        }
    });

    _$_ddlSortingTimes.combobox({
        url: LoadSortJSONUrl,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddlSortingTimes.combobox("setValue", "---请选择---");

    var QueryURL = "/ViewSubWayBillDetail/GetData?Detail_wbSerialNum=" + encodeURI($("#txtCode_Detail").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_Detail").val()) + "&txtswbDescription_CHN=" + encodeURI($("#txtswbDescription_CHN").val()) + "&txtswbDescription_ENG=" + encodeURI($("#txtswbDescription_ENG").val()) + "&txtSwbStatus=" + encodeURI(_$_ddlSwbStatus.combotree("getValues").join(',')) + "&ddlSortingTimes=" + encodeURI(_$_ddlSortingTimes.combobox("getValue")) + "&txtSwbReleaseFlag=" + encodeURI(_$_ddlSwbReleaseFlag.combotree("getValues").join(',')) + "&Detail_wbImportType=" + encodeURI($("#hid_wbImportType").val());

    $("#btnDetailQuery").click(function () {
        QueryURL = "/ViewSubWayBillDetail/GetData?Detail_wbSerialNum=" + encodeURI($("#txtCode_Detail").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_Detail").val()) + "&txtswbDescription_CHN=" + encodeURI($("#txtswbDescription_CHN").val()) + "&txtswbDescription_ENG=" + encodeURI($("#txtswbDescription_ENG").val()) + "&txtSwbStatus=" + encodeURI(_$_ddlSwbStatus.combotree("getValues").join(',')) + "&ddlSortingTimes=" + encodeURI(_$_ddlSortingTimes.combobox("getValue")) + "&txtSwbReleaseFlag=" + encodeURI(_$_ddlSwbReleaseFlag.combotree("getValues").join(',')) + "&Detail_wbImportType=" + encodeURI($("#hid_wbImportType").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    setTimeout(function () {
        $("#btnDetailQuery").click();
    },100);

    $("#btnPrint_dlg").click(function () {
        PrintURL = "/ViewSubWayBillDetail/Print?Detail_wbSerialNum=" + encodeURI($("#txtCode_Detail").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_Detail").val()) + "&txtswbDescription_CHN=" + encodeURI($("#txtswbDescription_CHN").val()) + "&txtswbDescription_ENG=" + encodeURI($("#txtswbDescription_ENG").val()) + "&txtSwbStatus=" + encodeURI(_$_ddlSwbStatus.combotree("getValues").join(',')) + "&ddlSortingTimes=" + encodeURI(_$_ddlSortingTimes.combobox("getValue")) + "&txtSwbReleaseFlag=" + encodeURI(_$_ddlSwbReleaseFlag.combotree("getValues").join(',')) + "&Detail_wbImportType=" + encodeURI($("#hid_wbImportType").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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


    $("#btnExcel_dlg").click(function () {
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

        PrintURL = "/ViewSubWayBillDetail/Excel?Detail_wbSerialNum=" + encodeURI($("#txtCode_Detail").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_Detail").val()) + "&txtswbDescription_CHN=" + encodeURI($("#txtswbDescription_CHN").val()) + "&txtswbDescription_ENG=" + encodeURI($("#txtswbDescription_ENG").val()) + "&txtSwbStatus=" + encodeURI(_$_ddlSwbStatus.combotree("getValues").join(',')) + "&ddlSortingTimes=" + encodeURI(_$_ddlSortingTimes.combobox("getValue")) + "&txtSwbReleaseFlag=" + encodeURI(_$_ddlSwbReleaseFlag.combotree("getValues").join(',')) + "&Detail_wbImportType=" + encodeURI($("#hid_wbImportType").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        sortName: 'swbID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        columns: [[
                    { field: 'cb', title: '', checkbox: true
                    },
                    { field: 'wbSerialNum', title: '总运单号', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbCompany', title: '货代公司', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbStorageDate', title: '报关日期', width: 60, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
					{ field: 'swbSerialNum', title: '分运单号', width: 100, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'swbDescription_CHN', title: '货物中文名', width: 160, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbDescription_ENG', title: '货物英文名', width: 140, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNumber', title: '件数', width: 40, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbActualNumber', title: '实际扫描件数', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbWeight', title: '重量(公斤)', width: 70, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
					{ field: 'swbNeedCheck', title: '状态', width: 150, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'swbID', title: '主键', hidden: true, width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					}
				]],
        pagination: true,
        pageSize: 15,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
        toolbar: "#toolBarDetail",
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

    var hid_bEnableReject = $("#hid_bEnableReject").val();
    var hid_bEnableUnRelease = $("#hid_bEnableUnRelease").val();
    var hid_bEnableRelease = $("#hid_bEnableRelease").val();
    _$_datagrid.datagrid('hideColumn', "cb");
    $("#btnSeleAll").hide();
    $("#btnInverseSele").hide();
    $("#btnReleaseWayBill").hide();
    $("#btnUnReleaseWayBill").hide();
    $("#btnRejectWayBill").hide();
    $("#TipForRejectSubWayBill").hide();
    _$_ddlSwbStatus.combotree("disable");
    _$_ddlSwbReleaseFlag.combotree("disable");
    if ($("#hid_wbImportType").val() == "0") {
        _$_ddlSwbStatus.combotree("enable");
    } else if ($("#hid_wbImportType").val() == "1") {
        _$_ddlSwbReleaseFlag.combotree("enable");
    }

    if (hid_bEnableReject == "1" || hid_bEnableUnRelease == "1" || hid_bEnableRelease == "1") {
        _$_datagrid.datagrid('showColumn', "cb");
        $("#btnSeleAll").show();
        $("#btnInverseSele").show();
    }

    if (hid_bEnableReject == "1") {
        $("#btnRejectWayBill").show();
        $("#TipForRejectSubWayBill").show();
    }

    //if (hid_bEnableUnRelease == "1" && $("#hid_wbImportType").val() == "0") {
    if (hid_bEnableUnRelease == "1") {
        $("#btnUnReleaseWayBill").show();
    }

    //if (hid_bEnableRelease == "1" && $("#hid_wbImportType").val() == "0") {
    if (hid_bEnableRelease == "1" ) {
        $("#btnReleaseWayBill").show();
    }

    $("#btnSeleAll").click(function () {
        SeleAll();
    });

    $("#btnInverseSele").click(function () {
        InverseSele();
    });

    $("#btnRejectWayBill").click(function () {
        //RejectSubWayBill();
        UnReleaseSubWayBill(99);
    });

    $("#btnUnReleaseWayBill").click(function () {
        UnReleaseSubWayBill(3);
    });

    $("#btnReleaseWayBill").click(function () {
        UnReleaseSubWayBill(2);
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

    function RejectSubWayBill() {
        reWriteMessagerConfirm("提示", "您确定需要将所选的分运单进行退货吗？</br><font style='color:red;font-weight:bold'>(退货后将无法再次入库，请慎重选择)</font>",
                    function (ok) {
                        if (ok) {
                            var selects = _$_datagrid.datagrid("getSelections");
                            var ids = [];
                            for (var i = 0; i < selects.length; i++) {
                                ids.push(selects[i].swbID);
                            }
                            if (selects.length == 0) {
                                reWriteMessagerAlert("提示", "<center>请选择需要进行退货的分运单</center>", "error");
                                return false;
                            }
                            $.ajax({
                                type: "POST",
                                url: RejectSubWayBillURL + encodeURI(ids.join(',')),
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

    function UnReleaseSubWayBill(iNeedCheck) {
        //        reWriteMessagerConfirm("提示", "您确定需要将所选的分运单进行退货吗？</br><font style='color:red;font-weight:bold'>(退货后将无法再次入库，请慎重选择)</font>",
        //                    function (ok) {
        //                        if (ok) {
        //                           
        //                        } else {

        //                        }
        //                    }
        //                );
        var selects = _$_datagrid.datagrid("getSelections");
        var ids = [];
        for (var i = 0; i < selects.length; i++) {
            ids.push(selects[i].swbID);
        }
        if (selects.length == 0) {
            reWriteMessagerAlert("提示", "<center>请先选择分运单</center>", "error");
            return false;
        }
        $.ajax({
            type: "POST",
            url: UnReleaseSubWayBillURL + encodeURI(ids.join(',')) + "&iNeedCheck=" + iNeedCheck,
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
                }
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
        _$_datagrid.datagrid("reload");
        _$_datagrid.datagrid("unselectAll");
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:150px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "swbserialnum":
                    break;
                case "swbid":
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
                if ($(item.text).attr("id") == "swbSerialNum") {

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
