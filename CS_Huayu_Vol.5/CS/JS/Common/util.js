
////////////////////////////////////////////////////////////////////////////////////////////////
/// javascript 的基础功能扩充
////////////////////////////////////////////////////////////////////////////////////////////////
(function () {
    ///定义文件标识符
    window.util3e6299dbd6d04e238eb64ae42c423b96 = "util.js";

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// javascript 的字符串函数功能扩充。
    /// 这部分不依赖于任何其他 javascript 组件。
    ////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////
    /// 判断传入的字符串是否为Null或者为空字符串。
    ////////////////////////////////////////////////
    String.isNullOrEmpty = function (str) { return str == undefined || str == null || str == ""; };
    String.prototype.isNullOrEmpty = function () { return String.isNullOrEmpty(this); };

    ////////////////////////////////////////////////
    /// 判断传入的字符串是否为Null或者为空字符串或者全是空格。
    ////////////////////////////////////////////////
    String.isNullOrWhiteSpace = function (str) { return String.isNullOrEmpty(str) || String(str).trim() == ""; };
    String.prototype.isNullOrWhiteSpace = function () { return String.isNullOrWhiteSpace(this); };


    ///格式化字符串，类似于 .NET 中的 string.format 函数功能
    ///使用方法：String.format('字符串{0}字符串{1}字符串','第一个变量','第二个变量');
    String.format = function (str) {
        for (var i = 0; i < arguments.length - 1; i++) { str = str.replaceAll("{" + i + "}", arguments[i + 1]); }
        return str;
    };
    String.prototype.format = function () {
        var str = String(String.isNullOrEmpty(this) ? "" : this);
        str = String(str);
        for (var i = 0; i < arguments.length; i++) { str = str.replaceAll("{" + i + "}", arguments[i]); }
        return str;
    };


    ////////////////////////////////////////////////
    /// 判断当前字符串对象是否包含指定的字符串内容。
    ////////////////////////////////////////////////
    String.contains = function (str, val) { return String(str).indexOf(val) > -1; };
    String.prototype.contains = function (val) { return String.contains(this, val); };


    ////////////////////////////////////////////////
    /// 字符串反转，该方法将返回原字符串内容反转后的结果，而不会更改其原来的值。
    ////////////////////////////////////////////////
    String.reverse = function (str) {
        var charArray = new Array();
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        for (var i = str.length - 1; i > -1; i--) { charArray.push(str[i]); }
        return charArray.join("");
    };
    String.prototype.reverse = function () { return String.reverse(this); };


    ////////////////////////////////////////////////
    /// 用新字符串替换与给定字符串匹配的所有子串
    ////////////////////////////////////////////////
    String.replaceAll = function (str, substr, replacement) {
        if (replacement.length > substr.length && replacement.indexOf(substr) > -1) {
            throw new Error("新字符串中含有被字符串的所有内容并且长度大于被字符串");
        }
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        while (str.indexOf(substr) > -1) { str = str.replace(substr, replacement); }
        return str;
    };
    String.prototype.replaceAll = function (substr, replacement) { return String.replaceAll(this, substr, replacement); };


    ////////////////////////////////////////////////
    /// 去除字符串左边的空格。
    ////////////////////////////////////////////////
    String.ltrim = function (str) {
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        return str.replace(/(^\s*)/g, "");
    };
    String.prototype.ltrim = function () { return String.ltrim(this); };


    ////////////////////////////////////////////////
    /// 去除字符串右边的空格。
    ////////////////////////////////////////////////
    String.rtrim = function () {
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        return str.replace(/(\s*$)/g, "");
    };
    String.prototype.rtrim = function () { return String.rtrim(this); };


    ////////////////////////////////////////////////
    /// 去除字符串左右两边的空格。
    ////////////////////////////////////////////////
    String.trim = function (str) {
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        return str.replace(/(^\s*)|(\s*$)/g, "");
    };
    String.prototype.trim = function () { return String.trim(this); };


    ////////////////////////////////////////////////
    /// 截取当前字符串左边的指定长度内容。
    ////////////////////////////////////////////////
    String.left = function (str, len) {
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        if (!window.isNumeric(len)) { len = str.length; } else {
            if (parseInt(len, 10) < 0 || parseInt(len, 10) > str.length) { len = str.length; }
        }
        return str.substr(0, len);
    };
    String.prototype.left = function (len) { return String.left(this, len); };


    ////////////////////////////////////////////////
    /// 截取当前字符串右边的指定长度内容。
    ////////////////////////////////////////////////
    String.right = function (str, len) {
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        if (!window.isNumeric(len)) { len = str.length; } else {
            if (parseInt(len, 10) < 0 || parseInt(len, 10) > str.length) { len = str.length; }
        }
        return str.substring(str.length - len, str.length);
    };
    String.prototype.right = function (len) { return String.right(this, len); };


    ////////////////////////////////////////////////
    /// 返回字符串中的的字符，注意从 0 开始。
    ////////////////////////////////////////////////
    String.mid = function (str, start, len) {
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        return str.substr(start, len)
    };
    String.prototype.mid = function (start, len) { return String.mid(this, start, len); };


    ////////////////////////////////////////////////
    /// 计算字符串的打印长度。
    ////////////////////////////////////////////////
    String.lengthOfPrint = function (str) {
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        return str.replace(/[^\x00-\xff]/g, "**").length;
    };
    String.prototype.lengthOfPrint = function () { return String.lengthOfPrint(this); };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否以指定的字符串开头。
    ////////////////////////////////////////////////
    String.startsWith = function (str, val) {
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        return str.substr(0, val.length) == val
    };
    String.prototype.startsWith = function (val) { return String.startsWith(this, val); };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否以指定的字符串结尾。
    ////////////////////////////////////////////////
    String.endsWith = function (str, val) {
        str = String.isNullOrEmpty(str) ? "" : str;
        str = String(str);
        return str.substr(str.length - val.length) == val;
    };
    String.prototype.endsWith = function (val) { return String.endsWith(this, val); };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的长日期格式。
    ////////////////////////////////////////////////
    String.prototype.isLongDate = function () {
        var r = this.replace(/(^\s*)|(\s*$)/g, "").match(/^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$/);
        if (r == null) { return false; }
        var d = new Date(r[1], r[3] - 1, r[4], r[5], r[6], r[7]);
        return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4] && d.getHours() == r[5] && d.getMinutes() == r[6] && d.getSeconds() == r[7]);
    };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的段日期格式。
    ////////////////////////////////////////////////
    String.prototype.isShortDate = function () {
        var r = this.replace(/(^\s*)|(\s*$)/g, "").match(/^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2})$/);
        if (r == null) { return false; }
        var d = new Date(r[1], r[3] - 1, r[4]);
        return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4]);
    };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的日期格式。
    ////////////////////////////////////////////////
    String.prototype.isDate = function () {
        return this.isLongDate() || this.isShortDate();
    };


    ////////////////////////////////////////////////
    /// 判断当前 String 独享是否是正确的电话号码格式(中国)。
    ////////////////////////////////////////////////
    String.prototype.isTel = function () {
        return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(this);
    };

    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的手机号码格式(中国)。
    ////////////////////////////////////////////////
    String.prototype.isMobile = function () {
        return /^(13|15|18)\d{9}$/i.test(this);
    };

    /// 判断当前 String 对象是否是正确的电话号码或者手机号码格式(中国)
    String.prototype.isTelOrMobile = function () {
        return this.isTel() || this.isMobile();
    };

    /// 判断当前 String 对象是否是正确的传真号码格式
    String.prototype.isFax = function () {
        return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(this);
    };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的 电子邮箱地址(Email) 格式。
    ////////////////////////////////////////////////
    String.prototype.isEmail = function () {
        return /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/.test(this);
    };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的 邮政编码(中国) 格式。
    ////////////////////////////////////////////////
    String.prototype.isZipCode = function () {
        return /^[\d]{6}$/.test(this);
    };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是否存在汉字字符。
    ////////////////////////////////////////////////
    String.prototype.existChinese = function () {
        //[\u4E00-\u9FA5]為漢字﹐[\uFE30-\uFFA0]為全角符號
        return /^[\x00-\xff]*$/.test(this);
    };

    /// 验证中文
    String.prototype.isChinese = function () {
        return /^[\u0391-\uFFE5]+$/i.test(this);
    };

    /// 验证英文
    String.prototype.isEnglish = function () {
        return /^[A-Za-z]+$/i.test(this);
    };

    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的 文件名称(路径) 格式。
    ////////////////////////////////////////////////
    String.prototype.isFileName = function () {
        return !/[\\\/\*\?\|:"<>]/g.test(this);
    };

    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的 IPv4 地址格式。
    ////////////////////////////////////////////////
    String.prototype.isIP = function () {
        var reSpaceCheck = /^(\d+)\.(\d+)\.(\d+)\.(\d+)$/;
        if (reSpaceCheck.test(this)) {
            this.match(reSpaceCheck);
            if (RegExp.$1 <= 255 && RegExp.$1 >= 0
                    && RegExp.$2 <= 255 && RegExp.$2 >= 0
                    && RegExp.$3 <= 255 && RegExp.$3 >= 0
                    && RegExp.$4 <= 255 && RegExp.$4 >= 0) {
                return true;
            } else { return false; }
        } else { return false; }
    };

    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的 url 格式。
    ////////////////////////////////////////////////
    String.prototype.isUrl = function () {
        var strRegex = "^((https|http|ftp|rtsp|mms)?://)"
    + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //ftp的user@
          + "(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP形式的URL- 199.194.52.184
          + "|" // 允许IP和DOMAIN（域名）
          + "([0-9a-z_!~*'()-]+\.)*" // 域名- www.
          + "([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // 二级域名
          + "[a-z]{2,6})" // first level domain- .com or .museum
          + "|" // 允许为本机
          + "localhost|127.0.0.1"   // 允许为本机地址
          + "(:[0-9]{1,4})?" // 端口- :80
          + "((/?)|" // a slash isn't required if there is no file name
          + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
        var re = new RegExp(strRegex);
        return re.test(this);
    };

    ///判断是否为合法的 ipv4 或者 url 格式
    String.prototype.isUrlOrIP = function () { return this.isUrl() || this.isIP(); };

    ///判断是否为合法的货币格式
    String.prototype.isCurrency = function () { return /^d{0,}(\.\d+)?$/i.test(this); };

    ///判断是否为合法的 QQ 号码格式
    String.prototype.isQQ = function () { return /^[1-9]\d{4,10}$/i.test(this); };

    ///判断是否为合法的 MSN 帐号格式
    String.prototype.isMSN = function () { return this.isEmail(); };

    ///验证是否包含空格和非法字符Z
    String.prototype.isUnNormal = function () { return /.+/i.test(this); };

    ///验证是否为合法的车牌号码
    String.prototype.isCarNo = function () { return /^[\u4E00-\u9FA5][\da-zA-Z]{6}$/.test(this); };

    ///验证是否为合法的汽车发动机序列号
    String.prototype.isCarEngineNo = function () { return /^[a-zA-Z0-9]{16}$/.test(this); };

    ///验证是否可以作为合法的用户名字符
    String.prototype.isUserName = function () { return /^[a-zA-Z][a-zA-Z0-9_]{5,15}$/i.test(this); };

    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的 身份证号码(中国) 格式。
    ////////////////////////////////////////////////
    String.prototype.isIDCard = function () {
        var iSum = 0;
        var info = "";
        var sId = this;
        var aCity = { 11: "北京", 12: "天津", 13: "河北", 14: "山西", 15: "内蒙古", 21: "辽宁", 22: "吉林", 23: "黑龙江", 31: "上海", 32: "江苏", 33: "浙江", 34: "安徽", 35: "福建", 36: "江西", 37: "山东", 41: "河南", 42: "湖北", 43: "湖南", 44: "广东", 45: "广西", 46: "海南", 50: "重庆", 51: "四川", 52: "贵州", 53: "云南", 54: "西藏", 61: "陕西", 62: "甘肃", 63: "青海", 64: "宁夏", 65: "新疆", 71: "台湾", 81: "香港", 82: "澳门", 91: "国外" };
        if (!/^\d{17}(\d|x)$/i.test(sId)) { return false; }
        sId = sId.replace(/x$/i, "a");
        //非法地区
        if (aCity[parseInt(sId.substr(0, 2), 10)] == null) { return false; }
        var sBirthday = sId.substr(6, 4) + "-" + Number(sId.substr(10, 2)) + "-" + Number(sId.substr(12, 2));
        var d = new Date(sBirthday.replace(/-/g, "/"))
        //非法生日
        if (sBirthday != (d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate())) { return false; }
        for (var i = 17; i >= 0; i--) { iSum += (Math.pow(2, i) % 11) * parseInt(sId.charAt(17 - i), 11); }
        if (iSum % 11 != 1) { return false; }
        return true;
    };

    /// 验证是否为整数格式
    String.prototype.isInteger = function () { return /^[+]?[1-9]+\d*$/i.test(this); };

    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的 数字 格式。
    ////////////////////////////////////////////////
    String.prototype.isNumeric = function (flag) {
        //验证是否是数字
        if (isNaN(this)) { return false; }
        if (arguments.length == 0) { return true; }
        switch (flag) {
            case "":
                return true;
            case "+":        //正数
                return /(^\+?|^\d?)\d*\.?\d+$/.test(this);
            case "-":        //负数
                return /^-\d*\.?\d+$/.test(this);
            case "i":        //整数
                return /(^-?|^\+?|\d)\d+$/.test(this);
            case "+i":        //正整数
                return /(^\d+$)|(^\+?\d+$)/.test(this);
            case "-i":        //负整数
                return /^[-]\d+$/.test(this);
            case "f":        //浮点数
                return /(^-?|^\+?|^\d?)\d*\.\d+$/.test(this);
            case "+f":        //正浮点数
                return /(^\+?|^\d?)\d*\.\d+$/.test(this);
            case "-f":        //负浮点数
                return /^[-]\d*\.\d$/.test(this);
            default:        //缺省
                return true;
        }
    };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否是正确的 颜色(#FFFFFF形式) 格式。
    ////////////////////////////////////////////////
    String.prototype.isColor = function () {
        var temp = this;
        if (temp == "") return true;
        if (temp.length != 7) return false;
        return (temp.search(/\#[a-fA-F0-9]{6}/) != -1);
    };


    ////////////////////////////////////////////////
    /// 判断当前 String 对象是否可以作为安全密码字符(由字符木数字组成，至少 6 位).
    ////////////////////////////////////////////////
    String.prototype.isSafePassword = function () {
        return !(/^(([A-Z]*|[a-z]*|\d*|[-_\~!@#\$%\^&\*\.\(\)\[\]\{\}<>\?\\\/\'\"]*)|.{0,5})$|\s/.test(this));
    };

    ////////////////////////////////////////////////
    /// 转换成全角
    ////////////////////////////////////////////////
    String.prototype.toCase = function () {
        var tmp = "";
        for (var i = 0; i < this.length; i++) {
            if (this.charCodeAt(i) > 0 && this.charCodeAt(i) < 255) { tmp += String.fromCharCode(this.charCodeAt(i) + 65248); }
            else { tmp += String.fromCharCode(this.charCodeAt(i)); }
        }
        return tmp
    };


    ////////////////////////////////////////////////
    /// 对字符串进行Html编码。
    ////////////////////////////////////////////////
    String.prototype.toHtmlEncode = function () {
        var str = this;
        str = str.replace(/&/g, "&amp;");
        str = str.replace(/</g, "&lt;");
        str = str.replace(/>/g, "&gt;");
        str = str.replace(/\'/g, "&apos;");
        str = str.replace(/\"/g, "&quot;");
        str = str.replace(/\n/g, "<br>");
        str = str.replace(/\ /g, "&nbsp;");
        str = str.replace(/\t/g, "&nbsp;&nbsp;&nbsp;&nbsp;");
        return str;
    };


    ////////////////////////////////////////////////
    /// 转换成日期。
    ////////////////////////////////////////////////
    String.prototype.toDate = function () {
        try { return new Date(this.replace(/-/g, "\/")); }
        catch (e) { return null; }
    };








    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// javascript 的数组函数功能扩充。
    /// 该扩展不依赖于任何其他 javascript 组件。
    ////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////
    /// 确认数组中是否包含指定的元素。
    ////////////////////////////////////////////////
    Array.prototype.contains = function (item, compare) {
        var result = false;
        for (var i = 0; i < this.length; i++) {
            var b = (item == this[i]);
            if (b || (window.isFunction(compare) && compare.call(this, item, this[i]) == true)) {
                result = true; break;
            }
        }
        return result;
    };

    ///过滤复制
    Array.prototype.ruleCopy = function (item, compare) {
        var temps = new Array();
        for (var i = 0; i < this.length; i++) {
            var b = item == this[i];
            if (!(b || (window.isFunction(compare) && compare.call(this, item, this[i]) == true))) { temps.push(this[i]); }
        }
        return temps;
    };

    ///移除数组中的指定项，并返回一个新的数组
    Array.prototype.remove = function (item, compare) {
        var temps = new Array();
        for (var i = 0; i < this.length; i++) {
            var b = item == this[i];
            if (!(b || (window.isFunction(compare) && compare.call(this, item, this[i]) == true))) { temps.push(this[i]); }
        }
        return temps;
    };

    ///如果数组中不存在指定的项，则添加
    Array.prototype.merge = function (item, compare) {
        if (!this.contains(item, compare)) { this.push(item); }
    };


    ///复制数组内的所有项到一个新的数组中。
    Array.prototype.copyTo = function (array) {
        for (var i = 0; i < this.length; i++) { array.push(this[i]); }
    };

    ///创建一个和当前数组对象相同的数组并返回
    Array.prototype.clone = function () {
        var temp = [];
        this.copyTo(temp);
        return temp;
    };

    ///根据比较函数获取数组中的元素集合，返回的是一个新的数组。
    ///参数 finder 为一个回调函数，该回调函数签名为 function(item)，其中参数item为数组中的元素遍历值。如果该函数返回 true，则表示该item是期望的值将会被返回。
    Array.prototype.filter = function (finder) {
        var array = new Array();
        for (var i = 0; i < this.length; i++) {
            var item = this[i];
            if (window.isFunction(finder) && finder.call(item, item) == true) { array.push(item); }
        }
        return array;
    };

    ///去除数组中重复的元素使之只保留一个，返回的是一个新的数组。
    Array.prototype.distinct = function (compare) {
        var array = new Array();
        for (var i = 0; i < this.length; i++) {
            var item = this[i];
            if (i == 0) { array.push(item); } else { array.merge(item, compare); }
        }
        return array;
    };



    ///获取数组中最大值的项

    ///获取数组中值等于最大值的集合

    ///获取数组中最小值的项

    ///获取数组中值等于最小值的集合







    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// javascript 的日期函数功能扩充。
    /// 该扩展不依赖于任何其他 javascript 组件。
    ////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////
    /// 返回当前日期对象的格式化字符值。
    ////////////////////////////////////////////////
    Date.prototype.format = function (format) {
        var f = function (date, format) {
            if (!date) return;
            if (!format) format = "yyyy-MM-dd";
            switch (typeof date) {
                case "string":
                    date = new Date(date.replace(/-/, "/"));
                    break;
                case "number":
                    date = new Date(date);
                    break;
            }
            if (!date instanceof Date) return;
            var dict = {
                "yyyy": date.getFullYear(),
                "M": date.getMonth() + 1,
                "d": date.getDate(),
                "H": date.getHours(),
                "m": date.getMinutes(),
                "s": date.getSeconds(),
                "MM": ("" + (date.getMonth() + 101)).substr(1),
                "dd": ("" + (date.getDate() + 100)).substr(1),
                "HH": ("" + (date.getHours() + 100)).substr(1),
                "mm": ("" + (date.getMinutes() + 100)).substr(1),
                "ss": ("" + (date.getSeconds() + 100)).substr(1)
            };
            return format.replace(/(yyyy|MM?|dd?|HH?|ss?|mm?)/g, function () {
                return dict[arguments[0]];
            });
        }
        return f(this, format);
    };



    ////////////////////////////////////////////////
    /// 获取当前日期时间的长字符串格式，返回的日期时间字符串格式如 “2012年08月06日 星期一, 上午 11:42:45”
    ////////////////////////////////////////////////
    Date.prototype.toLongDateTimeString = function () {
        var timeString;
        var year, month, day, hours, minutes, seconds, timePath, weekDay;
        var weekArray = new Array(" 星期日", " 星期一", " 星期二", " 星期三", " 星期四", " 星期五", " 星期六");
        year = this.getFullYear();
        month = this.getMonth() + 1;
        day = this.getDate();
        hours = this.getHours();
        minutes = this.getMinutes();
        seconds = this.getSeconds();
        weekDay = weekArray[this.getDay()];
        if (hours >= 23 || hours <= 0) {
            timePath = "午夜";
        } else if (hours >= 1 && hours <= 5) {
            timePath = "凌晨";
        } else if (hours >= 6 && hours <= 7) {
            timePath = "早上";
        } else if (hours >= 8 && hours <= 11) {
            timePath = "上午";
        } else if (hours >= 12 && hours <= 13) {
            timePath = "中午";
        } else if (hours >= 14 && hours <= 17) {
            timePath = "下午";
        } else if (hours >= 18 && hours <= 19) {
            timePath = "傍晚";
        } else if (hours >= 20 && hours <= 22) {
            timePath = "晚上";
        }
        if (month < 10) { month = "0" + month; }
        if (day < 10) { day = "0" + day; }
        if (hours < 10) { hours = "0" + hours; }
        if (minutes < 10) { minutes = "0" + minutes; }
        if (seconds < 10) { seconds = "0" + seconds; }
        timeString = year + "年" + month + "月" + day + "日" + weekDay + ", " + timePath + " " + hours + ":" + minutes + ":" + seconds;
        return timeString;
    };








    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// javascript 工具，提供常用的 js 工具方法。
    /// 包括获取系统当前时间的字符串格式，url 解析等功能。
    ////////////////////////////////////////////////////////////////////////////////////////////////



    var _c1 = "Boolean Number String Function Array Date RegExp Object";
    var _c2 = {};
    var _c3 = _c1.split(" ");
    for (var i = 0; i < _c3.length; i++) {
        _c2["[object " + _c3[i] + "]"] = _c3[i].toLowerCase();
    }
    var core_toString = Object.prototype.toString;
    var core_hasOwn = Object.prototype.hasOwnProperty;


    ////////////////////////////////////////////////
    /// 获取指定对象的类型。
    ////////////////////////////////////////////////
    window.type = function (obj) { return obj == null ? String(obj) : _c2[core_toString.call(obj)] || "object"; };


    ////////////////////////////////////////////////
    /// 测试对象是否是窗口（有可能是Frame）。
    ////////////////////////////////////////////////
    window.isWindow = function (obj) { return obj != null && obj == obj.window; };

    ////////////////////////////////////////////////
    /// 测试对象是否是数组。
    ////////////////////////////////////////////////
    window.isArray = function (obj) { return window.type(obj) === "array"; };

    ////////////////////////////////////////////////
    /// 测试对象是否是函数。
    /// 注意：在IE浏览器里，浏览器提供的函数比如'alert'还有 DOM 元素的方法比如 'getAttribute' 将不认为是函数。
    ////////////////////////////////////////////////
    window.isFunction = function (obj) { return window.type(obj) === "function"; };

    ////////////////////////////////////////////////
    /// 测试对象是否是数字。
    /// 方法检查它的参数是否代表一个数值。如果是这样，它返回 true。否则，它返回false。该参数可以是任何类型的。
    ////////////////////////////////////////////////
    window.isNumeric = function (obj) { return !isNaN(parseFloat(obj)) && isFinite(obj); };

    ////////////////////////////////////////////////
    /// 测试对象是否是空对象（不包含任何属性）。
    /// 这个方法既检测对象本身的属性，也检测从原型继承的属性（因此没有使用hasOwnProperty）。
    ////////////////////////////////////////////////
    window.isEmptyObject = function (obj) {
        var name;
        for (name in obj) { return false; }
        return true;
    };

    ////////////////////////////////////////////////
    /// 测试对象是否为空（不包含任何属性的空对象、null、undefined、空字符串、全空格）。
    /// 这个方法既检测对象本身的属性，也检测从原型继承的属性（因此没有使用hasOwnProperty）。
    ////////////////////////////////////////////////
    window.isEmptyObjectOrNull = function (obj) {
        if (obj == null || obj == undefined) { return true; }
        if (window.type(obj) == "string") { return String(obj).isNullOrWhiteSpace(); }
        if (window.type(obj) == "object") { return window.isEmptyObject(obj); }
        return window.type(obj) == "null";
    };

    ////////////////////////////////////////////////
    /// 测试对象是否是纯粹的对象（通过 "{}" 或者 "new Object" 创建的）。
    ////////////////////////////////////////////////
    window.isPlainObject = function (obj) {
        if (!obj || window.type(obj) !== "object" || obj.nodeType || window.isWindow(obj)) { return false; }
        try {
            if (obj.constructor &&
				!core_hasOwn.call(obj, "constructor") &&
				!core_hasOwn.call(obj.constructor.prototype, "isPrototypeOf")) {
                return false;
            }
        } catch (e) { return false; }
        var key;
        for (key in obj) { }
        return key === undefined || core_hasOwn.call(obj, key);
    };




    ////////////////////////////////////////////////
    /// 在不弹出关闭提示确认的情况下直接关闭当前浏览器窗口。
    ////////////////////////////////////////////////
    window.closeNoConfirm = function () {
        window.top.opener = null;
        window.top.open('', '_self', '');
        window.top.close();
    };


    var _i = function () {
        var search = window.location.search;
        if (search.substr(0, 1) == "?") {
            search = search.substr(1, search.length - 1);
        }
        var json = { "request": [] };
        if (search.length > 0) {
            var parameters = search.split("&");
            for (var i = 0; i < parameters.length; i++) {
                var postion = parameters[i].indexOf("=");
                var name = parameters[i].substring(0, postion);
                var value = parameters[i].substr(postion + 1);
                json.request.push({ "name": name, "value": value });
                json.request[name] = value;
            }
        }
        return json;
    };

    ////////////////////////////////////////////////
    /// 添加 jquery 的扩展属性 request。该属性表示 url 的所有参数集合。
    /// 该属性为一个数组元素。数组中的每个元素都是为格式如 { "name" : "id", "value" : "1" } 的 json 数据对象。
    /// 也可以通过数组访问器快速访问某个特定名称参数值，方法如： window.request["id"]。
    ////////////////////////////////////////////////
    window.request = _i().request;


    ////////////////////////////////////////////////
    /// 判断当前浏览器窗口是否为顶级窗口。
    ////////////////////////////////////////////////
    window.isTopMost = (window == window.top);

    window.isTop = window.isTopMost;


    ////////////////////////////////////////////////
    /// 生成一个 Guid(全球唯一标识符) 值。
    /// 该方法生成的 Guid 值格式如 xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx 。
    ////////////////////////////////////////////////
    window.guid = function () {
        var result = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            result += n;
            if ((i == 8) || (i == 12) || (i == 16) || (i == 20)) { result += "-"; }
        }
        return result;
    };



    var _g1 = function () {
        var href = window.location.href;
        var pathname = window.location.pathname;
        return href.substr(0, href.lastIndexOf(pathname));
    };

    ////////////////////////////////////////////////
    /// 获取当前应用程序的完整主机地址部分，返回的结果格式如( http://127.0.0.1 )
    ////////////////////////////////////////////////
    window.hostPath = _g1();



    var _g2 = function () {
        var pathname = window.location.pathname;
        return window.hostPath + pathname;
    };

    ////////////////////////////////////////////////
    /// 返回当前页面不带参数的完整路径。
    ////////////////////////////////////////////////
    window.currentUri = _g2();


    var _g3 = function () { return window.currentUri.substr(0, window.currentUri.lastIndexOf("/")); };

    ////////////////////////////////////////////////
    /// 返回当前页面所在目录的完整路径。
    ////////////////////////////////////////////////
    window.currentPath = _g3();




    ////////////////////////////////////////////////
    /// 表示当前应用程序所嵌套的虚拟目录的层数。默认为 0，表示未嵌套虚拟目录。
    ////////////////////////////////////////////////
    window.rootDegree = 1;

    ////////////////////////////////////////////////
    /// 返回当前应用程序（含站点名或者虚拟目录路径）的完整根路径。
    /// 该方法依赖于全局参数 window.rootDegree 的值。
    /// window.rootDegree 该全局参数表示 虚拟目录 的层数。
    /// window.rootDegree 参数设置正确，该方法方能返回正确的结果。
    /// window.rootDegree 默认值为 0，即应用程序没有设置虚拟目录。
    ////////////////////////////////////////////////
    window.getRootPath = function () {
        var result = window.hostPath;
        var pathname = window.location.pathname;
        if (pathname.indexOf("/") > -1) {
            var paths = pathname.split("/");
            var temps = new Array();
            for (var i = 0; i < paths.length; i++) { if (paths[i].length > 0) { temps.push(paths[i]); } }
            for (var i = 0; i < window.rootDegree && i < temps.length; i++) { result += "/" + temps[i]; }
        }
        return result;
    }


    ////////////////////////////////////////////////
    /// 返回当前应用程序（含站点名或者虚拟目录路径）的完整根路径。
    ////////////////////////////////////////////////
    window.rootPath = window.getRootPath();

    ////////////////////////////////////////////////
    /// 根据传入的 uri 地址返回该 uri 相对于应用程序的完整客户端访问url地址。
    /// 传入的 uri 地址应为相对于应用程序根路径的完整地址。
    /// 该方法依赖于当前文件的 window.rootPath 属性。
    ////////////////////////////////////////////////
    window.resolveUrl = function (url) {
        if (url == undefined || url == null) { return null; }
        if (url.isUrl()) { return url; }
        url = url.replaceAll("\\", "/");
        while (url.substring(0, 2) == "./" || url.substring(0, 1) == "/") { url = url.substring(1, url.length); }
        var tmps1 = window.rootPath.split("/");
        var tmps2 = url.split("/");
        while (tmps1.length > 3 && tmps2.length > 1 && tmps2[0] == "..") { tmps1.pop(); tmps2.shift(); }
        while (tmps2.length > 1 && tmps2[0] == "..") { tmps2.shift(); }
        return tmps1.join("/") + "/" + tmps2.join("/");
    };

    ////////////////////////////////////////////////
    /// 根据传入的 uri 地址返回该 uri 相对于应用程序的完整客户端访问url地址。
    ////////////////////////////////////////////////
    window.resolveClientUrl = function (url) { return window.resolveUrl(url); };


    ////////////////////////////////////////////////
    /// 屏蔽当前页面的 HTML 源代码。
    /// 有 bug，不建议使用。
    ////////////////////////////////////////////////
    window.shieldSourceCode = function () {
        var source = document.body.innerHTML;  //获取文档的原有内容
        document.open();                 //打开文档
        document.close();                //关闭文档
        document.body.innerHTML = source;  //重新写入旧内容
    };


    ////////////////////////////////////////////////
    /// 屏蔽当前页面的鼠标右键默认事件，而调用指定的回调函数。
    /// 如果回调函数为空，则点击鼠标右键没有任何反应。
    ////////////////////////////////////////////////
    window.shieldContxtMenuEvent = function (callback) {
        document.oncontextmenu = function (e) {
            e.preventDefault();
            if (callback && window.type(callback) == "function") { callback.apply(this, arguments); }
        };
    };



    ////////////////////////////////////////////////
    /// 动态引入一个或多个 javascript 脚本文件至当前页面文档，并在引入成功后调用指定的回调函数。
    /// 参数 url 表示需要载入的 javascript 脚本路径；如果需要一次性载入多个脚本，则 url 可以是一个数组，数组中每个元素表示 javascript 脚本路径。
    ////////////////////////////////////////////////
    window.loadJs = function (urls, callbackFunc) {
        var load = function (url, callback) {
            url = window.resolveUrl(url);
            var type = "text/javascript";
            var language = "javascript";
            var heads = document.getElementsByTagName("head");
            var scripts = heads[0].getElementsByTagName("script");
            for (var i = 0; i < scripts.length; i++) {
                var s = scripts[i];
                if (s.src == url) { if (callback && window.isFunction(callback)) { callback.call(s); } return; }
            }
            var done = false;
            var script = document.createElement('script');
            script.type = type;
            script.language = language;
            script.src = url;
            script.onload = script.onreadystatechange = function () {
                if (!done && (!script.readyState || script.readyState == "loaded" || script.readyState == "complete")) {
                    done = true;
                    script.onload = script.onreadystatechange = null;
                    if (callback && window.isFunction(callback)) { callback.call(script); }
                }
            };
            heads[0].appendChild(script);
        };
        if (window.isArray(urls)) {
            for (var i = 0; i < urls.length; i++) {
                var url = urls[i];
                var callback = (i == urls.length - 1) ? callbackFunc : null;
                load(url, callback);
            }
        } else { load(urls, callbackFunc); }
    };

    ////////////////////////////////////////////////
    /// 一次性执行某个 javascript 脚本文件，并在执行成功后调用指定的回调函数。
    ////////////////////////////////////////////////
    window.runJs = function (url, callback) {
        window.loadJs(url, function () {
            document.getElementsByTagName("head")[0].removeChild(this);
            if (callback && window.isFunction(callback)) { callback(); }
        });
    };

    ////////////////////////////////////////////////
    /// 动态引入一个或多个 css 样式表文件至当前页面文档，并在引入成功后调用指定的回调函数。
    ////////////////////////////////////////////////
    window.loadCss = function (urls, callbackFunc) {
        var load = function (url, callback) {
            var link = document.createElement('link');
            link.rel = "stylesheet";
            link.type = "text/css";
            link.media = "screen";
            link.href = url;
            document.getElementsByTagName("head")[0].appendChild(link);
            if (callback && window.isFunction(callback)) { callback.call(link); }
        };
        if (window.isArray(urls)) {
            for (var i = 0; i < urls.length; i++) {
                var url = urls[i];
                var callback = (i == urls.length - 1) ? callbackFunc : null;
                load(url, callback);
            }
        } else { load(urls, callbackFunc); }
    };



    var matched, browser;
    var userAgentMatch = function (userAgent) {
        userAgent = userAgent.toLowerCase();
        var match = /(chrome)[ \/]([\w.]+)/.exec(userAgent) ||
		    /(webkit)[ \/]([\w.]+)/.exec(userAgent) ||
		    /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(userAgent) ||
		    /(msie) ([\w.]+)/.exec(userAgent) ||
		    userAgent.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(userAgent) || [];
        return {
            browser: match[1] || "",
            version: match[2] || "0"
        };
    };
    matched = userAgentMatch(window.navigator.userAgent);
    browser = {};

    if (matched.browser) {
        browser[matched.browser] = true;
        browser.version = matched.version;
    }

    // Chrome is Webkit, but Webkit is also Safari.
    if (browser.chrome) { browser.webkit = true; } else if (browser.webkit) { browser.safari = true; }
    ////////////////////////////////////////////////
    /// 获取浏览器的名称以及版本号。
    /// 判断浏览器版本示例：判断浏览器是否为IE：  window.browser.msie == true，判断浏览器是否为 Chrome：window.browser.chrome == true
    /// 判断浏览器版本号：window.browser.version，IE下可能的值为 6.0/7.0/8.0/9.0/10.0 等等。
    ////////////////////////////////////////////////
    window.browser = browser;



    ////////////////////////////////////////////////
    /// 对某个对象及其所有可见属性进行多次嵌套递归循环调用某个方法
    ///     obj: 目标对象
    ///     call: 要被针对目标对象循环调用的方法
    ///     times: 嵌套的层数
    ////////////////////////////////////////////////
    window.foreachCall = function (obj, call, times) {
        if (obj == undefined) { obj = window.Nullable.Object; }
        if (call == undefined) { call = window.Nullable.Function; }
        if (times == undefined || times < 0) { times = 0; }
        call.call(this, obj);
        for (var i = 1; i <= times; i++) {
            for (var key in obj) {
                window.foreachCall.call(this, obj[key], call, times - 1);
            }
        }
    };

    ////////////////////////////////////////////////
    /// 模仿 Object.freeze 方法禁用对象的扩展状态、密封状态和可写状态。
    ////////////////////////////////////////////////
    //禁用对象的扩展状态
    window.preventExtensions = function (obj, times) {
        window.foreachCall.call(this, obj, Object.preventExtensions, times);
    };
    //禁用对象的密封状态
    window.seal = function (obj, times) {
        window.foreachCall.call(this, obj, Object.seal, times);
    };
    //禁用对象的可写状态
    window.freeze = function (obj, times) {
        window.foreachCall.call(this, obj, Object.freeze, times);
    };

    ////////////////////////////////////////////////
    /// 模仿 window.setTimeout 的延迟方法调用
    ////////////////////////////////////////////////
    window.call = function (code, delay) {
        if (window.isEmptyObjectOrNull(code)) { code = window.Nullable.Function; }
        if (delay == undefined) { delay = 0; }
        window.setTimeout(code, delay);
    };


    ////////////////////////////////////////////////
    /// 声明空值集合
    ////////////////////////////////////////////////
    if (!window.Nullable) { window.Nullable = new Object(); }
    if (!window.Nullable.String) { window.Nullable.String = new String(); }
    if (!window.Nullable.Function) { window.Nullable.Function = new Function(); }
    if (!window.Nullable.Date) { window.Nullable.Date = new Date(); }
    if (!window.Nullable.Boolean) { window.Nullable.Bool = new Boolean(); }
    if (!window.Nullable.Object) { window.Nullable.Object = new Object(); }
    if (!window.Nullable.Number) { window.Nullable.Number = new Number(); };
    //禁用扩展属性的可写状态
    window.freeze(window.Nullable, 1);
    window.freeze(window.Nullable.String);
    window.freeze(window.Nullable.Function);
    window.freeze(window.Nullable.Date);
    window.freeze(window.Nullable.Boolean);
    window.freeze(window.Nullable.Object);
    window.freeze(window.Nullable.Number);

})();