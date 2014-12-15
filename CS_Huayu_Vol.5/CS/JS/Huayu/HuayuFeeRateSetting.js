$(function () {
    var _$_datagrid = $("#DG_FeeRateSetting");
    var QueryURL = "/Huayu_FeeRateSetting/GetData";

    var PrintURL = "";
    var UpdateDlg = null;
    var UpdateFeeRate = "/Huayu_FeeRateSetting/UpdateFeeRate?frID=";

    $("#btnRefresh").click(function () {
        _$_datagrid.treegrid("reload");
        _$_datagrid.treegrid("unselectAll");
        ExpandAllNode();
    });

    _$_datagrid.treegrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'CategoryID',
        sortOrder: 'asc',
        remoteSort: true,
        border: false,
        idField: 'CategoryID',
        treeField: 'CategoryName',
        columns: [[
					{ field: 'CategoryName', title: '类别或项目名称', width: 200
					},
					{ field: 'CategoryValue', title: '项目值', width: 150
					},
                    { field: 'CategoryUnit', title: '单位', width: 150
                    },
                     { field: 'mMemo', title: '说明', width: 300
                     },
                     { field: 'updateFeeRate', title: '操作', width: 300,
                         formatter: function (value, rowData, rowIndex) {
                             if (rowData.isLeaf == "1") {
                                 return "<a href='#' class='btnUpdateFeeRate_cls' parentID='" + rowData.parentID + "' frID='" + rowData.frID + "' CategoryName='" + rowData.CategoryName + "' CategoryValue='" + rowData.CategoryValue + "' CategoryUnit='" + rowData.CategoryUnit + "' mMemo='" + rowData.mMemo + "'>修改费率信息</a>";
                             } else {
                                 return "";
                             }
                         }
                     },
                    { field: 'frID', title: '', hidden: true
                    },
                    { field: 'CategoryID', title: '', hidden: true
                    },
                    { field: 'CategoryPID', title: '', hidden: true
                    }
				]],
        pagination: true,
        pageSize: 15,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
        toolbar: "#toolBar",
        onRowContextMenu: function (e, rowIndex, rowData) {
            e.preventDefault();
            _$_datagrid.treegrid("unselectAll");
            _$_datagrid.treegrid("selectRow", rowIndex);

            var cmenu = $('<div id="cmenu" style="width:100px;"></div>').appendTo('body');
            $('<div  id="mnuRefresh" iconCls="icon-reload"/>').html("刷新").appendTo(cmenu);
            cmenu.menu({
                onClick: function (item) {
                    cmenu.remove();
                    switch (item.id.toLowerCase()) {
                        case "mnurefresh":
                            _$_datagrid.treegrid("reload");
                            _$_datagrid.treegrid("unselectAll");
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
            //_$_datagrid.treegrid("reload");
        },
        onClickRow: function (row) {
            //_$_datagrid.treegrid("expand", row.ID);
            //            if (row.isLeaf == "1") {
            //                Update(row.frID, row.CategoryName, row.CategoryValue, row.CategoryUnit, row.mMemo, row.parentID)
            //            } else {
            //                _$_datagrid.treegrid("toggle", row.CategoryID);
            //            }
            _$_datagrid.treegrid("toggle", row.CategoryID);
        },
        onDblClickRow: function (row) {
            //_$_datagrid.treegrid("expand", row.ID);
            if (row.isLeaf == "1") {
                Update(row.frID, row.CategoryName, row.CategoryValue, row.CategoryUnit, row.mMemo, row.parentID)
            }
        },
        onLoadSuccess: function (data) {
            var allUpdateFeeRateBtns = $(".btnUpdateFeeRate_cls");
            $.each(allUpdateFeeRateBtns, function (i, item) {
                var frID = $(item).attr("frID");
                var CategoryName = $(item).attr("CategoryName");
                var CategoryValue = $(item).attr("CategoryValue");
                var CategoryUnit = $(item).attr("CategoryUnit");
                var mMemo = $(item).attr("mMemo");
                var parentID = $(item).attr("parentID");
                $(item).click(function () {
                    Update(frID, CategoryName, CategoryValue, CategoryUnit, mMemo, parentID);
                });
            });

            _$_datagrid.treegrid("expandAll");
        }
    });

    function Update(frID, CategoryName, CategoryValue, CategoryUnit, mMemo, parentID) {
        $("#tipCategoryName").html(CategoryName);
        $("#txtCategoryValue").numberbox('setValue', CategoryValue);
        $("#txtCategoryUnit").val(CategoryUnit);
        $("#mMemo").val(mMemo);
        $("#hid_frID").val(frID);
        UpdateDlg = $('#dlg_Create').dialog({
            buttons: [{
                text: '保 存',
                iconCls: 'icon-ok',
                handler: function () {
                    var hid_frID = $("#hid_frID").val();
                    var txtCategoryUnit = $("#txtCategoryUnit").val();
                    var mMemo = $("#mMemo").val();
                    var txtCategoryValue = $('#txtCategoryValue').numberbox('getValue');
                    if (hid_frID == "" || txtCategoryValue == "") {
                        reWriteMessagerAlert('操作提示', '请填写完整信息<br/>(项目值)', "error");
                        return false;
                    }

                    var bOK = false;
                    $.ajax({
                        type: "POST",
                        url: UpdateFeeRate + encodeURI(hid_frID) + "&CategoryValue=" + encodeURI(txtCategoryValue) + "&CategoryUnit=" + encodeURI(txtCategoryUnit) + "&mMemo=" + encodeURI(mMemo),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            var JSONMsg = eval("(" + msg + ")");
                            if (JSONMsg.result.toLowerCase() == 'ok') {
                                bOK = true;
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
                    if (bOK) {
                        UpdateDlg.dialog('close');
                        _$_datagrid.treegrid("reload", parentID);
                        _$_datagrid.treegrid("unselectAll");
                    }
                }
            }, {
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    UpdateDlg.dialog('close');
                }
            }],
            title: '修改费率信息',
            modal: true,
            resizable: true,
            cache: false,
            closed: true,
            left: 50,
            top: 30,
            width: 400,
            height: 200
        });

        $('#dlg_Create').dialog("open");
    }

    function Print() {
        PrintURL = "/Huayu_UserMaintain/Print?order=" + _$_datagrid.treegrid("options").sortOrder + "&sort=" + _$_datagrid.treegrid("options").sortName + "&page=1&rows=10000000";
        if (_$_datagrid.treegrid("getData").rows.length > 0) {
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
    }

    function Excel() {
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

        PrintURL = "/Huayu_UserMaintain/Excel?order=" + _$_datagrid.treegrid("options").sortOrder + "&sort=" + _$_datagrid.treegrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.treegrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    }

    function SeleAll() {
        var rows = _$_datagrid.treegrid("getRows");
        for (var i = 0; i < rows.length; i++) {
            var m = _$_datagrid.treegrid("getRowIndex", rows[i]);
            _$_datagrid.treegrid("selectRow", m)
        }
    }

    function InverseSele() {
        var rows = _$_datagrid.treegrid("getRows");
        var selects = _$_datagrid.treegrid("getSelections");
        for (var i = 0; i < rows.length; i++) {
            var bSele = false;
            var m = _$_datagrid.treegrid("getRowIndex", rows[i]);
            for (var j = 0; j < selects.length; j++) {
                var n = _$_datagrid.treegrid("getRowIndex", selects[j]);
                if (m == n) {
                    bSele = true;
                }
            }
            if (bSele) {
                _$_datagrid.treegrid("unselectRow", m)
            } else {
                _$_datagrid.treegrid("selectRow", m)
            }
        }
    }

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.treegrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.treegrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "categoryname":
                    break;
                case "frid":
                    break;
                case "categoryid":
                    break;
                case "categorypid":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "CategoryName") {

                } else {
                    if (item.iconCls == 'icon-ok') {
                        _$_datagrid.treegrid('hideColumn', $(item.text).attr("id"));
                        tmenu.menu('setIcon', {
                            target: item.target,
                            iconCls: 'icon-empty'
                        });
                    } else {
                        _$_datagrid.treegrid('showColumn', $(item.text).attr("id"));
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
