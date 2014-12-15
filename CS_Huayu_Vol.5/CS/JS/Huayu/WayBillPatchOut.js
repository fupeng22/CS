$(function () {
    var _$_datagrid = $("#DG_SubWayBill");

    var UploadFileURL = "/Huayu_WayBillPatchOut/UploadFile";
    var SaveDataURL = "/Huayu_WayBillPatchOut/SaveData";
    var QueryURL = "/Huayu_WayBillPatchOut/GetSubWayBill";

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
                if (data != "") {
                    var msg = eval("(" + data + ")");
                    if (msg.result.toLowerCase() == "error") {
                        reWriteMessagerAlert("操作提示", msg.message, "error");
                        return false;
                    }
                }
                _$_datagrid.datagrid("reload");
            }
        });
    }

    function saveData() {
        $.ajax({
            type: "POST",
            url: SaveDataURL,
            data: "",
            async: false,
            cache: false,
            beforeSend: function (XMLHttpRequest) {

            },
            success: function (result) {
                var result = eval("(" + result + ")");
                if (result.result == "ok") {
                    reWriteMessagerAlert("操作提示", result.message, "info");
                    _$_datagrid.datagrid("reload");
                    return true;
                } else {
                    reWriteMessagerAlert("操作提示", result.message, "error");
                    _$_datagrid.datagrid("reload");
                    return false;
                }
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
        sortOrder: 'asc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        rowStyler: function (rowIndex, rowData) {
            var strRet = "";
            //singal:
            //0：未找到入库记录
            //1:入库异常(待处理)
            //2:入库异常(处理中)
            //3:入库异常(已处理)
            //4：已出库
            //5：出库异常(待处理)
            //6：出库异常(处理中)
            //7：出库异常(已处理)
            //8:未知
            //9:数据格式不正确
            //99:正常，可以出库,即已正常入库
            switch (rowData.singal) {
                case "99":
                    strRet = "background-color:#6699FF;";
                    break;
                case "0":
                    strRet = "background-color:#FF9966;";
                    break;
                case "9":
                    strRet = "background-color:#FF9966;";
                    break;
                default:
                    strRet = "background-color:Grey;";
            }
            return strRet;
        },
        columns: [[
					{ field: 'swbID', title: '序号', width: 30, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'wbSerialNum', title: '总运单编号', width: 100, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbSerialNum', title: '分运单号', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbDescription_CHN', title: '中文货物名称', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbDescription_ENG', title: '英文货物名称', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNumber', title: '件数', width: 40, sortable: true,
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
                    { field: 'swbMonetary', title: '币制', width: 50, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbRecipients', title: '收件人', width: 160, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbCustomsCategory', title: '报关类别', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'StatusDecription', title: '运单状态', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ExceptionStatusDescription', title: '说明', width: 230, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'ExceptionStatus', title: '说明', hidden: true, width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'singal', title: 'singal', hidden: true, width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'status', title: 'status', hidden: true, width: 120, sortable: true,
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

    //设置分页控件   
    var p = _$_datagrid.datagrid('getPager');
    $(p).pagination({
        pageSize: 15,
        pageList: [10, 15, 20, 25, 30]
    });

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "swbid":
                    break;
                case "singal":
                    break;
                case "status":
                    break;
                case "exceptionstatus":
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
