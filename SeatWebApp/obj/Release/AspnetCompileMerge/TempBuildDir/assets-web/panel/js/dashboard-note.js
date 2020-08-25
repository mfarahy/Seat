
$(function () {
    var remove = function () {
        var t = $(this);

        $.ajax({
            url: '/DashboardNote/Remove?id=' + t.data('id'),
            type: 'get',
            cache: false,
            success: function (ok) {
                if (ok == 'OK')
                    t.hide('fade').remove();
            }
        });
    };

    $.ajax({
        url: '/DashboardNote/GetList',
        type: 'get',
        cache: false,
        success: function (notes) {
            var c = $('#dash-notes');
            for (var i = 0; i < notes.length; ++i) {
                var n = $('#dash-note-template').clone();

                c.append(n);
                n.css({ 'background-color': '#' + notes[i].Color }).show();
                n.find('.text').html(notes[i].Text);
                n.find('button').data('id', notes[i].Id).click(remove);
            }
        }
    });
});