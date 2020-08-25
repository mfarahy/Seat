
$(function () {
    if ($.validator)
        $.validator.setDefaults({

            highlight: function (element, errorClass, validClass) {
                var bc = $(element).closest('.bocrud-control');
                if (!bc.hasClass('form-group'))
                    bc.addClass('form-group');
                bc.addClass('has-error').removeClass("has-success");
                bc.find('label.valid').remove();
            },
            unhighlight: function (element, errorClass, validClass) {
                var bc = $(element).closest('.bocrud-control');
                if (!bc.hasClass('form-group'))
                    bc.addClass('form-group');
                bc.removeClass('has-error').addClass("has-success");
                bc.find('label.error').remove();
            },
            invalidHandler: function (e, v) {
                var valSum = $(v.currentForm).children('.bocrud-validation-summary:first');
                if (valSum.length > 0) {
                    var m = [];
                    m.push('<br/>');
                    m.push('<ul>');
                    for (var i = 0; i < v.errorList.length; ++i) {
                        m.push('<li>');
                        m.push(v.errorList[i].message);
                        m.push('</li>');
                    }
                    m.push('</ul>');
                    valSum.html(m.join(''));
                }

                for (var i = 0; i < v.errorList.length; ++i) {
                    var elem = $(v.errorList[i].element);

                    var c = elem.closest('form,.ui-tabs-panel');
                    if (c.is('.ui-tabs-panel')) {
                        var ul = c.parent().children('ul');
                        var li = ul.find('li[aria-controls=' + c.attr('id') + ']:first');
                        if (li.length) {
                            var a = li.children('a');
                            if (a.children('i').length == 0) {
                                a.append('<i class="icon-warning-sign bigger-120" style="color:white"></i>');
                                a.addClass('label-warning');
                            }
                        }
                    }
                }// for
            }
        });
});


$.bocrud = $.extend($.bocrud || {},
    {
        openModal2: function (div, options) {

            var header = [];
            header.push('<div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><h4 class="modal-title">');
            header.push(options.title);
            header.push('</h4></div>');

            var footer = [];
            footer.push('<div class="modal-footer">');
            var first = true;
            for (var b in options.buttons) {
                footer.push('<button type="button" class="btn');
                if (first) {
                    footer.push(' btn-default');
                    first = !first;
                }
                footer.push('">');
                footer.push(b);
                footer.push('</button>');
            }
            footer.push('</div>');

            var modal = [];
            modal.push('<div class=" modal ');
            modal.push(options.dialogClass);
            modal.push('" tabindex="-1" ');
            modal.push(' role="dialog" aria-hidden="true"></div>');

            modalObj = div.show().wrap('<div class="modal-body"></div>').parent()
             .wrap('<div class="modal-content"></div>').parent()
                 .prepend(header.join(''))
                 .append(footer.join(''))
             .wrap(' <div class="modal-dialog"></div>').parent()
             .wrap(modal.join(''))
             .closest('.modal')
             .modal({
                 width: Math.max(options.width, div.width()),
                 height: options.height,
                 keyboard: options.closeOnEscape
             });
            //a.append('<div class="">width:<div>');
            //a.append('<div class="">' + Math.max(options.width, div.width()) + '<div>');
            //a.append('<div class="">height:<div>');
            //a.append('<div class="">' + options.height + '<div>');

            var footerObj = modalObj.children('.modal-dialog')
                  .children('.modal-content')
                  .children('.modal-footer');
            footerObj.find('button').each(function () {
                var b = $(this);
                b.click(function () {
                    options.buttons[b.text()].apply(modalObj);
                });
            });

            modalObj.on('shown.bs.modal', function (e) {
                if ($.isFunction(options.open)) {
                    try {
                        options.open.apply(modalObj);
                    } catch (e) {
                        $.console.error('ace-75:' + e);
                    }
                }
                modalObj.find('.ui-jqgrid').each(function () {
                    var gid = $(this).attr('id').substring(5);
                    var g = $('#' + gid);
                    var w = g.jqGrid('getGridParam', 'width');
                    if (w <= 100 && g.is(':visible'))
                        gridAutoWidth(gid, w);
                });
            })
            modalObj.on('hidden.bs.modal', function (e) {
                if ($.isFunction(options.close)) {
                    try {
                        options.close.apply(modalObj);
                    } catch (e) {
                        $.console.error('ace-75:' + e);
                    }
                }
            })
            return modalObj;
        }
    });

function toggle_toolbar_appbtn(bid, b) {
    var c = $('#b-' + bid)
                .children('.bocrud-toolbar:first');
    if (b)
        c.find('button.ui-toolbar-dependtorow').removeClass('disabled');
    else
        c.find('button.ui-toolbar-dependtorow').addClass('disabled');
}

$.bocrud.globalEvents.push({
    type: 'onselectrow', key: 'ace',
    func: function (g, s) {
        toggle_toolbar_appbtn(this.config.id, s.length >= 1);
    }
});

$.bocrud.globalEvents.push({
    type: 'onshowcontentcomplete', key: 'ace',
    func: function (form, mode, $form, xml) {
        $form.find('textarea.bocrud-textarea')
            .addClass('form-control');
    }
});

$.bocrud.globalEvents.push({
    type: 'ondeselectrow', key: 'ace',
    func: function (g) {
        toggle_toolbar_appbtn(this.config.id, false);
    }
});

$.bocrud.globalEvents.push({
    type: 'onpreedit', key: 'ace',
    func: function(id, g) {
        $('#' + this.config.id + "getDataEntryView").attr('disabled', 'disabled');
    }
});

$.bocrud.globalEvents.push({
    type: 'onedit', key: 'ace',
    func: function (formObj) {
        $('#' + this.config.id + "getDataEntryView").removeAttr('disabled');

        Ladda.bind('.bocrud-tb-submit', { timeout: 1000 });
    }
});

$.bocrud.globalEvents.push({
    type: 'onpreview', key: 'ace',
    func: function (id, g) {
        $('#' + this.config.id + "getView").attr('disabled', 'disabled');
    }
});

$.bocrud.globalEvents.push({
    type: 'onview', key: 'ace',
    func: function (formObj) {
        $('#' + this.config.id + "getView").removeAttr('disabled');
    }
});

$.bocrud.globalEvents.push({
    type: 'onpreaddnew', key: 'ace',
    func: function () {
        $('#' + this.config.id + "addNew").attr('disabled', 'disabled');
    }
});

$.bocrud.globalEvents.push({
    type: 'onaddnew', key: 'ace',
    func: function (formObj) {
        $('#' + this.config.id + "addNew").removeAttr('disabled');

        Ladda.bind('.bocrud-tb-submit', { timeout: 1000 });
    }
});

$.bocrud.globalEvents.push({
    type: 'onaddnewfailed', key: 'ace',
    func: function (formObj) {
        $('#' + this.config.id + "addNew").removeAttr('disabled');
    }
});

$.bocrud.globalEvents.push({
    type: 'onpresubmit', key: 'ace',
    func: function (formObj, params) {
        $('.bocrud-toolbar-form-op button').prop('disabled', true);
    }
});

$.bocrud.globalEvents.push({
    type: 'onsubmiterror', key: 'ace',
    func: function (formObj, saveResult) {
        $('.bocrud-toolbar-form-op button').prop('disabled', false);
    }
});

$.bocrud.globalEvents.push({
    type: 'onsubmitsuccess', key: 'ace',
    func: function (formObj, saveResult, showDetails) {
        $('.bocrud-toolbar-form-op button').prop('disabled', false);
    }
});

$.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
    _title: function (title) {
        var $title = this.options.title || '&nbsp;'
        if (("title_html" in this.options) && this.options.title_html == true)
            title.html($title);
        else title.text($title);
    }
}));

$.jgrid.extend({
    toolbarButtonAdd: function (elem, p) {
        p = $.extend({
            id: "",
            caption: "",
            title: '',
            context: '',
            buttonicon: '',
            onClickButton: null,
            position: "last",
            classes: ''
            //,
            //dependToRow: false
        }, p || {});
        var $t = this;
        return this.each(function () {
            if (!this.grid) { return; }

            if (elem.indexOf("#") != 0) {
                elem = "#" + elem;
            }

            var dependToRow = p.min != 0;

            if ($(elem).length == 0)
                elem = '#t_' + this.p.id;
            var closest = $(elem).closest('.bocrud-container,.bocrud-control-content');
            elem = closest.find('.bocrud-toolbar:first');
            if (elem.length == 0) {
                closest.prepend("<div class='bocrud-toolbar-container'><div class='bocrud-toolbar'><p class='grid-toolbar'/></div></div>");
                elem = closest.children('.bocrud-toolbar:first');
            }
            if (p.id && elem.find('#' + p.id).length > 0) return;

            if (elem.length == 0) elem = '#t_' + this.p.id;

            var hasParent = elem.data('has_parent');
            if (elem.data('has_parent') == undefined) {
                hasParent = elem.closest('.bocrud-page-form').length > 0;
                elem.data('has_parent', hasParent);
            }

            if (p.buttonicon && p.buttonicon.substring(0, 7) == "ui-icon")
                p.buttonicon = p.buttonicon.substring(3);

            var tbd = $("<button title=\"" + p.title + "\"  />");

            tbd.attr('data-min', p.min);
            tbd.attr('data-max', p.max);

            if (!p.buttonicon && !p.title && !p.caption) {
                $(tbd).append('<i class="icon-cog"></i>');
            } else {
                $(tbd).addClass('btn no-radius ' + (hasParent ? '' : ' ') + p.classes + (dependToRow ? ' ui-toolbar-dependtorow disabled' : ''))
                    .append("<i class='button-icon-with-padding " + (hasParent ? 'align-top ' : ' ') + p.buttonicon + "'></i>")
                    .click(function (e) {
                        if ($.isFunction(p.onClickButton) && !$(this).hasClass('disabled')) {
                            var selectedRow;
                            if ($t.getGridParam('multiselect'))
                                selectedRow = $t.getGridParam('selarrrow');
                            else
                                selectedRow = $t.getGridParam('selrow');
                            p.onClickButton(selectedRow, $t, p.context);
                        }

                        return false;
                    });

                if (p.showText)
                    $(tbd).append(p.caption);
            }

            if (p.id) { $(tbd).attr("id", p.id); }
            if (p.align) { $(elem).attr("align", p.align); }
            var findnav = $(elem).children('.grid-toolbar:first');
            p.position = 'last';
            if (p.position === 'first') {
                if ($(findnav).find('li').length === 0) {
                    $(findnav).append(tbd);
                } else {
                    $(findnav).prepend(tbd);
                }
            } else {
                $(findnav).prepend(tbd);
            }
        });
    }
});

$.jgrid.extend({ layout: { dir: 'rtl' } });

// change noty theme

; (function ($) {
    if (!$.noty) return;

    $.noty.themes.defaultTheme = {
        name: 'defaultTheme',
        helpers: {
            borderFix: function () {
                if (this.options.dismissQueue) {
                    var selector = this.options.layout.container.selector + ' ' + this.options.layout.parent.selector;
                    switch (this.options.layout.name) {
                        case 'top':
                            $(selector).css({ borderRadius: '0px 0px 0px 0px' });
                            $(selector).last().css({ borderRadius: '0px 0px 5px 5px' }); break;
                        case 'topCenter': case 'topLeft': case 'topRight':
                        case 'bottomCenter': case 'bottomLeft': case 'bottomRight':
                        case 'center': case 'centerLeft': case 'centerRight': case 'inline':
                            $(selector).css({ borderRadius: '0px 0px 0px 0px' });
                            $(selector).first().css({ 'border-top-left-radius': '5px', 'border-top-right-radius': '5px' });
                            $(selector).last().css({ 'border-bottom-left-radius': '5px', 'border-bottom-right-radius': '5px' }); break;
                        case 'bottom':
                            $(selector).css({ borderRadius: '0px 0px 0px 0px' });
                            $(selector).first().css({ borderRadius: '5px 5px 0px 0px' }); break;
                        default: break;
                    }
                }
            }
        },
        modal: {
            css: {
                position: 'fixed',
                width: '100%',
                height: '100%',
                backgroundColor: '#000',
                zIndex: 10000,
                opacity: 0.6,
                display: 'none',
                left: 0,
                top: 0
            }
        },
        style: function () {

            this.$message.css({
                fontSize: '13px',
                lineHeight: '16px',
                textAlign: 'center',
                padding: '8px 10px 9px',
                width: 'auto',
                position: 'relative'
            });

            this.$closeButton
                .html('<button data-dismiss="alert" class="close" type="button"><i class="icon-remove"></i></button>');

            this.$buttons.css({
                padding: 5,
                textAlign: 'right',
                borderTop: '1px solid #ccc',
                backgroundColor: '#fff'
            });

            this.$buttons.find('button').css({
                marginLeft: 5
            });

            this.$buttons.find('button:first').css({
                marginLeft: 0
            });

            this.$bar.bind({
                mouseenter: function () { $(this).find('.noty_close').stop().fadeTo('normal', 1); },
                mouseleave: function () { $(this).find('.noty_close').stop().fadeTo('normal', 0); }
            });

            switch (this.options.layout.name) {
                case 'top':
                    this.$bar.css({
                        borderRadius: '0px 0px 5px 5px',
                        borderBottom: '2px solid #eee',
                        borderLeft: '2px solid #eee',
                        borderRight: '2px solid #eee',
                        boxShadow: "0 2px 4px rgba(0, 0, 0, 0.1)"
                    });
                    break;
                case 'topCenter': case 'center': case 'bottomCenter': case 'inline':
                    this.$bar.css({
                        borderRadius: '5px',
                        border: '1px solid #eee',
                        boxShadow: "0 2px 4px rgba(0, 0, 0, 0.1)"
                    });
                    this.$message.css({ fontSize: '13px', textAlign: 'right' });
                    break;
                case 'topLeft': case 'topRight':
                case 'bottomLeft': case 'bottomRight':
                case 'centerLeft': case 'centerRight':
                    this.$bar.css({
                        borderRadius: '5px',
                        border: '1px solid #eee',
                        boxShadow: "0 2px 4px rgba(0, 0, 0, 0.1)"
                    });
                    this.$message.css({ fontSize: '13px', textAlign: 'left' });
                    break;
                case 'bottom':
                    this.$bar.css({
                        borderRadius: '5px 5px 0px 0px',
                        borderTop: '2px solid #eee',
                        borderLeft: '2px solid #eee',
                        borderRight: '2px solid #eee',
                        boxShadow: "0 -2px 4px rgba(0, 0, 0, 0.1)"
                    });
                    break;
                default:
                    this.$bar.css({
                        border: '2px solid #eee',
                        boxShadow: "0 2px 4px rgba(0, 0, 0, 0.1)"
                    });
                    break;
            }

            switch (this.options.type) {
                case 'alert': case 'notification':
                    this.$bar.addClass('alert alert-danger'); break;
                case 'warning':
                    this.$bar.css({ backgroundColor: '#FFEAA8', borderColor: '#FFC237', color: '#826200' });
                    this.$buttons.css({ borderTop: '1px solid #FFC237' }); break;
                case 'error':
                    this.$bar.addClass('alert alert-danger');
                    this.$message.css({ fontWeight: 'bold' });
                    this.$buttons.css({ borderTop: '1px solid darkred' }); break;
                case 'information':
                    this.$bar.css({ backgroundColor: '#57B7E2', borderColor: '#0B90C4', color: '#FFF' });
                    this.$buttons.css({ borderTop: '1px solid #0B90C4' }); break;
                case 'success':
                    this.$bar.css({ backgroundColor: 'lightgreen', borderColor: '#50C24E', color: 'darkgreen' });
                    this.$buttons.css({ borderTop: '1px solid #50C24E' }); break;
                default:
                    this.$bar.css({ backgroundColor: '#FFF', borderColor: '#CCC', color: '#444' }); break;
            }
        },
        callback: {
            onShow: function () { $.noty.themes.defaultTheme.helpers.borderFix.apply(this); },
            onClose: function () { $.noty.themes.defaultTheme.helpers.borderFix.apply(this); }
        }
    };

})(jQuery);


$.bocrud.globalEvents.push({
    type: 'onload', key: 'ace',
    func: function (cfg) {
        
        if (!cfg.parent) {
            var pageName = cfg.xml;
            var menu = $('#mastreMenu li').find('#' + pageName);
            menu.addClass("active");
            while (true) {
                var parentMenu = menu.parent();
                var grandMenu = parentMenu.parent();
                if (parentMenu.hasClass("submenu")) {
                    parentMenu.css({ 'display': "block" });
                    grandMenu.addClass("open");
                    menu = grandMenu;
                }
                else {
                    break;
                }
            }

            var active_menu = $('#mastreMenu').find('.bocrud-menu-item.active').first();
            if (active_menu.length > 0) {
                var icon = active_menu.find('i.icon');
                $('.page-header.bocrud-page-title').prepend(icon);
            }
        }
    }
});
