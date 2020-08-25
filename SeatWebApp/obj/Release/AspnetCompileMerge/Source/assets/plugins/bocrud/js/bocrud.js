// BOCRUD_START //////////////////////////////////////////////////////////////////
// version 0.99.05.30

(function ($) {

    var init_soon = function () {
        if (!window.Soon) {
            setTimeout(init_soon, 500);
            return;
        }
        var soons = document.querySelectorAll('.soon');
        for (var i = 0; i < soons.length; i++) {
            Soon.create(soons[i]);
        }
    }
    init_soon();


    $.extend($.fn, {
        validateDelegate: function (delegate, type, handler) {
            return this.bind(type, function (event) {
                var target = $(event.target);

                if (target[0].form && target.is(delegate) && $(target[0].form).data('validator')) {
                    return handler.apply(target, arguments);
                }
            });
        }
    });
    $.validator.staticRules = function (element) {
        var rules = {};
        var validator = $.data(element.form, "validator");
        if (validator && validator.settings.rules) {
            rules = $.validator.normalizeRule(validator.settings.rules[element.name]) || {};
        }
        return rules;
    };

    $(window).on('resize', function () {

        $('.ui-dialog.bocrud-dialog:visible').each(function () {
            var div = $(this);
            $.bocrud.alignModal(div.children('.ui-dialog-content'));
        });
        $('.ui-jqgrid').each(function () {
            $.bocrud.alignGrid($(this).parent());
        });
        setInterval(function () {
            $('.ui-jqgrid:visible').each(function () {
                $.bocrud.alignGrid($(this).parent());
            });
        }, 3000);
    });

    $(document).on('shown.bs.collapse', 'div.panel', function (e) {
        $.bocrud.alignGrid($(e.currentTarget));
    });

    $(document).on('hide.bs.popover', '.help-button', function (e) {
        var active_proper = $(this);
        if ($('.popover').is(':hover')) {
            var checker = function (currentTarget, active_proper) {
                if (!$(currentTarget).is(':hover') && !$(active_proper).is(':hover')) {
                    $(currentTarget).popover('hide');
                } else {
                    setTimeout(checker, 500, currentTarget, active_proper);
                }
            };
            setTimeout(checker, 500, e.currentTarget, active_proper);
            return false;
        }
    });

    $.bocrud = $.extend($.bocrud || {},
        {
            checkSessionTimeOut: function () {
                var soon = $('.session-timeout>.soon');
                if (soon.length = 0) return;

                $.ajax({
                    url: '/Bocrud/CheckSession',
                    method: 'GET',
                    showLoading: false,
                    type: 'json',
                    success: function (r) {
                        if (!r) return;
                        if (!r.t) {
                            var second_to_expire = new Date(r.e) - new Date();
                            var wt = 30000;
                            if (second_to_expire != NaN && second_to_expire < wt && second_to_expire > 0)
                                wt = second_to_expire;
                            setTimeout($.bocrud.checkSessionTimeOut, wt);
                            var soon = $('.session-timeout>.soon');
                            if (soon.length > 0) {
                                soon.soon().setOptions({
                                    due: r.e,
                                    labelsDays: $.bocrud.captions.day,
                                    labelsHours: $.bocrud.captions.hour,
                                    labelsMinutes: $.bocrud.captions.minut,
                                    labelsSeconds: $.bocrud.captions.second,
                                    layout: "group",
                                    face: "slot slide down",
                                    format: "m,s",
                                    padding: "false",
                                    scaleMax: "s"
                                });
                            }
                        } else {
                            if (r.t) {
                                $('.page-container').addClass('blur');
                                $.bocrud.showMsg($.bocrud.msg.idle_timeout, 'Alert', "", {
                                    modal: false,
                                    close: function () {
                                        if (r.l && r.l.length > 0)
                                            window.location = r.l + '?sourceUrl=' + window.location.href;
                                        else
                                            window.location.reload(true);
                                    }
                                });
                            }
                        }
                    }
                });
            },
            showMsg: function (text, type, title, dialogOptions) {
                var msgcId = "bocrud-msgc";
                var msgc = $("#" + msgcId);
                if (msgc.length == 0) {
                    $('body').append("<div id='bocrud-msgc'/>");
                    msgc = $("#" + msgcId);
                }
                msgc.html(text);
                msgc.dialog($.extend(dialogOptions || {}, {
                    title: title || type || $.bocrud.captions.msg,
                    width: 'auto',
                    maxWidth: 600,
                    buttons: {
                        "Ok": function () {
                            msgc.dialog("close");
                        }
                    }
                }));
            },
            digit_sep: function (e) {

                if ((e.which >= 96 && e.which <= 105) ||
                    (e.which >= 48 && e.which <= 57) ||
                    (e.which === 189 || e.which === 190 || e.which === 8 || e.which === 46)) { // - .

                    var tv = $(this).val();

                    if (tv.length == 0)
                        return;

                    var negative = tv[0] == "-";
                    if (negative) tv = tv.substring(1);

                    var fr = tv.split('.');

                    fr[0] = fr[0].split(',').join('').split('').reverse().join('').match(/.{1,3}/g).join(',').split('').reverse().join('');

                    $(this).val((negative ? "-" : "") + fr.join('.'));
                }
            },
            toast: function (bid, msg, state, type) {

                if (!type)
                    type = state;

                if (bid && bid.length > 0) {
                    window.bocruds[bid].raiseEvent('ontoast', msg, type, state);
                } else {
                    for (var bid in window.bocruds) {
                        window.bocruds[bid].raiseEvent('ontoast', msg, type, state);
                        break;
                    }
                }

            },
            _object_to_querystring: function (params) {
                if (params && typeof (params) == 'object' && !$.isEmptyObject(params)) {
                    var queryString = [];
                    jQuery.each(params, function (key, value) {
                        queryString.push(key + '=' + value);
                    });
                    return queryString.join("");
                }
                return params;
            },
            refine_url: function (link, xml, params, onLinkReady) {
                if (params && typeof (params) == 'string')
                    params = $.query.parseNew(params).keys;

                if (params && typeof (params) == 'object' && !$.isEmptyObject(params)) {
                    if (xml) {
                        var org_params = '';
                        if (link.indexOf('?') > 0)
                            org_params = link.split('?')[1];
                        var queryStringObj = $.query.parseNew(org_params).keys;

                        var merged = $.extend(queryStringObj, params);
                        merged["xml"] = xml;
                        merged["_auto_redirect"] = false;
                        $.ajax({
                            url: '/Bocrud/GetLink',
                            showLoading: true,
                            data: merged,
                            method: 'POST',
                            success: function (new_link) {
                                onLinkReady(new_link);
                            }
                        });
                    } else {
                        var qs = $.bocrud._object_to_querystring(params);
                        if (link.indexOf('?') > 0)
                            onLinkReady(link + '&' + qs);
                        else
                            onLinkReady(link + '?' + qs);
                    }
                } else {
                    onLinkReady(link);
                }
            },
            redirect: function (xml, params, bid, newWindow, confirm_msg, noty_msg, is_success) {
                if (confirm_msg && confirm_msg.length > 0)
                    if (!confirm(confirm_msg))
                        return;

                if (bid) {
                    window.bocruds[bid].raiseEvent('onredirect', xml, params, newWindow, noty_msg);
                } else {
                    for (var b in window.bocruds) {
                        bid = b;
                        window.bocruds[b].raiseEvent('onredirect', xml, params, newWindow, noty_msg);
                        break;
                    }
                }
                var url = '';

                if (noty_msg && noty_msg.length > 0) {
                    $.bocrud.toast(bid, noty_msg, is_success ? 'success' : 'warn');
                }

                if (xml.indexOf("http") == 0 || xml.indexOf('..') >= 0) {

                    $.bocrud.refine_url(xml, null, params, function (newLink) {
                        if (newWindow)
                            window.open(newLink, '_blank');
                        else
                            window.location = newLink;
                    });
                }
                else {

                    var form = [];
                    var jform = $('#GetLinkForm');
                    if (jform.length == 0) {
                        form.push('<form id="GetLinkForm" action="/Bocrud/GetLink" method="post" ' + (newWindow ? 'target="_blank"' : '') + '>');
                        form.push('</form>');
                        $('body').append(form.join(''));
                        jform = $('#GetLinkForm');
                        form.splice(0, form.length);
                    }
                    if (typeof (params) == 'string')
                        params = $.query.parseNew(params).keys;

                    params = $.extend(params || {}, { xml: xml });

                    for (var arg in params) {
                        form.push('<input name="');
                        form.push(arg);
                        form.push('" type="hidden" value="');
                        form.push(params[arg]);
                        form.push('">');
                    }
                    jform.html(form.join(''));
                    jform.submit();
                }
            },
            alignGrid: function (div) {
                div.find('.ui-jqgrid').each(function () {
                    var gid = $(this).attr('id').substring(5);
                    var g = $('#' + gid);
                    var w = g.jqGrid('getGridParam', 'autowidth');
                    if (w)
                        gridAutoWidth(gid, 100);
                });
            },
            alignModal: function (modal) {
                var m = $(modal);
                var ww = $(window).width();
                var mw = m.data('orgw') || m.width(),
                    mh = m.data('orgh') || m.height();
                var nw = Math.min(mw || 400, $(window).width()),
                    nh = Math.min(mh || 400, $(window).height());

                m.dialog({ width: nw, height: nh, position: { my: "center center", at: "center center", of: $(window) } });
            },
            popup: function (xml, link, title, w, h, params) {
                var show_popup = function (url) {
                    var buttons = {};
                    buttons[$.jgrid.edit.bClose] = function () {
                        $.bocrud.closeModal($(this), null, true);
                    }
                    var div = $('div[frame-container=' + xml + ']');
                    if (div.length == 0) {
                        $("<div frame-container='" + xml + "'/>").appendTo('body');
                        div = $('div[frame-container=' + xml + ']');
                        div.append($("<iframe style='width:100%;height:95%;border:none;'/>"));
                    }

                    div.children('iframe').attr("src", url);

                    $.bocrud.openModal(div, {
                        title: title,
                        width: w,
                        height: h,
                        resizable: true,
                        zIndex: 1040,
                        closeOnEscape: true,
                        draggable: true,
                        buttons: buttons
                    });
                }

                $.bocrud.refine_url(link, xml, params, show_popup);
            },
            openPopup: function (url, title, w, h, ignoreOutput, params) {

                if (ignoreOutput)
                    $.ajax({
                        url: url,
                        showLoading: true,
                        msg: url
                    });
                else {
                    var show_popup = function (url) {
                        $("<div class='bocrud-iframe-container'/>").append($("<iframe style='width:100%;height:95%;border:none;'/>").attr("src", url)).dialog({
                            title: title,
                            width: w,
                            height: h,
                            resizable: true,
                            zIndex: 1040,
                            closeOnEscape: true,
                            draggable: true
                        });
                    }
                    $.bocrud.refine_url(url, null, params, show_popup);
                }
                //window.open(url, title, 'scrollbars=yes, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left, false);
            },
            closeModal: function (div, dialogParent, remove) {

                if (!dialogParent)
                    dialogParent = $(div).parent();

                if ((div.is('.ui-dialog-content') || div.is('.ui-dialog')) && dialogParent) {

                    div.dialog('destroy');

                    if (remove)
                        div.remove();
                    else
                        div.appendTo(dialogParent);
                } else {
                    try {
                        div.dialog('close');
                    }
                    catch (e) { }
                }

                $.bocrud.popActiveForm();
            },
            openModal: function (div, o) {
                var w = o.width || 400, h = o.height || 400;

                var options = {
                    title: o.title,
                    buttons: o.buttons,
                    autoOpen: o.autoOpen == undefined ? true : o.autoOpen,
                    closeOnEscape: true,
                    zIndex: 1040,
                    dialogClass: 'bocrud-dialog ' + (o.dialogClass || ""),
                    draggable: true,
                    modal: o.modal === false ? false : true,
                    width: Math.min(w, $(window).width()),
                    height: Math.min(h, $(window).height()),
                    hide: $.bocrud.effects.dialog.hide,
                    resizable: false,
                    appendTo: o.dialogParent || 'body',
                    show: $.extend(o.show || { effect: 'fade', duration: 100 }),
                    focus: function () {
                        if (o && o.show && $.isFunction(o.show.complete))
                            o.show.complete.apply(this);
                        if (o && o.show && $.isFunction(o.show))
                            o.show.apply(this);
                        $.bocrud.alignGrid(div);
                    },
                    close: function () {
                        if ($.isFunction(o.close))
                            o.close.apply(this);

                        if (o.destroyOnClose) {

                            if ($(this).hasClass('ui-dialog-content'))
                                $(this).dialog('destroy');


                            if (o.dialogParent)
                                div.appendTo(o.dialogParent);
                        }

                        if ($.isFunction(o.closed))
                            o.closed.apply(this);
                    },
                    create: function () {
                        if ($.isFunction(o.create))
                            o.create.apply(this);

                        div.data('orgw', w).data('orgh', h);
                    }, open: function () {

                        var f = function () {
                            $.bocrud.alignGrid(div);
                        }
                        setTimeout(f, 1000);
                        if ($.isFunction(o.open))
                            o.open.apply(this);
                    }
                };

                if (o.appendTo)
                    options.appendTo = o.appendTo;

                div.removeData('ui-dialogExtend');

                var d = div.dialog(options);

                if (!d.is('.bocrud-dialog-extended')) {
                    d.addClass('bocrud-dialog-extended');
                    d.dialogExtend({
                        maximizable: true,
                        dblclick: "collapse",
                        icons: {
                            minimize: "icon-window-maximize",
                            maximize: 'icon-window-minimize',
                            restore: 'icon-window-restore',
                        },
                        maximize: function () {
                            $.bocrud.alignGrid(div);
                        },
                        restore: function () {
                            $.bocrud.alignGrid(div);
                        }
                    });
                }


                var btns = d.closest('.ui-dialog')
                    .children('.ui-dialog-buttonpane')
                    .find('button');
                btns.addClass('btn');
                if (btns.length > 0)
                    $(btns[0]).addClass('btn-primary');

                $.bocrud.pushActiveForm(d);

                return d;
            },
            globalEvents: [],
            basicShow: function ($form, formObj) {
                $form.html("<div class='bocrud-form-tabs'><ul></ul></div>");
                ul = $form.find('ul');
                var tabs = $form.find('.bocrud-form-tabs');
                var t = [];
                for (var i = 0; i < formObj.groups.length; ++i) {
                    var g = formObj.groups[i];
                    var gid = formObj.jsbid + '-' + g.id;

                    t.push('<li>');
                    t.push('<a href="#');
                    t.push(gid);
                    t.push('">');
                    t.push(g.title);
                    t.push('</a>');
                    t.push('</li>');
                    var li = ul.find('li a[href="#' + gid + '"]');
                    if (li.length == 0)
                        ul.append(t.join(''));
                    else {
                        li.find('a').html(g.title);
                    }
                    t.splice(0, t.length);

                    t.push('<div id="');
                    t.push(gid);
                    t.push('" name="');
                    t.push(g.Name);
                    t.push('" index="');
                    t.push(i);
                    t.push('">');
                    t.push(g.content);
                    t.push('</div>');

                    var gdiv = tabs.children('div#' + gid);
                    if (gdiv.length == 0)
                        tabs.append(t.join(''));
                    else {
                        gdiv.html(g.content);
                    }
                    t.splice(0, t.length);

                } // for
                if (formObj.groups.length > 1) {
                    tabs.tabs({}).tabs('refresh');
                } else {
                    tabs.children('ul').remove();
                }
                $form.data('formObj', formObj);
            },
            effects: {
                elems: 'slow',
                dialog: { show: 'explode', hide: null },
                search: 'fast',
                toolbars: null,
                thumbnails: { show: 'fade', hide: null }
            },
            pushActiveForm: function (form) {
                this.activeform.push(form);
            },
            popActiveForm: function () {
                this.activeform.pop();

            },
            peekActiveForm: function () {
                var forms = $.bocrud.activeform, form = forms[forms.length - 1], form;
                while (!form.is(':visible')) {
                    $.bocrud.popActiveForm();
                    if (forms.length == 0) {
                        return;
                    }
                    form = forms[forms.length - 1];
                }
                return form;
            },
            activeform: [],
            zoomin: function (container) {
                container.find('img').each(function () {
                    var t = $(this), w = t.width(), h = t.height();
                    $(this).animate({ width: w + (w * 0.1), height: h + (h * 0.1) });
                });
            },
            zoomout: function (container) {
                container.find('img').each(function () {
                    var t = $(this), w = t.width(), h = t.height();
                    $(this).animate({ width: w - (w * 0.1), height: h - (h * 0.1) });
                });
            },
            getExpEvalScript: function (exp) {

                var getPropValue = function ($input) {
                    var val = $input.val();
                    if ($input.is(":checkbox"))
                        val = iobj.is(':checked');
                    if ($input.is(":radio"))
                        val = $('input[name=' + $input.attr('name') + ']:checked').val();
                    return val;
                }

                var result = exp;
                var matches = result.match(/{([^}])+}/g);
                if (matches != null)
                    for (var j = 0; j < matches.length; ++j) {
                        var pid = matches[j].substr(1, matches[j].length - 2);

                        var val = null;
                        if (pid.indexOf('.') > 0) {
                            var parr = pid.split('.');
                            pid = parr[0];
                            var pp = parr[1];

                            val = $(pid).attr(pp);
                        }
                        else {
                            if (pid[0] == '#') {
                                // منظور مقدار نخستین است خصوصیت است و این شارپ مربوط به جی کوری نیست
                                var iobj = $(pid);
                                var initValue = '';
                                if (iobj.is('[initValue]'))
                                    val = iobj.attr('initValue');
                                else {
                                    val = getPropValue(iobj);
                                    iobj.attr('initValue', val);
                                }
                            } else {
                                var iobj = $("#" + pid);
                                val = getPropValue(iobj);
                            }
                        }
                        result = result.replace(matches[j], val);
                    }
                result = result.replace(/&quot;/g, '"');
                return result;
            },
            checkDepenExp: function (elem, exp) {
                var evalScript = $.bocrud.getExpEvalScript(exp);
                evalScript = evalScript.replace('$(this)', '$("#' + $(elem).attr('id') + '")');
                var result;
                try {
                    result = eval(evalScript);
                }
                catch (ex) { $.console.error("bocrud-109:" + ex + ' checkDepenExp eval Script : ' + evalScript); return false; }
                return result;
            },
            decorateUi: function ($container) {

                //$container.find('.bocrud-lookup').each(function () {

                //    var $btn = $(this);
                //    if (!$btn.hasClass('ui-button')) {
                //        $btn.addClass('ui-button ui-widget ui-state-default ui-corner-all')
                //	.html('<span class="ui-icon ' + $btn.attr('icon') + '"></span>')
                //	.hover(
                //		function () { $(this).addClass("ui-state-hover"); },
                //		function () { $(this).removeClass("ui-state-hover"); }
                //	);
                //    }
                //});
                //$container.find('.bocrud-free-button,.bocrud-button,button').each(function () {


                //    var $btn = $(this);
                //    if (!$btn.hasClass('ui-button') && !$btn.data('bs.button')) {
                //        var icon = $btn.attr('icon'), icons = {}, text = $btn.text();
                //        if (icon && icon.length > 0)
                //            icons.secondary = 'ui-icon-' + icon;
                //        $btn.button({
                //            icons: icons,
                //            text: text
                //        });
                //    }
                //});
                //$container.find('.bocrud-control-help-icon').each(function () {
                //    var t = $(this);
                //    t.tooltip({
                //        title: t.next(),
                //        position: {
                //            my: 'bottom left',
                //            at: 'right top',
                //            target: t
                //        },
                //        trigger:'click'
                //    });
                //});
                var parent_dialog = $container.closest('.ui-dialog');
                if (parent_dialog.length == 1) {
                    $container.find('input[type=checkbox].ace, input[type=radio].ace')
                        .css({ 'z-index': parent_dialog.css('z-index') + 1 });
                }
                $container.find('[data-toggle="tooltip"]').tooltip();

                var soons = $container.find('.soon');
                for (var i = 0; i < soons.length; i++) {
                    Soon.create(soons.get(i));
                }
            },
            groupHasProperty: function (group) {
                if (group.properties.length > 0)
                    return true;
                if (group.subgroups.length > 0)
                    for (var i = 0; i < group.subgroups.length; ++i)
                        if ($.bocrud.groupHasProperty(group.subgroups[i]))
                            return true;
            },
            makeAutoHide: function (exp) {
                var t = $(exp);
                var title = t.find('h3').text();
                t.hide();

                var btn = $('<div/>').attr('class', 'vertical-text ui-toolbar-button ui-corner-all ui-state-default');
                btn.append('<span class="ui-icon ui-icon-folder-collapsed"></span>');
                btn.append('<span class="title">' + title + '</span>');

                t.after(btn);

                btn.height(title.length * 10);

                var show_efe = 'drop'; //{ show: 'clip', hide: null };
                var hide_efe = null;

                t.hover(function () { }, function () { t.hide(hide_efe); btn.show('highlight'); });
                btn.hover(function () { t.show('fast'); btn.hide(hide_efe); });

            },

            add_validator: function (selector, messages, rules) {
                var closest_form = $(selector).closest('form');
                var validator = closest_form.data('validator');
                var form = closest_form.data('formObj');
                if (!validator && form.validationObj && jQuery.isEmptyObject(form.validationObj.rules)) {
                    form.validationObj = $.extend(form.validationObj, {
                        rules: rules,
                        messages: messages
                    });
                    closest_form.data('formObj', form.validationObj);
                    $.bocrud.reg_form(form);
                    return;
                }

                if (validator) {

                    form.validationObj = $.extend({
                        rules: {},
                        messages: {}
                    }, form.validationObj);

                    var vs = validator.settings;

                    if (vs.messages && messages)
                        for (var key in messages)
                            vs.messages[key] = messages[key];

                    if (vs.rules && rules)
                        for (var key in rules)
                            vs.rules[key] = rules[key];
                }

            },

            reg_form: function (form) {

                if (typeof (form) == 'string')
                    form = eval('(' + str_form + ')');

                var $form = form.id ? $('#' + form.id) : $('.bocrud-page-form[uid="' + form.uid + '"]');

                if (!$form.is('form')) { // not use form tag

                    var closest_form = $form.closest('form');

                    var validator = closest_form.data('validator');

                    if (validator) {

                        form.validationObj = $.extend({
                            rules: {},
                            messages: {}
                        }, form.validationObj);

                        var vs = validator.settings;

                        if (vs.messages && form.validationObj)
                            for (var key in form.validationObj.messages)
                                vs.messages[key] = form.validationObj.messages[key];

                        if (vs.rules && form.validationObj)
                            for (var key in form.validationObj.rules)
                                vs.rules[key] = form.validationObj.rules[key];

                    }
                } else {
                    if (form.validationObj && !jQuery.isEmptyObject(form.validationObj.rules)) {
                        var vo = $.extend({}, form.validationObj, {
                            success: function (label, element) {
                                var success_message = $(element).data('success-message');
                                if (success_message && success_message.length > 0) {
                                    $(element).closest('.bocrud-control').find('label.valid').remove();
                                    $(label).removeClass('error').addClass('valid').text(success_message);
                                }
                            },
                            errorPlacement: function (label, elem) {
                                if (elem.is('.select2-hidden-accessible')) {
                                    label.insertAfter(elem.next());
                                } else if (elem.is(':radio') || elem.is(':checkbox') || elem.is('.hasDatepicker') ||
                                    elem.is('.error-place-last')) {
                                    $(elem).closest('.bocrud-control-content').append(label);
                                } else {
                                    // ""
                                    label.insertAfter(elem);
                                    return false;
                                }
                            },
                            ignore: function (i, elem) {
                                var e = $(elem);

                                if (form.validationObj.ignore && $.isFunction(form.validationObj.ignore) && form.validationObj.ignore(i, elem))
                                    return true;

                                if (!e.is(':visible')) {
                                    if (e.closest('.bocrud-control').is('.bocrud-server-hide'))
                                        return true;

                                    var id = $(elem).attr('id');
                                    // for one2many controls that have hidden columns
                                    if (id && id.indexOf('p' + $(elem).closest('tr').attr('id')) == 0 && !$(elem).closest('td').is(':visible'))
                                        return true;
                                }

                                if (e.is('.bocrud-validation-ignore,.cbox,.ui-pg-input,.ui-pg-selbox,.bucrud-disabled-action')) return true;
                                var p = e.closest('.bocrud-hidden-action,.bocrud-page-form');
                                if (p.length > 0 && p.is('.bocrud-hidden-action')) return true;
                                return false;
                            },
                            focusCleanup: true,
                            debug: false,
                            onfocusout: function (elem, event) {
                                if ($(elem).hasClass('hasDatepicker')) {
                                    setTimeout(function () {
                                        $(elem).valid();
                                    }, 1000);
                                    return true;
                                }

                                return $(elem).valid();
                            }
                        });
                        $form.validate(vo);
                    } else {
                        $form.addClass('ignore-validation');
                    }
                }


                if (form.group) {
                    form.groups.splice(0, form.groups.length);
                    form.groups = null;
                }
                $form.data('formObj', form);

                if (!window.bocruds)// when web redirect programicaly
                    return;
                var cb = window.bocruds[form.jsbid];

                cb.raiseEvent('onregform', form, form.mode, $form, form.xml);
                cb.raiseEvent('oncontrolplaced', form, form.mode, $form, form.xml);
                cb.raiseEvent('oneverycontrolplaced', form, form.mode, $form, form.xml);
                cb.raiseEvent('onshowcontentcomplete', form, form.mode, $form, form.xml);

                $.bocrud.decorateUi($form);

                $.bocrud.alignGrid($form);

                $form.find('a.bocrud-form-anchor').click(function () {
                    var t = $(this);
                    var formObj = t.closest('.bocrud-page-form').data('formObj');
                    var copyText = t.find('input');
                    copyText.val(formObj.anchor);
                    copyText.select();
                    copyText.get(0).setSelectionRange(0, 99999);
                    var result = document.execCommand('copy');
                    if (result)
                        cb.raiseEvent('onanchorcopy', $(formObj.title).text());
                    else
                        cb.raiseEvent('onanchorcopyfailed', $(formObj.title).text());
                });

            },
            addButton: function (target, p) {
                p = $.extend({
                    id: "",
                    caption: "newButton",
                    title: '',
                    context: '',
                    buttonicon: 'ui-icon-newwin',
                    onClickButton: null,
                    position: "last",
                    classes: '',
                    dependToRow: false,
                    showText: false
                }, p || {});

                if (typeof (target) == 'string' && target.indexOf("#") != 0) {
                    target = "#" + target;
                }

                var tbd = $("<button type=\"button\" title=\"" + p.title + "\" />");
                $(tbd).addClass(p.classes)
                    .append("<span class=\"" + p.buttonicon + "\"></span>")
                    .click(function (e) {
                        if (!$(this).hasClass('ui-state-disabled')) {
                            if ($.isFunction(p.onClickButton))
                                return p.onClickButton(p.context);
                        }

                        return false;
                    });
                if (p.showText)
                    $(tbd).append("<span>" + p.caption + "</span>");

                if (p.id) { $(tbd).attr("id", p.id); }
                if (p.align) { $(target).attr("align", p.align); }
                var findnav = $(target);
                p.position = 'last';
                if (p.position === 'first') {
                    $(findnav).prepend(tbd);
                } else {
                    $(findnav).append(tbd);
                }
                tbd.data('context', p.context);
                if (p.title && p.title.length > 0) {
                    $(tbd).tooltip();
                }
                return tbd;
            },
            row_dblclick_action: function (rowid, op, pos, url, jsbid) {
                if (pos == "BrowserTab") {
                    window.open('Index' + url);
                    return;
                }
                var b = window.bocruds[jsbid];
                if (b.config.mode == "GridDataEntry") {
                    if (b.grid().find('tr#' + rowid).is('.bocrud-page-form')) {
                        return;
                    }
                }
                b.get_form(rowid, op, b.config.xml);
            },
            _grid_beforeSend: function (jsbid, xhr, settings) {
                xhr.start_time = new Date();
                settings.showLoading = true;
                settings.global = true;
                settings.msg = $.bocrud.msg.grid_loading;
            },
            _grid_beforeProcessing: function (jsbid, data, st, xhr) {


                if (data && data.columns && data.columns.length > 0) {
                    setTimeout(function (data, jsbid) {
                        var b = window.bocruds[jsbid];
                        var grid = b.grid();
                        var gridOptions = clone(grid.jqGrid('getGridParam'));
                        for (var i = 0; i < data.columns.length; ++i) {
                            var col = data.columns[i];
                            if (col.dataType == 'integer') {
                                col['formatter'] = 'integer';
                                col['classes'] = 'ltr';
                            }
                        }
                        gridOptions.colModel = data.columns;
                        gridOptions.colNames = [];
                        for (var i = 0; i < gridOptions.colModel.length; ++i) {
                            gridOptions.colNames.push(gridOptions.colModel[i].caption);
                        }
                        $.jgrid.gridUnload(grid.selector);
                        gridOptions.datatype = 'jsonstring';
                        gridOptions.datastr = JSON.serialize(data);
                        var grid = $(grid.selector).jqGrid(gridOptions);
                        b._grid = null;

                        setTimeout(function (jsbid) {
                            var b = window.bocruds[jsbid];
                            var grid = b.grid();
                            grid.jqGrid('setGridParam', {
                                datatype: 'json'
                            });
                        }, 100, jsbid);

                    }, 1, data, jsbid);

                    return true;
                }

                var target = $('#pg_' + jsbid + '-GridPager');
                $.bocrud._get_request_time(xhr, target);
            },
            _get_request_time: function (xhr, target) {
                var header_lines = xhr.getAllResponseHeaders().split('\n');
                var headers = [];
                for (var i = 0; i < header_lines.length; ++i) {
                    var kv = header_lines[i].split(':');
                    if (kv.length <= 1) continue;
                    headers.push({ key: kv[0], value: kv[1].trim() });
                }
                var elapsedTime = null;
                for (var i = 0; i < headers.length; ++i) {
                    if (headers[i].key == 'elapsedtime') {
                        elapsedTime = parseInt(headers[i].value, 10);
                        break;
                    }
                }
                var client_elapsedTime = new Date() - xhr.start_time;
                var logpc = target.find('.bocrud-srlog');
                if (logpc.length == 0) {
                    target.append('<span class="bocrud-srlog"/>');
                    logpc = target.find('.bocrud-srlog');
                }
                var t = [];
                t.push($.bocrud.captions.compute_time);
                t.push(elapsedTime / 1000);
                t.push('s ');
                t.push($.bocrud.captions.network_latency);
                t.push(client_elapsedTime - elapsedTime);
                t.push('ms');
                logpc.text(t.join(''));
            }
        });


    $.validator.addMethod('server', function (value, element, param) {
        var form = $(element).closest("form");
        var id = form.data('formObj').jsbid;
        return window.bocruds[id].remoteValidation(value, element, param);
    });

    var run_out_of_scope_events = function (type, arg) {
        var rslt = true;
        var clonedEvents = [];

        var is_same_type = function (e, type) {
            var same_type = false;
            if ($.isArray(e.type)) {
                same_type = e.type.length == 1 && e.type[0] == type;
                if (!same_type) {
                    var index = -1;
                    for (var i = 0; i < e.type.length; ++i)
                        if (e.type[i] == type) {
                            index = i; break;
                        }
                    if (index >= 0)
                        e.type.splice(index, 1);
                }
            } else
                same_type = e.type == type;
            return same_type;
        }

        for (var i = 0; i < $.bocrud.globalEvents.length; ++i) {
            var e = $.bocrud.globalEvents[i];
            if (is_same_type(e, type))
                clonedEvents.push({ type: e.type, func: e.func, key: e.key });
        }
        var executed_groups = [];
        for (var i = 0; i < clonedEvents.length; ++i) {

            var e = clonedEvents[i];
            try {
                var args = arguments.length == 2 && $.isArray(arg) ? arg : $.makeArray(arguments).slice(1);
                var result = e.func.apply(this, args);
                executed_groups.push(e.key);
                if (result === false)
                    rslt = undefined;
            }
            catch (ex) {
                $.console.error("bocrud-599:call back of " + type + " by key " + e.key + " error : " + ex);
                rslt = undefined;
            }
        }
        //if (executed_groups.length == 0)
        //    executed_groups.push("[none]");
        //$.console.info("event " + type + " was raised for " + executed_groups.join(','));

        return rslt;
    };


    $(document).ajaxSend(function (event, xhr, settings) {
        run_out_of_scope_events.apply(this, ["onAjaxSend", xhr, settings]);
    });

    $(document).ajaxComplete(function (event, xhr, settings) {
        run_out_of_scope_events.apply(this, ["onAjaxComplete", xhr, settings]);
    });

})(jQuery);


function bocrud(config) {
    var currBocrud = this;

    this.config = $.extend({
        id: "",
        title: "",
        xml: "",
        boId: "",
        hideGrid: false,
        mode: "DataEntry",
        isSupperType: false,
        tempMode: false,
        useDialog: false,
        urlPrefix: "",
        isTree: false,
        validator: {
            min: 0,
            max: 1000,
            minMsg: '',
            maxMsg: ''
        },
        parentField: "",
        isMultiSelect: false,
        context: "",
        style: 'Tabular'
    }, config);

    this.detailsPostFix = "Details";
    this.contentPostFix = "Content";
    this.contentId = config.id + this.contentPostFix;
    this.detailsContainerId = config.id + this.detailsPostFix;
    this.isLoaded = false;

    this.config.useDialog = currBocrud.config.detPos == "ExpandedDialog" || currBocrud.config.detPos == "NormalDialog";

    this.events = new Array();

    this.hasEvent = function (type, key) {
        var es = currBocrud.events;
        for (var i = 0; i < es.length; ++i)
            if ((!type || es[i].type == type) && (!key || es[i].key == key))
                return true;

        return false;
    }

    this.addEvent = function (type, func, key) {

        if (!func) return this;

        if (key && key.length > 0)
            for (var i = 0; i < currBocrud.events.length; ++i)
                if (currBocrud.events[i].type == type &&
                    ((key && currBocrud.events[i].key && currBocrud.events[i].key == key) ||
                        currBocrud.events[i].func == func))
                    return this;

        currBocrud.events.push({ type: type, func: func, key: key });
        return this;
    }

    this.removeEvent = function (type, key) {
        var index = -1;
        for (var i = 0; i < currBocrud.events.length; ++i)
            if (currBocrud.events[i].type == type && key && currBocrud.events[i].key && currBocrud.events[i].key == key) {
                index = i;
                break;
            }
        if (index >= 0)
            currBocrud.events.splice(index, 1);

        index = -1;
        for (var i = 0; i < $.bocrud.globalEvents.length; ++i)
            if ($.bocrud.globalEvents[i].type == type && key && $.bocrud.globalEvents[i].key && $.bocrud.globalEvents[i].key == key) {
                index = i;
                break;
            }
        if (index >= 0)
            $.bocrud.globalEvents.splice(index, 1);
    }

    this.closeModal = function (div, dialogParent, remove) {
        $.bocrud.closeModal(div, dialogParent, remove);
        currBocrud.raiseEvent('modalclose', div, dialogParent, remove);
    }
    this.openModal = function (div, o) {
        var oldshowComplete = o.showComplete;
        $.bocrud.openModal(div, $.extend(o, {
            showComplete: function () {
                if ($.isFunction(oldshowComplete))
                    oldshowComplete.apply(this);
                currBocrud.raiseEvent('modalshowcomplete', div, o);
            }
        }));
        currBocrud.raiseEvent('modalopen', div, o);
    }


    this.raiseEvent = function (type, arg) {
        var rslt = true;
        var clonedEvents = [];

        var is_same_type = function (e, type) {
            var same_type = false;
            if ($.isArray(e.type)) {
                same_type = e.type.length == 1 && e.type[0] == type;
                if (!same_type) {
                    var index = -1;
                    for (var i = 0; i < e.type.length; ++i)
                        if (e.type[i] == type) {
                            index = i; break;
                        }
                    if (index >= 0) {
                        return true;
                    }
                }
            } else
                same_type = e.type == type;
            return same_type;
        }

        for (var i = 0; i < currBocrud.events.length; ++i) {
            var e = currBocrud.events[i];

            if (is_same_type(e, type))
                clonedEvents.push({ type: e.type, func: e.func, key: e.key });
        }

        for (var i = 0; i < $.bocrud.globalEvents.length; ++i) {
            var e = $.bocrud.globalEvents[i];
            if (is_same_type(e, type))
                clonedEvents.push({ type: e.type, func: e.func, key: e.key });
        }
        var executed_groups = [];
        for (var i = 0; i < clonedEvents.length; ++i) {

            var e = clonedEvents[i];
            try {
                var args = arguments.length == 2 && $.isArray(arg) ? arg : $.makeArray(arguments).slice(1);
                var result = e.func.apply(currBocrud, args);
                executed_groups.push(e.key);
                if (result === false)
                    rslt = undefined;
            }
            catch (ex) {
                $.console.error("bocrud-528:call back of " + type + " by key " + e.key + " error : " + ex);
                rslt = undefined;
            }
        }
        if (executed_groups.length == 0)
            executed_groups.push("[none]");
        //$.console.info(this.config.id + " => event " + type + " was raised for " + executed_groups.join(','));

        currBocrud.runJs(type);
        return rslt;
    }

    this.destroy = function () {
        window.bocruds[currBocrud.config.id] = null;
        $('#' + currBocrud.contentId).html('');
        $('#' + currBocrud.detailsContainerId).html('');
    }

    this.runJs = function (type, area, ignore_timeout) {

        var get_events = function () {
            var js;
            if (area)
                js = area.find('textarea.bocrud-js[jsbid="' + currBocrud.config.id + '"]');
            else
                js = $('textarea.bocrud-js[jsbid="' + currBocrud.config.id + '"]');

            js = js.filter(function () {
                var e = $(this).attr('event');
                return !e || e == type;
            })
            return js;
        }

        var events = get_events();

        events.each(function () {
            var t = $(this);
            var nottemp = t.attr('temp') == 'false';
            var was_run = t.data('run-js-runned');

            if (!nottemp && was_run) {
                t.remove();
                return;
            }

            t.data('run-js-runned', true);
            try {
                eval(t.text());
            } catch (ex) {
                $('<script language="javascript">' + t.text() + '</script>').appendTo('body');
            }

            if (!nottemp)
                t.remove();

        });

        events = get_events();
    }
    this.validOpts = {};


    this.ckeditor = function (uid, uri, config) {
        var ruid = uid + "_ckeditor";

        if ($('body>div[tag=ckeditor]').length == 0) {
            $('head').append('<' + 'script src="' + uri + 'ckeditor.js" type="text/javascript"></script>');
            $('body').append('<div tag="ckeditor"/>');
        }

        var instance = CKEDITOR.replace(ruid, { customConfig: '/assets/plugins/CKEditor/' + config + '_config.js' });

        var updateInputFromCkeditorData = function () {
            CKEDITOR.instances[ruid].updateElement();
            var val = CKEDITOR.instances[ruid].getData();
            var base64Value = "Base64" + Base64.encode(val);
            $('#' + uid).val(base64Value)
        }

        instance.on('change', function () {
            updateInputFromCkeditorData();
        });

        instance.on('instanceReady', function () {
            updateInputFromCkeditorData();
        });

    };

    this.redirect = function (sender, action, params, defaultParams, method) {
        var p = params || {
        };
        if (defaultParams === true) {
            if (!p.xml)
                p.xml = config.xml;
            p.jsbid = config.id;
            p.tempMode = config.tempMode;
            p.nv = Date().toString();
            p.context = config.context;
        }
        var url = config.urlPrefix + "/" + action + "?";
        for (var pn in p)
            url += pn + "=" + p[pn] + "&";
        if (url[url.length - 1] == '&')
            url = url.substr(0, url.length - 1);

        if (method && method.toLowerCase() == "post") {
            var form = sender.closest("form[rel='" + currBocrud.config.id + "-details']");
            var action = form.attr('action');
            form.attr('action', url);

            form.submit();

        } else
            document.location = url;
    }

    this.getData = function (action, data, ajaxObj) {
        if (!data.xml)
            data.xml = config.xml;
        data.jsbid = config.id;
        data.parentJsbid = config.parent;
        data.tempMode = config.tempMode;
        data.nv = Date().toString();
        data.context = config.context;

        var url = config.urlPrefix + "/" + action;

        for (var key in data) {
            if ($.isArray(data[key])) {
                if (data[key].length == 1)
                    data[key] = data[key][0];
                else if (data[key].length == 0)
                    data[key] = '';
            }
        }

        $.ajax($.extend({
            data: data,
            showLoading: true,
            url: url,
            dataType: "html",
            type: "POST",
            error: function (data) {
                $.console.error("bocrud-341:server return error." + data.responseText);
            }
        }, ajaxObj));
    }

    var rvQueue = [];
    var rvHandler = null;
    var clearRvHandler = null;
    var rvLastCall = null;
    var rvIsInProgress = false;
    var rvLastResult = {
    };
    this.remoteValidation = function (value, element, param, timeout) {
        var sender = $(element);
        var name = sender.closest('.bocrud-control').attr('name');

        var form = sender.closest("form");
        var validator = form.data('validator');
        if (!validator) return false;

        var previous = validator.previousValue(element);
        if (!validator.settings.messages[element.name]) {
            validator.settings.messages[element.name] = {
            };
        }
        previous.originalMessage = validator.settings.messages[element.name].remote;
        validator.settings.messages[element.name].remote = previous.message;

        if (rvLastResult[element.name] && value == rvLastResult[element.name].value)
            return rvLastResult[element.name].result;

        previous.old = value;
        var exists = false;
        for (var i = 0; i < rvQueue.length; ++i)
            if (rvQueue[i].name == name) {
                exists = true; break;
            }

        if (!exists)
            rvQueue.push({
                name: name,
                value: value,
                sender: sender,
                param: param
            });
        else if (!timeout) {
            return "pending";
        }

        rvLastCall = new Date();
        if (!clearRvHandler) {
            clearRvHandler = setInterval(function () {
                if (rvQueue.length == 0) {
                    clearInterval(clearRvHandler);
                    clearRvHandler = null;
                    return;
                }
                var now = new Date();
                if (now - rvLastCall > 5000 && !rvIsInProgress) {
                    rvQueue.splice(0, rvQueue.length);
                    validator.pendingRequest = 0;
                    validator.pending = {
                    };
                }
            }, 1000);
        }

        validator.startRequest(element);

        if (!timeout) {
            if (rvHandler) {
                clearTimeout(rvHandler);
                rvHandler = setTimeout(function () { currBocrud.remoteValidation(value, element, param, true); }, 500);
                return "pending";
            }
            else {
                rvHandler = setTimeout(function () { currBocrud.remoteValidation(value, element, param, true); }, 500);
                return "pending";
            }
        } else {
            if (rvHandler)
                clearTimeout(rvHandler);
            rvHandler = null;
        }

        var data = {
        };
        data.xml = config.xml;
        data.jsbid = config.id;
        data.tempMode = config.tempMode;
        data.uidprefix = sender.closest('.bocrud-control').attr('uidPrefix');
        var p = [];
        for (var i = 0; i < rvQueue.length; ++i) p.push(rvQueue[i].name);
        data.p = p.join(',');

        var url = config.urlPrefix + "/RemoteValidation";

        data.key = form.data('formObj').boKey;

        form.find(':file:enabled:not(.temp-disable)').each(function () {
            $(this).addClass('temp-disable').attr('disabled', 'disabled');
        });

        var caption = [];
        for (var i = 0; i < rvQueue.length; ++i) {
            var c = rvQueue[i].sender.closest('.bocrud-control').first('.bocrud-control-caption').text();
            if (c && c.length > 0) {
                c = c.split(':').join('');
                caption.push(c);
            }
        }

        rvIsInProgress = true;

        form.ajaxSubmit({
            showLoading: $.bocrud.submit_validating,
            msg: $.bocrud.msg.sending_for_validation + ' ' + caption.join(' '),
            data: data,
            url: url,
            dataType: "html",
            type: "post",
            beforeSubmit: function () {
                form.find(':file.temp-disable').each(function () {
                    $(this).removeClass('temp-disable').removeAttr('disabled');
                });
            },
            error: function (data) {
                for (var i = 0; i < rvQueue.length; ++i) {
                    var element = rvQueue[i].sender.get(0);
                    var errors = {
                    };
                    var message = validator.defaultMessage(element, "server");
                    errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
                    errors[element.name] += ' ' + $.bocrud.msg.cannot_connect_to_server;
                    validator.invalid[element.name] = true;
                    validator.showErrors(errors);
                }
                rvIsInProgress = false;
            },
            success: function (response) {

                var jr = JSON.parse(response);

                if (jr.error) {
                    currBocrud.showError(jr);
                }
                for (var i = 0; i < rvQueue.length; ++i) {
                    var valid = true;
                    for (var j = 0; j < jr.invalid.length; ++j)
                        if (jr.invalid[j] == rvQueue[i].name) {
                            valid = false; break;
                        }
                    var element = rvQueue[i].sender.get(0);

                    if (rvLastResult[element.name]) delete rvLastResult[element.name];

                    rvLastResult[element.name] = {
                        value: rvQueue[i].value,
                        result: valid
                    };

                    $(element).removeData('success-message');

                    if (valid) {
                        var success_message = jr.messages[rvQueue[i].name];
                        if (success_message && success_message.length > 0)
                            $(element).data('success-message', success_message);

                        var submitted = validator.formSubmitted;
                        validator.formSubmitted = submitted;
                        validator.successList.push(element);
                        delete validator.invalid[element.name];
                        validator.showErrors();
                    } else {
                        var message = jr.messages[rvQueue[i].name] || validator.defaultMessage(element, "server");
                        validator.invalid[element.name] = true;

                        validator.errorList.push({
                            message: message,
                            element: element
                        });

                        validator.showErrors();
                    }
                    previous.valid = valid;
                    validator.stopRequest(element, valid);
                }

                for (var i = 0; i < jr.request.length; ++i) {
                    var rvi = -1;
                    for (var j = 0; j < rvQueue.length; ++j)
                        if (jr.request[i] == rvQueue[j].name) {
                            rvi = j; break;
                        }
                    if (rvi >= 0) {
                        rvQueue.splice(rvi, 1);
                    }
                }

                rvIsInProgress = false;
            }
        });

        return "pending";
    }


    this.userCmd = function (selrows, grid, ctx, oncomplete, showAsMain, bctor, cmd_name, confirm_msg, searchy, reloadGrid, passColumnsData) {

        var columnData = '';
        if (passColumnsData) {
            var columns = passColumnsData.split(',');
            var dataList = [];
            var row_ids = [];
            if (typeof (selrows) == "string") row_ids = [selrows];
            else row_ids = selrows;
            for (var i = 0; i < row_ids.length; ++i) {
                var row = $(grid).jqGrid('getRowData', row_ids[i]);
                var obj = {};
                for (var j = 0; j < columns.length; ++j) {
                    var c = columns[j].trim();
                    if (c.length == 0) continue;
                    if (row[c].indexOf('<') >= 0)
                        obj[c] = $(row[c]).text();
                    else
                        obj[c] = row[c];
                }
                dataList.push(obj);
            }
            columnData = Base64.encode(JSON.serialize(dataList));
        }

        if (confirm_msg && confirm_msg.length > 0)
            if (!confirm(confirm_msg))
                return;
        var b = ctx.split('|');
        var before = b[b.length - 2];
        var after = b[b.length - 1];

        if (before[0] == '{' && before[before.length - 1] == '}') {
            before = before.substring(1, before.length - 1);
            var result = false;
            try {
                result = eval(before);
            }
            catch (e) {
                $.console.error('bocrud:365 ' + e);
            }
            if (!result)
                return;
            before = null;
        }

        var data = currBocrud.grid().jqGrid("getGridParam", 'postData') || {
        };
        data['xml'] = config.xml;
        data['jsbid'] = config.id;
        data['ctx'] = b.splice(0, b.length - 2).join('');
        data['selrows'] = selrows;
        data['before'] = before;
        data['after'] = after;
        data['ctor'] = bctor;
        data['userCmd'] = cmd_name;
        data['cmd_model'] = currBocrud.config.xml;
        data['columnsData'] = columnData;

        if (searchy) {
            var search_form_id = "search-" + currBocrud.config.id;
            var sf = $('#' + search_form_id);
            if (!currBocrud.validate(sf))
                return;

            var grid = currBocrud.grid();
            var g_p = grid.jqGrid('getGridParam');
            var orgPostData = g_p.postData;
            var postData = {
                _search: true,
                fobj: "1",
                jsbid: orgPostData.jsbid,
                page: orgPostData.page,
                rows: orgPostData.rows,
                sidx: orgPostData.sndx,
                sord: orgPostData.sord,
                tempMode: orgPostData.tempMode,
                xml: orgPostData.xml
            };

            var formData = sf.formToArray();

            for (var i = 0; i < formData.length; ++i)
                postData[formData[i].name] = null;

            for (var i = 0; i < formData.length; ++i) {
                if (postData[formData[i].name] != null)
                    postData[formData[i].name] += "," + formData[i].value;
                else
                    postData[formData[i].name] = formData[i].value;
            }

            data = $.extend(data, postData);
        }

        currBocrud.raiseEvent("onusercmd", data, showAsMain);

        var main_show = function (formObj, mode) {
            if (formObj.error) {
                currBocrud.raiseEvent("onshowcontenterror", formObj);
                return;
            }
            var old_form_container = currBocrud.raiseEvent('onpreshowcontent', formObj);
            if (!old_form_container) {
                return;
            }
            currBocrud.raiseEvent('onunload', old_form_container, mode);

            if (old_form_container && old_form_container.find)
                old_form_container.find('[catch-display-events]').trigger('onhide', [{
                }]);

            currBocrud.show_form(formObj, mode, formObj.xml);
            $.bocrud.reg_form(formObj);
            return;
        }

        var onSubmitSuccess = function (response, container, formObj, boAsJson, script_evaled) {

            if (!currBocrud.raiseEvent("oncmdsuccessbeforeevalresponse", response, container, formObj, boAsJson)) {
                return;
            }

            if ((!formObj || formObj.error !== true) && reloadGrid && (!formObj || formObj.Success !== false))
                setTimeout(function () { currBocrud.grid().trigger("reloadGrid"); }, 1000);

            if (response != "OK") {

                if (formObj && typeof (formObj.Success) == 'boolean') {
                    if ($.isFunction(oncomplete))
                        oncomplete(formObj);
                    return formObj.Success;
                }

                // ممکنه از سرور یه اسکریپت برای اجرا بیاد و مدل نباشه
                if (!formObj || formObj.error || !formObj.groups) {

                    if (!script_evaled)
                        formObj = eval('(' + response + ')');

                    if (formObj.error) {
                        currBocrud.raiseEvent("oncmderror", formObj);
                        return;
                    }

                    if (!formObj || !formObj.groups) {
                        currBocrud.raiseEvent("oncmdsuccess", formObj);
                        return;
                    }
                }

                if (showAsMain) {
                    main_show(formObj, 'View');

                    if ($.isFunction(oncomplete))
                        oncomplete(formObj);

                    return;
                }

                var $form = $("#" + after);
                if ($form.length == 0) {

                    $('body').append("<form id='" + after + "' style='direction:" + $.bocrud.dir + "' class='bocrud-page-form'></form>");
                    $form = $("#" + after);
                }

                $.bocrud.basicShow($form, formObj);

                currBocrud.raiseEvent('oncontrolplaced', formObj, 'DataEntry', $form);
                currBocrud.raiseEvent('oneverycontrolplaced', formObj, 'DataEntry', $form);
                var buttons = {
                };
                buttons[$.jgrid.edit.bClose] = function () {
                    currBocrud.closeModal($(this), $form, true);

                    currBocrud.raiseEvent("oncmdsresultmodalclose", formObj);

                }

                currBocrud.openModal($form, {
                    buttons: buttons,
                    title: formObj.title,
                    width: formObj.width > 0 ? formObj.width : 400,
                    height: formObj.height > 0 ? formObj.height : 300,
                    modal: false,
                    position: 'top'
                });
                if ($.isFunction(oncomplete))
                    oncomplete(formObj);
            }

            return true;
        };

        if (before && before.length > 0) {

            var buttons = {
            };


            if (showAsMain) {
                currBocrud.get_form('', 'DataEntry', before, function (formObj) {

                    buttons[$.jgrid.edit.bCancel] = function () {
                        if (!currBocrud.raiseEvent('onviewcancel', formObj))
                            return;
                    }

                    buttons[$.jgrid.edit.bSubmit] = function () {

                        currBocrud.addEvent('onsubmitsuccess', function (formObj, saveResult, showDetails) {
                            onSubmitSuccess(saveResult, null, formObj, null);
                            currBocrud.removeEvent('onsubmitsuccess', 'user-cmd');
                            currBocrud.removeEvent('onsubmiterror', 'user-cmd');
                        }, 'user-cmd');

                        currBocrud.addEvent('onsubmiterror', function () {
                            currBocrud.removeEvent('onsubmitsuccess', 'user-cmd');
                            currBocrud.removeEvent('onsubmiterror', 'user-cmd');
                        }, 'user-cmd');

                        return submit($('#' + formObj.id), data, '', true, false, 'UserCommand');
                    }

                    currBocrud.raiseEvent("oncmdbeforeshown", $('#' + formObj.id), formObj);
                }, buttons, $.extend({ usrctx: data['ctx'], selrows: selrows }, data));
                return;
            } else {
                buttons[$.jgrid.edit.bCancel] = function () {
                    var $form = $(this).find('.bocrud-page-form:first');
                    var formObj = $form.data('formObj');
                    if (!currBocrud.raiseEvent('onviewcancel', formObj))
                        return;
                    currBocrud.closeModal($form, $('body'), true);
                    $form.remove();
                }
            }

            currBocrud.showDialog({
                xml: before,
                jsbid: currBocrud.config.id,
                container_id: before,
                text: {
                    bCancel: $.jgrid.edit.bCancel,
                    bSubmit: $.jgrid.edit.bSubmit
                },
                submitData: data,
                getData: {
                    usrctx: data['ctx'], selrows: selrows, ctor: bctor, userCmd: cmd_name, cmd_model: currBocrud.config.xml, columnsData: columnData
                },
                submitUrl: currBocrud.config.urlPrefix + "/UserCommand",
                bocrud: currBocrud,
                onSubmitSucess: onSubmitSuccess,
                onPreSubmit: null,
                onSubmitError: null,
                onCancel: null,
                onShowError: null,
                modal: true,
                onDialogOpen: null,
                buttons: buttons
            });
            return;
        }

        var _reloadGrid = reloadGrid;
        $.ajax({
            showLoading: true,
            msg: $.bocrud.msg.progress,
            data: data,
            url: currBocrud.config.urlPrefix + "/UserCommand",
            type: "POST",
            semantic: true,
            error: function (response) {
                currBocrud.showError(response.responseText);
            },
            success: function (r) {

                if (_reloadGrid)
                    setTimeout(function () { currBocrud.grid().trigger("reloadGrid"); }, 1000);

                if (r == "OK") {
                    currBocrud.raiseEvent("oncmdsuccess", data);
                    return;
                }

                var script_evaled = true;
                var formObj = eval('(' + r + ')');

                if (formObj && formObj.error) {
                    currBocrud.raiseEvent("onshowcontenterror", formObj);
                    return;
                }

                onSubmitSuccess(r, null, formObj, null, script_evaled);
            }
        });
    }

    this.lazy = function (uid, ps, asNew, delay, send) {

        var t = $('#' + uid);

        if (t.is(':visible')) {

            if (t.find('.bocrud-control-loading').length == 0)
                $('<div/>').addClass('bocrud-control-loading').text($.jgrid.defaults.loadtext).appendTo(t);

            setTimeout(function () {

                currBocrud.partialSubmit(uid, currBocrud.config.xml, ps, send, '', asNew, true, function () {
                    $('#' + uid).children('.bocrud-control-loading').remove();
                });

            }, delay || 1000);
        } else
            setTimeout(function () {
                currBocrud.lazy(uid, ps, asNew, delay, send);
            }, 350);
    }

    this.pcall = function (sender, ticket, userdata, callback, ajaxObj, validate, databind, background, mode) {
        sender = $(sender);
        var data = {
        };
        data.xml = (userdata && userdata.xml) || config.xml;
        data.jsbid = config.id;
        data.tempMode = config.tempMode;
        data.databind = databind;
        var form = sender.closest(".bocrud-page-form,.jqgrow");
        data.uidprefix = form.attr('namespace');
        if (userdata)
            data.userdata = escape(JSON.serialize(userdata));
        data.ticket = ticket;
        data.mode = mode || 'View';

        var url = config.urlPrefix + "/PropertyCommand";

        if (sender && validate) {
            var isValid = true;
            var invalidFormTitles = "";
            form.find('form[rel]').each(function () {
                if ($(this).valid)
                    if (!$(this).valid()) {
                        isValid = false;
                        invalidFormTitles += $(this).attr('title');
                    }
            });

            if (form.valid && !form.valid()) return;
        }

        if (!currBocrud.raiseEvent('onsubmit', 'pcall'))
            return;

        var files = [];
        if (databind) {
            form.find(':file:enabled').each(function () {
                var t = $(this);
                files.push(t);
                t.attr('disabled', 'disabled');
            });
            form.find('.cke').each(function () {
                var t = $(this).prev();
                files.push(t);
                t.attr('disabled', 'disabled');
            });
        }

        var ch;
        if (!background)
            ch = sender.closest('.bocrud-control').addClass('bocrud-control-loading');

        var finalAjaxObj = $.extend({
            msg: $.bocrud.msg.updating_form,
            data: data,
            url: url,
            dataType: "html",
            type: "post",
            error: function (data) {
                if (!background)
                    ch.removeClass('bocrud-control-loading');
                currBocrud.raiseEvent('onpcallerror', sender, data);
                if (callback)
                    callback(data);
            },
            success: function (response) {
                if (response && typeof (response) == 'string' && response.indexOf('error') > 0) {
                    var errObj = eval('(' + response + ')');
                    currBocrud.showError(errObj);
                    if (!background)
                        ch.removeClass('bocrud-control-loading');
                    currBocrud.raiseEvent('onpcallerror', sender, response);
                    if (callback)
                        callback(response);
                    return;
                }
                if (!background)
                    ch.removeClass('bocrud-control-loading');
                currBocrud.raiseEvent('onpcallsuccess', sender, response);
                if (callback)
                    callback(response);
            }
        }, ajaxObj || {
        });

        if (databind) {
            form.ajaxSubmit(finalAjaxObj);
        } else {
            $.ajax(finalAjaxObj);
        }

        if (databind) {
            $.each(files, function (key, value) {
                value.removeAttr('disabled');
            });
        }
    }

    this.raiseServerEvents = function (eventType, data) {
        //        data["et"] = eventType;
        //        currBocrud.getData("RaiseServerEvents", data, {});
    }

    this._finderCache = [];
    this.$ = function (selector, local) {
        for (var i = 0; i < currBocrud._finderCache.length; ++i)
            if (currBocrud._finderCache[i].key == selector)
                return currBocrud._finderCache[i].value;
        var val = local ? currBocrud.pane().find(selector) : $(selector);
        currBocrud._finderCache.push(selector, val);
        return val;
    }

    this._thumbnail_grid = function () {
        if (currBocrud.config.thumbCol &&
            currBocrud.config.thumbCol.length > 0 &&
            currBocrud.config.thumbTriggers &&
            currBocrud.config.thumbTriggers.length > 0) {

            var compute_position = function (tr, thumbnail, e) {
                var w = thumbnail.outerWidth(), h = thumbnail.outerHeight();
                var x = e.pageX - (w / 2), y = tr.offset().top - h;
                var ww = $(window).width(), wh = $(window).height();

                var pos = {
                    left: x, top: y
                };

                return pos;
            }

            currBocrud.grid().find('tr.jqgrow').hover(
                function (e) {
                    var tr = $(this);
                    var thumbnail = tr.data('thumbnail') || $('#' + currBocrud.config.id + '-' + tr.attr('id') + 'thumb');
                    if (!thumbnail || thumbnail.length == 0) {
                        var grid = currBocrud.grid();
                        var row = grid.jqGrid("getRowData", tr.attr('id'));
                        var t = row[currBocrud.config.thumbCol];
                        if (t && t.length > 0) {
                            var txt = $(t).text();
                            if (txt != '--' && txt.length > 0 || t.indexOf('img') >= 0) {
                                thumbnail = $('<div/>')
                                    .attr('id', currBocrud.config.id + '-' + tr.attr('id') + 'thumb')
                                    .addClass('bocrud-thumbnails tooltip fade top in')
                                    .append('<div class="tooltip-arrow"></div>')
                                    .append($('<div class="tooltip-inner"/>')
                                        .html(row[currBocrud.config.thumbCol]))
                                    .css({ display: 'none', position: 'absolute', 'z-index': 2000 })
                                    .css(compute_position(tr, thumbnail, e))
                                    .appendTo('body');
                                tr.data('thumbnail', thumbnail);
                                thumbnail.data('tr', tr);
                            }
                        }
                    }
                    if (!tr.is(':has(td.thumbnail-trigger)')) {
                        var selector = [];
                        for (var i = 0; i < currBocrud.config.thumbTriggers.length; ++i)
                            selector.push('td[aria-describedby="' + currBocrud.config.id + '-Grid_' + currBocrud.config.thumbTriggers[i] + '"]');
                        tr.find(selector.join(',').toString())
                            .hover(function (e) {
                                var tr = $(this).closest('tr');
                                var th = tr.data('thumbnail') || $('#' + currBocrud.config.id + '-' + tr.attr('id') + 'thumb');

                                $('.bocrud-thumbnails')
                                    .filter(':not(#' + currBocrud.config.id + '-' + tr.attr('id') + 'thumb)')
                                    .hide();

                                th.css(compute_position(tr, th, e))
                                    .show($.bocrud.effects.thumbnails.show);

                            }, function () {
                                var tr = $(this).closest('tr');
                                var th = tr.data('thumbnail') || $('#' + currBocrud.config.id + '-' + tr.attr('id') + 'thumb');
                                th.hide($.bocrud.effects.thumbnails.hide);
                            }).mousemove(function (e) {
                                var tr = $(this).closest('tr');
                                var th = tr.data('thumbnail') || $('#' + currBocrud.config.id + '-' + tr.attr('id') + 'thumb');
                                if (th)
                                    th.css(compute_position(tr, th, e));
                            }).addClass('thumbnail-trigger');
                        if ($(e.target).is(selector.toString())) {
                            thumbnail.css(compute_position(tr, thumbnail, e))
                                .show($.bocrud.effects.thumbnails.show);
                        }
                    }
                }
            );
        }
    };

    this.onGridLoad = function (fn, data) {

        if (currBocrud.grid().jqGrid('getGridParam', 'datatype') == 'local' || currBocrud.grid().jqGrid('getGridParam') == undefined)
            return;

        if (fn)
            fn();

        if (data && data.message) {
            $.bocrud.toast(currBocrud.config.id, data.message, 'information');

        }

        if (currBocrud.grid().jqGrid('getGridParam', 'shrinkToFit') == false) {
            $.bocrud.alignGrid(currBocrud.grid().closest('.ui-jqgrid').parent());
        }

        if (data && data.summary && data.summary.length > 0) {
            var formObj = eval('(' + data.summary + ')');
            if (formObj.error) {
                currBocrud.raiseEvent("onshowcontenterror", formObj);
            } else {
                var ph = $('#grid-' + currBocrud.config.id);
                if (formObj.detailPos == 'up')
                    ph = ph.prev('.bocrud-grid-summary-container-up');
                else
                    ph = ph.next('.bocrud-grid-summary-container-down');

                //formObj, mode, xml, place_holder, buttons
                currBocrud.show_form(formObj, 'View', formObj.xml, ph);
                $.bocrud.reg_form(formObj);
            }
        }

        if (!currBocrud.raiseEvent('ongridload', null))
            return;

        if (currBocrud.config.debug) {
            $('.ui-jqgrid-htable th').each(function () {
                var th = $(this);
                if (th.attr('id') && th.attr('id').length > 0) {
                    var splited_id = th.attr('id').split('_');
                    if (splited_id.length == 2) {
                        th.attr('title', splited_id[1]);
                    }
                }
            });
        }

        currBocrud.grid_data_fetched = true;

        if (data && data.error) {
            currBocrud.showError(data);
            return;
        }

        currBocrud.runJs("oncontrolplaced", currBocrud.grid());
        currBocrud.runJs("oneverycontrolplaced", currBocrud.grid());

        if (data && data.desc)
            currBocrud.$("#" + config.id + "PrintBtn", false)
                .data('desc', data.desc);

        if (data)
            $('#' + currBocrud.config.id + 'RefreshBtn').html('<i class="button-icon-with-padding  icon-refresh"/>' + $.bocrud.captions.bRefresh);
        currBocrud.initPasteBtn();

        if (currBocrud.config.standalone && currBocrud.config.mode == 'GridDataEntry') {
            var g = currBocrud.grid();
            $(data.rows).each(function () {
                var tr = g.find('tr[id="' + this.id + '"]');// not use tr#id
                tr.addClass('bocrud-page-form');
                var formObj = g.jqGrid('getRowData', this.id);
                formObj.jsbid = currBocrud.config.id;
                formObj.mode = "GridDataEntry";
                formObj.xml = currBocrud.config.xml;
                formObj.boKey = this.id;
                formObj.validationObj = this.validationObj;
                tr.data('formObj', formObj);
                currBocrud.show_form(formObj, formObj.mode, formObj.xml, null, null);
                $.bocrud.reg_form(formObj);
            });
        }

        if (!currBocrud.raiseEvent('ongridcomplete', null))
            return;

        currBocrud.$('#' + currBocrud.config.id + 'GridPane', true)
            .find('tr.ui-search-toolbar')
            .find(':input').addClass('bocrud-focusable');

        if (!window.hideIntervalID) {
            window.hideIntervalID = window.setInterval(function () {
                $('.bocrud-thumbnails:visible').each(function () {
                    var thumbnail = $(this);
                    var tr = thumbnail.data('tr');
                    if (!tr || !tr.is(':hover'))
                        thumbnail.hide();
                });
                $('.ui-jqgrid .jqgrow div[name=__operations]:visible').each(function () {
                    var t = $(this).closest('tr');
                    if (!t.is(':hover'))
                        $(this).hide();
                });
            }, 3000);
        }

        currBocrud._thumbnail_grid();

        currBocrud._update_validation_field();

        //ui-pg-table navtable
        //bp634355546221263205GridPager_left
        currBocrud.$('#' + currBocrud.config.id + 'GridPane', true).find('#' + currBocrud.config.id + 'GridPager_left').hide();
        // set auto complete for toolbar search
        //ui-state-default ui-jqgrid-hdiv
        var colModels = currBocrud.grid().jqGrid('getGridParam', 'colModel');

        // operations
        var has_op = false;
        for (var i = 0; i < colModels.length; ++i)
            if (colModels[i].name == "__operations") {
                has_op = true;
                break;
            }

        if (has_op) {
            currBocrud.grid().find('tr.jqgrow').hover(
                function () {
                    $(this).find('[name=__operations]').show('fade');
                },
                function () {
                    $(this).find('[name=__operations]').hide();
                });
        }// __operations



        var $grid = currBocrud.grid().closest('.ui-jqgrid-view');
        //Clear Search Value
        $grid.find(".ui-jqgrid-hdiv").find('a.clearsearchclass').click(function () {
            currBocrud.grid()[0].triggerToolbar();
        });
        $grid.find(".ui-jqgrid-hdiv").find(':input').each(function () {
            var $input = $(this);
            $input.keypress(function (e) {
                if (e.keyCode == 13) {
                    e.preventDefault();
                }
            });
            var colName = $input.attr('name');
            var autoc = $input.attr('autocomplete');
            var colModel = null;
            for (var i = 0; i < colModels.length; ++i)
                if (colModels[i].name == colName) {
                    colModel = colModels[i];
                    break;
                }
            if (colModel == null)
                return;



            switch (!0) {
                case colModel.dataType == 'text':
                    if ($.ui && $.ui.autocomplete && colModel.EnableAutoComplete == true)
                        $input.autocomplete({
                            close: function () {
                                $input.focus();
                            },
                            open: function () {
                                $input.focus();
                            },
                            source: function (term, callback) {
                                var data = {
                                };
                                data.p = colName;
                                data.t = term.term;

                                currBocrud.getData('GetAutoCompleteData', data, {
                                    global: false,
                                    async: false,
                                    success: function (response) {
                                        callback(eval(response));
                                    }
                                });
                            },
                            select: function (event, ui) {
                                $input.val(ui.item.value);
                                currBocrud.grid()[0].triggerToolbar();
                            }
                        });
                // dont break
                case colModel.dataType == 'integer':
                case colModel.dataType == 'float':
                    if ($input.is(":text"))
                        $input.keypress(function (e) {
                            if (e.which == 13)
                                currBocrud.grid()[0].triggerToolbar();
                        });
                    else
                        $input.change(function () {
                            currBocrud.grid()[0].triggerToolbar();
                        });
                    break;

                case colModel.dataType == 'date':
                    $input.attr('readonly', '');
                    setTimeout(function () {
                        if (!$input.hasClass('hasDatepicker'))
                            $input.myCalendar();
                    }, 2000);
                case colModel.dataType == 'bool':
                    $input.change(function () {
                        currBocrud.grid()[0].triggerToolbar();
                    });
                    break;
            }
        });

        var g = currBocrud.grid();

        if (!currBocrud.grid_event_bounded) {
            //onSortCol
            g.on('jqGridResizeStop', function (e, nw, idx) {
                currBocrud.raiseEvent('ongridcolumnresize', g, nw, idx);
            });
            g.on('jqGridSortCol', function (e, index, idxcol, so) {
                currBocrud.raiseEvent('ongridsorted', g, index, idxcol, so);
            });
            currBocrud.grid_event_bounded = true;
        }

        if (currBocrud.config.isTree) {
            var collapse = g.jqGrid('getGridParam', 'collapse');
            if (collapse)
                currBocrud.grid().find('.ui-icon-minus').click();
        }

        decorate_grid_rows();
    }

    this._update_validation_field = function () {
        var vf = currBocrud.$('#' + currBocrud.config.id + 'vf');
        if (vf.length == 1) {
            var rc = currBocrud.grid().jqGrid('getGridParam', 'records');
            vf.val(rc);
        }

    }

    var decorate_grid_rows = function () {
        var g = currBocrud.grid();

        g.find('[data-toggle="tooltip"]').tooltip();

        var cssClumn = g.jqGrid('getGridParam', 'cssColumn');
        if (cssClumn && cssClumn.length > 0) {
            var colModels = g.jqGrid('getGridParam', 'colModel');
            var cssColumnIndex = -1;
            for (var i = 0; i < colModels.length; ++i)
                if (colModels[i].name == cssClumn) {
                    cssColumnIndex = i;
                    break;
                }
            if (cssColumnIndex > -1) {
                g.find('tr.jqgrow').each(function () {
                    var tr = $(this);
                    var row_id = tr.attr('id');
                    var row = g.jqGrid('getRowData', row_id);
                    tr.addClass(row[cssClumn]);
                });
            }
        }
    }

    this.urlParams = {
    };

    var e,
        a = /\+/g,  // Regex for replacing addition symbol with a space
        r = /([^&;=]+)=?([^&;]*)/g,
        d = function (s) { return decodeURIComponent(s.replace(a, " ")); },
        q = window.location.search.substring(1);

    while (e = r.exec(q))
        this.urlParams[d(e[1])] = d(e[2]);

    this.getFormId = function (bokey) {
        return currBocrud.config.useDialog ?
            HashCode.value(currBocrud.config.id + bokey) + "-form" : currBocrud.config.id + "-form";
    }

    var getAddChildTextInfo = function () {
        var gobj = currBocrud.grid();
        var s = gobj.jqGrid('getGridParam', 'selrow');
        var parent = s == null ? null : gobj.jqGrid('getRowData', s);
        if (parent) {
            var strParent = currBocrud.item2Str(parent, currBocrud.config.toStringFormat, gobj);
            var text = '<i><b>' + $.bocrud.msg.create_child_item_for + ' :</b></i><blockquote>' + strParent + '</blockquote>';
            return text;
        }
        return '';
    }

    var cancelDataEntry = function (f, bokey) {
        if (currBocrud.config.style == 'Tabular')
            currBocrud.$('ul.bocrud-validation-summary').remove();

        if (!currBocrud.raiseEvent('onviewcancel', bokey))
            return;

        var gridObj = currBocrud.grid();
        if (currBocrud.config.useDialog) {
            f.dialog("close").empty().remove();
        } else {
            if (bokey != "" || currBocrud.config.showEmpty) {
                currBocrud.get_form(bokey, gridObj, "View");
            } else {
                f.dialog("close").empty().remove();
            }
        }
    }
    this.save = function (bokey, async, xml, showDetails, params, formId, onComplete) {
        var formid = formId || currBocrud.getFormId(bokey);
        var form = $('#' + formid);

        var proxyOnComplete = function (data) {

            if (onComplete)
                onComplete(data);

            currBocrud.raiseEvent('onsavecomplete', xml, bokey, formId, showDetails, params);
        };

        var result = submit(form, params, bokey, async, showDetails, null, proxyOnComplete);

        return result;
    }

    this._add_url_params = function (postData) {
        var ignore_list = [];
        ignore_list.push('id');
        ignore_list.push('xml');

        var exist_in_collection = function (key, collection) {
            var lkey = key.toLowerCase();
            if ($.isArray(collection)) {
                for (var i = 0; i < collection.length; ++i)
                    if (collection[i].toLowerCase() == lkey) {
                        return true;
                    }
                return false;
            } else {
                for (var k in collection)
                    if (k.toLowerCase() == lkey)
                        return true;
                return false;
            }
        }

        // for default value
        for (var k in currBocrud.urlParams) {
            if (k == 'p') {
                continue;
            }
            if (!exist_in_collection(k, postData) && !exist_in_collection(k, ignore_list)) {
                postData[k] = currBocrud.urlParams[k];
            }
        }

    }
    this.validate = function ($form, oncomplete) {

        currBocrud.raiseEvent('onvalidationstart', $form);

        var isValid = true;
        var invalidFormTitles = "";
        var validator = null;

        $form.find(":input, select, textarea")
            .not(":submit, :reset, :image, [disabled]")
            .each(function () {

                validator = $.data(this.form, "validator");
                if (!validator || !validator.settings) {
                    validator = {
                        settings: {}, valid: function () {
                            return true;
                        },
                        pendingRequest: 0
                    };
                    $.data(this.form, "validator", validator);
                    $(this.form).addClass('ignore-validation');
                }
            });

        $form.find('.bocrud-page-form:not(.ignore-validation)').each(function () {
            if ($(this).data('validator')) {
                if (!$(this).valid) {
                    validator = $(this).validate($(this).data('validator'));
                } else
                    validator = $(this);
                try {
                    if (!validator.valid()) {
                        isValid = false;
                        invalidFormTitles += $(this).attr('title');
                    }
                }
                catch (ex) {
                    $.console.error("bocrud-1767:" + ex);
                }
            }
        });

        if (rvLastResult)
            rvLastResult = {
            };

        if (!isValid || (!$form.is('.ignore-validation') && $form.valid && !$form.valid())) {
            var v = $form.data('validator')
            v.showErrors();
            currBocrud.raiseEvent('onvalidationfailed', $form);
            currBocrud.raiseEvent('onvalidationcomplete', $form, false);
            if ($.isFunction(oncomplete))
                oncomplete(currBocrud, false);
            return false;
        }

        if (validator.pendingRequest > 0) {

            var check = function () {

                var validator = $form.data("validator");
                if (validator.pendingRequest > 0) {
                    setTimeout(check, 510);
                    return;
                } else {
                    currBocrud.raiseEvent('onvalidationcomplete', $form, $form.valid());
                    if ($.isFunction(oncomplete))
                        oncomplete(currBocrud, $form.valid());
                }
            };
            check();
            return validator;
        }

        currBocrud.raiseEvent('onvalidationcomplete', $form, true);
        if ($.isFunction(oncomplete))
            oncomplete(currBocrud, true);
        return true;
    }

    var submit = function ($form, data, oldBoKey, async, showDetails, action, onsuccess, ignoreValidation, onerror, onpresubitsuccess) {

        var formObj = $form.data('formObj');
        var config = currBocrud.config;

        var params = $.extend(data || {}, {
            xml: data.xml && data.xml.length > 0 ? data.xml : config.xml,
            jsbid: config.id,
            id: oldBoKey ? oldBoKey : '',
            tempMode: config.tempMode,
            parentkey: $form.data('parentkey'),
            sel_cat: currBocrud.getSelCatNode()
        });

        var onSubmitComplete = function (result) {
            currBocrud.raiseEvent('onsubmitcomplete', formObj, params);
            return result;
        };

        if (currBocrud.config.isTree) {
            var s = currBocrud.grid().jqGrid('getGridParam', 'selrow');
            params.nodeid = s == null ? '' : s;
        }

        ignoreValidation = ignoreValidation || currBocrud.raiseEvent('onprevalidation', params) === false ? true : false;

        $.bocrud.submit_validating = true;

        if (!ignoreValidation) {

            var vr = currBocrud.validate($form, function (b, r) {
                $.bocrud.submit_validating = false;
                if (!r) {
                    return onSubmitComplete(false);
                } else {
                    submit($form, data, oldBoKey, async, showDetails, action, onsuccess, true, onerror, onpresubitsuccess);
                }
            });

            return;
        }

        $.bocrud.submit_validating = false;

        if (!currBocrud.raiseEvent('onpresubmit', formObj, params))
            return onSubmitComplete(false);

        var hasfile = $form.find(':file:first').length > 0;

        // این برای این هست که برخی مواقع شیش میزنه و با اینکه چک باکس تیک نخورده آن میفرسته به سرور
        $form.find(':checkbox').each(function () {
            var t = $(this);
            if (t.is(':checked'))
                t.attr('checked', 'checked')
            else
                t.removeAttr('checked');
        });

        var result = true;

        currBocrud._add_url_params(params);

        $form.ajaxSubmit({
            error_handler: onerror,
            showLoading: true,
            msg: $.bocrud.msg.saving + ' ' + currBocrud.config.title,
            data: params,
            url: currBocrud.config.urlPrefix + "/" + (action || "Save"),
            type: "POST",
            dataType: "text",
            semantic: true,
            global: true,
            async: async === false ? false : true,
            error: function (data) {

                var formObj = $form.data('formObj');
                currBocrud.raiseEvent('onsubmiterror', formObj, params, data);
                currBocrud.showError(data);
                if ($.isFunction(this.error_handler))
                    this.error_handler(data, formObj);
            },
            success: function (raw_response, response_code, xhr, form) {

                var formObj = $form.data('formObj');
                if (!currBocrud.raiseEvent('onsubmitresponsereceived', formObj, raw_response, response_code, xhr, form)) {
                    result = false;
                    return false;
                }

                if (raw_response == "OK") {
                    currBocrud.raiseEvent('onsubmitsuccess', formObj, "OK", showDetails);
                    return;
                }

                var response;
                try {
                    response = eval('(' + raw_response + ')')
                }
                catch (e) {
                    response = raw_response;
                }

                if (!$.isArray(response))
                    response = [response];

                var rlength = response.length;
                for (var i = 0; i < response.length; ++i) {
                    var data = response[i];

                    if (!data) continue;

                    if (data.error) {
                        currBocrud.raiseEvent('onsubmiterror', formObj, data);
                        currBocrud.showError(data);
                        result = false;
                        if ($.isFunction(this.error_handler))
                            this.error_handler(data, formObj);
                        return false;
                    }

                    var strSaveResult = '';
                    if (data.length > 0) {
                        if (data.indexOf("<pre") == 0) {
                            var pre_end_index = data.indexOf('>');
                            data = data.substring(pre_end_index + 1, data.length - 6);
                        }
                        var sixchar = data.substring(0, 6);
                        if (sixchar === "base64")
                            strSaveResult = Base64.decode(data.substring(6));
                        else
                            if (sixchar == "<pre>{")
                                strSaveResult = data.substring(5, data.length - 6);
                            else
                                strSaveResult = data;
                    }

                    var saveResult = null;
                    try {
                        saveResult = strSaveResult != '' ? eval('(' + strSaveResult + ')') : {
                        };
                    }
                    catch (e) {
                        currBocrud.raiseEvent('onsubmiterror', formObj, strSaveResult);
                        $('<div/>').html(strSaveResult).dialog();
                        return false;
                    }

                    if (!$.isEmptyObject(data) && typeof (data.Success) === "boolean") {
                        if (!data.Success)
                            saveResult.error = true;
                    }

                    if (saveResult.error || saveResult.Success === false) {
                        currBocrud.raiseEvent('onsubmiterror', formObj, saveResult);
                        currBocrud.showError(saveResult);
                        result = false;
                        return false;
                    }

                    if (!formObj) // for optimized bundle issue
                        formObj = form.data('formObj');

                    if (!formObj.boKey)
                        formObj.boKey = saveResult.row ? saveResult.row[0].id : saveResult.boKey;

                    result = true;

                    if ($.isFunction(onpresubitsuccess))
                        if (onpresubitsuccess(data, saveResult) == false)
                            return;

                    if (!currBocrud.raiseEvent('onsubmitsuccess', formObj, saveResult, showDetails)) {
                        result = false;
                        return false;
                    }

                    if (!currBocrud.config.hideGrid) {
                        var gridObj = currBocrud.grid();
                        if ((!currBocrud.config.isTree || currBocrud.config.tempMode) &&
                            saveResult && saveResult.row) {
                            saveResult.row[0].cell['_id_'] = saveResult.row[0].id;

                            var remc = currBocrud.config.remc;
                            if (remc && remc.length > 0) {
                                try {
                                    var row = saveResult.row[0];
                                    var c = eval(remc);
                                    if (c) {
                                        if (!saveResult.row[0].newRow)
                                            gridObj.jqGrid("delRowData", oldBoKey);
                                        return;
                                    }
                                }
                                catch (e) {
                                    $.console.error("bocrud-1030:" + e);
                                }
                            }

                            if (saveResult.row[0].newRow) {
                                gridObj.jqGrid('addRowData', saveResult.row[0].id, saveResult.row[0].cell, 'first');
                                if (currBocrud.config.onRowCreated)
                                    currBocrud.config.onRowCreated(currBocrud, saveResult);
                            }
                            else {
                                gridObj.jqGrid("delRowData", formObj.boKey);
                                gridObj.jqGrid('addRowData', saveResult.row[0].id, saveResult.row[0].cell, "first");
                                if (currBocrud.config.onRowUpdated)
                                    currBocrud.config.onRowUpdated(currBocrud, saveResult);
                            }
                            gridObj.jqGrid('setSelection', saveResult.row[0].id, true);

                            decorate_grid_rows();

                            currBocrud.runJs("oncontrolplaced", gridObj);
                            currBocrud.runJs("oneverycontrolplaced", gridObj);

                            currBocrud._update_validation_field();
                        }
                        else {
                            gridObj.trigger("reloadGrid");
                            gridObj.jqGrid('resetSelection');
                        }
                    }

                    if (showDetails != false) {
                        var key = saveResult.row ? saveResult.row[0].id : saveResult.boKey;

                        if (!saveResult.row) {

                            var old_form_container = currBocrud.raiseEvent('onpreshowcontent', saveResult);
                            if (!old_form_container)
                                return;

                            currBocrud.show_form(saveResult, 'View', saveResult.xml);
                            $.bocrud.reg_form(saveResult);

                        }
                        else
                            currBocrud.get_form(key, 'View', params.xml);
                    }

                    if ($.isFunction(onsuccess))
                        onsuccess(data, saveResult);

                }// for
                return result;

            }

        });
        return onSubmitComplete(result);
    }

    this.do_submit = submit;

    this.reloadGridData = function () {
        var gridObj = currBocrud.grid();
        gridObj.trigger("reloadGrid");
        gridObj.jqGrid('resetSelection');
    }


    this.renderGroup = function (g) {
        var r = "";
        if (g.properties.length > 0) {
            switch (g.template) {
                case "Columnar":
                    r = currBocrud.columnarTemplate(g);
                    break;
                case "Tabluar":
                    throw "not support Tabluar template";
                    break;
                case "Justified":
                    r = currBocrud.justifiedTemplate(g);
                    break;
                case "Consecutive":
                    r = currBocrud.consecutiveTemplate(g);
                    break;
            }
        }
        if (g.subgroups && g.subgroups.length > 0) {
            for (var i = 0; i < g.subgroups.length; ++i) {
                if ($.bocrud.groupHasProperty(g.subgroups[i])) {
                    r += "<fieldset name='" + g.subgroups[i].name + "' style='" +
                        ((g.subgroups[i].title && g.subgroups[i].title.length > 0) ? '' : 'border-style:none')
                        + "' class='bocrud-subgroup'>";
                    if (g.subgroups[i].title && g.subgroups[i].title.length > 0)
                        r += '<legend>' + g.subgroups[i].title + '</legend>';
                    r += currBocrud.renderGroup(g.subgroups[i]);
                    r += "</fieldset>";
                }
            }
        }
        return r;
    }

    this.columnarTemplate = function (group) {
        var cc = group.columnCount ? group.columnCount : 2;
        var pic = Math.ceil(group.properties.length / cc);
        var tdContent = [], label, help, hasBreak = false;

        for (var i = 0; i < group.properties.length; ++i)
            if (group.properties[i].isbreak) {
                hasBreak = true; break;
            }

        for (var j = 0, i = 0; j < group.properties.length; ++j, ++i) {
            var property = group.properties[j];
            if (property.hidden == true)
                continue;
            var pbody = [];
            pbody.push("<a name='" + property.id + "-anchor'></a>");
            pbody.push("<div rel='property' style='" + property.outerStyle + "' name='" + property.name + "' uidPrefix='" + property.uidPrefix + "' id='" + property.id + "-holder' class='bocrud-control '>");
            label = property.caption && property.caption.length > 0 ?
                "<label for='" + property.id + "' class='bocrud-control-caption'>" + property.caption + " : </label>" : "";
            help = "";
            if (property.help != null && property.help.length > 0) {
                help = "<span class='ui-icon ui-icon-info bocrud-control-help-icon'></span>";
                help += '<span rel="propertyHelp" class="bocrud-control-help">' + property.help + '</span>';
            }
            var pContent = "<span class='bocrud-control-content' rel='propertyContent' id='" + property.id + "pc'>";
            pContent += property.property + "</span>";
            if (property.caption && property.caption.length)
                pbody.push(label + "<br/>" + pContent + help);
            else
                pbody.push(pContent + help);
            pbody.push("</div>");

            if (hasBreak) {
                if (tdContent.length == 0)
                    tdContent.push("");
                if (property.isbreak)
                    tdContent.push("");
                else
                    tdContent[tdContent.length - 1] += pbody.join("");
            } else {
                var tdcIndex = Math.floor(i / pic);
                //++i;
                if (tdContent.length < (tdcIndex + 1))
                    tdContent.push("");
                if (tdContent[tdcIndex])
                    tdContent[tdcIndex] += pbody.join("");
                else
                    tdContent[tdcIndex] = pbody.join("");
            }
        }
        var tabContent = "<table width='100%'><tr>";
        for (var j = 0; j < tdContent.length; ++j)
            tabContent += "<td valign='top'>" + tdContent[j] + "</td>";
        tabContent += "</tr></table>";

        for (var j = 0; j < group.properties.length; ++j) {
            var property = group.properties[j];
            if (property.hidden != true)
                continue;
            pbody = "<div rel='property' name='" + property.name + "' id='" + property.id + "-holder' class='bocrud-control bocrud-hidden-property'>";
            var pContent = "<span class='bocrud-control-content' rel='propertyContent' id='" + property.id + "pc'>";
            pContent += property.property + "</span>";
            pbody += pContent;
            pbody += "</div>";
            tabContent += pbody;
        }

        return tabContent;
    }

    this.consecutiveTemplate = function (group) {
        var pbody = "", label = "", help = "", content = "";
        for (var j = 0; j < group.properties.length; ++j) {
            var property = group.properties[j];
            pbody = "<a name='" + property.id + "-anchor'></a>";
            pbody += "<div rel='property' name='" + property.name + "' uidPrefix='" + property.uidPrefix + "' id='" + property.id + "-holder' class='bocrud-control " + (property.hidden ? "bocrud-dfvh" : "") + "'>";
            label = property.caption && property.caption.length > 0 ?
                "<label for='" + property.id + "' class='bocrud-control-caption'>" + property.caption + " : </label>" : "";
            help = "";
            if (property.help != null && property.help.length > 0) {
                help = "<span class='ui-icon ui-icon-info bocrud-control-help-icon'></span>";
                help += '<span rel="propertyHelp" class="bocrud-control-help">' + property.help + '</span>';
            }
            var pContent = "<span class='bocrud-control-content' rel='propertyContent' id='" + property.id + "pc'>";
            pContent += property.property + "</span>";
            pbody += label + "<br/>" + pContent + help + "<br/>";
            pbody += "</div>";
            content += "<p>" + pbody + "</p>";
        }
        return content;
    }

    this.justifiedTemplate = function (group) {
        var pbody = "", content = "", label, help;
        for (var j = 0; j < group.properties.length; ++j) {
            var property = group.properties[j];
            pbody = "<a name='" + property.id + "-anchor'></a>";
            pbody += "<span rel='property' name='" + property.name + "' uidPrefix='" + property.uidPrefix + "' id='" + property.id + "-holder' class='bocrud-control " + (property.hidden ? "bocrud-dfvh" : "") + "' >";
            label = property.caption.length > 0 ?
                "<label for='" + property.id + "' class='bocrud-control-caption'>" + property.caption + " : </label>" : "";
            help = "";
            if (property.help != null && property.help.length > 0)
                help = '<span rel="propertyHelp" class="bocrud-control-help">' + property.help + '</span>';
            var pContent = "<span class='bocrud-control-content' rel='propertyContent' id='" + property.id + "pc'>";
            pContent += property.property + "</span>";
            pbody += label + pContent + help;
            pbody += "</span>";
            content += pbody;
        }
        return content;
    }


    this.cusfilt = "";
    this.setFilter = function (filter, noReload) {

        var gridObj = currBocrud.grid();

        if (gridObj.length == 0) {
            currBocrud.cusfilt = filter;
            return;
        }

        var postData = gridObj.jqGrid('getGridParam', "postData");

        var serializedFilter = JSON.serialize(filter);
        postData.cusfilt = serializedFilter;
        postData.search = "true";

        gridObj.jqGrid('setGridParam', "postData", postData);
        if (!noReload)
            gridObj.trigger("reloadGrid");
        gridObj.jqGrid("resetSelection");

        //$(currBocrud._dependToRowJqueryId).hide();
        currBocrud.initPasteBtn();

    }

    this._arrange = function (dialogId) {
        //        var dialogObj = $("#" + dialogId);
        //        var width = dialogObj.width();
        //        width = width == 0 ? 450 : width;
        //        dialogObj.find("table[rel=grid]").jqGrid('setGridWidth', width - 50);
    }



    this.get_form = function (bokey, mode, xml, oncomplete, buttons, data) {
        if (!bokey || bokey == null)
            bokey = "";

        if (bokey == "" && !currBocrud.checkActiveCat()) {
            currBocrud.raiseEvent("onaddnewfailed", currBocrud, postData);
            alert($.bocrud.msg.select_before_new.format(currBocrud.getActiveCatCaption()));
            return;
        }

        // چون ممکنه که مقادیر پیشفرض موجود در آدرس صفحه مربوط به این بوکراد نباشند
        var finalXml = xml ? xml : config.xml;
        var urlXml = currBocrud.urlParams["xml"] ? currBocrud.urlParams["xml"] : '';
        var dvs = '';

        var nodeid = currBocrud.grid().jqGrid('getGridParam', 'selrow');

        var postData = $.extend({}, data);

        postData['nodeid'] = nodeid;
        postData['id'] = bokey;
        postData['mode'] = mode;
        postData['xml'] = finalXml;
        postData['dvs'] = dvs;
        postData['sel_cat'] = currBocrud.getSelCatNode();

        if (currBocrud.raiseEvent("onpregetcontent", currBocrud, postData) == false)
            return;

        currBocrud._add_url_params(postData);

        var async = false;
        if ($.isFunction(oncomplete))
            async = true;

        var formObj;
        currBocrud.getData("GetDetails", postData, {
            showLoading: true,
            msg: $.bocrud.msg.load_form + ' ' + currBocrud.config.title,
            async: async,
            semantic: true,
            global: true,
            success: function (serverResponse) {

                formObj = eval('(' + serverResponse + ')');

                if (formObj.error) {
                    currBocrud.raiseEvent("onshowcontenterror", formObj);

                    if ($.isFunction(oncomplete))
                        oncomplete(formObj);
                    return;
                }
                var old_form_container = currBocrud.raiseEvent('onpreshowcontent', formObj);
                if (!old_form_container) {
                    if ($.isFunction(oncomplete))
                        oncomplete(formObj);
                    return;
                }

                if (mode == "GridView" || mode == "GridDataEntry") {
                    var grid = currBocrud.grid();

                    if (mode == "GridDataEntry") {
                        var old_inline_form = grid.find('.bocrud-page-form[uid="' + formObj.uid + '"]:first');
                        if (old_inline_form.length != 0) {
                            formObj.row[0].newRow = false;
                            formObj.row[0].id = old_inline_form.attr('id');
                        }
                    }

                    var old_row_data = null;

                    if (formObj.row[0].id)
                        old_row_data = grid.jqGrid('getRowData', formObj.row[0].id);

                    if (formObj.row[0].newRow)
                        grid.jqGrid("addRowData", formObj.row[0].id, formObj.row[0].cell, 'first');
                    else
                        grid.jqGrid("setRowData", formObj.row[0].id, formObj.row[0].cell, '', {}, true);

                    var tr;
                    var id = formObj.row[0].id;
                    if (id && id.length > 0)
                        tr = grid.children('tbody').children('tr#' + id);
                    else
                        tr = grid.find('tr.jqgfirstrow').next();


                    if (mode == "GridView") {
                        $.bocrud.decorateUi(tr);

                        currBocrud.raiseServerEvents("OnShowContent", {
                        });

                        currBocrud.raiseEvent('oncontrolplaced', formObj, mode, tr, formObj.xml);
                        currBocrud.raiseEvent('oneverycontrolplaced', formObj, mode, tr, formObj.xml);
                        currBocrud.raiseEvent('onshowcontentcomplete', formObj, mode, tr, formObj.xml);

                        if ($.isFunction(oncomplete))
                            oncomplete(formObj);
                        return;
                    }

                    tr.addClass('bocrud-page-form');
                    formObj.jsbid = currBocrud.config.id;
                    formObj.mode = "GridDataEntry";
                    formObj.xml = currBocrud.config.xml;
                    formObj.boKey = id;
                    tr.attr('uid', formObj.uid);
                    tr.data('formObj', formObj);
                    tr.data('old_row_data', old_row_data);
                }

                currBocrud.raiseEvent('onunload', old_form_container, mode);
                if (old_form_container && old_form_container.find)
                    old_form_container.find('[catch-display-events]').trigger('onhide', [{
                    }]);

                currBocrud.show_form(formObj, mode, formObj.xml, null, buttons);
                $.bocrud.reg_form(formObj);

                if ($.isFunction(oncomplete))
                    oncomplete(formObj);
                return;
            }
        });
        return formObj;
    }
    this.show_form = function (formObj, mode, xml, placeHolder, buttons) {
        mode = mode || formObj.mode;
        var defaultButtons = buttons || {
        };

        if (mode === "DataEntry") {

            var data = {
            };
            data.xml = xml || (formObj.xml || config.xml);
            data.jsbid = config.id;
            data.id = formObj.boKey;
            data.tempMode = config.tempMode;
            data.mainXml = config.xml;

            if (!defaultButtons[$.jgrid.edit.bSubmit])
                defaultButtons[$.jgrid.edit.bSubmit] = function (preventLoadView, xdata, onsuccess, ignoreValidation) {
                    //$form, data, oldBoKey, async, showDetails, action, onsuccess, ignoreValidation
                    return submit($('#' + formObj.id), $.extend(data, xdata), formObj.boKey, true, !preventLoadView, null, onsuccess, ignoreValidation);
                };
        }

        if (!defaultButtons[$.jgrid.edit.bCancel] && !defaultButtons[$.jgrid.edit.bClose])
            defaultButtons[$.jgrid.edit.bCancel] = function (preventLoadView) {
                if (!currBocrud.raiseEvent('onviewcancel', formObj))
                    return;
                if (preventLoadView) return;

                if (formObj.mode == 'DataEntry') {

                    if (preventLoadView !== true)
                        if (currBocrud.config.showEmpty ||
                            (formObj.boKey != '' && formObj.boKey.length > 0))
                            currBocrud.get_form(formObj.boKey, 'View', formObj.xml);
                }

            }

        if (mode === "GridDataEntry" && currBocrud.config.standalone) {

            var tb = currBocrud.$('.inline-edit-toolbar:first', true);

            if (tb.find('button').length == 0) {
                $.bocrud.addButton(tb, {
                    caption: $.jgrid.edit.bSubmit,
                    buttonicon: 'icon-save bigger-160',
                    onClickButton: function () {
                        currBocrud.save_inlines();
                    },
                    showText: true,
                    classes: 'bocrud-tb-submit btn btn-app btn-success btn-xs radius-4'
                });
                tb.show('slow');
            } else {
                tb.show('slow');
            }
        }

        currBocrud.raiseServerEvents("OnShowContent", {
        });

        currBocrud.raiseEvent('onshowcontent', formObj, (!buttons || $.isEmptyObject(buttons)) ? defaultButtons : buttons, mode, placeHolder);

    }

    this.save_inlines = function () {

        var cc = currBocrud.config;

        var params = {
        };

        params['xml'] = cc.xml;
        params['jsbid'] = cc.id;
        params['tempMode'] = cc.tempMode;

        var gif = currBocrud.$('#gif-' + currBocrud.config.id);

        var ignoreValidation = currBocrud.raiseEvent('onprevalidation', params) === false ? true : false;

        if (!ignoreValidation)
            if (!gif.valid()) return;

        var formObjects = [];
        var erids = [];
        gif.find('tr.bocrud-page-form').each(function () {
            var formObj = $(this).data('formObj');
            formObjects.push(formObj);
            if (!currBocrud.raiseEvent('onpresubmit', formObj, params))
                return;
            erids.push($(this).attr('id'));
        });
        params.erids = erids.join(',');

        var gridObj = currBocrud.grid();

        currBocrud._add_url_params(params);

        gif.ajaxSubmit({
            showLoading: true,
            msg: $.bocrud.msg.saving + ' ' + currBocrud.config.title,
            data: params,
            url: currBocrud.config.urlPrefix + "/SaveInline",
            type: "POST",
            dataType: "text",
            semantic: true,
            global: true,
            async: true,
            error: function (data) {
                currBocrud.raiseEvent('onsubmiterror', data);
                currBocrud.showError(data);
            },
            success: function (response) {

                var response_list = eval('(' + response + ')');

                if (response_list.error) {
                    currBocrud.raiseEvent('onsubmiterror', response_list);
                    currBocrud.showError(response_list);
                    return;
                }

                for (var i = 0; i < response_list.length; ++i) {
                    var sri = response_list[i]; //save result item

                    if (!sri.success) continue;

                    var formObj = null;
                    for (var k = 0; k < formObjects.length; ++k)
                        if (formObjects[k].row && formObjects[k].row[0].id) {
                            if (formObjects[k].row[0].id == sri.id) {
                                formObj = formObjects[k];
                                break;
                            }
                        } else {
                            if (formObjects[k].boKey == sri.id) {
                                formObj = formObjects[k];
                                break;
                            }
                        }

                    var data = sri.content;

                    var strSaveResult = '';
                    if (data.length > 0) {
                        if (data.indexOf("<pre") == 0) {
                            var pre_end_index = data.indexOf('>');
                            data = data.substring(pre_end_index + 1, data.length - 6);
                        }
                        var sixchar = data.substring(0, 6);
                        if (sixchar === "base64")
                            strSaveResult = Base64.decode(data.substring(6));
                        else
                            if (sixchar == "<pre>{")
                                strSaveResult = data.substring(5, data.length - 6);
                            else
                                strSaveResult = data;
                    }

                    var saveResult = null;
                    try {
                        saveResult = strSaveResult != '' ? eval('(' + strSaveResult + ')') : {
                        };
                    }
                    catch (e) {
                        sri.success = false;
                        sri.content = strSaveResult;
                        continue;
                    }
                    if (saveResult.error) {
                        sri.success = false;
                        sri.content = saveResult;
                        continue;
                    }


                    saveResult.row[0].cell['_id_'] = saveResult.row[0].id;

                    if (sri.wasNew || !sri.id) {

                        if (!gridObj.jqGrid("delRowData", sri.wasNew ? "" : sri.id))
                            gridObj.find('tr.bocrud-page-form').each(function () {
                                if (($(this).attr('id') == undefined && "" == sri.id) ||
                                    $(this).attr('id') == sri.id)
                                    $(this).remove();
                            });

                        gridObj.jqGrid('addRowData', saveResult.row[0].id, saveResult.row[0].cell, "first");

                    } else {
                        gridObj.jqGrid("setRowData", saveResult.row[0].id, saveResult.row[0].cell, '', {}, true);

                        if (!currBocrud.config.standalone || currBocrud.config.mode != 'GridDataEntry') {
                            if (!sri.id)
                                gif.find('tr').removeClass('bocrud-page-form');
                            else
                                gif.find('tr[id="' + sri.id + '"]').removeClass('bocrud-page-form');
                        }
                    }

                    gridObj.jqGrid('setSelection', saveResult.row[0].id, true);
                    if (currBocrud.config.onRowUpdated)
                        currBocrud.config.onRowUpdated(currBocrud, saveResult);

                    $.bocrud.reg_form(formObj);

                    currBocrud._update_validation_field();
                }

                var error_list = [];
                for (var i = 0; i < response_list.length; ++i) {
                    var sri = response_list[i]; //save result item
                    if (!sri.success) {
                        var errobj = null;
                        try {
                            errobj = eval('(' + sri.content + ')');
                        }
                        catch (e) {
                            errobj = null;
                        }

                        if (errobj) {
                            currBocrud.showError(errobj);
                            continue;
                        }
                        error_list.push("<li>");
                        error_list.push(sri.content.message || sri.content);
                        error_list.push("</li>");
                    }
                }
                if (error_list.length > 0) {
                    error_list.splice(0, 0, "<ul>");
                    error_list.push('</ul>');
                    $('<div/>').html(error_list.join('')).dialog();
                    return;
                }

                if (gif.find('tr.bocrud-page-form').length == 0) {
                    currBocrud.$('.inline-edit-toolbar:first', true).hide('slow');
                }

                currBocrud.raiseEvent('oninlinesubmitsuccess');
            }
        });
    }

    this.addNew = function (selId, gridObj) {

        if (!currBocrud.raiseEvent('onpreaddnew', selId, gridObj))
            return;

        var cmode = currBocrud.config.mode == "GridDataEntry" ? "GridDataEntry" : "DataEntry";
        currBocrud.get_form('', cmode, currBocrud.config.xml, function (formObj) {
            if (!formObj) return;
            if (!currBocrud.raiseEvent('onaddnew', formObj))
                return;
            if (cmode == "GridDataEntry")
                currBocrud._update_validation_field();
        });
    }

    this.do_view = function (selId, gridObj) {

        if (!currBocrud.raiseEvent('onpreview', selId, gridObj))
            return;

        var formObj = currBocrud.get_form(selId, "View", currBocrud.config.xml, function (formObj) {

            if (!currBocrud.raiseEvent('onview', formObj))
                return;
        });

    }

    this.do_medit = function (selected_rows, gridObj) {

        if (!currBocrud.raiseEvent('onpremedit', selected_rows, gridObj))
            return;

        if (!selected_rows || selected_rows.length == 0) {
            alert($.bocrud.msg.none_item_selected_for_edit);
            return;
        }


        var cmode = currBocrud.config.mode == "GridDataEntry" ? "GridDataEntry" : "DataEntry";
        var formObj = currBocrud.get_form(selected_rows, cmode, currBocrud.config.xml, function (formObj) {

            var msg = [];
            msg.push('<div>');
            msg.push($.bocrud.msg.you_are_editing);
            msg.push(':</div>');
            msg.push('<table class="table table-hover table-condensed table-responsive table-bordered ">');
            msg.push('<tr>');
            var colModel = currBocrud.grid().jqGrid('getGridParam', 'colModel');
            msg.push('<td>');
            msg.push('</td>');
            for (var i = 0; i < colModel.length; ++i) {
                if (i > 11) break;
                if (colModel[i].label) {
                    msg.push('<td>');
                    msg.push(colModel[i].label);
                    msg.push('</td>');
                }
            }
            msg.push('</tr>');

            for (var i = 0; i < selected_rows.length; ++i) {
                var data = currBocrud.grid().jqGrid('getRowData', selected_rows[i]);
                msg.push('<tr>');
                msg.push('<td>');
                msg.push(i);
                msg.push('</td>');
                var j = 0;
                for (var col in data) {
                    if (j++ > 12) break;
                    msg.push('<td>');
                    msg.push(data[col]);
                    msg.push('</td>');
                }
                msg.push("</tr>");
            }
            msg.push('</table>');
            $('#' + formObj.id).prepend(msg.join(''));

            if (!currBocrud.raiseEvent('onmedit', formObj))
                return;
        });
    }

    this.do_edit = function (selId, gridObj) {

        if (!currBocrud.raiseEvent('onpreedit', selId, gridObj))
            return;

        if (!selId || selId.length == 0) {
            alert($.bocrud.msg.none_item_selected_for_edit);
            return;
        }
        if ($.isArray(selId) && selId.length == 1)
            selId = selId[0];

        var cmode = currBocrud.config.mode == "GridDataEntry" ? "GridDataEntry" : "DataEntry";
        var formObj = currBocrud.get_form(selId, cmode, currBocrud.config.xml, function (formObj) {

            if (!currBocrud.raiseEvent('onedit', formObj))
                return;
        });
    }

    this.do_del = function (ids, gridObj, complete, silent) {

        if (ids == null || !ids || ids.length == 0)
            alert($.bocrud.msg.none_item_selected_for_delete);

        if (currBocrud.config.tempMode || silent || confirm($.bocrud.msg.gridDel)) {

            if (!$.isArray(ids))
                ids = [ids];

            if (ids.length > 0) {
                currBocrud.del(ids, function () {

                    currBocrud.refresh_toolbar();

                    if ($.isFunction(complete))
                        complete(ids);

                    if (currBocrud.config.mode != "GridDataEntry" && gridObj)
                        gridObj.trigger("reloadGrid", [{
                            page: 1
                        }]);

                    if (!currBocrud.raiseEvent('ondelete', ids))
                        return;
                });

            }
        }
    }



    this.del = function (ids, fn) {

        if (!currBocrud.raiseEvent('ondelete', ids))
            return;

        currBocrud.getData("Delete", {
            ids: ids.toString()
        }, {
            msg: $.bocrud.msg.deleting + ' ' + currBocrud.config.title,
            success: function (data) {
                var resultObj = JSON.parse(data);
                if (resultObj.error) {
                    currBocrud.raiseEvent('ondeleteerror', ids)
                    currBocrud.showError(resultObj);
                    return;
                }

                if (currBocrud.raiseEvent('ondeletesuccess', ids) === false)
                    return;

                var gridObj = currBocrud.grid();
                if (!currBocrud.config.isTree || currBocrud.config.tempMode)
                    for (var i = 0; i < resultObj.length; ++i) {
                        gridObj.jqGrid("delRowData", resultObj[i]);
                    }
                else {
                    if (gridObj.jqGrid("getGridParam", "datatype") != "jsonstring")
                        currBocrud.reloadGridData();
                }

                if (currBocrud.config.grouping && currBocrud.config.mode != 'GridDataEntry') {
                    gridObj.jqGrid('groupingGroupBy', currBocrud.config.groupBy);
                    gridObj.jqGrid('resetSelection');
                }

                if ($.isFunction(fn))
                    fn.apply(this, resultObj);

                if (!currBocrud.raiseEvent('ondeletedone', ids))
                    return;

            }
        });
    }

    this._grid = null;
    this.grid = function () {
        if (currBocrud._grid == null || currBocrud._grid.length == 0)
            currBocrud._grid = $("#" + currBocrud.config.id + "-Grid");
        return currBocrud._grid;
    }

    this._pane = null;
    this.pane = function () {
        if (currBocrud._pane == null)
            currBocrud._pane = $("#b-" + currBocrud.config.id);
        return currBocrud._pane;
    }

    this.addVfItem = false;
    this.vfField = $("#" + currBocrud.config.id + "vf");

    this.load = function (fn) {

        var cfg = currBocrud.config;
        var c = currBocrud;

        if (!currBocrud.raiseEvent('onload', cfg))
            return;

        if (cfg.validator) {
            c.addEvent("onsubmit", function (data) {
                currBocrud._update_validation_field();
                return true;
            });
            c.addEvent("onsubmiterror", function (bocrud, data) {
                currBocrud._update_validation_field();
                return true;
            });
            c.addEvent("ondeletedone", function (ids) {
                currBocrud._update_validation_field();
                return true;
            });
        } // if (cfg.validator) {

        if (!cfg.hideGrid) {

            var pagingMode = currBocrud.grid().jqGrid('getGridParam', 'pagingMode');
            if (pagingMode == 'NextBack') {
                var pginput = $(currBocrud.grid().jqGrid('getGridParam', 'pager'));
                pginput.find('.ui-icon-seek-first,.ui-icon-seek-end').each(function () {
                    $(this).parent().remove();
                });
            }

            c.addEvent('ongridload', function () {

                if (fn)
                    fn();
                c.initCats();
                c.initCatEvents();
                c.refresh_toolbar();

                if (cfg.search) {

                    var onInit = null;
                    if (cfg.search.onInit)
                        onInit = cfg.search.onInit;
                    else
                        if (cfg.style == "Report") {
                            onInit = function () {
                                this.bocrudSearch('show');
                                this.bocrudSearch('add');
                                this.bocrudSearch('add');
                                this.bocrudSearch('add');
                            };
                        }

                    c.grid().bocrudSearch({
                        bind: '#' + cfg.id + 'SearchBtn',
                        bocrud: c,
                        title: cfg.search.title && cfg.search.title.length > 0 ?
                            cfg.search.title : $.bocrud.msg.search_on + ' ' + c.grid().jqGrid('getGridParam', 'caption') + ' ...',
                        canCreate: cfg.search.canCreate,
                        canRemove: cfg.search.canRemove,
                        dialog: cfg.search.dialog,
                        usefilter: cfg.search.usefilter,
                        searchOnPrevRslt: cfg.search.searchOnPrevRslt,
                        groupOperation: cfg.search.groupOperation,
                        print: false,
                        onAfterReset: cfg.search.onAfterReset,
                        onPreReset: cfg.search.onPreReset,
                        onAdd: cfg.search.onAdd,
                        onPreSearch: cfg.search.onPreSearch,
                        onAfterSearch: cfg.search.onAfterSearch,
                        onInit: onInit,
                        adMgm: cfg.search.adMgm,
                        fix: false,
                        userFilters: cfg.userFilters
                    });

                }

            }, 'grid');

            //c.$('ul.bocrud-subtype-menu', true).dropdown();

            c.$('ul.bocrud-subtype-menu', true)
                .find('a').click(function () {
                    var t = $(this);
                    t.closest('ul').data('active', t);
                    var xml = t.attr('xml');
                    var enable = t.attr('enable');
                    if (enable == 'true') {
                        var gridObj = $("#" + config.id + "Grid");
                        c.get_form(null, "DataEntry", xml);
                    }
                });

            if (!cfg.autoLoadData)
                currBocrud.onGridLoad();

        } // if (!cfg.hideGrid) {

        $('.bocrud-thumbnails').hide();

        if ($.isFunction(cfg.onClientLoad))
            cfg.onClientLoad.apply(this);

        if (c.$("div#" + cfg.id + "pfs").length > 0)
            c.$("#" + cfg.id + "PrintBtn", false)
                .append('<span class="icon-caret-down icon-on-right"></span>');

        setTimeout(function () { c.refresh_toolbar(); }, 1000);

        if (cfg.volatileToolbar) {
            currBocrud.addEvent('ondeletesuccess', currBocrud.update_toolbar, 'volatile-toolbar');
            currBocrud.addEvent('onsubmitsuccess', currBocrud.update_toolbar, 'volatile-toolbar');
        }

        if (!currBocrud.raiseEvent('onloadcomplete', null))
            return;
    }

    this.update_toolbar = function () {

        currBocrud.getData("UpdateToolbar", {}, {
            msg: $.bocrud.msg.update_toolbar,
            datatype: 'json',
            success: function (data) {
                var resobj = eval('(' + data + ')');
                var toolobj = currBocrud.$('.bocrud-toolbar-container[jsbid=' + currBocrud.config.id + ']');
                toolobj.html('');
                toolobj.html(resobj.html);
                for (var i = 0; i < resobj.grid_toolbar.length; ++i)
                    currBocrud.grid().toolbarButtonAdd('#toolbar_' + currBocrud.config.id + '-Grid', resobj.grid_toolbar[i]);
            }
        });

    };

    this.do_find = function (pn) {

        if (!pn)
            pn = 1;

        var grid = currBocrud.grid();

        if (!currBocrud.config.autoLoadData || grid.jqGrid('getGridParam', 'isDynamic')) {
            grid.jqGrid('setGridParam', {
                datatype: 'json'
            });
            currBocrud.config.autoLoadData = true;
        }

        var search_form_id = "search-" + currBocrud.config.id;
        var $form = $('#' + search_form_id);
        if (!currBocrud.validate($form))
            return;

        var g_p = grid.jqGrid('getGridParam');
        var orgPostData = g_p.postData;
        var postData = {
            _search: true,
            fobj: "1",
            jsbid: orgPostData.jsbid,
            page: orgPostData.page,
            rows: orgPostData.rows,
            sidx: orgPostData.sndx,
            sord: orgPostData.sord,
            tempMode: orgPostData.tempMode,
            xml: orgPostData.xml
        };

        postData['fobj'] = '1';
        var formData = $('#' + search_form_id).formToArray();

        for (var i = 0; i < formData.length; ++i)
            postData[formData[i].name] = null;

        for (var i = 0; i < formData.length; ++i) {
            if (postData[formData[i].name] != null)
                postData[formData[i].name] += "," + formData[i].value;
            else
                postData[formData[i].name] = formData[i].value;
        }

        if (pn && grid.jqGrid('getGridParam', 'lastpage') < pn)
            grid.jqGrid('setGridParam', {
                lastpage: pn
            });

        currBocrud.raiseEvent('onpresearch', g_p.postData, postData);

        g_p.postData = postData;

        grid.jqGrid("setGridParam", {
            search: true, mtype: 'POST'
        });

        grid.trigger("reloadGrid", {
            page: pn
        });

        currBocrud.raiseEvent('onfindbtnclicked', null);
    }

    this.do_reset_search = function (ph) {

        if (!currBocrud.raiseEvent('onresetsearchclick', null))
            return;

        //search-ReceiptCashDoc-container
        var place_holder = ph || "search-" + currBocrud.config.id + '-container';

        $('#' + place_holder).html('');

        var dform = eval('(' + $('#search-formObj-' + currBocrud.config.id + ':first')
            .val().replace(RegExp('textareaa', 'gi'), 'textarea') + ')');

        currBocrud.searchStyler(dform, dform.xml, place_holder);

        $.bocrud.toast(currBocrud.config.id, $.bocrud.msg.search_reseted, 'hint');

        //$("html,body").animate({
        //     scrollTop: currBocrud.grid().offset().top
        //});

        //var search_form_id = "search-" + currBocrud.config.id;
        //var $form = $('#' + search_form_id);
        //if (!currBocrud.validate($form))
        //    return;

        //currBocrud.do_refresh(1, true, true);
    }

    // called in layout pages
    this.searchStyler = function (formObj, xml, place_holder) {

        var search_form_id = "search-" + currBocrud.config.id;

        var buttons = {
        };


        buttons[$.jgrid.search.Find] = function (e, pn) {
            currBocrud.do_find(pn);
        }

        buttons[$.jgrid.search.Reset] = function (e, ph) {
            currBocrud.do_reset_search(ph || place_holder);
        }

        var bh = $('#search-btn-' + currBocrud.config.id);

        if (bh.find('button').length == 0) {
            var searchBtn = "<button type='button' id='" + currBocrud.config.id + "-searchBtn'  >" + $.jgrid.search.Find + "</button>";
            var resetBtn = "<button type='button' id='" + currBocrud.config.id + "-resetBtn'  >" + $.jgrid.search.Reset + "</button>";
            var searchBtnProps = {
                icons: {
                    primary: 'ui-icon-search'
                }
            };
            var resetBtnProps = {
                icons: {
                    primary: 'ui-icon-cancel'
                }
            };

            $(searchBtn)
                .addClass("btn btn-pink")
                .button(searchBtnProps)
                .prepend('<i class="icon-search"></i>')
                .click(buttons[$.jgrid.search.Find])
                .appendTo(bh);

            $(resetBtn)
                .addClass("btn btn-purple")
                .button(resetBtnProps)
                .prepend('<i class="icon-refresh"></i>')
                .click(buttons[$.jgrid.search.Reset]).appendTo(bh);
        }

        $('.bocrud-toolbar[jsbid=' + currBocrud.config.id + ']:first .search-dependency').prependTo(bh);

        formObj.id = search_form_id;

        currBocrud.raiseEvent('onshowcontent', formObj, {}, 'DataEntry', '#' + place_holder);

        //formObj.validationObj = null; چون در برخی شرایط نیاز است
        $.bocrud.reg_form(formObj);
        //$('#' + formObj.id).removeData("validator");

        if (currBocrud.raiseEvent('onsearchstyled', buttons, formObj, '#' + place_holder) &&
            currBocrud.config.autoLoadData) {
            //currBocrud.do_refresh(1, true, true);
        }


    }

    var _format = function (format) {
        var args = $.makeArray(arguments).slice(1);
        if (format === undefined) {
            format = "";
        }
        return format.replace(/\{(\d+)\}/g, function (m, i) {
            return args[i];
        });
    }

    this.do_prj = function (elem, jqgrid) {
        jqgrid.jqGrid('columnChooser', {
            width: 550,
            height: 250,
            done: function (perm) {
                if (perm) {
                    jqgrid.jqGrid("remapColumns", perm, true);
                }
                jqgrid.data('prev-width', null);
                $.bocrud.alignGrid(jqgrid.closest('.ui-jqgrid').parent());
                currBocrud.raiseEvent('ongridcolumnchanged', jqgrid);
            }
        });
    }

    this.do_search = function () {
        currBocrud.raiseEvent('onbeforesearch');
        currBocrud.grid().bocrudSearch('show');
    }

    this.do_refresh = function (pn, silent, remove_filter) {

        $.ajax({
            showLoading: false,
            msg: $.bocrud.msg.redirecting,
            url: '/Bocrud/Refresh',
            dataType: "text",
            data: {
                xml: currBocrud.config.xml
            },
            type: "post",
            error: function (data) {
                $.console.error("bocrud-341:server return error." + data.responseText);
            },
            success: function (response) {

                currBocrud.raiseEvent('onbeforerefresh');
                var g = currBocrud.grid();

                var postData = g.jqGrid("getGridParam", 'postData');
                var load_all = remove_filter;
                if (!silent && (postData['filters'] || postData["_search"]))
                    load_all = false; // confirm($.bocrud.msg.retrieve_all);

                if (load_all) {

                    g.jqGrid("getGridParam").postData = {
                        _search: false,
                        filters: "",
                        xml: postData.xml,
                        tempMode: postData.tempMode,
                        jsbid: postData.jsbid
                    };

                    var st = g.closest('.ui-jqgrid').find('.ui-search-table:first');
                    st.find(':input').each(function () {
                        var t = $(this);
                        t.val('');
                    });
                }

                g.data('prev-selected-node', null)
                if (!currBocrud.config.autoLoadData) {
                    g.jqGrid('setGridParam', {
                        datatype: 'json'
                    });
                    if (pn)
                        g.jqGrid('setGridParam', {
                            lastpage: pn
                        });
                    currBocrud.config.autoLoadData = true;
                }
                var page = pn || g.getGridParam('page');

                g.trigger('reloadGrid', [{
                    page: page
                }]);

            }
        });


    }

    this.exprint = function (postfix, btnName, action, link) {
        var c = currBocrud;
        var cfg = c.config;

        if (c[postfix] == undefined) {
            var pfs = $("div#" + cfg.id + postfix)
                .css({
                    'position': 'absolute',
                    'z-index': 3000
                });
            if (pfs.length > 0) {
                pfs.children()
                    .menu({
                        position: {
                            my: 'right top', at: 'left top'
                        }
                    })
                    .find('a')
                    .click(function () {
                        c.exprint(postfix, btnName, action, $(this));
                    });

                var bc = function (t) {
                    var pfs = $("div#" + cfg.id + postfix);
                    pfs.show()
                        .position({
                            my: 'right top', at: 'right+30 bottom', of: t
                        });

                    $(document).one('click', function () {
                        pfs.hide();
                    });
                };
                var btn = c.$("#" + cfg.id + btnName, false)
                    .unbind('click')
                    .bind('click', function () {
                        bc(this);
                        return false;
                    });
                bc(btn.get(0));
                c[postfix] = true;
                return;
            } else
                c[postfix] = false;
        }

        var postData = c.grid().jqGrid('getGridParam', 'postData');

        postData['desc'] = c.$("#" + config.id + btnName, false).data('desc');
        postData['xml'] = cfg.xml;
        postData['xml'] = cfg.xml;
        postData['expkey'] = 'true';
        postData['jsbid'] = cfg.id; // for config filter
        if (link) {
            var path = link.attr('path');
            if (path) {
                var encodedPath = Base64.encode(path);
                postData['path'] = escape(encodedPath);
            } else
                postData['path'] = '';
            postData['expkey'] = link.attr('key');
        } else {
            postData['expkey'] = 'ExcelExporter';
        }

        var vCols = [];
        var colModel = c.grid().jqGrid('getGridParam', 'colModel');
        for (var i = 0; i < colModel.length; ++i)
            if (!colModel[i].hidden)
                vCols.push(colModel[i].name);

        postData['vcols'] = vCols.join(',');

        postData['rows'] = 500;

        var form = $('<form/>')
            .attr('action', cfg.urlPrefix + "/" + action)
            .attr('method', 'post')
            .attr('target', '_blank');

        for (var key in postData) {
            $('<input type="hidden"/>')
                .attr('name', key)
                .attr('value', postData[key])
                .appendTo(form);
        }
        form.appendTo('body').submit();
    }

    this.delCurr = function () {
    }

    this.onSaveClick = function () {
    }

    this.toolbarButtonAdd = function (elem, p) {
        p = $.extend({
            id: "",
            caption: "newButton",
            title: '',
            context: '',
            buttonicon: 'ui-icon-newwin',
            onClickButton: null,
            position: "last",
            classes: '',
            dependToRow: false,
            showText: false
        }, p || {
        });

        var tableString = "<div class=\"ui-toolbar-table\">";
        tableString += "</div>";
        if (elem.indexOf("#") != 0) {
            elem = "#" + elem;
        }

        if (currBocrud.$(elem, true).children('.ui-toolbar-table:first').length === 0) {
            currBocrud.$(elem).append(tableString);
        }
        var tbd = $("<button title=\"" + p.title + "\" ></button>");
        $(tbd).addClass('ui-toolbar-button ' + p.classes)
            .append("<span class=\"ui-icon " + p.buttonicon + "\"></span>")
            .click(function (e) {
                if (!$(this).hasClass('ui-state-disabled')) {
                    if (currBocrud[p.onClickButton]) {
                        try {
                            currBocrud[p.onClickButton](p.context);
                        }
                        catch (e) {
                            $.console.error("bocrud-2141:" + e);
                        }
                    } else {
                        if ($.isFunction(p.onClickButton))
                            p.onClickButton(p.context);
                    }
                }

                return false;
            });

        if (p.showText)
            $(tbd).append("<span>" + p.caption + "</span>");

        if (p.id) {
            $(tbd).attr("id", p.id);
        }
        if (p.align) {
            currBocrud.$(elem).attr("align", p.align);
        }
        var findnav = currBocrud.$(elem).children('ul');
        p.position = 'last';
        if (p.position === 'first') {
            if ($(findnav).find('li').length === 0) {
                $(findnav).append(tbd);
            } else {
                $(findnav).prepend(tbd);
            }
        } else {
            $(findnav).append(tbd);
        }
    }

    this.getActiveCatCaption = function () {
        var $t = $('#' + currBocrud.config.id + '-bctree-cats');
        //var active = $(".selector").accordion("option", "active");
        var activeTabHeader = $t.find("h3[tabindex='0']");
        return activeTabHeader.text();
    }

    this.checkActiveCat = function () {
        var $t = $('#' + currBocrud.config.id + '-bctree-cats');
        var activeTabContent = $t.find("h3[tabindex='0']").next();
        var req = activeTabContent.attr('req');
        var selNodeID = activeTabContent.find('li a.jstree-clicked').parent().attr('id');
        if (selNodeID == undefined && req == "True")
            return false;
        else
            return true;
    }

    this.getSelCatNode = function () {
        var $t = $('#' + currBocrud.config.id + '-bctree-cats');
        var activeTabContent = $t.find("h3[tabindex='0']").next();
        var prop = activeTabContent.attr('prop');
        var selNodeID = activeTabContent.find('li a.jstree-clicked').parent().attr('id');
        return selNodeID == undefined ? undefined : [prop, selNodeID];
    }

    this.initCatEvents = function () {

        var $t = $('#' + currBocrud.config.id + '-bctree-cats');

        var doFilter = function (div, ic, a) {
            var pkey = $t.attr('parentKey');
            var filter;
            var cli = a.parent(); // current li
            var nid = cli.attr('id');
            if (ic.is(':checked')) {
                var child_ids = [];
                cli.find('li').each(function () {
                    child_ids.push($(this).attr('id'));
                });
                child_ids.push(nid);
                filter = [{
                    groupOp: "AND", rules: [{
                        field: div.attr('prop'), op: "in", data: child_ids.toString()
                    }]
                }];
            } else
                filter = [{
                    groupOp: "AND", rules: [{
                        field: div.attr('prop'), op: "eq", data: nid
                    }]
                }];

            if (pkey != '')
                filter[0].rules.push({
                    field: pkey,
                    op: "eq",
                    data: ''
                });
            currBocrud.setFilter(filter);

        }


        $t.find('.bocrud-bctree-div').each(function () {

            var $div = $(this);
            var ic = $div.find("input[type='checkbox']")// include childs
                .first()
                .click(function () {
                    var snodes = $div.find('li a.jstree-clicked');
                    if (snodes.length > 0)
                        doFilter($div, $(this), snodes.first());
                });

            $div.css('height', '');
            var jstree = $div.find('div.jstree');
            jstree.unbind('select_node.jstree')
                .bind('select_node.jstree', function (event, data) {

                    // چون ممکنه بعدا یکی بیاد این کلید رو از بین ببره واسه همین همیشه می ریم می خونیمش

                    var nodeID = data.rslt.obj.attr('id');

                    if (jstree.data('clicked-node-id') == nodeID) {
                        jstree.jstree('deselect_all');
                        var filter = [];
                        currBocrud.setFilter(filter);
                        jstree.data('clicked-node-id', null);
                        return;
                    }
                    doFilter($div, ic, $(data.args[0]));

                    jstree.data('clicked-node-id', nodeID);
                })
                .unbind('deselect_node.jstree')
                .bind('deselect_node.jstree', function (event, data) {
                    var filter = [];
                    currBocrud.setFilter(filter);
                    jstree.data('clicked-node-id', null);
                });
        }); //bocrud-bctree-div
    }

    this.initCats = function () {

        var $t = $('#' + currBocrud.config.id + '-bctree-cats');
        var parentKey = $t.attr('parentKey');
        if ($t.closest('.bocrud-dialog').length == 0 || $t.find('.bocrud-bctree-div').length > 1) {
            $t.accordion();
        }
        else {
            $t.find('.bocrud-bctree-div').show();
        }

        if (currBocrud.config.autoHideCat === true)
            $.bocrud.makeAutoHide($t);

    }

    this._gridContainer = null;
    this.getGridContainer = function () {
        if (currBocrud._gridContainer == null)
            currBocrud._gridContainer = $('#' + currBocrud.config.id + "GridContainer");
        return currBocrud._gridContainer;
    }

    this.checkValidation_onload = function (oldLoadComplete) {
        var gridObj = currBocrud.grid();
        var dataLength;

        var ids = gridObj.jqGrid("getDataIDs");
        if (currBocrud.config.isTree) {
            dataLength = 0;
            for (var i = 0; i < ids.length; ++i) {
                var rowData = gridObj.jqGrid("getRowData", ids[i]);
                if (rowData[currBocrud.config.parentField] == '')
                    ++dataLength;
            }
        } else
            dataLength = ids.length;

        currBocrud._update_validation_field();

        if ($.isFunction(oldLoadComplete)) {
            oldLoadComplete();
        }
    }

    this.select2 = function (uid, remotely, ccs, ticket, tag, dir) {

        var cp = $("#" + uid);

        if (dir == 'None') dir = 'Rtl';

        cp.closest('.bocrud-control').addClass(dir);

        var o = {
            language: 'fa',
            openOnEnter: true,
            allowClear: true,
            minimumResultsForSearch: remotely ? 0 : 15,
            placeholder: $.bocrud.select.header,
            dir: dir.toLowerCase(),
        };

        var parent = cp.closest('.bocrud-page-form,.bocrud-dialog');
        if (parent.is('.bocrud-dialog'))
            o.dropdownParent = parent;
        else
            if (parent.is('.bocrud-page-form')) {
                if (parent.parent().is('.ui-dialog-content'))
                    o.dropdownParent = parent.parent().parent();
            }

        if (remotely) {
            o = $.extend(o, {
                debug: true,
                ajax: {
                    delay: 400,
                    dataType: 'JSON',
                    transport: function (p, callback) {
                        //(sender, ticket, userdata, callback, ajaxObj, validate, databind, background)
                        //sender, ticket, userdata, callback, ajaxObj, validate, databind, background
                        currBocrud.pcall(cp, p.ticket, p.data, callback, null, false, true, true);
                    },
                    ticket: ticket,
                    processResults: function (data) {
                        return JSON.parse(data);
                    }
                }
            });
        }

        if (tag) {
            o.placeholder = undefined;
            o = $.extend(o, {
                tags: true,
                multiple: true,
                tokenSeparators: [',', ' '],
                theme: 'classic'
            });
        }

        if (cp.closest('.bocrud-form-container').is('.bocrud-style-normaldialog'))
            o.dropdownParent = cp.closest('.ui-dialog');

        cp.select2(o).on("change", function () {

            if (ccs && $.isFunction(ccs))
                ccs();

            $('#' + uid + '-changes').val($('#' + uid).val() || 'null');
        });

        cp.closest('.bocrud-control-content').find('.select2-focusser,.select2-input').addClass('bocrud-focusable');

    }

    this.m2m_onSelectRow = function (rowid) {
        var g = currBocrud.grid();
        var s = g.jqGrid('getGridParam', 'selarrrow');
        var exists = false;
        for (var j = 0; j < s.length; ++j)
            if (s[j] == rowid) {
                exists = true; break;
            }

        var selarrrow = g.data('selarrrow') || [];

        var index = -1;
        for (var j = 0; j < selarrrow.length; ++j)
            if (selarrrow[j].id == rowid) {
                index = j; break;
            }
        var update = false;
        if (!exists && index >= 0) {
            selarrrow.splice(index, 1);
            update = true;
        } else if (index == -1) {
            var row = g.jqGrid('getRowData', rowid);
            selarrrow.push({
                id: rowid, row: row
            });
            update = true;
        }
        var deselected = [];
        var dataIds = g.jqGrid('getDataIDs');
        var currpage_selarrrow = g.jqGrid('getGridParam', 'selarrrow');
        for (var i = 0; i < selarrrow.length; ++i) {
            var index = -1;
            for (var j = 0; j < dataIds.length; ++j)
                if (dataIds[j] == selarrrow[i].id) {
                    index = j;
                    break;
                }
            if (index != -1) {
                var cindex = -1;
                for (var j = 0; j < currpage_selarrrow.length; ++j)
                    if (currpage_selarrrow[j] == selarrrow[i].id) {
                        cindex = j;
                        break;
                    }
                if (cindex == -1) {
                    deselected.push(selarrrow[i].id);
                }
            }
        }
        for (var i = 0; i < deselected.length; ++i) {
            var index = -1;
            for (var j = 0; j < selarrrow.length; ++j)
                if (selarrrow[j].id == deselected[i]) {
                    index = j;
                    break;
                }
            selarrrow.splice(index, 1);
        }
        if (update)
            g.data('selarrrow', selarrrow);
    }


    this.m2m_onPaging = function () {
        var g = currBocrud.grid();
        var selarrrow = g.data('selarrrow') || [];
        var currpage_selarrrow = g.jqGrid('getGridParam', 'selarrrow');
        for (var i = 0; i < currpage_selarrrow.length; ++i) {
            var exists = false;
            for (var j = 0; j < selarrrow.length; ++j)
                if (selarrrow[j].id == currpage_selarrrow[i]) {
                    exists = true; break;
                }
            if (!exists) {
                var row = g.jqGrid('getRowData', currpage_selarrrow[i]);
                selarrrow.push({
                    id: currpage_selarrrow[i], row: row
                });
            }
        }
        var dataIds = g.jqGrid('getDataIDs');
        for (var i = 0; i < dataIds.length; ++i)
            for (var j = 0; j < selarrrow.length; ++j)
                if (dataIds[i] == selarrrow[j].id) {
                    g.jqGrid("setSelection", selarrrow[j].id, false);
                }
        g.data('selarrrow', selarrrow);
    }

    this.m2m_add = function (id, grid, context) {

        var args = context.toString().split(',');
        var xml = args[0];
        var bocrudId = args[1];
        var bindingProperty = args[2];
        var keyProperty = args[3];
        var width = parseInt(args[4], 10);
        var height = parseInt(args[5], 10);
        var dialog;

        var bocrud = window.bocruds[bocrudId];
        var fromObj = bocrud.grid();
        var toObj = grid;

        try {
            fromObj.jqGrid('resetSelection');
        }
        catch (ex) {
        }

        var orgDiv = fromObj.closest('.bocrud-container').parent();
        var dialogParent = orgDiv.parent();

        // چون گرید درختی نیست
        orgDiv.find('#' + bocrudId + '-bctree-cats').attr('parentKey', '');

        var buttons = {
        };

        buttons[$.bocrud.select.bSelect] = function () {

            var selarrrow = fromObj.data('selarrrow') || [];
            var currpage_selarrrow = fromObj.jqGrid('getGridParam', 'selarrrow');
            for (var i = 0; i < currpage_selarrrow.length; ++i) {
                var exists = false;
                for (var j = 0; j < selarrrow.length; ++j)
                    if (selarrrow[j].id == currpage_selarrrow[i]) {
                        exists = true; break;
                    }
                if (!exists && currpage_selarrrow[i]) {
                    var row = fromObj.jqGrid('getRowData', currpage_selarrrow[i]);
                    selarrrow.push({
                        id: currpage_selarrrow[i], row: row
                    });
                }
            }

            fromObj.data('selarrrow', selarrrow);

            if (selarrrow.length == 0) {
                alert($.bocrud.msg.select_your_items);
                return;
            }

            toObj.jqGrid('clearGridData');


            for (var i = 0; i < selarrrow.length; ++i) {
                var data2 = toObj.jqGrid("getRowData", selarrrow[i].id);
                if (currBocrud.isEmpty(data2))
                    toObj.jqGrid("addRowData", selarrrow[i].id, selarrrow[i].row);
            }

            currBocrud.m2mRefreshValue(toObj, keyProperty, bindingProperty);

            currBocrud.closeModal($(this));
        }

        buttons[$.jgrid.edit.bCancel] = function () {
            currBocrud.closeModal($(this));
        }

        var ww = $(window).width() - 200;
        width = width <= 100 ? ww * (width / 100) : width;
        width = width > orgDiv.width() ? width : orgDiv.width();

        var wh = $(window).height() - 200;
        height = height <= 100 ? wh * (height / 100) : height;

        var fromCaption = fromObj.jqGrid('getGridParam', 'caption');

        var selectData = function () {
            //fromObj.jqGrid("resetSelection");
            var s2 = fromObj.data('selarrrow') || [];
            for (var i = 0; i < s2.length; ++i) {
                fromObj.jqGrid("setSelection", s2[i].id, false);
            }
        };

        dialog = currBocrud.openModal(orgDiv, {
            title: fromCaption,
            width: width + 40,
            height: height,
            buttons: buttons,
            //dialogParent: dialogParent,
            destroyOnClose: false
        });

        var selarrrow = [];
        var s = toObj.jqGrid('getDataIDs').toString();
        if (s.length != 0) {
            var IDs = s.split(',');
            for (var i = 0; i < IDs.length; ++i) {
                var row = toObj.jqGrid("getRowData", IDs[i]);
                selarrrow.push({
                    id: IDs[i], row: row
                });
            }
        }
        fromObj.data('selarrrow', selarrrow);
        selectData();

        fromObj.jqGrid('setGridParam', {
            onDataLoad: function () {
                selectData();
            }
        }); //.trigger("reloadGrid");
    }

    this.m2mRefreshValue = function (grid, keyProperty, uid) {
        var result = "";
        var allIds = grid.jqGrid("getDataIDs");
        if (keyProperty == "Key") {
            for (var i = 0; i < allIds.length; ++i)
                result += allIds[i] + ";";
        } else {
            for (var i = 0; i < allIds.length; ++i) {
                var rowData = grid.jqGrid("getRowData", allIds[i]);
                result += rowData[keyProperty] + ";";
            }
        }
        $("#" + uid).val(result);
        $("#" + uid).change();

        currBocrud.raiseEvent('onm2mrefreshvaluedone', uid, result);

    }

    this.m2m_addNew = function (id, grid, context) {

        var args = context.toString().split(',');
        var xml = args[0];
        var bocrudId = args[1];
        var bindingProperty = args[2];
        var width = parseInt(args[3], 10);
        var keyProperty = args[4];
        var mainGridObj = jQuery("#" + bocrudId + "Grid");
        var dialog;

        var bocrud = window.bocruds[bocrudId];

        bocrud.addEvent('onsubmitsuccess', function (formObj, saveResult, showDetails) {
            grid.jqGrid('addRowData', saveResult.row[0].id, saveResult.row[0].cell);

            var rowData = mainGridObj.jqGrid("getRowData", saveResult.row[0].id);
            grid.jqGrid("addRowData", saveResult.row[0].id, rowData);
            currBocrud.m2mRefreshValue(toObj, keyProperty, bindingProperty);
            return true;
        }, 'm2m_addNew');

        bocrud.addNew(null, bocrud.grid());
    }

    this.cdd_add = function (uid) {
        var ddl = currBocrud.$('#' + uid + "-ddl", false);
        var target = window.bocruds[uid + "bocrud"];

        target.addEvent('onsubmitsuccess', function (formObj, saveResult, showDetails) {

            var text = target.item2Str(saveResult.row[0].cell, target.config.toStringFormat, target.grid());
            ddl.append("<option value='" + saveResult.row[0].id + "' selected='selected'>" + text + "</option>");

            return true;
        }, "cdd_add");

        target.addNew(null, null);
    }

    this.cdd_del = function (uid) {
        var ddl = currBocrud.$('#' + uid + "-ddl", false);
        var target = window.bocruds[uid + "bocrud"];
        var value = ddl.val();
        if (value.length > 0)
            if (confirm($.bocrud.msg.gridDel)) {
                target.del(value, function () {
                    ddl.find('option[value=' + value + ']').remove();
                });
            }
    }

    this.m2m_del = function (id, grid, context) {

        var args = context.toString().split(',');
        var xml = args[0];
        var bocrudId = args[1];
        var bindingProperty = args[2];
        var keyProperty = args[3];

        var bocrud = window.bocruds[bocrudId];
        var toObj = grid;

        var selarrrow = grid.jqGrid('getGridParam', 'selarrrow');
        var selrow = grid.jqGrid('getGridParam', 'selrow');
        var multiselect = grid.jqGrid('getGridParam', 'multiselect');
        var rows = multiselect ? selarrrow : [selrow];
        while (rows.length > 0) {
            var sid = rows.pop();
            grid.jqGrid('delRowData', sid);
        }

        currBocrud.m2mRefreshValue(toObj, keyProperty, bindingProperty);
    }


    this.isEmpty = function (obj) {
        for (var prop in obj) {
            if (obj.hasOwnProperty(prop))
                return false;
        }
        return true;
    }

    this.relationalPropertyDel = function (id, grid, context) {
        var bindingProperty = context;
        grid.jqGrid("delRowData", id);
        var result = "";
        var allIds = grid.jqGrid("getDataIDs");
        for (var i = 0; i < allIds.length; ++i)
            result += allIds[i] + ";";
        $("#" + bindingProperty + "val").val(result);
    }


    this.removeFile = function (uniqeuid, liId, file) {
        if (confirm($.bocrud.msg.gridDel) == true) {
            var field = $("#" + uniqeuid + "RemovedFiles");
            field.val(field.val() + ";" + file);
            var li = $("#" + liId);
            li.remove();
            var fileInputs = $("#" + uniqeuid);
            fileInputs.removeAttr('disabled');
            fileInputs.data('MultiFile').clone.removeAttr('disabled');
            fileInputs.get(0).MultiFile.clone.removeAttr('disabled');
            fileInputs.get(0).MultiFile.max++;
            fileInputs.data('MultiFile').max++;
            fileInputs.get(0).MultiFile.current.disabled = false;
            fileInputs.MultiFile('reEnableEmpty');
        }
    }

    this.multiFile = function (id, maxsize, accept, max) {
        $('#' + id).MultiFile({
            maxsize: maxsize, accept: accept, max: max,
            onFileSelect: function (element, value, master_element) {
                var duplicate = false;
                if (value.indexOf('\\') > 0)
                    value = value.substr(value.lastIndexOf('\\') + 1);

                $('a[group=' + id + ']').each(function () {
                    var file = $(this).attr("file").toLowerCase();
                    if (file == value.toLowerCase()) {
                        alert($.bocrud.msg.file_selected);
                        duplicate = true;
                    }
                });

                return !duplicate;
            },
            afterFileAppend: function (slave, MultiFile, files) {
                var type = slave.files[0].type;

                var childs = files.list.children();
                var div = childs.last();
                var img = $('<img/>').insertAfter(div.find('a'));
                if (type.indexOf('image') == 0) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        img.attr('src', e.target.result);
                    }
                    reader.readAsDataURL(slave.files[0]);
                } else {
                    var fn = slave.files[0].name.split('.');
                    var ext = fn[fn.length - 1];
                    img.attr('src', '/assets/img/icons/' + ext + '.png');
                }
            }
        });
    }
    // crud control on select row
    this.ccosr = function (uid, rowid, stat, e) {
        var b = window.bocruds['bctrl-' + uid];
        var g = b.grid();
        var ms = g.jqGrid('getGridParam', 'multiselect');
        var sn = [];
        if (ms)
            sn = g.jqGrid('getGridParam', 'selarrrow');
        else {
            var sr = g.jqGrid('getGridParam', 'selrow');
            if (sr)
                sn.push(sr);
        }
        $('#' + uid).val(sn.join(','));
    }
    //این خصوصیت برای اینه که قابلیت از انتخاب درآمدن رکورد ها رو به گرید بدیم

    this.grid_onSelectRow = function (rowid, stat, e) {

        if (!e || e.button == 0) {
            var gridObj = currBocrud.grid();

            if (gridObj.data('prev-click-time')) {
                currBocrud.raiseEvent('onrowdblclick', rowid);
                var cc = currBocrud.config;
                gridObj.jqGrid('setSelection', rowid, false);
                if (cc.dblClickAction) {
                    switch (cc.dblClickAction) {
                        case "ViewDetail":
                            currBocrud.get_form(rowid, "View", cc.xml);
                            break;
                        case "Update":
                            currBocrud.get_form(rowid, "DataEntry", cc.xml);
                            break;
                        default:
                            currBocrud.refresh_toolbar();
                            var btn = currBocrud.$('#' + cc.id + cc.dblClickAction + 'ubtn:first');
                            if (!btn.is('.disabled'))
                                btn.click();
                            break;
                    }
                }
                return;
            } else {
                gridObj.data('prev-click-time', Date());
                setTimeout(function () { gridObj.data('prev-click-time', null); }, 200);
            }

            var ms = gridObj.jqGrid('getGridParam', 'multiselect');
            var sn = [];
            if (ms)
                sn = gridObj.jqGrid('getGridParam', 'selarrrow');
            else {
                var sr = gridObj.jqGrid('getGridParam', 'selrow');
                if (sr)
                    sn.push(sr);
            }

            var deselect = !ms && gridObj.data('prev-selected-node') == rowid;
            if (deselect) {
                gridObj.jqGrid('resetSelection');
                gridObj.removeData('prev-selected-node');
                currBocrud.raiseEvent('ondeselectrow', gridObj, rowid);

                currBocrud.refresh_toolbar();
                return;
            }

            if (sn.length == 0) {
                gridObj.data('prev-selected-node', null);
                currBocrud.raiseEvent('ondeselectrow', gridObj, rowid);
            }
            if (sn.length > 0) {
                currBocrud.raiseEvent('onselectrow', gridObj, sn);
                if (!ms) {
                    gridObj.data('prev-selected-node', sn[0]);
                }
            }

            currBocrud.refresh_toolbar();
        }
    }

    this.refresh_toolbar = function () {
        var gridObj = currBocrud.grid();
        var ms = gridObj.jqGrid('getGridParam', 'multiselect');
        var sn = [];
        if (ms)
            sn = gridObj.jqGrid('getGridParam', 'selarrrow');
        else {
            var sr = gridObj.jqGrid('getGridParam', 'selrow');
            if (sr)
                sn.push(sr);
        }

        if (sn.length == 0 || sn.length == 1)
            currBocrud.initPasteBtn();

        var p = $('.toolbar[jsbid="' + currBocrud.config.id + '"]');
        p.find('.ui-toolbar-dependtorow').each(function () {
            var t = $(this);
            var min = parseInt(t.attr('data-min'));
            var max = parseInt(t.attr('data-max'));
            if (max > min || min != 0) {
                if (sn.length >= min && sn.length <= max) {
                    t.removeClass('ui-state-disabled').removeClass('disabled').removeClass('bocrud-inactive-toolbar-btn').removeAttr('disabled');
                } else {
                    t.addClass('ui-state-disabled').addClass('disabled').addClass('bocrud-inactive-toolbar-btn').attr('disabled', 'disabled');
                }
            }
        });
    }

    this.pasteBtnObj = null;
    this.initPasteBtn = function () {
        if (!currBocrud.config.isTree)
            return;

        if (currBocrud.config.isTree) {
            if (currBocrud.pasteBtnObj == null) {
                currBocrud.pasteBtnObj = $("#" + currBocrud.config.id + "TreePasteBtn");
            }
            if (!currBocrud.tns || !currBocrud.tns.id || currBocrud.tns.id == "") {
                currBocrud.pasteBtnObj.hide();
                return;
            }
            var gridObj = currBocrud.grid();
            var ids = gridObj.jqGrid('getDataIDs');
            var sel = gridObj.jqGrid('getGridParam', 'selrow');
            var reader = gridObj.jqGrid('getGridParam', 'treeReader');

            if (sel == currBocrud.tns.pid) {
                currBocrud.pasteBtnObj.hide();
                return;
            }

            if (sel == null) {
                //در اینجا می خوایم ببینیم آیا یکی از آیتم های ریشه را کپی یا انتقال کرده یا نه 
                var show = true;

                for (var i = 0; i < ids.length; ++i) {
                    var data = gridObj.jqGrid('getRowData', ids[i]);
                    if ((data[reader.parent_id_field] == '' || data[reader.parent_id_field] == '0') &&
                        ids[i] == currBocrud.tns.id) {
                        show = false;
                        break;
                    }
                }
                if (!show) {
                    currBocrud.pasteBtnObj.hide($.bocrud.effects.toolbars);
                    return;
                }
            }
            currBocrud.pasteBtnObj.show($.bocrud.effects.toolbars);
        }
    }

    // tree node subject to move or copy
    this.tns = {
    };

    this.treeCopy = function (id, grid, context) {
        currBocrud.tns.id = id;
        currBocrud.tns.op = 'TreeCopy';
        var data = grid.jqGrid('getRowData', id);
        var reader = grid.jqGrid('getGridParam', 'treeReader');
        currBocrud.tns.pid = data[reader.parent_id_field];
    }

    this.treeMove = function (id, grid, context) {
        currBocrud.treeCopy(id, grid, context);
        currBocrud.tns.op = 'TreeMove';
    }
    this.treePaste = function (id, grid, context) {

        if (!currBocrud.config.treeInfo || !currBocrud.tns)
            return;

        var xml = currBocrud.tns.op === 'TreeMove' ?
            currBocrud.config.treeInfo.mcxml : currBocrud.config.treeInfo.ccxml;

        var info = {
            src: currBocrud.tns.id,
            dest: id,
            xml: currBocrud.config.xml,
            jsbid: currBocrud.config.id
        };

        if (xml && xml.length > 0) {

            currBocrud.getData("TreeGetPasteDialog", {
                id: '',
                mode: 'DataEntry',
                xml: xml,
                src: currBocrud.tns.id,
                dest: id
            }, {
                msg: $.bocrud.msg.loading_paste_form,
                success: function (serverResponse) {

                    var viewDetailInfo = eval('(' + serverResponse + ')');
                    var $form = $("#pastedialog");
                    if ($form.length == 0) {

                        $('body').append("<form id='pastedialog' style='direction:" + $.bocrud.dir + "'></form>");
                        $form = $("#pastedialog");
                    }

                    currBocrud.showDetails(viewDetailInfo, $form, false);

                    // این برای اجرای کد مربوط به ولیدیشن هاست
                    currBocrud.raiseEvent('oneverypropertyplaced', viewDetailInfo, 'DataEntry', $form);

                    var buttons = {
                    };
                    buttons[$.jgrid.edit.bCancel] = function () {
                        currBocrud.closeModal($(this));
                    }
                    buttons[$.jgrid.edit.bPaste] = function () {

                        if ($form.valid && !$form.valid()) return;

                        $form.ajaxSubmit({
                            msg: $.bocrud.msg.paste_tree,
                            data: info,
                            url: currBocrud.config.urlPrefix + "/" + currBocrud.tns.op,
                            type: "POST",
                            semantic: true,
                            error: function (data) {
                                currBocrud.showError(data);
                            },
                            success: function (data) {

                                currBocrud.reloadGridData();

                                if (currBocrud.config.grouping) {
                                    gridObj.jqGrid('groupingGroupBy', currBocrud.config.groupBy);
                                    gridObj.jqGrid('resetSelection');
                                    //$(currBocrud._dependToRowJqueryId).hide();
                                    //currBocrud.lastId = "";
                                    //currBocrud.rowSelected = false;
                                }

                                currBocrud.closeModal($form);
                            }
                        });

                    }
                    currBocrud.openModal($form, {
                        buttons: buttons,
                        title: viewDetailInfo.title,
                        width: viewDetailInfo.width > 0 ? viewDetailInfo.width : 400,
                        height: viewDetailInfo.height > 0 ? viewDetailInfo.height : 300,
                        modal: true
                    });
                }
            });
        } else {

            $.ajax({
                showLoading: true,
                msg: currBocrud.tns.op == 'TreeMove' ? $.bocrud.msg.tree_move : $.bocrud.msg.tree_copy,
                data: info,
                url: currBocrud.config.urlPrefix + "/" + currBocrud.tns.op,
                type: "POST",
                semantic: true,
                error: function (data) {
                    currBocrud.showError(data);
                },
                success: function (data) {

                    var gobj = currBocrud.grid();
                    gobj.trigger('reloadGrid');
                    gobj.jqGrid('resetSelection');

                    if (currBocrud.tns.op === 'TreeMove') {
                        currBocrud.tns = {
                        };
                        currBocrud.initPasteBtn();
                    }

                    if (currBocrud.config.grouping) {
                        gobj.jqGrid('groupingGroupBy', currBocrud.config.groupBy);
                        gobj.jqGrid('resetSelection');
                    }
                }
            });
        }
    }

    this.checkConditions = function (propId, condition, bokey, xml, e, context) {

        condition = condition.replace(/'/g, "\"");
        var conditionObj = JSON.parse(condition);

        if (!context) {
            var p = $('#' + propId);
            var form = p.closest(".bocrud-page-form,.jqgrow");
            context = form.data('formObj');

        }

        for (var i = 0; i < conditionObj.length; ++i) {
            var evalScript = $.bocrud.getExpEvalScript(conditionObj[i].evalScript);

            var result;
            try {
                result = eval(evalScript);
            }
            catch (ex) {
                $.console.error("bocrud-2793:" + ex + ' on evaling ' + evalScript);
            }

            var actionResult = true;
            if (result == true)
                actionResult = currBocrud.doAction(conditionObj[i].actions, propId, xml, e);
            else
                actionResult = currBocrud.doAction(conditionObj[i].elseActions, propId, xml, e);

            if (actionResult === false) {
                event.preventDefault();
                return;
            }
        }
    }

    this.doAction = function (actions, propId, xml, action) {

        var currentProperty = $('#' + propId + '.conditional');
        if (currentProperty.length == 0) {
            currentProperty = $('#' + propId);
            if (currentProperty.length > 0)
                $.console.warn('a not conditional property but have some conditions !', currentProperty);
        }
        var parent = currentProperty.closest('.bocrud-page-form,tr.jqgrow'),
            firstChild = parent.children().first(),
            tabs = firstChild.hasClass('ui-tabs') ? firstChild : firstChild.siblings('.ui-tabs'),
            tabular = currBocrud.config.style == 'Tabular' ? 1 : 0;

        var show = function (ps, tabs, form, is, filter) { // properties as ps, inputs as is
            for (var j in ps) {
                var dp = form.find("#" + ps[j] + '-holder.bocrud-hidden-action,.bocrud-control[name=' + ps[j] + ']');

                if (filter && filter.length > 0 && dp.length > 0 && !dp.is(filter)) continue;

                is.push(dp);

                if (!dp.hasClass('bocrud-dfvh'))
                    dp.show($.bocrud.effects.elems, function () {
                        $.bocrud.alignGrid(form);
                    })
                        .removeClass("bocrud-hidden-action");

                if (dp.length == 0) { // may be it is a group
                    var subg = form.find('[name="' + ps[j] + '"]').filter('.bocrud-subgroup,.bocrud-group').filter('.bocrud-hidden-action');
                    if (subg.length >= 1) {

                        if (filter && filter.length > 0 && !subg.is(filter)) continue;

                        subg.show($.bocrud.effects.elems).removeClass("bocrud-hidden-action");

                        currBocrud.raiseEvent('onactionshowgroup', ps[j], form, actions);

                    } else {
                        if (form.data('formObj') && $.isFunction(form.data('formObj').showGroup))
                            form.data('formObj').showGroup(ps[j], form);
                    }
                }
            }


        }

        var hide = function (ps, tabs, form, filter_class_name) {
            var disTabs = new Array();
            for (var j in ps) {

                var prop = form.find("#" + ps[j] + '-holder,.bocrud-control[name=' + ps[j] + ']');
                prop.hide($.bocrud.effects.elems)
                    .addClass('bocrud-hidden-action');

                if (filter_class_name && filter_class_name.length > 0 && prop.length > 0) prop.addClass(filter_class_name);

                if (prop.length == 0) { // may be it is a group
                    var subg = form.find('[name="' + ps[j] + '"]').filter('.bocrud-subgroup,.bocrud-group');
                    if (subg.length >= 1) {

                        subg.hide($.bocrud.effects.elems).addClass("bocrud-hidden-action");

                        currBocrud.raiseEvent('onactionhidegroup', ps[j], form, actions);

                        if (filter_class_name && filter_class_name.length > 0) subg.addClass(filter);

                    } else {
                        if (form.data('formObj') && $.isFunction(form.data('formObj').hideGroup))
                            form.data('formObj').hideGroup(ps[j], form);
                    }
                }
            }
            if (disTabs.length > 0)
                tabs.tabs({
                    disabled: disTabs
                });
        }

        var enable = function (ps, is, form, filter) {
            for (var j in ps) {
                var pgn = ps[j];

                var dp = form.find("#" + pgn + '-holder,.bocrud-control[name=' + ps[j] + ']');

                if (dp.length == 0) {
                    dp = form.find('[name="' + ps[j] + '"]').filter('.bocrud-subgroup,.bocrud-group');
                    if (dp.length == 0) {
                        if (form.data('formObj') && $.isFunction(form.data('formObj').enableGroup))
                            form.data('formObj').enableGroup(ps[j], form);
                    }
                }

                if (is != null)
                    is.push(dp);

                dp.find('.bucrud-disabled-action').each(function () {
                    //bucrud-disabled-action disabled ui-state-disabled
                    var p = $(this);

                    if (filter && filter.length > 0 && !p.is(filter)) return;

                    p.removeAttr('disabled');
                    if (p.is(':button.ui-button'))
                        p.button({
                            disabled: false
                        });
                    //if (p.is('.ui-toolbar-button'))
                    p.removeClass('ui-state-disabled');
                    p.removeClass('disabled');
                    p.removeClass('bucrud-disabled-action');
                    p.prop('disabled', false);
                });
            }
        }

        var setValue = function (ps, value, form) {

            if (value && value.indexOf('{') == 0 && value.endsWith('}')) {
                var cn = value.substring(1, value.length - 1);
                value = form.find('.bocrud-control[name=' + cn + '] :input.bocrud-input').val();
            }

            for (var j in ps) {
                var pgn = ps[j];

                var dp = form.find("#" + pgn + '-holder,.bocrud-control[name=' + ps[j] + ']');

                if (dp.length == 1) {
                    var t = dp.attr('type');
                    switch (t) {
                        case 'CheckBox':
                        case 'List':
                        case 'Radio':
                            dp.find(':radio').removeAttr('checked').filter('[value=' + value + ']').prop('checked', true).click();
                            break;
                        case 'One2Many':
                            if (!value || value == '') {

                            } else
                                Error("cannot set One2Many");
                            break;
                        default:
                            dp.find(':input.bocrud-input').not(':radio').val(value).change();
                            break;
                    }
                }
            }
        }

        var disable = function (ps, form, filter_class_name) {
            for (var j in ps) {
                var pgn = ps[j];

                var p = form.find("#" + pgn + '-holder,.bocrud-control[name=' + ps[j] + ']');

                if (p.length == 0)// may be it is a group
                {
                    p = form.find('[name="' + ps[j] + '"]').filter('.bocrud-subgroup,.bocrud-group');
                    if (p.length == 0) {
                        if (form.data('formObj') && $.isFunction(form.data('formObj').disableGroup))
                            form.data('formObj').disableGroup(ps[j], form);
                    }
                }

                p.find(':input,.bocrud-input,:button,.ui-toolbar-button')
                    .filter(':not([disabled=disabled],.ui-state-disabled,.disabled)')
                    .each(function () {
                        var dp = $(this);

                        if (filter_class_name && filter_class_name.length > 0) dp.addClass(filter_class_name);

                        if (dp.is(':button.ui-button') || dp.data('bs.button')) {
                            dp.button("option", "disabled", true);
                        }
                        if (dp.is('.ui-toolbar-button'))
                            dp.addClass('ui-state-disabled');
                        dp.attr('disabled', 'disabled');
                        dp.addClass('disabled');
                        dp.addClass('bucrud-disabled-action');
                        dp.addClass('ui-state-disabled');
                        dp.prop('disabled', true);
                    });
            }
        }

        var changeCaption = function (ps, form, text) {
            for (var j in ps) {
                var pgn = ps[j];

                var p = form.find("#" + pgn + '-holder,.bocrud-control[name=' + ps[j] + ']');
                if (p.length == 0)// may be it is a group
                {
                    p = form.find('[name="' + ps[j] + '"]').filter('.bocrud-subgroup,.bocrud-group');
                    p.children('legend:first').text(text);
                } else {
                    p.find('.bocrud-control-caption:first .caption-text').text(text);
                }
            }
        }

        var get_ps = function (pss) {
            if (!pss) return [];
            var temp_ps = pss.split(',');
            var ps = [];
            for (var pi = 0; pi < temp_ps.length; ++pi) {
                var pn = temp_ps[pi];

                if (pn.substring(0, "group:".length).toLowerCase() == "group:") {
                    var gn = pn.split(':')[1];
                    parent.find('[name="' + gn + '"]')
                        .find(".bocrud-control")
                        .each(function () {
                            ps.push($(this).attr('name').trim());
                        });
                } else {
                    ps.push(pn.trim());
                }
            }
            return ps;
        }

        for (var i = 0; i < actions.length; ++i) {

            if (!(actions[i].event == 'All' || actions[i].event == action || action == 'All'))
                continue;

            var ps = get_ps(actions[i].properties);
            var pos = new Array();

            switch (actions[i].action) {
                case "Toast":
                    $.bocrud.toast(currBocrud.config.id, ps, 'information');
                    break;
                case "Confirm":
                    if (!confirm(ps)) return false;
                    break;
                case "Alert":
                    setTimeout(function (msg) { alert(msg); }, 1000, ps);
                    break;
                case "SetCaption":
                    changeCaption(ps, parent, actions[i].serverUpdate);
                    break;
                case "Hide":
                    if (parent.data('formObj') && parent.data('formObj').mode == 'DataEntry')
                        disable(ps, parent, 'bocrud-disable-for-hidden-action');
                    hide(ps, tabs, parent);
                    break;
                case "HideButEnable":
                    hide(ps, tabs, parent);
                    break;
                case "Show":
                    show(ps, tabs, parent, pos);
                    if (parent.data('formObj') && parent.data('formObj').mode == 'DataEntry')
                        enable(ps, null, parent, '.bocrud-disable-for-hidden-action');
                    break;
                case "Enable":
                    enable(ps, pos, parent);
                    break;
                case "Disable":
                    disable(ps, parent);
                    break;
                case "Save":
                    currBocrud.save(parent.attr('id'), false, currBocrud.config.xml, false);
                    break;
                case "Reload":
                    if (parent.data('formObj')) {
                        var key = parent.data('formObj').boKey;
                        var mode = parent.data('formObj').mode;
                        currBocrud.get_form(key, mode, currBocrud.config.xml, function () {
                        });
                    }
                    break;
                case "Set":
                    setValue(ps, actions[i].userData, parent);
                    break;
                case "Update":
                    //if (parent.data('formObj')
                    //&& parent.data('formObj').mode != 'DataEntry' &&
                    //parent.data('formObj').mode != 'GridDataEntry' 
                    // برای اینکه در حالات خاصی نیاز است که فرم در حالت مشاهده این قابلیت را داشته باشد
                    //)
                    //return;

                    var async = true;
                    if (i < actions.length - 1) {
                        var found = false;
                        for (var j = i + 1; j < actions.length && !found; ++j) {
                            var affect_on = get_ps(actions[j].properties);
                            for (var k = 0; k < affect_on.length && !found; ++k) {
                                for (var w = 0; w < ps.length; ++w)
                                    if (ps[w] == affect_on[k]) {
                                        found = true; break;
                                    }
                            }
                        }
                        if (found) {
                            async = false;
                        }

                    }

                    currBocrud.partialSubmit
                        (propId /* pid : */
                            , xml /* xml : */
                            , actions[i].properties /* ps : */
                            , actions[i].serverUpdate /* sups : */
                            , actions[i].userData /* ud : */
                            , actions[i].fake /* fake : */
                            , async /* async : */
                            , null /*oncomplete*/
                            , null /*timeout*/
                            , action /*action */
                        );

                    break;
                case "Custom":
                    try {
                        eval(actions[i].properties);
                    }
                    catch (ex) {
                        $.console.error('4036: bocrud custom  action error : ' + actions[i].properties);
                    }
                    break;
            }

            for (var k = 0; k < pos.length; ++k) {
                var co = pos[k].find('.conditional');
                if (co.length > 0) {
                    if (co.attr("type") && co.attr("type").toLowerCase() == "checkbox") {
                        var val = co.is(':checked');
                        if ($.isFunction(co.attr('onclick'))) {
                            try {
                                co.attr('onclick').call();
                            }
                            catch (ex) {
                                $.console.error("bocrud-2921:" + ex);
                            }
                        }

                    } else {
                        try {
                            co.change();
                        }
                        catch (ex) {
                            $.console.error("bocrud-2927:" + ex);
                        }
                    }
                }
            }
        }
        if (currBocrud)
            currBocrud.raiseEvent('onactiondone', actions, propId, xml);

    }

    var partialSubmitQueue = [], partialSubmitLastCall, clearPartialSubmitHandler, partialSubmitIsInProgress, partialSubmitHandler;
    this.partialSubmit = function (pid, xml, ps, sups, ud, fake, async, oncomplete, timeout, action) {

        var exists = false;
        for (var i = 0; i < partialSubmitQueue.length; ++i)
            if (partialSubmitQueue[i].pid == pid) {
                exists = true; break;
            }

        if (!exists)
            partialSubmitQueue.push({
                pid: pid,
                xml: xml,
                ps: ps,
                sups: sups,
                ud: ud,
                fake: fake,
                async: async,
                oncomplete: oncomplete,
            });
        else if (!timeout) {
            return "pending";
        }

        partialSubmitLastCall = new Date();
        if (!clearPartialSubmitHandler) {
            clearPartialSubmitHandler = setInterval(function () {
                if (partialSubmitQueue.length == 0) {
                    clearInterval(clearPartialSubmitHandler);
                    clearPartialSubmitHandler = null;
                    return;
                }
                var now = new Date();
                if (now - partialSubmitLastCall > 5000 && !partialSubmitIsInProgress) {
                    partialSubmitQueue.splice(0, partialSubmitQueue.length);
                }
            }, 1000);
        }

        var wait_time = action == 'OnLoad' ? 1000 : 300;

        if (!timeout) {
            if (partialSubmitHandler) {
                clearTimeout(partialSubmitHandler);
                partialSubmitHandler = setTimeout(function () { currBocrud.partialSubmit(pid, xml, ps, sups, ud, fake, async, oncomplete, true); }, wait_time);
                return "pending";
            }
            else {
                partialSubmitHandler = setTimeout(function () { currBocrud.partialSubmit(pid, xml, ps, sups, ud, fake, async, oncomplete, true); }, wait_time);
                return "pending";
            }
        } else {
            if (partialSubmitHandler)
                clearTimeout(partialSubmitHandler);
            partialSubmitHandler = null;
        }

        var groups = {
        };
        for (var i = 0; i < partialSubmitQueue.length; ++i) {
            var c = partialSubmitQueue[i];
            if (!groups[c.xml]) groups[c.xml] = [];
            groups[c.xml].push(c);
        }

        partialSubmitQueue.splice(0, partialSubmitQueue.length);

        for (var gxml in groups) {
            var cpid = [], cps = [], csups = [], cud = [], cfake = true, casync = true, concomplete = [];
            for (var i = 0; i < groups[gxml].length; ++i) {
                var c = groups[gxml][i];

                if (c.pid && c.pid.length > 0) cpid.push(c.pid);
                if (c.ps && c.ps.length > 0) cps.push(c.ps);
                if (c.sups && c.sups.length > 0) csups.push(c.sups);
                if (c.ud && c.ud.length > 0) cud.push(c.ud);
                if (c.fake === false) cfake = false;
                if (c.async === false) casync = false;
                if ($.isFunction(c.oncomplete)) concomplete.push(c.oncomplete);
            }

            __partialSubmit(cpid.join(','), gxml, cps.join(','), csups.join(','), cud.join(','), cfake, casync, concomplete);
        }
    }

    var __partialSubmit = function (senders, xml, ps, sups, ud, fake, async, oncomplete_array, tryCount) {

        // ps : result properties
        // sups : controls that data must bound
        // ud : user data
        var senders_array = senders.split(',');
        var dialogObj = null;
        for (var i = 0; i < senders_array.length; ++i) {
            dialogObj = $('#' + senders_array[i]).closest('.bocrud-page-form,tr.jqgrow,.bocrud-singletone');
            if (dialogObj.length > 0) break;
        }

        if (!dialogObj || !dialogObj.data('formObj'))
            return;

        currBocrud.raiseEvent('onpartialupdate', dialogObj, senders, xml, ps, sups, ud, fake);

        var tps = "," + ps + ",";
        var tsups = "," + sups + ",";
        dialogObj.find('.bocrud-control').each(function () {
            var t = $(this);

            if (tps.indexOf("," + t.attr('name') + ",") >= 0) {
                var o = t.offset(), w = t.width(), h = t.height();

                $('<div class="bocrud-control-loading"/>')
                    .appendTo(t.find('.bocrud-control-content:first'))
                    .offset(o).width(w).height(h).show();
            }

            if (tsups.indexOf("," + t.attr('name') + ",") >= 0) {
                t.find(':input').each(function () {
                    if ($(this).is(':disabled')) {
                        $(this).addClass('partial-update-temp-enabled').removeAttr('disabled');
                    }
                });
            }
        });

        var selId = '';

        if (dialogObj.is('tr.jqgrow'))
            selId = dialogObj.attr('id');
        else
            selId = dialogObj.data('formObj').boKey; //dialogObj.attr("boKey");

        if (!selId || selId == null || selId.indexOf('__temp_') >= 0)
            selId = "";

        // چون ممکنه که مقادیر پیشفرض موجود در آدرس صفحه مربوط به این بوکراد نباشند
        var urlXml = currBocrud.urlParams["xml"] ? currBocrud.urlParams["xml"] : '';
        var dvs = '';
        var triggers = [];
        for (var i = 0; i < senders_array.length; ++i) {
            var h = currBocrud.$('#' + senders_array[i] + '-holder');
            triggers.push(h.attr('uidPrefix') + ":" + h.attr('name'));
        }

        var data = {
            fake: fake,
            xml: dialogObj.data('formObj').xml || config.xml,
            jsbid: config.id,
            id: selId,
            tempMode: config.tempMode,
            mainXml: config.xml,
            nodeid: currBocrud.config.isTree ? currBocrud.grid().jqGrid('getGridParam', 'selrow') : '',
            ps: ps,
            sups: sups,
            dvs: dvs,
            ud: ud,
            tr: triggers.join(','),
            nv: Date().toString(),
            context: config.context,
            mode: currBocrud.config.mode == 'GridDataEntry' ? 'GridDataEntry' : dialogObj.data('formObj').mode,
        };

        currBocrud._add_url_params(data);

        var url = config.urlPrefix + "/PartialUpdate";

        dialogObj.ajaxSubmit({
            msg: $.bocrud.msg.partial_updating,
            url: url,
            async: async,
            semantic: true,
            type: 'POST',
            dataType: "text",
            global: true,
            data: data,
            error: function (sr) {
                if (!tryCount || tryCount < 3) {
                    __partialSubmit(senders, xml, ps, sups, ud, fake, async, oncomplete_array, (tryCount || 0) + 1);
                    return;
                }
                currBocrud.raiseEvent('onpartialerror', sr, data.mode, dialogObj);

                currBocrud.showError(sr);
            },
            success: function (sr) {

                if (!sr || sr.length == 0) return;

                if (sr == 'Retry' && (!tryCount || tryCount < 3)) {
                    __partialSubmit(senders, xml, ps, sups, ud, fake, async, oncomplete_array, (tryCount || 0) + 1);
                    return;
                }
                if (sr == 'Retry') {
                    $.bocrud.toast(currBocrud.config.id, $.bocrud.msg.partial_error, 'error');
                    return;
                }
                currBocrud.raiseEvent('onpartialsuccess', sr, data.mode, dialogObj);

                var ro; // response object
                if (typeof (sr) == 'string') {
                    if (sr.indexOf("<pre>") >= 0)
                        sr = sr.substring(5, sr.length - 6);
                    ro = eval('(' + sr + ')');
                }
                else
                    ro = sr;

                if (ro.error) {
                    currBocrud.showError(ro);
                    return;
                }

                var up = function (cprr) {
                    var pc = dialogObj.find('#' + cprr.uid).closest('.bocrud-control');
                    pc.removeClass('has-success');
                    if (pc.length == 0)
                        pc = dialogObj.find('.bocrud-control[name="' + cprr.name + '"]');
                    if (pc.length == 0) {
                        pc = dialogObj.find('#' + cprr.uid + '-holder');
                    }

                    currBocrud.raiseEvent('onprepartialupdatereplaced', cprr.name, ro, data.mode, dialogObj);

                    if (cprr.content == null || cprr.content == undefined) return;

                    var inner_content = $(cprr.content).filter('.bocrud-control').first().html();

                    //if (cprr.content.indexOf("&lt;") == 0)
                    pc.html(inner_content);
                    //else
                    //    pc.html(inner_content);

                    var elem = $('#' + cprr.uid);
                    var form = elem.form || elem.closest('.bocrud-page-form');
                    if (form.length > 0 && form.data("validator"))
                        for (var k in cprr.rules) {
                            elem.rules('remove');

                            var newRule = $.extend(cprr.rules[k], {
                                messages: cprr.messages[k]
                            });
                            $('#' + k).rules('add', newRule);
                        }
                }

                for (var i = 0; i < ro.length; ++i)
                    up(ro[i]);

                $.bocrud.decorateUi(dialogObj);

                currBocrud.raiseEvent('oncontrolplaced', ro, data.mode, dialogObj);
                currBocrud.raiseEvent('oneverycontrolplaced', ro, data.mode, dialogObj);
                currBocrud.raiseEvent('onpartialupdatedone', ro, data.mode, dialogObj);

                if (oncomplete_array && oncomplete_array.length > 0)
                    for (var i = 0; i < oncomplete_array.length; ++i)
                        if (oncomplete_array[i])
                            oncomplete_array[i](ro, data.mode, dialogObj);
            },
            complete: function () {
                dialogObj.find('.bocrud-control-loading').remove();
                currBocrud.raiseEvent('onpartialupdatecomplete', data.mode, dialogObj);
                dialogObj.find('.partial-update-temp-enabled').each(function () {
                    $(this).removeClass('partial-update-temp-enabled').attr('disabled', 'disabled');
                });
            }
        }); //ajaxSubmit
    }

    this.showError = function (sr) {

        if (sr.responseText && sr.responseText.length > 0) {
            sr = sr.responseText;
        }

        var exception = null;
        if (typeof (sr) == 'string') {
            try {
                exception = eval('(' + sr + ')');
            } catch (e) {
                exception = null;
                if (sr.indexOf('<!DOCTYPE html>') >= 0) {
                    var st = '<h2> <i>';
                    var si = sr.indexOf(st);
                    var ei = sr.indexOf('</i> </h2>');
                    exception = {
                        error: true,
                        dir: 'ltr',
                        message: sr.substring(si + st.length, ei)
                    };
                }
            }
        }
        else {
            exception = sr;
        }
        if (!(exception != null && exception.error)) {
            var text = sr.responseText;
            if (!text) return;
            var startIndex = text.indexOf("<!--", 0);
            var comment = text.substring(startIndex + 4, text.length - 3);
            var mc = comment.match(/^\[[^\n]*\n/gm);
            var ex = {
            };
            var temp_ex = ex;
            if (mc != null) {
                for (var i = 0, mi = 0; i < mc.length; ++i) {
                    ex.dir = 'ltr';
                    ex.error = true;
                    ex.message = mc[i];
                    mi = comment.indexOf(mc[i], mi);
                    mi += mc[i].length;
                    if (i == mc.length - 1) {
                        ex.stackTrace += comment.substring(mi);
                    }
                    else {
                        var nextIndex = comment.indexOf(mc[i + 1], mi);
                        ex.stackTrace += comment.substring(mi, nextIndex);
                        ex.innerException = {
                        };
                    }
                    ex = ex.innerException;
                }
            }
        }
        if (currBocrud.raiseEvent('onerror', exception) === false)
            return;
        //$.showException(exception);
    }

    this.o2m_init = function (uid, sortable) {
        if (sortable) {
            var grid = currBocrud.grid();
            grid.sortable({
                items: " tr.jqgrow",
                axis: 'y',
                forceHelperSize: true,
                forcePlaceholderSize: true
            }).on('sortdeactivate', function (event, ui) {
                var grid = currBocrud.grid();
                var orders = grid.sortable('toArray');
                currBocrud.$('input#' + uid + '-orders').val(orders);
            });
        }
    }

    this.o2m_init_grid_inline_data = function (o2m_jsbid, strdata) {
        var b = window.bocruds[o2m_jsbid];
        var g = b.grid();
        var data = eval('(' + strdata + ')');

        var main_form = null;

        for (var i = 0; i < data.rows.length; ++i) {
            var tr = g.find('tr#' + data.rows[i].id);

            if (main_form == null)
                main_form = tr.closest('form');

            var ruid = o2m_jsbid + '-' + data.rows[i].id;

            var formObj = {
                id: data.rows[i].id,
                uid: ruid,
                validationObj: data.rows[i].validationObj,
                jsbid: o2m_jsbid,
                mode: 'GridDataEntry',
                xml: b.config.xml
            };

            tr.addClass('bocrud-page-form')
                .attr('uid', ruid)
                .data('formObj', formObj);

            if (formObj.validationObj && !jQuery.isEmptyObject(formObj.validationObj.rules)) {
                main_form.validate(formObj.validationObj);
            }

            $.bocrud.reg_form(formObj);
        }
    }

    this.m2oOnGridLoadComplete = function (uid) {
        var gid = "m2o-" + uid + "-Grid";
        var first_row = $('table#' + gid + ':first .jqgrow:first');
        if (first_row.length > 0) {
            var first_row_id = first_row.attr('id');
            currBocrud.grid().jqGrid('setSelection', first_row_id, true);
        }
    }

    this.m2ocp = function (uid, ticket, xml) {
        var cp = $('#' + uid + 'cp');
        if (!cp.hasClass('blur-event-assigned')) {
            cp.addClass('blur-event-assigned');
            var idc = $('#' + uid);
            var dsc = $("#" + uid + 'sel');

            var checkCode = function () {
                var cpv = cp.val();
                if (cpv.length == 0) {
                    idc.val('');
                    dsc.val('');
                    return;
                }
                cp.attr('disabled', 'disabled');
                var whenDone = function (r) {
                    cp.removeAttr('disabled');
                    var di = r.indexOf('$');
                    if (di > 0) {
                        idc.val(r.substring(0, di)).change();
                        dsc.val(r.substring(di + 1));
                        return;
                    } else {
                        if (r == 'not_found')
                            dsc.val($.bocrud.msg.not_found);
                        else
                            dsc.val(r);
                        idc.val('').change();
                        return;
                    }

                };
                currBocrud.pcall(cp, ticket, { cpval: cpv }, whenDone, null, false, false);
            };

            cp.blur(checkCode).keypress(function (e) {
                if (e.which == 32) {
                    $(this).closest('tr')
                        .find('button.uie-many-to-one:visible:first')
                        .click();

                    e.preventDefault();
                }
            });
        }
    }

    this.m2o_ondblClick = function (rowid, uid) {
        var t = $('#' + uid + "dialog");

        if (t.data('uiDialog') && t.data('uiDialog').options)
            t.data('uiDialog').options.buttons[$.bocrud.select.bSelect].apply(t);
        else {
            t.data('buttons')[$.bocrud.select.bSelect].apply(t);
        }
    }

    this.m2o_LookupBtn_OnClick = function (dlgid, puid, width, height, title, tbocrudid, format, xml, autoLoadData) {
        var lookupBtn = $('#' + puid + '-holder button.uie-many-to-one');
        var buttons = {
        };
        var orgDiv = $('#' + dlgid);
        var dialogParent = orgDiv.parent();

        buttons[$.bocrud.select.bSelect] = function (rowid) {
            var gridObj = currBocrud.grid();
            var s = '';
            if (typeof (rowid) == 'String')
                s = rowid;
            else
                s = gridObj.jqGrid('getGridParam', 'selrow');
            var displaybox = $("#" + puid + 'sel'), valuebox = $("#" + puid);
            var show = function (textOrHtml) {
                var text = $('<div/>').html(textOrHtml).text();
                if (displaybox.is('input'))
                    displaybox.val(text);
                else
                    displaybox.text(text);
            };
            if (!s || s == null || s == undefined) {

                show('');
                valuebox.val('').change();
            }
            else {
                var item = gridObj.jqGrid("getRowData", s);

                var cpi = $('#' + puid + 'cp');
                if (cpi.length) {
                    var cp = cpi.attr("name");

                    cpi.val($('<div/>').html(item[cp]).text()).change();
                }
                var strItem = currBocrud.item2Str(item, format, gridObj);
                show(strItem);
                valuebox.val(s).change();
                $("#" + puid + 'xml').val(xml);
            }
            currBocrud.closeModal($(this), dialogParent);

            var cpi2 = $('#' + puid + 'cp');
            if (cpi2.length > 0)
                cpi2.focus();
            else
                displaybox.focus();
        };
        buttons[$.jgrid.edit.bCancel] = function () {
            currBocrud.closeModal($(this), dialogParent);
        };
        var ww = $(window).width();
        width = width <= 100 ? ww * (width / 100) : width;
        width = width > orgDiv.width() ? width : orgDiv.width();

        currBocrud.initCatEvents();
        orgDiv.data('buttons', buttons);
        currBocrud.openModal(orgDiv, {

            title: title,
            buttons: buttons,
            width: width + 40,
            height: height,
            destroyOnClose: true,
            dialogParent: dialogParent,
            modal: false,
            position: 'top',
            create: function () {
                setTimeout(function () {
                    orgDiv.closest('.ui-dialog')
                        .children('.ui-dialog-buttonpane')
                        .find('button').addClass('bocrud-focusable');
                }, 100);
            },
            open: function () {
                $(document).on('keydown.m2o', function (e) {
                    var move = function (dir) {
                        var selrow = currBocrud.grid().jqGrid('getGridParam', 'selrow');
                        var nextrow = '';
                        var gid = "m2o-" + puid + "-Grid";
                        if (!selrow) {
                            var first_row = $('table#' + gid + ':first .jqgrow:first');
                            nextrow = first_row.attr('id');
                        } else {
                            var data = currBocrud.grid().jqGrid('getDataIDs');
                            if (data.length == 0) return;
                            var index = -1;
                            for (var i = 0; i < data.length; ++i) {
                                if (data[i] == selrow) {
                                    index = i;
                                    break;
                                }
                            }
                            if (dir == 'down') {
                                if (data.length == index + 1)
                                    index = -1;
                                nextrow = data[index + 1];
                            } else {
                                if (data.length == 0)
                                    index = data.length;
                                nextrow = data[index - 1];
                            }
                        }
                        if (nextrow) {
                            currBocrud.grid().jqGrid('setSelection', nextrow, true);
                        }
                        currBocrud.grid().find('tr#' + nextrow).focus();

                        e.preventDefault();
                        e.stopPropagation();
                    }
                    if (e.keyCode == 40)// down
                        move('down');
                    if (e.keyCode == 38)
                        move('up');
                    if (e.keyCode == 13) {

                        if ($(e.target).is(':input')) return;

                        var selrow = currBocrud.grid().jqGrid('getGridParam', 'selrow');
                        if (!selrow) {
                            move('down');
                            return;
                        }
                        var t = $('#' + puid + "dialog");
                        t.data('buttons')[$.bocrud.select.bSelect].apply(t.get(0), new Array(selrow));

                        lookupBtn.focus();

                        e.preventDefault();
                        e.stopPropagation();
                    }

                });
            },
            close: function () {
                $(document).off('keydown.m2o');
            }

        });

        if (autoLoadData) {
            var gridObj = currBocrud.grid();
            var ids = gridObj.jqGrid("getDataIDs");
            if (ids.length == 0) {
                window.bocruds["m2o-" + puid].do_refresh();
            }
        }
    }

    this.item2Str = function (item, format, gridObj) {
        var result = "";
        if (format && format.length > 0) {
            result = format;
            for (var p in item) {
                var re = new RegExp('{' + p + '}', "gi");
                result = result.replace(re, $('<div/>').html(item[p]).text());
            }
        } else {
            var str = "";
            for (var p in item) {
                var caption = p, val = $('<div/>').html(item[p]).text();
                if (gridObj)
                    caption = gridObj.jqGrid('getColProp', p).label;
                if (!caption || caption.indexOf('__') == 0) continue;
                if (caption) {
                    var pure_val = val.replace(/\s*/gm, "");
                    if (pure_val.length > 0)
                        str += caption + " : <b>" + val + "</b>, ";
                }
            }
            result = str.length > 0 ? str.substr(0, str.length - 2) : "";
        }
        return result;
    }

    this.onComboTree = function (uniqueId, cbx) {

        var ctr = $('#' + uniqueId + 'ctr:first');

        if (ctr.is(':visible')) {
            ctr.hide();
        } else {
            var sel = $('#' + uniqueId + 'sel:first');
            var so = sel.offset();
            so.top += sel.outerHeight();
            ctr.show()
                .offset(so)
                .width(sel.outerWidth());
        }

        if (!ctr.data('event')) {
            $(document).mouseup(function (e) {
                if (ctr.is(':visible') &&
                    !$(e.target).is('.bocrud-combotree') &&
                    !$(e.target).closest('div.bocrud-combotree').length > 0)
                    ctr.hide();
            });
            ctr.data('event', true);

            ctr.children(':eq(0)').bind('select_node.jstree', function (tree, data) {
                var li = data.rslt.obj;
                $('#' + uniqueId + 'sel:first').val($(data.args[0]).text());
                $('#' + uniqueId + ':first').val(li.attr('id'));
                ctr.hide();
            });
        }
    }

    this.simpleTree_LookupBtn_OnClick = function (uniqueId, cbx, getAll) {
        var buttons = {
        };
        var dialog;
        var orgDiv = $('#' + uniqueId + 'dialog');
        var dialogParent = orgDiv.parent();

        buttons[$.jgrid.edit.bSubmit] = function () {
            var nods, lm = cbx == "True", txt = "", ids = "", maxCols = 0, getAllNodes = getAll == "True";

            if (!lm)
                nods = $('#' + uniqueId + "tree").find(".jstree-clicked");
            else
                nods = $('#' + uniqueId + "tree").jstree('get_checked', null, getAllNodes);

            var selected_ids = [];
            nods.each(function () {
                if (lm)
                    selected_ids.push($(this).attr('id'));
                else
                    selected_ids.push($(this).parent().attr('id'));
                var fc = lm ? $(this).attr("caption") : ''; // full caption
                $(this).parents("li[rel=jstree]").each(function () {
                    var parentTitle = $(this).attr("caption") + (fc.length == 0 ? "" : " -> ");
                    fc = parentTitle + fc;
                });
                if (fc.length > 150)
                    fc = fc.substring(0, 75) + '...' + fc.substring(fc.length - 75);
                if (maxCols < fc.length)
                    maxCols = fc.length;
                txt += fc + "\r\n";
            });
            ids = selected_ids.join(';');
            currBocrud.closeModal($(this), dialogParent, false);
            $('#' + uniqueId).val(ids);
            $('#' + uniqueId).change();
            var $sel = $('#' + uniqueId + 'sel');
            $sel.attr("rows", nods.length > 0 ? nods.length : 1);
            if (maxCols < 20)
                maxCols = 25;
            if ($sel.attr('fix') != 'true')
                $sel.attr("cols", maxCols);
            if (txt.length > 0)
                $sel.val(txt.substr(0, txt.length - 2));
            else
                $sel.val('');
            $('#' + uniqueId + 'LookupBtn').button('enable');
        };
        buttons[$.jgrid.edit.bCancel] = function () {
            currBocrud.closeModal($(this), dialogParent, false);
            $('#' + uniqueId + 'LookupBtn').button('enable');
        };

        dialog = currBocrud.openModal($('#' + uniqueId + 'dialog'), {
            title: $.bocrud.select.header,
            buttons: buttons,
            destroyOnClose: true,
            dialogParent: dialogParent,
            open: function () {
                var selectedNodes = $('#' + uniqueId).val();
                if (selectedNodes && selectedNodes.length > 0) {
                    var selectedNodesArr = selectedNodes.split(";");
                    var tree = $('#' + uniqueId + "tree");
                    selectedNodesArr.forEach(function (item) {
                        tree.jstree('check_node', '#' + item);
                    });
                }
            }
        });
        $('#' + uniqueId + 'LookupBtn').button('disable');
    }


    this.treeDdlIniter = function (uid) {
        $('.fg-button').hover(
            function () {
                $(this).removeClass('ui-state-default').addClass('ui-state-focus');
            },
            function () {
                $(this).removeClass('ui-state-focus').addClass('ui-state-default');
            });

        $('#' + uid + 'a').fgmenu({
            content: function (caller) {
                return $('#' + uid + 'ul').html();
            },
            flyOut: false,
            backLink: false,
            crumbDefaultText: $.bocrud.select.header + ':',
            topLinkText: $.bocrud.select.show_all,
            positionOpts: {
                posX: 'left',
                posY: 'bottom',
                offsetX: 0,
                offsetY: 0,
                directionH: 'right',
                directionV: 'down',
                detectH: false,
                detectV: false,
                linkToFront: false
            },
            onClick: function (item) {
                var caption = $(item).html();
                var key = $(item).attr('key');
                if (key != '-1') {
                    $('#' + uid).val(key);
                    $('#' + uid + 'a').html(caption);
                } else {
                    $('#' + uid).val('');
                    $('#' + uid + 'a').html('');
                }
            }
        });
    }

    this.relationalDdlIniter = function (uniqueId, multiple, selectedList) {
        var msObj = currBocrud.$('#' + uniqueId + 'sel');
        msObj.ddlms({
            checkAllText: $.bocrud.select.checkAllText,
            uncheckAllText: $.bocrud.select.uncheckAllText,
            noneSelectedText: $.bocrud.select.noneSelectedText,
            selectedText: $.bocrud.select.selectedText,
            multiple: multiple,
            selectedList: selectedList,
            click: function (event, ui) {
                var si = msObj.ddlms("getChecked").map(function () {
                    return this.value;
                }).get();
                var siVal = "";
                if (si.length > 0) {
                    for (var i = 0; i < si.length - 1; ++i)
                        siVal += si[i] + ";";
                    siVal += si[si.length - 1];
                }
                $('#' + uniqueId).val(siVal);
                msObj.change();
            }
        });
        msObj.change();
    }

    this.init_inputfile = function (uid) {

        currBocrud.$("#" + uid).position({
            of: currBocrud.$("#" + uid + "btn"),
            offset: "60 0"
        }).hover(
            function () {
                {
                    currBocrud.$('#' + uid + 'btn').addClass('ui-state-hover');
                }
            },
            function () { { currBocrud.$('#' + uid + 'btn').removeClass('ui-state-hover'); } }
        );
    }
    this.single_show = function (c, mode, place_holder, dontRegValidation) {
        var ph = $('#' + place_holder);
        var content = c.content.replace(RegExp('textareaa', 'gi'), 'textarea');
        var formObj = eval('(' + content + ')');

        formObj.ignore_form_tag = true;

        ph.data('formObj', formObj);
        currBocrud.show_form(formObj, mode, formObj.xml, '#' + place_holder);
        if (formObj.detailPos == 'up') // اگر میخواد پنهان نشه باید موقعیت رو پایین قرار بده
            ph.hide();

        if (dontRegValidation !== true) {
            if (formObj.validationObj && !jQuery.isEmptyObject(formObj.validationObj.rules)) {

                var closest_form = ph.closest('form');

                var validator = closest_form.data('validator');

                if (!validator && closest_form.data('formObj') && closest_form.data('formObj').validationObj) {
                    closest_form.removeClass('ignore-validation');
                    closest_form.validate(closest_form.data('formObj').validationObj);
                    validator = closest_form.data('validator');
                }

                if (validator) {

                    validationObj = $.extend({
                        rules: {
                        },
                        messages: {}
                    }, formObj.validationObj);

                    var vs = validator.settings;

                    if (vs.messages && validationObj)
                        for (var key in validationObj.messages)
                            vs.messages[key] = validationObj.messages[key];

                    if (vs.rules && validationObj)
                        for (var key in validationObj.rules)
                            vs.rules[key] = validationObj.rules[key];

                }

            }
        }

        formObj.groups.splice(0, formObj.groups.length);
        formObj.groups = null;
        $.bocrud.decorateUi(ph);
        currBocrud.runJs('oncontrolplaced', ph);
        currBocrud.runJs('oneverycontrolplaced', ph);
    }
    this.single_onclick = function (uniqueid, width, height, title, expand) {
        var buttons = {
        };
        var form_id = uniqueid + '-div';
        var originalDivId = '#' + form_id;
        var orgDiv = $(originalDivId);
        var dialogParent = orgDiv.parent();
        var valField = $('#' + uniqueid);

        buttons[$.jgrid.edit.bClose] = function () {
            currBocrud.closeModal($(this));
        };

        var tempForm = $('#' + form_id + '-tempform');
        if (tempForm.length == 0) {
            $('body').append("<form id='" + form_id + "-tempform' style='direction:rtl'></form>");
            tempForm = $('#' + form_id + '-tempform');
        }
        orgDiv.appendTo(tempForm);
        orgDiv.css('display', 'block');

        var validationObj = null;
        if (orgDiv.data('validationObj')) {
            validationObj = orgDiv.data('validationObj');
            tempForm.validate(validationObj);
        }

        currBocrud.openModal(tempForm, {
            buttons: buttons,
            title: title,
            width: width > 0 ? width : 400,
            height: height > 0 ? height : 300,
            dialogParent: dialogParent,
            destroyOnClose: true,
            autoOpen: true,
            close: function (event, ui) {

                if (!$(this).valid()) {
                    valField.val('');
                } else {
                    valField.val('true');
                }
                orgDiv.css('display', 'none');
            },
            open: function () {
                if (expand)
                    tempForm.dialogExtend("maximize");
            }
        });
    }


    this.autocomplete = function (t, ticket, binddata) {
        t.autocomplete({
            close: function () {
                t.focus();
            },
            open: function () {
                t.focus();
            },
            source: function (term, callback) {

                currBocrud.pcall(t, ticket, term.term, function (response) {

                    callback(eval(response));

                }, {
                    global: false
                }, false, binddata, true);
            },
            select: function (event, ui) {
                t.val(ui.item.value);
            }
        });
    }

    this.gridPopupClick = function (xml, sdvs, key, method, is_editable, callback) {

        var params = {
            xml: xml,
            container_id: currBocrud.config.id + "gpc",
            getData: {
                dvs: sdvs, id: key, mode: is_editable ? 'DataEntry' : 'View'
            },
            submitData: {
                dvs: sdvs, id: key, action: method
            },
            onSubmitSucess: function () {
                callback(); currBocrud.reloadGridData();
            }
        };
        if (!is_editable)
            params.text = {
                bCancel: $.jgrid.edit.bClose
            };
        currBocrud.showDialog(params);
    }

    this.captcha_refresh = function (sender, ticket) {
        var oncallback = function (response) {
            var container = $(sender).closest('.bocrud-captcha');
            var img = container.find('img');
            img.attr('src', response.path);
            container.find(':hidden').val(response.hash);
        };
        this.pcall(sender, ticket, {}, oncallback, { dataType: "json" }, false, false, false);
    }

    this.showDialog = function (params) {

        var p = $.extend({
            xml: params.xml,
            jsbid: currBocrud.config.id,
            container_id: '',
            text: {
                bCancel: $.jgrid.edit.bCancel,
                bSubmit: $.jgrid.edit.bSubmit
            },
            submitData: {
            },
            getData: {
            },
            submitUrl: currBocrud.config.urlPrefix + "/Save",
            bocrud: currBocrud,
            onSubmitSucess: null,
            onPreSubmit: null,
            onSubmitError: null,
            onCancel: null,
            onShowError: null,
            modal: true,
            onDialogOpen: null,
            buttons: {}
        }, params || {
        });

        p.getData = $.extend({
            xml: p.xml,
            mode: 'DataEntry',
            jsbid: p.jsbid
        }, p.getData || {
        });

        p.submitData = $.extend({
            xml: p.xml,
            mode: 'DataEntry',
            jsbid: p.jsbid,
            genjson: false
        }, p.submitData || {
        });

        var cb = p.bocrud;

        cb.getData("GetDetails", p.getData, {
            msg: $.bocrud.msg.load_window_data,
            success: function (serverResponse) {

                var formObj = eval('(' + serverResponse + ')');

                formObj.id = p.container_id;
                formObj.mode = p.getData.mode;
                //formObj.detPos = 'NormalDialog';

                if (formObj.error) {
                    if ($.isFunction(p.onShowError))
                        p.onShowError(formObj);

                    currBocrud.showError(formObj);

                    return;
                }

                var fcid = p.container_id + '_container';
                var fc = $("#" + fcid);
                if (fc.length == 0) {
                    $('body').append("<div id='" + fcid + "' ></div>");
                    fc = $("#" + fcid);
                }

                var buttons = p.buttons;

                if (p.text.bSubmit && p.getData.mode == 'DataEntry' && !buttons[p.text.bSubmit])
                    buttons[p.text.bSubmit] = function () {

                        var $form = $('#' + formObj.id);

                        currBocrud.validate($form, function (b, vresult) {

                            if (!vresult) return;

                            if ($.isFunction(p.onPreSubmit))
                                if (p.onPreSubmit($form) === false)
                                    return;

                            var formObj = $form.data('formObj');

                            if (p.submitUrl)
                                $form.ajaxSubmit({
                                    showLoading: true,
                                    msg: $.bocrud.msg.post_window_data + ' ' + formObj.title,
                                    data: p.submitData,
                                    url: p.submitUrl,
                                    type: "POST",
                                    datatype: 'text',
                                    semantic: true,
                                    error: function (jqXHR, textStatus, errorThrown) {


                                        if ($.isFunction(p.onSubmitError))
                                            p.onSubmitError(arguments);
                                        else {
                                            var eobj = null;
                                            try {
                                                eobj = eval('(' + jqXHR.responseText + ')');
                                            }
                                            catch (e) {
                                                console.error('bocrud 4302: ', ex);
                                            }
                                            if (eobj.error) {
                                                currBocrud.showError(eobj);
                                            }
                                        }

                                        var formObj = $form.data('formObj');
                                        currBocrud.raiseEvent('onsubmiterror', formObj, p.submitData, eobj);
                                    },
                                    success: function (data) {

                                        var strSaveResult = '';
                                        if (data.length > 0) {
                                            var sixchar = data.substring(0, 6);
                                            if (sixchar === "base64")
                                                strSaveResult = Base64.decode(data.substring(6));
                                            else
                                                if (sixchar == "<pre>{")
                                                    strSaveResult = data.substring(5, data.length - 6);
                                                else
                                                    strSaveResult = data;
                                        }

                                        if (!strSaveResult || strSaveResult.length == 0) {
                                            if ($.isFunction(p.onSubmitError))
                                                p.onSubmitError(data);
                                        }

                                        var json = null;
                                        try {
                                            json = eval('(' + strSaveResult + ')');
                                        }
                                        catch (ex) {
                                            try {
                                                json = eval(strSaveResult);
                                            }
                                            catch (ex) {
                                            }
                                        }

                                        var close = true, boAsJson;
                                        if (json && json.boAsJson)
                                            boAsJson = eval('(' + json.boAsJson + ')');
                                        if ($.isFunction(p.onSubmitSucess))
                                            close = p.onSubmitSucess(data, $form, json, boAsJson, true);
                                        var formObj = $form.data('formObj');
                                        if (!json || !json.error)
                                            currBocrud.raiseEvent('onsubmitsuccess', formObj, json, false);

                                        //if (close == undefined || close == true) {
                                        //    //currBocrud.closeModal($form, $('body'), true);
                                        //    $form.remove();
                                        //}
                                    }
                                });
                            else {
                                //currBocrud.closeModal($form);
                                if ($.isFunction(p.onSubmitSucess)) {
                                    var data = {
                                    };
                                    $form.find('.bocrud-control').each(function () {
                                        var e = $(this);
                                        var name = e.attr('name');
                                        var id = e.attr('id').substring(7);
                                        var input = e.find('#' + id);
                                        var val = input.val();
                                        if (input.is(':checkbox'))
                                            val = input.is(':checked');
                                        data[name] = val;
                                    });
                                    p.onSubmitSucess($form, data);
                                }

                            }

                        });
                        //if (formObj.validationObj &&
                        //	!$.isEmptyObject(formObj.validationObj.rules) &&
                        //	$form.valid && !$form.valid()) return;


                    }

                if (!buttons[p.text.bCancel])
                    buttons[p.text.bCancel] = function () {
                        if ($.isFunction(p.onCancel))
                            p.onCancel();
                        //currBocrud.closeModal($form, $('body'), true);
                        //$form.remove();
                    }
                formObj.detailPos = "normaldialog";
                currBocrud.raiseEvent('onshowcontent', formObj, buttons, 'DataEntry', fc);
                $.bocrud.reg_form(formObj);

                //var $form = formObj.id ? $('#' + formObj.id) : $('.bocrud-page-form[uid="' + formObj.uid + '"]');

                //currBocrud.openModal($form, {
                //    buttons: buttons,
                //    title: formObj.title,
                //    width: formObj.width > 0 ? formObj.width : 400,
                //    height: formObj.height > 0 ? formObj.height : 300,
                //    closeOnEscape: true,
                //    draggable: true,
                //    modal: p.modal,
                //    open: p.onDialogOpen,
                //    close: function () {
                //        //var dialog = $(this).closest('.bocrud-dialog');
                //        //currBocrud.closeModal(dialog, null, true);
                //        $(this).remove();
                //    },
                //    dialogParent: null,
                //    destroyOnClose: true
                //});
            }
        });
    }

    this.serializeFilter = function (uid) {

        var g = currBocrud.grid();
        $('#' + uid).val(g.jqGrid('getGridParam', 'postData').filters);

    }

    this.changeDetector = function (id) {
        var onChange = function () {
            $('#' + id).val('true');
        };

        var root = $('#' + id).closest('.bocrud-page-form');

        var hh = function () {
            root.find(':input[type=hidden]').filter(':not(.bocrud-watched,.cbox,:button)').addClass('bocrud-watched').bind('DOMSubtreeModified', onChange);
            root.find(':input,:file').filter(':not(.bocrud-watched,.cbox,:button)').addClass('bocrud-watched').change(onChange);
        }

        root.bind('DOMSubtreeModified', function () {
            hh();
        });
        hh();
    }

    this.init_template = function (uid, jsbid, page_size) {
        var me = $('#' + uid + '-holder');
        var ticket = me.find('.template-body:first').data('ticket');

        var edit = function (response, index, key, onSuccess, onCanceled) {
            var row_container = me.find('.template-feed:first');

            var new_row = $('<div class="template-row clearfix template-editable"><div></div><div class="action-buttons"></div></div>');

            if (index == 0)
                new_row.prependTo(row_container);
            else
                row_container.children(':nth-child(' + index + ')').after(new_row);

            var nrow = row_container.find('.template-editable>div:first');

            var formObj = eval('(' + response + ')');
            formObj.detailPos = 'up';

            currBocrud.show_form(formObj, 'DataEntry', currBocrud.config.xml, nrow, null);

            var tools = nrow.next();
            tools.append('<a href="javascript:void(0)" class="green save"><i class="ace-icon icon-check bigger-125"></i></a>')
                .append('<a href="javascript:void(0)" class="red cancel"><i class="ace-icon icon-times bigger-125"></i></a>');

            $.bocrud.reg_form(formObj);

            tools.find('.save').click(function () {
                var form = row_container.find('.bocrud-page-form:first');
                submit(form, {
                }, key, true, false, null, function (rawdata, saveResult) {

                    currBocrud.pcall(me, ticket, {
                        action: 'get-specific', boKey: saveResult.boKey
                    }, function (response) {

                        nrow.closest('.template-row').hide();

                        var index = nrow.closest('.template-row').index();
                        if (index < 1)
                            row_container.prepend(response);
                        else
                            row_container.children(':nth-child(' + index + ')').after(response);

                        var recently_added;
                        if (index < 1)
                            recently_added = row_container.find('.template-row:first');
                        else
                            recently_added = row_container.find('.template-row:nth-child(' + (index + 1) + ')');

                        window.bocruds[jsbid].runJs('oncontrolplaced', recently_added);
                        window.bocruds[jsbid].runJs('oneverycontrolplaced', recently_added);
                        $.bocrud.decorateUi(recently_added);

                        bind_action_buttons();

                        nrow.closest('.template-row').remove();

                        if ($.isFunction(onSuccess)) onSuccess();

                    }, {
                    }, false, false, false);

                }, false);
            });
            tools.find('.cancel').click(function () {
                nrow.closest('.template-row').remove();
                if ($.isFunction(onCanceled)) onCanceled();
            });
        };

        var bind_action_buttons = function () {
            me.find('.template-body .template-feed:first .action-buttons a').filter(':not(.templete-event-bound)').each(function () {
                var action = $(this).data('action');
                if (action == 'edit') {
                    $(this).addClass('templete-event-bound').click(function () {
                        var row = $(this).closest('.template-row');
                        var key = row.data('id');
                        window.bocruds[jsbid].pcall(me, ticket, {
                            action: 'edit',
                            boKey: key
                        }, function (response) {

                            row.hide();
                            var index = row.index();

                            edit(response, index, key, function () {
                                row.remove();
                            }, function () {
                                row.show();
                            });

                        }, {
                        }, false, false, false, 'DataEntry');
                    });
                }
                if (action == 'delete') {
                    $(this).addClass('templete-event-bound').click(function () {
                        var row = $(this).closest('.template-row');
                        var key = row.data('id');
                        currBocrud.do_del(key, null, function () {
                            row.remove();
                        });
                    });
                }
            });
        };

        var bind_event = function () {
            var toolbar = me.find('.widget-toolbar:first');
            var newbtn = toolbar.find('[data-action=new]');
            newbtn.click(function () {
                window.bocruds[jsbid].pcall(me, ticket, {
                    action: 'new'
                }, function (response) {

                    edit(response, 0);

                }, {
                }, false, false, false, 'DataEntry');
            });

            var refreshbtn = toolbar.find('[data-action=reload]');
            refreshbtn.click(function () {
                var cp = me.data('page') || 1;
                page(cp);
            });

            me.find('.template-body>.modal-footer>.pagination a').each(function () {
                var action = $(this).data('action');
                var mp = me.data('max_page');
                if (!mp) {
                    mp = me.find('.template-body>.modal-footer>.pagination li:not(.next,.back)').length;
                    me.data('max_page', max_page);
                }
                var max_page = parseInt(me.data('max_page'), 10);
                var cp = me.data('page');
                if (!cp) {
                    cp = 1;
                }
                var next_page = 0;
                if (action == 'prev-page') {
                    if (cp == 1) {
                        $(this).parent().addClass('disabled');
                        return;
                    } else
                        $(this).click(function () {
                            if ($(this).parent().is('.disabled')) return;
                            var cp = me.data('page');
                            page(cp - 1);
                        });
                }
                else if (action == 'next-page') {
                    if (cp == max_page) {
                        $(this).parent().addClass('disabled');
                        return;
                    } else
                        $(this).click(function () {
                            if ($(this).parent().is('.disabled')) return;
                            var cp = me.data('page');
                            page(cp + 1);
                        });
                }
                else {
                    if (cp == action) {
                        $(this).parent().addClass('disabled');
                        return;
                    } else {
                        $(this).click(function () {
                            var action = $(this).data('action');
                            page(action);
                        });
                    }
                }

            });

            bind_action_buttons();
        };

        var page = function (page) {

            me.data('page', page);

            window.bocruds[jsbid].pcall(me, ticket, {
                action: 'refresh',
                page: page,
                page_size: page_size
            }, function (response) {
                var body = me.find('.template-body:first');
                var p = body.parent();
                body.remove();
                p.prepend(response);
                body = me.find('.template-body:first');
                window.bocruds[jsbid].runJs('oncontrolplaced', body);
                window.bocruds[jsbid].runJs('oneverycontrolplaced', body);
                var max_page = me.find('.template-body>.modal-footer>.pagination li:not(.next,.prev)').length;
                me.data('max_page', max_page);
                bind_event();
            }, {
            }, false, false, false, 'DataEntry');
        };

        me.find('.template-body>.modal-footer>.pagination li.prev').addClass('disabled');

        bind_event();

        var max_page = me.find('.template-body>.modal-footer>.pagination li:not(.next,.prev)').length;
        if (max_page < 2) {
            me.find('.template-body>.modal-footer>.pagination li.next').addClass('disabled');
        }
    }
}

// BOCRUD_END //////////////////////////////////////////////////

