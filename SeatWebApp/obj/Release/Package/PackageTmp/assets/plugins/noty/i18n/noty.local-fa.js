
; (function ($) {

    $.notyr = $.extend($.notyr || {}, {
        dir: 'rtl',
        messages: {
            validation_error: "فرم شما دارای {0} فیلد خالی یا اشتباه است. لطفا قبل از ذخیره فرم اطلاعات مورد نیاز آن را تکمیل نمایید.",
            submit_success: "اطلاعات فرم {0} با موفقیت ثبت شد.",
            oninlinesubmitsuccess: "به روز رسانی با موفقیت انجام شد.",
            onvalidationfailed: 'اطلاعات فرم صحیح نیست:',
            onredirect: 'صفحه در حال انتقال است لطفا صبور باشید...',
            anchor_copy: 'لینک به فرم {0} کپی شد.',
            anchor_copy_failed:'امکان کپی لینک به فرم {0} وجود ندارد!'

        }
    });
})(jQuery);
