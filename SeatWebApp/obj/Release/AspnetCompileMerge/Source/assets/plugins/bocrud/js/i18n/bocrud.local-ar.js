
; (function ($) {

    $.bocrud = {
        dir: 'rtl',
        captions: {
            bNext: 'البعدي',
            bPrev: 'القبلي',
            bFinish: 'الانتهاء',
            bSelect: "التحديد",
            bCancel: "الالغاء",
            bSave: "الخزن",
            compute_time: 'حساب الزمن:',
            network_latency: 'اخفاء الشبكة:',
            msg: 'الرسالة',
            day: 'اليوم',
            hour: 'الساعة',
            minut: 'الدقيقة',
            second: 'الثانية',
            month: 'الشهر',
        },
        select: {
            bSelect: "التحديد",
            show_all: "العرض الكلي",
            header: "العنوان الرئيسيب",
            checkAllText: "التحديد الكلي",
            uncheckAllText: "عدم تحديد الكل",
            noneSelectedText: "العنوان الرئيسيب",
            selectedText: "النص المختار"
        },
        tree: {
            bPaste: "اللصق"
        },
        msg: {
            currDel: 'التاكد من الحذف ؟',
            gridDel: 'حذف الشبكة ؟',
            wait: 'الانتظار... ',
            progress: 'العمليات الجارية',
            sending: 'الارسال',
            loading: 'التحميل',
            load_form: 'نوع التحميل',
            sending_for_validation: 'التحقق من صحة معلومات النموذج',
            updating_form: 'تحديث الاستمارة',
            create_child_item_for: 'تصنيع مستلزمات الاطفال',
            saving: 'الخزن',
            none_item_selected_for_edit: 'يرجى تحديد سطر لتحريره.',
            none_item_selected_for_delete: 'يرجى تحديد سطر لحذف تحريره.',
            deleting: 'الحذف',
            retrieve_all: 'الاسترداد الكل؟',
            redirecting: 'اعادة التوجيه',
            grid_loading: "تحميل الشبكة...",
            cannot_connect_to_server: '(عدم قدرة الاتصال بالخدمة)',
            select_before_new: 'التحديد قبل التحديث :',
            you_are_editing: 'أنت تقوم بتحرير العناصر التالية',
            select_your_items: 'يرجى تحديد العناصر التي تريدها!',
            file_selected: 'تحديد الملف!',
            loading_paste_form: 'تحميل الاستمارة الملصقة',
            partial_updating: 'تحديث جزئي',
            idle_timeout: 'لقد انتهى الوقت المسموح به لعدم استخدام النظام.',
            search_on: 'البحث عن',
            paste_tree: 'اللصق الشجري',
            post_window_data: 'إرسال معلومات النافذة',
            not_found: 'عدم الحضول!',
            load_window_data: 'تحميل البيانات النافذة',
            tree_copy: 'نسخة من الشجرة',
            tree_move: 'التحريك الشجري'
        },
        search: {
            EQ: "التقابل",
            NE: "عدم التقابل",
            LT: "الاصغر",
            LE: "الاصغر او التساوي",
            GT: "الاكبر",
            GE: "الاكبر او التساوي",
            IN: "في مجموعة",
            NI: "خارج المجموعة",
            BW: "الشروع بالـ",
            BN: "عدم الشروع بالـ",
            EW: "الانتهاء بالـ",
            EN: "عدم الانتهاء بالـ",
            CN: "يتضمن العبارة",
            NC: "عدم التعبير الشمولي",
            caption: 'اكتشاف',
            reset: 'عرض الكل',
            print: 'الطباعة'
        }
    };

    var makePersian = function () {
        if ($.fn.MultiFile && $.fn.MultiFile.options) {
            $.fn.MultiFile.options.STRING.denied = "االملف الملغية...";
            $.fn.MultiFile.options.STRING.selected = "الملف المحددة: $file";
            $.fn.MultiFile.options.STRING.duplicate = "تم بالفعل اختيار هذا الملف: \n$file";
            $.fn.MultiFile.options.STRING.toomuch = " زيادة في حجم الملف.";
            $.fn.MultiFile.options.STRING.toomany = "مسموح لك باختيار الحد الأقصى لملف $max.";
            $.fn.MultiFile.options.STRING.toobig = "الملف $file ملف ضخم جدا. الحد الأقصى للحجم المسموح به هو حجم $Size." ;
        } else {
            setTimeout(makePersian, 100);
        }
    }
    setTimeout(makePersian, 100);

})(jQuery);
