

$(function () {
    var intervalHandler;
    var countable;

    var doUpdate = function (loading) {

        if (!countable && countable !== false) {
            countable = $('.bocrud-menu-item[countable=true]').length > 0;
        }

        if (countable === false) {
            if (intervalHandler)
                window.clearInterval(intervalHandler);
            return;
        }

        $.ajax({
            showLoading: loading,
            msg: 'به روز رسانی منوها',
            url: '/Bocrud/GetCountableMenuStates',
            dataType: "text",
            type: "get",
            error: function (data) {
                $.console.error("bocrud-341:server return error." + data.responseText);
            },
            success: function (response) {
                var data = eval('(' + response + ')');
                for (var id in data) {
                    console.log('#' + id + '.bocrud-menu-item .menu-text');
                    $('#' + id + '.bocrud-menu-item .menu-text').text(data[id]);
                }
            }
        });

    };

    if ($ && $.bocrud) {
        $.bocrud.globalEvents.push({
            type: 'ondeletesuccess', func: function (ids) {
                doUpdate(true);
            }
           , key: 'bocrud_update_menu'
        });
        $.bocrud.globalEvents.push({
            type: 'onsubmitsuccess', func: function (formObj) {
                doUpdate(true);
            }
          , key: 'bocrud_update_menu'
        });
    }

    intervalHandler = setInterval(function () { doUpdate(false); }, 15000);
});