$(function () {
    var _$_datagrid = $("#DG_MainQueryResult");
    var _$_ddCompany = $('#txtVoyage');
    var _$_ddlInOutStoreType = $("#ddlInOutStoreType");
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";

    //var Detail_RejectSubWayURL = "/ViewRejectSubWayBillDetail/Index?Detail_wbSerialNum=";
    var Detail_RejectSubWayURL = "/ViewSubWayBillDetail/Index?Detail_bEnableReject=0&Detail_swbStatus=99&Detail_wbSerialNum=";

    _$_ddlInOutStoreType.combotree('loadData', [
    {
        id: -99,
        text: '---请选择(可多选)---'
    },
    {
        id: 1,
        text: '正常已入库'
    },
    {
        id: 3,
        text: '正常已出库'
    }]);

    _$_ddlInOutStoreType.combotree("setValue", "-99");

    var PrintSubWayBill = "";
    var LoadSubWayBillURL ="";// "/Huayu_QueryMainFrame/GetSubWayBill?txtWBID=";

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    $('#ddlCheckStatus').combobox({
        data: [{ "text": "---请选择---", "id": "---请选择---" }, { "text": "放行", "id": "0" }, { "text": "等待预检", "id": "1" }, { "text": "查验放行", "id": "2" }, { "text": "查验扣留", "id": "3" }, { "text": "查验待处理", "id": "4"}, { "text": "退货", "id": "99"}],
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    //    $('#ddlInOutStoreType').combobox({
    //        data: [{ "text": "---请选择---", "id": "---请选择---" }, { "text": "正常已入库", "id": "1" }, { "text": "正常已出库", "id": "3"}],
    //        valueField: 'id',
    //        textField: 'text',
    //        editable: false,
    //        panelHeight: null
    //    });

    $('#txtNeedCheckStatus').combobox({
        data: [{ "text": "---请选择---", "id": "---请选择---" }, { "text": "放行", "id": "0" }, { "text": "等待预检", "id": "1" }, { "text": "查验放行", "id": "2" }, { "text": "查验扣留", "id": "3" }, { "text": "查验待处理", "id": "4"}, { "text": "退货", "id": "99"}],
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    //$('#ddlInOutStoreType').combobox("setValue", "---请选择---");
    $('#ddlCheckStatus').combobox("setValue", "---请选择---");
    _$_ddCompany.combobox("setValue", "---请选择---");

    var PrintURL = "";
    var QueryURL ="";// "/Huayu_QueryMainFrame/GetData?inputBeginDate=" + encodeURI($("#txtBeginD").val()) + "&inputEndDate=" + encodeURI($("#txtEndD").val()) + "&txtCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtWBSerialNum=" + encodeURI($("#txtCode").val()) + "&txtSWBSerialNum=" + encodeURI($("#txtSubWayBillCode").val() + "&txtSWBNeedCheck=" + encodeURI($('#ddlCheckStatus').combobox("getValue")) + "&InOutStoreType=" + encodeURI(_$_ddlInOutStoreType.combotree("getValues").join(',')));

    $("#btnQuery").click(function () {
        QueryURL = "/Huayu_QueryMainFrame/GetData?inputBeginDate=" + encodeURI($("#txtBeginD").val()) + "&inputEndDate=" + encodeURI($("#txtEndD").val()) + "&txtCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtWBSerialNum=" + encodeURI($("#txtCode").val()) + "&txtSWBSerialNum=" + encodeURI($("#txtSubWayBillCode").val() + "&txtSWBNeedCheck=" + encodeURI($('#ddlCheckStatus').combobox("getValue")) + "&InOutStoreType=" + encodeURI(_$_ddlInOutStoreType.combotree("getValues").join(',')));
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 20); //延迟100毫秒执行，时间可以更短
    });

    $("#btnReset").click(function () {
        $("#txtBeginD").val("");
        $("#txtEndD").val("");
        $("#txtCode").val("");
        $("#txtSubWayBillCode").val("");
        _$_ddCompany.combobox("setValue", "---请选择---");
        $('#ddlCheckStatus').combobox("setValue", "---请选择---");
        _$_ddlInOutStoreType.combotree("setValue", "-99");
        $('#txtNeedCheckStatus').combotree("setValue", "-99");
        $("#btnQuery").click();
    });

    $("#btnPrint").click(function () {
        PrintURL = "/Huayu_QueryMainFrame/PrintWayBillInfo?inputBeginDate=" + encodeURI($("#txtBeginD").val()) + "&inputEndDate=" + encodeURI($("#txtEndD").val()) + "&txtCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtWBSerialNum=" + encodeURI($("#txtCode").val()) + "&txtSWBSerialNum=" + encodeURI($("#txtSubWayBillCode").val() + "&txtSWBNeedCheck=" + encodeURI($('#ddlCheckStatus').combobox("getValue")) + "&InOutStoreType=" + encodeURI(_$_ddlInOutStoreType.combotree("getValues").join(','))) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            var div_PrintDlg = self.parent.$("#dlg_GlobalPrint");
            div_PrintDlg.show();
            var PrintDlg = null;

//           div_PrintDlg.find("#p").show();
//            div_PrintDlg.find("#frmPrintURL").load(function(){
//                div_PrintDlg.find("#p").hide();
//            });
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

        PrintURL = "/Huayu_QueryMainFrame/ExcelWayBillInfo?inputBeginDate=" + encodeURI($("#txtBeginD").val()) + "&inputEndDate=" + encodeURI($("#txtEndD").val()) + "&txtCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtWBSerialNum=" + encodeURI($("#txtCode").val()) + "&txtSWBSerialNum=" + encodeURI($("#txtSubWayBillCode").val() + "&txtSWBNeedCheck=" + encodeURI($('#ddlCheckStatus').combobox("getValue")) + "&InOutStoreType=" + encodeURI(_$_ddlInOutStoreType.combotree("getValues").join(','))) + "&order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
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
        idField: 'wb_swb_ID',
        view: detailview,
        singleSelect: true,
        showFooter: true,
        detailFormatter: function (index, row) {
            //return '<div style="padding:2px"><center><span style="color:red;font-weight:bold">[<span id="WayBillSerialNum' + index + '"></span>]子运单明细</span></center><table  id="ddv-' + index + '"></table></div>';
            //return '<div style="padding:2px"><table  id="ddv-' + index + '"></table></div>';
            return '<div style="padding:2px"><center><span style="color:red;font-weight:bold">[<span id="WayBillSerialNum' + index + '"></span>]子运单明细</span></center><table  id="ddv-' + index + '"></table></div></br>';
        },
        columns: [[
                    { field: 'wbSerialNum', title: '总运单号', width: 150
                        //                    ,
                        //                        formatter: function (value, row, index) {
                        //                            return "<a href='#' class='link_wbSerialNum' wbSerialNum='" + row.wbSerialNum + "'>" + row.wbSerialNum + "</a>";
                        //                        }
                    },
					{ field: 'wbCompany', title: '货代公司', width: 200
					},
					{ field: 'wbTotalNumbe', title: '总运单件数', width: 120, align: "center",
					    formatter: function (value, rowData, rowIndex) {
                            if (rowData.wbTotalNumbe.indexOf("wbTotalNumbe")>-1) 
                            {
                                    return rowData.wbTotalNumbe.replace("wbTotalNumbe","");
                            }else{
                                if (rowData.wbTotalNumbe!="0") {
                                    return "<center><a href='#' class='load_InOutStoreSubWayBill' wbID='" + rowData.wbID + "' InOutType='-99' rowIndex='" + rowIndex + "'>" + rowData.wbTotalNumbe + "</a></center>";
                                }else{
                                    return rowData.wbTotalNumbe;
                                }
                                
                            }
					        
					    }
					},
                    { field: 'wbTotalWeight', title: '总运单重量', width: 120, align:"center"
                    },
					{ field: 'InStoreCount', title: '已入库分单数', width: 110,align: "center",
					    formatter: function (value, rowData, rowIndex) {
                            if (rowData.InStoreCount.indexOf("InStoreCount")>-1) 
                            {
                                    return rowData.InStoreCount.replace("InStoreCount","");
                            }else{
                                if (rowData.InStoreCount!="0") {
                                    return "<center><a href='#' class='load_InOutStoreSubWayBill' wbID='" + rowData.wbID + "' InOutType='1' rowIndex='" + rowIndex + "'>" + rowData.InStoreCount + "</a></center>";
                                }else{
                                    return rowData.InStoreCount;
                                }
                                
                            }
					        
					    }
					},
                    { field: 'OutStoreCount', title: '已出库分单数', width: 110,  align: "center",
                        formatter: function (value, rowData, rowIndex) {
                        if (rowData.OutStoreCount.indexOf("OutStoreCount")>-1) 
                            {
                                    return rowData.OutStoreCount.replace("OutStoreCount","");
                            }else{
                            if (rowData.OutStoreCount!="0") {
                                   return "<center><a href='#' class='load_InOutStoreSubWayBill' wbID='" + rowData.wbID + "' InOutType='3' rowIndex='" + rowIndex + "'>" + rowData.OutStoreCount + "</a></center>";
                                }else{
                                   return rowData.OutStoreCount ;
                                }
                            
                            }
                        }
                    },
                    { field: 'StoreCount', title: '库存件数', width: 100, align: "center",
                        formatter: function (value, rowData, rowIndex) {
                        if (rowData.StoreCount.indexOf("StoreCount")>-1) 
                            {
                                    return rowData.StoreCount.replace("StoreCount","");
                            }else{
                            if (rowData.StoreCount!="0") {
                                   return "<center><a href='#' class='load_InOutStoreSubWayBill' wbID='" + rowData.wbID + "' InOutType='-1' rowIndex='" + rowIndex + "'>" + rowData.StoreCount + "</a></center>";
                                }else{
                                   return rowData.StoreCount;
                                }
                            
                            }
                        }
                    },
                    { field: 'NotInStoreCount', title: '未入库件数', width: 110,  align: "center",
                        formatter: function (value, rowData, rowIndex) {
                        if (rowData.NotInStoreCount.indexOf("NotInStoreCount")>-1) 
                            {
                                    return rowData.NotInStoreCount.replace("NotInStoreCount","");
                            }else{
                            if (rowData.NotInStoreCount!="0") {
                                   return "<center><a href='#' class='load_InOutStoreSubWayBill' wbID='" + rowData.wbID + "' InOutType='99' rowIndex='" + rowIndex + "'>" + rowData.NotInStoreCount + "</a></center>";
                                }else{
                                   return  rowData.NotInStoreCount;
                                }
                            
                            }
                        }
                    },
                    { field: 'RejectNum', title: '退运件数', width: 110,  align: "center",
                        formatter: function (value, rowData, rowIndex) {
                        if (rowData.RejectNum.indexOf("RejectNum")>-1) 
                            {
                                    return rowData.RejectNum.replace("RejectNum","");
                            }else{
                            if (rowData.RejectNum!="0") {
                                   return "<center><a href='#' class='load_RejectSubWayBill' wbID='" + rowData.wbID + "' wbSerialNum='"+rowData.wbSerialNum+"'>" + rowData.RejectNum + "</a></center>";
                                }else{
                                   return rowData.RejectNum;
                                }
                            
                            }
                        }
                    },
                    { field: 'wbID', title: '', hidden: true, width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    }
				]],
        pagination: true,
        pageSize: 20,
        pageList: [ 20, 25, 30, 35, 40, 45, 50],
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
            _$_datagrid.datagrid("reload");
        },
        onLoadSuccess: function (data) {
            delete $(this).datagrid('options').queryParams['id'];
            if ($("#txtSubWayBillCode").val() != "") {
                for (var i = 0; i < data.rows.length; i++) {
                    _$_datagrid.datagrid("expandRow", i);
                }
            }

            var allViewInOutStoreLnk = $(".load_InOutStoreSubWayBill");
            var allViewRejectSubWayBillLnk = $(".load_RejectSubWayBill");
//            $.each(allViewInOutStoreLnk, function (i, item) {
//                $(item).click(function () {
//                    var _$_wbID = $(item).attr("wbID");
//                    var _$_InOutType = $(item).attr("InOutType");
//                    var _$_rowIndex = $(item).attr("rowIndex");

//                    var _$_subDataGrid = $('#ddv-' + _$_rowIndex);
//                    $("#WayBillSerialNum" + _$_rowIndex).html(data.rows[_$_rowIndex].wbSerialNum);

//                    var expander = _$_datagrid.datagrid('getExpander', _$_rowIndex);
////                    if (expander.hasClass('datagrid-row-collapse')) {
////                        _$_datagrid.datagrid("collapseRow",  _$_rowIndex);
////                    } else {
//                        _$_datagrid.datagrid("expandRow", _$_rowIndex);
//                        window.setTimeout(function () {
//                            $.extend(_$_subDataGrid.datagrid("options"), {
//                                url: LoadSubWayBillURL + _$_wbID + "&InOutType="+_$_InOutType+"&strSwbSerialNum=",
//                            });
//                            _$_subDataGrid.datagrid("reload");
//                        },10);
////                    }

//                });
//            });

            $.each(allViewInOutStoreLnk, function (i, item) {
                $(item).click(function () {
                    var _$_wbID = $(item).attr("wbID");
                    var _$_InOutType = $(item).attr("InOutType");
                    var _$_rowIndex = $(item).attr("rowIndex");

                    var _$_subDataGrid = $('#ddv-' + _$_rowIndex);
                    $("#WayBillSerialNum" + _$_rowIndex).html(data.rows[_$_rowIndex].wbSerialNum);

                    var expander = _$_datagrid.datagrid('getExpander', _$_rowIndex);
                    window.setTimeout(function () {
                        $.extend(_$_subDataGrid.datagrid("options"), {
                            url: LoadSubWayBillURL + _$_wbID + "&InOutType="+_$_InOutType+"&strSwbSerialNum=",
                        });
                        _$_subDataGrid.datagrid("reload");
                    },10);
                   _$_datagrid.datagrid("expandRow", _$_rowIndex);
                });
            });

            $.each(allViewRejectSubWayBillLnk, function (i, item) {
                $(item).click(function () {
                    var _$_wbSerialNum = $(item).attr("wbSerialNum");
                   Detail_RejectSubWayBill(_$_wbSerialNum);
                });
            });
        },
        onClickRow: function (index, row) {
            var expander = _$_datagrid.datagrid('getExpander', index);
            if (expander.hasClass('datagrid-row-collapse')) {
                _$_datagrid.datagrid("collapseRow", index);
            } else {
                _$_datagrid.datagrid("expandRow", index);
            }
        },
        onExpandRow: function (index, row) {
            var _$_subDataGrid = $('#ddv-' + index);
            $("#WayBillSerialNum" + index).html(row.wbSerialNum);
            _$_subDataGrid.datagrid({
                url: LoadSubWayBillURL + row.wbID + "&InOutType=-99&strSwbSerialNum=" + encodeURI($("#txtSubWayBillCode").val()),
                fitColumns: true,
                //singleSelect: true,
                //rownumbers: true,
                loadMsg: '',
                height: 'auto',
                iconCls: 'icon-save',
                nowrap: true,
                autoRowHeight: false,
                autoRowWidth: false,
                striped: true,
                collapsible: true,
                sortName: 'swbID',
                sortOrder: 'desc',
                remoteSort: true,
                border: true,
                idField: 'swbID',
                columns: [[
                    { field: 'swbSerialNum', title: '分运单号', width: 170, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },{ field: 'swbDescription_CHN', title: '货物中文名', width: 200, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},{ field: 'swbDescription_ENG', title: '货物英文名', width: 200, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'swbNumber', title: '件数', width: 60, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'swbWeight', title: '重量', width: 80, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'FinalStatusDecription', title: '出入库类型', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'InOutStoreDate', title: '出入库日期', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'InOutStoreOperator', title: '出入库操作员', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNeedCheckDescription', title: '海关预检状态', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbID', title: '', hidden: true, width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    }
				]],
                pagination: true,
                pageSize: 20,
                pageList: [ 20, 25, 30, 35, 40, 45, 50],
                toolbar: [{
                    id: 'btnPrint',
                    text: '打印',
                    disabled: false,
                    iconCls: 'icon-print',
                    handler: function () {
                    var _$_url=_$_subDataGrid.datagrid('options').url;
                     PrintSubWayBillInfo(_$_subDataGrid, row.wbID,_$_url.substring(_$_url.indexOf("InOutType"),_$_url.length).split("&")[0].split("=")[1]);
                    }
                }, '-', {
                    id: 'btnExcel',
                    text: '导出',
                    disabled: false,
                    iconCls: 'icon-excel',
                    handler: function () {
                    var _$_url=_$_subDataGrid.datagrid('options').url;
                        ExcelSubWayBillInfo(_$_subDataGrid, row.wbID,_$_url.substring(_$_url.indexOf("InOutType"),_$_url.length).split("&")[0].split("=")[1]);
                    }
                }],
                onResize: function () {
                    _$_datagrid.datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        _$_datagrid.datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });
            _$_datagrid.datagrid('fixDetailRowHeight', index);
        }
    });

     function Detail_RejectSubWayBill(wbSerialNum) {
       var div_DetailDlg = self.parent.$("#dlg_GlobalDetail");
        if (wbSerialNum) {
            DG_ViewRejectSubWayBillDetailResult = div_DetailDlg.dialog({
                buttons: [{
                    text: '关 闭',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        DG_ViewRejectSubWayBillDetailResult.dialog('close');
                    }
                }],
                title: '查看退货子运单明细',
                href: Detail_RejectSubWayURL + wbSerialNum,
                modal: true,
                resizable: true,
                cache: false,
                left: 0,
                top: 0,
                width: 1020,
                height: 480,
                closed: true
            });
            _$_datagrid.datagrid("unselectAll");
        } else {
            var selects = _$_datagrid.datagrid("getSelections");
            if (selects.length != 1) {
                reWriteMessagerAlert("提示", "<center>请选择数据进行查看(<font style='color:red'>只可查看一行</font>)</center>", "error");
                return false;
            } else {
                var wbSerialNum = selects[0].wbSerialNum;
                DG_ViewRejectSubWayBillDetailResult = div_DetailDlg.dialog({
                    buttons: [{
                        text: '关 闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            DG_ViewRejectSubWayBillDetailResult.dialog('close');
                        }
                    }],
                    title: '查看退货子运单明细',
                    href: Detail_RejectSubWayURL + wbSerialNum,
                    modal: true,
                    resizable: true,
                    cache: false,
                    left: 0,
                    top: 0,
                    width: 1020,
                    height: 480,
                    closed: true
                });
                _$_datagrid.datagrid("unselectAll");
            }
        }

        div_DetailDlg.dialog("open");
    }

    function PrintSubWayBillInfo(dg, wbID,InOutType) {
        PrintSubWayBill = "/Huayu_QueryMainFrame/PrintSubWayBillInfo?txtWBID=" + encodeURI(wbID) +"&InOutType="+InOutType+ "&strSwbSerialNum=" + encodeURI($("#txtSubWayBillCode").val()) + "&order=" + dg.datagrid("options").sortOrder + "&sort=" + dg.datagrid("options").sortName + "&page=1&rows=10000000";
        if (dg.datagrid("getData").rows.length > 0) {
            var div_PrintDlg = self.parent.$("#dlg_GlobalPrint");
            div_PrintDlg.show();
            var PrintDlg = null;
            div_PrintDlg.find("#frmPrintURL").attr("src", PrintSubWayBill);
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
    }

    function ExcelSubWayBillInfo(dg, wbID,InOutType) {
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

        PrintURL = "/Huayu_QueryMainFrame/ExcelSubWayBillInfo?txtWBID=" + encodeURI(wbID) +"&InOutType="+InOutType+ "&strSwbSerialNum=" + encodeURI($("#txtSubWayBillCode").val()) + "&order=" + dg.datagrid("options").sortOrder + "&sort=" + dg.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (dg.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:200px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "wbserialnum":
                    break;
                case "wbid":
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
