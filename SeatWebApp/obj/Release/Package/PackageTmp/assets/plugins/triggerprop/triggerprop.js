
; (function ($) {

    $.triggerprop = $.triggerprop || {};
    $.extend($.triggerprop, {

        init: function (options) {
            var o = $.extend({
                jsbid: '',
                once: true,
                icon: '',
                type: '',
                method: '',
                oncallsuccess: null,
                text: '',
                ctx: '',
                css: '',
                databind: true,
                reloadGrid: false
            }, options || {});

            this.each(function () {

                

                var t = $(this);

                if (t.data('triggerprop'))
                    return;

                t.data('triggerprop', o);

                var opt = {};

                if (o.icon && o.icon.length > 0)
                    opt.icon = o.icon;
                if (o.text)
                    opt.text = o.text;

                var css = o.icon;
                if (!o.text || o.text.length == 0)
                    css += " icon-only";

                t.prepend('<i class="' + css + '"></i>')
                    .addClass(o.css)
                    .click(function () {

                        if (o.confirmation && o.confirmation.length > 0 && !confirm(o.confirmation))
                            return;

                        var id = '';
                        var closestForm = $(this).closest('.bocrud-page-form,tr.jqgrow');
                        if (closestForm.is('tr'))
                            id = closestForm.attr('id');
                        else
                            id = closestForm.data('formObj').boKey;

                        var oncomplete = function (call_result) {
                            if ($.isFunction(o.oncallsuccess)) {
                                o.oncallsuccess();
                            }

                            if ($.isFunction(o.checkcondition))
                                o.checkcondition(call_result);
                        }

                        if (o.method && o.method.length > 0) {
                            if (o.type == "Action") {
                                // selrows, grid, ctx, oncomplete, showAsMain, bctor
                                window.bocruds[o.jsbid].userCmd(id, window.bocruds[o.jsbid].grid(), o.ctx, oncomplete, o.showAsMain, o.beforeCtor, o.method, null, false, o.reloadGrid, o.passColumnsData);                                                            
                                return;
                            }

                            if (o.type == 'System') {
                                if (o.method == "Update")
                                    window.bocruds[o.jsbid].do_edit(id, window.bocruds[o.jsbid].grid(), o.ctx, oncomplete);
                                if (o.method == "Get")
                                    window.bocruds[o.jsbid].do_view(id, window.bocruds[o.jsbid].grid(), o.ctx, oncomplete);
                                if (o.method == "Delete")
                                    window.bocruds[o.jsbid].do_del(id, window.bocruds[o.jsbid].grid(), o.ctx, oncomplete);
                                return;
                            }


                            var ticket = $(this).attr('ticket');

                            window.bocruds[o.jsbid].pcall($(this), ticket, {}, function (data) {

                                var result = eval('(' + data + ')');

                                var buttons = {};
                                buttons['Ok'] = function () {
                                    $(this).dialog('close');
                                };

                                if (result.error)
                                    return;

                                if (result.message && result.message.length > 0) {
                                    if (result.message.startsWith("javascript:")) {
                                        eval(result.message.substring(11));
                                        return;
                                    } else {
                                        $('<div/>')
                                            .text(result.message)
                                            .dialog({
                                                buttons: buttons,
                                                modal: true,
                                                zIndex: 1040
                                            });
                                    }
                                }

                                if (o.once) {
                                    t.attr('disabled', 'disabled');
                                    t.addClass('bucrud-disabled-action');
                                    t.addClass('disabled ui-state-disabled');
                                    t.button({ disabled: true });
                                }

                                oncomplete(result);

                            }, {}, o.validate, o.databind);
                        } else {
                            if (o.validate) {
                                var form = $(this).closest(".bocrud-page-form,.jqgrow");

                                form.find('form[rel]').each(function () {
                                    if ($(this).valid)
                                        if (!$(this).valid()) {
                                            isValid = false;
                                            invalidFormTitles += $(this).attr('title');
                                        }
                                });

                                if (form.valid && !form.valid()) return;

                            }

                            if (o.once) {
                                t.attr('disabled', 'disabled');
                                t.addClass('bucrud-disabled-action');
                                t.addClass('disabled ui-state-disabled');
                                t.button({ disabled: true });
                            }

                            
                            if (o.type == 'Submit') {
                                var form = $(this).closest(".bocrud-page-form,.jqgrow");
                                form.submit();
                            }

                            oncomplete();
                        }

                        return true;
                    });

                t.tooltip();

            }); // each
        }
    });

    $.fn.extend({

        //This is where you write your plugin's name
        triggerprop: function (pin) {
            var _this = this;

            if (typeof pin == 'string') {
                var fn = $.triggerprop[pin];
                if (!fn) {
                    throw ("triggerprop - No such method: " + pin);
                }
                var args = $.makeArray(arguments).slice(1);
                return fn.apply(this, args);
            } else if (typeof pin === 'object' || !pin) {
                return $.triggerprop.init.apply(this, arguments);
            }
            else {
                $.error('Method ' + pin + ' does not exist on jQuery.triggerprop');
            }
        }
    });
})(jQuery);