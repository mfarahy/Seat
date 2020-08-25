
(function ($) {


    var notify = function (title, message, icon, link) {
        if (Notification.permission !== "granted")
            Notification.requestPermission();
        else {
            var notification = new Notification(title, {
                icon: window.location.protocol + "//" + window.location.host + '/assets/img/notification/' + icon + '.png',
                body: $('<div/>').html(message).text(),
            });

            notification.onclick = function () {
                if (link && !link.startsWith("http")) {
                    if (!link.startsWith("/")) link = "/" + link;
                    link = window.location.protocol + "//" + window.location.hostname + link;
                }
                window.open(link);
            };

            if ($.noty) {
                switch (icon.toLowerCase()) {
                    case 'alert':
                        $.noty.warn(message);
                        break;
                    case 'error':
                        $.noty.error(message);
                        break;
                    case 'clock':
                    case 'email':
                    case 'info':
                    case 'success':
                        $.noty.success(message);
                        break;
                }
            }
        }
    }

    var tryCount = 10;
    var signalr = function () {
        if (tryCount < 0) return;
        tryCount--;

        if (!$.connection) {
            if ($.console)
                $.console.warn("signalr connection not found, wait for 500 ms and check it again.");
            setTimeout(signalr, 500);
            return;
        }

        var notification = $.connection.notification;
        if (!notification) {
            if ($.console)
                $.console.warn("signalR notification not found, wait for 500 ms and check it again.");
            setTimeout(signalr, 500);
            return;
        }

        notification.client.notify = notify;


        $.connection.logger.connection.start().done();

        if ($.console)
            $.console.info("signalR notification established successfuly.");
    }
    signalr();

})(jQuery);

