
$(function () {

    $.noty = $.noty || {};

    $.noty.defaults = $.extend($.noty.defaults, {
        layout: 'topCenter',
        timeout: 7 * 1000,
        closeWith: ['click', 'button']
    });

    var recently = [];

    var check = function (text) {
        for (var i = 0; i < recently.length; ++i) {
            if ((new Date() - recently[i].dt) > 1000) {
                recently.splice(i, 1);
                --i;
            }
        }
        for (var i = 0; i < recently.length; ++i) {
            if (recently[i].text == text) return false;
        }
        recently.push({
            text: text,
            dt: new Date()
        });
        return true;
    }

    var error = function (text) {
        if (!check(text)) return;
        if (typeof (text) == "string")
            return noty({ text: text, type: 'error' });
        else
            return noty($.extend(text, {
                type: 'error'
            }));
    }

    var warn = function (text) {
        if (!check(text)) return;
        if (typeof (text) == "string")
            return noty({ text: text, type: 'warn' });
        else
            return noty($.extend(text, {
                type: 'error'
            }));
    }
    var success = function (text) {
        if (!check(text)) return;

        return noty({
            text: text,
            type: 'success',
            dismissQueue: true,
            animation: {
                open: { height: 'toggle' }, // jQuery animate function property object
                close: { height: 'toggle' }, // jQuery animate function property object
                easing: 'swing', // easing
                speed: 500 // opening & closing animation speed
            }
        });
    }
    $.noty.error = error;
    $.noty.success = success;
    $.noty.warn = warn;
    var show_exception = function (exception) {

        var text = [];
        var count = 1;
        while (exception != null) {
            text.push('<div class="blockquote exception">');
            text.push('<div>');
            var msg = exception.message;
            msg = msg.replace(/\n/gm, "<br/>");
            text.push('<div class="exception-title">');
            text.push('<span class="ui-state-none exception-icon"><span class="icon-chevron-sign-down"></span></span>');
            text.push('<div style="display:inline;direction:');
            text.push(exception.dir);
            if (exception.dir == 'ltr') {
                text.push(";text-align:left");
            } else {
                text.push(";text-align:right");
            }
            text.push('">');
            text.push(msg);
            text.push('</div></div></div>');
            text.push('<div class="exception-stacktrace ui-helper-hidden">');
            var stackTrace = '';
            if (exception.stackTrace) {
                stackTrace = exception.stackTrace
                    .replace(/\\(\w+.cs):line ([0-9]+)/gm, "\\<span class='exception-file'>$1</span>:line <span class='exception-line'>$2</span>")
                    .replace(/\.(\w+)\(/gm, ".<span class='exception-method'>$1</span>(")
                    .replace(/\n/gm, "<br/>");
            }
            text.push(stackTrace);

            exception = exception.innerException;
            count++;
        }
        text.push(rep("</div></div>", count - 1));
        var n = error({
            text: text.join(''),
            timeout: 0,
            closeWith: ['button']
        }).$message;
        n.find('.exception-icon').click(function () {
            if ($(this).children().hasClass('ui-icon-squaresmall-minus')) {
                $(this).children().removeClass('ui-icon-squaresmall-minus');
                $(this).children().addClass('ui-icon-squaresmall-plus');
            } else {
                $(this).children().removeClass('ui-icon-squaresmall-plus');
                $(this).children().addClass('ui-icon-squaresmall-minus');
            }
            $(this).parent().parent().siblings('.exception-stacktrace').toggleClass('ui-helper-hidden');
        });

        return false;
    };

    if ($.validator)
        $.validator.setDefaults({
            invalidHandler: function (e, v) {
                console.log('$.validator.invalidHandler invoked');
                var c = v.numberOfInvalids();
                error($.notyr.messages.validation_error.format(c));
            }
        });

    $.bocrud.globalEvents.push({
        type: 'oncmderror',
        key: 'noty',
        func: function (ex) { $.showException(ex); }
    });

    $.bocrud.globalEvents.push({
        type: 'onshowcontenterror',
        key: 'noty',
        func: function (ex) { $.showException(ex); }
    });

    $.bocrud.globalEvents.push({
        type: 'ontoast',
        key: 'noty',
        func: function (msg, type) {
            type = type.toLowerCase();
            if (type == "info") type = "information";
            return noty({
                text: msg,
                type: type,
                timeout: msg.length > 100 ? 30 * 1000 : 7 * 1000
            });
        }
    });

    $.bocrud.globalEvents.push({
        type: 'onsubmitsuccess',
        key: 'noty',
        func: function (formObj, saveResult, showDetails) {

            if (saveResult && saveResult.Success === false) return;

            if (this.config.tempMode)
                return;
            if (saveResult && saveResult.row && saveResult.row[0].id) {
                var title = formObj.brevity || this.item2Str(saveResult.row[0].cell, this.config.toStringFormat, this.grid());
                success($.notyr.messages.submit_success.format(title));
                return;
            }
            if (formObj.boKey) {
                success($.notyr.messages.submit_success.format(formObj.title));
            }
        }
    });
    $.bocrud.globalEvents.push({
        type: 'onanchorcopy',
        key: 'noty',
        func: function (caption) {
            success($.notyr.messages.anchor_copy.format(caption));
        }
    });
    $.bocrud.globalEvents.push({
        type: 'onanchorcopyfailed',
        key: 'noty',
        func: function (caption) {
            warn($.notyr.messages.anchor_copy_failed.format(caption));
        }
    });
    $.bocrud.globalEvents.push({
        type: 'oninlinesubmitsuccess',
        key: 'noty',
        func: function () {
            if (this.config.temp)
                return;
            success($.notyr.messages.oninlinesubmitsuccess);
        }
    });
    $.bocrud.globalEvents.push({
        type: 'onvalidationfailed',
        key: 'noty',
        func: function (form) {
            var validator = form.data('validator');
            var m = [];
            m.push($.notyr.messages.onvalidationfailed);
            $.each(validator.errorList, function (e) {
                var caption = $(this.element).closest('.bocrud-control').children('label:first').text();
                caption = caption.trim().replace(/^[:*?\s]+|[:*?\s]+$/g, '').trim();

                if (this.message.indexOf(caption) < 0)
                    m.push(caption + ' - ' + this.message);
                else
                    m.push(this.message);
            });
            $.noty.warn({ text: m.join('<br/>') });
        }
    });
    $.bocrud.globalEvents.push({
        type: 'onerror',
        key: 'noty',
        func: function (ex) { $.showException(ex); }
    });
    $.bocrud.globalEvents.push({
        type: 'onredirect',
        key: 'noty',
        func: function (xml, params, newWindow) {
            if (!newWindow) {
                success($.notyr.messages.onredirect);
            }
        }
    });

    $.bocrud.globalEvents.push({
        type: 'onpartialupdate',
        key: 'noty',
        func: function (dialogObj, pid, xml, ps, sups, ud, fake) {
        }
    });

    $.bocrud.globalEvents.push({
        type: 'onpartialupdatecomplete',
        key: 'noty',
        func: function (dialogObj, pid, xml, ps, sups, ud, fake) {
        }
    });
});