$(function () {
    var ChangePWDURL = "/Huayu_ChangePassword/ChangePWD";

    $("#xgBtn").click(function () {
        if ($("#oldPassword").val() == "") {
            reWriteMessagerAlert("操作提示", "请输入当前密码", "error");
            $("#oldPassword").focus();
            return;
        }
        if ($("#newPassword").val() == "") {
            reWriteMessagerAlert("操作提示", "请输入新密码", "error");
            $("#newPassword").focus();
            return;
        }
        if ($("#newComfirm").val() == "") {
            reWriteMessagerAlert("操作提示", "请输入确认密码", "error");
            $("#newComfirm").focus();
            return;
        }
        if ($("#newComfirm").val() != $("#newPassword").val()) {
            reWriteMessagerAlert("操作提示", "新密码与确认新密码不一致", "error");
            $("#newComfirm").focus();
            return;
        }
        if ($("#oldPassword").val() == $("#newPassword").val()) {
            reWriteMessagerAlert("操作提示", "新密码与旧密码一致，无需修改", "error");
            $("#newComfirm").focus();
            return;
        }

        $.ajax({
            type: "GET",
            url: ChangePWDURL + "?oldPassword=" + encodeURI($("#oldPassword").val()) + "&newPassword=" + encodeURI($("#newPassword").val()) + "&newComfirm=" + encodeURI($("#newComfirm").val()),
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (result) {
                var result = eval("(" + result + ")");
                if (result.result == "ok") {
                    $.fn.window.defaults.closable = false;  // disable the closable button
                    reWriteMessagerAlert("操作提示", result.message, "info", function () {
                        parent.window.location = "/Login/Index?comment=1";
                    });
                    $.fn.window.defaults.closable = true;  // restore the closable button
                } else {

                    reWriteMessagerAlert("操作提示", result.message, "error");
                }
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
    });
});
