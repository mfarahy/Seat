
$.bocrud = $.extend($.bocrud || {},
    {
        closeModal: function (div) {
            if (div.is('.modal-stacker')) {
                div.hide('slow');
                $("html,body").animate({
                    scrollTop: 0
                }, 10);
            }
            return div;
        },
        openModal: function (div, options) {


            var header = [];
            header.push('<div class="widget-header">');
            header.push('<h5>');
            header.push(options.title);
            header.push('</h5>');
            header.push('<div class="widget-toolbar">');
            header.push('<a data-action="reload" href="#">');
            header.push('<i class="icon-refresh"></i>');

            header.push('</a>');
            header.push('<a data-action="collapse" href="#">');
            header.push('<i class="icon-chevron-up"></i>');
            header.push('</a>');
            header.push('<a data-action="close" href="#">');
            header.push('<i class="icon-remove"></i>');
            header.push('</a>');
            header.push('</div>');
            header.push('</div>');

            /*
            <div class="widget-box">

				<div class="widget-body">
					<div class="widget-main"></div>
				</div>
			</div>
            */

            var footer = [];
            footer.push('<div class="modal-stacker-footer">');
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

            var widget = div.wrap('<div class="widget-main"></div>').parent()
                .wrap('<div class="widget-body"></div>').parent()
                .wrap('<div class="widget-box"></div>').parent()
                .prepend(header.join(''))
                .append(footer.join(''))
            .wrap('<div class="modal-stacker dialog"></div>')
            .parent();

            var footerObj = widget.children('.widget-box')
                  .children('.modal-stacker-footer');
            footerObj.find('button').each(function () {
                var b = $(this);
                b.click(function () {
                    options.buttons[b.text()].apply(widget);
                });
            });

            $('.bocrud-page-image:first').after(widget);
            div.show('slow',function () {
                div.find('.ui-jqgrid').each(function () {
                    var gid = $(this).attr('id').substring(5);
                    var g = $('#' + gid);
                    var w = g.jqGrid('getGridParam', 'width');
                    if (w <= 100 && g.is(':visible'))
                        gridAutoWidth(gid, w);
                });
                var wh = $(window).height();
                var b=widget.children('.widget-box')
                    .children('.widget-body');
                var t = b.offset().top;
                b.css({
                        'max-height': wh - (t + footerObj.height()),
                        'overflow-y': 'auto',
                        'overflow-x':'hidden'
                });
            });

            $("html,body").animate({
                scrollTop: 0
            }, 10);

            
        }
    }
    );