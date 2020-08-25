

function test_recorder(p) {
    var p = $.extend(p, {
        main: ''
    });

    var r = [];
    var b = window.bocruds[p.main];


    r.push({ t: 'init', host: window.location.origin + '/' });
    r.push({ t: 'login', username: '{username}', password: '{password}' });
    r.push({ t: 'open', xml: p.main });

    b.addEvent('onaddnew', function () {
        r.push({ t: 'new' });
    }, 'test_recorder');

    b.addEvent('onpresubmit', function () {
        r.push({ t: 'save' });
    }, 'test_recorder');

    b.addEvent('onselectrow', function (gridObj, sn) {
        r.push({ t: 'select_row', id: sn });
    }, 'test_recorder');

    b.addEvent('onusercmd', function (data) {
        var d = data['ctx'].split('&'), name = '';
        for (var i = 0; i < d.length; ++i)
            if (d[i].indexOf('name=')) {
                name = d[i].split('=')[1];
            }
        r.push({ t: 'cmd', name: name });
    }, 'test_recorder');

};
