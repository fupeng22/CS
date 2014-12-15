﻿$(function () {
    var _$_datagrid = $("#DG_WayBillResult");
    var _$_ddCompany = $('#ddCompany');
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    var getSubWayBillSumInfoURL = "/ViewSubWayBillDetail/getSubWayBillSumInfo?wbID=";
    var getNeedCheckNumURL = "/Customer_Check/getNeedCheckNum?wbID=";
    var getSwbControlFlagURL = "/Customer_Check/getSwbControlFlag?wbID=";

    var DetailURL = "/Customer_Detail/Index?Detail_wbID=";
    var CheckURL = "/Customer_Check/upDateSwbNeedCheck";

    var DetailDlg = null;
    var DetailDlgForm = null;

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL = "";// "/Customer_Check/GetData?txtBeginDate=" + encodeURI($("#txtBeginDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val()) + "&ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val());

    $("#btnQuery").click(function () {
        QueryURL = "/Customer_Check/GetData?txtBeginDate=" + encodeURI($("#txtBeginDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val()) + "&ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#btnPrint").click(function () {
        PrintURL = "/Customer_Check/Print?txtBeginDate=" + encodeURI($("#txtBeginDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val()) + "&ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

    $("#btnExcel").click(function () {
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

        PrintURL = "/Customer_Check/Excel?txtBeginDate=" + encodeURI($("#txtBeginDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val()) + "&ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtCode=" + encodeURI($("#txtCode").val()) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'wbID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'wbID',
        columns: [[
					{ field: 'wbStorageDate', title: '导入日期', width: 80, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'wbCompany', title: '货代公司', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'wbSerialNum', title: '总运单号', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalNumber_Customize', title: '子运单总件数', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetSubWayBillSumInfo' type='swbTotalNumber' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'wbTotalWeight_Customize', title: '子运单总重量(公斤)', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetSubWayBillSumInfo' type='swbTotalWeight'  wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'wbStatus', title: '状态', width: 60, sortable: true,
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeWBStatus' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                    { field: 'swbControlFlag0', title: '无需布控', width: 70, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_SynchorizeGetControlFlag' type='swbControlFlag0'  wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    },
                     { field: 'swbControlFlag1', title: '海关布控', width: 70, sortable: true,
                         sorter: function (a, b) {
                             return (a > b ? 1 : -1);
                         },
                         formatter: function (value, rec, index) {
                             var strRet = "";
                             strRet = "<span class='cls_SynchorizeGetControlFlag' type='swbControlFlag1'  wbID='" + rec.wbID + "'></span>";
                             return strRet;
                         }
                     },
                      { field: 'swbControlFlag2', title: '检疫布控', width: 70, sortable: true,
                          sorter: function (a, b) {
                              return (a > b ? 1 : -1);
                          },
                          formatter: function (value, rec, index) {
                              var strRet = "";
                              strRet = "<span class='cls_SynchorizeGetControlFlag' type='swbControlFlag2'  wbID='" + rec.wbID + "'></span>";
                              return strRet;
                          }
                      },
                       { field: 'swbControlFlag3', title: '同时布控', width: 70, sortable: true,
                           sorter: function (a, b) {
                               return (a > b ? 1 : -1);
                           },
                           formatter: function (value, rec, index) {
                               var strRet = "";
                               strRet = "<span class='cls_SynchorizeGetControlFlag' type='swbControlFlag3'  wbID='" + rec.wbID + "'></span>";
                               return strRet;
                           }
                       },
                    { field: 'wbImportType', title: '数据来源', width: 120, sortable: true,
                        formatter: function (value, rec, index) {
                            var s = "";
                            if (rec.wbImportType == 0) {
                                s = "内部数据";
                            } else {
                                s = "海关同步";
                            }
                            return s;
                        }
                    },
                    { field: 'hiddenFields', title: '', hidden: true, width: 0, sortable: true,
                        formatter: function (value, rec, index) {
                            var strRet = "";
                            strRet = "<span class='cls_getSubWayBillSumInfo' wbID='" + rec.wbID + "'></span>";
                            return strRet;
                        }
                    }
				]],
        pagination: true,
        pageSize: 15,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
        toolbar: "#toolBar",
        onRowContextMenu: function (e, rowIndex, rowData) {
            e.preventDefault();
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);

            var cmenu = $('<div id="cmenu" style="width:100px;"></div>').appendTo('body');
            $('<div  id="mnuDetail" iconCls="icon-search"/>').html("查看子运单明细").appendTo(cmenu);
            cmenu.menu({
                onClick: function (item) {
                    cmenu.remove();
                    switch (item.id.toLowerCase()) {
                        case "mnudetail":
                            Detail();
                            break;
                    }
                }
            });

            $('#cmenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        },
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
        },
        onClickRow: function (rowIndex, rowData) {
            Detail(rowData.wbID, rowData.wbSerialNum, rowData.wbImportType);
        },
        onLoadSuccess: function (data) {
            var allSynchorizeGetSubWayBillSumInfoSpan = $(".cls_SynchorizeGetSubWayBillSumInfo[type='swbTotalNumber']");
            $.each(allSynchorizeGetSubWayBillSumInfoSpan, function (i, item) {
                var strRet = "";
                var wbID = $(item).attr("wbID");
                $.ajax({
                    type: "POST",
                    url: getSubWayBillSumInfoURL + encodeURI(wbID),
                    data: "",
                    async: true,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var msg = eval("(" + msg + ")");
                        if (msg.result == 'ok') {
                            strRet = msg.message;

                        } else {
                            strRet = "获取出错";
                        }
                        $(".cls_SynchorizeGetSubWayBillSumInfo[wbID=" + wbID + "][type='swbTotalNumber']").html(msg.swbTotalNumber.toString());
                        $(".cls_SynchorizeGetSubWayBillSumInfo[wbID=" + wbID + "][type='swbTotalWeight']").html(msg.swbTotalWeight.toString());
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });

                $.ajax({
                    type: "POST",
                    url: getNeedCheckNumURL + encodeURI(wbID),
                    data: "",
                    async: true,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var msg = eval("(" + msg + ")");
                        if (msg.result == 'ok') {
                            strRet = msg.message;

                        } else {
                            strRet = "获取出错";
                        }

                        var arr = msg.wbStatus.split("$");
                        var s = "";
                        if (arr.length == 2) {
                            var needCheck = arr[0];
                            var status = arr[1];
                            if (needCheck == "0") {
                                s = "<center><div style='background-color:#00CC66;width:60px'>" + needCheck + "</div></center>";
                            } else {
                                s = "<center><div style='background-color:#FF9966;width:60px'>" + needCheck + "</div></center>";
                            }
                        }
                        $(".cls_SynchorizeWBStatus[wbID=" + wbID + "]").html(s);
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });

                $.ajax({
                    type: "POST",
                    url: getSwbControlFlagURL + encodeURI(wbID),
                    data: "",
                    async: true,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var msg = eval("(" + msg + ")");
                        if (msg.result == 'ok') {
                            strRet = msg.message;

                        } else {
                            strRet = "获取出错";
                        }
                        $(".cls_SynchorizeGetControlFlag[wbID=" + wbID + "][type='swbControlFlag0']").html(msg.swbControlFlag0.toString());
                        $(".cls_SynchorizeGetControlFlag[wbID=" + wbID + "][type='swbControlFlag1']").html(msg.swbControlFlag1.toString());
                        $(".cls_SynchorizeGetControlFlag[wbID=" + wbID + "][type='swbControlFlag2']").html(msg.swbControlFlag2.toString());
                        $(".cls_SynchorizeGetControlFlag[wbID=" + wbID + "][type='swbControlFlag3']").html(msg.swbControlFlag3.toString());
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });
            });
        }
    });

    function Detail(id, wbSerialNum, wbImportTypeTmp) {
        var div_DetailDlg = self.parent.$("#dlg_GlobalDetail");
        if (id) {
            if (wbImportTypeTmp == 0) {
                DetailDlg = div_DetailDlg.dialog({
                    buttons: [{
                        text: '提  交',
                        iconCls: 'icon-ok',
                        handler: function () {
                            var hid_Checked_swbSerialNum = DetailDlg.find('#hid_Checked_swbSerialNum').val();
                            var hid_unChecked_swbSerialNum = DetailDlg.find('#hid_unChecked_swbSerialNum').val();
                            $.ajax({
                                type: "GET",
                                url: CheckURL + "?swID=" + encodeURI(id) + "&ids=" + encodeURI(hid_Checked_swbSerialNum) + "&swbNeedCheck=1",
                                data: "",
                                async: false,
                                cache: false,
                                beforeSend: function (XMLHttpRequest) {

                                },
                                success: function (msg) {
                                    var msg = eval("(" + msg + ")");
                                    if (msg.result == 'ok') {
                                        $.ajax({
                                            type: "GET",
                                            url: CheckURL + "?swID=" + encodeURI(id) + "&ids=" + encodeURI(hid_unChecked_swbSerialNum) + "&swbNeedCheck=0",
                                            data: "",
                                            async: false,
                                            cache: false,
                                            beforeSend: function (XMLHttpRequest) {

                                            },
                                            success: function (msg) {
                                                var msg = eval("(" + msg + ")");
                                                if (msg.result == 'ok') {
                                                    reWriteMessagerAlert('操作提示', msg.message, 'info');
                                                    //DetailDlg.dialog('close');
                                                    //_$_datagrid.datagrid("reload");
                                                    //_$_datagrid.datagrid("unselectAll");

                                                } else {
                                                    reWriteMessagerAlert('操作提示', msg.message, 'error');
                                                    return false;
                                                }
                                            },
                                            complete: function (XMLHttpRequest, textStatus) {

                                            },
                                            error: function () {

                                            }
                                        });
                                    } else {
                                        reWriteMessagerAlert('操作提示', msg.message, 'error');
                                        return false;
                                    }
                                },
                                complete: function (XMLHttpRequest, textStatus) {

                                },
                                error: function () {

                                }
                            });
                        }
                    }, {
                        text: '关 闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            DetailDlg.dialog('close');
                        }
                    }],
                    title: '布        控',
                    href: DetailURL + id + "&Detail_wbSerialNum=" + wbSerialNum + "&wbImportType=" + encodeURI(wbImportTypeTmp),
                    modal: true,
                    resizable: true,
                    cache: false,
                    left: 0,
                    top: 0,
                    width: 830,
                    height: 460,
                    closed: true
                });
                _$_datagrid.datagrid("unselectAll");
            } else {
                DetailDlg = div_DetailDlg.dialog({
                    buttons: [{
                        text: '关 闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            DetailDlg.dialog('close');
                        }
                    }],
                    title: '布        控',
                    href: DetailURL + id + "&Detail_wbSerialNum=" + wbSerialNum + "&wbImportType=" + encodeURI(wbImportTypeTmp),
                    modal: true,
                    resizable: true,
                    cache: false,
                    left: 0,
                    top: 0,
                    width: 830,
                    height: 460,
                    closed: true
                });
                _$_datagrid.datagrid("unselectAll");
            }

        } else {
            var selects = _$_datagrid.datagrid("getSelections");
            if (selects.length != 1) {
                reWriteMessagerAlert("提示", "<center>请选择数据进行查看(<font style='color:red'>只可查看一行</font>)</center>", "error");
                return false;
            } else {
                id = selects[0].wbID;
                var wbSerialNum = selects[0].wbSerialNum;
                var wbImportType = selects[0].wbImportType;
                if (wbImportType == 0) {
                    DetailDlg = div_DetailDlg.dialog({
                        buttons: [{
                            text: '提  交',
                            iconCls: 'icon-ok',
                            handler: function () {
                                var hid_Checked_swbSerialNum = DetailDlg.find('#hid_Checked_swbSerialNum').val();
                                var hid_unChecked_swbSerialNum = DetailDlg.find('#hid_unChecked_swbSerialNum').val();
                                $.ajax({
                                    type: "GET",
                                    url: CheckURL + "?swID=" + encodeURI(id) + "&ids=" + encodeURI(hid_Checked_swbSerialNum) + "&swbNeedCheck=0",
                                    data: "",
                                    async: false,
                                    cache: false,
                                    beforeSend: function (XMLHttpRequest) {

                                    },
                                    success: function (msg) {
                                        var msg = eval("(" + msg + ")");
                                        if (msg.result == 'ok') {
                                            $.ajax({
                                                type: "GET",
                                                url: CheckURL + "?swID=" + encodeURI(id) + "&ids=" + encodeURI(hid_unChecked_swbSerialNum) + "&swbNeedCheck=0",
                                                data: "",
                                                async: false,
                                                cache: false,
                                                beforeSend: function (XMLHttpRequest) {

                                                },
                                                success: function (msg) {
                                                    var msg = eval("(" + msg + ")");
                                                    if (msg.result == 'ok') {
                                                        reWriteMessagerAlert('操作提示', msg.message, 'info');
                                                        //DetailDlg.dialog('close');
                                                        //_$_datagrid.datagrid("reload");
                                                        //_$_datagrid.datagrid("unselectAll");

                                                    } else {
                                                        reWriteMessagerAlert('操作提示', msg.message, 'error');
                                                        return false;
                                                    }
                                                },
                                                complete: function (XMLHttpRequest, textStatus) {

                                                },
                                                error: function () {

                                                }
                                            });

                                        } else {
                                            reWriteMessagerAlert('操作提示', msg.message, 'error');
                                            return false;
                                        }
                                    },
                                    complete: function (XMLHttpRequest, textStatus) {

                                    },
                                    error: function () {

                                    }
                                });
                            }
                        }, {
                            text: '关 闭',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                DetailDlg.dialog('close');
                            }
                        }],
                        title: '布        控',
                        href: DetailURL + id + "&Detail_wbSerialNum=" + wbSerialNum + "&wbImportType=" + encodeURI(wbImportTypeTmp),
                        modal: true,
                        resizable: true,
                        cache: false,
                        left: 0,
                        top: 0,
                        width: 830,
                        height: 460,
                        closed: true
                    });
                    _$_datagrid.datagrid("unselectAll");
                } else {
                    DetailDlg = div_DetailDlg.dialog({
                        buttons: [{
                            text: '关 闭',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                DetailDlg.dialog('close');
                            }
                        }],
                        title: '布        控',
                        href: DetailURL + id + "&Detail_wbSerialNum=" + wbSerialNum + "&wbImportType=" + encodeURI(wbImportTypeTmp),
                        modal: true,
                        resizable: true,
                        cache: false,
                        left: 0,
                        top: 0,
                        width: 830,
                        height: 460,
                        closed: true
                    });
                    _$_datagrid.datagrid("unselectAll");
                }

            }
        }

        div_DetailDlg.dialog("open");
        //console.info(DetailDlg.find('#txtSubWayBillCode'));
        if (DetailDlg.find('#txtSubWayBillCode')) {
            DetailDlg.find('#txtSubWayBillCode').focus();
        }

        //        $('#dlg_Detail').find("#txtGlobalTextInput").focus();

        //        // ESC\Enter方式关闭dialog
        //        $('#dlg_Detail').bind('keyup', function (event) {
        //            if (event.keyCode == '27' || event.keyCode == '13') {
        //                msgAlertDlg.dialog('close');
        //            }
        //        });
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:150px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "wbserialNum":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "wbSerialNum") {

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
