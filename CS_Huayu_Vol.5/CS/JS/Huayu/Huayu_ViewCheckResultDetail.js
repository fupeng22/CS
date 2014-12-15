$(function () {
    var _$_datagrid = $("#DG_SubWayBillDetail");
    var PrintURL = "";
    var _$_ddlSwbStatus = $("#ddlSwbStatus");
    var _$_ddlSortingTimes = $("#ddlSortingTimes");
    var LoadStatusJSONUrl = "/CustomerMain/CreateStatusJSON";

    _$_ddlSwbStatus.combotree({
        url: LoadStatusJSONUrl,
        valueField: 'id',
        textField: 'text',
        onLoadSuccess: function () {
            switch ($("#hid_swbStatus").val()) {
                case "":
                    _$_ddlSwbStatus.combotree("setValues", ["-99"]);
                    break;
                default:
                    _$_ddlSwbStatus.combotree("setValues", eval("[" + $("#hid_swbStatus").val() + "]"));
                    break;
            }
        }
    });

    switch ($("#hid_swbStatus").val()) {
        case "":
            _$_ddlSwbStatus.combotree("setValues", ["-99"]);
            break;
        default:
            _$_ddlSwbStatus.combotree("setValues", eval("[" + $("#hid_swbStatus").val() + "]"));
            break;
    }

    var QueryURL = "/Huayu_ViewCheckResultDetail/GetData?Detail_wbSerialNum=" + encodeURI($("#txtCode_Detail").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_Detail").val()) + "&txtswbDescription_CHN=" + encodeURI($("#txtswbDescription_CHN").val()) + "&txtswbDescription_ENG=" + encodeURI($("#txtswbDescription_ENG").val()) + "&txtSwbStatus=" + encodeURI(_$_ddlSwbStatus.combotree("getValues").join(','));

    $("#btnDetailQuery").click(function () {
        QueryURL = "/Huayu_ViewCheckResultDetail/GetData?Detail_wbSerialNum=" + encodeURI($("#txtCode_Detail").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_Detail").val()) + "&txtswbDescription_CHN=" + encodeURI($("#txtswbDescription_CHN").val()) + "&txtswbDescription_ENG=" + encodeURI($("#txtswbDescription_ENG").val()) + "&txtSwbStatus=" + encodeURI(_$_ddlSwbStatus.combotree("getValues").join(','));
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnPrint_dlg").click(function () {
        PrintURL = "/Huayu_ViewCheckResultDetail/Print?Detail_wbSerialNum=" + encodeURI($("#txtCode_Detail").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_Detail").val()) + "&txtswbDescription_CHN=" + encodeURI($("#txtswbDescription_CHN").val()) + "&txtswbDescription_ENG=" + encodeURI($("#txtswbDescription_ENG").val()) + "&txtSwbStatus=" + encodeURI(_$_ddlSwbStatus.combotree("getValues").join(',')) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
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

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可打印", "error");
            return false;
        }

    });

    $("#btnExcel_dlg").click(function () {
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

        PrintURL = "/Huayu_ViewCheckResultDetail/Excel?Detail_wbSerialNum=" + encodeURI($("#txtCode_Detail").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode_Detail").val()) + "&txtswbDescription_CHN=" + encodeURI($("#txtswbDescription_CHN").val()) + "&txtswbDescription_ENG=" + encodeURI($("#txtswbDescription_ENG").val()) + "&txtSwbStatus=" + encodeURI(_$_ddlSwbStatus.combotree("getValues").join(',')) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    });

    _$_datagrid.datagrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'swbID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        columns: [[
                    { field: 'wbSerialNum', title: '总运单号', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbCompany', title: '货代公司', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbStorageDate', title: '报关日期', width: 60, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
					{ field: 'swbSerialNum', title: '分运单号', width: 100, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'swbDescription_CHN', title: '货物中文名', width: 160, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbDescription_ENG', title: '货物英文名', width: 140, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNumber', title: '件数', width: 40, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbWeight', title: '重量(公斤)', width: 70, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
					{ field: 'swbNeedCheckDescription', title: '状态', width: 80, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbControlFlag_Des', title: '布控状态', width: 80, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbCheckFlag_Custom_Des', title: '海关查验结果', width: 80, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbCheckFlag_Quarantine_Des', title: '检疫查验结果', width: 80, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'swbID', title: '主键', hidden: true, width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					}
				]],
        pagination: true,
        pageSize: 15,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
        toolbar: "#toolBarDetail",
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

        }
    });

    function SeleAll() {
        var rows = _$_datagrid.datagrid("getRows");
        for (var i = 0; i < rows.length; i++) {
            var m = _$_datagrid.datagrid("getRowIndex", rows[i]);
            if (rows[i].wbID == "0" && rows[i].swbID == "0") {
                _$_datagrid.datagrid("uncheckRow", i);
                _$_datagrid.datagrid("unselectRow", i);
            } else {
                _$_datagrid.datagrid("selectRow", m)
            }
        }
    }

    function InverseSele() {
        var rows = _$_datagrid.datagrid("getRows");
        var selects = _$_datagrid.datagrid("getSelections");
        for (var i = 0; i < rows.length; i++) {
            var bSele = false;
            var m = _$_datagrid.datagrid("getRowIndex", rows[i]);
            for (var j = 0; j < selects.length; j++) {
                var n = _$_datagrid.datagrid("getRowIndex", selects[j]);
                if (m == n) {
                    bSele = true;
                }
            }
            if (bSele) {
                _$_datagrid.datagrid("unselectRow", m)
            } else {
                if (rows[i].wbID == "0" && rows[i].swbID == "0") {
                    _$_datagrid.datagrid("uncheckRow", i);
                    _$_datagrid.datagrid("unselectRow", i);
                } else {
                    _$_datagrid.datagrid("selectRow", m)
                }

            }
        }
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:150px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "swbserialnum":
                    break;
                case "swbid":
                    break;
                case "cb":
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
