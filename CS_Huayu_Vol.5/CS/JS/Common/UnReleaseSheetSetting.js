$(function () {
    var _$_datagrid = $("#DG_SubWayBillResult");

    var PrintURL = "";
    var QueryURL = "/UnReleaseSheetSetting/GetData?swbNeedCheck=3&wbID=" + encodeURI($("#wbID_ForPrint").val());

    var SendEmail_PDF_Url = "";
    var SendEmail_Excel_Url = "";

    var Manul_ComputeFeeURL = "/UnReleaseSheetSetting/Manul_ComputeUnReleaseInfo?customCategory=";

    //    $('#ddlReceiptMode_ForSetting').combobox({
    //        data: [{ "text": "直航", "id": "0.5" }, { "text": "转关", "id": "0.7"}],
    //        valueField: 'id',
    //        textField: 'text',
    //        editable: false,
    //        panelHeight: null
    //    });

    $('#ddlPayMode_ForSetting').combobox({
        data: [{ "text": "现金", "id": "现金" }, { "text": "月结", "id": "月结" }, { "text": "欠条", "id": "欠条"}],
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    //$('#ddlReceiptMode_ForSetting').combobox("setValue", '0.5');
    $('#ddlPayMode_ForSetting').combobox("setValue", "现金");

    var SaveDialyReportURL = "/UnReleaseSheetSetting/SaveSaleInfo?strSwbSerialNums=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForPrint").val()) + "&hid_CustomCategory_ForSetting=" + encodeURI($("#hid_CustomCategory_ForSetting").val()) + "&wbID_ForPrint=" + encodeURI($("#wbID_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#PickGoodsDate_ForSetting").val()) + "&wbActualWeight_ForPrint=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()) + "&OperateFee_ForSetting=" + "0.00" + "&PickGoodsFee_ForSetting=" + "0.00" + "&KeepGoodsFee_ForSetting=" + encodeURI($("#EverUnReleaseFee_ForSetting").val()) + "&ShiftGoodsFee_ForSetting=" + "0.00" + "&CollectionFee_ForSetting=" + "0.00" + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&ShouldPayUnit_ForSetting=" + "" + "&shouldPay_ForSetting=" + "0.00" + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val()) + "&ddlReceiptMode_ForSetting=" + "" + "&Receipt_ForSetting=" + "";

    $("#btnPrePrint").click(function () {
        if ($("#currentReleaseSubWayBill").val() == "") {
            reWriteMessagerAlert('操作提示', "没有填写需要放行的放行单不允许导出或打印", 'error');
            return false;
        }

        PrintURL = "/UnReleaseSheetSetting/Print?iPrintType=0&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForPrint").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#PickGoodsDate_ForSetting").val()) + "&EverUnReleaseNum_ForSetting=" + encodeURI($("#EverUnReleaseNum_ForSetting").val()) + "&IntervalDays_ForSetting=" + encodeURI($("#IntervalDays_ForSetting").val()) + "&EverUnReleaseWeight_ForSetting=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()) + "&EverUnReleaseFee_ForSetting=" + encodeURI($("#EverUnReleaseFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue"));
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
    });

    $("#btnConfirmPrint").click(function () {
        if ($("#currentReleaseSubWayBill").val() == "") {
            reWriteMessagerAlert('操作提示', "没有填写需要放行的放行单不允许导出或打印", 'error');
            return false;
        }
        reWriteMessagerConfirm("提示", "确定现在就打印吗？</br><font style='color:red;font-weight:bold'>打印成功后，本次打印信息将进入该日销售日报表</font>",
                    function (ok) {
                        var bOK = false;
                        SendEmail_PDF_Url = "/UnReleaseSheetSetting/SendMail_PDF?iPrintType=1&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForPrint").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#PickGoodsDate_ForSetting").val()) + "&EverUnReleaseNum_ForSetting=" + encodeURI($("#EverUnReleaseNum_ForSetting").val()) + "&IntervalDays_ForSetting=" + encodeURI($("#IntervalDays_ForSetting").val()) + "&EverUnReleaseWeight_ForSetting=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()) + "&EverUnReleaseFee_ForSetting=" + encodeURI($("#EverUnReleaseFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue"));
                        $.ajax({
                            type: "POST",
                            url: SendEmail_PDF_Url,
                            data: "",
                            async: false,
                            cache: false,
                            beforeSend: function (XMLHttpRequest) {
                                $("#lblTip").html("<font style='color:blue;font-weight:bold'>正在验证并发送邮件……</font>");
                            },
                            success: function (msg) {
                                $("#lblTip").html("");
                                var JSONMsg = eval("(" + msg + ")");
                                if (JSONMsg.result.toLowerCase() == 'ok') {
                                    bOK = true;
                                } else {
                                    //reWriteMessagerAlert('操作提示', JSONMsg.message,'error');
                                    //alert(JSONMsg.message);
                                    $.messager.alert('警告', JSONMsg.message);
                                }
                            },
                            complete: function (XMLHttpRequest, textStatus) {

                            },
                            error: function () {

                            }
                        });

                        if (bOK == false) {
                            return;
                        }

                        bOK = false;
                        SaveDialyReportURL = "/UnReleaseSheetSetting/SaveSaleInfo?strSwbSerialNums=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForPrint").val()) + "&hid_CustomCategory_ForSetting=" + encodeURI($("#hid_CustomCategory_ForSetting").val()) + "&wbID_ForPrint=" + encodeURI($("#wbID_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#PickGoodsDate_ForSetting").val()) + "&wbActualWeight_ForPrint=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()) + "&OperateFee_ForSetting=" + "0.00" + "&PickGoodsFee_ForSetting=" + "0.00" + "&KeepGoodsFee_ForSetting=" + encodeURI($("#EverUnReleaseFee_ForSetting").val()) + "&ShiftGoodsFee_ForSetting=" + "0.00" + "&CollectionFee_ForSetting=" + "0.00" + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&ShouldPayUnit_ForSetting=" + "" + "&shouldPay_ForSetting=" + "0.00" + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val()) + "&ddlReceiptMode_ForSetting=" + "" + "&Receipt_ForSetting=" + "";
                        $.ajax({
                            type: "POST",
                            url: SaveDialyReportURL,
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

                        if (bOK) {
                            PrintURL = "/UnReleaseSheetSetting/Print?iPrintType=1&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForPrint").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#PickGoodsDate_ForSetting").val()) + "&EverUnReleaseNum_ForSetting=" + encodeURI($("#EverUnReleaseNum_ForSetting").val()) + "&IntervalDays_ForSetting=" + encodeURI($("#IntervalDays_ForSetting").val()) + "&EverUnReleaseWeight_ForSetting=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()) + "&EverUnReleaseFee_ForSetting=" + encodeURI($("#EverUnReleaseFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue"));
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
                        }
                    });


    });

    $("#btnPreExcel").click(function () {
        if ($("#currentReleaseSubWayBill").val() == "") {
            reWriteMessagerAlert('操作提示', "没有填写需要放行的放行单不允许导出或打印", 'error');
            return false;
        }
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

        PrintURL = "/UnReleaseSheetSetting/Excel?iPrintType=0&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForPrint").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#PickGoodsDate_ForSetting").val()) + "&EverUnReleaseNum_ForSetting=" + encodeURI($("#EverUnReleaseNum_ForSetting").val()) + "&IntervalDays_ForSetting=" + encodeURI($("#IntervalDays_ForSetting").val()) + "&EverUnReleaseWeight_ForSetting=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()) + "&EverUnReleaseFee_ForSetting=" + encodeURI($("#EverUnReleaseFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&browserType=" + browserType;
        window.open(PrintURL);
    });

    $("#btnConfirmExcel").click(function () {
        if ($("#currentReleaseSubWayBill").val() == "") {
            reWriteMessagerAlert('操作提示', "没有填写需要放行的放行单不允许导出或打印", 'error');
            return false;
        }
        reWriteMessagerConfirm("提示", "确定现在就导出吗？</br><font style='color:red;font-weight:bold'>导出成功后，本次导出信息将进入该日销售日报表</font>",
                    function (ok) {
                        var bOK = false;
                        SendEmail_Excel_Url = "/UnReleaseSheetSetting/SendEmail_Excel?iPrintType=1&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForPrint").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#PickGoodsDate_ForSetting").val()) + "&EverUnReleaseNum_ForSetting=" + encodeURI($("#EverUnReleaseNum_ForSetting").val()) + "&IntervalDays_ForSetting=" + encodeURI($("#IntervalDays_ForSetting").val()) + "&EverUnReleaseWeight_ForSetting=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()) + "&EverUnReleaseFee_ForSetting=" + encodeURI($("#EverUnReleaseFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&browserType=" + browserType;
                        $.ajax({
                            type: "POST",
                            url: SendEmail_Excel_Url,
                            data: "",
                            async: false,
                            cache: false,
                            beforeSend: function (XMLHttpRequest) {
                                $("#lblTip").html("<font style='color:blue;font-weight:bold'>正在验证并发送邮件……</font>");
                            },
                            success: function (msg) {
                                $("#lblTip").html("");
                                var JSONMsg = eval("(" + msg + ")");
                                if (JSONMsg.result.toLowerCase() == 'ok') {
                                    bOK = true;
                                } else {
                                    //reWriteMessagerAlert('操作提示', JSONMsg.message, 'error');
                                    //alert(JSONMsg.message);
                                    $.messager.alert('警告',JSONMsg.message);
                                }
                            },
                            complete: function (XMLHttpRequest, textStatus) {

                            },
                            error: function () {

                            }
                        });

                        if (bOK == false) {
                            return;
                        }

                        bOK = false;
                        SaveDialyReportURL = "/UnReleaseSheetSetting/SaveSaleInfo?strSwbSerialNums=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForPrint").val()) + "&hid_CustomCategory_ForSetting=" + encodeURI($("#hid_CustomCategory_ForSetting").val()) + "&wbID_ForPrint=" + encodeURI($("#wbID_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#PickGoodsDate_ForSetting").val()) + "&wbActualWeight_ForPrint=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()) + "&OperateFee_ForSetting=" + "0.00" + "&PickGoodsFee_ForSetting=" + "0.00" + "&KeepGoodsFee_ForSetting=" + encodeURI($("#EverUnReleaseFee_ForSetting").val()) + "&ShiftGoodsFee_ForSetting=" + "0.00" + "&CollectionFee_ForSetting=" + "0.00" + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&ShouldPayUnit_ForSetting=" + "" + "&shouldPay_ForSetting=" + "0.00" + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val()) + "&ddlReceiptMode_ForSetting=" + "" + "&Receipt_ForSetting=" + "";
                        $.ajax({
                            type: "POST",
                            url: SaveDialyReportURL,
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
                        if (bOK) {
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

                            PrintURL = "/UnReleaseSheetSetting/Excel?iPrintType=1&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForPrint").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#PickGoodsDate_ForSetting").val()) + "&EverUnReleaseNum_ForSetting=" + encodeURI($("#EverUnReleaseNum_ForSetting").val()) + "&IntervalDays_ForSetting=" + encodeURI($("#IntervalDays_ForSetting").val()) + "&EverUnReleaseWeight_ForSetting=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()) + "&EverUnReleaseFee_ForSetting=" + encodeURI($("#EverUnReleaseFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&browserType=" + browserType;
                            window.open(PrintURL);
                        }
                    });

    });

    $("#EverUnReleaseNum_ForSetting").blur(function () {
        ManulComputeFee();
    });

    $("#IntervalDays_ForSetting").blur(function () {
        ManulComputeFee();
    });

    $("#EverUnReleaseNum_ForSetting").blur(function () {
        ManulComputeFee();
    });

    $("#EverUnReleaseWeight_ForSetting").blur(function () {
        ManulComputeFee();
    });

    //string customCategory,string intervalNum,string intervalDays,string ActualWeight
    function ManulComputeFee() {
        $.ajax({
            type: "POST",
            url: Manul_ComputeFeeURL + encodeURI($("#hid_CustomCategory_ForSetting").val()) + "&intervalNum=" + encodeURI($("#EverUnReleaseNum_ForSetting").val()) + "&intervalDays=" + encodeURI($("#IntervalDays_ForSetting").val()) + "&ActualWeight=" + encodeURI($("#EverUnReleaseWeight_ForSetting").val()),
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                $("#EverUnReleaseFee_ForSetting").val(msg);
                $("#TotalFee_ForSetting").val(msg);
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
    }

    _$_datagrid.datagrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'swbID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        columns: [[
        //                    { field: 'cb', title: '', width: 100, checkbox: true
        //                    },
					{field: 'swbSerialNum', title: '分运单号', width: 120, sortable: true,
					sorter: function (a, b) {
					    return (a > b ? 1 : -1);
					}
	},
                    { field: 'swbDescription_CHN', title: '货物中文名', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbDescription_ENG', title: '货物英文名', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNumber', title: '件数', width: 60, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbWeight', title: '重量', width: 70, sortable: true, align: "right",
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'DetainDate', title: '扣留日期', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    }
				]],
        pagination: true,
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
        //,
        //        onSelect: function (rowIndex, rowData) {
        //            Append_hid_Checked_swbSerialNum(rowData.swbSerialNum, true, 0, false);
        //        },
        //        onUnselect: function (rowIndex, rowData) {
        //            Append_hid_Checked_swbSerialNum(rowData.swbSerialNum, false, 0, false);
        //        },
        //        onCheck: function (rowIndex, rowData) {
        //            Append_hid_Checked_swbSerialNum(rowData.swbSerialNum, true, 0, false);
        //        },
        //        onUncheck: function (rowIndex, rowData) {
        //            Append_hid_Checked_swbSerialNum(rowData.swbSerialNum, false, 0, false);
        //        },
        //        onSelectAll: function (rows) {
        //            Append_hid_Checked_swbSerialNum("", true, 1, true);
        //        },
        //        onUnselectAll: function (rows) {
        //            Append_hid_Checked_swbSerialNum("", false, 1, false);
        //        },
        //        onCheckAll: function (rows) {
        //            Append_hid_Checked_swbSerialNum("", true, 1, true);
        //        },
        //        onUncheckAll: function (rows) {
        //            Append_hid_Checked_swbSerialNum("", false, 1, false);
        //        }
    });

    function Append_hid_Checked_swbSerialNum(v, bAdd, iType, bSeleAll) {

        switch (iType) {
            case 0:
                if ($("#currentReleaseSubWayBill").val() != "") {
                    var hid_Checked_swbSerialNum_OldValues = [];
                    var hid_Checked_swbSerialNum_NewValues = [];
                    hid_Checked_swbSerialNum_OldValues = $("#currentReleaseSubWayBill").val().split(",");
                    for (var i = 0; i < hid_Checked_swbSerialNum_OldValues.length; i++) {
                        if (hid_Checked_swbSerialNum_OldValues[i] != v) {
                            hid_Checked_swbSerialNum_NewValues.push(hid_Checked_swbSerialNum_OldValues[i]);
                        }
                    }

                    if (bAdd) {
                        hid_Checked_swbSerialNum_NewValues.push(v);
                    }

                    $("#currentReleaseSubWayBill").val(hid_Checked_swbSerialNum_NewValues.join(","));
                } else {
                    if (bAdd) {
                        $("#currentReleaseSubWayBill").val(v);
                    }
                }
                break;
            case 1:
                if (!bSeleAll) {
                    $("#currentReleaseSubWayBill").val("");
                } else {
                    var rows = _$_datagrid.datagrid("getRows");
                    var hid_Checked_swbSerialNum_NewValues = [];
                    for (var i = 0; i < rows.length; i++) {
                        hid_Checked_swbSerialNum_NewValues.push(rows[i].swbSerialNum);
                    }
                    $("#currentReleaseSubWayBill").val(hid_Checked_swbSerialNum_NewValues.join(","));
                }
                break;
            default:
                break;

        }

    }

    function SeleAll() {
        var rows = _$_datagrid.datagrid("getRows");
        for (var i = 0; i < rows.length; i++) {
            var m = _$_datagrid.datagrid("getRowIndex", rows[i]);
            _$_datagrid.datagrid("selectRow", m)
        }
    }

    //    setTimeout(function () {
    //        _$_datagrid.datagrid("selectAll");
    //    }, 1000);

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "swbserialnum":
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
