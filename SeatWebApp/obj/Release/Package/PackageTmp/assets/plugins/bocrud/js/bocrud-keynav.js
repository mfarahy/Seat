
; (function ($) {

    $.keynav = $.keynav || {};
    $.extend($.keynav, {
        sex: '.bocrud-focusable:not([disabled],.select2-container):visible',
        forms: [],
        bs: [],
        init: function (options) {
            var o = $.extend({
                bocrud: '',
                uid: '',
                multi: true
            }, options || {});

            $.bocrud.globalEvents.push({
                type: 'onshowcontentcomplete',
                key: 'keynav',
                func: function (form, mode, container, xml) {
                    setTimeout(function () {
                        $.keynav._onshowcontentcomplete(form, mode, container, xml);
                    }, 1000);

                }
            });
            $.bocrud.globalEvents.push({
                type: 'modalclose',
                key: 'keynav',
                func: function (div, dialogParent, remove) {
                    $.keynav.forms.pop();
                }
            });
            $.bocrud.globalEvents.push({
                type: 'modalopen',
                key: 'keynav',
                func: function (div, o) {
                    $.keynav.forms.push(div);
                }
            });
            $.bocrud.globalEvents.push({
                type: 'onviewcancel',
                key: 'keynav',
                func: function (bokey) {
                    $.keynav.forms.pop();
                }
            });

            $(document).on('keypress.keynav', $.keynav._switch);

            $(document).on('keypress.keynav', '.bocrud-focusable', function (e) {

                if (e.keyCode != 13 && e.keyCode != 9)
                    return;

                if (e.keyCode == 13 && $(e.currentTarget).is('textarea')) return;

                var form = $('body');
                if ($.keynav.forms.length > 0) {
                    form = $.keynav.forms[$.keynav.forms.length - 1];
                } else {
                    form = $(e.target).closest('.bocrud-page-form');
                }
                if (e.keyCode == 13) {
                    var p = form.parent();
                    if (p.length == 0)
                        p = $('#' + form.attr('id')).parent();
                    if (p.is('.bocrud-search-form')) {
                        e.preventDefault();
                        var bid = form.attr('id').substring(7);
                        $('#' + bid + '-searchBtn').click();
                        return;
                    }
                }

                //get the next index of text input element
                var next_idx = form.find($.keynav.sex).index(this);
                next_idx++;

                //get number of text input element in a html document
                var tot_idx = form.find($.keynav.sex).length;

                if (e.keyCode == 13 && $(this).is('button'))
                    return;

                //enter button in ASCII code
                if (e.keyCode == 13 || e.keyCode == 9) {
                    if (tot_idx == next_idx) {
                        //go to the first text element if focused in the last text input element
                        form.find($.keynav.sex + ':eq(0)').focus();
                    }
                    else {
                        //go to the next text input element
                        form.find($.keynav.sex + ':eq(' + next_idx + ')').focus();
                    }

                    e.preventDefault();
                }
            });

            this.each(function () { // .bocrud-container
                var bid = $(this).attr('id').substring(2);
                if (bid) {
                    $.keynav.bs.push(bid);
                } else
                    console.log('bocrud not found !')
            }); //each 

        } // init
        , _switch: function (e) {
            var stop_default = false;
            if (e.ctrlKey) {
                switch (e.which) {
                    case 110: // n
                    case 78: // N
                        $.keynav._add_new();
                        stop_default = true;
                        break;
                }
            }

            //switch (e.keyCode) {
            //    case 13:
            //    case 9:
            //        $.keynav._move_next();
            //        stop_default = true;
            //        break;
            //}
            if (stop_default) {
                e.preventDefault();
            }
        } // _switch
        , _add_new: function () {
            for (var i = 0; i < this.bs.length; ++i) {
                window.bocruds[this.bs[i]].addNew();
            }
        }//_add_new
        , _onshowcontentcomplete: function (formObj, mode, container, xml) {

            if (formObj.searchForm) return;

            if (mode != "DataEntry" && mode != "GridDataEntry")
                return;

            if (mode == "GridDataEntry" && formObj.uid) {
                container = container.find('tr[uid="' + formObj.uid + '"]:first');
            }
            if (mode == "GridDataEntry" && container.is('.grid-inline-form')) {
                var tr = null;
                var id = formObj.row[0].id;
                if (id)
                    tr = container.find('tr#' + id);
                else
                    tr = container.find('tr.jqgfirstrow').next();
                tr.find($.keynav.sex + ':eq(0)').focus();
                return;
            }

            if (container && container.length > 0 && (mode == 'DataEntry' || mode == 'GridDataEntry')) {
                var elem = container.find($.keynav.sex + ':eq(0)');

                

                if (!elem.is('.uie-datetime')) 
                    elem.focus();

                $.keynav.forms.push(container);
            }

        }
    });

    $.fn.extend({

        //This is where you write your plugin's name
        keynav: function (pin) {
            var _this = this;

            if (typeof pin == 'string') {
                var fn = $.keynav[pin];
                if (!fn) {
                    throw ("keynav - No such method: " + pin);
                }
                var args = $.makeArray(arguments).slice(1);
                return fn.apply(this, args);
            } else if (typeof pin === 'object' || !pin) {
                return $.keynav.init.apply(this, arguments);
            }
            else {
                $.error('Method ' + pin + ' does not exist on jQuery.keynav');
            }
        }
    });
})(jQuery);