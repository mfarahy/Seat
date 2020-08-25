function filterMenu() {
    var input, filter, ul, li, a, i;
    input = document.getElementById('filtermenutext');
    filter = input.value.toUpperCase();
    ul = document.getElementsByClassName("nav-list")[0];
    li = ul.getElementsByTagName('li');
    for (i = 0; i < li.length ; i++) {
        a = li[i].getElementsByTagName('a')[0];
        if (li[i].innerHTML.toUpperCase().indexOf(filter) > -1) {
            li[i].style.display = "";
            continue;
        }
        if (a.innerHTML.toUpperCase().indexOf(filter) > -1) {

            li[i].style.display = "";
        } else {
            li[i].style.display = "none";
        }
    }
};


$(function () {
    var path = window.location.href;
    path = path.toLowerCase().split('#')[0];
    $("#mastreMenu a").each(function (e) {
        if ($(this).prop("href").toLowerCase() == path) {
            var li = $(this).closest("li");
            li.parents("li").addClass("active");
            li.parents("li").addClass("open");
            li.addClass("active");
        }

    });

    var breadcrumb = $('.breadcrumb');
    breadcrumb.html(' ');
    $('#mastreMenu').find('li.active:first').each(function () {
        var text = $(this).children('a').find('.menu-text').text();
        breadcrumb.append('<li class="bread-li">' + text + '</li>');
    });

    $("#mastreMenu>ul.nav>li:not(:first-child)").each(function () {
        if ($(this).find(".submenu").length == 0 || $(this).find(".submenu li").length == 0)
            $(this).hide();
    });


    var c = function () {
        if (window.performance) {
            var ct = [];
            ct.push('<div class="page-elapsed-time">');
            ct.push('<span>زمان محاسبه: ');
            var server_elapsedTime = parseInt($('#elapsed-time').val(), 0);
            ct.push((server_elapsedTime / 1000).toFixed(3));
            ct.push('s</span><br/>');
            ct.push('<span>تاخیر شبکه: ');
            var t = window.performance.timing;
            var elapsed = t.responseEnd - t.navigationStart;
            ct.push(elapsed - server_elapsedTime);
            ct.push('ms</span><br/>');
            ct.push('<span>تاخیر مرورگر: ');
            var t = window.performance.timing;
            elapsed = t.loadEventEnd - t.responseEnd;
            ct.push(elapsed);
            ct.push('ms</span></div>');
            $('.page-container').append($(ct.join('')));
        }
    };
    setTimeout(c, 1000);

    $.extend($.validator.messages, {
        CheckEvidence: 'تکمیل یکی از موارد این گروه ضروری می باشد.',
    });

    $.validator.methods['CheckEvidence'] = function (value, elem, param) {
        if (param.context && param.context != 'null') {
            var files = [];
            $(elem).closest('.bocrud-control[type=Repeater]')
                .find('.bocrud-control[name=Group]').each(function () {
                    if ($(this).find(':input').val() == param.context) {
                        var file = $(this).parent().children('[name=RequestImage]').find(':input[type=file]');
                        files.push(file);
                    }
                });
            for (var i = 0; i < files.length; ++i) {
                var file_count = files[i].closest('.bocrud-control').find('.bocrud-file-item').length;
                if (file_count > 0)
                    return true;
                file_count = files[i].MultiFile('files').length;
                if (file_count > 0)
                    return true;
            }
            return false;
        } else
            return true;
    };
});