$(function () {
    var SaveEmailURL = "/Huayu_EmailManagement/SaveEmail?txt_EmailSMTP=";
    var SaveEmailFormURL = "/Huayu_EmailManagement/SaveEmailForm";

    var editor_FirstPickGoodEmail_Body = $('#txt_FirstPickGoodEmail_Body').xheditor({ skin: 'vista', tools: 'Cut,Copy,Paste,Pastetext,Blocktag,Fontface,FontSize,Bold,Italic,Underline,Strikethrough,FontColor,BackColor,SelectAll,Removeformat,Align,List,Outdent,Indent,Link,Unlink,Hr,Table,Source,Fullscreen' });
    var editor_UnReleaseGoodEmail_Body = $('#txt_UnReleaseGoodEmail_Body').xheditor({ skin: 'vista', tools: 'Cut,Copy,Paste,Pastetext,Blocktag,Fontface,FontSize,Bold,Italic,Underline,Strikethrough,FontColor,BackColor,SelectAll,Removeformat,Align,List,Outdent,Indent,Link,Unlink,Hr,Table,Source,Fullscreen' });
    var editor_RejectGoodEmail_Body = $('#txt_RejectGoodEmail_Body').xheditor({ skin: 'vista', tools: 'Cut,Copy,Paste,Pastetext,Blocktag,Fontface,FontSize,Bold,Italic,Underline,Strikethrough,FontColor,BackColor,SelectAll,Removeformat,Align,List,Outdent,Indent,Link,Unlink,Hr,Table,Source,Fullscreen' });
    var editor_SendDialyReportMail_Body = $('#txt_SendDialyReportMail_Body').xheditor({ skin: 'vista', tools: 'Cut,Copy,Paste,Pastetext,Blocktag,Fontface,FontSize,Bold,Italic,Underline,Strikethrough,FontColor,BackColor,SelectAll,Removeformat,Align,List,Outdent,Indent,Link,Unlink,Hr,Table,Source,Fullscreen' });

    $("#txt_EmailSMTP").combobox("setValue", $("#hid_txt_EmailSMTP").val());

    $("#btnSubmitForm").click(function () {
        $('#form_EmailInfo').form('submit', {
            url: SaveEmailFormURL,
            onSubmit: function () {
                //var txt_EmailSMTP = $("#txt_EmailSMTP").val();
                var txt_EmailSMTP = $("#txt_EmailSMTP").combobox("getValue");
                var txt_EmailUserName = $("#txt_EmailUserName").val();
                var txt_EmailPwd = $("#txt_EmailPwd").val();
                var txt_FirstPickGoodEmail_Subject = $("#txt_FirstPickGoodEmail_Subject").val();
                var txt_FirstPickGoodEmail_Body = editor_FirstPickGoodEmail_Body.getSource();
                var txt_UnReleaseGoodEmail_Subject = $("#txt_UnReleaseGoodEmail_Subject").val();
                var txt_UnReleaseGoodEmail_Body = editor_UnReleaseGoodEmail_Body.getSource();
                var txt_RejectGoodEmail_Subject = $("#txt_RejectGoodEmail_Subject").val();
                var txt_RejectGoodEmail_Body = editor_RejectGoodEmail_Body.getSource();
                var txt_SendDialyReportMail_Subject = $("#txt_SendDialyReportMail_Subject").val();
                var txt_SendDialyReportMail_Body = editor_SendDialyReportMail_Body.getSource();

                $("#hid_txt_FirstPickGoodEmail_Body").val(editor_FirstPickGoodEmail_Body.getSource());
                $("#hid_txt_UnReleaseGoodEmail_Body").val(editor_UnReleaseGoodEmail_Body.getSource());
                $("#hid_txt_RejectGoodEmail_Body").val(editor_RejectGoodEmail_Body.getSource());
                $("#hid_txt_SendDialyReportMail_Body").val(editor_SendDialyReportMail_Body.getSource());

                if (txt_EmailSMTP == "" || txt_EmailUserName == "" || txt_EmailPwd == "") {
                    reWriteMessagerAlert('操作提示', "请填写完整信息(SMTP、邮箱地址、邮箱密码必填)", 'error');
                    return false;
                }

                return $(this).form('validate');

                var win = $.messager.progress({
                    title: '请稍等',
                    msg: '正在处理数据……'
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
            }
        });
    });

    $("#btnSubmit").click(function () {
        var txt_EmailSMTP = $("#txt_EmailSMTP").val();
        var txt_EmailUserName = $("#txt_EmailUserName").val();
        var txt_EmailPwd = $("#txt_EmailPwd").val();
        var txt_FirstPickGoodEmail_Subject = $("#txt_FirstPickGoodEmail_Subject").val();
        var txt_FirstPickGoodEmail_Body = editor_FirstPickGoodEmail_Body.getSource();
        var txt_UnReleaseGoodEmail_Subject = $("#txt_UnReleaseGoodEmail_Subject").val();
        var txt_UnReleaseGoodEmail_Body = editor_UnReleaseGoodEmail_Body.getSource();
        var txt_RejectGoodEmail_Subject = $("#txt_RejectGoodEmail_Subject").val();
        var txt_RejectGoodEmail_Body = editor_RejectGoodEmail_Body.getSource();

        if (txt_EmailSMTP == "" || txt_EmailUserName == "" || txt_EmailPwd == "") {
            reWriteMessagerAlert('操作提示', "请填写完整信息(SMTP、邮箱地址、邮箱密码必填)", 'error');
            return false;
        }

        $.ajax({
            type: "POST",
            url: SaveEmailURL + encodeURI(txt_EmailSMTP) + "&txt_EmailUserName=" + encodeURI(txt_EmailUserName) + "&txt_EmailPwd=" + encodeURI(txt_EmailPwd) + "&txt_FirstPickGoodEmail_Subject=" + encodeURI(txt_FirstPickGoodEmail_Subject) + "&txt_FirstPickGoodEmail_Body=" + encodeURI(txt_FirstPickGoodEmail_Body) + "&txt_UnReleaseGoodEmail_Subject=" + encodeURI(txt_UnReleaseGoodEmail_Subject) + "&txt_UnReleaseGoodEmail_Body=" + encodeURI(txt_UnReleaseGoodEmail_Body) + "&txt_RejectGoodEmail_Subject=" + encodeURI(txt_RejectGoodEmail_Subject) + "&txt_RejectGoodEmail_Body=" + encodeURI(txt_RejectGoodEmail_Body),
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
    });

});