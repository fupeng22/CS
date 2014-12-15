var bEnableCheck = false;
if ($("#hid_wbImportType").val() == "0") {
    bEnableCheck = true;
} else {
    bEnableCheck = false;
}

function Append_hid_Checked_swbSerialNum(v) {
    if ($("#hid_unChecked_swbSerialNum").val() != "") {
        var hid_unChecked_swbSerialNum_val = $("#hid_unChecked_swbSerialNum").val().split("*");
        var hid_unChecked_swbSerialNum_newVal = "";
        for (var i = 0; i < hid_unChecked_swbSerialNum_val.length; i++) {
            if (hid_unChecked_swbSerialNum_val[i] != "") {
                if (hid_unChecked_swbSerialNum_val[i] == v) {

                } else {
                    hid_unChecked_swbSerialNum_newVal = hid_unChecked_swbSerialNum_newVal + hid_unChecked_swbSerialNum_val[i] + "*";
                }
            }

        }
        $("#hid_unChecked_swbSerialNum").val(hid_unChecked_swbSerialNum_newVal);
    }

    var hid_Checked_swbSerialNum_val = $("#hid_Checked_swbSerialNum").val().split("*");
    var bExist = false;
    for (var i = 0; i < hid_Checked_swbSerialNum_val.length; i++) {
        if (hid_Checked_swbSerialNum_val[i] == v) {
            bExist = true;
        }
    }
    if (!bExist) {
        $("#hid_Checked_swbSerialNum").val($("#hid_Checked_swbSerialNum").val() + v + "*");
    }
}

function Append_hid_unChecked_swbSerialNum(v) {
    if ($("#hid_Checked_swbSerialNum").val() != "") {
        var hid_Checked_swbSerialNum_val = $("#hid_Checked_swbSerialNum").val().split("*");
        var hid_Checked_swbSerialNum_newVal = "";
        for (var i = 0; i < hid_Checked_swbSerialNum_val.length; i++) {
            if (hid_Checked_swbSerialNum_val[i] != "") {
                if (hid_Checked_swbSerialNum_val[i] == v) {

                } else {
                    hid_Checked_swbSerialNum_newVal = hid_Checked_swbSerialNum_newVal + hid_Checked_swbSerialNum_val[i] + "*";
                }
            }
        }
        $("#hid_Checked_swbSerialNum").val(hid_Checked_swbSerialNum_newVal);

    }

    var hid_unChecked_swbSerialNum_val = $("#hid_unChecked_swbSerialNum").val().split("*");
    var bExist = false;
    for (var i = 0; i < hid_unChecked_swbSerialNum.length; i++) {
        if (hid_unChecked_swbSerialNum[i] == v) {
            bExist = true;
        }
    }
    if (!bExist) {
        $("#hid_unChecked_swbSerialNum").val($("#hid_unChecked_swbSerialNum").val() + v + "*");
    }
}

$(function () {
    var _$_datagrid = $("#DG_SubWayBillDetail");
    var QueryURL = "/Customer_Detail/GetData?Detail_wbID=" + encodeURI($("#hid_wbID").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode").val());

    $("#btnQuery_Detail").click(function () {
        QueryURL = "/Customer_Detail/GetData?Detail_wbID=" + encodeURI($("#hid_wbID").val()) + "&Detail_swbSerialNum=" + encodeURI($("#txtSubWayBillCode").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短
    });

    $("#txtSubWayBillCode").focus();

    $("#hid_Checked_swbSerialNum").val("");
    $("#hid_unChecked_swbSerialNum").val("");

    _$_datagrid.datagrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'swbID',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'swbID',
        rowStyler: function (rowIndex, rowData) {
            //任务完成100%， 并且已审核通过，背景色置灰 
            if (rowData.chkNeedCheck == "1") {
                return 'background-color:#FF9966;';
            }
            //            if (rowData && rowData.status && (rowData.status == 'TASK_ASSIGNER_AUDITED' || rowData.status == 'TASK_MONITOR_AUDITED') && rowData.finishRate == 100) {
            //                return 'background-color:#d9d9c2;';
            //            }
        },
        columns: [[
                    { field: 'cb', title: '', checkbox: true, width: 40, align: 'center'
                    },
					{ field: 'swbSerialNum', title: '分运单号', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'differentiateColor', title: '布控状态', width: 60, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            var sRet = "";
                            console.info(rowData.swbControlFlag);
                            switch (rowData.swbControlFlag) {
                                case "0":

                                    break;
                                case "1":
                                    sRet = "<span type='swbControlFlag1' strID=" + rowData.swbID + " class='cls_swbControlFlag' font_color='Green' style='width:40px;background-color:Green'>&nbsp;</span>&nbsp;&nbsp;";
                                    break;
                                case "2":
                                    sRet = "<span type='swbControlFlag2' strID=" + rowData.swbID + " class='cls_swbControlFlag' font_color='Blue' style='width:40px;background-color:Blue'>&nbsp;</span>&nbsp;&nbsp;";
                                    break;
                                case "3":
                                    sRet = "<span type='swbControlFlag3' strID=" + rowData.swbID + " class='cls_swbControlFlag' font_color='Red' style='width:40px;background-color:Red'>&nbsp;</span>&nbsp;&nbsp;";
                                    break;
                                default:
                                    break;
                            }
                            return sRet;
                        }
                    },
					{ field: 'swbDescription_CHN', title: '货物中文名', width: 400, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'swbDescription_ENG', title: '货物英文名', width: 160, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbNumber', title: '件数', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'swbWeight', title: '重量(公斤)', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
					{ field: 'chkNeedCheck', title: '是否需要预检', hidden: true, width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'currentSele', title: '<font style="color:red;">当前所选行</font>', width: 120,
                        formatter: function (value, rowData, rowIndex) {
                            return "<span class='currentRows' select='0' currentRowSwbSerialNum='" + rowData.swbSerialNum + "'></span>";
                        }
                    },
					{ field: 'swbID', title: '主键', hidden: true, width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					}
				]],
        toolbar: "#toolBarDetail",
        pagination: true,
        pageSize: 20,
        pageList: [20, 25, 30, 35, 40, 45, 50],
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
        onLoadSuccess: function (data) {
            var rowData = data.rows;
            for (var i = 0; i < rowData.length; i++) {
                var oneRow = rowData[i];
                if (rowData[i].chkNeedCheck == "1") {
                    _$_datagrid.datagrid("checkRow", i);
                } else {
                    _$_datagrid.datagrid("uncheckRow", i);
                }
            }

            $("#hid_Checked_swbSerialNum").val("");
            $("#hid_unChecked_swbSerialNum").val("");

            if (!bEnableCheck) {
                $(".datagrid-cell-check input[name=cb]").hide();
            }
        },
        onCheck: function (rowIndex, rowData) {
            if (bEnableCheck) {
                Append_hid_Checked_swbSerialNum(rowData.swbID);
            }
        },
        onUncheck: function (rowIndex, rowData) {
            if (bEnableCheck) {
                Append_hid_unChecked_swbSerialNum(rowData.swbID);
            }
        },
        onClickRow: function (rowIndex, rowData) {
            if (bEnableCheck) {
                setCurrentSele(rowData.swbSerialNum, true, 0);
            }
            $("#txtSubWayBillCode").focus();
        }
    });

    $(".datagrid-header-check input").hide();

    function getRowIndexBySwbSerialNum(swbSerialNum) {
        var i = -1;
        var rows = _$_datagrid.datagrid("getRows");
        for (var j = 0; j < rows.length; j++) {
            if (rows[j].swbSerialNum == swbSerialNum) {
                i = j;
            }
        }
        return i;
    }

    function setCurrentSele(swbSerialNum, bMouseClick, keyCode) {
        var bSele = false;
        var rowIndex = -1;
        var preSeleSwbSerialNum = "";
        var allCurrentRows = $(".currentRows");
        if (bMouseClick) {//鼠标单击
            $.each(allCurrentRows, function (i, item) {
                if ($(item).attr("currentRowSwbSerialNum") == swbSerialNum) {
                    //$(item).html("-----");
                    $(item).html("<img  src='../../images/Customer/currentsele.jpg' />");
                    //$(item).addClass("icon-currentsele");
                    $(item).attr("select", "1");
                } else {
                    //$(item).html("");
                    $(item).html("");
                    //$(item).removeClass("icon-currentsele");
                    $(item).attr("select", "0");
                }
            });
        } else {
            bSele = false;
            $.each(allCurrentRows, function (i, item) {
                if ($(item).attr("select") == "1") {
                    bSele = true;
                    preSeleSwbSerialNum = $(item).attr("currentRowSwbSerialNum");
                }
            });
            if (bSele) {//先前有选择
                rowIndex = getRowIndexBySwbSerialNum(preSeleSwbSerialNum);

                if (rowIndex == -1) {//先前没选择
                    switch (keyCode) {
                        case 38: // up
                            SetSeleStyle(_$_datagrid.datagrid("getRows").length - 1);
                            break;
                        case 40: // down
                            SetSeleStyle(0);
                            break;
                        case 13: //回车
                            SaveStatus();
                            break;
                        case 9: //Tab键
                            ProcessTab();
                            break;
                    }
                } else {//先前选择了第rowIndex行
                    switch (keyCode) {
                        case 38: // up
                            if (rowIndex == 0) {//先前选择的是第一行,现在就不管

                            } else {
                                SetSeleStyle(rowIndex - 1);
                            }
                            break;
                        case 40: // down
                            if (rowIndex == _$_datagrid.datagrid("getRows").length - 1) {//先前选择的是最后一行,现在就不管

                            } else {
                                SetSeleStyle(rowIndex + 1);
                            }
                            break;
                        case 13: //回车
                            SaveStatus();
                            break;
                        case 9: //Tab键
                            ProcessTab();
                            break;
                    }
                }
            } else {//先前没选择
                switch (keyCode) {
                    case 38: // up
                        SetSeleStyle(_$_datagrid.datagrid("getRows").length - 1);
                        break;
                    case 40: // down
                        SetSeleStyle(0);
                        break;
                    case 13: //回车
                        SaveStatus();
                        break;
                    case 9: //Tab键
                        ProcessTab();
                        break;
                }
            }
        }
    }

    _$_datagrid.datagrid('getPanel').keydown(function (e) {
        //console.info("键盘");
        if (e.ctrlKey && e.which == 13 || e.which == 10) {
            if (bEnableCheck) {
                ProcessCtrlEnter();
            }

        } else if (e.shiftKey && e.which == 13 || e.which == 10) {
            if (bEnableCheck) {
                ProcessCtrlEnter();
            }
        } else {
            setCurrentSele("", false, e.keyCode);
        }
    });

    $("#txtSubWayBillCode").keyup(function (e) {
        switch (e.keyCode) {
            case 13:
                $("#btnQuery_Detail").click();
                break;
            default:
                break;
        }
    });

    function SetSeleStyle(rowIndex) {
        var allCurrentRows = $(".currentRows");
        var swbSerialNum = "";
        swbSerialNum = _$_datagrid.datagrid('getRows')[rowIndex].swbSerialNum;

        $.each(allCurrentRows, function (i, item) {
            if ($(item).attr("currentRowSwbSerialNum") == swbSerialNum) {
                $(item).html("<img  src='../../images/Customer/currentsele.jpg' />");
                //$(item).addClass("icon-currentsele");
                $(item).attr("select", "1");
            } else {
                $(item).html("");
                //$(item).removeClass("icon-currentsele");
                $(item).attr("select", "0");
            }
        });

        var bCheckPre = false;

        var chkRows = _$_datagrid.datagrid('getChecked');
        //console.info(chkRows);
        //console.info(swbSerialNum);
        for (var i = 0; i < chkRows.length; i++) {
            if (chkRows[i].swbSerialNum == swbSerialNum) {
                bCheckPre = true;
            }
        }
        if (bCheckPre) {
            _$_datagrid.datagrid('unselectRow', rowIndex);
            _$_datagrid.datagrid('uncheckRow', rowIndex);
        } else {
            _$_datagrid.datagrid('selectRow', rowIndex);
            _$_datagrid.datagrid('checkRow', rowIndex);
        }
    }

    function SaveStatus() {
        //alert("保存状态");
    }

    function ProcessCtrlEnter() {
        self.parent.$("#dlg_GlobalDetail").find(".dialog-button").find(".icon-ok").click();
        $("#btnQuery_Detail").click();
    }

    function SeleAll() {
        var rows = _$_datagrid.datagrid("getRows");
        for (var i = 0; i < rows.length; i++) {
            var m = _$_datagrid.datagrid("getRowIndex", rows[i]);
            _$_datagrid.datagrid("selectRow", m)
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
                _$_datagrid.datagrid("selectRow", m)
            }
        }
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:150px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "cb":
                    break;
                case "swbserialnum":
                    break;
                case "chkneedcheck":
                    break;
                case "swbid":
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
