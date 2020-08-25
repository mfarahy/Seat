function checkOverflow(el) {
    var curOverflow = el.style.overflow;
    if (!curOverflow || curOverflow === "visible")
        el.style.overflow = "hidden";

    var isOverflowing = el.clientWidth + 10 < el.scrollWidth
       || el.clientHeight + 10 < el.scrollHeight;

    el.style.overflow = curOverflow;

    return isOverflowing;
}

function gridAutoWidth(g, w) {
    var resize = function () {
        var grid = $("#" + g);

        var p = grid.closest('.ui-jqgrid').parent();
        var row = grid.closest('.row');
        var fw = row.width();
        if (fw <= 0)
            fw = $(window).width();
        var mw = Math.min(p.width(), $(window).width() , fw);
        


        if (grid.data('prev-width')) {
            var pw = grid.data('prev-width');

            var gw = grid.jqGrid('getGridParam', 'width');
            var larg_changed = Math.abs(gw - mw) > 50;

            //if (!larg_changed && !grid.is(':visible')) {
            //    console.log('not width changed !', gw, mw, p.width());
            //    if (mw == gw && gw <= 100) {
            //        console.log('run resize again');
            //        setTimeout(resize, 500);
            //    }
            //    return;
            //}
        }

        var fit = function () {
            if (mw <= 0) {
                return;
            }
            if (grid.data.mw != mw) {
                //console.log('grid is not fit !', gw, mw, p.width());
                grid.data.width = mw;
                var nw = Math.round(mw * (w / 100));
                grid.jqGrid('setGridWidth', nw);
                grid.data('prev-width', nw);
            }
        }
        fit();

        var colModel = grid.jqGrid('getGridParam', 'colModel');

        var gw = grid.jqGrid('getGridParam', 'width');

        // تشخیص بدیم که گرید احتیاج به بزرگتر شدن دارد یا کوچکتر شدن
        var gc = grid.closest('.ui-jqgrid');

        var hideColumn = function () {
            var hideList = [];

            for (var i = 0; i < colModel.length; ++i) {
                var c = colModel[i];
                if (c.index && !c.hidden) {
                    if (c.dispp > 0) {
                        c.colIndex = i;
                        hideList.push(c);
                    }
                }
            }

            if (hideList.length == 0) return false;

            hideList.sort(function (item1, item2) {
                if (item1.dispp == item2.dispp)
                    return item1.colIndex < item2.colIndex;
                return item1.dispp < item2.dispp;
            });
            var candidate_column = hideList.splice(0, 1)[0];
            //self.jqGrid("showCol", colModel[this.value].name);

            grid.jqGrid("hideCol", candidate_column.name);

            candidate_column.hide_to_fit = true;

            fit();

            return true;
        }

        var getShowCandidateColumn = function () {
            var showList = [];

            for (var i = 0; i < colModel.length; ++i) {
                var c = colModel[i];
                if (c.index && c.hidden && c.hide_to_fit) {
                    if (c.dispp == 0) c.dispp = 1000;
                    c.colIndex = i;
                    showList.push(c);
                }
            }

            if (showList.length == 0) return false;

            showList.sort(function (item1, item2) {
                if (item1.dispp == item2.dispp)
                    return item1.colIndex > item2.colIndex;
                return item1.dispp > item2.dispp;
            });

            var candidate_column = showList.splice(0, 1)[0];

            return candidate_column;
        }

        var showColumn = function () {
            var cc = getShowCandidateColumn();

            if (!cc) return false;

            //self.jqGrid("showCol", colModel[this.value].name);

            grid.jqGrid("showCol", cc.name);

            fit();

            return true;
        }

        var get_decide = function (offset_width) {
            var offset = offset_width || 0;
            var occ = 0;//overflowed column count
            var okcc = 0, tvcc = 0; // ok width column count, total visibile column count
            if (!colModel) return;

            var first_run = !grid.data('first-align-run');
            grid.data('first-align-run', true);
            if (first_run) {
                for (var i = 0; i < colModel.length; ++i) {
                    var c = colModel[i];
                    if (!c.index) continue;
                    c._widthOrg = c.width;
                }
            }

            for (var i = 0; i < colModel.length; ++i) {
                var c = colModel[i];

                if (!c.index) continue;

                if ((c.width + offset) / c.widthOrg < 0.7 && !c.hidden)
                    ++occ;
                if ((c.width + offset) > c.widthOrg && !c.hidden)
                    ++okcc;
                if (!c.hidden) tvcc++;
            }
            var result = "";
            if (colModel.length < 3)
                result = "do-nothing";
            else {
                // اگر بیشتر از 35 درصد ستون ها فشرده شده بودند
                if ((1.0 * occ / colModel.length) > 0.35)
                    result = "decrease";
                else
                    // اگر بیشتر از 10 درصد ستون ها جا زیاد داشتند
                    if ((1.0 * okcc / tvcc) > 0.05)
                        result = "increase";
                    else
                        result = "do-nothing";
            }
            //console.log(gc.attr('id'), ' -->', result, "okcc", okcc, "tvcc", tvcc, "occ", occ);
            return result;
        }

        var main = function () {
            var decide = get_decide();

            if (decide == "decrease") {
                if (hideColumn())
                    setTimeout(main, 0);

            } else if (decide == "increase") {

                var tvcc = 0;
                for (var i = 0; i < colModel.length; ++i) {
                    var c = colModel[i];
                    if (!c.hidden) tvcc++;
                }

                var cc = getShowCandidateColumn();

                if (get_decide(-cc.width / tvcc) == "increase")
                    if (showColumn())
                        setTimeout(main, 0);
            }
        }
        main();
    };
    resize();
    //$(window).resize(resize);
}

$.jgrid.extend({
    toolbarButtonAdd: function (elem, p) {
        p = $.extend({
            id: "",
            caption: "",
            title: '',
            context: '',
            buttonicon: '',
            onClickButton: null,
            position: "last",
            classes: '',
            dependToRow: false
        }, p || {});
        var $t = this;
        return this.each(function () {
            if (!this.grid) { return; }
            var tableString = "<ul class=\"ui-widget ui-helper-clearfix ui-toolbar-table\" grid=\"" + this.p.id + "\">";
            tableString += "</ul>";
            if (elem.indexOf("#") != 0) {
                elem = "#" + elem;
            }

            if ($(elem).length == 0)
                elem = '#t_' + this.p.id;

            if ($(elem).children('ul').length === 0) {
                $(elem).append(tableString);
            }
            var tbd = $("<li title=\"" + p.title + "\"  style='cursor:pointer'></li>");

            if (!p.buttonicon && !p.title && !p.caption) {
                $(tbd).attr('style', 'cursor: pointer; padding-right: 0px; padding-left: 0px; margin-left: 0px; margin-right: 0px;');
                $(tbd).append("<span class='ui-toolbar-div ui-icon ui-icon-grip-dotted-vertical' style='margin-right: 0px; margin-left: 0px;'></span>");
            } else {
                $(tbd).addClass('ui-toolbar-button ui-corner-all ui-state-default ' + p.classes + (p.dependToRow ? ' ui-toolbar-dependtorow ui-state-disabled' : ''))
                    .append("<span class=\"ui-toolbar-div ui-icon " + p.buttonicon + "\"></span>")
                    .click(function (e) {
                        if ($.isFunction(p.onClickButton) && !$(this).hasClass('ui-state-disabled')) {
                            var selectedRow;
                            if ($t.getGridParam('multiselect'))
                                selectedRow = $t.getGridParam('selarrrow');
                            else
                                selectedRow = $t.getGridParam('selrow');
                            p.onClickButton(selectedRow, $t, p.context);
                        }

                        return false;
                    })
                    .hover(
                        function () { $(this).addClass("ui-state-hover"); },
                        function () { $(this).removeClass("ui-state-hover"); }
                        );

                if (p.showText)
                    $(tbd).append("<span class=\"ui-toolbar-div\" style=\"float:right;padding:6px;\">" + p.caption + "</span>");
            }

            if (p.id) { $(tbd).attr("id", p.id); }
            if (p.align) { $(elem).attr("align", p.align); }
            var findnav = $(elem).children('ul');
            p.position = 'last';
            if (p.position === 'first') {
                if ($(findnav).find('li').length === 0) {
                    $(findnav).append(tbd);
                } else {
                    $(findnav).prepend(tbd);
                }
            } else {
                $(findnav).append(tbd);
            }

            return tbd;
        });
    }
});

$.jgrid.extend({ layout: { dir: 'rtl' } });

$.fn.fmatter.integer = function (cellval, opts) {
    var op = $.extend({}, opts.integer);
    if (opts.colModel !== undefined && opts.colModel.formatoptions !== undefined) {
        op = $.extend({}, op, opts.colModel.formatoptions);
    }
    if ($.fmatter.isEmpty(cellval)) {
        return op.defaultValue;
    }

    var result = $.fmatter.util.NumberFormat(cellval, op);
    if (result.toString() == 'NaN')
        return cellval;

    return result;
};

$.fn.fmatter.number = function (cellval, opts) {
    var op = $.extend({}, opts.number);
    if (opts.colModel !== undefined && opts.colModel.formatoptions !== undefined) {
        op = $.extend({}, op, opts.colModel.formatoptions);
    }
    if ($.fmatter.isEmpty(cellval)) {
        return op.defaultValue;
    }
    var result = $.fmatter.util.NumberFormat(cellval, op);
    if (result.toString() == 'NaN')
        return cellval;

    return result;
};