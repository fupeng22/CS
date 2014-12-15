$(function () {
    function onDrag(e) {
        var d = e.data;
        if (d.left < 0) { d.left = 0 }
        if (d.top < 0) { d.top = 0 }
        if (d.left + $(d.target).outerWidth() > $(d.parent).width()) {
            d.left = $(d.parent).width() - $(d.target).outerWidth();
        }
        if (d.top + $(d.target).outerHeight() > $(d.parent).height()) {
            d.top = $(d.parent).height() - $(d.target).outerHeight();
        }
    }

    $('#divDraggable').draggable({
        onDrag: function (e) {
            onDrag(e);
        }
    }).resizable().bind('contextmenu', function (e) {
        e.preventDefault();
        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });
    });

    $("#btnUpLoad").click(function () {
        var UploadFileURL = "/Test1/UploadFileToFTP";
        $('#form_FileUpload').form('submit', {
            url: UploadFileURL,
            onSubmit: function () {
                var win = $.messager.progress({
                    title: '请稍等',
                    msg: '正在处理数据……'
                });
            },
            success: function (data) {
                $.messager.progress('close');
            }
        });
    });

    $("#btnGetFileList").click(function () {
        $.ajax({
            type: "POST",
            url: "/Test1/getFTPFileList",
            data: "",
            async: true,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                $("#span_FileList").html(msg);
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });
    })
});