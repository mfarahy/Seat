
var widget_count = 0;
if (!window.widgets)
    window.widgets = {};

$(function () {

    var options = {
        resizable: true,
        draggable: true,
        float: true,
        always_show_resize_handle: /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent),
        animate: false,
        vertical_margin: 5,
        rtl: true,
        removable: true
    };
    var gridElement = $('.dashboard').gridstack(options);

    window.dashboard = new function () {

        this.grid = $('.dashboard').data('gridstack');
        var skip_change = false;
        $('.dashboard').on('added', function (e, nodes) {
            for (var i = 0; nodes && i < nodes.length; ++i) {
                var box = $(nodes[i].el);
                var c = box.find('.widget-main');
                var w = Math.round(c.width() / gridElement.width() * 12), h = Math.round(c.height() / gridElement.width() * 12);
                skip_change = true;
                window.dashboard.grid.resize(nodes[i].el, nodes[i].width, nodes[i].height);
                skip_change = false;
                var boxid = box.attr('id');
                var wiid = boxid.split('-')[0];
                setTimeout(function () {
                    window.dashboard.refresh_size(wiid);
                    box.css({ opacity: 100 });
                }, 500);
            }
        });

        $('.dashboard').on('change', function (e, nodes) {

            if (skip_change) return;

            for (var i = 0; nodes && i < nodes.length; ++i) {
                var box = $(nodes[i].el);
                var boxid = box.attr('id');
                var wiid = boxid.split('-')[0];
                window.dashboard.refresh_size(wiid);
            }
        });

        $('.dashboard').on('dragstop', function (event, ui) {
            var id = ui.helper.data('id');
            if (window.widgets[id])
                window.widgets[id].onmoved(ui.position);
            dashboard.save_desktop();
        });

        $('.dashboard').on('resizestop', function (event, ui) {
            var id = ui.helper.data('id');
            if (window.widgets[id])
                window.widgets[id].onresized({ width: ui.size.width, height: ui.size.height });

            dashboard.save_desktop();
        });
        this.refresh_handlers = [];
        this.add_new_widget = function (widget_iid, x, y, w, h, dont_save, lock) {

            $.ajax({
                url: '/Dashboard/GetWidget',
                data: {
                    id: widget_iid,
                    width: 0,
                    height: 0,
                },
                success: function (widget_model) {
                    var widget = $('.widget-template').html();

                    widget = widget
                        .replace('{caption}', widget_model.name)
                        .replace('{content}', widget_model.content);

                    var $w = $(widget).css({ opacity: 0 });
                    $w.attr('id', widget_model.uid + '-box');
                    $w.data('id', widget_model.uid);
                    $w.data('wiid', widget_iid);

                    $w.find('.action-refresh').click(function () {
                        var w = $(this).closest('.widget');
                        dashboard.refresh(w.data('id'), w.data('wiid'));
                    });

                    $w.find('.action-remove').click(function () {
                        var w = $(this).closest('.widget');
                        dashboard.remove(w, w.data('wiid'));
                    });

                    if (!w || !h) {
                        w = Math.round(widget_model.w / gridElement.width() * 12);
                        h = Math.round(widget_model.h / gridElement.width() * 12);
                    }

                    window.dashboard.grid.add_widget($w, x || 0, y || 0, w || 3, h || 5, !(x || y));
                    if (lock)
                        window.dashboard.grid.movable($w, false);
                    if (!dont_save)
                        dashboard.save_desktop();

                    if (widget_model.refreshRate > 0) {

                        var handler = setInterval(function () {
                            dashboard.refresh(widget_model.uid, widget_model.id, true);
                        }, widget_model.refreshRate);

                        dashboard.refresh_handlers.push({ id: widget_model.uid, handler: handler });
                    }

                    $('#add-widget-btn').show('fade');
                    //$('#menu-' + widget_iid).attr('disable');
                    //if ($('.widget-menu-item:visible').length == 0) {
                    //    $('#add-widget-btn').hide();
                    //} else


                    $w.find('.widget-header').hide();
                    if (!lock) {
                        $w.hover(function () {
                            $(this).find('.widget-header').show('fade');
                        }, function () {
                            $(this).find('.widget-header').hide();
                        });
                    }
                }
            });

        };

        this.refresh = function (id, wiid, dont_block) {
            var remove = null;
            if (!dont_block) {
                var b = $('#' + id);
                var q = false;
                if (b.css("position") == "static") {
                    q = true;
                    b.addClass("position-relative")
                }
                b.append('<div class="widget-box-overlay"><i class="icon-spinner icon-spin icon-2x white"></i></div>');
                remove = function () {
                    b.find(".widget-box-overlay").remove();
                    if (q) {
                        b.removeClass("position-relative")
                    }
                };
            }

            $.ajax({
                url: '/Dashboard/Refresh',
                data: {
                    instance_id: wiid
                },
                type: 'get',
                success: function (refresh_data) {
                    if (window.widgets[id])
                        window.widgets[id].refresh(refresh_data);
                    if (remove)
                        remove();
                }
            });

            console.log('refresh');
        }

        this.remove = function (widget, wiid) {
            $('#add-widget-btn').show();
            $('#menu-' + wiid).show();
            window.dashboard.grid.remove_widget(widget);
            dashboard.save_desktop();
        }



        this.refresh_size = function (wuid) {
            var resize = function (id) {
                var box = $('#' + id + '-box .widget-box');
                var w = box.width();
                var h = box.height();
                if (window.widgets[id])
                    window.widgets[id].onresized({ width: w, height: h });

            };
            if (wuid)
                resize(wuid);
            else
                for (var id in window.widgets) resize(id);
        };

        this.notify = function (text) {
            return noty({
                text: text,
                type: 'success',
                dismissQueue: true,
                animation: {
                    open: { height: 'toggle' }, // jQuery animate function property object
                    close: { height: 'toggle' }, // jQuery animate function property object
                    easing: 'swing', // easing
                    speed: 500 // opening & closing animation speed
                }
            });
        };

        this.load_desktop = function () {
            $.ajax({
                url: '/Dashboard/LoadDesktop',
                type: 'get',
                success: function (widgets) {
                    for (var i = 0; i < widgets.length; ++i) {
                        var d = widgets[i];
                        dashboard.add_new_widget(d.InstanceId, d.X, d.Y, d.Width, d.Height, true, widgets[i].Lock);
                    }
                }
            });
        };

        var save_in_progress = false;
        this.save_desktop = function () {
            if (save_in_progress) return;

            save_in_progress = true;

            var ws = [];
            $('.dashboard .widget:visible').each(function () {
                var node = $(this).data('_gridstack_node');
                ws.push({
                    InstanceId: $(this).data('wiid'),
                    X: node.x,
                    Y: node.y,
                    Width: node.width,
                    Height: node.height
                });
            });
            $.ajax({
                url: '/Dashboard/SaveDesktop',
                data: {
                    desktop: JSON.serialize({ widgets: ws })
                },
                type: 'post',
                error: function () {
                    save_in_progress = false;
                },
                success: function () {
                    console.log('desktop saved successfuly.');
                    save_in_progress = false;
                }
            });
        };

        this.load_desktop();

        $(window).resize(function () {
            dashboard.refresh_size();
        });
    };
});

function new_c3_chart(id, data, width, height, fix_size) {

    window.widgets[id] = {
        id: id,
        flow_buffer: [],
        chart: null,
        fix_size: fix_size,
        onload: function () {

            this.chart = c3.generate($.extend({
                size: {
                    width: parseInt(width.toString()),
                    height: parseInt(height.toString())
                },
                legend: {
                    position: 'bottom'
                }
            }, data));

        },
        onresized: function (newsize) {
            if (!this.fix_size)
                this.chart.resize({ width: newsize.width - 20, height: newsize.height - 20 });
        },
        refresh: function (o) {
            if (o.data.type != 'line') {
                this.chart.load({
                    columns: o.data.columns,
                    colors: o.data.colors,
                    unload: o.data.type != 'gauge'
                });
            }
            else {
                var t = this;

                t.flow_buffer.push(o.data.columns[0][1]);

                this.chart.flow({
                    columns: o.data.columns,
                    to: t.flow_buffer[0]
                });

                if (t.flow_buffer.length > 10) {
                    t.flow_buffer.splice(0, 1);
                }
            }
        },
        ondeleted: function () { },
        onmoved: function (newposition) { }
    };

}

function _clock(id, size) {

    window.widgets[id] = {
        id: id,
        clock: null,
        onload: function () {

            clock(id, size);

            $('#' + id).width(size - 20).height(size - 20);
        },
        onresized: function (newsize) {
            var size = Math.min(newsize.width, newsize.height);
            $('#' + id).width(size - 20).height(size - 20);
        },
        refresh: function (o) {

        },
        ondeleted: function () { },
        onmoved: function (newposition) { }
    };


}

function defaultWidget(id, width, height) {
    $('#' + id).click(function () {
        $.noty.success('صفحه در حال انتقال می باشد...');
        window.location = $(this).attr('href');
    });
    window.widgets[id] = {
        id: id,
        onload: function () {
        },
        onresized: function (newsize) {
            $('#' + this.id).width(newsize.width - 20).height(newsize.height - 20);
        },
        refresh: function (o) {
        },
        ondeleted: function () { },
        onmoved: function (newposition) {
            console.log(newposition);
        }
    };


}

function CircleLink(id, width, height) {

    window.widgets[id] = {
        id: id,
        onload: function () {
        },
        onresized: function (newsize) {
            $('#' + this.id).width(newsize.width - 20).height(newsize.height - 20);
        },
        refresh: function (o) {
        },
        ondeleted: function () { },
        onmoved: function (newposition) {
            console.log(newposition);
        }
    };


}