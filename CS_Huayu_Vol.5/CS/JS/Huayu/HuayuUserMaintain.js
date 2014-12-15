$(function () {
    var _$_datagrid = $("#DG_UserMaintain");
    var QueryURL = "/Huayu_UserMaintain/GetData";
    var CreateURL = "/Huayu_UserMaintain/Create";
    var UpdateURL = "/Huayu_UserMaintain/Edit/";
    var DeleteURL = "/Huayu_UserMaintain/Delete";
    var TestExist_UserName = "/Huayu_UserMaintain/ExistUserName?strUsername=";
    var TestExist_UserName_Update = "/Huayu_UserMaintain/ExistUserName_Update?strUsername=";
    var CreateDlg = null;
    var CreateDlgForm = null;
    var EditDlg = null;
    var EditDlgForm = null;

    var PrintURL = "";

    _$_datagrid.datagrid({
        iconCls: 'icon-save',
        nowrap: true,
        autoRowHeight: false,
        autoRowWidth: false,
        striped: true,
        collapsible: true,
        url: QueryURL,
        sortName: 'userID',
        sortOrder: 'asc',
        remoteSort: true,
        border: false,
        idField: 'userID',
        columns: [[
                    { field: 'cb', width: 120, checkbox: true },
					{ field: 'userName', title: '用户名', width: 120, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
					{ field: 'userPassword', title: '密码', hidden: true, width: 220, sortable: true,
					    sorter: function (a, b) {
					        return (a > b ? 1 : -1);
					    }
					},
                    { field: 'authorityDescription', title: '权限', width: 220, sortable: true, hidden: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                     { field: 'commentDescription', title: '部门类别', width: 120, sortable: true,
                         sorter: function (a, b) {
                             return (a > b ? 1 : -1);
                         }
                     },
                    { field: 'company', title: '单位名称', width: 150, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CompanyFullName', title: '单位全称', width: 170, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'LinkPerson', title: '联系人', width: 80, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'IdentityCode', title: '联系人身份证号', width: 130, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'LinkTel', title: '联系人电话', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CompanyPhone', title: '公司电话', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'CompanyAddr', title: '公司地址', width: 200, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'LinkMail', title: '邮箱地址', width: 100, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        }
                    },
                    { field: 'iSendFirstPickGoodsEmail', title: '放行单邮件通知', width: 110, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            if (rowData.iSendFirstPickGoodsEmail == "1") {
                                return "<input type='checkbox' checked disabled='true'>";
                            } else {
                                return "<input type='checkbox' disabled='true'>";
                            }
                        }

                    },
                    { field: 'iSendUnReleaseGoodsEmail', title: '扣货单邮件通知', width: 110, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            if (rowData.iSendUnReleaseGoodsEmail == "1") {
                                return "<input type='checkbox' checked  disabled='true'>";
                            } else {
                                return "<input type='checkbox'  disabled='true'>";
                            }
                        }
                    },
                    { field: 'iSendRejectGoodsEmail', title: '退货单邮件通知', width: 110, sortable: true,
                        sorter: function (a, b) {
                            return (a > b ? 1 : -1);
                        },
                        formatter: function (value, rowData, rowIndex) {
                            if (rowData.iSendRejectGoodsEmail == "1") {
                                return "<input type='checkbox' checked  disabled='true'>";
                            } else {
                                return "<input type='checkbox'  disabled='true'>";
                            }
                        }
                    },
                    { field: 'userID', title: '', hidden: true
                    }
				]],
        pagination: true,
        pageSize: 15,
        pageList: [15, 20, 25, 30, 35, 40, 45, 50],
        toolbar: [{
            id: 'btnQuery',
            text: '查询',
            iconCls: 'icon-search',
            handler: function () {
                _$_datagrid.datagrid("reload");
                _$_datagrid.datagrid("unselectAll");
            }
        }, '-', {
            id: 'btnAdd',
            text: '添加',
            iconCls: 'icon-add',
            handler: function () {
                Add();
            }
        }, '-', {
            id: 'btnUpdate',
            text: '修改',
            iconCls: 'icon-edit',
            handler: function () {
                Update();
            }
        }, '-', {
            id: 'btnDelete',
            text: '删除',
            disabled: false,
            iconCls: 'icon-remove',
            handler: function () {
                Delete();
            }
        }, '-', {
            id: 'btnSeleAll',
            text: '全选',
            disabled: false,
            iconCls: 'icon-seleall',
            handler: function () {
                SeleAll();
            }
        }, '-', {
            id: 'btnInverseSele',
            text: '反选',
            disabled: false,
            iconCls: 'icon-inversesele',
            handler: function () {
                InverseSele();
            }
        }, '-', {
            id: 'btnPrint',
            text: '打印',
            disabled: false,
            iconCls: 'icon-print',
            handler: function () {
                Print();
            }
        }, '-', {
            id: 'btnExcel',
            text: '导出',
            disabled: false,
            iconCls: 'icon-excel',
            handler: function () {
                Excel();
            }
        }],
        onRowContextMenu: function (e, rowIndex, rowData) {
            e.preventDefault();
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);

            var cmenu = $('<div id="cmenu" style="width:100px;"></div>').appendTo('body');
            $('<div  id="mnuQuery" iconCls="icon-search"/>').html("查询").appendTo(cmenu);
            $('<div  id="mnuAdd" iconCls="icon-add"/>').html("添加").appendTo(cmenu);
            $('<div  id="mnuUpdate" iconCls="icon-edit"/>').html("修改").appendTo(cmenu);
            $('<div  id="mnuDelete" iconCls="icon-remove"/>').html("删除").appendTo(cmenu);
            $('<div  id="mnuSeleAll" iconCls="icon-seleall"/>').html("全选").appendTo(cmenu);
            $('<div  id="mnuInverseSele" iconCls="icon-inversesele"/>').html("反选").appendTo(cmenu);
            $('<div  id="mnuPrint" iconCls="icon-print"/>').html("打印").appendTo(cmenu);
            $('<div  id="mnuExcel" iconCls="icon-excel"/>').html("导出").appendTo(cmenu);
            cmenu.menu({
                onClick: function (item) {
                    cmenu.remove();
                    switch (item.id.toLowerCase()) {
                        case "mnuadd":
                            Add();
                            break;
                        case "mnuupdate":
                            Update();
                            break;
                        case "mnudelete":
                            Delete();
                            break;
                        case "mnuseleall":
                            SeleAll();
                            break;
                        case "mnuinversesele":
                            InverseSele();
                            break;
                        case "mnuprint":
                            Print();
                            break;
                        case "mnuexcel":
                            Excel();
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
            // _$_datagrid.datagrid("reload");
        },
        onDblClickRow: function (rowIndex, rowData) {
            _$_datagrid.datagrid("unselectAll");
            _$_datagrid.datagrid("selectRow", rowIndex);
            Update(rowData.iEmpTypeId);
        }
    });

    function Print() {
        PrintURL = "/Huayu_UserMaintain/Print?order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000";
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

        PrintURL = "/Huayu_UserMaintain/Excel?order=" + _$_datagrid.datagrid("options").sortOrder + "&sort=" + _$_datagrid.datagrid("options").sortName + "&page=1&rows=10000000&browserType=" + browserType;
        if (_$_datagrid.datagrid("getData").rows.length > 0) {
            window.open(PrintURL);

        } else {
            reWriteMessagerAlert("提示", "没有数据，不可导出", "error");
            return false;
        }
    }

    function Add() {
        _$_datagrid.datagrid("unselectAll");
        CreateDlg = $('#dlg_Create').dialog({
            buttons: [{
                text: '保 存',
                iconCls: 'icon-ok',
                handler: function () {
                    var txtUserID = CreateDlg.find('#txtUserID').val();
                    var txtUserPassword = CreateDlg.find('#txtUserPassword').val();
                    var txtReUserPassword = CreateDlg.find('#txtReUserPassword').val();
                    var txtCompany = CreateDlg.find('#txtCompany').val();
                    var ddAuthority = CreateDlg.find('#ddAuthority').combobox("getValue");
                    var ddComment = CreateDlg.find('#ddComment').combobox("getValue");

                    if (txtUserID == "" || txtUserPassword == "" || txtReUserPassword == "" || txtCompany == "" || ddAuthority == "---请选择---" || ddComment == "---请选择---") {
                        reWriteMessagerAlert('操作提示', '请填写完整信息<br/>(用户名、密码、确认密码、权限、单位类型、公司名称)', "error");
                        return false;
                    }

                    if (txtUserPassword != txtReUserPassword) {
                        reWriteMessagerAlert('操作提示', '密码与确认密码不一致', "error");
                        return false;
                    }

                    //验证此用户名是否已使用
                    var bExist = false;
                    $.ajax({
                        type: "GET",
                        url: TestExist_UserName + encodeURI(txtUserID),
                        data: "",
                        async: false,
                        cache: false,
                        beforeSend: function (XMLHttpRequest) {

                        },
                        success: function (msg) {
                            if (msg.toLowerCase() == 'true') {
                                reWriteMessagerAlert('操作提示', '此用户名已经使用!', 'error');
                                bExist = true;
                            }
                        },
                        complete: function (XMLHttpRequest, textStatus) {

                        },
                        error: function () {

                        }
                    });

                    if (!bExist) {
                        CreateDlgForm = CreateDlg.find('form');
                        CreateDlgForm.form('submit', {
                            url: CreateDlgForm.url,
                            onSubmit: function () {
                                return $(this).form('validate');
                            },
                            success: function () {
                                reWriteMessagerAlert('提示', '成功', "info");
                                CreateDlg.dialog('close');
                                _$_datagrid.datagrid("reload");
                                _$_datagrid.datagrid("unselectAll");
                            }
                        });
                    }

                }
            }, {
                text: '关 闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    CreateDlg.dialog('close');
                }
            }],
            title: '添加系统用户信息',
            href: CreateURL,
            modal: true,
            resizable: true,
            cache: false,
            closed: true,
            left: 50,
            top: 30,
            width: 800,
            height: 320
        });

        $('#dlg_Create').dialog("open");
    }

    function Update(id) {
        if (id) {
            EditDlg = $('#dlg_Update').dialog({
                buttons: [{
                    text: '保 存',
                    iconCls: 'icon-ok',
                    handler: function () {
                        var txtUserID = EditDlg.find('#txtUserID').val();
                        var txtUserPassword = EditDlg.find('#txtUserPassword').val();
                        var txtReUserPassword = EditDlg.find('#txtReUserPassword').val();
                        var txtCompany = EditDlg.find('#txtCompany').val();
                        var ddAuthority = EditDlg.find('#ddAuthority').combobox("getValue");
                        var ddComment = EditDlg.find('#ddComment').combobox("getValue");

                        if (txtUserID == "" || txtUserPassword == "" || txtReUserPassword == "" || txtCompany == "" || ddAuthority == "---请选择---" || ddComment == "---请选择---") {
                            reWriteMessagerAlert('操作提示', '请填写完整信息<br/>(用户名、密码、确认密码、权限、单位类型、公司名称)', "error");
                            return false;
                        }

                        if (txtUserPassword != txtReUserPassword) {
                            reWriteMessagerAlert('操作提示', '密码与确认密码不一致', "error");
                            return false;
                        }

                        //验证此用户名是否已使用
                        var bExist = false;
                        $.ajax({
                            type: "GET",
                            url: TestExist_UserName_Update + encodeURI(txtUserID) + "&id=" + id,
                            data: "",
                            async: false,
                            cache: false,
                            beforeSend: function (XMLHttpRequest) {

                            },
                            success: function (msg) {
                                if (msg.toLowerCase() == 'true') {
                                    reWriteMessagerAlert('操作提示', '此用户名已经使用!', 'error');
                                    bExist = true;
                                }
                            },
                            complete: function (XMLHttpRequest, textStatus) {

                            },
                            error: function () {

                            }
                        });

                        if (!bExist) {
                            EditDlgForm = EditDlg.find('form');
                            EditDlgForm.form('submit', {
                                url: EditDlgForm.url,
                                onSubmit: function () {
                                    return $(this).form('validate');
                                },
                                success: function () {
                                    reWriteMessagerAlert('提示', '成功', "info");
                                    EditDlg.dialog('close');
                                    _$_datagrid.datagrid("reload");
                                    _$_datagrid.datagrid("unselectAll");
                                }
                            });
                        }


                    }
                }, {
                    text: '关 闭',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        EditDlg.dialog('close');
                    }
                }],
                title: '修改系统用户信息',
                href: UpdateURL + id,
                modal: true,
                resizable: true,
                cache: false,
                left: 50,
                top: 30,
                width: 800,
                height: 320,
                closed: true
            });
            _$_datagrid.datagrid("unselectAll");
        } else {
            var selects = _$_datagrid.datagrid("getSelections");
            if (selects.length != 1) {
                reWriteMessagerAlert("提示", "<center>请选择数据进行修改(<font style='color:red'>只可选择一行</font>)</center>", "error");
                return false;
            } else {
                id = selects[0].userID;
                EditDlg = $('#dlg_Update').dialog({
                    buttons: [{
                        text: '保 存',
                        iconCls: 'icon-ok',
                        handler: function () {
                            var txtUserID = EditDlg.find('#txtUserID').val();
                            var txtUserPassword = EditDlg.find('#txtUserPassword').val();
                            var txtReUserPassword = EditDlg.find('#txtReUserPassword').val();
                            var txtCompany = EditDlg.find('#txtCompany').val();
                            var ddAuthority = EditDlg.find('#ddAuthority').combobox("getValue");
                            var ddComment = EditDlg.find('#ddComment').combobox("getValue");

                            if (txtUserID == "" || txtUserPassword == "" || txtReUserPassword == "" || txtCompany == "" || ddAuthority == "---请选择---" || ddComment == "---请选择---") {
                                reWriteMessagerAlert('操作提示', '请填写完整信息<br/>(用户名、密码、确认密码、权限、单位类型、公司名称)', "error");
                                return false;
                            }

                            if (txtUserPassword != txtReUserPassword) {
                                reWriteMessagerAlert('操作提示', '密码与确认密码不一致', "error");
                                return false;
                            }

                            //验证此用户名是否已使用
                            var bExist = false;
                            $.ajax({
                                type: "GET",
                                url: TestExist_UserName_Update + encodeURI(txtUserID) + "&id=" + id,
                                data: "",
                                async: false,
                                cache: false,
                                beforeSend: function (XMLHttpRequest) {

                                },
                                success: function (msg) {
                                    if (msg.toLowerCase() == 'true') {
                                        reWriteMessagerAlert('操作提示', '此用户名已经使用!', 'error');
                                        bExist = true;
                                    }
                                },
                                complete: function (XMLHttpRequest, textStatus) {

                                },
                                error: function () {

                                }
                            });

                            if (!bExist) {
                                EditDlgForm = EditDlg.find('form');
                                EditDlgForm.form('submit', {
                                    url: EditDlgForm.url,
                                    onSubmit: function () {
                                        return $(this).form('validate');
                                    },
                                    success: function () {
                                        reWriteMessagerAlert('提示', '成功', "info");
                                        EditDlg.dialog('close');
                                        _$_datagrid.datagrid("reload");
                                        _$_datagrid.datagrid("unselectAll");
                                    }
                                });
                            }
                        }
                    }, {
                        text: '关 闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            EditDlg.dialog('close');
                        }
                    }],
                    title: '修改系统用户信息',
                    href: UpdateURL + id,
                    modal: true,
                    resizable: true,
                    cache: false,
                    left: 50,
                    top: 30,
                    width: 800,
                    height: 320,
                    closed: true
                });
                _$_datagrid.datagrid("unselectAll");
            }
        }
        $('#dlg_Update').dialog("open");
    }

    function Delete() {
        reWriteMessagerConfirm("提示", "您确定需要删除所选的系统用户信息吗？",
                    function (ok) {
                        if (ok) {
                            var selects = _$_datagrid.datagrid("getSelections");
                            var ids = [];
                            for (var i = 0; i < selects.length; i++) {
                                ids.push(selects[i].userID);
                            }
                            if (selects.length == 0) {
                                reWriteMessagerAlert("提示", "<center>请选择需要删除的数据</center>", "error");
                                return false;
                            }

                            $.ajax({
                                url: DeleteURL + '?ids=' + ids.join(','),
                                //data: { 'ids': ids.join(',') },
                                type: "POST",
                                cache: false,
                                async: false,
                                success: function () {

                                }
                            });
                            _$_datagrid.datagrid("reload");
                            _$_datagrid.datagrid("unselectAll");
                        } else {

                        }
                    }
                );
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
        var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
        var fields = _$_datagrid.datagrid('getColumnFields');

        for (var i = 0; i < fields.length; i++) {
            var title = _$_datagrid.datagrid('getColumnOption', fields[i]).title;
            switch (fields[i].toLowerCase()) {
                case "cb":
                    break;
                case "userid":
                    break;
                case "username":
                    break;
                case "userpassword":
                    break;
                case "authoritydescription":
                    break;
                default:
                    $('<div iconCls="icon-ok"/>').html("<span id='" + fields[i] + "'>" + title + "</span>").appendTo(tmenu);
                    break;
            }
        }
        tmenu.menu({
            onClick: function (item) {
                if ($(item.text).attr("id") == "userID") {

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
