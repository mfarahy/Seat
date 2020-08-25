function Random() {
    this.bool = function () {
        return this.number(0, 100) > 50;
    }

    this.number = function (min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    this.word = function (min, max) {
        var l = [];
        for (var i = 0; i < this.number(min, max) ; ++i)
            l.push(String.fromCharCode(this.number('a'.charCodeAt(0), 'z'.charCodeAt(0))));
        return l.join('');
    }
    this.sentence = function (max) {
        var l = [];
        for (var i = 0; i < this.number(4, 10) ; ++i)
            l.push(this.word(4, 10));
        var s = l.join(' ');
        return s.length > max ? s.substring(0, max) : s;
    }

    this.select2 = function (input) {
        var options = input.find('option[value]')
        .filter(function () {
            return $(this).attr('value').length > 0;
        });
        var count = options.length;
        if (count > 0) {
            do {
                var op = $(options[this.number(0, count - 1)]).attr('value');
                if (op.length > 0 || op.length <= 1) {
                    return op;
                }
            } while (true);
        } else
            return -1;
    }

    this.textbox = function (input, min, max) {
        var bc = input.closest('.bocrud-control');
        var maxLength = max || parseInt(input.attr('maxLength'));
        if (maxLength.toString() == 'NaN' || !maxLength)
            maxLength = 50;
        if (!min)
            min = 0;

        var name = input.closest('.bocrud-control').attr('name');

        if (name.indexOf('LatinName') >= 0)
            return this.word(6, Math.min(maxLength, 10));
        else
            if (name.indexOf('Name') >= 0)
                return this.word(6, Math.min(maxLength, 10));

        if (name.indexOf('Code') >= 0)
            return this.number(1111, 9999).toString();

        if (name.indexOf('Desc') >= 0)
            return this.sentence(maxLength);

        return this.word(min, maxLength);
    }
}

function tester(scenario, o) {
    this.rnd = new Random();
    this.c = $('#console');
    this.win = $('body #win');
    var _this = this;

    var log = function (message) {
        for (var i = 1; i < arguments.length; ++i)
            message = message.replace('{' + (i - 1).toString() + '}', arguments[i]);
        var logitem = $('<div>').text(message);
        _this.c.prepend(logitem);
        return logitem;
    }
    var err = function (message) {
        error = true;
        log.apply(this, arguments).addClass('has-error');
        t.run('err');
    }
    var trace = function (message) {
        log.apply(this, arguments).addClass('trace');
    }
    var debug = function (message) {
        log.apply(this, arguments).addClass('debug');
    }
    var user_log = function (message) {
        log.apply(this, arguments).addClass('user-log');
    }

    var index = -1;
    var h = '';
    var t = this;
    var tryCount = 0;
    var success = false;
    var error = false;
    var options = o || {
        delay: 500
    };
    var context = {};
    var namespace = '';

    this.run = function (caller) {
        var waited = null;
        var waitCount = 0;
        var f = function () {
            if (t.pending_ajax > 0 && waitCount < 120) {
                if (waited) {
                    var timer = waited.find('.timer');
                    if (timer.length == 0) {
                        timer = ctx.$('<span class="timer"/>');
                        waited.append(timer);
                    }
                    timer.text('(' + (120 - waitCount) + ')');
                }
                else
                    waited = log('wait for {0} thread to complete (max 60 sec) ', t.pending_ajax);
                ++waitCount;
                setTimeout(f, 500);
                return;
            } else {
                t.pending_ajax = 0;
            }
            waitCount = 0;
            t.__run(caller);
        }
        if (options.delay)
            setTimeout(f, options.delay);
        else
            f();
    }

    this.get_index = function () {
        return index;
    }

    this.__run = function (caller) {
        if (error) {
            log('test complete with no success !');
            if ($.isFunction(options.complete))
                options.complete(false);
            return;
        }

        index++;
        if (scenario.length <= index) {
            if ($.isFunction(options.complete))
                options.complete(success);
            if (!h) {
                if (!success) {
                    err('test complete with no success !');
                } else {
                    log('test complete successfuly.');
                }
            }
            return;
        }
        var l = [];
        if (namespace.length > 0)
            l.push(namespace + ' - ');
        l.push(index + '- ');
        l.push(scenario[index].t);
        var args = [];
        for (var k in scenario[index]) {
            if (k == 't' || typeof (k).toString() == 'object') continue;

            l.push(k);
            l.push(':');
            l.push(scenario[index][k]);
            args.push({ n: k, v: scenario[index][k] });
        }
        l.push(' called by ' + caller);

        if ($.isFunction(scenario[index].t)) {
            setTimeout(function () {
                scenario[index].t(scenario[index]);
            }, 300);

            return;
        }
        trace(l.join(' '));
        //setTimeout(function () {
        try {
            if (t[scenario[index].t](scenario[index]) < 0) {
                index--;
                tryCount++;
                if (tryCount < 5) {
                    log('action {0} cannt done its job in {1}th run and we run it again', scenario[index + 1].t, tryCount);
                    setTimeout(function () { t.run(caller); }, 300);
                } else {
                    err('action {0} cannt done its job in {1}th run', scenario[index + 1].t, tryCount);
                }
            } else {
                tryCount = 0;
            }
        }
        catch (exception) {
            var sargs = [];
            for (var k = 0; k < args.length; ++k)
                sargs.push(args[k].n + ":" + args[k].v);
            err('An exception occured in executing {0} with arguments {1} !', scenario[index].t, sargs.join(', '));
            err(exception);
        }
        //}, 1000);
    }

    this.init = function (p) {
        h = p.host;

        this.c = $('#console');
        this.win = $('#win');

        t.run('init');
    }

    var open_page = function (url, onload) {
        if ($.isFunction(onload)) {
            _this.win.one("load", onload);
        }

        _this.win.attr('src', h + url);
    }

    this.login = function (p) {
        var t = this;
        if (!window.loginAs || window.loginAs != p.username) {
            open_page(p.url || 'Account/Login', function () {
                t.win.contents().find('#UserName').val(p.username);
                t.win.contents().find('#Password').val(p.password);
                t.win.contents().find('button[type=submit],input[type=submit]').click();
                t.win.one('load', function () {
                    window.loginAs = p.username;
                    t.run('login');
                });
            });
        } else
            t.run('login');
    };

    this.logout = function (p) {
        var t = this;

        t.win.one('load', function () {
            t.run('logout');
        });
        ctx.$('#logoutForm').submit();
    };
    var ctx = {};

    this.inherit = function (p) {
        namspace = p.namespace;
        h = p.of.h;
        ctx.xml = p.workon.config.xml;
        ctx.b = p.workon;
        ctx.$ = t.win.get(0).contentWindow.$;
        t.run('inherit');
    }

    this.redirect = function (p) {
        if (p.url) {
            open_page(p.url, function () {
                t.run('redirect');
            });
        }
    }

    this.open = function (p) {
        var t = this;

        if (!p.xml) {
            err('please specify xml for open command !');
            return;
        }

        open_page('Browse/' + p.xml + '?TEST_MODE=true&' + (p.params ? '&' + p.params : ''), function () {
            ctx.xml = p.xml.toLowerCase();

            var bs = $('iframe').get(0).contentWindow.bocruds;
            var b = null;
            for (var bid in bs)
                if (bs[bid].config.xml.toLowerCase() == ctx.xml) {
                    b = bs[bid]; break;
                }
            if (b) {
                b.config.saveStates = false;

                ctx.b = b;

                if (!ctx.bstack) {
                    ctx.bstack = [];
                } else
                    ctx.bstack.splice(0, ctx.bstack.length);

                if (b.grid().jqGrid("getDataIDs").length == 0) {
                    var cindex = p.index || index;
                    scenario.splice(cindex + 1, 0, { t: 'wait', e: 'ongridcomplete' });
                    setTimeout(function () {
                        if (cindex + 1 == t.get_index()) {
                            // یعنی گرید خیلی سریع تر از این حرف ها لود شده و یا اینقدر سنگین هست که هنوز لود نشده
                            if (!ctx.$('#load_' + ctx.b.config.id + '-Grid').is(':visible')) {
                                //یعنی گرید لود شده تموم شده ولی سیستم گیر کرده
                                log('we think that test was stop because cindex:{0} remain {1} is not increase and get decide to continue any way...', cindex, t.get_index());
                                t.run('open');
                                return;
                            }
                        }
                    }, 30000);
                }

                ctx.$ = t.win.get(0).contentWindow.$;

                if (ctx.b.config.showEmpty) {//fFlightFavor-MultiUpdate
                    ctx.form = ctx.$('#f' + ctx.b.config.id);
                }

                t.run('open');

                log('set current bocrud to {0}.', b.config.id);

                t.win.get(0).contentWindow.$.bocrud.globalEvents.push({
                    type: 'onAjaxSend',
                    key: 'test',
                    func: function (xhr, settings) {
                        if (scenario.length > index && scenario[index + 1] && scenario[index + 1].t == 'wait') return;

                        var url = settings.url;
                        if (url.indexOf('/signalr/') >= 0)
                            url = "signalr ping";
                        else {
                            t.pending_ajax++;
                            if (url.indexOf(h) >= 0) {
                                if (url.lastIndexOf('?') > 0)
                                    url = url.substring(h.length, url.lastIndexOf('?'));
                                else
                                    url = url.substring(h.length);
                            }
                            if (!t.ajax_reqs)
                                t.ajax_reqs = [];

                            t.ajax_reqs.push({ item: log('Ajax {0}', url), url: settings.url });
                        }
                    }
                });

                t.win.get(0).contentWindow.$.bocrud.globalEvents.push({
                    type: 'onAjaxComplete',
                    key: 'test',
                    func: function (xhr, settings) {
                        var url = settings.url;
                        if (url.indexOf('/signalr/') >= 0)
                            url = "signalr ping";
                        else {
                            t.pending_ajax--;
                            if (t.pending_ajax <= 0)
                                t.pending_ajax = 0;

                            if (url.indexOf(h) >= 0) {
                                if (url.lastIndexOf('?') > 0)
                                    url = url.substring(h.length, url.lastIndexOf('?'));
                                else
                                    url = url.substring(h.length);
                            }
                            var i = -1;
                            for (var j = 0; j < t.ajax_reqs.length; ++j)
                                if (t.ajax_reqs[j].url == settings.url) {
                                    i = j; break;
                                }
                            if (i >= 0) {
                                t.ajax_reqs[i].item.css({ color: 'green' });
                            }
                        }
                    }
                });

                window.onerror = function (error) {
                    err(error);
                    return;
                };

                t.pending_ajax = 0;
                t.ajax_reqs = [];

                t.win.get(0).contentWindow.$.showException = function (excep, containerId, dialog) {
                    var exception;
                    try {
                        exception = typeof (excep) == 'string' ? JSON.parse(excep) : excep;
                    }
                    catch (e) { return; }
                    err(exception.message);
                };
            } else {
                return -1;
            }
        });
    }

    this.new = function (p) {
        var b = ctx.b;

        var puc = 0;

        b.addEvent('onaddnew', function (formObj) {
            if (ctx.b.config.mode == "GridDataEntry") {
                var grid = ctx.$('#' + ctx.b.config.id + '-Grid');
                ctx.form = grid.find('tr.bocrud-page-form:first');
            } else {
                var formid = 'f' + ctx.b.config.id;
                var form = ctx.$('#' + formid);
                ctx.form = form;
            }

            var run = function () {
                if (puc <= 0) {
                    b.removeEvent('onpartialupdate', 'tester-addnew');
                    b.removeEvent('onpartialupdatecomplete', 'tester-addnew');
                    t.run('new');
                }
                else {
                    log('wait for init partial updates done');
                    setTimeout(run, 3000);
                }
            };

            if (formObj.detailPos == 'normaldialog')
                b.addEvent('modalshowcomplete', function (formObj) {
                    setTimeout(run, 3000);

                    b.removeEvent('modalshowcomplete', 'tester');
                }, 'tester');
            else {
                setTimeout(run, 3000);
            }

            b.removeEvent('onaddnew', 'tester');
        }, 'tester');

        b.addEvent('onpartialupdate', function (dialogObj, pid, xml, ps, sups, ud, fake) {
            log('a partial update going to run');
            puc++;
        }, 'tester-addnew');

        b.addEvent('onpartialupdatecomplete', function (mode, dialogObj) {
            log('a partial update done');
            puc--;
        }, 'tester-addnew');

        b.addNew();
    }

    this.change = function (p) {
        var t = this;
        var form = ctx.form;

        if (!ctx.form && p.f && p.f != "*") {
            ctx.form = ctx.$('[name=' + p.f + '].bocrud-control').closest('.bocrud-page-form');
        }

        if (p.tab >= 0) {
            var tabs = ctx.form.children('div.bocrud-form-tabs:first');
            if (tabs.is('.tabbable')) {
                if (!tabs.children('.tab-content')
                    .children('.tab-pane[index="' + p.tab + '"]').is(':visible')) {
                    scenario.splice(index + 1, 0, { t: 'tab', i: p.tab });
                    scenario.splice(index + 2, 0, $.extend(p, { tab: p.tab }));
                    t.run('change-457');
                    return;
                }
            }
            if (tabs.is('.ui-tabs')) {
                if (tabs.tabs("option", "active") != p.tab) {
                    scenario.splice(index + 1, 0, { t: 'tab', i: p.tab });
                    scenario.splice(index + 2, 0, $.extend(p, { tab: p.tab }));
                    t.run('change-465');
                    return;
                }
            }
        } else {
            if (p.f == '*') {
                var tabs = ctx.form.children('div.bocrud-form-tabs:first');
                if (tabs.length > 0) {
                    var tabsCount = tabs.children('ul').children().length;
                    for (var i = 1; i < tabsCount; ++i) {
                        scenario.splice(index + i, 0, $.extend(p, { tab: i }));
                    }
                }
            }
        }

        if (!form || !form.data) return -1;

        var vo = {};
        if (form.data('formObj'))
            vo = form.data('formObj').validationObj;

        var f = null;
        if (p.f != "*") {
            f = [];
            var field_names = p.f.split(',');

            for (var fi = 0; fi < field_names.length; ++fi) {
                var bc = form.find('.bocrud-control[name=' + field_names[fi] + ']');

                if (bc.length == 0) {
                    err('control {0} not found !', field_names[fi]);
                    return;
                }

                if (bc.is(':visible')) {
                    form.find('.bocrud-control[name=' + field_names[fi] + ']:visible')
                        .each(function () {
                            $(this).removeClass('modified-by-tester');
                            f.push(this);
                        });
                } else {
                    var parent = bc.closest('.bocrud-form-tabs,.bocrud-page-form');
                    if (parent.is('.bocrud-page-form')) {
                        err('control {0} is not visible !', field_names[fi]);
                        return;
                    } else {
                        var tab_index = bc.closest('.tab-pane,.ui-tabs-panel').attr('index');
                        scenario.splice(index + 1, 0, { t: 'tab', i: tab_index });
                        scenario.splice(index + 2, 0, $.extend(p, { tab: p.tab }));
                        t.run('change-515');
                        return;
                    }
                }
            }

            if (f.length == 0 && form.is('.ui-search-toolbar')) {
                var search_input = form.find('input[name="' + p.f + '"]');
                f.push(search_input);
            }
        }
        else {
            f = form.find('.bocrud-control:not(.modified-by-tester):visible')
            .filter(function () {
                return $(this).closest('.bocrud-page-form').attr('id') == ctx.form.attr('id');
            });
        }

        if (f.length == 0) {
            err('cannot find any control({0}) to change value !', p.f);
            t.run('change');
            return;
        }
        var skip = [];
        for (var i = 0; i < f.length; ++i) {
            var bc = ctx.$(f[i]);

            switch (bc.attr('type')) {
                case 'Trigger':
                case 'PartialView':
                    continue;
                    break;
            }

            var find = function (name) {
                var found = false;
                for (var i = index; i < scenario.length ; ++i) {
                    if (scenario[i].f == name && scenario[i].t == 'change') {
                        found = true; break;
                    }
                }
                return found;
            }

            //if (p.f == '*') {
            if (!bc.is('.scheduled-by-tester')) {
                var affecton = bc.attr('affecton');
                if (affecton && affecton.length > 0) {
                    var affectonarr = affecton.split(',');
                    var name = bc.attr('name');

                    var key = t.rnd.number(0, 100000);
                    scenario.splice(index + 1, 0, { t: 'wait', key: key, e: 'onpartialupdatedone' });
                    if (!find(name)) {
                        scenario.splice(index + 1, 0, { t: 'change', f: name, v: '*' });
                        skip.push(name);
                    }

                    setTimeout(function () {
                        if (index < scenario.length &&
                            scenario[index].t == 'wait' &&
                            scenario[index].e == 'onpartialupdatedone' &&
                            scenario[index].key == key &&
                            t.pending_ajax == 0) {
                            // یعنی گیرکرده و پارشال آپدیت سریع اجرا شده و تموم شده

                            ctx.b.removeEvent('onpartialupdatedone', 'tester' + key);

                            log('skip waiting !');

                            t.run('change-585');
                        }
                    }, 7000);

                    var last_change_inde = -1;
                    for (last_change_inde = index;
                        last_change_inde < scenario.length &&
                        scenario[last_change_inde].t == 'change' &&
                        (!scenario[last_change_inde].tab || scenario[last_change_inde].tab == p.tab) ;
                        ++last_change_inde);

                    debug('last change index is : {0} for control {1}', last_change_inde, name);

                    for (var j = 0; j < affectonarr.length; ++j) {
                        skip.push(affectonarr[j]);

                        if (!find(affectonarr[j]))
                            scenario.splice(index + 3 + j, 0, { t: 'change', f: affectonarr[j], v: '*' });
                    }

                    bc.addClass('scheduled-by-tester');
                }
            }
            // }
        }

        for (var i = 0; i < f.length; ++i) {
            var bc = ctx.$(f[i]);

            //var affecton = bc.attr('affecton');
            //if (affecton && affecton.length > 0 && p.f != '*') {
            //    scenario.splice(index + 1, 0, { t: 'wait',  e: 'onpartialupdatedone' });
            //}

            var name = bc.attr('name');

            var mustSkip = false;
            for (var j = 0; j < skip.length; ++j)
                if (skip[j] == name) {
                    mustSkip = true; break;
                }

            if (mustSkip) {
                log('let control {0} change value in next.', name);
                continue;
            }

            if (bc.find('.bucrud-disabled-action').length > 0) {
                log('control {0} was disabled and we skip of it', name);
                continue;
            }

            if (bc.is('.modified-by-tester')) continue;

            switch (bc.attr('type')) {
                case 'Trigger':
                case 'PartialView':
                    log('>> {0} control not supported !', bc.attr('type'));
                    break;
                case 'RuntimeDefined':
                    var runtime_container = bc.find('.uie-runtime-defined');
                    log('try to find runtime control');
                    if (runtime_container.length != 0) {
                        var type = runtime_container.attr('type');
                        log('runtime control found of type {0} and let control value changed in next.', type);
                        scenario.splice(index + 1, 0, $.extend({}, p, { t: type, a: 'change', fn: name, bc: bc, vo: vo }));
                        bc.addClass('modified-by-tester');
                    } else
                        return -1;
                    break;
                default:
                    scenario.splice(index + 1, 0, $.extend({}, p, { t: bc.attr('type'), a: 'change', fn: name, bc: bc, vo: vo }));
                    break;
            }
        }
        t.run('change-660');
    }

    this.focus = function (p) {
        if (p.on == 'search') {
            ctx.form = ctx.$('#search-' + ctx.b.config.id);
            var pg = ctx.form.closest('.panel-group');
            //search-ReceiptCashDoc-container
            if (ctx.$('#search-' + ctx.b.config.id + '-container').is(':visible')) {
                t.run('focus-669');
                return;
            }
            ctx.$("#b-" + ctx.b.config.id + " #" + ctx.b.config.id + '-sacc')
                .on('shown.bs.collapse', function () {
                    t.run('focu-674');
                }).collapse('show');
            return;
        }
        if (p.on == 'grid-search') {
            ctx.form = ctx.b.$('#gbox_' + ctx.b.config.id + '-Grid .ui-search-toolbar:first');
            t.run('focus-680');
            return;
        }
    }

    this.search = function (p) {
        if (ctx.form.is('.ui-search-toolbar')) {
            scenario.splice(index + 1, 0, { t: 'wait', e: 'ongridcomplete' });

            t.run('search-689');

            ctx.form.find('.ui-search-input input:first').trigger($.Event('keypress', { which: 13 }));
        } else {
            var search_was_run = false;
            var search_checker_handler;
            var ongridcomplete = false, onfindbtnclicked = false;

            var op = function () {
                if (ongridcomplete && onfindbtnclicked) {
                    search_was_run = true;

                    clearTimeout(search_checker_handler);
                    search_checker_handler = false;
                    ctx.b.removeEvent('onfindbtnclicked', 'tester-search-find');
                    ctx.b.removeEvent('ongridcomplete', 'tester-search-find');

                    log('both events onfindbtnclicked and ongridcomplete was run and we continue...');
                    t.run('search-707');
                }
            }

            ctx.b.addEvent('onfindbtnclicked', function () {
                log('search onfindbtnclicked event was run');
                onfindbtnclicked = true;
                op();
                return true;
            }, 'tester-search-find');

            ctx.b.addEvent('ongridcomplete', function () {
                log('search ongridcomplete event was run');
                ongridcomplete = true;
                op();
                return true;
            }, 'tester-search-find');

            var find = function () {
                if (!search_checker_handler || !search_was_run) {
                    ctx.b.do_find();

                    search_checker_handler = setTimeout(find, 4000);
                }
            }
            setTimeout(find, 1000);

        }
    }
    this.reset = function (p) {
        scenario.splice(index + 1, 0, { t: 'wait', e: 'ongridcomplete' });
        t.run('reset');
        setTimeout(function () {
            ctx.b.do_reset_search();
        }, 1000);
    }

    this.FileControl = function (p) {
        if (p.a == 'assert') {
            err('FileControl assertion not implement yet !');
            return;
        }

        var bc = p.bc;
        var file = bc.find('input.multi:first');
        if (file.length == 0) return -1;
        bc.addClass('modified-by-tester');
        var file_id = file.attr('id');
        bc.find('input').remove();
        var fake_file = $('<input type="hidden"/>');
        fake_file.attr('id', file_id).attr('name', file_id).val('__test_file');
        bc.append(fake_file);
        log('set {0} to test file.', bc.attr('name'));
        t.run('FileControl');
    }

    this.Many2One = function (p) {
        if (p.a == 'assert') {
            err('Many2One assertion not implement yet !');
            return;
        }

        var bc = p.bc;
        var m2obid = bc.attr('id');
        var bs = t.win.get(0).contentWindow.bocruds;
        m2obid = m2obid.substring(0, m2obid.length - '-holder'.length);
        var m2ob = null;
        for (var bid in bs)
            if (bs[bid].config.id.indexOf(m2obid) > 0) {
                m2ob = bs[bid]; break;
            }
        if (m2ob) {
            log('i find m2o {0}', bid);

            var op = function () {
                var ids = m2ob.grid().jqGrid('getDataIDs');

                log('we find {0} row for control {1}', ids.length, bc.attr('name'));

                if (modalshowcomplete && (ongridload || ids.length > 0)) {
                    var rid = ids[t.rnd.number(0, ids.length)];
                    m2ob.grid().jqGrid('setSelection', rid, true);

                    m2ob.addEvent('modalclose', function () {
                        var sel = ctx.$('#' + m2obid);
                        var try_count = parseInt(sel.data('try-count'));
                        if (isNaN(try_count))
                            try_count = 1;

                        if (try_count > 5) {
                            if (ids.length == 0) {
                                log('there is no record to select for control {0}', bc.attr('name'));
                                log('we skip of set value for control {0}', bc.attr('name'));
                                t.run('Many2One-800');
                                return;
                            }
                            err('Many2One can not set value {0} for control {1}', rid, bc.attr('name'));
                        }

                        if (sel.val() != rid && try_count <= 5) {
                            sel.data('try-count', try_count + 1);
                            index--;
                        }
                        m2ob.removeEvent('modalclose', 'tester');

                        t.run('Many2One');
                    }, 'tester');

                    ctx.b.m2o_ondblClick(rid, m2obid);

                    bc.addClass('modified-by-tester');
                }
            };

            var modalshowcomplete = false, ongridload = false;

            m2ob.addEvent('modalshowcomplete', function () {
                log('m2o modalshowcomplete event ran');
                modalshowcomplete = true;
                m2ob.removeEvent('modalshowcomplete', 'tester');
                op();
                m2ob.do_refresh();

            }, 'tester');

            m2ob.addEvent('ongridload', function () {
                log('m2o ongridload event ran');
                ongridload = true;
                m2ob.removeEvent('ongridload', 'tester');
                op();
            }, 'tester');

            bc.find('button.uie-many-to-one:first')
            .click();
        }
    }

    this.DateTimePicker = function (p) {
        var bc = p.bc;
        var input = bc.find('.bocrud-input:not(.modified-by-tester)');

        var v = '';

        if ($.isFunction(p.v)) {
            p.v = p.v(input.val(), context);
        }

        if (p.a == 'assert') {
            if (input.val() != p.v)
                err('assertion fail web expect "{1}" for field "{0}" realy has "{2}".', p.fn, p.v, input.val());

            t.run('DateTimePicker-857');

            return;
        }

        //2015/07/09
        if (p.v == '*') {
            var culture = 'Gregorian';
            if (input.is('.bocrud-mycalendar') && input.attr('culture') == 'Persian')
                culture = 'Persian';

            var begin_year = 2013, end_year = 2015;
            if (culture == 'Persian') {
                begin_year = 1390;
                end_year = 1394;
            }

            var vr = p.vo.rules[input.attr('id')] || {};

            var by = t.rnd.number(begin_year, end_year), bm = t.rnd.number(1, 12), bd = t.rnd.number(1, 29);
            if (vr.compare) {
                var cc = ctx.form.find('.bocrud-control[name=' + vr.compare.control + '] .uie-datetime:input');
                var ccval = cc.val();
                if (ccval && cc.length) {
                    var date = ccval.split('/');
                    by = parseInt(date[0]);
                    bm = parseInt(date[1]);
                    bd = parseInt(date[2]);
                }
                if (vr.compare.op == 'ge' || vr.compare.op == 'g') {
                    bm += t.rnd.number(0, 12 - bm);
                    bd += t.rnd.number(0, 29 - bd);
                }
            }

            v = by
                + '/'
                + bm
                + '/'
                + bd;
        } else
            v = p.v;

        input.val(v).trigger("change");
        bc.addClass('modified-by-tester');
        log('set {0} to {1}.', bc.attr('name'), v);

        t.run('DateTimePicker-904');
    }

    this.Time = function (p) {
        var bc = p.bc;
        var input = bc.find('.bocrud-input:not(.modified-by-tester)');
        var vr = p.vo.rules[input.attr('id')] || {};
        var v = '';

        if ($.isFunction(p.v)) {
            p.v = p.v(input.val(), context);
        }

        if (p.a == 'assert') {
            if (input.val() != p.v)
                err('assertion fail web expect "{1}" for field "{0}" realy has "{2}".', p.fn, p.v, input.val());

            t.run('Time-921');

            return;
        }

        if (p.v == '*') {
            v = t.rnd.number(0, 23) + ':' +
                t.rnd.number(0, 59);
        } else
            v = p.v;

        input.val(v).trigger("change");
        bc.addClass('modified-by-tester');
        log('set {0} to {1}.', bc.attr('name'), v);

        t.run('Time-936');
    }

    this.TextBox = function (p) {
        var bc = p.bc;
        var input = bc.find('.bocrud-input:not(.modified-by-tester)');
        var vr = p.vo ? p.vo.rules[input.attr('id')] || {} : {};
        var v = '';

        if ($.isFunction(p.v)) {
            p.v = p.v(input.val(), context);
        }

        if (p.a == 'assert') {
            if (input.val() != p.v)
                err('assertion fail web expect "{1}" for field "{0}" realy has "{2}".', p.fn, p.v, input.val());

            t.run('TextBox-953');

            return;
        }

        if (p.v == '*') {
            if (vr.number == true)
                v = t.rnd.number(0, 100);
            else
                v = t.rnd.textbox(input, vr.minlength || 1, vr.maxlength);
        } else
            v = p.v;

        input.val(v).trigger("change");
        bc.addClass('modified-by-tester');
        log('set {0} to {1}.', bc.attr('name'), v);

        t.run('TextBox-970');
    }

    this.SimpleTreeSelector = function (p) {
        var bc = p.bc;
        var input = bc.find('.bocrud-input:not(.modified-by-tester)');
        var v = '';

        if ($.isFunction(p.v)) {
            p.v = p.v(input.val(), context);
        }

        if (p.a == 'assert') {
            if (input.val() != p.v)
                err('assertion fail web expect "{1}" for field "{0}" realy has "{2}".', p.fn, p.v, input.val());

            t.run('TextBox-953');

            return;
        }

        if (p.v == '*') {
            if (vr.number == true)
                v = t.rnd.number(0, 100);
            else
                v = t.rnd.textbox(input, vr.minlength || 1, vr.maxlength);
        } else
            v = p.v;

        input.val(v).trigger("change");
        bc.addClass('modified-by-tester');
        log('set {0} to {1}.', bc.attr('name'), v);

        t.run('TextBox-970');
    }

    this.text = function (p) {
        var bc = p.bc;

        var v = '';

        if ($.isFunction(p.v)) {
            p.v = p.v(bc.val(), context);
        }

        bc.val(p.v).trigger("change");
        bc.addClass('modified-by-tester');
        log('set search field {0} to {1}.', bc.attr('name'), v);

        t.run('text');
    }

    this.CheckBox = function (p) {
        var bc = p.bc;
        var input = bc.find('.bocrud-input:not(.modified-by-tester)');
        var v = '';

        if ($.isFunction(p.v)) {
            p.v = p.v(input.is(':checked'), context);
        }

        if (p.a == 'assert') {
            if (input.is(':checked') != p.v)
                err('assertion fail web expect "{1}" for field "{0}" realy has "{2}".', p.fn, p.v, input.is(':checked'));

            t.run('CheckBox-1002');

            return;
        }

        if (p.v == '*') {
            v = t.rnd.number(0, 100) > 50;
        } else
            v = p.v;

        if (input.is(':checked')) {
            if (!v)
                input.click();
        } else
            if (v)
                input.click();

        bc.addClass('modified-by-tester');
        log('set {0} to {1}.', bc.attr('name'), v);

        t.run('CheckBox-1022');
    }

    this.Select2 = function (p) {
        var bc = p.bc;
        var input = bc.find('select.bocrud-input:not(.modified-by-tester)');
        var v = p.v;

        if (p.v == '*') {
            v = t.rnd.select2(input);
            if (v == -1) {
                log('there is no item for {0} to select !', p.fn);
                t.run('Select2-1034');
                return;
            }
        } else {
           v= input.find('option').filter(function () {
                var ov = $(this).val();
                var ot = $(this).text();
                if (v == ov || ot.trim() == v) return true;
                return false;
            }).val();
        }

        if ($.isFunction(v)) {
            v = v(input.select2('val'), context);
        }

        if (p.a == 'assert') {
            if (input.select2('val') != v)
                err('assertion fail web expect "{1}" for field "{0}" realy has "{2}".', p.fn, p.v, input.val());

            t.run('Select2-1047');

            return;
        }

        if (ctx.$.fn.select2 && input.select2) {
            if ($.isFunction(p.v)) {
                v = p.v(input.select2('val'), context);
                v = v == '*' ? t.rnd.select2(input) : v;
            }

            input.val(v).trigger('change');

            if ((!$.isArray(v) && input.select2('val') != v) ||
                ($.isArray(v) && input.select2('val').length != v.length)) {
                log('select2 value {0} is not equal with {1} !', input.select2('val'), v);
                return -1;
            }
            console.log('select2 ', bc.attr('name'), ' changed');

            bc.addClass('modified-by-tester');
            log('set {0} to {1}.', bc.attr('name'), v);

            t.run('Select2-1071');
        }
        else
            return -1;
    }

    this.One2Many = function (p) {
        var bs = t.win.get(0).contentWindow.bocruds;
        var bcid = p.bc.attr('id');
        //FlightFavorConfigurations-p-1581
        bcid = bcid.substring(0, bcid.length - '-holder'.length);
        var o2mb = null;
        for (var bid in bs)
            if (bs[bid].config.id.indexOf(bcid) > 0) {
                o2mb = bs[bid]; break;
            }
        if (o2mb) {
            if (p.a == 'assert') {
                if (typeof (p.v) == "number") {
                    var data_count = o2mb.grid().jqGrid('getDataIDs');
                    if (data_count.length != p.v)
                        err('assertion fail we expect "{0}" number of rows but realy grid has "{1}" records.', p.v, data_count.length);
                }
                return;
            }

            p.bc.addClass('modified-by-tester');
            if (p.op == 'delete') {
                var delete_btn = ctx.$('#' + o2mb.config.id + 'del:visible');
                if (delete_btn.length > 0) {
                    {
                        if (p.i == '*' || !p.i) {
                            var ids = o2mb.grid().jqGrid('getDataIDs');

                            if (ids.length > 0 && (p.op == 'delete' || t.rnd.number(0, 100) > (100 / ids.length))) {
                                var rnd = t.rnd.number(0, ids.length - 1);
                                p.id = ids[rnd];
                            }
                        }

                        log('remove item {0} from grid {1}.', p.id, p.bc.attr('name'));
                        o2mb.do_del(p.id, o2mb.grid(), function () {
                            t.run('One2Many-1113');
                        }, true);
                    }
                }
            }
            if (!p.op || p.op == 'add') {
                var addNew = ctx.$('#' + o2mb.config.id + 'addNew:visible');
                if (addNew.length > 0) {
                    var subscenario = [
                        { t: 'inherit', of: t, workon: o2mb, namespace: namespace + '-' + index },
                        { t: 'new' },
                    ];

                    if (p.v && $.isArray(p.v)) {
                        for (var i = 0; i < p.v.length; ++i)
                            subscenario.push(p.v[i]);
                    } else {
                        subscenario.push({ t: 'change', f: '*', v: '*' });
                    }

                    if (o2mb.config.mode != 'GridDataEntry')
                        subscenario.push({ t: 'save' });

                    subscenario.push({ t: 'success' });

                    log('add random generated item to grid {0}.', p.bc.attr('name'));

                    new tester(subscenario, {
                        complete: function () {
                            t.run('One2Many-1142');
                        }
                    }).run();
                    return;
                }
            }
        }
    }

    this.Many2Many = function (p) {
        if (p.a == 'assert') {
            err('Many2Many assertion not implement yet !');
            return;
        }

        var bc = p.bc;
        var bcid = bc.attr('id');//p376014370-holder
        bcid = bcid.substring(0, bcid.length - '-holder'.length);
        p.bc.addClass('modified-by-tester');

        if (!p.op || p.op == 'add') {
            ctx.b.addEvent('modalshowcomplete', function (div, o) {
                var bs = $('iframe').get(0).contentWindow.bocruds;

                var subscenario = [
                        { t: 'inherit', of: t, workon: bs['p-m2m-' + bcid], namespace: namespace + '-' + index },
                ];

                ctx.b.removeEvent('modalshowcomplete', 'tester-m2m-modalshowcomplete');

                if (p.v && $.isArray(p.v)) {
                    for (var i = 0; i < p.v.length; ++i)
                        subscenario.push(p.v[i]);
                    subscenario.push({ t: 'success' });

                    new tester(subscenario, {
                        complete: function () {
                            scenario.splice(index + 1, 0, { t: 'wait', e: ['modalclose', 'onm2mrefreshvaluedone'] });
                            t.run('Many2Many-1180');

                            var orgDiv = ctx.$('#' + bcid + 'BocrudContainer');
                            var buttons = orgDiv.dialog('option').buttons;
                            buttons[ctx.$.bocrud.select.bSelect].apply(orgDiv);
                        }
                    }).run();
                }
                else
                    err('not support');

                return true;
            }, 'tester-m2m-modalshowcomplete');

            bc.find('#p-m2m-' + bcid + 'SubGridAdd').click();
        }
        if (p.op == 'delete') {
            var grid = bc.find('#' + bcid + '-grid');

            if (p.i == '*') {
                var ids = grid.jqGrid('getDataIDs');

                if (ids.length > 0 && (p.op == 'delete' || t.rnd.number(0, 100) > (100 / ids.length))) {
                    var rnd = t.rnd.number(0, ids.length - 1);
                    p.id = ids[rnd];
                }
            }

            log('try remove item {0} from grid {1}.', p.id, p.bc.attr('name'));

            grid.jqGrid('setSelection', p.id);

            scenario.splice(index + 1, 0, { t: 'wait', e: 'onm2mrefreshvaluedone' });
            t.run('Many2Many-1213');

            bc.find('#p-m2m-' + bcid + 'SubGridDel').click();
        }
    }

    this.tab = function (p) {
        var tabs = ctx.form.children('div.bocrud-form-tabs:first');

        if (tabs.is('.tabbable')) {
            tabs.find('li:eq(' + p.i + ') a').one('shown.bs.tab', function (e) {
                t.run('tab-1224');
            }).tab('show');
        }
        if (tabs.is('.ui-tabs')) {
            if (tabs.tabs("option", "active") == p.i) {
                t.run('tab-1229');
                return;
            }
            tabs.one('tabsactivate', function () {
                t.run('tab-1233');
            });
            tabs.tabs({
                active: p.i,
            });
        }
    }

    this.save = function (p) {
        var $$ = this.win.get(0).contentWindow.$;
        var bc = $$('#b-' + ctx.b.config.id + ".bocrud-container");

        var event = 'onsubmitsuccess';
        if (ctx.b.config.mode == "GridDataEntry" && ctx.b.config.standalone)
            event = 'oninlinesubmitsuccess';

        ctx.b.addEvent(event, function (formObj, response) {
            if (formObj && formObj.boKey && scenario[index + 1].t == 'edit') {
                scenario.splice(index + 1, 0, { t: 'select_row', id: formObj.boKey });
            }

            if ($.isFunction(p.onComplete))
                p.onComplete(formObj.boKey, context);

            ctx.b.removeEvent(event, 'tester');
            t.run('save-1258');
        }, 'tester');

        ctx.b.addEvent('onvalidationfailed', function () {
            err('validation failed ! system cannot continue test process !');
            ctx.b.removeEvent('onvalidationfailed', 'tester');
        }, 'tester');

        var toolbar = bc.find('.bocrud-toolbar-form-op:first');
        if (toolbar.length != 1) {
            if (ctx.form.is('.ui-dialog-content')) {
                ctx.form.next().find('.btn-primary:first').click();
            } else if (ctx.form.parent().is('.ui-dialog-content')) {
                ctx.form.parent().next().find('.btn-primary:first').click();
            } else
                bc.find('.inline-edit-toolbar .bocrud-tb-submit').click();
            return;
        } else {
            toolbar.find('.bocrud-tb-submit:first')
            .click();
        }
    }

    this.wait = function (p) {
        var key = p.key || t.rnd.number(0, 10000);

        log('wait for event {0} fo bocrud {1} with key {2}', p.e, ctx.b.config.id, key);

        ctx.b.addEvent(p.e, function () {
            log('event {0} with key {1} was triggered', p.e, key);
            ctx.b.removeEvent(p.e, 'tester' + key);
            t.run('wait');
            return true;
        }, 'tester' + key);
    }

    this.select_row = function (p) {
        var select_form = function (rowid) {
            if (rowid.length == 1 && rowid[0]) {
                var tr = ctx.b.grid().find('tr#' + rowid[0]);
                if (tr.is('.bocrud-page-form')) {
                    ctx.form = tr;
                }
            }
        }

        if (p.id) {

            var dataIDs = ctx.b.grid().jqGrid('getDataIDs');

            if ($.isFunction(p.id)) {
                p.id = p.id(context, dataIDs).toString();
            }

            var selected_ids = [];
            var ids = p.id.split(',');
            for (var i = 0; i < ids.length; ++i) {
                var pid = ids[i];

                if (pid == '*') {
                    var rindex = _this.rnd.number(0, dataIDs.length - 1);
                    var id = dataIDs[rindex];

                    dataIDs.splice(rindex, 1);

                    selected_ids.push(id);
                } else
                    selected_ids.push(pid);
            }
            ctx.b.grid().jqGrid('resetSelection');
            for (var i = 0; i < selected_ids.length; ++i)
                ctx.b.grid().jqGrid('setSelection', selected_ids[i], false);

            ctx.b.refresh_toolbar();

            select_form(selected_ids);

            if (selected_ids.length == 0 || !selected_ids[0]) {
                err('there is no row to select !');
                return;
            }

            log('select rows with id in [{0}]', selected_ids.join(','));

            var selarrrow = ctx.b.grid().jqGrid('getGridParam', 'selarrrow');
            var selrow = ctx.b.grid().jqGrid('getGridParam', 'selrow');
            var multiselect = ctx.b.grid().jqGrid('getGridParam', 'multiselect');
            var selected_rows = [];
            if (multiselect)
                selected_rows = selarrrow;
            else
                selected_rows = [selrow];
            if (selected_ids.length != selected_rows.length) {
                return -1;
            }


            setTimeout(function () {
                t.run('select_row-1356');
            }, 1000);

            return;
        }
        if (p.x) {
            var multiselect = ctx.b.grid().jqGrid('getGridParam', 'multiselect');
            var data = ctx.b.grid().jqGrid('getRowData');
            var data_ids = ctx.b.grid().jqGrid('getDataIDs');
            var row = [];
            for (var i = 0; i < data.length; ++i) {
                data[i].id = data_ids[i];

                for (var k in data[i])
                    data[i][k] = $('<div/>').html(data[i][k]).text().trim();

                if (p.x(data[i], context)) {
                    row.push(data_ids[i]);
                    if (!multiselect)
                        break;
                }
            }
            ctx.b.grid().jqGrid('resetSelection');
            if (row.length == 0) {
                err('cannot find any row !');
                return;
            }
            for (var i = 0; i < row.length; ++i)
                ctx.b.grid().jqGrid('setSelection', row[i], false);

            ctx.b.refresh_toolbar();

            log('select rows with id in [{0}]', row.join(','));
            select_form(row);

            setTimeout(function () {
                t.run('select_row-1392');
            }, 1000);
            return;
        }
        if (p.i >= 0 || p.i == '*') {
            var data_ids = ctx.b.grid().jqGrid('getDataIDs');
            if (p.i == '*')
                p.i = t.rnd.number(0, data_ids.length - 1);
            if (p.i > data_ids.length - 1) {
                err('cannot find row at {0} !', p.i);
                return;
            }
            ctx.b.grid().jqGrid('resetSelection');
            ctx.b.grid().jqGrid('setSelection', data_ids[p.i], false);

            ctx.b.refresh_toolbar();

            log('select row {0}', data_ids[p.i]);

            select_form([data_ids[p.i]]);

            setTimeout(function () {
                t.run('select_row-1414');
            }, 1000);
            return;
        }
    }

    this.workbox = function (p) {
        if (p.cmd) {
            var $$ = ctx.$;
            var toolbar = ctx.b.$('#f__Box').next();

            ctx.b.addEvent('onsubmitsuccess', function (formObj, response) {
                scenario.splice(index + 1, 0, { t: 'refresh_grid' });
                ctx.b.removeEvent('onsubmitsuccess', 'tester');
                t.run('workbox-1428');

            }, 'tester');

            ctx.b.addEvent('onvalidationfailed', function () {
                err('validation failed ! system cannot continue test process !');
                ctx.b.removeEvent('onvalidationfailed', 'tester');
            }, 'tester');

            var btn = toolbar.find('button')
                .filter(function () {
                    return $(this).text().trim() == p.cmd;
                }).filter(':first');

            if (btn.length == 0) {
                return -1;
            }

            var cmd = btn.data('context');
            if (cmd.RequireDescription || cmd.AvailableRecievers) {
                ctx.b.addEvent('modalshowcomplete', function () {
                    var commandInfo = $('fieldset[name=CommandInfo]');
                    commandInfo.find(':input').each(function () {
                        var input = $$(this);
                        if (input.is('textarea')) {
                            t.rnd.textbox(input, 1, 200);
                        }
                    });
                    commandInfo.next().find('button:first').click();

                    ctx.b.removeEvent('modalshowcomplete', 'workbox-tester');
                }, 'workbox-tester');
            }

            btn.click();
        }
    }

    this.cmd = function (p) {
        var bc = ctx.$('#b-' + ctx.b.config.id + ".bocrud-container");

        if (p.name) {
            var btn = bc.find('button#' + ctx.b.config.id + p.name + 'ubtn:first');

            if (btn.length == 0) {
                var selarrrow = ctx.b.grid().jqGrid('getGridParam', 'selarrrow');
                var selrow = ctx.b.grid().jqGrid('getGridParam', 'selrow');
                var multiselect = ctx.b.grid().jqGrid('getGridParam', 'multiselect');
                var selected_rows = [];
                if (multiselect)
                    selected_rows = selarrrow;
                else
                    selected_rows = [selrow];

                var random_row = _this.rnd.number(0, selected_rows.length - 1);
                var tr = ctx.b.grid().find('tr#' + selected_rows[random_row]);
                btn = tr.find('button[name=' + p.name + ']:first');
            }
        }

        if (btn.length == 0) {
            err('command {0} not found !', p.name);
            return;
        }

        ctx.b.addEvent('onusercmd', function (data, showAsMain) {
            var before = false, after = false;

            before = data['before'] && data['before'].length > 0;
            after = data['after'] && data['after'].length > 0;

            if (before) {
                var e = 'modalshowcomplete';
                if (showAsMain)
                    e = 'oncmdbeforeshown';
                ctx.b.addEvent(e, function (div, o) {
                    ctx.form = div;

                    t.run('cmd-1500');

                    //var event = ['ongridcomplete'];
                    if (data['after'] && data['after'].length > 0) {
                        //event.push('modalshowcomplete');

                        ctx.b.addEvent('modalshowcomplete', function (div, o) {
                            ctx.form = div;
                            ctx.b.removeEvent('modalshowcomplete', 'tester.cmd.after');
                        }, 'tester.cmd.after');
                    }

                    //ctx.b.addEvent(event, function () {
                    //    ctx.b.removeEvent('ongridcomplete', 'tester');
                    //    t.run('cmd-1514');
                    //}, 'tester');

                    ctx.b.removeEvent('modalshowcomplete', 'tester-before');
                }, 'tester-before');
            } else {
                if (after) {
                    ctx.b.addEvent('modalshowcomplete', function (div, o) {
                        ctx.form = div;
                        ctx.b.removeEvent('modalshowcomplete', 'tester.cmd.after');
                    }, 'tester.cmd.after');
                }
            }

            if (!before) {
                ctx.b.addEvent('ongridcomplete', function (div, o) {
                    ctx.b.removeEvent('ongridcomplete', 'tester-after');
                    t.run('cmd-1534');
                }, 'tester-after');
            }

        }, 'tester');

        ctx.b.addEvent('oncmdsuccessbeforeevalresponse', function (r) {
            log('Cmd success beforeeval response');
            ctx.b.removeEvent('oncmdsuccessbeforeevalresponse', 'tester');

            if (r.indexOf('$.bocrud.redirect(') >= 0) {
                if (r.indexOf('http://') > 0) {
                    log('http redirect is not support by tester system !');
                    t.run('cmd-1535');
                    return;
                }
                var indexOfCama = r.indexOf(',');
                var sp = '$.bocrud.redirect('.length;
                var xml = r.substring(sp + 1, indexOfCama - 1);
                var params = r.substring(indexOfCama + 2, r.length - 2);
                //index--;
                scenario.splice(index + 1, 0, { t: 'open', xml: xml, params: params, index: index + 1 });
                t.run('cmd-1544');
            }

        }, 'tester');
        btn.click();
    }

    this.form_assert = function (p) {
        var t = this;
        var form = ctx.form;

        var f = form.find('.bocrud-control[name="' + p.f + '"]');

        if (f.length == 0) {
            err('cannot find control {0} to assert value !', p.f);
            return -1;
        }
        for (var i = 0; i < f.length; ++i) {
            var bc = ctx.$(f[i]);
            var name = bc.attr('name');

            switch (bc.attr('type')) {
                case 'RuntimeDefined':
                    var runtime_container = bc.find('.uie-runtime-defined');
                    log('try to find runtime control');
                    if (runtime_container.length != 0) {
                        var type = runtime_container.attr('type');
                        log('runtime control found of type {0} and assert control value in next.', type);
                        scenario.splice(index + 1, 0, { t: type, a: 'assert', fn: name, bc: bc, f: p.f, v: p.v });
                        bc.addClass('modified-by-tester');
                    } else
                        return -1;
                    break;
                default:
                    scenario.splice(index + 1, 0, { t: bc.attr('type'), a: 'assert', fn: name, bc: bc, f: p.f, v: p.v });
                    break;
            }
        }
        t.run('form_assert');
    }

    this.grid_assert = function (p) {
        var data_ids = ctx.b.grid().jqGrid('getDataIDs');

        if (p.count >= 0) {
            if ($.isFunction(p.count))
                p.count = p.count(null, context);

            var records = ctx.b.grid().jqGrid('getGridParam', 'records');

            if (p.count != records) {
                err('assertion fail we expect "{0}" number of rows but realy grid has "{1}" records.', p.count, records);
                return;
            } else {
                t.run('grid_assert');
                return;
            }
        }

        if ($.isFunction(p.x)) {
            var multiselect = ctx.b.grid().jqGrid('getGridParam', 'multiselect');
            var data = ctx.b.grid().jqGrid('getRowData');
            var data_ids = ctx.b.grid().jqGrid('getDataIDs');
            var row = [];
            for (var i = 0; i < data.length; ++i) {
                data[i]['id'] = data_ids[i];
                if (p.x(data[i], context)) {
                    row.push(data_ids[i]);
                    if (!multiselect)
                        break;
                }
            }
            p.id = row.join(',');
        }

        if (p.i == 'last')
            p.id = data_ids[data_ids.length - 1];
        if (p.i >= 0 || p.i == '*') {
            if (p.i == '*')
                p.i = t.rnd.number(0, data_ids.length - 1);
            if (p.i > data_ids.length - 1) {
                err('cannot assert row at {0} !', p.i);
                return;
            }
            p.id = data_ids[p.i];
        }

        if (!p.id) {
            var selarrrow = ctx.b.grid().jqGrid('getGridParam', 'selarrrow');
            var selrow = ctx.b.grid().jqGrid('getGridParam', 'selrow');
            var multiselect = ctx.b.grid().jqGrid('getGridParam', 'multiselect');
            var selected_rows = [];
            if (multiselect)
                selected_rows = selarrrow;
            else
                selected_rows = [selrow];

            p.id = selected_rows[0];
        }
        var row = ctx.b.grid().jqGrid('getRowData', p.id);

        if ($.isArray(row) && row.length > 0) {
            err('there are many row to assert !');
            return;
        }

        if ($.isFunction(p.v))
            p.v = p.v(row, context);

        if (!row[p.c] && row[p.c] != "") {
            err('cannot find column {0} to assert !', p.c);
            return;
        }

        if (row[p.c].indexOf(p.v) < 0) {
            err('assertion fail web expect "{1}" for field "{0}" realy has "{2}".', p.c, p.v, row[p.c]);
        } else
            t.run('grid_assert-1666');
    }

    this.primary = function (p) {
        var $$ = this.win.get(0).contentWindow.$;
        var bc = $$('#b-' + ctx.b.config.id + ".bocrud-container");
        if (ctx.form.is('.ui-dialog-content')) {
            ctx.form.next().find('.btn-primary:first').click();
            t.run('primary');
        }
    }

    this.back = function (p) {
        var win = this.win.get(0).contentWindow;
        win.history.back()
    }

    this.success = function (p) {
        success = true;
        t.run('success');
    }

    this.refresh_grid = function (p) {
        scenario.splice(index + 1, 0, { t: 'wait', e: 'ongridcomplete' });

        ctx.b.do_refresh();

        t.run('refresh_grid');
    }

    this.delete = function (p) {
        var selarrrow = ctx.b.grid().jqGrid('getGridParam', 'selarrrow');
        var selrow = ctx.b.grid().jqGrid('getGridParam', 'selrow');
        var multiselect = ctx.b.grid().jqGrid('getGridParam', 'multiselect');
        var selected_rows = [];
        if (multiselect)
            selected_rows = selarrrow;
        else
            selected_rows = [selrow];

        if (ctx.b.config.mode != "GridDataEntry") {
            scenario.splice(index + 1, 0, { t: 'wait', e: 'ongridcomplete' });
            t.run('delete-1708');
        }
        log('delete rows with id {0}', selected_rows.join(','));

        ctx.b.do_del(selected_rows, ctx.b.grid(), function () {
            if (ctx.b.config.mode == "GridDataEntry")
                t.run('delete-1714');
        }, true);
    }

    this.edit = function (p) {
        if (ctx.$('#' + ctx.b.config.id + 'getDataEntryView').length == 0) {
            err('امکان ویرایش برای این فرم وجود ندارد !');
            return;
        }
        var selarrrow = ctx.b.grid().jqGrid('getGridParam', 'selarrrow');
        var selrow = ctx.b.grid().jqGrid('getGridParam', 'selrow');
        var multiselect = ctx.b.grid().jqGrid('getGridParam', 'multiselect');
        var selected_rows = [];
        if (multiselect)
            selected_rows = selarrrow;
        else
            selected_rows = [selrow];

        var puc = 0;

        ctx.b.addEvent('onedit', function (formObj) {
            var run = function () {
                if (puc <= 0) {
                    ctx.b.removeEvent('onpartialupdate', 'tester-addnew');
                    ctx.b.removeEvent('onpartialupdatecomplete', 'tester-addnew');
                    t.run('edit-1739');
                }
                else {
                    log('wait for init partial updates done');
                    setTimeout(run, 3000);
                }
            };

            if (ctx.b.config.mode == "GridDataEntry") {
                var grid = ctx.$('#' + ctx.b.config.id + '-Grid');
                ctx.form = grid.find('tr.bocrud-page-form:first');
            } else {
                var form = ctx.$('#' + formObj.id);
                ctx.form = form;
            }
            ctx.b.removeEvent('onedit', 'tester');
            run();
        }, 'tester');

        ctx.b.addEvent('onpartialupdate', function (dialogObj, pid, xml, ps, sups, ud, fake) {
            log('a partial update going to run');
            puc++;
        }, 'tester-addnew');

        ctx.b.addEvent('onpartialupdatecomplete', function (mode, dialogObj) {
            log('a partial update done');
            puc--;
        }, 'tester-addnew');

        ctx.b.do_edit(selected_rows, ctx.b.grid(), null, true);
    }

    this.view = function (p) {
        var selarrrow = ctx.b.grid().jqGrid('getGridParam', 'selarrrow');
        var selrow = ctx.b.grid().jqGrid('getGridParam', 'selrow');
        var multiselect = ctx.b.grid().jqGrid('getGridParam', 'multiselect');
        var selected_rows = [];
        if (multiselect)
            selected_rows = selarrrow;
        else
            selected_rows = [selrow];

        ctx.b.addEvent('onview', function (formObj) {
            if (ctx.b.config.mode == "GridDataEntry") {
                var grid = ctx.$('#' + ctx.b.config.id + '-Grid');
                ctx.form = grid.find('tr.bocrud-page-form:first');
            } else {
                var form = ctx.$('#' + formObj.id);
                ctx.form = form;
            }
            ctx.b.removeEvent('onview', 'tester');
            t.run('view-1790');
        }, 'tester');

        ctx.b.do_view(selected_rows, ctx.b.grid(), null, true);
    }
}