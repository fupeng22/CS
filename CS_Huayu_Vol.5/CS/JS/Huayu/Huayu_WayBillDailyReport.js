$(function () {
    var _$_datagrid = $("#DG_WayBillDailyReportResult");
    var _$_sele_EmailReciever = $("#sele_EmailReciever");

    var PrintURL = "";
    var QueryURL = "/Huayu_WayBillDailyReport/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtWbrCode=" + encodeURI($("#txtWbrCode").val());

    var SendEmail_PDF_Url = "";
    var SendEmail_Excel_Url = "";

    $("#btnQuery").click(function () {
        QueryURL = "/Huayu_WayBillDailyReport/GetData?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtWbrCode=" + encodeURI($("#txtWbrCode").val());
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
        $("#btnQuery").click();
    });

    $("#btnPrint").click(function () {
        PrintURL = "/Huayu_WayBillDailyReport/Print?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtWbrCode=" + encodeURI($("#txtWbrCode").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Huayu_WayBillDailyReport/Excel?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtWbrCode=" + encodeURI($("#txtWbrCode").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    });

    $("#btnEmail_PDF").click(function () {
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            var seleReciever = _$_sele_EmailReciever.combobox("getValue");
            if (seleReciever == "") {
                reWriteMessagerAlert("提示", "请填写或选择邮件接收人邮箱地址", "error");
                return false;
            }

            SendEmail_PDF_Url = "/Huayu_WayBillDailyReport/SendMail_PDF?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtWbrCode=" + encodeURI($("#txtWbrCode").val()) + "&seleReciever=" + encodeURI(seleReciever) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
            $.ajax({
                type: "POST",
                url: SendEmail_PDF_Url,
                data: "",
                async: false,
                cache: false,
                beforeSend: function (XMLHttpRequest) {
                    var win = $.messager.progress({
                        title: '请稍等',
                        msg: '正在发送邮件，请稍等……'
                    });
                },
                success: function (msg) {
                    $.messager.progress('close');
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

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可打印", "error");
            return false;
        }
    });


    $("#btnEmail_Excel").click(function () {
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

        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            var seleReciever = _$_sele_EmailReciever.combobox("getValue");
            if (seleReciever == "") {
                reWriteMessagerAlert("提示", "请填写或选择邮件接收人邮箱地址", "error");
                return false;
            }
            SendEmail_Excel_Url = "/Huayu_WayBillDailyReport/SendMail_Excel?txtBeginD=" + encodeURI($("#txtBeginD").val()) + "&txtEndD=" + encodeURI($("#txtEndD").val()) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&txtWbrCode=" + encodeURI($("#txtWbrCode").val()) + "&seleReciever=" + encodeURI(seleReciever) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
            $.ajax({
                type: "POST",
                url: SendEmail_Excel_Url,
                data: "",
                async: false,
                cache: false,
                beforeSend: function (XMLHttpRequest) {
                    var win = $.messager.progress({
                        title: '请稍等',
                        msg: '正在发送邮件，请稍等……'
                    });
                },
                success: function (msg) {
                    $.messager.progress('close');
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
					{ field: 'wbrCode', title: '放行单号', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'CustomsCategory', title: '业务类型', width: 80, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'wbVoyage', title: '航班号', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbSerialNum', title: '空运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbSRport', title: '始发地/目的地', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'InStoreDate', title: '入库(交货)日期', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'OutStoreDate', title: '出库(提货)日期', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalNumber', title: '件数', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalWeight', title: '毛重', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'WayBillActualWeight', title: '计费重量', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'OperateFee', title: '进出库操作费', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'PickGoodsFee', title: '提货费', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'KeepGoodsFee', title: '仓储费', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ShiftGoodsFee', title: '移库费', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'RejectGoodsFee', title: '退运费', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CollectionKeepGoodsFee', title: '代收仓储费', width: 90, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ActualPay', title: '收费金额', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'PayMethod', title: '结算方式', width: 60, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'Receipt', title: '应付单位', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ShouldPayUnit', title: '应付单价', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'shouldPay', title: '应付金额', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ReceptMethod', title: '直航/转关', width: 60, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbCompany', title: '客户名', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'SalesMan', title: '业务员', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'mMemo', title: '备注', width: 150, sortable: true,
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
            _$_datagrid.datagrid("reload");
        }
    });

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
