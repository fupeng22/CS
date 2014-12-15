$(function () {
    $('#ddAuthority').combobox({
        data: [{ "text": "---请选择---", "id": "---请选择---" }, { "text": "一级权限", "id": "1" }, { "text": "二级权限", "id": "2" }, { "text": "三级权限", "id": "3" }, { "text": "四级权限", "id": "4"}],
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    $('#ddComment').combobox({
        data: [{ "text": "---请选择---", "id": "---请选择---" }, { "text": "天津机场海关", "id": "0" }, { "text": "TAT货运站", "id": "1" }, { "text": "货运代理", "id": "2"}],
        valueField: 'id',
        textField: 'text',
        editable: false,
        panelHeight: null
    });

    $('#ddAuthority').combobox("setValue", $("#Authority").val());
    $('#ddComment').combobox("setValue", $("#Comment").val());

    switch ($("#hid_chkiSendFirstPickGoodsEmail").val()) {
        case "1":
            break;
        default:
            $("#chkiSendFirstPickGoodsEmail").removeAttr("checked")
            break;
    }

    switch ($("#hid_chkiSendUnReleaseGoodsEmail").val()) {
        case "1":
            break;
        default:
            $("#chkiSendUnReleaseGoodsEmail").removeAttr("checked")
            break;
    }

    switch ($("#hid_chkiSendRejectGoodsEmail").val()) {
        case "1":
            break;
        default:
            $("#chkiSendRejectGoodsEmail").removeAttr("checked")
            break;
    }
});
