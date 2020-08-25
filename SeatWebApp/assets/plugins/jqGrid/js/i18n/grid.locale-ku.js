;(function($){
/**
 * jqGrid Arabic Translation
 * 
 * http://trirand.com/blog/ 
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = $.jgrid || {};
$.extend($.jgrid,{
	defaults : {
	    recordtext: "نمایش {0} - {1} لة {2}",
	    emptyrecords: "توَمارنةكردنى دةق",
	    loadtext: "باركردنى دةق...",
	    pgtext : "لاثةرِةى {0} لة {1}"
	},
	search : {
	    caption: "ثشكنين...",
	    Find: "دةستكةوتن",
	    Reset: "رِيَكخستن",
	    odata: [{ oper: 'eq', text: "يساوي" }, { oper: 'ne', text: "نا بةرامبةر" }, { oper: 'lt', text: "بوَ" }, { oper: 'le', text: "بضوكتر" }, { oper: 'gt', text: "لة" }, { oper: 'ge', text: "طةورةتر" }, { oper: 'bw', text: "دةستثيَكردن بة" }, { oper: 'bn', text: "دةستثيَنةكردن بة" }, { oper: 'in', text: "نابيَت" }, { oper: 'ni', text: "ئةندام نيت" }, { oper: 'ew', text: "تةحقيقكردن لةطةلَ" }, { oper: 'en', text: "تةحقيق نةكردن لةطةلَ" }, { oper: 'cn', text: "تيَيداية" }, { oper: 'nc', text: "تيَيدانيية" }],
	    groupOps: [{ op: "AND", text: "طشت يا هةموو" }, { op: "OR", text: "سةرجةم" }]
	},
	edit : {
	    addCaption: "زيادكردنى توَمار",
	    editCaption: "نوسينةوةى توَمار",
	    bSubmit: "جيَطيركردن",
	    bCancel: "لابردن يا حزف",
	    bClose: "داخستن",
	    saveData: "هةلَطرتنى داتاكان ?",
	    bYes: "بةلَىَ",
	    bNo: "نةخيَر",
	    bExit: "رةتكردنةوة",
		msg: {
		    required: "ثرِكردنةوةى فوَرم",
		    number: "داخلكردنى ذمارة",
		    minValue: "كةمترين نرخ",
		    maxValue: "بةرزترين نرخ",
		    email: "ثوَستةى ئةلكترِوَنى",
		    integer: "ذمارةى دروست و راست",
		    date: "داخلَكردنى بةروار",
		    url: "ئةو عينوانة دورست نيية ('http://' یا 'https://')",
		    nodefined : " نةناسراوة!",
		    novalue : " برِى طةرِاوة!",
		    customarray : "ئاسايى",
		    customfcheck : "بوَ بوونى رِيَطةيةكى تايبةت دةبيَت ضوارطوَشةى دياريكراوت دةستكةويَت"
		}
	},
	view : {
	    caption: "نمايشى توَمار",
	    bClose: "داخستن"
	},
	del : {
	    caption: "لابردن",
	    msg: "ثةيام ?",
	    bSubmit: "لابردن",
	    bCancel: "رةتكردنةوة"
	},
	nav : {
		edittext: " ",
		edittitle: "نوسينةوةى تايتلَ",
		addtext:" ",
		addtitle: "زيادكردنى عينوان",
		deltext: " ",
		deltitle: "لابردنى عينوان",
		searchtext: " ",
		searchtitle: "طةران بة دواى رةديف",
		refreshtext: "",
		refreshtitle: "دياريكردنى عينوان",
		alertcap: "ئاطاداركردنةوة",
		alerttext: "دةقى ئاطاداركردنةوة",
		viewtext: "",
		viewtitle: "نمايشكردنى عينوان"
	},
	col : {
	    caption: "نمايش/ نمايشنةكردنى ستوون",
	    bSubmit: "جيَطيركردن",
	    bCancel: "لابردن"
	},
	errors : {
	    errcap : "هةلَة",
	    nourl : "نةبوونى عينوان",
	    norecords: "نةبوونى هيض توَماريَك",
	    model : "نمونة!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0,00'},
		currency : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0,00'},
		date : {
			dayNames:   [
				"يةك", "دوو", "سىَ", "ضوار", "ثيَنج", "شةش", "شةممة",
				"يةكشةممة", "دوو شةممة", "سىَ شةممة", "ضوار شةممة", "ثيَنج شةممة", "هةينى", "شةممة"
			],
			monthNames: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "كانونى دووةم", "شوبات", "مارس", "نيسان", "مايس", "حوزةيران", "تةموز", "ئاب", "ئةيلول", "تشرينى يةكةم", "تشرينى دووةم", "كانونى يةكةم"],
			AmPm : ["صباحا","مساءا","صباحا","مساءا"],
			S: function (j) {return j == 1 ? 'er' : 'e';},
			srcformat: 'Y-m-d',
			newformat: 'd/m/Y',
			parseRe : /[Tt\\\/:_;.,\t\s-]/,
			masks : {
				ISO8601Long:"Y-m-d H:i:s",
				ISO8601Short:"Y-m-d",
				ShortDate: "n/j/Y",
				LongDate: "l, F d, Y",
				FullDateTime: "l, F d, Y g:i:s A",
				MonthDay: "F d",
				ShortTime: "g:i A",
				LongTime: "g:i:s A",
				SortableDateTime: "Y-m-d\\TH:i:s",
				UniversalSortableDateTime: "Y-m-d H:i:sO",
				YearMonth: "F, Y"
			},
			reformatAfterEdit : false
		},
		baseLinkUrl: '',
		showAction: '',
		target: '',
		checkbox : {disabled:true},
		idName : 'id'
	}
});
})(jQuery);
