; $(function () {
    var exceptionContainerId = "exceptionContainer";
    var exceptionContainer = $("#" + exceptionContainerId);
    if (exceptionContainer.length == 0) {
        exceptionContainer = $("<div id='" + exceptionContainerId + "' style='direction:lrt'></div>");
        $("body").append(exceptionContainer);
    }
    //    $(document).ajaxComplete(function (e, xhr, settings) {
    //        $.showException(xhr.responseText);
    //    });

    $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
        _title: function (title) {
            if (!this.options.title) {
                title.html("&#160;");
            } else {
                title.html(this.options.title);
            }
        }
    }));

});


String.prototype.format = function () {
    a = this;
    for (k in arguments) {
        a = a.replace("{" + k + "}", arguments[k])
    }
    return a
}
$.showException = function (excep, containerId, dialog) {

    var exceptionContainerId = containerId || "exceptionContainer";
    var exceptionContainer = $("#" + exceptionContainerId);
    var exception, root_exception;
    try {
        exception = typeof (excep) == 'string' ? JSON.parse(excep) : excep;
    }
    catch (ex) {
        return;
    }
    var hasInnerException = false;
    if (exception.error) {
        root_exception = exception;
        var text = [];
        var links = [];
        var count = 1;
        while (exception != null) {

            if (exception.helpLink && exception.helpLink.length > 0)
                links.push(exception.helpLink);

            text.push('<div class="blockquote">');
            text.push('<div>');

            text.push('<span class="ui-state-none exception-icon">');
            text.push('<span class="ui-icon icon-plus" style="display: inline-block; float: left;"></span></span>');
            var msg = exception.message;
            if (exception.code && exception.code.length > 0) {
                msg = "خطا " + exception.code + " : " + msg;
            }
            msg = msg.replace(/\n/gm, "<br/>");
            text.push('<div class="exception-title"><div style="float:left;direction: ');
            text.push(exception.dir);
            text.push('">');
            text.push(msg);
            text.push('</div></div></div>');
            text.push('<div class="exception-stacktrace ui-helper-hidden">');
            var stackTrace = '';
            if (exception.stackTrace) {
                hasInnerException = true;
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

        if (links.length > 0) {
            text.push('</br>');
            text.push('برای راهنمایی جهت حل مشکل می توانید به ');
            if (links.length == 1) {
                text.push('<a  target="_blank" href="');
                text.push(links[0]);
                text.push('">');
                text.push('<i class="icon-external-link"></i>');
                text.push(' این بخش ');
                text.push('</a>');
            } else {
                for (var i = 0; i < links.length; ++i) {
                    text.push('<a target="_blank" href="');
                    text.push(links[i]);
                    text.push('">');
                    text.push('<i class="icon-external-link"></i>');
                    text.push(' لینک ');
                    text.push(i + 1);
                    text.push('</a>');
                }
            }
            text.push('مراجعه فرمایید.<br/>');
        }

        if (root_exception.enableUserMessage) {
            text.push('</br>');
            text.push('<i class="icon-flower-tulip"></i>');
            text.push('<p>ضمن پوزش از مشکل به وجود آمده،<br/>لطفا در صورت تمایل توضیحات و اطلاعات بیشتری در خصوص مشکل به وجود آمده برای ما ارسال بفرمایید:</p>');
            text.push('<textarea rows="4" id="txtUserMessage" maxlength="500"/>');
        }


        exceptionContainer.html(text.join(''));
        if (hasInnerException) {
            exceptionContainer.find('.exception-icon').hover(
                function () { $(this).addClass('ui-state-active'); },
                function () { $(this).removeClass('ui-state-active'); }
            );

            exceptionContainer.find('.exception-icon').click(function () {
                if ($(this).children().hasClass('icon-minus')) {
                    $(this).children().removeClass('icon-minus');
                    $(this).children().addClass('icon-plus');
                } else {
                    $(this).children().removeClass('icon-plus');
                    $(this).children().addClass('icon-minus');
                }
                $(this).parent().siblings('.exception-stacktrace').toggleClass('ui-helper-hidden');
            });
        } else {
            exceptionContainer.find('.exception-icon').hide();
        }
        if (dialog === false)
            return;
        exceptionContainer.dialog({
            title: '<i class="icon-exclamation-triangle"></i> خطا',
            width: 'auto',
            maxWidth: 600,
            buttons: {
                "OK": function () {
                    var utxt = $('#txtUserMessage:visible').val();
                    if (root_exception.enableUserMessage && utxt.length > 0) {
                        $.ajax({
                            method: 'POST',
                            data: {
                                id: root_exception.code,
                                message: utxt
                            },
                            url: '/ExceptionHandling/SaveUserMessage',
                            success: function () {
                                exceptionContainer.dialog("close");
                            }
                        });
                    } else
                        exceptionContainer.dialog("close");
                }
            }
        }).dialogExtend({
            maximizable: true,
            dblclick: "collapse",
            icons: {
                minimize: "icon-window-maximize",
                maximize: 'icon-window-minimize',
                restore: 'icon-window-restore',
            },
        });
    }
};
function rep(text, count) {
    var result = '';
    for (var i = 0; i < count; ++i)
        result += text;
    return result;
}

$.console = {
    error: function () {
        if (console && console.error) console.error(arguments);
    },
    info: function () {
        //if (console && console.info) console.info(arguments);
    },
    debug: function () {
        if (console && console.debug) console.debug(arguments);
    },
    warn: function () {
        if (console && console.warn) console.info(arguments);
    },
    trace: function () {
        if (console && console.trace) console.trace(arguments);
    }
}

$.attach = function (files, callback) {
    var fileStates = [];

    for (var i = 0; i < files.length; ++i) {
        fileStates.push({
            url: files[i],
            complete: false
        });
    }

    var isComplete = function () {
        for (var j = 0; j < fileStates.length; ++j)
            if (!fileStates[j].complete)
                return false;
        return true;
    };
    var complete = function (url) {
        for (var j = 0; j < fileStates.length; ++j)
            if (fileStates[j].url == url) {
                fileStates[j].complete = true;
                break;
            }
        if (isComplete())
            callback();
    }

    for (var i = 0; i < files.length; ++i) {
        var url = files[i];
        if (url.substring(url.length - 3).toLowerCase() == ".js")
            $.attachScript(url, function (fileUrl) {
                complete(fileUrl);
            }, true);
        else
            $.attachCss(url, function (fileUrl) {
                complete(fileUrl);
            }, true);
    }
}

$.attachScript = function (fileUrl, callback, sync) {
    var tag = HashCode.value(fileUrl);
    if (!$.res) {
        $.res = {};
    }
    if ($.res[tag] != null) {
        if ($.res[tag].state == 'Loaded') {
            if (callback && $.isFunction(callback)) {
                try {
                    callback(fileUrl);
                }
                catch (ex) {
                    $.console.error('common-152:', ex.message, ex);
                }
            }
            return;
        }
        if ($.res[tag].state == 'Pending') {
            $.res[tag].e.push(callback);
            return;
        }
    }
    $.res[tag] = {};
    $.res[tag].state = 'Pending';
    $.res[tag].e = new Array();
    $.res[tag].e.push(callback);
    try {
        if (fileUrl.indexOf('?') < 0)
            fileUrl = fileUrl + "?" + new Date();
        else
            fileUrl = fileUrl + "&" + new Date();
        jQuery.ajax({
            url: fileUrl , dataType: 'script', cache: true,
            async: sync ? false : true,
            error: function (e) {
                $.console.error('common-172: GET ' + e);
            },
            success: function () {
                $.res[tag].state = "Loaded";
                $.console.info('common-121: GET ' + fileUrl);
                for (var i = 0; i < $.res[tag].e.length; ++i) {
                    try {
                        if ($.isFunction($.res[tag].e[i]))
                            $.res[tag].e[i](fileUrl);
                    }
                    catch (ex) {
                        $.console.error('common-179:', ex.message, ex);
                    }
                }
            }
        });
    }
    catch (ex) { $.error('common-134:', ex); }
    return;
}

$.attachCss = function (fileUrl, callback, sync) {
    var tag = HashCode.value(fileUrl);
    if (!$.res) {
        $.res = {};
    }
    if ($.res[tag] != null) {
        if ($.res[tag].state == 'Loaded') {
            if (callback && $.isFunction(callback)) {
                try {
                    callback(fileUrl);
                }
                catch (ex) {
                    alert(ex);
                }
            }
            return;
        }
        if ($.res[tag].state == 'Pending') {
            $.res[tag].e.push(callback);
            return;
        }
    }
    $.res[tag] = {};
    $.res[tag].state = 'Pending';
    $.res[tag].e = new Array();
    $.res[tag].e.push(callback);
    jQuery.ajax({
        url: fileUrl, dataType: 'text', cache: true,
        async: sync ? false : true,
        error: function (e) {
            $.console.error('common-223: GET ' + e);
        },
        success: function (data) {
            $('body').append('<style rel="stylesheet" type="text/css">' + data + '</style>');
            $.res[tag].state = "Loaded";
            $.console.info('common-168: stylesheet file loaded ', fileUrl);
            for (var i = 0; i < $.res[tag].e.length; ++i) {
                try {
                    if ($.isFunction($.res[tag].e[i]))
                        $.res[tag].e[i](fileUrl);
                }
                catch (ex) {
                    $.console.error('common-175:', ex);
                }
            }
        }
    });
}

$.loadContent = function (uri, containerId, data, cache, success) {
    var cache2 = false;
    if (cache)
        cache2 = cache;
    else
        data.nv = Date();

    $.ajax({
        url: uri,
        dataType: 'html',
        cache: cache2,
        data: data,
        type: "get",
        success: function (data) {
            $("#" + containerId).html(data);
            if ($.isFunction(success))
                success(data);
        }
    });
}

if (!$.httpData)
    $.httpData = function (xhr, type, s) {
        var ct = xhr.getResponseHeader("content-type") || "",
            xml = type === "xml" || !type && ct.indexOf("xml") >= 0,
            data = xml ? xhr.responseXML : xhr.responseText;

        if (xml && data.documentElement.nodeName === "parsererror") {
            jQuery.error("parsererror");
        }

        // Allow a pre-filtering function to sanitize the response
        // s is checked to keep backwards compatibility
        if (s && s.dataFilter) {
            data = s.dataFilter(data, type);
        }

        // The filter can actually parse the response
        if (typeof data === "string") {
            // Get the JavaScript object, if JSON is used.
            if (type === "json" || !type && ct.indexOf("json") >= 0) {
                data = jQuery.parseJSON(data);

                // If the type is "script", eval it in global context
            } else if (type === "script" || !type && ct.indexOf("javascript") >= 0) {
                jQuery.globalEval(data);
            }
        }

        return data;
    };

(function ($) {
    if ($.fn.style) {
        return;
    }

    // Escape regex chars with \
    var escape = function (text) {
        return text.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
    };

    // For those who need them (< IE 9), add support for CSS functions
    var isStyleFuncSupported = !!CSSStyleDeclaration.prototype.getPropertyValue;
    if (!isStyleFuncSupported) {
        CSSStyleDeclaration.prototype.getPropertyValue = function (a) {
            return this.getAttribute(a);
        };
        CSSStyleDeclaration.prototype.setProperty = function (styleName, value, priority) {
            this.setAttribute(styleName, value);
            var priority = typeof priority != 'undefined' ? priority : '';
            if (priority != '') {
                // Add priority manually
                var rule = new RegExp(escape(styleName) + '\\s*:\\s*' + escape(value) +
                    '(\\s*;)?', 'gmi');
                this.cssText =
                    this.cssText.replace(rule, styleName + ': ' + value + ' !' + priority + ';');
            }
        };
        CSSStyleDeclaration.prototype.removeProperty = function (a) {
            return this.removeAttribute(a);
        };
        CSSStyleDeclaration.prototype.getPropertyPriority = function (styleName) {
            var rule = new RegExp(escape(styleName) + '\\s*:\\s*[^\\s]*\\s*!important(\\s*;)?',
                'gmi');
            return rule.test(this.cssText) ? 'important' : '';
        }
    }

    // The style function
    $.fn.style = function (styleName, value, priority) {
        // DOM node
        var node = this.get(0);
        // Ensure we have a DOM node
        if (typeof node == 'undefined') {
            return this;
        }
        // CSSStyleDeclaration
        var style = this.get(0).style;
        // Getter/Setter
        if (typeof styleName != 'undefined') {
            if (typeof value != 'undefined') {
                // Set style property
                priority = typeof priority != 'undefined' ? priority : '';
                style.setProperty(styleName, value, priority);
                return this;
            } else {
                // Get style property
                return style.getPropertyValue(styleName);
            }
        } else {
            // Get CSSStyleDeclaration
            return style;
        }
    };
})(jQuery);


var querystring = {
    parse: function (string) {
        var parsed = {};
        string = (string !== undefined) ? string : window.location.search;

        if (typeof string === "string" && string.length > 0) {
            if (string[0] === '?') {
                string = string.substring(1);
            }

            string = string.split('&');

            for (var i = 0, length = string.length; i < length; i++) {
                var element = string[i],
                    eqPos = element.indexOf('='),
                    keyValue, elValue;

                if (eqPos >= 0) {
                    keyValue = element.substr(0, eqPos);
                    elValue = element.substr(eqPos + 1);
                }
                else {
                    keyValue = element;
                    elValue = '';
                }

                elValue = decodeURIComponent(elValue);

                if (parsed[keyValue] === undefined) {
                    parsed[keyValue] = elValue;
                }
                else if (parsed[keyValue] instanceof Array) {
                    parsed[keyValue].push(elValue);
                }
                else {
                    parsed[keyValue] = [parsed[keyValue], elValue];
                }
            }
        }

        return parsed;
    },
    stringify: function (obj) {
        var string = [];

        if (!!obj && obj.constructor === Object) {
            for (var prop in obj) {
                if (obj[prop] instanceof Array) {
                    for (var i = 0, length = obj[prop].length; i < length; i++) {
                        string.push([encodeURIComponent(prop), encodeURIComponent(obj[prop][i])].join('='));
                    }
                }
                else {
                    string.push([encodeURIComponent(prop), encodeURIComponent(obj[prop])].join('='));
                }
            }
        }

        return string.join('&');
    }
};

function clone(obj) {
    var copy;

    // Handle the 3 simple types, and null or undefined
    if (null == obj || "object" != typeof obj) return obj;

    // Handle Date
    if (obj instanceof Date) {
        copy = new Date();
        copy.setTime(obj.getTime());
        return copy;
    }

    // Handle Array
    if (obj instanceof Array) {
        copy = [];
        for (var i = 0, len = obj.length; i < len; i++) {
            copy[i] = clone(obj[i]);
        }
        return copy;
    }

    // Handle Object
    if (obj instanceof Object) {
        copy = {};
        for (var attr in obj) {
            if (obj.hasOwnProperty(attr)) copy[attr] = clone(obj[attr]);
        }
        return copy;
    }

    throw new Error("Unable to copy obj! Its type isn't supported.");
}