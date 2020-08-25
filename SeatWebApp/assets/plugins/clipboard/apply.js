

; $(function () {
    $.bocrud.globalEvents.push({
        type: 'oneverycontrolplaced',
        key: 'clipboard',
        func: function (form) {
            var btn = $('#copy_btn');
            if (btn.length == 0) {
                btn = $('<button id="copy_btn" class="btn btn-xs"><i class="icon icon-copy ace-icon bigger-110 icon-only"></i></button>').appendTo('body').hide();
                new ClipboardJS('#copy_btn', {
                    text: function (trigger) {
                        var btn = $('#copy_btn');
                        return btn.data('active-elem').val();
                    }
                });
            }
            $('.bocrud-control[type="TextBox"]').hover(function () {

                var input = $(this).find(':input');

                if (!input.is('[disabled]')) return;

                var btn = $('#copy_btn');

                if (btn.data('active-elem') == input) return;

                //btn.position({
                //    my: 'right middle',
                //    at: 'left middle',
                //    of: input
                //});

                var w = btn.width();
                var o = input.offset();
                btn.css({
                    left: (o.left - w ) + 'px',
                    top: o.top + 'px',
                    position: 'absolute',
                    display: 'block'
                });

                //if (!btn.is(':visible'))
                //    btn.show();

                btn.data('active-elem', input);

                if (window.copy_btn_clear_time_out)
                    clearTimeout(window.copy_btn_clear_time_out);

                window.copy_btn_clear_time_out = setTimeout(function () {
                    var btn = $('#copy_btn');
                    if (!btn.is(':hover'))
                        btn.hide();
                }, 5000);

            }, function () {
                var btn = $('#copy_btn');
                if (!btn.is(':hover'))
                    btn.hide();
            });


        }
    });
});