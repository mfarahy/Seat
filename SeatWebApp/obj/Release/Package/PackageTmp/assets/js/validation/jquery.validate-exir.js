
// this one requires the value to be the same as the first parameter
$.validator.methods.condition = function (value, element, param) {
    return $.bocrud.checkDepenExp(element, param);
};

var getNumber = function (val, elem, enforceTime) {
    if (typeof (val) == 'number') return val;

    if (val.indexOf(',') >= 0)
        val = val.split(',').join('');

    if (val.indexOf('/') > 0 || val.indexOf('\\') > 0 || val.indexOf('-') > 0) {

        var isMyCal = elem && $(elem).is('.hasDatepicker');
        var culture = '';
        if (isMyCal)
            culture = $(elem).attr('culture');
        if (isMyCal && culture == 'Persian') {
            var datepicker = $(elem).data('datepicker');
            var f = val.split('/');
            var time = null;
            if (f[2].indexOf(':') > 0) {
                f[2] = f[2].split(' ')[0];
            }
            var gd = new JalaliDate(parseInt(f[0]), parseInt(f[1]), parseInt(f[2])).getGregorianDate();
            var date = [];
            date.push(gd.getFullYear());
            date.push('/');
            date.push(gd.getMonth() < 10 ? '0' + gd.getMonth() : gd.getMonth());
            date.push('/');
            date.push(gd.getDate() < 10 ? '0' + gd.getDate() : gd.getDate());
            if (time != null) {
                date.push(time[0]);
                date.push(time[1]);
            }
            var sd = date.join('');

            if (val.indexOf(' ') > 0) {
                var time = val.split(' ')[1];
                var tf = time.split(':');
                var ta = [];
                ta.push(tf[0].length < 2 ? '0' + tf[0] : tf[0]);
                ta.push(':');
                ta.push(tf[1].length < 2 ? '0' + tf[1] : tf[1]);
                if (tf.length < 3)
                    ta.push('00');
                else {
                    ta.push(':');
                    ta.push(tf[2].length < 2 ? '0' + tf[2] : tf[2]);
                }
                val = sd + ' ' + ta.join('');
            }
            else {
                if (enforceTime)
                    val = sd + ' 00:00:00';
            }
        }
        var date, time, ad;
        if (val.indexOf(' ') > 0) {
            var arr = val.split(' ');
            date = arr[0];
            time = arr[1].split(':').join('');
        } else {
            date = val;
            time = '';
        }

        if (date.indexOf('/') > 0)
            ad = date.split('/');
        if (date.indexOf('\\') > 0)
            ad = date.split('\\');
        if (date.indexOf('-') > 0)
            ad = date.split('-');
        return parseInt(ad.join('') + time);
    }


    if (val.indexOf('.') >= 0) return parseFloat(val);


    return parseInt(val);
}


$.validator.addMethod('min', function (value, element, param) {
    return this.optional(element) || getNumber(value, element) >= param;
});

$.validator.addMethod('max', function (value, element, param) {
    return this.optional(element) || getNumber(value, element) <= param;
});

$.validator.addMethod('range', function (value, element, param) {
    return this.optional(element) || (getNumber(value, element) >= param[0] && getNumber(value, element) <= param[1]);
});

$.validator.addMethod('expression', function (value, element, param) {

    var evalScript = $.bocrud.getExpEvalScript(param.context);

    var result;
    try {
        result = eval(evalScript);
    }
    catch (ex) { $.console.error("bocrud-2793:" + ex + ' on evaling ' + evalScript); }
    return result;
});


$.validator.addMethod('compare', function (value, element, param) {

    var ccv = param.value;

    if (param.op == "eq" && !param.control) {
        return value == ccv;
    }

    var enforceTime = ccv && ccv.indexOf(' ') > 0;

    if (param.control) {
        // قبلا به شکل زیر بود که کامنت شد
        // ایرادش این بود که چند تا کنترل با این اسم رو صفحه ممکنه باشه که باعث بروز مشکل می شه
        // - var cc = $('.bocrud-control[name="' + param.control + '"]');

        var cc = $(element).closest('.bocrud-page-form').find('.bocrud-control[name="' + param.control + '"]');

        if (cc.length) {
            var elem = cc.find('input,select,textarea');
            var rawValue = elem.val();
            if ($(elem).is('.hasDatepicker'))
                ccv = getNumber(rawValue, elem, rawValue && rawValue.indexOf(' ') > 0);
        } else {
            console.log("control " + param.control + " not found to compare value and validation will be ignored.");
            return true;
        }
    }


    if ($(element).is(':checkbox')) {
        var val = $(element).is(':checked');
        var d = ccv.toLowerCase() == 'on' || ccv.toLowerCase() == 'true';
        return val == d;
    }

    var v1 = getNumber(value, element, enforceTime), v2 = getNumber(ccv);

    switch (param.op) {
        case "gt": return v1 > v2;
        case "ge": return v1 >= v2;
        case "lt": return v1 < v2;
        case "le": return v1 <= v2;
        case "eq": return v1 == v2;
        case "ne": return v1 != v2;
    }

    return true;
});

$.validator.addMethod('nationalCode', function (value, element, param) {

    var ccv = param.value;

    //در صورتی که کد ملی وارد شده تهی باشد

    if (!value || value.length == 0)
        return true;


    //در صورتی که کد ملی وارد شده طولش کمتر از 10 رقم باشد
    if (value.length != 10)
        return false;

    //در صورتی که کد ملی ده رقم عددی نباشد
    if (!/^\d{10}$/i.test(value))
        return false;

    //در صورتی که رقم‌های کد ملی وارد شده یکسان باشد
    var allDigitEqual = true;
    for (var j = 0; j < value.length; ++j)
        for (var i = 0; i < value.length; ++i)
            if (value[j] != value[i]) {
                allDigitEqual = false;
                break;
            }
    if (allDigitEqual) return false;


    //عملیات شرح داده شده در بالا
    var chArray = value;
    var num0 = parseInt(chArray[0]) * 10;
    var num2 = parseInt(chArray[1]) * 9;
    var num3 = parseInt(chArray[2]) * 8;
    var num4 = parseInt(chArray[3]) * 7;
    var num5 = parseInt(chArray[4]) * 6;
    var num6 = parseInt(chArray[5]) * 5;
    var num7 = parseInt(chArray[6]) * 4;
    var num8 = parseInt(chArray[7]) * 3;
    var num9 = parseInt(chArray[8]) * 2;
    var a = parseInt(chArray[9]);

    var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
    var c = b % 11;

    return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));

    return true;
});

$.validator.addMethod('digit', function (value, element, param) {
    if (!value || value.length == 0) return true;
    var allDigitEqual = eval(param.context);
    for (var j = 0; j < value.length; ++j)
        if (value.charCodeAt(j) < 48 || value.charCodeAt(j) > 57)
            return false;

    for (var j = 0; j < value.length; ++j)
        for (var i = 0; i < value.length; ++i)
            if (value[j] != value[i]) {
                allDigitEqual = false;
                break;
            }
    if (allDigitEqual) return false;

    return true;
});

$.validator.addMethod('IranLegalNationalIdentityValidator', function (value, element, param) {

    if (!value || value.length == 0)
        return true;

    if (value.trim().length != 11)
        return false;

    var intModSum = 0;
    var intChar = [];

    for (var i = 0; i <= 9; ++i) {
        var temp = parseInt(value[i]);
        if (isNaN(temp))
            return false;
        intChar.push(temp);
    }

    for (var i = 0; i <= 9; ++i)
        intChar[i] += intChar[9] + 2;

    intChar[0] *= 29;

    intChar[1] *= 27;

    intChar[2] *= 23;

    intChar[3] *= 19;

    intChar[4] *= 17;

    intChar[5] *= 29;

    intChar[6] *= 27;

    intChar[7] *= 23;

    intChar[8] *= 19;

    intChar[9] *= 17;

    var intSum = 0;

    for (var i = 0; i <= 9; ++i)
        intSum += intChar[i];

    intModSum = intSum % 11;

    if (intModSum.toString().length > 1)
        intModSum = parseInt(intModSum.toString().substring(1, 2));

    if (value[10].toString() != intModSum.toString())
        return false;

    return true;
});

$.validator.addMethod('persianTerm', function (value, element, param) {

    if (!value || value.length == 0) return true;

    var _charset = [
        32, // 
        45, //-
        48, //0
        49, //1
        50, //2
        51, //3
        52, //4
        53, //5
        54, //6
        55, //7
        56, //8
        57, //9
        1548, //،
        1567, //؟
        1569, //ء
        1570, //آ
        1571, //أ
        1572, //ؤ
        1574, //ئ
        1575, //ا
        1576, //ب
        1578, //ت
        1579, //ث
        1580, //ج
        1581, //ح
        1582, //خ
        1583, //د
        1584, //ذ
        1585, //ر
        1586, //ز
        1587, //س
        1588, //ش
        1589, //ص
        1590, //ض
        1591, //ط
        1592, //ظ
        1593, //ع
        1594, //غ
        1600, //ـ
        1601, //ف
        1602, //ق
        1603, //ك
        1604, //ل
        1605, //م
        1606, //ن
        1607, //ه
        1608, //و
        1610, //ي
        1632, //٠
        1633, //١
        1634, //٢
        1635, //٣
        1636, //٤
        1637, //٥
        1638, //٦
        1639, //٧
        1640, //٨
        1641, //٩
        1662, //پ
        1670, //چ
        1688, //ژ
        1705, //ک
        1711, //گ
        1740, //ی
    ];

    var p = eval("(" + param.context + ")");

    var invalidCharacters = [];
    for (var i = 0; i < value.length; ++i) {
        var c = value.charCodeAt(i);
        var found = false;
        for (var j = 0; j < _charset.length; ++j)
            if (_charset[j] == c) {
                found = true;
                break;
            }
        if (!found && p.additional && p.additional.length > 0) {
            for (var j = 0; j < p.additional.length; ++j)
                if (p.additional[j] == c) {
                    found = true;
                    break;
                }
        }
        if (!p.allowNumber && (c >= 48 && c <= 57) || (c >= 1632 && c <= 1641)) {
            found = false;
        }
        if (!p.allowSpace && c == 32) {
            found = false;
        }
        if (!found)
            invalidCharacters.push(c);
    }

    if (invalidCharacters.length > 0)
        return false;



    return true;
});

$.validator.addMethod('domain', function (value, element, param) {
    if (!value || value.length == 0) return true;

    return /^((\w+):\/\/)?([w]{3}\.)?([A-Za-z0-9-_]+(\.[A-Za-z0-9-_]+)+)(:\d+)?$/i.test(value);
});

$.validator.addMethod('xdatetime', function (value, element, param) {

    if (!value || value.length == 0) return true;

    var r = /^([0-9]{4})\/([01]?[0-9])\/([0123]?[0-9])(\s([012][0-9]):([0-5][0-9]))?$/i;
    if (!r.test(value)) return false;
    var mg = r.exec(value);
    var year = parseInt(mg[1], 10);
    var month = parseInt(mg[2], 10);
    var day = parseInt(mg[3], 10);
    if (day < 0 || day > 31) return false;
    if (month < 0 || month > 12) return false;
    if (year < 0 || year < 1200 || year > 2200) return false;
    if (mg.length > 4 && mg[4] != undefined) {
        var hour = parseInt(mg[5], 10);
        var minute = parseInt(mg[6], 10);
        if (hour < 0 || hour > 24) return false;
        if (minute < 0 || minute > 59) return false;
    }
    return true;
});

$(function () {
    $.validator.messages['digit'] = 'مقدار وارد شده برای این فیلد باید عددی باشد.',
        $.validator.messages['required'] = 'تکمیل این فیلد ضروری می باشد.',
        $.validator.messages['email'] = 'آدرس ایمیل وارد شده صحیح نیست.',
        $.validator.messages['digits'] = 'لطفا فقط کاراکترهای عددی وارد کنید.',
        $.validator.messages['minlength'] = 'حداقل طول عبارت وارد شده باید {0} کاراکتر باشد.',
        $.validator.messages['maxlength'] = 'حداکثر طول عبارت وارد شده باید {0} کاراکتر باشد.',
        $.validator.messages['rangelength'] = 'طول عبارت وارد شده باید بین {0} و {1} باشد.',
        $.validator.messages['equalTo'] = 'لطفا مقدار یکسان وارد نمایید.',
        $.validator.messages['pattern'] = 'مقدار وارد شده صحیح نمی باشد.',
        $.validator.messages['extension'] = 'فایل انتخاب شده صحیح نمی باشد، فقط فایل‌هایی از نوع {0} قابل قبول می باشند.'

    $.validator.addMethod(
        "regex",
        function (value, element, regexp) {
            var re = new RegExp(regexp);
            return re.test(value);
        },
        "عبارت وارد شده مورد قبول نیست!"
    );
});