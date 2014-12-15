$(function () {
    var _$_datagrid = $("#DG_SubWayBillResult");

    var PrintURL = "";
    var QueryURL = "/RejectSheetSetting/GetData?wbID=" + encodeURI($("#wbID_ForPrint").val()) + "&txtBeginD=" + encodeURI($("#txtBeginD_Print").val()) + "&txtEndD=" + encodeURI($("#txtEndD_Print").val());

    var SendEmail_PDF_Url = "";
    var SendEmail_Excel_Url = "";

    var QueryFeeRateURL = "/Huayu_FeeRateSetting/getFeeRateValue?categoryID=";

    var ManualComputeURL = "/RejectSheetSetting/ManualComputeFee?txtBeginD=";
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

    var SaveDialyReportURL = "/RejectSheetSetting/SaveSaleInfo?FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForSetting").val()) + "&hid_CustomCategory_ForSetting=" + encodeURI($("#hid_CustomCategory_ForSetting").val()) + "&wbID_ForPrint=" + encodeURI($("#wbID_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#RejectDate_ForSetting").val()) + "&wbActualWeight_ForPrint=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&OperateFee_ForSetting=" + "0.00" + "&PickGoodsFee_ForSetting=" + "0.00" + "&KeepGoodsFee_ForSetting=" + encodeURI($("#KeepFee_ForSetting").val()) + "&ShiftGoodsFee_ForSetting=" + "0.00" + "&CollectionFee_ForSetting=" + "0.00" + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&ShouldPayUnit_ForSetting=" + "" + "&shouldPay_ForSetting=" + "0.00" + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val()) + "&ddlReceiptMode_ForSetting=" + "" + "&Receipt_ForSetting=" + "&RejectFee_ForSetting=" + encodeURI($("#RejectFee_ForSetting").val());

    $("#btnConfirmPrint").click(function () {
        reWriteMessagerConfirm("提示", "确定现在就打印吗？</br><font style='color:red;font-weight:bold'>打印成功后，本次打印信息将进入该日销售日报表</font>",
                    function (ok) {
                        var bOK = false;
                        SendEmail_PDF_Url = "/RejectSheetSetting/SendMail_PDF?iPrintType=1&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&txtBeginD=" + encodeURI($("#txtBeginD_Print").val()) + "&txtEndD=" + encodeURI($("#txtEndD_Print").val()) + "&FlowNum_ForSetting=" + encodeURI($("#FlowNum_ForSetting").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&RejectDate_ForSetting=" + encodeURI($("#RejectDate_ForSetting").val()) + "&RejectNum_ForSetting=" + encodeURI($("#RejectNum_ForSetting").val()) + "&RejectWeight_ForSetting=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&RejectFee_ForSetting=" + encodeURI($("#RejectFee_ForSetting").val()) + "&KeepFee_ForSetting=" + encodeURI($("#KeepFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val());
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
                        SaveDialyReportURL = "/RejectSheetSetting/SaveSaleInfo?FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForSetting").val()) + "&hid_CustomCategory_ForSetting=" + encodeURI($("#hid_CustomCategory_ForSetting").val()) + "&wbID_ForPrint=" + encodeURI($("#wbID_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#RejectDate_ForSetting").val()) + "&wbActualWeight_ForPrint=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&OperateFee_ForSetting=" + "0.00" + "&PickGoodsFee_ForSetting=" + "0.00" + "&KeepGoodsFee_ForSetting=" + encodeURI($("#KeepFee_ForSetting").val()) + "&ShiftGoodsFee_ForSetting=" + "0.00" + "&CollectionFee_ForSetting=" + "0.00" + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&ShouldPayUnit_ForSetting=" + "" + "&shouldPay_ForSetting=" + "0.00" + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val()) + "&ddlReceiptMode_ForSetting=" + "" + "&Receipt_ForSetting=" + "&RejectFee_ForSetting=" + encodeURI($("#RejectFee_ForSetting").val());
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
                            PrintURL = "/RejectSheetSetting/Print?iPrintType=1&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&txtBeginD=" + encodeURI($("#txtBeginD_Print").val()) + "&txtEndD=" + encodeURI($("#txtEndD_Print").val()) + "&FlowNum_ForSetting=" + encodeURI($("#FlowNum_ForSetting").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&RejectDate_ForSetting=" + encodeURI($("#RejectDate_ForSetting").val()) + "&RejectNum_ForSetting=" + encodeURI($("#RejectNum_ForSetting").val()) + "&RejectWeight_ForSetting=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&RejectFee_ForSetting=" + encodeURI($("#RejectFee_ForSetting").val()) + "&KeepFee_ForSetting=" + encodeURI($("#KeepFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val());
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

    $("#btnPrePrint").click(function () {
        PrintURL = "/RejectSheetSetting/Print?iPrintType=0&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&txtBeginD=" + encodeURI($("#txtBeginD_Print").val()) + "&txtEndD=" + encodeURI($("#txtEndD_Print").val()) + "&FlowNum_ForSetting=" + encodeURI($("#FlowNum_ForSetting").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&RejectDate_ForSetting=" + encodeURI($("#RejectDate_ForSetting").val()) + "&RejectNum_ForSetting=" + encodeURI($("#RejectNum_ForSetting").val()) + "&RejectWeight_ForSetting=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&RejectFee_ForSetting=" + encodeURI($("#RejectFee_ForSetting").val()) + "&KeepFee_ForSetting=" + encodeURI($("#KeepFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val());
        console.info(PrintURL);
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

    $("#btnPreExcel").click(function () {
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

        PrintURL = "/RejectSheetSetting/Excel?iPrintType=0&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&txtBeginD=" + encodeURI($("#txtBeginD_Print").val()) + "&txtEndD=" + encodeURI($("#txtEndD_Print").val()) + "&FlowNum_ForSetting=" + encodeURI($("#FlowNum_ForSetting").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&RejectDate_ForSetting=" + encodeURI($("#RejectDate_ForSetting").val()) + "&RejectNum_ForSetting=" + encodeURI($("#RejectNum_ForSetting").val()) + "&RejectWeight_ForSetting=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&RejectFee_ForSetting=" + encodeURI($("#RejectFee_ForSetting").val()) + "&KeepFee_ForSetting=" + encodeURI($("#KeepFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val()) + "&browserType=" + browserType;
        window.open(PrintURL);
    });

    $("#btnConfirmExcel").click(function () {
        reWriteMessagerConfirm("提示", "确定现在就导出吗？</br><font style='color:red;font-weight:bold'>导出成功后，本次导出信息将进入该日销售日报表</font>",
                    function (ok) {
                        var bOK = false;
                        SendEmail_Excel_Url = "/RejectSheetSetting/SendEmail_Excel?iPrintType=1&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&txtBeginD=" + encodeURI($("#txtBeginD_Print").val()) + "&txtEndD=" + encodeURI($("#txtEndD_Print").val()) + "&FlowNum_ForSetting=" + encodeURI($("#FlowNum_ForSetting").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&RejectDate_ForSetting=" + encodeURI($("#RejectDate_ForSetting").val()) + "&RejectNum_ForSetting=" + encodeURI($("#RejectNum_ForSetting").val()) + "&RejectWeight_ForSetting=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&RejectFee_ForSetting=" + encodeURI($("#RejectFee_ForSetting").val()) + "&KeepFee_ForSetting=" + encodeURI($("#KeepFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val()) + "&browserType=" + browserType;
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
                        SaveDialyReportURL = "/RejectSheetSetting/SaveSaleInfo?FlowNum_ForPrint=" + encodeURI($("#FlowNum_ForSetting").val()) + "&hid_CustomCategory_ForSetting=" + encodeURI($("#hid_CustomCategory_ForSetting").val()) + "&wbID_ForPrint=" + encodeURI($("#wbID_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&PickGoodsDate_ForSetting=" + encodeURI($("#RejectDate_ForSetting").val()) + "&wbActualWeight_ForPrint=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&OperateFee_ForSetting=" + "0.00" + "&PickGoodsFee_ForSetting=" + "0.00" + "&KeepGoodsFee_ForSetting=" + encodeURI($("#KeepFee_ForSetting").val()) + "&ShiftGoodsFee_ForSetting=" + "0.00" + "&CollectionFee_ForSetting=" + "0.00" + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&ShouldPayUnit_ForSetting=" + "" + "&shouldPay_ForSetting=" + "0.00" + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val()) + "&ddlReceiptMode_ForSetting=" + "" + "&Receipt_ForSetting=" + "&RejectFee_ForSetting=" + encodeURI($("#RejectFee_ForSetting").val());
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

                            PrintURL = "/RejectSheetSetting/Excel?iPrintType=1&strCurrentReleaseSubWayBill=" + encodeURI($("#currentReleaseSubWayBill").val()) + "&strWBID=" + encodeURI($("#wbID_ForPrint").val()) + "&txtBeginD=" + encodeURI($("#txtBeginD_Print").val()) + "&txtEndD=" + encodeURI($("#txtEndD_Print").val()) + "&FlowNum_ForSetting=" + encodeURI($("#FlowNum_ForSetting").val()) + "&wbSerialNum_ForPrint=" + encodeURI($("#wbSerialNum_ForPrint").val()) + "&InStoreDate_ForSetting=" + encodeURI($("#InStoreDate_ForSetting").val()) + "&RejectDate_ForSetting=" + encodeURI($("#RejectDate_ForSetting").val()) + "&RejectNum_ForSetting=" + encodeURI($("#RejectNum_ForSetting").val()) + "&RejectWeight_ForSetting=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&RejectFee_ForSetting=" + encodeURI($("#RejectFee_ForSetting").val()) + "&KeepFee_ForSetting=" + encodeURI($("#KeepFee_ForSetting").val()) + "&ddlPayMode_ForSetting=" + encodeURI($("#ddlPayMode_ForSetting").combobox("getValue")) + "&TotalFee_ForSetting=" + encodeURI($("#TotalFee_ForSetting").val()) + "&browserType=" + browserType;
                            window.open(PrintURL);
                        }
                    });

    });

    $("#btnQuery_Print").click(function () {
        QueryURL = "/RejectSheetSetting/GetData?wbID=" + encodeURI($("#wbID_ForPrint").val()) + "&txtBeginD=" + encodeURI($("#txtBeginD_Print").val()) + "&txtEndD=" + encodeURI($("#txtEndD_Print").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#RejectNum_ForSetting").blur(function () {
        ManualComputeFee();
    });

    $("#RejectWeight_ForSetting").blur(function () {
        //        $.ajax({
        //            type: "POST",
        //            url: QueryFeeRateURL + encodeURI("98-1-1"),
        //            data: "",
        //            async: false,
        //            cache: false,
        //            beforeSend: function (XMLHttpRequest) {

        //            },
        //            success: function (msg) {
        //                $("#RejectFee_ForSetting").val(parseFloat(msg) * parseFloat($("#RejectWeight_ForSetting").val()));
        //                ComputeTotal();
        //            },
        //            complete: function (XMLHttpRequest, textStatus) {

        //            },
        //            error: function () {

        //            }
        //        });
        ManualComputeFee();
    });

    $("#RejectFee_ForSetting").blur(function () {
        ComputeTotal();
    });

    $("#KeepFee_ForSetting").blur(function () {
        ComputeTotal();
    });

    function ComputeTotal() {
        var RejectFee_ForSetting = $("#RejectFee_ForSetting").val() == "" ? 0.00 : parseFloat($("#RejectFee_ForSetting").val());
        var KeepFee_ForSetting = $("#KeepFee_ForSetting").val() == "" ? 0.00 : parseFloat($("#KeepFee_ForSetting").val());
        $("#TotalFee_ForSetting").val(parseFloat(RejectFee_ForSetting + KeepFee_ForSetting).toFixed(2));
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
					{ field: 'swbSerialNum', title: '分运单号', width: 120, sortable: true,
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
                    { field: 'swbNumber', title: '件数', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbWeight', title: '重量', width: 100, sortable: true, align: "right",
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'RejectDate', title: '退货日期', width: 100, sortable: true, align: "right",
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    }
				]],
        pagination: true,
        toolbar: "#toolBar_Print",
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
        onLoadSuccess: function (data) {
            var rows = data.rows;
            var totalWeight = 0.00;
            for (var i = 0; i < rows.length; i++) {
                totalWeight = totalWeight + parseFloat(rows[i].swbWeight);
            }
            $("#RejectNum_ForSetting").val(data.rows.length);
            $("#RejectWeight_ForSetting").val(totalWeight.toFixed(2));

            ManualComputeFee();
        }
    });

    function ManualComputeFee() {
        $.ajax({
            type: "POST",
            url: ManualComputeURL + encodeURI($("#InStoreDate_ForSetting").val()) + "&txtEndD=" + encodeURI($("#RejectDate_ForSetting").val()) + "&RejectNum=" + encodeURI($("#RejectNum_ForSetting").val()) + "&RejectWeight=" + encodeURI($("#RejectWeight_ForSetting").val()) + "&CustomCategory=" + encodeURI($("#hid_CustomCategory_ForSetting").val()),
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                var msgJSON = eval("(" + msg + ")");
                if (msgJSON.result == "ok") {
                    $("#KeepFee_ForSetting").val(msgJSON.row[0].KeepFee_ForSetting);
                    $("#RejectFee_ForSetting").val(msgJSON.row[0].RejectFee_ForSetting);

                    $("#TotalFee_ForSetting").val(parseFloat(parseFloat(msgJSON.row[0].KeepFee_ForSetting) + parseFloat(msgJSON.row[0].RejectFee_ForSetting)).toFixed(2));
                } else {
                    reWriteMessagerAlert('操作提示', msgJSON.message, 'error');
                }
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "swbserialnum":
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
