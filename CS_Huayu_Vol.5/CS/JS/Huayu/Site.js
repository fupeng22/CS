$(function () {
    var defaultTabTitle = "使用指南";
    var LogOutURL = "/Login/LogOut";
    var GetDateTimeURL = "/Util/getDatetime";

    var bGetDateOK = false;
    var iDifferent = 0;

    tabClose();
    tabCloseEven();

    function addTab(subtitle, url, icon) {
        if (!$('#tab_Content').tabs('exists', subtitle)) {
            $('#tab_Content').tabs('add', {
                title: subtitle,
                content: createFrame(url),
                closable: true,
                icon: icon
            });
        } else {
            $('#tab_Content').tabs('select', subtitle);
            //$('#mm-tabupdate').click();
            var currTab = $('#tab_Content').tabs('getSelected');
            var url = $(currTab.panel('options').content).attr('src');
            if (url != undefined && currTab.panel('options').title != defaultTabTitle) {
                $('#tab_Content').tabs('update', {
                    tab: currTab,
                    options: {
                        content: createFrame(url)
                    }
                })
            }
        }
        tabClose();
    }

    function createFrame(url) {
        var s = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:99%;"></iframe>';
        return s;
    }

    function tabClose() {
        /*双击关闭TAB选项卡*/
        $(".tabs-inner").dblclick(function () {
            var subtitle = $(this).children(".tabs-closable").text();
            $('#tab_Content').tabs('close', subtitle);
        })
        /*为选项卡绑定右键*/
        $(".tabs-inner").bind('contextmenu', function (e) {
            $('#mm').menu('show', {
                left: e.pageX,
                top: e.pageY
            });

            var subtitle = $(this).children(".tabs-closable").text();

            $('#mm').data("currtab", subtitle);
            $('#tab_Content').tabs('select', subtitle);
            return false;
        });
    }
    //绑定右键菜单事件
    function tabCloseEven() {
        //刷新
        $('#mm-tabupdate').click(function () {
            var currTab = $('#tab_Content').tabs('getSelected');
            var url = $(currTab.panel('options').content).attr('src');
            $('#tab_Content').tabs('update', {
                tab: currTab,
                options: {
                    content: createFrame(url)
                }
            })
        })
        //关闭当前
        $('#mm-tabclose').click(function () {
            var currtab_title = $('#mm').data("currtab");
            $('#tab_Content').tabs('close', currtab_title);
        })
        //全部关闭
        $('#mm-tabcloseall').click(function () {
            $('.tabs-inner span').each(function (i, n) {
                var t = $(n).text();
                if (t == defaultTabTitle) {
                    return;
                }
                $('#tab_Content').tabs('close', t);
            });
        });
        //关闭除当前之外的TAB
        $('#mm-tabcloseother').click(function () {
            $('#mm-tabcloseright').click();
            $('#mm-tabcloseleft').click();
        });
        //关闭当前右侧的TAB
        $('#mm-tabcloseright').click(function () {
            var nextall = $('.tabs-selected').nextAll();
            if (nextall.length == 0) {
                //msgShow('系统提示','后边没有啦~~','error');
                //alert('后边没有啦~~');
                return false;
            }
            nextall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                if (t != defaultTabTitle) {
                    $('#tab_Content').tabs('close', t);
                }
            });
            return false;
        });
        //关闭当前左侧的TAB
        $('#mm-tabcloseleft').click(function () {
            var prevall = $('.tabs-selected').prevAll();
            if (prevall.length == 0) {
                //alert('到头了，前边没有啦~~');
                return false;
            }
            prevall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                if (t != defaultTabTitle) {
                    $('#tab_Content').tabs('close', t);
                }
            });
            return false;
        });

        //退出
        $("#mm-exit").click(function () {
            $('#mm').menu('hide');
        })
    }

    //设置导航菜单栏的事件
    $("#btnQueryStatus").click(function () {
        addTab($(this).text(), "/Huayu_Query/Index", "icon-reload");
    });

    $("#btnPrintWayBill_Main").click(function () {
        addTab($(this).text(), "/Huayu_Relese/Index", "icon-reload");
    });

    $("#btnQuery").click(function () {
        addTab($(this).text(), "/Huayu_Total/Index", "icon-reload");
    });

    $("#btnPwdManage").click(function () {
        addTab($(this).text(), "/Huayu_ChangePassword/Index", "icon-reload");
    });

    $("#btnForeStore").click(function () {
        addTab($(this).text(), "/Huayu_ForeStore/Index", "icon-reload");
    });

    $("#btnWayBillPatchIn").click(function () {
        addTab($(this).text(), "/Huayu_WayBillPatchIn/Index", "icon-reload");
    });

    $("#btnWayBillPatchInExcel").click(function () {
        addTab($(this).text(), "/Huayu_WayBillPatchInExcel/Index", "icon-reload");
    });

    $("#btnWayBillPatchOut").click(function () {
        addTab($(this).text(), "/Huayu_WayBillPatchOut/Index", "icon-reload");
    });

    $("#btnWayBillPatchOutExcel").click(function () {
        addTab($(this).text(), "/Huayu_WayBillPatchOutExcel/Index", "icon-reload");
    });

    $("#btnInStore").click(function () {
        addTab($(this).text(), "/Huayu_InStore/Index", "icon-reload");
    });

    $("#btnOutStore").click(function () {
        addTab($(this).text(), "/Huayu_OutStore/Index", "icon-reload");
    });

    $("#btnExceptionStore").click(function () {
        addTab($(this).text(), "/Huayu_ExceptionStore/Index", "icon-reload");
    });

    $("#btAnanalyseStatistic").click(function () {
        addTab($(this).text(), "/Huayu_AnanalyseStatistic/Index", "icon-reload");
    });

    $("#btnInOutStoreExceptionEvent").click(function () {
        addTab($(this).text(), "/Huayu_InOutStoreExceptionEvent/Index", "icon-reload");
    });

    $("#btnUserManage").click(function () {
        addTab($(this).text(), "/Huayu_UserMaintain/Index", "icon-reload");
    });

    $("#btnHuayu_QueryMainFrame").click(function () {
        addTab($(this).text(), "/Huayu_QueryMainFrame/Index", "icon-reload");
    });

    $("#btnViewExceptionStoreHistory").click(function () {
        addTab($(this).text(), "/Huayu_ViewExceptionStoreHistory/Index", "icon-reload");
    });

    $("#btnCustomsCheck").click(function () {
        addTab($(this).text(), "/Customer_Check/Index", "icon-reload");
    });

    $("#btnCustomsConfirm").click(function () {
        addTab($(this).text(), "/Customer_Confirm/Index", "icon-reload");
    });

    $("#btnConfirmInStore").click(function () {
        addTab($(this).text(), "/Huayu_ConfirmInStore/Index", "icon-reload");
    });

    $("#btnWayBillWeight").click(function () {
        addTab($(this).text(), "/Huayu_WayBillWeight/Index", "icon-reload");
    });

    $("#btnUnReleasedWayBill_Main").click(function () {
        addTab($(this).text(), "/Huayu_UnReleaseWayBillHandler/Index", "icon-reload");
    });

    $("#btnRejectWayBill_Main").click(function () {
        addTab($(this).text(), "/Huayu_RejectWayBillHandler/Index", "icon-reload");
    });

    $("#btnQueryRejectSubWayBillInfo").click(function () {
        addTab($(this).text(), "/Huayu_QueryRejectSubWayBill/Index", "icon-reload");
    });

    $("#btnFeeSetting").click(function () {
        addTab($(this).text(), "/Huayu_FeeRateSetting/Index", "icon-reload");
    });

    $("#btnWayBillDailyReport").click(function () {
        addTab($(this).text(), "/Huayu_WayBillDailyReport/Index", "icon-reload");
    });

    $("#btnEmailManage").click(function () {
        addTab($(this).text(), "/Huayu_EmailManagement/Index", "icon-reload");
    });

    $("#btnTotalStatisticByMonth").click(function () {
        addTab($(this).text(), "/Huayu_TotalStatisticByMonth/Index", "icon-reload");
    });

    $("#btnSaveInStore").click(function () {
        addTab($(this).text(), "/Huayu_SaveInStore/Index", "icon-reload");
    });

    $("#btnRemoveStore").click(function () {
        addTab($(this).text(), "/Huayu_RemoveStore/Index", "icon-reload");
    });

    $("#btnViewCheckResult").click(function () {
        addTab($(this).text(), "/Huayu_ViewCheckResult/Index", "icon-reload");
    });

    $("#btnWayBillFeeForeSetting").click(function () {
        addTab($(this).text(), "/Huayu_WayBillFeeForeSetting/Index", "icon-reload");
    });

    $("#btnViewWayBillWeightStatistic").click(function () {
        addTab($(this).text(), "/Huayu_ViewWayBillWeightStatistic/Index", "icon-reload");
    });

//    $('#LeftNavigation').accordion({
//        onSelect: function (title, index) {
//            console.info(title);
//        }
//    });

    $("#btnRelogin").click(function () {
        $.messager.confirm("操作提示", "您确定需要注销吗?", function (ok) {
            var bLogOutOK = false;
            if (ok) {
                $.ajax({
                    type: "GET",
                    url: LogOutURL,
                    data: "",
                    async: false,
                    cache: false,
                    beforeSend: function (XMLHttpRequest) {

                    },
                    success: function (msg) {
                        var msg = eval("(" + msg + ")");
                        if (msg.result.toLowerCase() == "ok") {
                            bLogOutOK = true;
                        } else {
                            $.messager.alert('操作提示', msg.message, 'error');
                            bLogOutOK = false;
                        }
                    },
                    complete: function (XMLHttpRequest, textStatus) {

                    },
                    error: function () {

                    }
                });
            }
            if (bLogOutOK) {
                window.location = "/Login/Index";
            }
        });
    });

    setInterval(function () {
        if (!bGetDateOK) {
            $.ajax({
                type: "GET",
                url: GetDateTimeURL,
                data: "",
                async: true,
                cache: false,
                beforeSend: function (XMLHttpRequest) {

                },
                success: function (msg) {
                    $("#lblCurrentDate").html(new Date(Date.parse(msg.replace(/-/g, "/"))).toLongDateTimeString());
                    bGetDateOK = true;

                    var date = new Date();
                    var now = "";
                    now = date.getFullYear() + "-";
                    now = now + (date.getMonth() + 1) + "-";  //取月的时候取的是当前月-1如果想取当前月+1就可以了
                    now = now + date.getDate() + " ";
                    now = now + date.getHours() + ":";
                    now = now + date.getMinutes() + ":";
                    now = now + date.getSeconds() + "";

                    msg = new Date(Date.parse(msg.replace(/-/g, "/")));
                    now = new Date(Date.parse(now.replace(/-/g, "/")));

                    iDifferent = dateDiff(now, msg, "s");

                },
                complete: function (XMLHttpRequest, textStatus) {

                },
                error: function () {

                }
            });
        } else {
            var date = new Date();
            var now = "";
            now = date.getFullYear() + "-";
            now = now + (date.getMonth() + 1) + "-";  //取月的时候取的是当前月-1如果想取当前月+1就可以了
            now = now + date.getDate() + " ";
            now = now + date.getHours() + ":";
            now = now + date.getMinutes() + ":";
            now = now + date.getSeconds() + "";

            now = new Date(Date.parse(now.replace(/-/g, "/")));
            $("#lblCurrentDate").html(new Date(Date.parse(dateCon(DateFormat(now, "yyyy-MM-dd HH:mm:ss"), iDifferent).replace(/-/g, "/"))).toLongDateTimeString());
        }

    }, 1000);
});