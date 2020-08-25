
(function ($) {



    $.waitbox = {
        getMsg: function () {
            var s = '';
            s += '<span class="waitbox_op_title">' + $.bocrud.msg.progress + '(' + $.waitbox.reqs.length + ')</span><br/>';

            for (var r = 0; r < $.waitbox.reqs.length; ++r) {
                var robj = $.waitbox.reqs[r];
                if (robj.msg && robj.msg.length > 0)
                    s += '<span class="waitbox_op">◄ ' + robj.msg + '</span><br/>';
                else {
                    var t = robj.type.toLowerCase();
                    switch (t) {
                        case "post":
                            s += '<span class="waitbox_op">◄ ' + $.bocrud.msg.sending + ' ...</span><br/>'
                            break;
                        case "get":
                            s += '<span class="waitbox_op">◄ ' + $.bocrud.msg.loading + ' ...</span><br/>';
                            break;
                    }
                }
            }
            return s;
        },
        isShown: function () {
            return $.waitbox.elem && $.waitbox.elem.hasClass('ui-dialog') && $.waitbox.elem.dialog('isOpen') == true;
        },
        show: function (msg, timeout) {
            var wiatingMsg = $.bocrud.msg.wait;
            if (!$.waitbox.elem) {
                $.waitbox.elem = $("#waitBox");
                if ($.waitbox.elem.length == 0) {
                    var wb = $("<div id='waitBox'></div>");
                    wb.html("<div class='image' /> <span>" + wiatingMsg + "</span>");
                    $.waitbox.status = $('<div class="waitbox_op_div"></div>');
                    wb.append($.waitbox.status);
                    $('body').append(wb).append('<svg version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" class="blur-svg"><defs><filter id="blur-filter"><feGaussianBlur stdDeviation="3"></feGaussianBlur></filter></defs></svg>');
                    $.waitbox.elem = wb;
                }
            }

            $.waitbox.status.html(msg);
            if ($.waitbox.elem.hasClass('ui-dialog') && $.waitbox.elem.dialog('isOpen') == true)
                return;
            if (!$.waitbox.elem.hasClass('ui-dialog'))
                $.waitbox.elem.dialog({
                    draggable: false,
                    height: 110,
                    width: 200,
                    resizable: true,
                    modal: true,
                    autoOpen: false,
                    dialogClass: 'waitbox',
                    close: function () {
                        $('.page-container').removeClass('blur');
                    }
                }).parent().find('.ui-dialog-titlebar').remove();

            if (!timeout) {
                setTimeout(function () { $.waitbox.show(msg, true); }, 1000);
                return;
            }

            if ($.waitbox.reqs && $.waitbox.reqs.length > 0) {
                $('.page-container').addClass('blur');
                $.waitbox.elem.dialog("open");
            }
        }
    };

    $.bocrud.globalEvents.push({
        type: 'onredirect',
        key: 'loading',
        func: function (xml, params, newWindow, noty_msg) {
            if (!newWindow) {
                setTimeout(function () {
                    $.waitbox.show(noty_msg || 'در حال انتقال صفحه');
                }, 200);
            }
        }
    });

    $.bocrud.globalEvents.push({
        type: 'onAjaxSend',
        key: 'loading',
        func: function (xhr, settings) {
            var _this = settings;
            if (!_this.showLoading)
                return;

            if (!$.waitbox.reqs)
                $.waitbox.reqs = [];
            $.waitbox.reqs.push(_this);
            $.waitbox.show($.waitbox.getMsg());

        }
    });

    $.bocrud.globalEvents.push({
        type: 'onAjaxComplete',
        key: 'loading',
        func: function (xhr, settings) {
            if (!$.waitbox || !$.waitbox.reqs)
                return;

            var _this = settings;
            var i = -1;
            for (var ri = 0; ri < $.waitbox.reqs.length; ++ri)
                if ($.waitbox.reqs[ri].url == _this.url) {
                    i = ri; break;
                }
            if (i >= 0)
                $.waitbox.reqs.splice(i, 1);

            $.waitbox.status.html($.waitbox.getMsg());
            if ($.waitbox.reqs.length == 0)
                if ($.waitbox.elem && $.waitbox.elem.dialog('isOpen'))
                    $.waitbox.elem.dialog("close");
        }
    });

})(jQuery);

