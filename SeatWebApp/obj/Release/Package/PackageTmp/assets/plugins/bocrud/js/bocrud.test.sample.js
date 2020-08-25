$(function () {
    var scenario = [
        { t: 'init', host: 'http://localhost:4608/' },
        { t: 'login', username: 'rostamkhani', password: '123456' },
        { t: 'load', xml: 'Package' },
        { t: 'new' },
        /*{ t: 'change', f: '*', v: '*' },
        { t: 'change', f: 'ToCityID,FromCityID', v: '*' },
        { t: 'tab', index: 1 },
        { t: 'change', f: '*', v: '*' },
        { t: 'tab', index: 2 },
        { t: 'change', f: '*', v: '*' },*/
        { t: 'tab', index: 3 },
        { t: 'change', f: '*', v: '*' },
    ];

    new tester(scenario).run();

});

