// version 0.96.04.29
var DataSourceTree = function (options) {
    this._data = options.data;
    this._delay = options.delay;
}

DataSourceTree.prototype.data = function (options, callback) {
    var self = this;
    var $data = null;

    if (!("name" in options) && !("type" in options)) {
        $data = this._data;//the root tree
        callback({ data: $data });
        return;
    }
    else if ("type" in options && options.type == "folder") {
        if ("additionalParameters" in options && "children" in options.additionalParameters)
            $data = options.additionalParameters.children;
        else $data = {}//no data
    }

    if ($data != null)//this setTimeout is only for mimicking some random delay
        setTimeout(function () { callback({ data: $data }); }, parseInt(Math.random() * 500) + 200);

    //we have used static data here
    //but you can retrieve your data dynamically from a server using ajax call
    //checkout examples/treeview.html and examples/treeview.js for more info
};

(function ($) {

    $.workbox_layout = $.workbox_layout || {};
    $.extend($.workbox_layout, {
        url: '',
        get_bocrud: function (name) {
            for (var k in window.bocruds)
                if (k.indexOf(name) >= 0)
                    return window.bocruds[k];
            return null;
        },
        onshowcontent: function (formObj, buttons, mode, place_holder_selector) {

            var bCancel = buttons[$.jgrid.edit.bCancel];

            if (mode == "GridDataEntry" || mode == "GridView") return;

            $.classic_layout.onshowcontent(formObj, {}, mode, place_holder_selector);

            if (formObj.xml == '__Box') {
                var bc = $('#b-' + formObj.jsbid);

                var ph = $(place_holder_selector);
                var fc = ph.length == 1 ? ph : bc.children('.bocrud-form-container');

                var b = $.workbox_layout.get_bocrud('__Box');

                var submit_func = function (preventLoadView, xdata) {

                    var ignoreValidation = !xdata["@JobContentMustSave"] || false;

                    var success = function (data) {
                        var b = $.workbox_layout.get_bocrud('__Box');
                        if (b.config.hideGrid) {
                            if (window.opener != null) {
                                window.close();
                                return;
                            } else {
                                window.location = document.referrer;
                                return;
                            }
                        }
                    };

                    var on_error_in_step2 = function () {
                        if (b.config.hideGrid)
                            window.location.reload();
                        else
                            b.do_edit(formObj.boKey, b.grid());
                    };

                    var onpresubitsuccess = function () {
                        if (op != 'RawSave') {
                            //$form, data, oldBoKey, async, showDetails, action, onsuccess, ignoreValidation, onerror, onpresubitsuccess
                            b.do_submit(form, xdata, formObj.boKey, true, !preventLoadView, null, success, ignoreValidation, on_error_in_step2, null);
                        }
                        return op == 'RawSave';
                    };

                    var form = $('#' + formObj.id);
                    var op = xdata["@Operation"];
                    if (op == 'RawSave' || xdata["@JobContentMustSave"]) {
                        var oxdata = {
                            "@Operation": 'RawSave',
                            "@JobContentMustSave": true,
                            "@SaveJob": true
                        };
                        var s = op == 'RawSave' ? success : null;
                        b.do_submit(form, oxdata, formObj.boKey, true, !preventLoadView, null, s, ignoreValidation, null, onpresubitsuccess);

                    } else {
                        b.do_submit(form, xdata, formObj.boKey, true, !preventLoadView, null, success, ignoreValidation, null, null);
                    }
                }

                var add_buttons = function (row) {
                    var tb = fc.find('.bocrud-toolbar-form-op:first');
                    tb.html('');

                    var add_static_btns = function (enableDraftSave) {
                        if (row['CanRefer'] == 'True' || row['CanRefer'] === true) {
                            $.bocrud.addButton(tb, {
                                caption: 'ارجاع',
                                buttonicon: 'icon-share bigger-160',
                                onClickButton: function () {
                                    b.userCmd(formObj.boKey, g, 'name=Refer|__Refer|');
                                },
                                showText: true,
                                classes: 'btn btn-inverse btn-sm'
                            });
                        }

                        if (enableDraftSave && row['CanSaveJob'] == 'True' || row['CanSaveJob'] === true) {
                            $.bocrud.addButton(tb, {
                                title: 'با زدن این دکمه اطلاعات فرم موقتا ذخیره می شود ولی کار از کارپوشه شما خارج نمی شود و موقتا بسته می شود. بعدا می توانید مجدد این کار را باز کنید و روی آن عملیات انجام دهید.',
                                caption: 'ذخیره پیشنویس',
                                buttonicon: 'icon-save bigger-160',
                                onClickButton: function () {
                                    var form = $('fieldset[name=WorkTab]').closest('.bocrud-page-form');
                                    form.find('.bocrud-control[name=SaveJob] :input').val('true');

                                    submit_func(true, {
                                        "@Operation": 'RawSave',
                                        "@JobContentMustSave": true
                                    });
                                },
                                showText: true,
                                classes: 'bocrud-tb-submit btn btn-primary btn-sm btn-draft-save'
                            });
                        }


                        $.bocrud.addButton(tb, {
                            title: 'انصراف، با زدن این دکمه کار بسته می شود ولی از کارپوشه شما خارج نمی شود تا بعدا مجدد بتوانید آن را باز کنید و روی آن عملیات انجام دهید.',
                            caption: 'بستن کار',
                            buttonicon: 'icon-ban bigger-160',
                            onClickButton: function () {
                                var b = $.workbox_layout.get_bocrud('__Box');
                                if (b.config.hideGrid) {
                                    if (window.opener != null) {
                                        window.close();
                                        return;
                                    } else {
                                        window.location = document.referrer;
                                        return;
                                    }
                                }
                                bCancel(true);
                            },
                            showText: true,
                            classes: 'bocrud-tb-cancel btn btn-warning btn-sm margin-left-12'
                        });


                        if (row['CanTakebackRefer'] == 'True' || row['CanTakebackRefer'] === true)
                            $.bocrud.addButton(tb, {
                                caption: 'بازپس گیری جریان',
                                buttonicon: 'icon-undo bigger-110',
                                onClickButton: function () {

                                },
                                showText: true,
                                classes: 'btn btn-inverse btn-sm'
                            });

                        if (row['CanReject'] == 'True' || row['CanReject'] === true)
                            $.bocrud.addButton(tb, {
                                caption: 'بازگشت ارجاع',
                                buttonicon: 'icon-reply bigger-110',
                                onClickButton: function () {

                                },
                                showText: true,
                                classes: 'btn btn-inverse btn-sm'
                            });

                        if (row['CanCustomSpin'] == 'True' || row['CanCustomSpin'] === true)
                            $.bocrud.addButton(tb, {
                                caption: 'گردش خاص',
                                buttonicon: 'icon-bolt bigger-110',
                                onClickButton: function () {

                                },
                                showText: true,
                                classes: 'btn btn-inverse btn-sm'
                            });
                    }

                    if (row['IsArchived'] != 'True' || row['IsArchived'] === true) {
                        $.ajax({
                            cache: false,
                            async: true,
                            showLoading: true,
                            msg: 'جهت دهی کار',
                            url: $.workbox_layout.url + 'Workbox/GetJobCommands?vn=' + new Date().getTime(),
                            data: { entry_id: formObj.boKey },
                            dataType: "text",
                            type: "POST",
                            error: function (data) {
                                $.console.error("bocrud-341:server return error." + data.responseText);
                            },
                            success: function (data) {
                                var station_config = eval('(' + data + ')');
                                if (station_config.error) {
                                    $.showException(station_config);
                                    return;
                                }

                                if (station_config.Commands.length == 0) {
                                    $.bocrud.addButton(tb, {
                                        buttonicon: 'icon-archive bigger-160',
                                        caption: 'تأیید و خروج',
                                        onClickButton: function () {
                                            tb.find('li').addClass('ui-state-disabled');
                                            submit_func(true, {
                                                "@Operation": 'Flowed',
                                                "@JobContentMustSave": false
                                            });
                                        },
                                        showText: true,
                                        classes: 'bocrud-tb-submit btn btn-inverse '
                                    });
                                }

                                for (var btn_index = 0; btn_index < station_config.Commands.length; ++btn_index) {
                                    var css = [];
                                    css.push('bocrud-tb-submit');
                                    css.push('btn');
                                    if (station_config.Commands[btn_index].Color && station_config.Commands[btn_index].Color.length > 0)
                                        css.push('btn-' + station_config.Commands[btn_index].Color);
                                    else
                                        css.push('btn-purple')
                                    if (btn_index == station_config.Commands.length - 1)
                                        css.push('margin-left-12');


                                    $.bocrud.addButton(tb, {
                                        buttonicon: 'icon-' + (station_config.Commands[btn_index].Icon && station_config.Commands[btn_index].Icon.length > 0 ? station_config.Commands[btn_index].Icon : 'arrow-right') + ' bigger-160',
                                        caption: station_config.Commands[btn_index].Caption,
                                        context: station_config.Commands[btn_index],
                                        onClickButton: function (cmd) {
                                            var b = $.workbox_layout.get_bocrud('__Box');
                                            var form = $('fieldset[name=WorkTab]').closest('.bocrud-page-form');

                                            form.find('.ui-tabs:first').tabs('option', 'active', 0);

                                            if (cmd.JobContentMustSave && !b.validate(form))
                                                return;

                                            form.find('.bocrud-control[name=SaveJob] :input').val(cmd.JobContentMustSave);
                                            var commandInfo = $('fieldset[name=CommandInfo]');

                                            tb.find('li').addClass('ui-state-disabled');

                                            if (cmd.RequireDescription || (cmd.AvailableRecievers && cmd.AvailableRecievers.length > 1)) {

                                                var modalContainer = commandInfo.parent();

                                                var parent = $('fieldset[name=WorkTab]').parent();
                                                var modal_buttons = {};
                                                modal_buttons[cmd.Caption] = function () {
                                                    if (cmd.RequireDescription) {
                                                        var bcontrol = $('.bocrud-control[name=CommandDescription]');
                                                        var txt = bcontrol.find(':input');
                                                        var desc = txt.val();
                                                        if (desc.length == 0) {
                                                            bcontrol.addClass('has-error');
                                                            return;
                                                        } else {
                                                            bcontrol.removeClass('has-error');
                                                        }
                                                    }
                                                    b.closeModal(commandInfo, parent, false);

                                                    if (!cmd.JobContentMustSave) {
                                                        b.addEvent('onprevalidation', function () {
                                                            return false;
                                                        }, 'workbox');
                                                    }

                                                    submit_func(true, {
                                                        "@Operation": 'Flowed',
                                                        "@WorkReportIsRequired": cmd.WorkReportIsRequired,
                                                        "@JobContentMustSave": cmd.JobContentMustSave
                                                    });

                                                    if (!cmd.JobContentMustSave) {
                                                        b.removeEvent('onprevalidation', 'workbox');
                                                    }

                                                };
                                                modal_buttons['انصراف'] = function () {
                                                    b.closeModal(commandInfo, parent, false);
                                                };

                                                b.openModal(commandInfo,
                                                    {
                                                        title: cmd.Caption,
                                                        appendTo: modalContainer,
                                                        buttons: modal_buttons,
                                                        open: function () {
                                                            commandInfo.find('.bocrud-control[name=JobCommand] :input').val(cmd.Id).blur();
                                                            var CommandTargets = commandInfo.find('[name=CommandTargets] select');
                                                            var commandTargetsContainer = CommandTargets.closest('.bocrud-control');
                                                            CommandTargets.html('');
                                                            var first_uid = '', count = 0;
                                                            for (var uid in cmd.AvailableRecievers) {
                                                                first_uid = uid;
                                                                CommandTargets.append('<option value="' + uid + '">' + cmd.AvailableRecievers[uid] + '</option>');
                                                                count++;
                                                            }
                                                            if (count == 1) {
                                                                console.log(CommandTargets.attr('id'));
                                                                $('#' + CommandTargets.attr('id')).val([first_uid]).trigger('change');
                                                                CommandTargets.prop("disabled", true);
                                                            } else {
                                                                CommandTargets.prop("disabled", false);
                                                            }
                                                            if (!cmd.AvailableRecievers || cmd.AvailableRecievers.length == 0)
                                                                commandTargetsContainer.hide();
                                                            else
                                                                commandTargetsContainer.show();

                                                        },
                                                        closed: function () {
                                                            b.closeModal(commandInfo, parent, false);
                                                            var CommandTargets = commandInfo.find('[name=CommandTargets] select');
                                                            CommandTargets.prop("disabled", false);
                                                        }
                                                    });
                                            } else {
                                                commandInfo.find('.bocrud-control[name=JobCommand] :input').val(cmd.Id).blur();
                                                submit_func(true, {
                                                    "@Operation": 'Flowed',
                                                    "@WorkReportIsRequired": cmd.WorkReportIsRequired,
                                                    "@JobContentMustSave": cmd.JobContentMustSave
                                                });
                                            }
                                        },
                                        showText: true,
                                        classes: css.join(' ')
                                    });
                                }


                                add_static_btns(station_config.EnableDraftSave);
                            } // success
                        });
                    } else { // can spin
                        add_static_btns(false);
                    }
                }
                var row = null;
                if (b.config.hideGrid) {
                    $.ajax({
                        cache: false,
                        showLoading: false,
                        url: $.workbox_layout.url + "Workbox/GetJobState?vn=" + new Date().getTime(),
                        data: { jobid: formObj.boKey },
                        dataType: "json",
                        type: "post",
                        async: false,
                        success: function (data) {
                            add_buttons(data);
                        }
                    });
                } else {
                    var g = b.grid();
                    row = g.jqGrid('getRowData', formObj.boKey);
                    add_buttons(row);
                }


            }

            $.workbox_layout.update_count_hint(this);
        },
        update_count_hint: function (b) {
            if (b.config.xml == '__Box' || b.config.xml == '__Comment') {
                var c = $('.bocrud-control[name=Comments]');
                var count = c.find('.row.bocrud-group').length;
                if (count > 0) {
                    var cid = c.closest('.ui-tabs-panel').attr('id');
                    var a = $('li.ui-tabs-tab[aria-controls=' + cid + '] a');
                    if (!a.data('org-text'))
                        a.data('org-text', a.text());

                    var t = a.data('org-text') + ' <span style="color:red">(' + count + ')</span>';
                    a.html(t);
                }
            }
        },
        onload: function (jsbid, url) {
            $.classic_layout.onload(jsbid);
            this.url = url;
            $('.wbx-folders a[data-action="reload"]')
                .click(this.reset_folders);

            var b = $.workbox_layout.get_bocrud('__Box');
            var gtb = $('.grid-toolbar[jsbid="' + b.config.id + '"]');
            gtb.addClass('btn-group');
            var b = [];
            b.push('<button data-toggle="dropdown" class="btn btn-inverse dropdown-toggle">');
            b.push('<span class="icon-caret-down icon-only"></span>');
            b.push('عملیات سریع');
            b.push('</button>');
            b.push(this.make_shortcut());
            gtb.append(b.join(''));

            $.workbox_layout.load_folderfilter();
            $.workbox_layout.load_subs();

            var wbxcmdmenu = gtb.find('.wbx-cmd-menu:first');
            wbxcmdmenu.find('a').click(function () {
                $.workbox_layout.onshortcut_click(this);
            });

            $.workbox_layout.load_notifier();
        },
        load_notifier: function () {
            if ($.workbox_layout.load_notifier_try_count == undefined) {
                $.workbox_layout.load_notifier_try_count = 10;
            }
            var t = $.workbox_layout;
            t.notifier = $.connection.notifier;
            if (!t.notifier || !t.notifier.client) {
                $.workbox_layout.load_notifier_try_count--;
                if ($.workbox_layout.load_notifier_try_count == 0) return;
                setTimeout(t.load_notifier, 500);
                return;
            }
            t.notifier.client.notify = function (strwbxj) {
                var wbj = eval('(' + strwbxj + ')');
                var b = $.workbox_layout.get_bocrud('__Box');
                var g = b.grid();
                g.jqGrid('addRowData', wbj.Id, wbj, 'first');
                t.ongridload();
                b._thumbnail_grid();
            };
            $.connection.hub.start().done(function () {
                console.log('hub started');
            });
        },
        onselectrow: function (gridObj, sn) {
        },
        onsubmitsuccess: function (formObj, saveResult, showDetails) {
            $.classic_layout.onsubmitsuccess(formObj, saveResult, showDetails);
            this.grid().trigger("reloadGrid");

        },
        reset_folders: function () {
            var _this_wbx = $.workbox_layout;
            _this_wbx.folders = undefined;
            $('#foldersContainer').html(' ');
            _this_wbx.load_folderfilter();
        },
        on_folder_select: function (event, data) {
            console.log(data);
        },
        load_folderfilter: function () {
            var _this_wbx = $.workbox_layout;

            $.workbox_layout.loading_folderfilter = true;

            _this_wbx.folders = $('#folders');

            if (_this_wbx.folders == undefined || _this_wbx.folders.length == 0) {
                return;
            }
            if (!$.jstree) {
                setTimeout(function () { _this_wbx.load_folderfilter() }, 500);
                return;
            }

            if (!_this_wbx.folders.hasClass('jstree')) {
                setTimeout(function () { _this_wbx.load_folderfilter() }, 500);
                return;
            }

            var fs = _this_wbx.folders;
            fs.bind('select_node.jstree deselect_node.jstree', function (e, data) {

                if (data.args.length < 3) {
                    console.log('ignore loading workbox twice!');
                    return;
                }

                var li = $(data.args[0]).closest('li');
                if (li.is('[isinactive]'))
                    return;

                if (!$(e.target).closest('li').is('[IsInactive]')) {
                    _this_wbx.doff();
                }

            });

            fs.find('li[color],li[background-color]').each(function (i, n) {
                var node = $(n);
                node.children('a').children('span').css({
                    color: '#' + node.attr('color'),
                    'background-color': '#' + node.attr('background-color')
                });//.addClass('wbx-label ui-corner-all');
            });
            fs.find('li[IsInactive]').each(function (i, n) {
                var node = $(n);
                node.children('a').children('ins.jstree-checkbox').remove();
            });
            //fs.find('li[IsDefault]').hide();
            fs.jstree('deselect_all');


        },
        load_subs: function () {
            var _this_wbx = this;

            var init_control = function (data) {
                var sbs = $('#wbx-substitute');

                sbs.empty();

                var v = [];
                for (var i = 0; i < data.length; ++i) {
                    sbs.append("<option value='" + data[i].AbsentUserId + "'>" + data[i].AbsentUserName + "</option>");
                    if (data[i].Selected)
                        v.push(data[i].AbsentUserId);
                }
                sbs.val(v);

                sbs.select2({
                    openOnEnter: true,
                    allowClear: true,
                    minimumResultsForSearch: 15
                }).on("change", function () {
                    var sbs = $('#wbx-substitute');
                    $.ajax({
                        cache: false,
                        showLoading: true,
                        msg: 'تغییر کارتابل',
                        url: _this_wbx.url + "Workbox/SetSubs?vn=" + new Date().getTime(),
                        dataType: "text",
                        type: "post",
                        async: false,
                        data: { absents: (sbs.val() || ['']).join(',') },
                        success: function (data) {
                            var b = _this_wbx.get_bocrud('__Box');
                            b.do_refresh(1, true, true);
                        }
                    });
                });


                $.ajax({
                    cache: false,
                    showLoading: true,
                    msg: 'وضعیت کنونی جانشینی',
                    url: _this_wbx.url + "Workbox/GetCurrentSubs?vn=" + new Date().getTime(),
                    dataType: "json",
                    type: "post",
                    async: false,
                    success: function (data) {
                        sbs.val(data);
                    }
                });
            }


            if (!_this_wbx.subs) {
                $.ajax({
                    cache: false,
                    showLoading: true,
                    msg: 'دریافت لیست کارتابل های قابل مشاهده',
                    url: _this_wbx.url + "Workbox/GetSubs?vn=" + new Date().getTime(),
                    dataType: "text",
                    type: "GET",
                    async: false,
                    success: function (data) {
                        try {
                            _this_wbx.subs = eval('(' + data + ')');

                            init_control(_this_wbx.subs);
                        }
                        catch (e) {
                            alert(e);
                        }
                    }
                });
            }


        },
        doff: function (node, state) {

            var _this_wbx = this;
            var fs = $('#foldersContainer');

            var selected_nodes = [];
            var persisted_nodes = [];
            var default_nodes = [];

            fs.find('a').each(function () {
                var a = $(this);
                var li = a.closest('li');
                var isPersisted = li.is('[IsPersisted]');
                var isDefault = li.is('[IsDefault]');

                if (a.hasClass('jstree-clicked')) {
                    if (!isPersisted)
                        selected_nodes.push(li);
                } else {
                    if (isPersisted)
                        persisted_nodes.push(li);
                }

                // if (init && isDefault)
                //   default_nodes.push(li);
            });

            var filter = [];


            var decorate = function (not, dom_node, root_filter) {

                var node = $(dom_node);

                if (!node.is('[IsInactive]') && node.attr('Filter')) {

                    var node_filter = eval('(' + node.attr('Filter') + ')');
                    var filters;
                    if ($.isArray(node_filter))
                        node_filter = node_filter[0];

                    node_filter.not = not;
                    root_filter.filters.push(node_filter);
                }
            }

            var root_filter = { not: false, groupOp: "OR", srchOnPrev: "And", filters: [] };

            for (var n = 0; n < selected_nodes.length; ++n) decorate(false, selected_nodes[n], root_filter);
            for (var n = 0; n < persisted_nodes.length; ++n) decorate(true, persisted_nodes[n], root_filter);
            if (selected_nodes.length == 0 && persisted_nodes.length == 0)
                for (var n = 0; n < default_nodes.length; ++n) decorate(true, default_nodes[n], root_filter);

            var bocrud = $.workbox_layout.get_bocrud('__Box');

            bocrud.setFilter(root_filter, true);
            bocrud.do_refresh();
        },
        data_loaded: false,
        ongridload: function () {
            $.workbox_layout.data_loaded = true;
            var b = $.workbox_layout.get_bocrud('__Box');
            var g = b.grid();

            g.find('td[aria-describedby=' + b.config.id + '-Grid_Subject]').each(function () {
                var a = $('<a/>').text($(this).text());
                $(this).html(a);
                a.click(function () {
                    var tr = $(this).closest('tr');
                    b.do_edit(tr.attr('id'), g);

                    g.find('tr.jqgrow.ui-state-highlight').removeClass('ui-state-highlight');
                    tr.addClass('ui-state-highlight');

                    if (!tr.hasClass('wbx-read-job')) {
                        tr.addClass('wbx-read-job');
                        tr.removeClass('wbx-unread-job');
                    }
                }).mousedown(function (e) {
                    if (e.which == 2 || (e.which == 1 && e.ctrlKey)) {
                        var tr = $(e.target).closest('tr');
                        var id = tr.attr('id');
                        var p = {
                            id: id,
                            sv: 1,
                            style: 'workbox',
                            mode: 'DataEntry'
                        };
                        $.bocrud.redirect('__Box', p, b.config.id, true, null, null, true);
                        if (!tr.hasClass('wbx-read-job')) {
                            tr.addClass('wbx-read-job');
                            tr.removeClass('wbx-unread-job');
                        }
                        e.preventDefault();
                    }
                });
            });
            $.workbox_layout.add_labels_to_rows();
        },
        add_labels_to_rows: function () {
            var _this_wbx = this;

            if (!_this_wbx.labels) {
                return;
            }

            if (!_this_wbx.data_loaded) {
                return;
            }

            var b = $.workbox_layout.get_bocrud('__Box');
            var g = b.grid();

            g.find('td[aria-describedby=' + b.config.id + '-Grid_Subject]').each(function () {
                var id = $(this).closest('tr').attr('id');
                var row = g.jqGrid('getRowData', id);
                var l = row['Labels'] + '|' + row["InstanceLabels"];
                if (row["HasComment"] == "True") l += '|CMNT'
                if (l) {
                    var ls = l.split(/\||,/);
                    for (var i = 0; i < ls.length; ++i) {
                        if (!ls[i] || ls[i].length == 0) continue;

                        var lobj = null;
                        for (var j = 0; j < _this_wbx.labels.length; ++j)
                            if (_this_wbx.labels[j].LatinName == ls[i]) {
                                lobj = _this_wbx.labels[j];
                                break;
                            }
                        if (lobj == null) continue;

                        _this_wbx.addLabelOffline(id, lobj);
                    }
                }
            });
        },
        make_shortcut: function () {
            var _this_wbx = this;
            if (!_this_wbx.labels)
                $.ajax({
                    cache: false,
                    showLoading: true,
                    msg: 'دریافت لیست برچسب‌ها',
                    url: _this_wbx.url + "Workbox/GetLabalsAsJson?vn=" + new Date().getTime(),
                    dataType: "text",
                    type: "GET",
                    async: false,
                    success: function (data) {
                        try {
                            _this_wbx.labels = eval('(' + data + ')');
                            $.workbox_layout.add_labels_to_rows();
                        }
                        catch (e) {
                            alert(e);
                        }
                    }
                });

            var b = $.workbox_layout.get_bocrud('__Box');
            var g = b.grid();

            if (_this_wbx.opt)
                return;

            _this_wbx.opt = true;

            var m = [];
            m.push('<ul class="wbx-cmd-menu dropdown-menu dropdown-inverse">');

            //m.push('<li><a href="javascript:void(0)" cmd="wbxop:workcycle" class="sf-with-ul">');
            //m.push('مشاهده چرخه کار</a></li>');

            m.push('<li><a href="javascript:void(0)" cmd="wbxop:agree" class="sf-with-ul">');
            m.push('<i class="ui-icon icon-arrow-right"></i>');
            m.push('روند پیش فرض</a></li>');

            //m.push('<li class="divider"></li>');
            //m.push('<li><a href="javascript:void(0)" cmd="wbxop:takeback" class="sf-with-ul">');
            //m.push('<i class="ui-icon icon-undo" ></i>');
            //m.push('بازپس گیری جریان کاری</a></li>');

            //m.push('<li><a href="javascript:void(0)" cmd="wbxop:takebackref" class="sf-with-ul">');
            //m.push('<i class="ui-icon icon-undo"></i>');
            //m.push('بازپس گیری ارجاعات</a></li>');

            m.push('<li class="divider"></li>');
            m.push('<li><a href="javascript:void(0)" cmd="mark:delete" class="sf-with-ul">انتقال به پوشه حذف شده‌ها</a></li>');
            m.push('<li class="divider"></li>');

            if (_this_wbx.labels && _this_wbx.labels.length > 0) {
                var anylabel = false;
                m.push('<li class="dropdown-hover dropdown-submenu"><a href="javascript:void(0)" tabindex="-1" class="clearfix">');
                m.push('زدن برچسب</a>')

                var fl = function (mu, per_instance) {
                    mu.push('<ul class="dropdown-menu dropdown-danger">');
                    for (var i = 0; i < _this_wbx.labels.length; ++i) {
                        var lobj = _this_wbx.labels[i];

                        if (per_instance && !lobj.IsPublic) continue;

                        if (!lobj.WorkboxFilter) {
                            mu.push('<li><a href="javascript:void(0)" id="');
                            mu.push(lobj.Id);
                            mu.push('" cmd="addlabel:');
                            mu.push(lobj.LatinName);
                            mu.push(',');
                            mu.push(per_instance);
                            mu.push('">');
                            mu.push('<span style="color:#');
                            mu.push(lobj.ForegroundColor);
                            mu.push(';background-color:#');
                            mu.push(lobj.BackgroundColor);
                            mu.push('">');
                            mu.push(lobj.Name);
                            mu.push('</span></a></li>');
                            anylabel = true;
                        }
                    }
                    if (!anylabel)
                        mu.splice(mu.length - 1, 1);
                    else
                        mu.push('</ul>');
                }

                m.push('<ul class="dropdown-menu dropdown-danger">'); {
                    m.push('<li class="dropdown-hover dropdown-submenu"><a href="javascript:void(0)" tabindex="-1" class="clearfix">'); {
                        m.push('به شکل عمومی');
                        m.push('</a>');
                        fl(m, true);
                    } m.push('</li>');

                    m.push('<li class="dropdown-hover dropdown-submenu"><a href="javascript:void(0)" tabindex="-1" class="clearfix">'); {
                        m.push('صرفا برای خودم');
                        m.push('</a>');
                        fl(m, false);
                    } m.push('</li>');
                } m.push('</ul></li>');


                anylabel = false;
                m.push('<li class="dropdown-hover dropdown-submenu"><a href="javascript:void(0)" tabindex="-1" class="clearfix">');
                m.push('حذف برچسب</a>')
                m.push('<ul class="dropdown-menu dropdown-danger">');
                for (var i = 0; i < _this_wbx.labels.length; ++i) {
                    var lobj = _this_wbx.labels[i];
                    if (!lobj.WorkboxFilter) {
                        m.push('<li><a href="javascript:void(0)" id="' + lobj.Id + '" cmd="removelabel:' + lobj.LatinName + '">');
                        m.push('<span label="" class="ui-corner-all" style="color:#' + lobj.ForegroundColor + ';background-color:#' + lobj.BackgroundColor + '">');
                        m.push(lobj.Name);
                        m.push('</span></a></li>');
                        anylabel = true;
                    }
                }

                if (!anylabel)
                    m.splice(m.length - 1, 1);
                else
                    m.push('</ul><li>');
            }


            m.push('<li class="dropdown-hover dropdown-submenu"><a href="javascript:void(0)" tabindex="-1" class="clearfix">');
            m.push('علامت گذاری</a>')
            m.push('<ul class="dropdown-menu dropdown-danger">');
            m.push('<li><a href="javascript:void(0)" cmd="mark:read" class="wbx-read-job sf-with-ul">باز شده</a></li>');
            m.push('<li><a href="javascript:void(0)" cmd="mark:unread" class="wbx-unread-job sf-with-ul">خوانده نشده</a></li>');

            m.push('<li class="divider"></li>');

            m.push('<li><a href="javascript:void(0)" cmd="mark:archive" class="sf-with-ul">بایگانی</a></li>');

            m.push('<li class="divider"></li>');

            m.push('<li><a href="javascript:void(0)" cmd="mark:recycle" class="sf-with-ul">');
            m.push('<i class="ui-icon icon-recycle"></i>');
            m.push('بازیابی</a></li>');

            m.push('</ul>');

            m.push('</li>');
            m.push('</ul>');

            return m.join('');
        },
        onshortcut_click: function (item) {
            var _this_wbx = this;
            var g = $.workbox_layout.get_bocrud('__Box').grid();
            var cmd = $(item).attr('cmd');
            var selarrrow = g.jqGrid('getGridParam', 'selarrrow');
            var arr = cmd.split(':');
            var op = arr[0];
            var arg = arr[1];
            if (op == "mark")
                _this_wbx.mark(arg, selarrrow);
            if (op == "addlabel") {
                var fa = arg.split(',');
                console.log(fa[1]);
                _this_wbx.addLabel(fa[0], selarrrow, true, fa[1] == 'true');
            }
            if (op == "removelabel")
                _this_wbx.addLabel(arg, selarrrow, false);
            if (op == "delete")
                _this_wbx.del(selarrrow, false);
            if (op == "wbxop")
                _this_wbx.wbxop(arg, selarrrow);
            if (op == "refer")
                _this_wbx.refer(selarrrow, g, true);
        },
        mark: function (arg, ids) {
            var _this = $.workbox_layout;
            $.ajax({
                cache: false,
                showLoading: true,
                msg: 'علامت گذاری کارها',
                url: this.url + "Workbox/Mark?vn=" + new Date().getTime(),
                data: {
                    ids: ids.toString(),
                    m: arg
                },
                dataType: "text",
                type: "post",
                success: function (data) {
                    var sucessIDs = eval("(" + data + ")");
                    var g = $.workbox_layout.get_bocrud('__Box').grid();
                    for (var i = 0; i < sucessIDs.length; ++i) {
                        var tr = g.find('tr#' + sucessIDs[i]);
                        if (_this.canShowRow(arg) === false) {
                            g.jqGrid("delRowData", sucessIDs[i]);
                            continue;
                        }

                        if (arg == 'read') {
                            if (!tr.hasClass('wbx-read-job'))
                                tr.addClass('wbx-read-job');
                            if (tr.hasClass('wbx-unread-job'))
                                tr.removeClass('wbx-unread-job');
                        } else if (arg == 'unread') {
                            if (!tr.hasClass('wbx-unread-job'))
                                tr.addClass('wbx-unread-job');
                            if (tr.hasClass('wbx-read-job'))
                                tr.removeClass('wbx-read-job');
                        } else if (arg == 'archive') {
                            if (!tr.hasClass('wbx-arh-job'))
                                tr.addClass('wbx-arh-job');
                        } else if (arg == 'delete') {
                            if (!tr.hasClass('wbx-trb-job'))
                                tr.addClass('wbx-trb-job');
                        } else if (arg == 'recycle') {
                            tr.removeClass('wbx-trb-job');
                            tr.removeClass('wbx-arh-job');
                        }
                    }
                    g.jqGrid('resetSelection');
                }
            });
        },
        canShowRow: function (mark) {
            var _this = $.workbox_layout;
            var mark_field = {
                read: [{ f: 'IsRead', v: 'true' }],
                unread: [{ f: 'IsRead', v: 'false' }],
                archive: [{ f: 'IsArchived', v: 'true' }],
                'delete': [{ f: 'IsDeleted', v: 'true' }],
                recycle: [{ f: 'IsArchived', v: 'false' }, { f: 'IsDeleted', v: 'false' }]
            };

            var result = true;
            _this.folders.find('a').each(function () {
                var a = $(this);
                var li = a.closest('li');
                var isPersisted = li.is('[IsPersisted]');
                var isDefault = li.is('[IsDefault]');
                var isSelected = a.hasClass('jstree-clicked');
                if (isSelected || isPersisted) {
                    var node_filter = eval('(' + li.attr('Filter') + ')');
                    if ($.isArray(node_filter))
                        node_filter = node_filter[0];
                    var not = (isPersisted && !isSelected) || node_filter.not;
                    for (var k = 0; k < node_filter.rules.length && result; ++k) {
                        var rule = node_filter.rules[k];
                        for (var j = 0; j < mark_field[mark].length && result; ++j) {
                            var f = mark_field[mark][j].f, v = mark_field[mark][j].v;
                            if (rule.field == f) {
                                result = rule.data == v;
                                break;
                            }
                        }
                    }
                }
            });
            return result;
        },
        addLabel: function (target, id_array, addTag, per_instance) {
            var _this_wbx = this;

            if (addTag && _this_wbx.currentLabel == target)
                return;

            if (per_instance && addTag) {
                var instance_ids_array = [];
                var g = $.workbox_layout.get_bocrud('__Box').grid();
                for (var i = 0; i < id_array.length; ++i) {
                    row = g.jqGrid('getRowData', id_array[i]);
                    instance_ids_array.push(row["InstanceId"]);
                }
                id_array = instance_ids_array;
            }

            var l = _this_wbx.getItem(target, _this_wbx.labels);

            $.ajax({
                cache: false,
                showLoading: true,
                msg: 'پیوست و حذف برچسب از کارها',
                url: this.url + (addTag ? "Workbox/AddLabel" : "Workbox/RemoveLabel"),
                data: {
                    ids: id_array.toString(),
                    label: target,
                    per_instance: per_instance
                },
                dataType: "text",
                type: "post",
                success: function (data) {
                    var sucessIDs = eval("(" + data + ")");

                    var b = $.workbox_layout.get_bocrud('__Box');
                    var g = b.grid();

                    if (per_instance && addTag) {
                        var ids = g.jqGrid('getDataIDs');
                        var new_row_ids = [];
                        for (i = 0; i < ids.length; ++i) {
                            row = g.jqGrid('getRowData', ids[i]);
                            for (var j = 0; j < sucessIDs.length; ++j)
                                if (row["InstanceId"] == sucessIDs[j]) {
                                    new_row_ids.push(ids[i]);
                                    break;
                                }
                        }
                        sucessIDs = new_row_ids;
                    }

                    for (var i = 0; i < sucessIDs.length; ++i) {
                        var id = sucessIDs[i];

                        if (!id || id.length == 0) continue;

                        var tr = b.grid().find('tr#' + id);
                        var td = tr.find('td[aria-describedby="' + b.config.id + '-Grid_Subject"]');

                        if (addTag) {
                            if (td.find('span.wbx-label[label="' + l.LatinName + '"]').length == 0) {
                                if (target == 'arh' || target == 'trb') {
                                    g.jqGrid('delRowData', id);
                                } else {
                                    _this_wbx.addLabelOffline(id, l);
                                }
                            }
                        } else {
                            if (l.LatinName == _this_wbx.currentLabel)
                                g.jqGrid('delRowData', id);
                            td.find('span.wbx-label[label="' + l.LatinName + '"]').remove();
                        }
                    }
                }
            });
        },

        addLabelOffline: function (rowid, labelObj, tooltip) {
            var failLabel = {
                LatinName: 'error',
                Name: 'خطا',
                BackgroundColor: '990000',
                ForegroundColor: 'FF7451'
            }, succLabel = {
                LatinName: 'success',
                Name: 'انجام شد',
                BackgroundColor: '006600',
                ForegroundColor: '99FF66'
            };

            if (labelObj === 'fail') labelObj = failLabel;
            if (labelObj === 'succ') labelObj = succLabel;
            var b = $.workbox_layout.get_bocrud('__Box');
            var tr = $('tr#' + rowid);
            var td = tr.find('td[aria-describedby="' + b.config.id + '-Grid_Subject"]');
            td.prepend('<span style="background-color:#' + labelObj.BackgroundColor +
                ';color:#' + labelObj.ForegroundColor + ';" label="' + labelObj.LatinName +
                '" class="wbx-label ui-corner-all" title="' + tooltip + '">' + labelObj.Name + '</span>&nbsp;');
            if (tooltip && tooltip.length > 0)
                tr.find('.wbx-label').tooltip({
                    content: false,
                    show: 'mouseover',
                    hide: 'mouseout'
                });
        },

        refer: function (selarrrow, gridObj, bulk) {
            var _this_wbx = $.workbox_layout;
            var wbxb = $.workbox_layout.get_bocrud('__Box');

            wbxb.showDialog({
                text: {
                    bCancel: $.jgrid.edit.bCancel,
                    bSubmit: 'ارسال'
                },
                xml: 'Refer',
                container_id: 'referDialog',
                getData: {
                    "@CurrentWorkboxID": _this_wbx.currWbxID,
                    "@StrWbxjIDs": selarrrow
                },
                submitData: {
                    "@CurrentWorkboxID": _this_wbx.currWbxID,
                    "@StrWbxjIDs": selarrrow,
                    genjson: true
                },
                onSubmitSucess: function (rawReponse, dialog, jsonReponse, jsbo) {
                    if (jsonReponse.error)
                        return false;

                    if (jsbo.WorksCount == 1) {
                        var ss = jsbo.ReferResult.Successes;
                        var hasError = false;

                        if (jsbo.ReferResult.Fails.length > 0) {
                            alert(jsbo.ReferResult.Fails[0].ErrorMessage);
                            hasError = true;
                        }

                        var grid = dialog.find('.bocrud-property[name="Items"] .ui-jqgrid-btable:eq(0)');
                        var rows = grid.find('tbody:eq(0)>tr.jqgrow[role=row]');
                        var ssRowIds = [];
                        var errmsgs = [];

                        for (var wbxjid in ss)
                            rows.each(function (i) {
                                if (ss[wbxjid][i] == '') {
                                    var row = $(this);
                                    ssRowIds.push(row.attr('id'));
                                } else {
                                    errmsgs.push(ss[wbxjid][i]);
                                }
                            });

                        if (ssRowIds.length == 0 && hasError)
                            return false;

                        for (var i = 0; i < ssRowIds.length; ++i)
                            grid.jqGrid('delRowData', ssRowIds[i]);

                        if (errmsgs.length > 0) {
                            var msg = [];
                            msg.push("<ol style='color:red'>");
                            for (var i = 0; i < errmsgs.length; ++i) {
                                msg.push("<li>" + errmsgs[i] + "</li>");
                            }
                            msg.push("</ol>");

                            var pivot = grid.closest('.ui-jqgrid');

                            var prev = pivot.prev();
                            if (prev.hasClass('error'))
                                prev.html(msg.join(""));
                            else
                                pivot.before($("<div class='error'>" + msg.join("") + "</div>"));

                            return false;
                        }
                        if (confirm('آیا آیتم ارجاع شده بایگانی گردد ؟')) {
                            _this_wbx.mark('archive', selarrrow);
                        }
                    } else {
                        var fails = jsbo.ReferResult.Fails;
                        for (var i = 0; i < fails.length; ++i) {
                            _this_wbx.addLabelOffline(fails[i].ID, 'fail', fails[i].ErrorMessage);
                        }
                        var succs = jsbo.ReferResult.Successes;
                        var succIds = [];
                        for (var id in succs) {
                            succIds.push(id);
                            _this_wbx.addLabelOffline(id, 'succ');
                        }
                        if (confirm('آیا آیتم های ارجاع شده بایگانی گردد ؟')) {
                            _this_wbx.mark('archive', succIds.join(','));
                        }
                    }



                    return true;
                }
            });
        },
        getItem: function (label, source) {
            for (var i = 0; i < source.length; ++i)
                if (source[i].LatinName == label)
                    return source[i];
            return null;
        },
        addLabelOffline: function (rowid, labelObj, tooltip) {
            var failLabel = {
                LatinName: 'error',
                Name: 'خطا',
                BackgroundColor: '990000',
                ForegroundColor: 'FF7451'
            }, succLabel = {
                LatinName: 'success',
                Name: 'انجام شد',
                BackgroundColor: '006600',
                ForegroundColor: '99FF66'
            };

            if (labelObj === 'fail') labelObj = failLabel;
            if (labelObj === 'succ') labelObj = succLabel;

            var wbxb = $.workbox_layout.get_bocrud('__Box');
            var tr = wbxb.grid().find('tr#' + rowid);
            var td = tr.find('td[aria-describedby="' + wbxb.config.id + '-Grid_Subject"]');
            td.prepend('<span style="background-color:#' + labelObj.BackgroundColor +
                ';color:#' + labelObj.ForegroundColor + ';" label="' + labelObj.LatinName +
                '" class="wbx-label ui-corner-all" title="' + tooltip + '">' + labelObj.Name + '</span>&nbsp;');
            if (tooltip && tooltip.length > 0)
                tr.find('.wbx-label').tooltip({
                    content: false,
                    show: 'mouseover',
                    hide: 'mouseout',
                    container: 'body'
                });
        },

        wbxop: function (op, selarrrow) {
            var _this_wbx = $.workbox_layout;
            if (!selarrrow || selarrrow.length == 0)
                return;

            var wbxb = $.workbox_layout.get_bocrud('__Box');

            if (op == 'agree' && !confirm('آیا از تأیید کار(ها) بدون باز کردن آن اطمینان دارید ؟'))
                return;
            if (op == 'disagree' && !confirm('آیا از عدم تأیید کردن کار(ها) بدون باز کردن آن اطمینان دارید ؟'))
                return;
            if (op == 'takeback' && !confirm('آیا از بازپس گیری جریان کاری کار(ها) بدون باز کردن آن اطمینان دارید ؟'))
                return;

            switch (op) {
                case 'workcycle':
                    this._workCycle(selarrrow[0]);
                    break;
                case 'agree':
                case 'disagree':
                case 'takeback':
                    $.ajax({
                        cache: false,
                        showLoading: true,
                        msg: 'حرکت جریان کاری',
                        url: "/Workbox/WbxOp",
                        dataType: "text",
                        type: "post",
                        async: true,
                        data: {
                            op: op,
                            ids: selarrrow.join(',')
                        },
                        success: function (data) {
                            var json = eval('(' + data + ')');

                            var fails = json.Fails;
                            for (var i = 0; i < fails.length; ++i) {
                                _this_wbx.addLabelOffline(fails[i].Id, 'fail', fails[i].ErrorMessage);
                            }
                            var succs = json.Successes;
                            for (var id in succs) {
                                _this_wbx.addLabelOffline(id, 'succ');
                            }
                        },
                        error: function () {
                            result = false;
                        }
                    });
                    break;
                case 'takebackref':
                    wbxb.showDialog({
                        text: { bSubmit: '', bCancel: 'بستن' },
                        xml: 'TakebackRefer',
                        container_id: 'takebackrefDialog',
                        getData: {
                            "@CurrentWorkboxID": _this_wbx.currWbxID,
                            "@CurrentWorkboxJobID": selarrrow[0]
                        },
                        submitData: {
                            "@CurrentWorkboxID": _this_wbx.currWbxID,
                            "@CurrentWorkboxJobID": selarrrow[0],
                            genjson: true
                        },
                        onPreSubmit: function (dialog) {
                        },
                        onSubmitSucess: function (rawReponse, dialog, jsonReponse, boAsJson) {
                            if (boAsJson) {
                                var fails = boAsJson.ReferResult.Fails;

                                for (var i = 0; i < fails.length; ++i) {
                                    _this_wbx.addLabelOffline(fails[i].Id, 'fail', fails[i].ErrorMessage);
                                }
                                var succs = boAsJson.ReferResult.Successes;
                                for (var id in succs) {
                                    _this_wbx.addLabelOffline(id, 'succ');
                                }
                            }
                            return true;
                        }
                    });
                    break;
            } // if refer                    
        },
        onpresearch: function (orginal_data, new_data) {
            new_data['cusfilt'] = orginal_data['cusfilt'];
        }
    });

    $.bocrud.globalEvents.push({
        type: 'ondeletedone',
        key: 'workbox_layout',
        func: function (ids) {
            $.workbox_layout.update_count_hint(this);
        }
    });

})(jQuery);