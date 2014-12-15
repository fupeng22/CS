$(function () {
    var QueryAllWayBillURL = "/Huayu_ConfirmInStore/GetAllWayBillData";

    //    $('#ddlwbSerialNum').combogrid({
    //        panelWidth: 150,
    //        idField: 'wbSerialNum',
    //        textField: 'wbSerialNum',
    //        url: QueryAllWayBillURL,
    //        editable: true,
    //        mode: "remote",
    //        columns: [[
    //					{ field: 'wbSerialNum', title: '总运单号', width: 120, sortable: true,
    //					    sorter: function (a, b) {
    //					        return (a > b ? 1 : -1);
    //					    }
    //					}
    //				]]
    //    });

    $('#ddlwbSerialNum').combobox({
        url: QueryAllWayBillURL,
        valueField: 'id',
        textField: 'text',
        editable: true,
        panelHeight: 200
    });
});