$(function () {
    var LoginURL = "/Login/Login";
    var enableEnter = true;

    var MsgDialog = null;

    $('#txtComment').combobox({
        //data: [{ "value": "---请选择---", "id": "---请选择---" }, { "text": "天津机场海关", "id": "0" }, { "text": "TAT货运站", "id": "1" }, { "text": "货运代理", "id": "2"}],
        data: [{ "value": "---请选择---", "id": "---请选择---" }, { "text": "TAT货运站", "id": "1" }, { "text": "货运代理", "id": "2"}],
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    $("#txtUserName").focus();

    var hidComment = $("#hidComment").val();
    if (hidComment.toLowerCase() == "null" || hidComment == "") {
        $("#btnOtherComment").css("display", "none");
    } else {

        switch (hidComment) {
//            case "0":
//                $('#txtComment').combobox({
//                    data: [{ "text": "天津机场海关", "id": "0"}],
//                    valueField: 'id',
//                    textField: 'text',
//                    editable: false,
//                    panelHeight: null
//                });
//                break;
            case "1":
                $('#txtComment').combobox({
                    data: [{ "text": "TAT货运站", "id": "1"}],
                    valueField: 'id',
                    textField: 'text',
                    editable: false,
                    panelHeight: null
                });
                break;
            case "2":
                $('#txtComment').combobox({
                    data: [{ "text": "货运代理", "id": "2"}],
                    valueField: 'id',
                    textField: 'text',
                    editable: false,
                    panelHeight: null
                });
                break;
            default:

        }
        $('#txtComment').combobox("setValue", hidComment);
    }

    $("#btnLogin").click(function () {
        Login();
    });

    $(document).keydown(function (keycode) {
        if (keycode.keyCode == 13) {
            if (enableEnter) {
                $('#formLogin').form('validate');
                Login();
            }

        }
    }); ;

    function Login() {
        if (MsgDialog != null) {
            MsgDialog.dialog('close');
        }

        var userName = $("#txtUserName").val();
        var Password = $("#txtPassword").val();
        var comment = $("#txtComment").combobox("getValue");
        $('#formLogin').form('submit', {
            url: LoginURL,
            onSubmit: function () {
                $("#lblMessage").html("<span style='color:red;font-weight:bold'>正在登录....</span>");
                if (userName == "" || Password == "" || comment == "") {
                    $("#lblMessage").html("<span style='color:red;font-weight:bold'></span>");
                    $.fn.window.defaults.closable = false;  // disable the closable button
                    enableEnter = false;
                    $("#msgContent").html('登录不成功，请填写完整信息<br/>(用户名、密码以及登录类型必须填写)');
                    $("#dlg_msg").show();
                    MsgDialog = $('#dlg_msg').dialog({
                        buttons: [{
                            text: '关 闭',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                enableEnter = true;
                                MsgDialog.dialog('close');
                            }
                        }],
                        title: '操作提示',
                        left: 50,
                        top: 30,
                        width: 300,
                        height: 150,
                        modal: true,
                        resizable: true,
                        cache: false,
                        closed: true,
                        closable: false
                    });

                    MsgDialog.dialog("open");
                    $.fn.window.defaults.closable = true;  // restore the closable button
                    return false;
                }
            },
            success: function (result) {
                var result = eval("(" + result + ")");

                if (result.result == "ok") {
                    $("#lblMessage").html("<span style='color:blue;font-weight:bold'>登录成功,正在进入主页面</span>");
                    var strRetURL = "";
                    if ($("#hidRetURL").val() != "") {
                        strRetURL = $("#hidRetURL").val();
                    } else {
                        switch (comment) {
                            case "0": //海关
                                strRetURL = "/CustomerMain";
                                break;
                            case "1": //清关
                                strRetURL = "/HuayuMain";
                                break;
                            case "2": //货代公司
                                strRetURL = "/ForwarderMain";
                                break;
                            default:

                        }
                    }
                    window.location = strRetURL;
                } else {
                    $("#lblMessage").html("<span style='color:red;font-weight:bold'>登录失败</span>");
                    $.fn.window.defaults.closable = false;  // disable the closable button
                    enableEnter = false;
                    $("#msgContent").html(result.message);
                    $("#dlg_msg").show();
                    MsgDialog = $('#dlg_msg').dialog({
                        buttons: [{
                            text: '关 闭',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                enableEnter = true;
                                MsgDialog.dialog('close');
                            }
                        }],
                        title: '操作提示',
                        left: 50,
                        top: 30,
                        width: 300,
                        height: 150,
                        modal: true,
                        resizable: true,
                        cache: false,
                        closed: true,
                        closable: false
                    });
                    MsgDialog.dialog("open");
                    $.fn.window.defaults.closable = true;  // restore the closable button
                    return false;
                }

            }
        });
    }
});