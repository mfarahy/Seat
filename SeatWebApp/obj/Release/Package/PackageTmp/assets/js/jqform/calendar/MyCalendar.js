
; (function ($) {

    $.myCalendar = $.myCalendar || {};
    $.extend($.myCalendar, {
        init: function (options) {
            var o = options || {};

            $(this).each(function () {
                var txt = $(this);
                var p = txt.parent();
                var isTxt = txt.is(':input');

                var c = p.find('.bocrud-mycalendar:first');
                if (c.length == 1)
                    return;

                var afterShow = o.afterShow;

                txt.dblclick(function () {
                    $(this).val('').trigger('change');
                })
                .click(function () {
                    var real_txt = $(this).data('datepicker');
                    if (real_txt.input.get(0) != $(this).get(0))
                        console.log('yes');
                    //$(this).focus();
                }).change(function () {
                    var id = $(this).attr('id');
                    var cid = '#' + id + "-culture";
                    switch ($(this).attr('culture')) {
                        case 'Persian':
                            $(cid).val('fa-IR');
                            break;
                        case 'Gregorian':
                            $(cid).val('en-US');
                            break;
                        default:
                            break;
                    }
                });

                var zindex, position;
                if (isTxt) {
                    txt.data('zindex', txt.css('z-index'));
                    txt.data('position', txt.css('position'));

                    var close = o.onClose;
                    o.onClose = function () {
                        if ($.isFunction(close))
                            close.apply(this);
                        txt.css({
                            'position': txt.data('position'),
                            'z-index': txt.data('zindex')
                        });
                    };
                }

                var f;
                o.afterShow = function (txt, inst) {

                    if (isTxt) {
                        inst.input.css({
                            'position': 'relative',
                            'z-index': '10000'
                        });
                    }

                    f = function () {
                        var c = inst.dpDiv;

                        var header = c.find('.ui-datepicker-header');
                        if (header.length == 0) {
                            setTimeout(f, 100);
                            return;
                        }

                        if (o.multiCulture) {
                            var isNew = false;
                            if (c.find('.ace-switch:first').length == 0) {
                                c.addClass('bocrud-mycalendar');
                                c.prepend('<label><input name="switch-field-1" class="ace ace-switch ace-switch-6" type="checkbox" /><span class="lbl"></span></label>');
                                isNew = true;
                            }
                            var chkbx = c.find('input.ace-switch');
                            var oreg = inst.input.attr('culture');
                            if (oreg == 'Persian')
                                chkbx.attr('checked', 'checked');
                            else
                                chkbx.removeAttr('checked');

                            if (isNew) {
                                chkbx.click(function () {
                                    var reg = inst.input.attr('culture');
                                    var newReg = null;
                                    if (reg == 'Persian') {
                                        newReg = $.datepicker.regional[''];
                                        inst.input.data('regional', 'en');

                                        inst.input.attr('culture', 'Gregorian');
                                    } else {
                                        newReg = $.datepicker.regional['fa'];
                                        inst.input.data('regional', 'fa');
                                        inst.input.attr('culture', 'Persian');
                                    }

                                    if (o.fdefaultDate) {
                                        if (reg == 'Persian') {
                                            o.defaultDate = o.fdefaultDate;
                                            o.minDate = o.fminDate;
                                            o.maxDate = o.fmaxDate;
                                        } else {
                                            o.defaultDate = o.fdefaultDate.getGregorianDate();
                                            o.minDate = o.fminDate.getGregorianDate();
                                            o.maxDate = o.fmaxDate.getGregorianDate();
                                        }
                                    }

                                    if (!JalaliDate.prototype.getMicroseconds) {
                                        JalaliDate.prototype.microseconds = 0;
                                        JalaliDate.prototype.getMicroseconds = function () { return this.microseconds; };
                                        JalaliDate.prototype.setMicroseconds = function (m) {
                                            this.setMilliseconds(this.getMilliseconds() + Math.floor(m / 1000));
                                            this.microseconds = m % 1000;
                                            return this;
                                        };
                                    }


                                    inst.input.datepicker('option', $.extend(newReg, o));

                                    if (o.onCultureChanged)
                                        o.onCultureChanged.apply(inst.input);


                                    inst.input.datepicker('show');


                                    setTimeout(f, 100);
                                });
                            }
                        }


                    };
                    setTimeout(f, 100);

                    if (afterShow)
                        afterShow.apply(inst.input);
                };
                o.onChangeMonthYear = function (y, m, inst) {
                    setTimeout(function () {
                        o.afterShow(txt, inst);
                    }, 100);
                };

                var beforeShow = o.beforeShow;

                o.beforeShow = function (txt, inst) {
                    var reg = inst.input.attr('culture');
                    if (!inst.settings.orgYearRange) {
                        inst.settings.orgYearRange = inst.settings.yearRange;
                        inst.settings.orgCulture = reg;
                    }
                    var yearRange = inst.settings.orgYearRange;
                    if (yearRange && yearRange.length > 0) {
                        var frags = yearRange.split(':');
                        var faStartYear = parseInt(frags[0]), faEndYear = parseInt(frags[1]);
                        var enStartYear = parseInt(frags[0]), enEndYear = parseInt(frags[1]);

                        if (inst.settings.orgCulture == 'Persian') {
                            enStartYear = new JalaliDate(faStartYear, 1, 1).getGregorianDate().getFullYear();
                            enEndYear = new JalaliDate(faEndYear, 1, 1).getGregorianDate().getFullYear();
                        } else {
                            faStartYear = jd_to_persian(gregorian_to_jd(enStartYear, 1, 1))[0];
                            faEndYear = jd_to_persian(gregorian_to_jd(enEndYear, 1, 1))[0];
                        }

                        var reg = inst.input.attr('culture');
                        if (reg == 'Persian') {
                            inst.settings.yearRange = faStartYear + ':' + faEndYear;
                        } else {
                            inst.settings.yearRange = enStartYear + ':' + enEndYear;
                        }
                    }
                };

                if (o.multiCulture) {
                    if (o.regional == 'fa') {
                        o.defaultDate = o.fdefaultDate;
                        o.minDate = o.fminDate;
                        o.maxDate = o.fmaxDate;
                    } else {
                        if (o.fdefaultDate) {
                            o.defaultDate = o.fdefaultDate.getGregorianDate();
                            o.minDate = o.fminDate.getGregorianDate();
                            o.maxDate = o.fmaxDate.getGregorianDate();
                        }
                    }
                } // if (o.multiCulture)

                o.closeText = 'بستن';
                o.currentText = 'امروز';
                o.timeText = 'زمان';
                o.hourText = 'ساعت';
                o.minuteText = 'دقیقه';

                if (o.showTime)
                    txt.datetimepicker(o, {
                        timeFormat: "HH:mm",
                        isRTL: true,
                        showSecond: false,
                        showMillisec: false,
                        showMicrosec: false,
                        showTimezone: false,
                        showTime: false,
                        timeText: 'زمان',
                        hourText: 'ساعت',
                        minuteText: 'دقیقه'
                    });
                else
                    txt.datepicker(o);

                txt.click(function () {
                    try {
                        txt.datetimepicker('show');
                    }
                    catch (e) { }
                });

                //if (txt.attr('disabled') != 'disabled')
                //    txt.mask(o.showTime ? '0000/00/00 00:00' : '0000/00/00', {
                //        placeholder: o.showTime ? "__:__ __/__/____" : "__/__/____",
                //        translation: {
                //            'Z': {
                //                pattern: /[0-9]/, optional: true
                //            },
                //            'H': {
                //                pattern: /[012]/, optional: true
                //            },
                //            'M': {
                //                pattern: /[0123456]/, optional: true
                //            },
                //            'Y': {
                //                pattern: /[12]/, optional: true
                //            }
                //        }
                //    });

                txt.attr('culture', o.regional == 'fa' ? 'Persian' : 'Gregorian');
            });//each


        } // init

    });

    $.fn.extend({

        //This is where you write your plugin's name
        myCalendar: function (pin) {
            var _this = this;

            if (typeof pin == 'string') {
                var fn = $.myCalendar[pin];
                if (!fn) {
                    throw ("myCalendar - No such method: " + pin);
                }
                var args = $.makeArray(arguments).slice(1);
                return fn.apply(this, args);
            } else if (typeof pin === 'object' || !pin) {
                return $.myCalendar.init.apply(this, arguments);
            }
            else {
                $.error('Method ' + pin + ' does not exist on jQuery.myCalendar');
            }
        }
    });
})(jQuery);