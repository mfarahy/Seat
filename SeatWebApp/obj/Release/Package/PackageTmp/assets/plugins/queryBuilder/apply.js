
; (function ($) {

    var selectors = {
        group_container: '.rules-group-container',
        rule_container: '.rule-container',
        filter_container: '.rule-filter-container',
        operator_container: '.rule-operator-container',
        value_container: '.rule-value-container',
        error_container: '.error-container',
        condition_container: '.rules-group-header .group-conditions',

        rule_header: '.rule-header',
        group_header: '.rules-group-header',
        group_actions: '.group-actions',
        rule_actions: '.rule-actions',

        rules_list: '.rules-group-body>.rules-list',

        group_condition: '.rules-group-header [name$=_cond]',
        rule_filter: '.rule-filter-container [name$=_filter]',
        rule_operator: '.rule-operator-container [name$=_operator]',
        rule_value: '.rule-value-container [name*=_value_]',

        add_rule: '[data-add=rule]',
        delete_rule: '[data-delete=rule]',
        add_group: '[data-add=group]',
        delete_group: '[data-delete=group]'
    };




    $.myQueryBuilder = $.myQueryBuilder || {};
    $.extend($.myQueryBuilder, {
        serialize: function () {
            var t = $(this).first();
            if (t.is(':visible')) {
                var rules = t.queryBuilder('getRules', { allow_invalid: true });
                if (rules == null) return false;
                var v = JSON.serialize(rules);
                t.next(':hidden').val(v);
            }

            return true;
        },
        init: function (filters, jsbid, isReadonly) {

            window.bocruds[jsbid].addEvent("onprepartialupdatereplaced", function (name, ro, mode, dialogObj) {
                $('.query-builder').each(function () {
                    var t = $(this);
                    if (t.is('.query-builder')) {
                        var myName = t.closest('.bocrud-control').attr('name');
                        console.log(myName, "destoyed");
                        if (myName == name) {
                            t.queryBuilder('destroy');
                        }
                    }
                });
            });

            window.bocruds[jsbid].addEvent("onpresubmit", function (data) {
                var valid = true;
                $('.query-builder').each(function () {
                    if (!valid) return false;
                    valid = $(this).myQueryBuilder('serialize');
                });
                return valid;
            });

            $(this).each(function () {
                var c = $(this);

                c.css({
                    'direction': 'ltr',
                    'text-align': 'left'
                });
                var rules = JSON.parse($(this).next(':hidden').val());

                var plugins = [
                        'bt-tooltip-errors',
                        'not-group'];
                if (!isReadonly)
                    plugins.push('sortable');

                c.queryBuilder({
                    plugins: plugins,
                    filters: filters
                });
                c.queryBuilder('reset');
                c.queryBuilder('setRules', rules);
                if (isReadonly)
                    c.find('button,:input').attr('disabled', 'disabled');
            });//each


        } // init

    });

    $.fn.extend({

        //This is where you write your plugin's name
        myQueryBuilder: function (pin) {
            var _this = this;

            if (typeof pin == 'string') {
                var fn = $.myQueryBuilder[pin];
                if (!fn) {
                    throw ("myQueryBuilder - No such method: " + pin);
                }
                var args = $.makeArray(arguments).slice(1);
                return fn.apply(this, args);
            } else if (typeof pin === 'object' || !pin) {
                return $.myQueryBuilder.init.apply(this, arguments);
            }
            else {
                $.error('Method ' + pin + ' does not exist on jQuery.myQueryBuilder');
            }
        }
    });
})(jQuery);