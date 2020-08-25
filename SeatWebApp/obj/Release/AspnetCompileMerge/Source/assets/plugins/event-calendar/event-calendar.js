
; (function ($) {

    $.eventCalendar = $.eventCalendar || {};
    $.extend($.eventCalendar, {
        _onDateSelect: function (date, inst, id) {

            if (!inst.dpDiv && inst.data && inst.data('datepicker'))
                inst = inst.data('datepicker');

            var data = $(this).data('eventCalendar');
            if (!data) return;
            if (!data.editable && !id) return;

            if ($.eventCalendar.lock) return;
            $.eventCalendar.lock = true;

            var day = parseInt(inst.selectedDay);
            if (isNaN(day) && !id) {
                return;
            }
            var t = this;
            var b = window.bocruds[data.jsbid];

            var userdata = {
                date: inst.selectedYear + "/" + (inst.selectedMonth + 1) + "/" + inst.selectedDay,
                culture: $(this).attr('culture'),
                id: id
            };

            var _formObj = function () {
                var form = $('#f' + data.jsbid);//fevca-p627535671
                return form.data("formObj");
            }

            var closeDialog = function () {
                var formObj = _formObj();
                var fc = $('#' + formObj.id).parent();
                var dialog = fc.closest('.dialog');
                if (dialog.length == 0) {
                    dialog = fc;
                    while (!dialog.data('dialog')) {
                        dialog = dialog.parent();
                    }
                }

                b.closeModal(dialog);
            }

            var buttons = [];
            if (data.editable) {
                buttons[$.jgrid.edit.bSubmit] = function (preventLoadView, xdata) {

                    var sdata = {
                        xml: data.xml,
                        jsbid: data.jsbid,
                        id: id,
                        tempMode: false,
                        parentkey: null,
                        sel_cat: null,
                        mode: 'DataEntry'
                    };
                    if (!id)
                        sdata['@' + userdata.datep] = userdata.date;

                    b.save(id, true, data.xml, false, sdata, 'f' + data.jsbid, function () {
                        $.eventCalendar.lock = false;
                        closeDialog();
                        $.eventCalendar._onRefresh.apply(t);
                    });
                }
            }

            buttons[$.jgrid.edit.bClose] = function (preventLoadView) {
                $.eventCalendar.lock = false;
                // closeDialog();
            }


            if (id) {
                buttons[$.jgrid.del.caption] = function (preventLoadView) {
                    $.eventCalendar.lock = false;
                    if (confirm($.jgrid.del.msg)) {
                        b.do_del(id, null, function () {

                            //closeDialog();
                            $(t).find('li[data-id=' + id + ']').remove();

                        }, true);
                    }
                };
            }

            var sdata = {};
            if (!id)
                sdata['@' + data.datep] = userdata.date;

            b.get_form(id, 'DataEntry', data.xml, null, buttons, sdata);

            //sender, ticket, userdata, callback, ajaxObj, validate, databind, background
            //b.pcall($(this), data.createTicket, userdata, callback, null, false, true, true);
        },
        _onRefresh: function () {

            var datepicker = $(this).data('datepicker');
            var month_names = $(this).datepicker("option", "monthNames");
            $(this).style('opacity', 0);

            $(this).find("tbody td").each(function () {
                var td = $(this);
                var day = parseInt(td.find('a').text());

                if (!isNaN(day)) {
                    var dtClass = "dt_" + datepicker.selectedYear + "_" + (datepicker.selectedMonth + 1) + "_" + day;
                    td.addClass(dtClass);
                    var caption = td.find('.caption');
                    if (caption.length == 0) {
                        td.append($('<div class="caption"/>'));
                        caption = td.find('.caption');
                    }
                    caption.text(day + ' ' + month_names[datepicker.selectedMonth]);
                }
            });

            var data = $(this).data('eventCalendar');
            var b = window.bocruds[data.jsbid];

            var t = this;

            var onDataFetched = function (serverData) {
                $(t).style('opacity', 1);

                if ($.eventCalendar.beginFetched) return;
                $.eventCalendar.beginFetched = true;
                var data = eval('(' + serverData + ')');
                for (var i = 0; i < data.length; ++i) {
                    var dt = data[i].Dt.split('T', 2)[0];
                    var frags = dt.split('-');
                    var c = ".dt_" + frags[0] + "_" + parseInt(frags[1]) + "_" + parseInt(frags[2]);
                    var ul = $(t).find(c).find('ul.event-items');
                    if (ul.length == 0) {
                        $('<ul class="event-items"/>')
                        .appendTo($(t).find(c));
                    }
                    ul.html("");
                }
                for (var i = 0; i < data.length; ++i) {
                    var dt = data[i].Dt.split('T', 2)[0];
                    var frags = dt.split('-');
                    var c = ".dt_" + frags[0] + "_" + parseInt(frags[1]) + "_" + parseInt(frags[2]);
                    var ul = $(t).find(c).find('ul.event-items');
                    var li = $('<li class="event-item label label-sm" title="' + (data[i].Tooltip || "") + '" />')
                     .text(data[i].Text)
                         .attr("data-id", data[i].Id)
                         .click(function () {
                             $.eventCalendar._onDateSelect.apply(t, [null, t, $(this).attr('data-id')]);
                         })
                         .appendTo(ul);

                    var c = data[i].Color;
                    if (c) {
                        if (c.length > 0 && c[0] != '#')
                            c = '#' + c;

                        li.style("background-color", c, "important");
                    }
                    if (data[i].Tooltip) {
                    }
                }
                $.eventCalendar.beginFetched = false;
            };

            var userdata = {
                date: datepicker.selectedYear + "/" + (datepicker.selectedMonth + 1) + "/1",
                culture: $(this).attr('culture')
            };

            b.pcall($(this), data.queryTicket, userdata, onDataFetched, null, false, true);

        },
        init: function (options) {
            var o = options || {};

            $(this).each(function () {

                $(this)
                    .addClass("uie-event-calendar")
                    .data('eventCalendar', o)
                    .myCalendar({
                        hideIfNoPrevNext: true,
                        changeMonth: true,
                        changeYear: true,
                        onSelect: $.eventCalendar._onDateSelect,
                        afterShow: $.eventCalendar._onRefresh,
                        multiCulture: true,
                        showButtonPanel: true,
                        onCultureChanged: function () {
                            var dayNames = $(this).datepicker("option", "dayNames");
                            $(this).datepicker("option", "dayNamesMin", dayNames);
                        },
                        regional: 'fa'
                    });

                var dayNames = $(this).datepicker("option", "dayNames");
                $(this).datepicker("option", "dayNamesMin", dayNames);

            });//each


        } // init

    });

    $.fn.extend({

        //This is where you write your plugin's name
        eventCalendar: function (pin) {
            var _this = this;

            if (typeof pin == 'string') {
                var fn = $.eventCalendar[pin];
                if (!fn) {
                    throw ("eventCalendar - No such method: " + pin);
                }
                var args = $.makeArray(arguments).slice(1);
                return fn.apply(this, args);
            } else if (typeof pin === 'object' || !pin) {
                return $.eventCalendar.init.apply(this, arguments);
            }
            else {
                $.error('Method ' + pin + ' does not exist on jQuery.eventCalendar');
            }
        }
    });

    $.eventCalendar.lock = false;

})(jQuery);