; $(function () {

    $.bocrud.globalEvents.push({
        type: 'onload',
        key: 'help-info',
        func: function (cfg) {
            $.ajax({
                cache: true,
                type: 'GET',
                url: '/HelpInformation/GetFormHelpInfo',
                data: { formName: cfg.xml },
                success: function (result) {
                    var formHelpInfo = JSON.parse(result);
                    if (!window.bhelp)
                        window.bhelp = {};

                    if (!formHelpInfo) return;

                    window.bhelp[cfg.xml] = formHelpInfo;
                    var formHelpInfo = window.bhelp[cfg.xml];
                    for (var i = 0; i < formHelpInfo.length; i++) {
                        var text = "";
                        if (formHelpInfo[i].FieldName.startsWith(":") && !formHelpInfo[i].FieldName.startsWith("::")) {
                            var t = [];
                            t.push('<div class="help-info-page">');
                            t.push(formHelpInfo[i].Text);
                            t.push('</div>');
                            text = t.join('');

                            switch (formHelpInfo[i].FieldName) {
                                case ":TOP":
                                    if (!cfg.parent) {
                                        $(text).appendTo($('.page-header:first')).show('fade');
                                    }
                                    break;
                                case ":DOWN":
                                    if (!cfg.parent) {
                                        $(text).insertAfter($('#b-' + cfg.id)).show('fade');
                                    }
                                    break;
                                default:
                                    $('#' + cfg.id + formHelpInfo[i].FieldName.substring(1) + 'ubtn').popover({
                                        html: true,
                                        content: text,
                                        placement: 'auto',
                                        trigger: 'hover',
                                        rel: 'popover',
                                        title: 'اطلاعات راهنما'
                                    });
                                    break;
                            }
                        }
                    }
                }
            });
        }
    });

    $.bocrud.globalEvents.push({
        type: 'onshowcontentcomplete',
        key: 'help-info',
        func: function (form, mode, $form, xml) {

            var doit = function () {
                if (xml != undefined && window.bhelp) {
                    var formHelpInfo = window.bhelp[xml];

                    if (!formHelpInfo) return;

                    for (var i = 0; i < formHelpInfo.length; i++) {

                        var top_text = "";
                        if (formHelpInfo[i].FieldName.startsWith("::")) {
                            var t = [];
                            t.push('<div class="help-info-page">');
                            t.push(formHelpInfo[i].Text);
                            t.push('</div>');
                            top_text = t.join('');
                        }

                        switch (formHelpInfo[i].FieldName) {
                            case "::NEW":
                                if (mode == 'DataEntry' && form.boKey == "" || form.boKey == "0") {
                                    $form.prepend(top_text);
                                }
                                break;
                            case "::EDIT":
                                if (mode == 'DataEntry' && form.boKey && form.boKey.length > 0) {
                                    $form.prepend(top_text);
                                }
                                break;
                            case "::VIEW":
                                if (mode == 'View') {
                                    $form.prepend(top_text);
                                }
                                break;
                            default:

                                if (formHelpInfo[i].FieldName.startsWith('::GROUP')) {
                                    var group_name_or_title = formHelpInfo[i].FieldName.substring(7);
                                    var group = $form.find('fieldset[name="' + group_name_or_title + '"]');
                                    if (group.length == 0 || group.length > 1) {
                                        group = $form.find('legend').filter(function (legend) {
                                            return $(legend).text() == group_name_or_title;
                                        });
                                    }
                                    var t = [];
                                    t.push('<div class="alert alert-info help-info-group-desc">');
                                    t.push('<button type="button" class="close" data-dismiss="alert">\
												<i class="ace-icon icon-times"></i>\
											</button>');
                                    t.push(formHelpInfo[i].Text);
                                    t.push('</div>');
                                    var html = t.join('');
                                    $(html).insertAfter(group.children('legend'));

                                } else {

                                    var element = $form.find('.bocrud-control').filter("[name='" + formHelpInfo[i].FieldName + "']");

                                    // find static tooltip and merge content with new dynamic tooltip
                                    var staticTooltip = element.find('.help-button');
                                    var t = [];
                                    t.push('<div class="help-info-desc">');
                                    t.push(staticTooltip.data("content"));
                                    t.push("<br>");
                                    t.push(formHelpInfo[i].Text);
                                    t.push('</div>');
                                    var html = t.join('');
                                    $(staticTooltip).remove();
                                    element.append(html);

                                    var helpBtn = "<span class='help-button' >?</span>";
                                    element.find('.bocrud-control-caption:first').after(helpBtn);
                                    element.find('.help-button').popover({
                                        html: true,
                                        content: function () {
                                            return $(this).closest('.bocrud-control').find('.help-info-desc').html();
                                        },
                                        placement: 'auto',
                                        trigger: 'hover',
                                        rel: 'popover',
                                        title: 'اطلاعات راهنما'
                                    });
                                }
                                break;
                        }
                    }

                } else {
                    setTimeout(doit, 500);
                }
            }

            doit();
        }
    });

});