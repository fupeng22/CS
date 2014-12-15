$(function () {
//    var mapOption = {
//        mapType: BMAP_NORMAL_MAP,
//        maxZoom: 18,
//        drawMargin: 0,
//        enableFulltimeSpotClick: true,
//        enableHighResolution: true
//    }

//    var map = new BMap.Map("container", mapOption); //初始化地图
//    //map.centerAndZoom(new BMap.Point(117.234963, 31.858815), 18);

//    map.addEventListener('load', function () {
//        $("#place").text("当前中心点经纬度： " + map.getCenter().lng + ', ' + map.getCenter().lat);
//    });

//    
//    map.enableScrollWheelZoom();
//    map.addControl(new BMap.NavigationControl());               // 添加平移缩放控件
//    map.addControl(new BMap.ScaleControl());                    // 添加比例尺控件
//    map.addControl(new BMap.OverviewMapControl());              //添加缩略地图控件
//    map.enableScrollWheelZoom();                            //启用滚轮放大缩小
//    map.addControl(new BMap.MapTypeControl());          //添加地图类型控件

    //    map.centerAndZoom("合肥");

    var map = new BMap.Map("container", { mapType: BMAP_PERSPECTIVE_MAP });   //设置3D图为底图
    var point = new BMap.Point(116.4035, 39.915);
    map.setCurrentCity("北京");          // 设置地图显示的城市 此项是必须设置的
    map.centerAndZoom(point, 19);
    map.enableScrollWheelZoom(true);                            //启用滚轮放大缩小
    map.addControl(new BMap.NavigationControl());               // 添加平移缩放控件
    map.addControl(new BMap.ScaleControl());                    // 添加比例尺控件
    map.addControl(new BMap.OverviewMapControl());              //添加缩略地图控件
});