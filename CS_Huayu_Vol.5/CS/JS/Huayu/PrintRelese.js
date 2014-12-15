$(function () {
    var getSubWayBillUnReleased = "/Huayu_PrintRelease/produceSubWayBillUnReleased?wbID=";

    $.ajax({
        type: "POST",
        url: getSubWayBillUnReleased + encodeURI($("#hid_wbID").val()),
        data: "",
        async: false,
        cache: false,
        beforeSend: function (XMLHttpRequest) {

        },
        success: function (msg) {
            if (msg.toLowerCase() == 'none') {
                $("#subWayBill_UnReleased_Info").css("display", "none");
            } else {
                $("#subWayBill_UnReleased_Info").html(msg);
                $("#subWayBill_UnReleased_Info").css("display", "block");
            }
        },
        complete: function (XMLHttpRequest, textStatus) {

        },
        error: function () {

        }
    });

    $("#btnPrintContent").click(function () {
        $("#printArea_Content").printArea();
    });
});