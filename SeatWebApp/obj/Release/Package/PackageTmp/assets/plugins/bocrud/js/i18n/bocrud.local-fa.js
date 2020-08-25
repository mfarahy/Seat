
; (function ($) {

    $.bocrud = {
        dir: 'rtl',
        captions: {
            bNext: 'بعدی',
            bPrev: 'قبلی',
            bFinish: 'پایان',
            bSelect: "انتخاب",
            bCancel: "انصراف",
            bSave: "ذخیره",
            compute_time: 'زمان محاسبه:',
            network_latency: 'تاخیر شبکه:',
            msg:'پیام',
            day: 'روز',
            hour: 'ساعت',
            minut: 'دقیقه',
            second: 'ثانیه',
            month: 'ماه',
	    bRefresh: "بروزرسانی",
        },
        select: {
            bSelect: "انتخاب",
            show_all: "نمایش همه",
            header: "انتخاب کنید",
            checkAllText: "انتخاب همه",
            uncheckAllText: "غیر انتخاب همه",
            noneSelectedText: "انتخاب کنید",
            selectedText: "# آیتم از # آیتم انتخاب شده است"
        },
        tree: {
            bPaste: "پیوست"
        },
        msg: {
            update_toolbar: 'بروزرسانی عملیات‌های قابل انجام',
            partial_error: 'متاسفانه مشکلی در به روزرسانی بخش هایی از فرم پیش آمده است.',
            search_reseted:'فرم جستجو خالی شد، اکنون می توانید جهت جستجوی مجدد اقدام نمایید...',
            currDel: 'آیا از حذف آیتم کنونی "{0}" اطمینان دارید ؟',
            gridDel: 'آیا از حذف آیتم(‌های) انتخاب شده اطمینان دارید؟',
            wait: 'لطفا چند لحظه صبر نمائید... ',
            progress: 'عملیات در حال انجام',
            sending: 'ارسال',
            loading: 'بارگذاری',
            load_form: 'بارگذاری فرم اطلاعات',
            sending_for_validation: 'اعتبارسنجی اطلاعات فرم',
            updating_form: 'بروزرسانی اطلاعات فرم',
            create_child_item_for: 'ایجاد زیر گره برای',
            saving: 'در حال ذخیره فرم',
            none_item_selected_for_edit: 'لطفا جهت ویرایش یک سطر را انتخاب فرمایید.',
            none_item_selected_for_delete: 'لطفا جهت حذف حداقل یک سطر را انتخاب فرمایید.',
            deleting: 'در حال حذف آیتم',
            retrieve_all: 'آیا مایل هستید تمام داده‌ها بازیابی شوند ؟',
            redirecting: 'انتقال به صفحه دیگر',
            grid_loading: "محاسبه و دریافت اطلاعات ...",
            cannot_connect_to_server: '(ارتباط با سرور مقدور نیست)',
            select_before_new: 'قبل از ایجاد آیتم جدید {0} آن را مشخص کنید :',
            you_are_editing: 'شما درحال ویرایش آیتم‌های زیر هستید',
            select_your_items: 'لطفا آیتم‌های مورد نظر خود را انتخاب فرمائید!',
            file_selected: 'فایل مورد نظر قبلا انتخاب شده است !',
            loading_paste_form: 'بارگذاری فرم الصاق',
            partial_updating: 'به روزرسانی بخش هایی از فرم',
            idle_timeout: 'زمان مجاز عدم استفاده از سیستم به پایان رسیده است.',
            search_on: 'جست‌وجو در',
            paste_tree: 'پیوست درخت',
            post_window_data: 'ارسال اطلاعات پنجره',
            not_found: 'یافت نشد!',
            load_window_data: 'بارگذاری پنجره',
            tree_copy: 'رونوشت از درخت',
            tree_move:'انتقال درخت'
        },
        search: {
            EQ: "برابر",
            NE: "نا برابر",
            LT: "کوچکتر",
            LE: "کوچکتر یا مساوی",
            GT: "بزرگتر",
            GE: "بزرگتر یا مساوی",
            IN: "در مجموعه",
            NI: "در مجموعه نباشد",
            BW: "شروع با",
            BN: "شروع نشود با",
            EW: "انتها با",
            EN: "منتهی نشود با",
            CN: "شامل عبارت",
            NC: "شامل عبارت نباشد",
            caption: 'بیاب',
            reset: 'نمایش همه',
            print: 'پرینت'
        }
    };

    var makePersian = function () {
        if ($.fn.MultiFile && $.fn.MultiFile.options) {
            $.fn.MultiFile.options.STRING.denied = "فایل انتخابی شما صحیح نیست.\n لطفا مجدد تلاش کنید...";
            $.fn.MultiFile.options.STRING.selected = "فایل انتخاب شده: $file";
            $.fn.MultiFile.options.STRING.duplicate = "این فایل قبلا انتخاب شده است: \n$file";
            $.fn.MultiFile.options.STRING.toomuch = "فایل انتخاب شما از حد مجاز ($size) حجیم تر می باشد و قابل انتخاب نیست.";
            $.fn.MultiFile.options.STRING.toomany = "شما حداکثر مجاز به انتخاب $max فایل هستید.";
            $.fn.MultiFile.options.STRING.toobig = "فایل شمار $file بسیار حجیم هست. حداکثر حجم مجاز $size است.";
        } else {
            setTimeout(makePersian, 100);
        }
    }
    setTimeout(makePersian, 100);

})(jQuery);
