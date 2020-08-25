
; (function ($) {

    $.iranmap = $.iranmap || {};
    $.extend($.iranmap, {
        init: function (options) {
            var o = options || {};

            $(this).each(function () {
                var t = $(this);
                var b = t.children();
                b.attr('title', '');
                b.tooltip();

                for (var province_name in o.data) {
                    var path = t.find('path.' + province_name);
                    var value = parseInt(o.data[province_name], 10);
                    var c = value * 1.0 / o.max;
                    path.css({ fill: $.iranmap._colorLuminance(o.color, c) }).data('pvalue', value.toString());
                    path.hover(function (e) {
                        b.attr('title', $(this).data('pvalue'))
                        .tooltip('fixTitle')
                        .tooltip('show');
                       
                    }, function () {
                        b.tooltip('hide');
                    });
                }


            });//each


        }, // init
        _colorLuminance: function (hex, lum) {

            // validate hex string
            hex = String(hex).replace(/[^0-9a-f]/gi, '');
            if (hex.length < 6) {
                hex = hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
            }
            lum = lum || 0;

            // convert to decimal and change luminosity
            var rgb = "#", c, i;
            for (i = 0; i < 3; i++) {
                c = parseInt(hex.substr(i * 2, 2), 16);
                c = Math.round(Math.min(Math.max(0, c + (c * lum)), 255)).toString(16);
                rgb += ("00" + c).substr(c.length);
            }

            return rgb;
        }
    });

    $.fn.extend({

        //This is where you write your plugin's name
        iranmap: function (pin) {
            var _this = this;

            if (typeof pin == 'string') {
                var fn = $.iranmap[pin];
                if (!fn) {
                    throw ("iranmap - No such method: " + pin);
                }
                var args = $.makeArray(arguments).slice(1);
                return fn.apply(this, args);
            } else if (typeof pin === 'object' || !pin) {
                return $.iranmap.init.apply(this, arguments);
            }
            else {
                $.error('Method ' + pin + ' does not exist on jQuery.iranmap');
            }
        }
    });
})(jQuery);