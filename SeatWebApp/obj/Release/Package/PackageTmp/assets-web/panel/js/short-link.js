

$(function () {

    $.bocrud.globalEvents.push({
        type: 'onregform',
        key: 'short-link',
        func: function (form, mode, $form, xml) {
            $form.find('a.bocrud-form-anchor').mouseover(function () {
                var t = $(this);
                if (t.data('short-link')) return;

                var formObj = t.closest('.bocrud-page-form').data('formObj');

                var i = t.find('i');
                i.removeClass('icon-anchor').addClass('icon-spinner icon-spin');

                var title = $(formObj.title).text();
                if (title.length > 50)
                    title = title.substring(0, 22) + " ... " + title.substring(title.length - 23, title.length);
                $.ajax({
                    data: {
                        link: formObj.anchor,
                        title: title
                    },
                    showLoading: false,
                    url: '/R/Register',
                    dataType: "text",
                    type: "POST",
                    error: function (data) {
                        $.console.error("bocrud-341:server return error." + data.responseText);
                    },
                    success: function (shortLink) {
                        t.data('short-link', shortLink);
                        formObj.org_anchor = formObj.anchor;
                        formObj.anchor = shortLink;
                    },
                    complete: function () {
                        i.removeClass('icon-spinner icon-spin').addClass('icon-anchor green');
                    }
                });
            });
        }
    });
});