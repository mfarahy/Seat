
; (function ($) {

    $.notyr = $.extend($.notyr || {}, {
        dir: 'rtl',
        messages: {
            validation_error: "يحتوي النموذج الخاص بك على {0} حقل فارغ أو في مكان غير صحيح. يرجى استكمال المعلومات المطلوبة قبل حفظها.",
            submit_success: "تم تثبيت بنجاح.",
            oninlinesubmitsuccess: "النجاح عبر الإنترنت.",
            onvalidationfailed: 'عدم صحة معلومات النموذج :',
            onredirect: 'تغير الصفحة ...'

        }
    });
})(jQuery);
