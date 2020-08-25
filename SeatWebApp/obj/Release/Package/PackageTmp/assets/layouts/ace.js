// version 0.93.10.02

(function ($) {

    $.ace_layout = $.ace_layout || {};
    $.extend($.ace_layout, {
        onshowcontent: function (formObj, buttons, mode, place_holder_selector) {

            if (mode == "GridDataEntry" || mode == "GridView") return;

            var posinfo = formObj.detailPos.split('-');
            var showstyle = posinfo[0].toLowerCase(), showstyle_section2 = posinfo[1];

            if ((showstyle != 'tabbable' && showstyle != 'accordion' && showstyle != 'wizard')||
                (showstyle == 'wizard' && formObj.parent_id))
                return $.classic_layout.onshowcontent(formObj, buttons, mode, place_holder_selector);

            var bc = $('#b-' + formObj.jsbid);
            var ph = $(place_holder_selector);
            var fc = ph.length == 1 ? ph : bc.children('.bocrud-form-container');
            var ignore_toolbar = ph.length == 1;
            var ct = [];

            ct.push();

            if (!formObj.ignore_form_tag) {
                ct.push('<form class="bocrud-page-form" id="');

                ct.push(formObj.id);
                ct.push('" namespace="');
                ct.push(formObj.namespace);
                ct.push('">');
            }

            if (formObj.valSum)
                ct.push('<div class="bocrud-validation-summary"></div>')
            ct.push('<div class="bocrud-current-item"></div>');

            switch (showstyle) {
                case "accordion":
                    ct.push($.ace_layout._accordion(formObj));
                    break;
                case "tabbable":
                    ct.push($.ace_layout._tabbable(formObj, showstyle_section2));
                    break;
                case "wizard":
                    ct.push($.ace_layout._wizard(formObj));
                    break;
            }

            if (!ignore_toolbar)
                ct.push('<div class="bocrud-classic bocrud-toolbar-form-op"></div>');

            if (!formObj.ignore_form_tag) {
                ct.push('</form>');
            }

            // inject ct to page
            if (fc.length == 0) {
                ct.splice(0, 0, '<div class="bocrud-form-container ui-widget-content ui-corner-all bocrud-style-' + formObj.detailPos + ' ">');
                ct.push('</div>');

                bc.prepend(ct.join(''));

                fc = bc.children('.bocrud-form-container:first');
            }
            else {
                fc.html(' ').append(ct.join(''));// firefox and chrom bug issue
            }

            if (showstyle == 'wizard') {
                $.ace_layout._active_wizard(formObj, buttons);
            }

            var b = $('#b-' + formObj.jsbid);
            var tb = fc.find('.bocrud-toolbar-form-op:first');
            tb.html('');
            if (mode == 'DataEntry' && !$.isEmptyObject(buttons)) {
                if (showstyle != 'wizard') {
                    $.bocrud.addButton(tb, {
                        caption: $.jgrid.edit.bSubmit,
                        buttonicon: 'icon-save bigger-160',
                        onClickButton: function () {
                            tb.find('li').addClass('ui-state-disabled');
                            buttons[$.jgrid.edit.bSubmit](true);
                        },
                        showText: true,
                        classes: 'bocrud-tb-submit btn btn-app btn-success btn-xs radius-4'
                    });
                }
                if (!formObj.hideGrid || formObj.parent_id)
                    $.bocrud.addButton(tb, {
                        caption: $.jgrid.edit.bCancel,
                        buttonicon: 'icon-ban bigger-160',
                        onClickButton: function () {
                            tb.find('li').addClass('ui-state-disabled');
                            buttons[$.jgrid.edit.bCancel](false);
                            bc.find('#' + formObj.id).hide('slow', function () {
                                $(this).remove();
                            });
                            //$("html,body").animate({
                            //    scrollTop: 0
                            //});

                        },
                        showText: true,
                        classes: 'bocrud-tb-cancel btn btn-app btn-warning btn-xs radius-4'
                    });
            }

            fc.show('slow');

            fc.find('[data-rel=popover]').popover({ container: 'body' });

            switch (showstyle) {
                case 'accordion':
                    fc.find('.panel-collapse').on('shown.bs.collapse', function () {
                        $.bocrud.alignGrid($(this));
                    })
                    break;
                case 'tabbable':
                    fc.find('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                        $.bocrud.alignGrid($($(e.target).attr('href')));
                    })
                    break;
                case 'wizard':
                    break;
            }
        },
        _accordion: function (formObj) {
            var ct = [];

            ct.push('<div id="');
            ct.push('acc-' + formObj.id); // form id
            ct.push('" class="accordion-style1 panel-group">');
            var t = [];
            for (var i = 0; i < formObj.groups.length; ++i) {
                var g = formObj.groups[i];
                var gid = formObj.jsbid + '-' + g.id;

                t.push('<div class="panel panel-default">');
                t.push('<div class="panel-heading">');
                t.push('<h4 class="panel-title">');
                t.push('<a class="accordion-toggle" data-toggle="collapse" data-parent="#');
                t.push('acc-' + formObj.id);
                t.push('" href="#');
                t.push(gid);
                t.push('" ');
                t.push('name="');
                t.push(g.name);
                t.push('" ');
                t.push('index="');
                t.push(i);
                t.push('" ');
                t.push('>');
                t.push('<i class="icon-angle-');
                if (i == 0)
                    t.push('down');
                else
                    t.push('left');
                t.push(' bigger-110" data-icon-hide="icon-angle-down" data-icon-show="icon-angle-left"></i>');
                t.push('&nbsp;' + g.title);
                t.push('</a>');
                t.push('</h4>');
                t.push('</div>');

                t.push('<div class="panel-collapse collapse ');
                if (i == 0)
                    t.push('in');
                t.push('" id="');
                t.push(gid);
                t.push('">');
                t.push('<div class="panel-body">');
                t.push(g.content);
                t.push('</div>');
                t.push('</div>');
                t.push('</div>');

                ct.push(t.join(''));
                t.splice(0, t.length);
            } // for

            formObj.hideGroup = function (group_name, form) {
                var a = form.find('a[name="' + group_name + '"]:first');
                var panel = a.closest('.panel');
                if (panel.children('.panel-collapse').is('.in')) {
                    var i = parseInt(a.attr('index'));
                    var panel_group = a.closest('.panel-group');
                    var total_tabs = panel_group.children().length;
                    $(panel_group.children()[(i + 1) % total_tabs]).find('a.accordion-toggle:first').click();
                }
                panel.hide('slow');
            }
            formObj.showGroup = function (group_name, form) {
                var a = form.find('a[name="' + group_name + '"]:first');
                a.closest('.panel').show('slow');
            }

            formObj.disableGroup = function (group_name, form) {
                formObj.hideGroup(group_name, form);
            }
            formObj.enableGroup = function (group_name, form) {
                formObj.showGroup(group_name, form);
            }

            return ct.join('');
        },
        _tabbable: function (formObj, showstyle_section2) {

            var ct = [];
            ct.push('<div class="bocrud-form-tabs tabbable ');
            ct.push('tabs-' + showstyle_section2);
            ct.push('">');
            ct.push('<ul class="nav nav-tabs">');

            var t = [];
            for (var i = 0; i < formObj.groups.length; ++i) {
                var g = formObj.groups[i];
                var gid = formObj.jsbid + '-' + g.id;

                t.push('<li');
                if (i == 0)
                    t.push(' class="active"');
                t.push('>');
                t.push('<a class="tabbable-toggle" href="#');
                t.push(gid);
                t.push('" ');
                t.push('name="');
                t.push(g.name);
                t.push('" ');
                t.push('index="');
                t.push(i);
                t.push('" ');
                t.push(' data-toggle="tab">');
                t.push(g.title);
                t.push('</a>');
                t.push('</li>');
                ct.push(t.join(''));
                t.splice(0, t.length);
            } // for
            ct.push('</ul><div class="tab-content">');

            for (var i = 0; i < formObj.groups.length; ++i) {
                var g = formObj.groups[i];
                var gid = formObj.jsbid + '-' + g.id;

                t.push('<div id="');
                t.push(gid);
                t.push('" name="');
                t.push(g.Name);
                t.push('" index="');
                t.push(i);
                t.push('" class="tab-pane ');
                if (i == 0)
                    t.push('active');
                t.push('">');
                t.push(g.content);
                t.push('</div>');

                ct.push(t.join(''));

                t.splice(0, t.length);

            } // for

            ct.push('</div></div>');

            formObj.hideGroup = function (group_name, form) {
                var a = form.find('a[name="' + group_name + '"]:first');
                var gid = a.attr('href');
                a.closest('li').hide('slow');
                if (form.find(gid).is(':visible')) {
                    var i = parseInt(a.attr('index'));
                    var ul = a.closest('ul');
                    var total_tabs = ul.children().length;
                    $(ul.children()[(i + 1) % total_tabs]).children('a').click();
                }
            }
            formObj.showGroup = function (group_name, form) {
                var a = form.find('a[name="' + group_name + '"]:first');
                a.closest('li').show('slow');
            }

            formObj.disableGroup = function (group_name, form) {
                formObj.hideGroup(group_name, form);
            }
            formObj.enableGroup = function (group_name, form) {
                formObj.showGroup(group_name, form);
            }

            return ct.join('');
        },
        _wizard: function (formObj) {

            if (formObj.validationObj) {
                formObj.validationObj.ignore = function (i, elem) {
                    if (!$(elem).closest('.step-pane').is('.active'))
                        return true;
                    return false;
                }
            }

            var ct = [];

            ct.push('<div id="');
            ct.push('wiz-' + formObj.id); // form id
            ct.push('" class="fuelux-wizard-container bocrud-wizard-container wizard">');
            {
                ct.push('<div class="steps-container">');
                {
                    ct.push('<ul class="steps">'); {
                        for (var i = 0; i < formObj.groups.length; ++i) {
                            var g = formObj.groups[i];
                            var gid = formObj.jsbid + '-' + g.id;
                            ct.push('<li data-step="');
                            ct.push(i + 1);
                            ct.push('" class="');
                            if (i == 0)
                                ct.push('active');
                            ct.push('">'); {
                                ct.push('<span class="step">'); {
                                    ct.push(i + 1);
                                } ct.push('</span>');
                                ct.push('<span class="title">'); {
                                    ct.push(g.title);
                                } ct.push('</span>');
                            } ct.push('</li>');
                        }
                    } ct.push('</ul>');
                } ct.push('</div>');

                ct.push('<hr/>');

                ct.push('<div class="step-content pos-rel">');
                {
                    for (var i = 0; i < formObj.groups.length; ++i) {
                        var g = formObj.groups[i];
                        var gid = formObj.jsbid + '-' + g.id;
                        ct.push('<div class="step-pane ');
                        if (i == 0)
                            ct.push('active');
                        ct.push('" data-step="');
                        ct.push(i + 1);
                        ct.push('" ');
                        ct.push('id="');
                        ct.push(gid);
                        ct.push('" ');
                        if (g.name) {
                            ct.push('name="');
                            ct.push(g.name);
                            ct.push('" ');
                        }
                        ct.push('>'); {
                            ct.push('<h3 class="lighter block green">');
                            ct.push(g.title);
                            ct.push('</h3>');
                            ct.push(g.content);
                        } ct.push('</div>');
                    }
                } ct.push('</div>');


                ct.push('<hr/>');
                ct.push('<div class="wizard-actions actions">'); {
                    ct.push('<button type="button" class="btn btn-prev">'); {
                        ct.push('<i class="ace-icon fa fa-arrow-left"></i>');
                        ct.push($.bocrud.captions.bPrev);
                    } ct.push('</button>');
                    ct.push('<button type="button" class="btn btn-success btn-next" data-last="');
                    ct.push($.bocrud.captions.bFinish);
                    ct.push('">'); {
                        ct.push($.bocrud.captions.bNext);
                        ct.push('<i class="ace-icon fa fa-arrow-right icon-on-right"></i>');
                    } ct.push('</button>');
                } ct.push('</div>');
            } ct.push('</div>');

            formObj.hideGroup = function (group_name, form) {

            }
            formObj.showGroup = function (group_name, form) {
               
            }

            formObj.disableGroup = function (group_name, form) {
            }
            formObj.enableGroup = function (group_name, form) {
            }

            return ct.join('');
        },
        _syncSteps: function (formObj) {
            var wiz = $('#wiz-' + formObj.id);
            if (wiz.length > 0) {
                var steps = wiz.find('.steps:first');
                var i = 1;
                steps.children('li').each(function () { 
                    $(this).find('.step').text(i++);
                });
            }
        },
        onactionshowgroup: function (group_name, form, actions) {
            var formObj = form.data('formObj');
            var group = $('[form-id="#wiz-' + formObj.id + '"][name=' + group_name + '].bocrud-hide');
            if (group.length > 0) {
                var wiz = $('#wiz-' + formObj.id);
                var steps = wiz.find('.steps:first');
                var stepContent = wiz.find('.step-content:first');
                var index = group.data('step');

                var ct = [];
                ct.push('<li data-step="'); {
                    ct.push(index);
                    ct.push('">');
                    ct.push('<span class="step">'); {
                        ct.push(index);
                    } ct.push('</span>');
                    ct.push('<span class="title">'); {
                        ct.push(group.children('h3').text());
                    } ct.push('</span>');
                } ct.push('</li>');
                var step= $(ct.join(''));
                group.removeClass('bocrud-hide');
                if (wiz.data('fu.wizard').numSteps < index) {
                    step.appendTo(steps);
                    group.appendTo(stepContent);
                } else {
                    var cstep = steps.find('li:nth-child(' + index + ')');
                    step.insertBefore(cstep);
                    var cgroup = stepContent.find('.step-pane:nth-child(' + index + ')');
                    group.insertBefore(cgroup);
                }
                group.find('.bocrud-group[name=' + group_name + ']:first').removeClass('bocrud-hidden-action').show();
                wiz.wizard('syncSteps').wizard('setState');
                wiz.data('fu.wizard').numSteps++;
                $.ace_layout._syncSteps(formObj);
            }
        },
        onactionhidegroup: function (name, form, actions) {
            var formObj = form.data('formObj');
            var group = $('#wiz-' + formObj.id).children('.step-content').children('[name=' + name + ']');
            if (group.length > 0) {
                var index = group.data('step');
                group.attr('form-id', '#wiz-' + formObj.id).removeClass('active').addClass('bocrud-hide').appendTo(form);
                $('#wiz-' + formObj.id)
                    .wizard('removeSteps', index, 1)
                    .wizard('syncSteps')
                    .wizard('setState');
                $.ace_layout._syncSteps(formObj);
            }
        },
        _active_wizard: function (formObj, buttons) {
            var wiz = $('#wiz-' + formObj.id);

            wiz.wizard({
                //step: 2 //optional argument. wizard will jump to step "2" at first
                buttons: '#wiz-' + formObj.id + ' .wizard-actions:eq(0)'
            })
              .on('actionclicked.fu.wizard', function (e, info) {

                  if (formObj.mode == 'DataEntry' && info.direction == 'next' && !$('#wiz-' + formObj.id).data('wizard-ignore-validation')
                      && !$('#' + formObj.id).data('ignore-validation')) {

                      var bocrud = window.bocruds[formObj.jsbid];
                      var form = $('#' + formObj.id);
                      var validator = form.data("validator");

                      $('#wiz-' + formObj.id).children('.wizard-actions').find('button').attr('disabled', 'disabled');

                      bocrud.validate(form, function (b, r) {
                          $('#wiz-' + formObj.id).children('.wizard-actions').find('button').removeAttr('disabled');

                          if (r === true) {

                              $('#wiz-' + formObj.id).data('wizard-ignore-validation', true);

                              $('#wiz-' + formObj.id).wizard('next');

                          } else {
                              return;
                          }
                      });

                      e.preventDefault();
                      return;
                  }

                  $('#wiz-' + formObj.id).removeData('wizard-ignore-validation');
              })
              .on('changed.fu.wizard', function (e, info) {
                  var form = $('#' + formObj.id);
                  if (formObj.mode == 'DataEntry' && !form.data('ignore-validation')) {
                      form.validate({ cancelSubmit: true });
                      form.find('.bocrud-control').each(function () {
                          $(this).removeClass('has-error').removeClass('has-success');
                          $(this).find('label.error').remove();
                      });
                  }
                  var step = $('#wiz-' + formObj.id).children('.step-content').find('[data-step=' + info.step + ']');
                  $.bocrud.alignGrid(step);

                  if (formObj.mode == 'View') {
                      var stepCounts = wiz.find('.step-pane').length;
                      if (info.step == stepCounts) {
                          wiz.find('.wizard-actions .btn-success').attr('disabled', 'disabled');
                      }
                  }
              })
              .on('finished.fu.wizard', function (e) {
                  if (formObj.mode == 'DataEntry')
                      buttons[$.jgrid.edit.bSubmit](true);

              }).on('stepclick.fu.wizard', function (e) {
                  //e.preventDefault();//this will prevent clicking and selecting steps
              });

            var b = window.bocruds[formObj.jsbid];
            if (!b.config.parent) {
                var qs = querystring.parse();
                var step = qs["step"];
                if (step && step.length > 0) {
                    var pane = wiz.find('.step-pane[name=' + step + ']');
                    var stepn = pane.data('step');
                    for (var i = 1; i < stepn ; ++i)
                        wiz.wizard('next');
                }
            }
        },
        onvalidationstart: function (form) {
            var formObj = form.data('formObj');
            $.ace_layout.validation_spinner_handler = setTimeout(function (target) {
                $.ace_layout.validation_spinner_handler = null;
                if (target && target.is('button') && target.parent().is('.wizard-actions') && target.find('bocrud-btn-loading').length == 0) {
                    target.append("<i class='bocrud-btn-loading'></i>");
                }
            }, 1000, $(event.currentTarget));
        },
        onvalidationcomplete: function (form, result) {
            var formObj = form.data('formObj');
            $('#wiz-' + formObj.id + ' .wizard-actions button .bocrud-btn-loading').remove();
            if ($.ace_layout.validation_spinner_handler)
                clearTimeout($.ace_layout.validation_spinner_handler);
        }
    });


})(jQuery);