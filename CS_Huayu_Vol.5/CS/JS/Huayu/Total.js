$(function () {
    var _$_datagrid = $("#DG_TotalWayBillResult");
    var _$_ddCompany = $('#ddCompany');
    var QueryCompanyURL = "/ForwarderMain/LoadComboxJSON";
    var defaultValue = "---请选择---";

    _$_ddCompany.combobox({
        url: QueryCompanyURL,
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    _$_ddCompany.combobox("setValue", defaultValue);

    var QueryURL = "/Huayu_Total/GetData?ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtStartDate=" + encodeURI($("#txtStartDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val());

    $("#BtnQuery").click(function () {
        if (_$_ddCompany.combobox("getValue") == defaultValue && $("#txtStartDate").val() == "" && $("#txtEndDate").val() == "") {
            reWriteMessagerAlert("操作提示", "请至少选择一项条件进行查询<br/>(选择日期时，必须设置完整的开始日期和截止日期)", "error");
            return false;
        }

        if (($("#txtStartDate").val() == "" && $("#txtEndDate").val() != "") || ($("#txtStartDate").val() != "" && $("#txtEndDate").val() == "")) {
            reWriteMessagerAlert("操作提示", "选择日期时，必须设置完整的开始日期和截止日期", "error");
            return false;
        }

        var QueryURL = "/Huayu_Total/GetData?ddCompany=" + encodeURI(_$_ddCompany.combobox("getValue")) + "&txtStartDate=" + encodeURI($("#txtStartDate").val()) + "&txtEndDate=" + encodeURI($("#txtEndDate").val());
        window.setTimeout(function () {
            $.extend(_$_datagrid.datagrid("options"), {
                url: QueryURL
            });
            _$_datagrid.datagrid("reload");
        }, 10); //延迟100毫秒执行，时间可以更短

    });

    _$_datagrid.datagrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'wbCompany',
        sortOrder: 'desc',
        remoteSort: true,
        border: false,
        idField: 'wbCompany',
        columns: [[
					{ field: 'wbCompany', title: '货代公司', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'wbTotalNum', title: '总件数', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'wbTotalUnReleased', title: '总扣留数', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalWeight', title: '总重量(公斤)', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalWeight_Category2', title: '样品重量(公斤)', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalWeight_Category3', title: 'KJ-3重量(公斤)', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalWeight_Category4', title: 'D类重量(公斤)', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalWeight_Category5', title: '个人物品重量(公斤)', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalWeight_Category6', title: '分运行李(公斤)', width: 120, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'wbTotalWeightWithOutUnit', hidden: true, title: '0'
                    }
				]],
        pagination: true,
        pageSize: 15,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
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
        },
        onLoadSuccess: function (data) {
            var rowData = data.rows;
            var totalNum = 0;
            var totalWeight = 0.0;

            for (var i = 0; i < rowData.length; i++) {
                totalNum = totalNum + parseInt(rowData[i].wbTotalNum);
                totalWeight = totalWeight + parseInt(rowData[i].wbTotalWeightWithOutUnit);
            }
            $("#lbNumber").html(totalNum);
            $("#lbWeight").html(totalWeight + "公斤");

            if ($("#txtStartDate").val() != "" && $("#txtEndDate").val() != "") {
                var txtStartDate = $("#txtStartDate").val();
                var txtEndDate = $("#txtEndDate").val();
                var d_txtStartDate = txtStartDate.toDate(); // new Date(txtStartDate);
                var d_txtEndDate = txtEndDate.toDate(); // new Date(txtEndDate);

                $("#lbDate").html(String(d_txtStartDate.getFullYear()) + "" + String(d_txtStartDate.getMonth() + 1) + "" + String(d_txtStartDate.getDate()) + "" + "-" + String(d_txtEndDate.getFullYear()) + "" + String(d_txtEndDate.getMonth() + 1) + "" + String(d_txtEndDate.getDate()));
            }

        }
    });

    function createColumnMenu() {
        var tmenu = $('<div id="tmenu" style="width:130px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "wbcompany":
                    break;
                case "wbtotalweightwithoutunit":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "wbCompany") {

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
