$(function () {
    var _$_datagrid = $("#DG_SubWayBill");

    var UploadFileURL = "/Forwarder_Personal/UploadFile";
    var SaveDataURL = "/Forwarder_Personal/SaveData";
    var QueryURL = "/Forwarder_Personal/GetSubWayBill";

    var TestExistWBSerialNum = "/Forwarder_Personal/TestExistWBSerialNum?strWBSerialNum=";
    var TestSWBSerialNumExistInOtherWayBillURL = "/Forwarder_Personal/TestExistSWBSerialNumInOtherWayBill?wbSerialNum=";
    var TestHasUploadData = "/Forwarder_Personal/TestHasUploadData";

    $("#btnUpLoad").click(function () {
        uploadFile();
    });

    $("#btnSave").click(function () {
        saveData();
    });

    function uploadFile() {
        $('#form_FileUpload').form('submit', {
            url: UploadFileURL,
            onSubmit: function () {
                // do some check
                // return false to prevent submit;
                if ($("#myFile").val() == "") {
                    reWriteMessagerAlert("操作提示", "请先选择文件", "error");
                    return false;
                }

                var win = $.messager.progress({
                    title: '请稍等',
                    msg: '正在处理数据……'
                });
            },
            success: function (data) {
                $.messager.progress('close');
                var MainWayBill = eval("(" + data + ")").MainWayBill.data;
                if (eval("(" + data + ")").MainWayBill.result == "error") {
                    reWriteMessagerAlert("操作提示", eval("(" + data + ")").MainWayBill.message, "error"); 
                    $("#txtSeriaNum").val("");
                    $("#txtWbVoyage").val("");
                    $("#txtWbIOmark").val("");
                    $("#txtWbChinese").val("");
                    $("#txtWbEnglish").val("");
                    $("#txtWbTransportMode").val("");
                    $("#txtWbEntryDate").val("");
                    $("#txtWbSRport").val("");
                    $("#txtWbPortCode").val("");
                    $("#txtWbTotalWeight").val("");
                    $("#txtWbTotalNumber").val("");
                    $("#txtSubNumber").val("");
                } else {
                    $("#txtSeriaNum").val(MainWayBill.txtSeriaNum);
                    $("#txtWbVoyage").val(MainWayBill.txtWbVoyage);
                    $("#txtWbIOmark").val(MainWayBill.txtWbIOmark);
                    $("#txtWbChinese").val(MainWayBill.txtWbChinese);
                    $("#txtWbEnglish").val(MainWayBill.txtWbEnglish);
                    $("#txtWbTransportMode").val(MainWayBill.txtWbTransportMode);
                    $("#txtWbEntryDate").val(MainWayBill.txtWbEntryDate);
                    $("#txtWbSRport").val(MainWayBill.txtWbSRport);
                    $("#txtWbPortCode").val(MainWayBill.txtWbPortCode);
                    $("#txtWbTotalWeight").val(MainWayBill.txtWbTotalWeight);
                    $("#txtWbTotalNumber").val(MainWayBill.txtWbTotalNumber);
                    $("#txtSubNumber").val(MainWayBill.txtSubNumber);
                }

                _$_datagrid.datagrid("reload");
            }
        });
    }

    function saveData() {
        if ($("#txtSeriaNum").val() == "") {
            reWriteMessagerAlert("操作提示", "请先选择文件进行数据导入", "error");
            return false;
        }

        var bExist = false;
        var msgRet = "";
        $.ajax({
            type: "GET",
            url: TestHasUploadData,
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (msg) {
                var JSONMsg = eval("(" + msg + ")");
                if (JSONMsg.result.toLowerCase() == 'yes') {
                    bExist = true;
                } else {
                    msgRet = JSONMsg.message;
                }
            },
            complete: function (XMLHttpRequest, textStatus) {

            },
            error: function () {

            }
        });

        if (!bExist) {
            reWriteMessagerAlert("操作提示", msgRet, "error");
            return false;
        }

        bExist = false;
//        $.ajax({
//            type: "GET",
//            url: TestExistWBSerialNum + encodeURI($("#txtSeriaNum").val()),
//            data: "",
//            async: false,
//            cache: false,
//            beforeSend: function (XMLHttpRequest) {

//            },
//            success: function (msg) {
//                var JSONMsg = eval("(" + msg + ")");
//                if (JSONMsg.result.toLowerCase() == 'true') {
//                    bExist = true;
//                }
//            },
//            complete: function (XMLHttpRequest, textStatus) {

//            },
//            error: function () {

//            }
//        });

        if (bExist) {
            reWriteMessagerConfirm("提示信息", "已经存在总运单号为[" + $("#txtSeriaNum").val() + "]的总运单,是否继续导入?<br/><font style='color:red;font-weight:bold'>继续导入将覆盖此总运单之前的分运单信息,请谨慎选择<font/>", function (ok) {
                if (ok) {
                    bExist = false;
                    $.ajax({
                        type: "GET",
                        url: TestSWBSerialNumExistInOtherWayBillURL + encodeURI($("#txtSeriaNum").val()),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            var JSONMsg = eval("(" + msg + ")");
                            if (JSONMsg.result.toLowerCase() == 'error') {
                                bExist = true;
                                msgRet = JSONMsg.message;
                            }
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });

                    if (bExist) {
                        reWriteMessagerConfirm("提示信息", msgRet, function (ok) {
                            if (ok) {
                                $('#MainWayBillForm').form('submit', {
                                    url: SaveDataURL,
                                    onSubmit: function () {
                                        var win = $.messager.progress({
                                            title: '请稍等',
                                            msg: '正在处理数据……'
                                        });
                                    },
                                    success: function (result) {
                                        $.messager.progress('close');
                                        var result = eval("(" + result + ")");
                                        if (result.result == "ok") {
                                            reWriteMessagerAlert("操作提示", result.message, "info");

                                            $("#txtSeriaNum").val("");
                                            $("#txtWbVoyage").val("");
                                            $("#txtWbIOmark").val("");
                                            $("#txtWbChinese").val("");
                                            $("#txtWbEnglish").val("");
                                            $("#txtWbTransportMode").val("");
                                            $("#txtWbEntryDate").val("");
                                            $("#txtWbSRport").val("");
                                            $("#txtWbPortCode").val("");
                                            $("#txtWbTotalWeight").val("");
                                            $("#txtWbTotalNumber").val("");
                                            $("#txtSubNumber").val("");
                                            _$_datagrid.datagrid("reload");

                                            return true;
                                        } else {
                                            reWriteMessagerAlert("操作提示", result.message, "error");
                                            return false;
                                        }
                                    }
                                });
                            } else {
                                return false;
                            }
                        });
                    } else {
                        $('#MainWayBillForm').form('submit', {
                            url: SaveDataURL,
                            onSubmit: function () {
                                var win = $.messager.progress({
                                    title: '请稍等',
                                    msg: '正在处理数据……'
                                });
                            },
                            success: function (result) {
                                $.messager.progress('close');
                                var result = eval("(" + result + ")");
                                if (result.result == "ok") {
                                    reWriteMessagerAlert("操作提示", result.message, "info");

                                    $("#txtSeriaNum").val("");
                                    $("#txtWbVoyage").val("");
                                    $("#txtWbIOmark").val("");
                                    $("#txtWbChinese").val("");
                                    $("#txtWbEnglish").val("");
                                    $("#txtWbTransportMode").val("");
                                    $("#txtWbEntryDate").val("");
                                    $("#txtWbSRport").val("");
                                    $("#txtWbPortCode").val("");
                                    $("#txtWbTotalWeight").val("");
                                    $("#txtWbTotalNumber").val("");
                                    $("#txtSubNumber").val("");
                                    _$_datagrid.datagrid("reload");

                                    return true;
                                } else {
                                    reWriteMessagerAlert("操作提示", result.message, "error");
                                    return false;
                                }
                            }
                        });
                    }
                } else {
                    return false;
                }
            });
        } else {
            bExist = false;
            var msgRet = "";
//            $.ajax({
//                type: "GET",
//                url: TestSWBSerialNumExistInOtherWayBillURL + encodeURI($("#txtSeriaNum").val()),
//                data: "",
//                async: false,
//                cache: false,
//                beforeSend: function (XMLHttpRequest) {

//                },
//                success: function (msg) {
//                    var JSONMsg = eval("(" + msg + ")");
//                    if (JSONMsg.result.toLowerCase() == 'error') {
//                        bExist = true;
//                        msgRet = JSONMsg.message;
//                    }
//                },
//                complete: function (XMLHttpRequest, textStatus) {

//                },
//                error: function () {

//                }
//            });

            if (bExist) {
                reWriteMessagerConfirm("提示信息", msgRet, function (ok) {
                    if (ok) {
                        $('#MainWayBillForm').form('submit', {
                            url: SaveDataURL,
                            onSubmit: function () {
                                var win = $.messager.progress({
                                    title: '请稍等',
                                    msg: '正在处理数据……'
                                });
                            },
                            success: function (result) {
                                $.messager.progress('close');
                                var result = eval("(" + result + ")");
                                if (result.result == "ok") {
                                    reWriteMessagerAlert("操作提示", result.message, "info");

                                    $("#txtSeriaNum").val("");
                                    $("#txtWbVoyage").val("");
                                    $("#txtWbIOmark").val("");
                                    $("#txtWbChinese").val("");
                                    $("#txtWbEnglish").val("");
                                    $("#txtWbTransportMode").val("");
                                    $("#txtWbEntryDate").val("");
                                    $("#txtWbSRport").val("");
                                    $("#txtWbPortCode").val("");
                                    $("#txtWbTotalWeight").val("");
                                    $("#txtWbTotalNumber").val("");
                                    $("#txtSubNumber").val("");
                                    _$_datagrid.datagrid("reload");

                                    return true;
                                } else {
                                    reWriteMessagerAlert("操作提示", result.message, "error");
                                    return false;
                                }
                            }
                        });
                    } else {
                        return false;
                    }
                });
            } else {
                $('#MainWayBillForm').form('submit', {
                    url: SaveDataURL,
                    onSubmit: function () {
                        var win = $.messager.progress({
                            title: '请稍等',
                            msg: '正在处理数据……'
                        });
                    },
                    success: function (result) {
                        $.messager.progress('close');
                        var result = eval("(" + result + ")");
                        if (result.result == "ok") {
                            reWriteMessagerAlert("操作提示", result.message, "info");

                            $("#txtSeriaNum").val("");
                            $("#txtWbVoyage").val("");
                            $("#txtWbIOmark").val("");
                            $("#txtWbChinese").val("");
                            $("#txtWbEnglish").val("");
                            $("#txtWbTransportMode").val("");
                            $("#txtWbEntryDate").val("");
                            $("#txtWbSRport").val("");
                            $("#txtWbPortCode").val("");
                            $("#txtWbTotalWeight").val("");
                            $("#txtWbTotalNumber").val("");
                            $("#txtSubNumber").val("");
                            _$_datagrid.datagrid("reload");

                            return true;
                        } else {
                            reWriteMessagerAlert("操作提示", result.message, "error");
                            return false;
                        }
                    }
                });
            }
        }

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
        sortOrder: 'asc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        columns: [[
					{ field: 'swbID', title: '序号', width: 50, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'wbSerialNum', title: '总运单编号', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbSerialNum', title: '分运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbDescription_CHN', title: '中文货物名称', width: 150, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbDescription_ENG', title: '英文货物名称', width: 150, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNumber', title: '件数', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbWeight', title: '重量(公斤)', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbValue', title: '价值', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbMonetary', title: '币制', width: 60, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbRecipients', title: '收件人', width: 160, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbCustomsCategory', title: '报关类别', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    }
				]],
        pagination: true,
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
            //_$_datagrid.datagrid("reload");
        }
    });


    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "swbid":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "swbID") {

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
