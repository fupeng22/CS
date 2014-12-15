$(function () {
    var GetCodeURL = "/Test2/GetCode?";
    $("#btn1").click(function () {
        GetCodeURL = "/Test2/GetCode?type=" + encodeURI($("#seleType").val()) + "&str=" + encodeURI($("#txt1").val()) + "&width=" + encodeURI($("#width").val()) + "&height=" + encodeURI($("#height").val());
        $.ajax({
            type: "GET",
            url: GetCodeURL,
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                $("#span_Result").html(msg);
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
    });

    $("#btn2").click(function () {
        var QueryURL = "http://10.45.9.128:8181/ems/api/process";
        $.ajax({
            type: "POST",
            url: QueryURL,
            data: { parternID: "TESTJSON", serviceType: "RequestQuery", mailNo: "dfg", digest: "jyS6tAgzjT1QO%2Fb%2F%2FSMabw%3D%3D" },
            async: false,
            cache: false,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                //console.info(msg);
                alert(msg);
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
    });

    $("#btn3").click(function () {
        $('#form_FileUpload').form('submit', {
            url: "http://10.45.9.128:8181/ems/api/process?parternID=TESTJSON&serviceType=RequestQuery&mailNo=dfg&digest=jyS6tAgzjT1QO%2Fb%2F%2FSMabw%3D%3D",
            success: function (data) {
                console.info(data);
            }
        });
    });

    $("#btn4").click(function () {
        $("#span_Result").printArea()
    });
});