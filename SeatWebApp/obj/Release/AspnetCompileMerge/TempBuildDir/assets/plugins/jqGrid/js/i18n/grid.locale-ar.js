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
	    recordtext: "العرض {0} - {1} عن {2}",
	    emptyrecords: "عدم تسجيل النص",
	    loadtext: "تحميل النص...",
	    pgtext : "الصفحة {0} ف {1}"
	},
	search : {
	    caption: "التفتيش...",
	    Find: "الحصول",
	    Reset: "الترتيب",
	    odata: [{ oper: 'eq', text: "التقابل" }, { oper: 'ne', text: "عدم التقابل" }, { oper: 'lt', text: "هو" }, { oper: 'le', text: "الاصغر" }, { oper: 'gt', text: "عن" }, { oper: 'ge', text: "الاكبر" }, { oper: 'bw', text: "الشروع بالـ" }, { oper: 'bn', text: "عدم الشروع بالـ" }, { oper: 'in', text: "لايمكن" }, { oper: 'ni', text: "عدم العضوية في" }, { oper: 'ew', text: "التحقق مع" }, { oper: 'en', text: "عدم التحقق مع" }, { oper: 'cn', text: "يحتوى على" }, { oper: 'nc', text: "عدم الاحتواء على" }],
	    groupOps: [{ op: "AND", text: "الكل" }, { op: "OR", text: "المجموع" }]
	},
	edit : {
	    addCaption: "إضافة سجل",
	    editCaption: "تحرير السجل",
	    bSubmit: "التثبيت",
	    bCancel: "الالغاء",
	    bClose: "اغلاق",
	    saveData: "خزن البيانات ?",
	    bYes: "نعم",
		bNo: "لا",
		bExit: "الالغاء",
		msg: {
		    required: "ملأ الحقول المطلوبة",
		    number: "ادخال الرقم",
		    minValue: "القيمة الادنى",
		    maxValue: "القيمة الاعلى",
		    email: "البريد الالكتروني",
		    integer: "العدد الصحيح",
		    date: "ادخال التاريخ",
		    url: "هذا العنوان غير صحيح. البادئة المطلوبة ('http://' أو 'https://') ",
		    nodefined : " غير معرّف!",
		    novalue : " اللا قيمة!",
		    customarray : "الاعتيادي",
		    customfcheck : "للحصول على طريقة مخصصة خاصة بك ، يجب أن يكون لديك مربعات محددة!"
		}
	},
	view : {
	    caption: "متابعة",
	    bClose: "اغلاق"
	},
	del : {
	    caption: "الحذف",
	    msg: "الرسالة ?",
	    bSubmit: "الحذف",
	    bCancel: "الالغاء"
	},
	nav : {
		edittext: " ",
		edittitle: "تحرير العنوان",
		addtext:" ",
		addtitle: "اضافة العنوان",
		deltext: " ",
		deltitle: "حذف العنوان",
		searchtext: " ",
		searchtitle: "البحث عن صف",
		refreshtext: "",
		refreshtitle: "تحديث العنوان",
		alertcap: "التنبيه",
		alerttext: "نص التنبيه",
		viewtext: "",
		viewtitle: "عرض العنوان"
	},
	col : {
	    caption: "عرض / عدم عرض العنوان",
	    bSubmit: "التثبيت",
	    bCancel: "الالغاء"
	},
	errors : {
	    errcap : "غلط في المتابعة",
	    nourl : "عدم وجود الرابط",
	    norecords: "عدم  وجود التسجيلات",
	    model : "نموذج!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0,00'},
		currency : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0,00'},
		date : {
			dayNames:   [
				"واحد", "إثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "السبت",
				"الأحد", "الإثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت"
			],
			monthNames: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "كانون الثاني", "فبرايل", "مارس", "أبريل", "ماي", "يونيو", "يوليو", "أغستس", "سبتمبر", "أكتوبر", "تشرينى الثاني", "ديسمبر"],
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
