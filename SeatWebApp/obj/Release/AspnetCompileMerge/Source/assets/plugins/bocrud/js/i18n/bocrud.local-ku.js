
; (function ($) {

    $.bocrud = {
        dir: 'rtl',
        captions: {
            bNext: 'دواتر',
            bPrev: 'ثيَشتر',
            bFinish: 'كوَتايي',
            bSelect: "دياركردن يا هةلَبذاردن",
            bCancel: "رةتكردنةوة",
            bSave: "هةلَطرتن",
            compute_time: 'هةذماركردنى كات:',
            network_latency: 'شاردنةوةى توَرِ:',
            msg:'ثةيام',
            day: 'رِوَذ',
            hour: 'كاذيَرِ',
            minut: 'دةقيقة',
            second: 'سانية',
            month: 'مانط',
        },
        select: {
            bSelect: "انتخاب",
            show_all: "نمايشى طشتى",
            header: "تايتلَى سةركى",
            checkAllText: "الانتخاب همه",
            uncheckAllText: "هةلَنةبذاردنى طشتى",
            noneSelectedText: "تايتلَى سةركى",
            selectedText: "دةقى دياركراو"
        },
        tree: {
            bPaste: "نوساندن يا ثةيوةستكردن"
        },
        msg: {
            currDel: 'دلَنياكردنةوة لة سرينةوة ؟',
            gridDel: 'سرِينةوةى توَرِ ؟',
            wait: 'ضاوةرِيَكردن... ',
            progress: 'ثرِوَسة ئةنجامدراوةكان',
            sending: 'ناردن',
            loading: 'باركردن',
            load_form: 'جوَرى باركردن',
            sending_for_validation: 'تحفيفكردن لة رِاستى زانيارييةكانى نمونة',
            updating_form: 'نويَكردنةوةى فوَرم',
            create_child_item_for: 'دامزراندنى بابةتيَكى فةرعى بوَ',
            saving: 'هةلَطرتن',
            none_item_selected_for_edit: 'ديَريَك دياربكة بوَ نوسينةوة.',
            none_item_selected_for_delete: 'ديَريَك نوسين دياركة بوَ لابردنى.',
            deleting: 'لابردن يا حزف',
            retrieve_all: 'اطةرِاندنةوةى طشتى ؟',
            redirecting: 'دوبارة ئاراستةكردنةوة',
            grid_loading: "باركردنى توَرِ...",
            cannot_connect_to_server: '(نةتوانينى ثةيوةندىكردن بة خزمةتطوزارى)',
            select_before_new: 'دياركردن ثيَش نويَكردنةوة:',
            you_are_editing: 'توَ هةلدةستيت بة نوسينةوةى ئةو بابةتانة',
            select_your_items: 'تكاية ئةو بابةتانةى دةتةويَن دياري بكة!',
            file_selected: 'دياريكردنى فايلى ويستراو!',
            loading_paste_form: 'باركردنى فوَرمى نوساو',
            partial_updating: 'نويَكردنةوةى بةشيَك',
            idle_timeout: 'كاتى بةكارنةهيَنانى سيستم كوَتايى هات.',
            search_on: 'طةران بة دواى',
            paste_tree: 'ثةيوةستكردنى درةختى',
            post_window_data: 'ناردنى زانيارييةكانى ثةنجةرة',
            not_found: 'بةردةست نةكةوتن!',
            load_window_data: 'باركردنى داتاكانى ثةنجةرة',
            tree_copy: 'طوَثييةك لة درةخت',
            tree_move:'طواستنةوةى درةختى'
        },
        search: {
            EQ: "بةرامبةر",
            NE: "نا بةرامبةر",
            LT: "بضوكتر",
            LE: "بضوكتر يا يةكسان",
            GT: "طةورةتر",
            GE: "طةورةتر يا يةكسان",
            IN: "لة ناوةوة",
            NI: "لة دةرةوة",
            BW: "دةستثيَكردن بة",
            BN: "دةستثيَنةكردن بة",
            EW: "كوَتايهيَنان بة",
            EN: "كوَتاينةهيَنان بة",
            CN: "ئةوةى لةو دةربرِينة هةية",
            NC: "ئةوةى لةو دةربرِينة نيية",
            caption: 'دوَزينةوة',
            reset: 'نمايشكردنى طشت',
            print: 'ضاثكردن'
        }
    };

    var makePersian = function () {
        if ($.fn.MultiFile && $.fn.MultiFile.options) {
            $.fn.MultiFile.options.STRING.denied = "فايلى وةلانراو...";
            $.fn.MultiFile.options.STRING.selected = "فايلى دياريكراو: $file";
            $.fn.MultiFile.options.STRING.duplicate = "ئةم فايلة دياريكرا: \n$file";
            $.fn.MultiFile.options.STRING.toomuch = "زيادة لة قةبارةى فايل.";
            $.fn.MultiFile.options.STRING.toomany = "ئةوةندة رِيَثيَدراوة لة فايلى  $max.";
            $.fn.MultiFile.options.STRING.toobig = "فايلى $file زوَر طةورةية ئةوةى رِيَثيَدراوة لة قةبارةى$size";
        } else {
            setTimeout(makePersian, 100);
        }
    }
    setTimeout(makePersian, 100);

})(jQuery);
