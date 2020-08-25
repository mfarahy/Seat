
; (function ($) {

    $.textboxList = $.textboxList || {};
    $.extend($.textboxList, {
        init: function (options) {
            var o = $.extend({
                dir: 'rtl',
                splitter: ','
            }, options || {});

            this.each(function () {
                var $tt = $(this); //template textbox
                var name = $tt.attr('name'), id = $tt.attr('id');
                var maskVal = $tt.attr('mask');

                $tt.attr('name', name + '-txtlst');
                $tt.attr('id', id + '-txtlst');
                var template = $(
                    '<div class="uie-textbox-list">' +
                    '<div class="uie-textbox-list-container"></div>' +
                    '<div class="uie-textbox-list-separator"></div>' +
                    '<div class="uie-textbox-list-action-box"></div>' +
                    '</div>'
                );
                var divContainer = template.find(".uie-textbox-list-container");
                var templateContainer = template.find(".uie-textbox-list-action-box");
                $tt.before(template);
                var $mrow = $('<span role="add" style="border:none; float : right;"></span><span role="txt" style="border:none; float : right;"></span>');

                templateContainer.html($mrow);

                var $addBtn = $('<button type="button" class="btn btn-primary btn-xs bocrud-txblst-btn"/>');
                $addBtn
                    .html('<i class="ace-icon icon-only icon icon-plus"/>')
                    .click(function () {
                        var str = $tt.val();
                        $tt.val('');
                        if (str.length > 0) {
                            addRow(str);
                        }
                        refreshData();
                    });

                templateContainer.find('span[role=add]').html($addBtn);
                var $mtxt = $('<input type="text" style="display:none" id="' + id + '" name="' + name + '" class="ignore-validation" />');
                var strVal = $tt.val();
                $mtxt.val(strVal);
                $tt.val("");
                if (maskVal != undefined && maskVal != '') {
                    var options = {
                        onComplete: function (currentValue) {
                            addRow(currentValue);
                            refreshData();
                            $tt.val('');
                        },
                        translation: {
                            'N': {
                                pattern: /[0-9]/, optional: true
                            }
                        }
                    };
                    $tt.mask(maskVal, options);

                    $addBtn.attr('disabled', 'disabled');
                }
                $tt.after($mtxt);
                templateContainer.find('span[role=txt]').html($tt);

                var refreshData = function () {
                    var data = '';
                    divContainer.find('.bocrud-txtlst-item').each(function () {
                        var val = $(this).text();
                        if (val.length > 0)
                            data += val + ',';
                    });
                    if (data.length > 0)
                        data = data.substring(0, data.length - 1);
                    $mtxt.val(data);
                }

                function htmlEntities(str) {
                    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
                }

                var addRow = function (item) {
                    var $row = $('<div class="uie-textbox-list-item"><span role="del" style="border:none; float: right; margin-left : 5px"></span><span class="bocrud-txtlst-item">' + htmlEntities(item) + '</span></div>');
                    var $del = $('<i class="ace-icon icon-only icon icon-times-circle"/>');
                    $del
                        .click(function () {
                            $row.remove();
                            refreshData();
                        });
                    $row.find('span[role=del]').html($del);
                    if (o.dir == 'ltr')
                        $row.css({
                            direction: 'ltr',
                            float: 'left',
                            'text-align': 'left'
                        });
                    divContainer.append($row);
                }

                if (strVal.length > 0) {
                    var arrVal = strVal.split(o.splitter);
                    for (var i = 0; i < arrVal.length; ++i) {
                        var item = arrVal[i];
                        addRow(item);
                    }
                }

                $tt.change(function () {
                    var inputElement = $(this);
                    var maskVal = inputElement.attr('mask');
                    if (maskVal != undefined && !Inputmask.isValid($tt.val(), getMaskValForValidation(maskVal)))
                        return;
                    var currentValue = inputElement.val();
                    if (currentValue.length > 0) {
                        addRow(currentValue);
                        refreshData();
                        inputElement.val('');
                    }
                });

                function getMaskValForValidation(input)
                {
                    var result = input;
                    result = input.replace(/N/g, '[9]');

                    return result;
                }
            });
        }
    });

    $.fn.extend({

        //This is where you write your plugin's name
        textboxList: function (pin) {
            var _this = this;

            if (typeof pin == 'string') {
                var fn = $.leftScrollbar[pin];
                if (!fn) {
                    throw ("textboxList - No such method: " + pin);
                }
                var args = $.makeArray(arguments).slice(1);
                return fn.apply(this, args);
            } else if (typeof pin === 'object' || !pin) {
                return $.textboxList.init.apply(this, arguments);
            }
            else {
                $.error('Method ' + pin + ' does not exist on jQuery.textboxList');
            }
        }
    });
})(jQuery);