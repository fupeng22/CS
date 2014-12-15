$(function () {
    var UpdateFeeForeSettingURL = "/Huayu_ViewFeeForeSettingInfo/updateFeeForeSettingInfo?wbwId=";
    var QueryFeeRateURL = "/Huayu_FeeRateSetting/getFeeRateValue?categoryID=";
    var CumputeFeeForTCSURL = "/FirstPickGoodsSheetSetting/CumputeFeeForTCS?strActualWeight=";
    $('#ddlReceiptMode_ForSetting').combobox({
        data: [{ "text": "直航", "id": "直航" }, { "text": "转关", "id": "转关"}],
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null,
        onLoadSuccess: function (node, data) {
            $('#ddlReceiptMode_ForSetting').combobox("setValue", $("#hid_ddlReceiptMode_ForSetting").val());
        }
    });

    $('#ddlPayMode_ForSetting').combobox({
        data: [{ "text": "现金", "id": "现金" }, { "text": "月结", "id": "月结" }, { "text": "欠条", "id": "欠条"}],
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null,
        onLoadSuccess: function (node, data) {
            $('#ddlPayMode_ForSetting').combobox("setValue", $("#hid_ddlPayMode_ForSetting").val());
        }
    });

    $('#Receipt_ForSetting').combobox({
        data: [{ "text": "其他", "id": "其他" }, { "text": "TAT", "id": "TAT" }, { "text": "TCS", "id": "TCS" }, { "text": "CA", "id": "CA" }, { "text": "TCS快件库", "id": "TCS快件库" }, { "text": "场外中转", "id": "场外中转"}],
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null,
        onLoadSuccess: function (node, data) {
            $('#Receipt_ForSetting').combobox("setValue", $("#hid_Receipt_ForSetting").val());
        }
    });

    $("#btnUpdateFeeFore").click(function () {
        var wbwID = $("#hid_wbwID").val();
        var ddlReceiptMode_ForSetting = $('#ddlReceiptMode_ForSetting').combobox("getValue");
        var OperateFee_ForSetting = $("#OperateFee_ForSetting").val();
        var PickGoodsFee_ForSetting = $("#PickGoodsFee_ForSetting").val();
        var ShiftGoodsFee_ForSetting = $("#ShiftGoodsFee_ForSetting").val();
        var CollectionFee_ForSetting = $("#CollectionFee_ForSetting").val();
        var ddlPayMode_ForSetting = $('#ddlPayMode_ForSetting').combobox("getValue");
        var ShouldPayUnit_ForSetting = $("#ShouldPayUnit_ForSetting").val();
        var shouldPay_ForSetting = $("#shouldPay_ForSetting").val();
        var wbCompany_ForSetting = $("#wbCompany_ForSetting").val();
        var ReportSystem_ForSetting = $("#ReportSystem_ForSetting").val();
        var QuarantineCheckFee_ForSetting = $("#QuarantineCheckFee_ForSetting").val();
        var QuarantinePacketFee_ForSetting = $("#QuarantinePacketFee_ForSetting").val();
        var Receipt_ForSetting = $('#Receipt_ForSetting').combobox("getValue");

        if (wbwID == "" || wbwID == "-1") {
            $.messager.alert('操作提示', "数据ID不正确不允许提交", 'error');
            return false;
        }

        if (ddlReceiptMode_ForSetting == "" || OperateFee_ForSetting == "" || PickGoodsFee_ForSetting == "" || ShiftGoodsFee_ForSetting == "" || CollectionFee_ForSetting == "" || ddlPayMode_ForSetting == "" || ShouldPayUnit_ForSetting == "" || shouldPay_ForSetting == "" || wbCompany_ForSetting == "" || Receipt_ForSetting == "" || ReportSystem_ForSetting == "" || QuarantineCheckFee_ForSetting == "" || QuarantinePacketFee_ForSetting == "") {
            $.messager.alert('操作提示', "数据填写不完整，不允许提交", 'error');
            return false;
        }

        $.ajax({
            type: "POST",
            url: UpdateFeeForeSettingURL + encodeURI(wbwID) + "&ddlReceiptMode_ForSetting=" + encodeURI(ddlReceiptMode_ForSetting) + "&OperateFee_ForSetting=" + encodeURI(OperateFee_ForSetting) + "&PickGoodsFee_ForSetting=" + encodeURI(PickGoodsFee_ForSetting) + "&ShiftGoodsFee_ForSetting=" + encodeURI(ShiftGoodsFee_ForSetting) + "&CollectionFee_ForSetting=" + encodeURI(CollectionFee_ForSetting) + "&ddlPayMode_ForSetting=" + encodeURI(ddlPayMode_ForSetting) + "&ShouldPayUnit_ForSetting=" + encodeURI(ShouldPayUnit_ForSetting) + "&shouldPay_ForSetting=" + encodeURI(shouldPay_ForSetting) + "&wbCompany_ForSetting=" + encodeURI(wbCompany_ForSetting) + "&Receipt_ForSetting=" + encodeURI(Receipt_ForSetting) + "&ReportSystem_ForSetting=" + encodeURI(ReportSystem_ForSetting) + "&QuarantineCheckFee_ForSetting=" + encodeURI(QuarantineCheckFee_ForSetting) + "&QuarantinePacketFee_ForSetting=" + encodeURI(QuarantinePacketFee_ForSetting),
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                var JSONMsg = eval("(" + msg + ")");
                if (JSONMsg.result.toLowerCase() == 'ok') {
                    $.messager.alert('操作提示', JSONMsg.message, 'info');
                } else {
                    $.messager.alert('操作提示', JSONMsg.message, 'error');
                }
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
    });

    $('#Receipt_ForSetting').combobox({
        onSelect: function (param) {
            switch ($('#Receipt_ForSetting').combobox("getValue")) {
                case "场外中转":
                    $.ajax({
                        type: "POST",
                        url: QueryFeeRateURL + encodeURI("99-1-1"),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            $("#ShiftGoodsFee_ForSetting").val(parseFloat(msg) * parseFloat($("#txtActualWeight").val()));
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });
                    break;
                case "TCS快件库":
                    $.ajax({
                        type: "POST",
                        url: CumputeFeeForTCSURL + encodeURI($("#txtActualWeight").val()),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            var JSONMsg = eval("(" + msg + ")");
                            
                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                $("#OperateFee_ForSetting").val(JSONMsg.data[0].strOperateFee);
                                $("#PickGoodsFee_ForSetting").val(JSONMsg.data[0].strPickGoodsFee);
                                $("#ShouldPayUnit_ForSetting").val(JSONMsg.data[0].strShouldPayUnit);
                                $("#shouldPay_ForSetting").val(JSONMsg.data[0].strShouldPayValue);
                                //$.messager.alert('操作提示', JSONMsg.message, 'info');
                                //$("#btnDetailQuery").click();
                            } else {
                                $.messager.alert('操作提示', JSONMsg.message, 'error');
                            }
                            //$("#ShiftGoodsFee_ForSetting").val(parseFloat(msg) * parseFloat($("#wbActualWeight_ForPrint").val()));
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });
                    break;
                case "其他":
                    $("#ShouldPayUnit_ForSetting").val("0.00");
                    $("#shouldPay_ForSetting").val("0.00");
                    $("#ShiftGoodsFee_ForSetting").val("0.00");
                    break;
                default:
                    $("#ShiftGoodsFee_ForSetting").val("0.00");
                    break;
            }
            ComputeTotal();
        }
    });

    $('#ddlReceiptMode_ForSetting').combobox({
        onSelect: function (param) {
            switch ($('#ddlReceiptMode_ForSetting').combobox("getValue")) {
                case "直航":
                    $("#ShouldPayUnit_ForSetting").val("0.5");
                    $("#shouldPay_ForSetting").val(parseFloat($("#txtActualWeight").val()) * parseFloat($("#ShouldPayUnit_ForSetting").val()) + parseFloat($("#CollectionFee_ForSetting").val()));
                    break;
                case "转关":
                    $("#ShouldPayUnit_ForSetting").val("0.7");
                    $("#shouldPay_ForSetting").val(parseFloat($("#txtActualWeight").val()) * parseFloat($("#ShouldPayUnit_ForSetting").val()) + parseFloat($("#CollectionFee_ForSetting").val()));
                    var categoryID = "";
                    switch ($("#hid_txtCustomCategory").val()) {
                        case "2":
                            categoryID = "2-1-4";
                            break;
                        case "3":
                            categoryID = "2-1-4";
                            break;
                        case "4":
                            categoryID = "2-1-4";
                            break;
                        case "5":
                            categoryID = "5-1-6";
                            break;
                        case "6":
                            categoryID = "6-1-6";
                            break;
                        default:
                            break;
                    }
                    if (categoryID != "") {
                        $.ajax({
                            type: "POST",
                            url: QueryFeeRateURL + encodeURI(categoryID),
                            data: "",
                            async: false,
                            cache: false,
                            beforeSend: function (XMLHttpRequest) {

                            },
                            success: function (msg) {
                                if ($("#PickGoodsFee_ForSetting").val() == "") {
                                    $("#PickGoodsFee_ForSetting").val(parseFloat(msg) * parseFloat($("#txtActualWeight").val()));
                                } else {
                                    $("#PickGoodsFee_ForSetting").val(parseFloat($("#PickGoodsFee_ForSetting").val()) + parseFloat(msg) * parseFloat($("#txtActualWeight").val()));
                                }
                            },
                            complete: function (XMLHttpRequest, textStatus) {

                            },
                            error: function () {

                            }
                        });
                    }

                    break;
                default:
                    $("#ShouldPayUnit_ForSetting").val("0.00");
                    $("#shouldPay_ForSetting").val("0.00");
                    break;
            }

            ComputeTotal();
        }
    });

    $("#OperateFee_ForSetting").blur(function () {
        ComputeTotal();
    });

    $("#PickGoodsFee_ForSetting").blur(function () {
        ComputeTotal();
    });

    $("#ShiftGoodsFee_ForSetting").blur(function () {
        ComputeTotal();
    });

    $("#CollectionFee_ForSetting").blur(function () {
        ComputeTotal();
    });

    $("#ShouldPayUnit_ForSetting").blur(function () {
        ComputeTotal();
    });

    $("#shouldPay_ForSetting").blur(function () {
        ComputeTotal();
    });

    $("#ReportSystem_ForSetting").blur(function () {
        ComputeTotal();
    });

    $("#QuarantineCheckFee_ForSetting").blur(function () {
        ComputeTotal();
    });

    $("#QuarantinePacketFee_ForSetting").blur(function () {
        ComputeTotal();
    });

    function ComputeTotal() {
        var OperateFee_ForSetting = $("#OperateFee_ForSetting").val() == "" ? 0.00 : parseFloat($("#OperateFee_ForSetting").val());
        var PickGoodsFee_ForSetting = $("#PickGoodsFee_ForSetting").val() == "" ? 0.00 : parseFloat($("#PickGoodsFee_ForSetting").val());
        var ShiftGoodsFee_ForSetting = $("#ShiftGoodsFee_ForSetting").val() == "" ? 0.00 : parseFloat($("#ShiftGoodsFee_ForSetting").val());
        var CollectionFee_ForSetting = $("#CollectionFee_ForSetting").val() == "" ? 0.00 : parseFloat($("#CollectionFee_ForSetting").val());
        var ReportSystem_ForSetting = $("#ReportSystem_ForSetting").val() == "" ? 0.00 : parseFloat($("#ReportSystem_ForSetting").val());
        var QuarantineCheckFee_ForSetting = $("#QuarantineCheckFee_ForSetting").val() == "" ? 0.00 : parseFloat($("#QuarantineCheckFee_ForSetting").val());
        var QuarantinePacketFee_ForSetting = $("#QuarantinePacketFee_ForSetting").val() == "" ? 0.00 : parseFloat($("#QuarantinePacketFee_ForSetting").val());
    }
});