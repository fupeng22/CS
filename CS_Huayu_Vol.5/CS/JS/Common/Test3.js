var baseUrl = "http://api.map.baidu.com/geodata/v3/";
var ak = "gxF36PjOj7CqjGFijqbrq610";
function GetList() {
    $.ajax({
        type: "get",
        url: baseUrl + "geotable/list?ak=" + ak,
        dataType: "jsonp",
        success: function (data) {
            /*                 eval("data=" + data);*/
            var text = "执行状态:" + data.status + "<br/>";
            text += "结果：" + data.message + "<br/>";
            var tables = data.geotables;
            text += "您在LBS云建立了" + data.size + "张表<br/>";
            $("#showContainer").html(text);

            var tablesHTML = "<tr><td>id</td><td>表名</td><td>数据类型</td><td>修改时间</td><td>创建时间</td><td>是否发布到检索</td></tr>";
            for (var i = 0; i < tables.length; i++) {
                var leixing = tables[i].geotype;
                if (leixing == 1) {
                    leixing = "点";
                } else if (leixing == 2) {
                    leixing = "线";
                } else {
                    leixing = "面";
                }
                tablesHTML += "<tr><td>" + tables[i].id + "</td><td>" + tables[i].name + "</td><td>" + leixing + "</td><td>" + tables[i].create_time + "</td><td>" + tables[i].modify_time + "</td><td>" + (tables[i].is_published == 1 ? "是" : "否") + "</td></tr>";
            }
            $("#tablesDetail").html(tablesHTML);

        }
    });

}

    


function CreateTable() {
    var getURL = baseUrl + "geotable/create";
    $.ajax({
        type: "post",
        data: { "geotype": 1, "ak": ak, "is_published": parseInt($("#is_published").val()), "name": $("#name").val() },
        url: getURL,
        dataType: "json",
        success: function (data) {
            var text = "执行状态:" + data.status + ";";
            text += "结果：" + data.message + ";";
            text += "刚刚您在LBS云上新建的表ID是" + data.id;
            $("#createContainer").html(text);

        }
    });

}

function GetTable() {
    var getURL = baseUrl + "geotable/detail?ak=" + ak + "&id=" + $("#tableId").val();
    $.ajax({
        type: "get",
        url: getURL,
        dataType: "jsonp",
        success: function (data) {
            data = data.geotable;
            var tableHTML = "<tr><td>id</td><td>表名</td><td>数据类型</td><td>修改时间</td><td>创建时间</td><td>是否发布到检索</td></tr>";
            var leixing = data.geotype;
            if (leixing == 1) {
                leixing = "点";
            } else if (leixing == 2) {
                leixing = "线";
            } else {
                leixing = "面";
            }
            tableHTML += "<tr><td>" + data.id + "</td><td>" + data.name + "</td><td>" + leixing + "</td><td>" + data.create_time + "</td><td>" + data.modify_time + "</td><td>" + (data.is_published == 1 ? "是" : "否") + "</td></tr>";

            $("#tableDetail").html(tableHTML);

        }
    });

}

function DelTable() {
    var getURL = baseUrl + "geotable/delete";
    $.ajax({
        type: "post",
        data: { "id": $("#deleteTableId").val(), "ak": ak },
        url: getURL,
        async: true,
        dataType: "json",
        success: function (data) {

            if (data.status == 0) {
                //alert("删除成功!");
                GetList();
            } else {
                alert("状态码：" + data.status + ";信息：" + data.message);
            }
        },
        error: function (e) {
            console.info(e);
            //alert(e);
        }
    });
}


$(function () {
    //    var GetCodeURL = "/Test2/GetCode?";
    //    $("#btn1").click(function () {
    //        GetCodeURL = "/Test2/GetCode?type=" + encodeURI($("#seleType").val()) + "&str=" + encodeURI($("#txt1").val()) + "&width=" + encodeURI($("#width").val()) + "&height=" + encodeURI($("#height").val());
    //        $.ajax({
    //            type: "GET",
    //            url: GetCodeURL,
    //            data: "",
    //            async: false,
    //            cache: false,
    //            beforeSend: function (XMLHttpRequest) {

    //            },
    //            success: function (msg) {
    //                $("#span_Result").html(msg);
    //            },
    //            complete: function (XMLHttpRequest, textStatus) {

    //            },
    //            error: function () {

    //            }
    //        });
    //    });

    //    $("#btn2").click(function () {
    //        var QueryURL = "http://10.45.9.128:8181/ems/api/process";
    //        $.ajax({
    //            type: "POST",
    //            url: QueryURL,
    //            data: { parternID: "TESTJSON", serviceType: "RequestQuery", mailNo: "dfg", digest: "jyS6tAgzjT1QO%2Fb%2F%2FSMabw%3D%3D" },
    //            async: false,
    //            cache: false,
    //            contentType: "application/x-www-form-urlencoded; charset=utf-8",
    //            beforeSend: function (XMLHttpRequest) {

    //            },
    //            success: function (msg) {
    //                //console.info(msg);
    //                alert(msg);
    //            },
    //            complete: function (XMLHttpRequest, textStatus) {

    //            },
    //            error: function () {

    //            }
    //        });
    //    });

    //    $("#btn3").click(function () {
    //        $('#form_FileUpload').form('submit', {
    //            url: "http://10.45.9.128:8181/ems/api/process?parternID=TESTJSON&serviceType=RequestQuery&mailNo=dfg&digest=jyS6tAgzjT1QO%2Fb%2F%2FSMabw%3D%3D",
    //            success: function (data) {
    //                console.info(data);
    //            }
    //        });
    //    });

    //    $("#btn4").click(function () {
    //        $("#span_Result").printArea()
    //    });

   
    GetList();
});