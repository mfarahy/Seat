// version 0.93.08.17

(function ($) {

    $.classic_layout = $.classic_layout || {};
    $.extend($.classic_layout, {
        onshowcontentcomplete: function (formObj, mode, $form, xml) {
            if (formObj.searchForm) return;

            if (!formObj.parent_id &&
                formObj.xml == window.bocruds[formObj.jsbid].config.xml && // for ignore dialogs
                formObj.detailPos == 'up' && (mode == "DataEntry" || mode == "View")) {
                setTimeout(function () {

                    var form = $('#' + formObj.id);
                    if (form.length > 0)
                        $.bocrud.alignGrid(form);
                }, 500);
            }
            if (formObj.detailPos != 'replacement') {
                setTimeout(function () {
                    $.bocrud.alignGrid($form);
                }, 1000);
            }

        },
        onpreshowcontent: function (formObj) {


            return true;
        },
        onshowcontent: function (formObj, buttons, mode, place_holder_selector) {

            if (mode == "GridDataEntry" || mode == "GridView") return;

            formObj.detailPos = formObj.detailPos.toLowerCase();

            var bc = $('#b-' + formObj.jsbid);

            var ph = $(place_holder_selector);
            var cph = ph.length == 1;
            var fc = cph ? ph : bc.children('.bocrud-form-container');
            var ignore_toolbar = ph.length == 1;
            var ct = [];

            ct.push();

            if (formObj.detailPos == 'down' && !ignore_toolbar)
                ct.push('<div class="bocrud-classic bocrud-toolbar-form-op"></div>');

            if (!formObj.ignore_form_tag) {
                ct.push('<form class="bocrud-page-form" id="');
                ct.push(formObj.id);
                ct.push('" namespace="');
                ct.push(formObj.namespace);
                ct.push('">');
            }

            if (formObj.valSum)
                ct.push('<div class="bocrud-validation-summary"></div>');
            ct.push('<div class="bocrud-current-item"></div><div class="bocrud-form-tabs">');
            ct.push('<ul></ul></div>');

            if (!formObj.ignore_form_tag)
                ct.push('</form>');

            if ((formObj.detailPos == 'up' || formObj.detailPos == 'replacement') && !ignore_toolbar)
                ct.push('<div class="bocrud-classic bocrud-toolbar-form-op"></div>');

            if (fc.length == 0) {
                ct.splice(0, 0, '<div class="bocrud-form-container ui-widget-content ui-corner-all bocrud-style-' + formObj.detailPos + ' ">');
                ct.push('</div>');

                if (formObj.detailPos == 'down')
                    bc.append(ct.join(''));
                else
                    bc.prepend(ct.join(''));

                fc = bc.children('.bocrud-form-container:first');
            }
            else {
                fc.html(' ').append(ct.join(''));// firefox and chrom bug issue
            }

            var makeTabular = formObj.groups.length > 1;

            var tabs = fc.children('div.bocrud-form-tabs:first');
            if (tabs.length == 0) {
                fc.append('<div class="bocrud-form-tabs"><ul></ul></div>');
                tabs = fc.find('.bocrud-form-tabs:first')
            }
            var ul = tabs.children('ul');

            var t = [];
            for (var i = 0; i < formObj.groups.length; ++i) {
                var g = formObj.groups[i];
                var gid = formObj.jsbid + '-' + g.id;

                if (makeTabular) {
                    t.push('<li>');
                    t.push('<a href="#');
                    t.push(gid);
                    t.push('" ');
                    t.push('name="');
                    t.push(g.name);
                    t.push('" ');
                    t.push('index="');
                    t.push(i);
                    t.push('" ');
                    t.push('>');
                    t.push(g.title);
                    t.push('</a>');
                    t.push('</li>');

                    var li = ul.find('li a[href="#' + gid + '"]');
                    if (li.length == 0)
                        ul.append(t.join(''));
                    else {
                        li.find('a').html(g.title);
                    }
                }
                t.splice(0, t.length);

                t.push('<div id="');
                if (makeTabular) t.push(gid);
                t.push('" name="');
                t.push(g.name);
                t.push('" index="');
                if (makeTabular) {
                    t.push(i);
                }
                t.push('">');
                if (!formObj.namespace && !formObj.tempMode && !formObj.parent_id && i == 0 && formObj.boKey && formObj.boKey.length > 0 && (formObj.mode == 'View' || formObj.mode == 'DataEntry')) {
                    t.push('<a class="bocrud-form-anchor"><i class="ace-icon icon icon-anchor"></i><input type="text"/></a>');
                }
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

            if (formObj.detailPos == 'up' && !cph)
                fc.append('<hr/>');

            if (makeTabular) {
                if (!tabs.is('.ui-tab'))
                    tabs.tabs({
                        activate: function (event, ui) {
                            var gid = ui.newTab.children('a').attr('href');
                            $.bocrud.alignGrid($(gid));
                        },
                        show: { effect: "fade" }
                    });
                tabs.tabs('refresh');

                formObj.hideGroup = function (group_name, form) {
                    var a = form.find('a[name="' + group_name + '"]:first');
                    var gid = a.attr('href');
                    a.closest('li').hide('slow');
                    if (form.find(gid).is(':visible')) {
                        var i = parseInt(a.attr('index'));
                        var total_tabs = a.closest('ul').children().length;
                        tabs.tabs('option', 'active', (i + 1) % total_tabs);
                    }
                }
                formObj.showGroup = function (group_name, form) {
                    var a = form.find('a[name="' + group_name + '"]:first');
                    a.closest('li').show('slow');
                }

                formObj.disableGroup = function (group_name, form) {
                    var a = form.find('a[name="' + group_name + '"]:first');
                    var gid = a.attr('href');
                    var i = parseInt(a.attr('index'));
                    if (form.find(gid).is(':visible')) {
                        var total_tabs = a.closest('ul').children().length;
                        tabs.tabs('option', 'active', (i + 1) % total_tabs);
                    }
                    tabs.tabs('option', 'disabled', [i]);
                }
                formObj.enableGroup = function (group_name, form) {
                    var a = form.find('a[name="' + group_name + '"]:first');
                    var gid = a.attr('href');
                    var i = parseInt(a.attr('index'));
                    tabs.tabs('enable', i);
                }
            }

            var b = $('#b-' + formObj.jsbid);

            var cancelCaption = $.jgrid.edit.bClose;

            var tb = fc.children('.bocrud-toolbar-form-op:first');
            tb.html('');
            if ((formObj.detailPos == 'up' ||
                formObj.detailPos == 'down' || formObj.detailPos == 'replacement')) {

                var icon = 'icon-save';
                var submitCaption = formObj.submitCaption;
                if (submitCaption && submitCaption.length > 0)
                    icon = '';
                else
                    submitCaption = $.jgrid.edit.bSubmit;

                if (buttons[$.jgrid.edit.bSubmit] && mode == 'DataEntry')
                    $.bocrud.addButton(tb, {
                        caption: submitCaption,
                        buttonicon: icon,
                        onClickButton: function () {
                            tb.find('li').addClass('ui-state-disabled');
                            buttons[$.jgrid.edit.bSubmit](!formObj.hideGrid);

                        },
                        showText: true,
                        classes: 'bocrud-tb-submit btn btn-app btn-success btn-xs radius-4'
                    });

                if ((buttons[$.jgrid.edit.bCancel] || buttons[$.jgrid.edit.bClose]) && (!formObj.hideGrid || formObj.parent_id)) {
                    var closeCaption = buttons[$.jgrid.edit.bCancel] ? $.jgrid.edit.bCancel : $.jgrid.edit.bClose;
                    $.bocrud.addButton(tb, {
                        caption: mode == 'DataEntry' ? closeCaption : $.jgrid.edit.bClose,
                        buttonicon: 'icon-ban',
                        onClickButton: function () {
                            tb.find('li').addClass('ui-state-disabled');
                            buttons[$.jgrid.edit.bCancel](true);
                            bc.find('#' + formObj.id).hide('slow', function () {
                                $(this).remove();
                            });
                            if (formObj.detailPos == 'up') {
                                $("html,body").animate({
                                    scrollTop: b.offset().top
                                });
                            }
                        },
                        showText: true,
                        classes: 'bocrud-tb-cancel btn btn-app btn-warning btn-xs radius-4'
                    });
                }
                if (formObj.hideGrid && formObj.mode == 'View' && formObj.parent_id == '') {

                    if (formObj.allowCreate) {
                        $.bocrud.addButton(tb, {
                            caption: formObj.createCaption,
                            buttonicon: 'icon-plus-circle bigger-160',
                            onClickButton: function () {
                                window.bocruds[formObj.jsbid].addNew();
                            },
                            showText: true,
                            classes: 'bocrud-tb-new btn btn-app btn-xs radius-4  btn-green'
                        });
                    }
                    if (formObj.allowDelete && formObj.boKey && formObj.boKey.length > 0) {
                        $.bocrud.addButton(tb, {
                            caption: formObj.deleteCaption,
                            buttonicon: 'icon-trash bigger-160',
                            onClickButton: function () {
                                window.bocruds[formObj.jsbid].do_del(formObj.boKey, null, function () {
                                    window.bocruds[formObj.jsbid].addNew();
                                });
                            },
                            showText: true,
                            classes: 'bocrud-tb-delete btn btn-app btn-xs radius-4  btn-danger '
                        });
                    }
                }
            }

            //if (formObj.detailPos == 'replacement' && mode == 'View') {
            //    if (buttons[$.jgrid.edit.bCancel] && (!formObj.hideGrid || formObj.parent_id))
            //        $.bocrud.addButton(tb, {
            //            caption: $.jgrid.edit.bCancel,
            //            buttonicon: 'icon-ban bigger-160',
            //            onClickButton: function() {
            //                tb.find('li').addClass('ui-state-disabled');
            //                buttons[$.jgrid.edit.bCancel](true);
            //                bc.find('#' + formObj.id).hide('slow', function() {
            //                    $(this).remove();
            //                });
            //            },
            //            showText: true,
            //            classes: 'bocrud-tb-cancel btn btn-app btn-warning btn-xs radius-4'
            //        });
            //}

            if (formObj.detailPos == 'normaldialog') {
                var newButtons = {};

                var bocrud = window.bocruds[formObj.jsbid];
                if (mode == 'DataEntry') {
                    if (buttons[$.jgrid.edit.bSubmit])
                        newButtons[$.jgrid.edit.bSubmit] = function () {

                            // غیر فعال کردن دکمه ها
                            $(this).siblings('.ui-dialog-buttonpane:first')
                                .find('button')
                                .addClass('ui-state-disabled')
                                .addClass('bocrud-anti-multiple-click');

                            setTimeout(function () {
                                $('.bocrud-anti-multiple-click').removeClass('ui-state-disabled').removeClass('bocrud-anti-multiple-click');
                            }, 1000);

                            // این باعث می شد که قبل از پیغام موفقیت یا عدم موفقیت سرور پنجره بسته بشه
                            //if (buttons[$.jgrid.edit.bSubmit](true))
                            //    bocrud.closeModal($(this));

                            // خوب حالا اگر فرم خطا داشته باشه چه موقع باید دکمه ها رو مجدد فعال کنیم؟
                            buttons[$.jgrid.edit.bSubmit](true);

                        }
                    cancelCaption = $.jgrid.edit.bCancel;
                }
                if ((buttons[$.jgrid.edit.bCancel] || buttons[$.jgrid.edit.bClose]) && (!formObj.hideGrid || formObj.parent_id)) {

                    cancelCaption = $.jgrid.edit.bCancel;
                    cancelCaption = buttons[$.jgrid.edit.bClose] ? $.jgrid.edit.bClose : cancelCaption;

                    newButtons[cancelCaption] = function () {
                        if (mode == 'DataEntry')
                            $(this).siblings('.ui-dialog-buttonpane:first')
                                .find('button')
                                .addClass('ui-state-disabled');
                        (buttons[$.jgrid.edit.bCancel] || buttons[$.jgrid.edit.bClose])(true);
                        bocrud.closeModal($(this));
                    }
                }
                for (var c in buttons) {
                    if (!newButtons[c])
                        newButtons[c] = buttons[c];
                }

                bocrud.openModal(fc, {
                    title: !formObj.title || formObj.title.length == 0 ? formObj.groups[0].title : formObj.title,
                    width: formObj.width > 0 ? formObj.width : 400,
                    height: formObj.height > 0 ? formObj.height : 300,
                    buttons: newButtons,
                    modal: true,
                    close: function () {
                        bocrud.closeModal($(this));
                        fc.remove();
                    },
                    open: function () {
                        $(this).find('.ui-dialog-titlebar-close').blur();
                        if (formObj.detailPos == "ExpandedDialog") {
                            $(this).addClass('modal-lg');
                        }
                    }
                });

            } else // if dialog
                fc.show('slow');

            fc.find('[data-rel=popover]').popover({ container: 'body' });

            $.classic_layout._hide_grid_if_replacement(formObj, 'show');

        },
        _hide_grid_if_replacement: function (formObj) {
            if (formObj.detailPos == 'replacement' && !formObj.searchForm) {
                var b = $('#b-' + formObj.jsbid);

                b.children('.bocrud-search-container').hide('fade', 500);
                b.children('.bocrud-toolbar').hide('fade', 500);
                b.find('#bc-' + formObj.jsbid).hide('fade', 500);
            }
        },
        _show_grid_if_replacement: function (formObj) {
            if (formObj.detailPos == 'replacement') {
                var b = $('#b-' + formObj.jsbid);
                b.children('.bocrud-search-container').show('fade', 500);
                b.children('.bocrud-toolbar').show('fade', 500);
                b.find('#bc-' + formObj.jsbid).show('fade', 500);
            }
        },
        onvalidationfailed: function ($form) {
            var formObj = $form.data('formObj');
            $.classic_layout._enable_toolbar(formObj);
        },
        onsubmitcomplete: function (formObj, params) {
            $.classic_layout._enable_toolbar(formObj);
        },
        onviewcancel: function (formObj) {
            var fc = $('#' + formObj.id).parent();
            var bocrud = window.bocruds[formObj.jsbid];
            if (formObj.detailPos == 'normaldialog') {
                var dialog = fc.closest('.dialog');
                if (dialog.length == 0) {
                    dialog = fc;
                    while (!dialog.data('ui-dialog') && dialog.length == 1) {
                        dialog = dialog.parent();
                    }
                }

                bocrud.closeModal(dialog);
                return true;
            }
            var b = $('#b-' + formObj.jsbid);
            var tb = fc.find('.bocrud-toolbar-form-op:first');
            tb.html('');
            fc.hide('slow').children('.bocrud-page-form').html('');

            $.classic_layout._show_grid_if_replacement(formObj);
        },
        onpregetcontent: function (bocrudObj, data) {
            var detPos = bocrudObj.config.detPos.toLowerCase();
            if (detPos == 'browsertab' || detPos == 'newwin') {
                var p = [];
                p.push(bocrudObj.config.urlPrefix);
                p.push('/Index?xml=');
                p.push(data.xml);
                if (data.id && data.id.length > 0) {
                    p.push('&id=');
                    p.push(data.id);
                }
                p.push('&sv=1');
                p.push('&mode=');
                p.push(data.mode);
                p.push('&username=');
                p.push(bocrudObj.urlParams['username']);
                window.open(p.join(''));
                return false;
            }
            return true;
        },
        onsubmit: function () {
        },
        _enable_toolbar: function (formObj) {
            if (formObj.detailPos == 'normaldialog')
                $('form#' + formObj.id).parent()
                    .siblings('.ui-dialog-buttonpane:first')
                    .find('button')
                    .removeClass('ui-state-disabled');
            else {
                var op = $('form#' + formObj.id).children('.bocrud-toolbar-form-op:first');
                if (op.length == 0) {
                    op = $('form#' + formObj.id).parent().find('.bocrud-toolbar-form-op:first');
                }
                op.find('button').removeClass('ui-state-disabled').removeAttr('disabled');
            }
        },
        onsubmitsuccess: function (formObj, saveResult, showDetails) {
            $.classic_layout._enable_toolbar(formObj);

            var bocrud = window.bocruds[formObj.jsbid];

            //if (bocrud.config.hideGrid) {
            //    if (showDetails)
            //        bocrud.do_view(formObj.boKey, bocrud.grid());
            //}
            //else
            // اینجا چون خود سیستم داره هنهدل می کنه داستان نمایش یا عدمنمایش رو نیازی نیست
            $.classic_layout.onviewcancel(formObj);

            $.classic_layout._show_grid_if_replacement(formObj);
        },
        onsubmiterror: function (formObj, params) {
            $.classic_layout._enable_toolbar(formObj);
        },
        onload: function (jsbid) {
            var addBtn = $('#b-' + jsbid).find('#' + jsbid + 'addNew');
            var menubtn = addBtn
                .button({ icons: { primary: 'ui-icon-circle-plus' } })
                .click(function () {
                    var t = $(this);
                    var active = t.parent().next().data('active');
                    if (!active) menubtn.click();
                    else
                        active.click();
                }).addClass('ui-button-success')
                .next();

            if (menubtn.length > 0 && menubtn.is('button')) {
                menubtn.button({
                    text: false,
                    icons: {
                        primary: "ui-icon-triangle-1-s"
                    }
                }).css({ 'margin-right': '-10px' })
                    .addClass('ui-button-success bocrud-toolbar-addbtn')
                    .click(function () {
                        var menu = $(this).parent().next().show().position({
                            my: "left top",
                            at: "left bottom",
                            of: this
                        });
                        $(document).one("click", function () {
                            menu.hide();
                        });
                        return false;
                    }).parent().buttonset().next()
                    .hide().menu()
                    .css({ 'z-index': 10, position: 'absolute' });
            }
        },
        onaddnew: function (formObj) {
            var bc = $('#bc-' + formObj.jsbid);
            if (bc.is('.ui-tab'))
                bc.tabs({ active: 1 });
            if (formObj.isTree && formObj.tree_parent_id) {
                var b = window.bocruds[formObj.jsbid];
                var g = b.grid();
                var rd = g.jqGrid('getRowData', g.jqGrid('getGridParam', 'selrow'));
                var str = [];
                str.push("<div class='bocrud-tree-add-info'>");
                str.push("شما در حال اضافه کردن زیر گره به این آیتم هستید :");
                str.push("<div class='bocrud-current-item-text'>");
                str.push(b.item2Str(rd, b.config.toStringFormat, g));
                str.push("</div>");
                str.push("</div>");
                $('#' + formObj.id).find('.bocrud-current-item').html(str.join(''));
            }
        },
        ondeletesuccess: function () {
            if (this.config.tempMode) return;
            var ids = arguments;
            $('form.bocrud-page-form').each(function () {
                var formObj = $(this).data('formObj');
                for (var i = 0; i < ids.length; ++i) {
                    if (formObj.boKey == ids[i]) {
                        $.classic_layout.onviewcancel(formObj);
                    }
                }
            });
        }
    });


})(jQuery);