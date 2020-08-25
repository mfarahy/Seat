
(function ($) {



    $.waitbox = {
        getMsg: function () {
            var s = '';
            s += '<span class="waitbox_op_title">عملیات در حال انجام(' + $.waitbox.reqs.length + ')</span><br/>';

            for (var r = 0; r < $.waitbox.reqs.length; ++r) {
                var robj = $.waitbox.reqs[r];
                if (robj.msg && robj.msg.length > 0)
                    s += '<span class="waitbox_op">◄ ' + robj.msg + '</span><br/>';
                else {
                    var t = robj.type.toLowerCase();
                    switch (t) {
                        case "post":
                            s += '<span class="waitbox_op">◄ ارسال ...</span><br/>'
                            break;
                        case "get":
                            s += '<span class="waitbox_op">◄ بارگذاری ...</span><br/>';
                            break;
                    }
                }
            }
            return s;
        },
        show: function (msg) {
            $.waitbox.status.html(msg);
            if ($.waitbox.elem.hasClass('ui-dialog') && $.waitbox.elem.dialog('isOpen') == true)
                return;
            if (!$.waitbox.elem.hasClass('ui-dialog'))
                $.waitbox.elem.dialog({
                    draggable: false,
                    height: 400,
                    width: 700,
                    resizable: true,
                    modal: false,
                    autoOpen: false,
                    dialogClass: 'waitbox',
                    buttons: {
                        'Ok': function () {
                            if ($.waitbox.elem && $.waitbox.elem.dialog('isOpen'))
                                $.waitbox.elem.dialog("close");
                            $.waitbox.elem.find('#logTable').html('').removeClass('haserror');
                        }
                    }
                }).parent().find('.ui-dialog-titlebar').remove();
            $.waitbox.elem.dialog("open");
        }
    };
    $.extend($.jgrid.defaults, {
        /*
          A pre-callback to modify the XMLHttpRequest object (xhr) before it is sent. 
          Use this to set custom headers etc. Returning false will cancel the request.
          */
        loadBeforeSend: function (xhr, settings) {
            $.extend(xhr, {
                global: true,
                type: "get"
            });
            if (jQuery.ajaxSettings.beforeSend)
                jQuery.ajaxSettings.beforeSend.apply(xhr, settings);
        },
        beforeProcessing: function (data, st, xhr) {
            if (jQuery.ajaxSettings.complete)
                jQuery.ajaxSettings.complete.apply(xhr);
        },
        loadError: function (xhr, st, err) {
            if (jQuery.ajaxSettings.complete)
                jQuery.ajaxSettings.complete.apply(xhr);
        }
    });

    $.bocrud.globalEvents.push({
        type: 'onAjaxSend',
        key: 'debugger-loading',
        func: function (xhr, settings) {
            var _this = settings;
            if (!this.global)
                return;
            var wiatingMsg = "لطفا چند لحظه صبر نمائید... ";
            if (!$.waitbox.elem) {
                $.waitbox.elem = $("#waitBox");
                if ($.waitbox.elem.length == 0) {
                    var wb = $("<div id='waitBox'></div>");
                    wb.html("<div class='image' /> <span>" + wiatingMsg + "</span>");
                    $.waitbox.status = $('<div class="waitbox_op_div"></div>');
                    wb.append($.waitbox.status);
                    wb.append("<table id='logTable' style='font-family: Lucida Console;font-size: 9pt;width:100%;text-align: left;'></table>");
                    $('body').append(wb);
                    $.waitbox.elem = wb;
                }
            }
            if (!$.waitbox.reqs)
                $.waitbox.reqs = [];
            $.waitbox.reqs.push(_this);
            $.waitbox.show($.waitbox.getMsg());
            $.waitbox.elem.closest('.ui-dialog').find('button').hide();
        }
    });

    $.bocrud.globalEvents.push({
        type: 'onAjaxComplete',
        key: 'debugger-loading',
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
                if ($.waitbox.elem && $.waitbox.elem.dialog('isOpen')) {
                    var okbtn = $.waitbox.elem.closest('.ui-dialog').find('button');
                    okbtn.show();
                    if (!$("#logTable").is('.haserror'))
                        setTimeout(function () { $.waitbox.elem.dialog('close'); }, 1000);
                }

        }
    });

    var signalr = function () {

        var logTable = $("#logTable");
        if (logTable.length == 0) {
            logTable = $("#logTable");
        }

        var log = function (dt, level, message) {
            if (logTable.length == 0) {
                logTable = $("#logTable");
            }

            var color = "";
            switch (level) {
                case "info":
                    color = 'white';
                    break;
                case "debug":
                    color = 'lightgray';
                    break;
                case "trace":
                    color = 'lightgreen';
                    break;
                case "error":
                    color = 'red';
                    logTable.addClass('haserror');
                    break;
            }

            var tr = $('<tr/>');
            tr.append($('<td/>').text(dt));
            tr.append($('<td/>')
                .text(message)
                .css({ 'direction': 'ltr' }))
            .css({ 'background-color': color });

            logTable.prepend(tr);

            logTable.parent().scrollTop(0);
        };

        if (!$.connection) {
            log(Date(), "info", "connection not found, wait for 500 ms and check it again.");
            setTimeout(signalr, 500);
            return;
        }

        var nlog = $.connection.logger;
        if (!nlog) {
            setTimeout(signalr, 500);
            return;
        }
        nlog.client.logEvent = log;
        $.connection.logger.connection.start().done();

    }
    signalr();

})(jQuery);

