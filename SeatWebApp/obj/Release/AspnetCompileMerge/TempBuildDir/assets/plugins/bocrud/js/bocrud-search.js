

(function ($) {
	$.bocrudSearch = $.bocrudSearch || {};
	$.extend($.bocrudSearch, {
		init: function (options) {
			var o = $.extend({
				bind: '',
				bocrud: null,
				title: 'جست‌وجو...',
				onInit: null,
				onAfterReset: null,
				onPreReset: null,
				onAdd: null,
				onPreSearch: null,
				onAfterSearch: null,
				searchOnPrevRslt: true,
				dialog: true,
				canCreate: true,
				canRemove: true,
				usefilter: false,
				print: true,
				cols: 1,
				fix: true,
				userFilters: null
			}, options || {});


			this.each(function () {

				var t = $(this);

				if (t.data('bocrud-search'))
					return;

				var canMgm = o.bocrud.config.isAdmin || !o.fix;
				var canc = o.canCreate || !o.fix, cand = o.canRemove || !o.fix;

				var dtext = '<table width="100%" class="bocrud-search" cellpadding="0" cellspacing="0" border="0">';

				if (canMgm) {
				    dtext += '<tr class="xheader"><td class="xr"><div class="ui-corner-all template"><div class="usetemplate col-sm-12"><label><input type="checkbox" name="xtemplate" id="xtemplate" /><span class="lbl">استفاده از الگوی جست‌وجو</span></label></div><div class="xt" style="padding-right:5px"><label for="xflist">فیلتر : </label><select name="xflist" class="bocrud-input" id="xflist"></select>&nbsp;';
					if (cand)
						dtext += '<input type="button" class="deletebtn btn btn-xs  btn-primary" value="حذف" name="del" />';
					dtext += '</div></div></td></tr>';

					dtext += '<tr class="xfooter"><td class="xr"><button type="button" class="btn  btn-success btn-xs addBtn"><i class="icon-plus  bigger-110 icon-only"></i></button>';
					if (canc) {
						dtext += '<div class="xt ui-corner-all save"><label for="xname">نام فیلتر : </label><input type="text" class="nametxt" style="width:240px" />';
						if (o.bocrud.config.isAdmin) {
						    dtext += '<label for="gadgetcbox"><input type="checkbox" name="gadgetcbox" id="gadgetcbox"/><span class="lbl">ابزار میزکار ؟</span></label> ';
						    dtext += '<label for="publiccbox"><input type="checkbox" name="publiccbox" id="publiccbox"/><span class="lbl">مشترک ؟</span></label> ';
						}
						dtext += '<input type="button" class="savebtn btn btn-xs  btn-primary" value="ذخیره" /></div>';
					}
					dtext += '</td></tr>';
				}
				else {
					if (!o.isAdmin && o.fix)
						dtext += '<tr class="xfooter"><td class="xr"></td></tr>';
					else
						dtext += '<tr class="xfooter"><td class="xr"><button type="button" class="btn  btn-success btn-xs addBtn"><i class="icon-plus  bigger-110 icon-only"></i></button></td></tr>';
				}
				dtext += '</table>';

				var did = t.attr('id') + 'borud-search';

				var old_tbody = $('#' + did);
				if (old_tbody.length)
					old_tbody.remove();

				var tbody = $('<form id="' + did + '" class="form-horizontal" rol="form"></form>');

				tbody.html(dtext);
				$('body').append(tbody);

				tbody.find('input.deletebtn').enable(false);
				tbody.find('input.savebtn').enable(false);

				tbody.find('input.nametxt').keyup(function () {
					var e = $(this).val().length > 0;
					tbody.find('input.savebtn').enable(e);
				});

				var data = {
					did: did,
					id: t.attr('id'),
					bocrud: o.bocrud,
					options: o
				};
				t.data('bocrud-search', data);

				var ops = tbody.find('#xflist');
				if (canc)
					ops.html('<option value=""> -- جدید -- </option>');
				else
					ops.html('');

				if (o.userFilters) {
					var ufs = o.userFilters;
					for (var i = 0; i < ufs.length; ++i)
						ops.append('<option value="' + ufs[i].Id + '" owener="' + ufs[i].Owener + '">' + ufs[i].Name + '</option>');
				}

				var disable = function () {
					tbody.find("input.savebtn,input.nametxt").attr('disabled', 'disabled');
				}
				var enable = function () {
					tbody.find("input.savebtn,input.nametxt").removeAttr('disabled');
				}

				var monitor = function () {
					tbody.find('tr.rule :input').each(function () {
						if (!$(this).is('[monitor]')) {
							$(this).attr('monitor', 'true');
							$(this).change(function () {
								if ($(this).is('[monitor]'))
									enable();
							});
						}
					});
					tbody.find('tr.rule .delBtn').each(function () {
						if (!$(this).is('[monitor]')) {
							$(this).attr('monitor', 'true');
							$(this).click(function () {
								if ($(this).is('[monitor]'))
									enable();
							});
						}
					});
					var addBtn = tbody.closest('.ui-dialog,.bocrud-search-pane').find('.addBtn');
					if (!addBtn.is('[monitor]')) {
						addBtn.attr('monitor', 'true');
						addBtn.click(function () {
							if ($(this).is('[monitor]'))
								enable();
						});
					}
				}

				tbody.find('.delBtn').click(function () {
					monitor();
					enable();
				});

				var reset = function () {
					t.bocrudSearch('reset');
					tbody.find("input.nametxt").val('');
					enable();
					tbody.find('input.deletebtn').enable(false);
				};
				if (cand)
					tbody.find("input.deletebtn").click(function () {
						var params = {
							xml: o.bocrud.config.xml,
							fid: ops.val()
						};
						if (params.fid == '')
							return;
						else
							if (!confirm("آیا از حذف این الگو اطمینان دارید ؟"))
								return;

						$.ajax({
							msg: 'حذف الگوی جست‌وجو',
							data: params,
							url: o.bocrud.config.urlPrefix + "/RemoveFilter",
							type: "POST",
							semantic: true,
							error: function (data) {
								o.bocrud.showError(data);
							},
							success: function (data) {
								if (data != "ok")
									o.bocrud.showError(data);

								ops.find("option[value='" + params.fid + "']").remove();
								ops.val('');
								var index = -1;
								for (var i = 0; i < o.userFilters.length; ++i)
									if (o.userFilters[i].Id == params.fid) {
										index = i;
										break;
									}
								o.userFilters.splice(index, 1);
								reset();
							}
						});
					});

				ops.change(function () {

					tbody = $('#' + did + ' table');

					var id = $(this).val();

					reset();

					if (id == '') {
						tbody.find('input.nametxt').val('');
						if (cand)
							tbody.find("input.deletebtn").enable(false);
						//tbody.find("input[name='del']").attr('disabled', 'disabled');
						enable();
						return;
					}
					else {

						tbody.find("input[name='del']").removeAttr('disabled');
						var uf = {};
						for (var i = 0; i < o.userFilters.length; ++i)
							if (o.userFilters[i].Id == id) {
								uf = o.userFilters[i];
								break;
							}
						tbody.find('input.nametxt').val(uf.Name);


						t.bocrudSearch('deserialize', uf.Value);

						var publiccbox = tbody.find("input#publiccbox");
						if (uf.isPublic && !publiccbox.is(':checked'))
							publiccbox.click();
						if (!uf.isPublic && publiccbox.is(':checked'))
							publiccbox.click();

						monitor();
						disable();
						return;
					}

					if (!o.bocrud.config.isAdmin &&
					(!cand || $(this).find('option[value="' + $(this).val() + '"]').attr('owener') != o.bocrud.config.ufConfig.owener)) {
						tbody.find("input[name='del']").attr('disabled', 'disabled');
					}

				});


				tbody.find('input[name=xtemplate]').click(function () {
					if ($(this).is(':checked'))
						tbody.find('.xt').show('fade');
					else
						tbody.find('.xt').hide('fade');
				});

				tbody.find('.addBtn').hover(
					function () { $(this).addClass("ui-state-hover"); },
					function () { $(this).removeClass("ui-state-hover"); }
					).click(function () {
						t.bocrudSearch('add');
					});
				if (canc) {
					var saveBtn = tbody.find('input.savebtn');
					saveBtn.click(function () {

						var filter = t.bocrudSearch('serialize');

						tbody = $('#' + did + ' table');

						var params = {
							xml: o.bocrud.config.xml,
							name: tbody.find("input.nametxt").val(),
							filter: filter,
							fid: ops.val(),
							jsbid: o.bocrud.config.id,
							isGadget: tbody.find("input#gadgetcbox").val(),
							isPublic: tbody.find("input#publiccbox").val(),
							rf: o.bocrud.config.reportPath,
							style: o.bocrud.config.style
						};

						if (params.name.length == 0)
							alert("لطفا برای فیلتر نام وارد کنید!");

						$.ajax({
							msg: 'ذخیره الگوی جست‌وجو',
							data: params,
							url: o.bocrud.config.urlPrefix + "/SaveFilter",
							type: "POST",
							semantic: true,
							error: function (data) {
								o.bocrud.showError(data);
							},
							success: function (data) {

								if (data.indexOf("ok") == 0) {
									var ufs = o.userFilters;
									var fid = data.split(':')[1];
									disable();
									if (params.fid == '') {
										ops.append('<option value="' + fid + '">' + params.name + '</option>');
										ops.val(fid);
										ufs.push({ Id: fid, Name: params.name, Value: params.filter });
									} else {
										var index = -1;
										for (var i = 0; i < ufs.length; ++i)
											if (ufs[i].Id == fid) {
												index = i;
												break;
											}
										if (index >= 0) {
											ufs[index].Name = params.name;
											ufs[index].Value = params.filter;
										}
									}
								}
								else
									o.bocrud.showError(data);
							}
						});
					});
				}

				$('#' + did).validate({ messages: "*" });

				var btns = {}, search, reset;
				btns[$.bocrud.search.caption] = search = function () {
					t.bocrudSearch('search');
				};

				btns[$.bocrud.search.reset] = reset = function () {

					if ($.isFunction(o.onPreReset))
						o.onPreReset.apply(t);

					var d = t.data('borud-search');
					if (!d.filters)
						d.filters = [];
					d.filters.splice(0, d.filters.length);

					var postData = o.bocrud.grid().jqGrid('getGridParam', 'postData');
					postData['filters'] = '';
					o.bocrud.grid().jqGrid('setGridParam', 'postData', postData)
					.jqGrid("setGridParam", { search: false })
					.trigger("reloadGrid", [{ page: 1 }]);

					if ($.isFunction(o.onAfterReset))
						o.onAfterReset.apply(t);
				};

				var footer;
				t.data('borud-search', data);

				if (o.dialog) {
					var dialog = $.bocrud.openModal($('form#' + did), {
						autoOpen: false,
						width: 700,
						title: o.title,
						buttons: btns,
						dialogClass: 'bocrud-search-pane',
						dialogParent: tbody.parent()
					});

					if (o.bind != '') {
						$(o.bind).click(function () { t.bocrudSearch('show'); });
					}
					footer = tbody.closest('.ui-dialog')
				.find('.ui-dialog-buttonset').addClass('bocrud-search-footer');

				} else {
					var btnRow = $('<tr></tr>');
					var btnCell = footer = $('<td colspan="4"></td>');
					btnRow.append(btnCell);
					for (var bc in btns)
						btnCell.append($('<button type="button"></button')
						.button({ label: bc })
						.click(btns[bc]));
					tbody.addClass('bocrud-search-pane');
					tbody.append(btnRow);

					$('#' + o.bocrud.config.id + 'Pane').before(tbody);

				}

				if ($.isFunction(o.onInit))
					o.onInit.apply(t);

				if (!o.usefilter)
					tbody.find('.xt').hide();

				var srchControl = '';
				if (o.groupOperation != 'None') {
					var gop = [];
					gop.push('نوع انطباق : ');
					gop.push('<select name="groupOp">');
					if (o.groupOperation == 'Both' || o.groupOperation == 'And')
						gop.push('<option value="AND">همه شروط</oprion>');
					if (o.groupOperation == 'Both' || o.groupOperation == 'Or')
						gop.push('<option value="OR">هر کدام که شد</oprion>');
					gop.push('</select>&nbsp;');
					srchControl = gop.join('');
				}

				if (o.searchOnPrevRslt != 'None') {
					var gop = [];
					gop.push('جست‌وجو روی نتایج قبلی : ');
					gop.push('<select name="searchOnPrevRslt">');
					gop.push('<option value="None">نادیده گرفتن</option>');
					if (o.searchOnPrevRslt == 'Both' || o.searchOnPrevRslt == 'And')
						gop.push('<option value="And">اشتراک با نتایج قبلی</option>');
					if (o.searchOnPrevRslt == 'Both' || o.searchOnPrevRslt == 'Or')
						gop.push('<option value="Or">جمع با نتایج قبلی</option>');
					gop.push('</select>&nbsp;');
					srchControl += gop.join('');
				}
				footer.prepend(srchControl);

				if (o.print) {
					var printBtn = $("<button class='bocrud-free-button'>" + $.bocrud.search.print + "</button>").button({});
					if (o.bocrud.config.style != 'Report') {
						var pfs = $("div#" + o.bocrud.config.id + "pfs");
						if (pfs.length > 0) {

							printBtn.fgmenu({
								content: pfs.html(),
								backLink: true,
								flyOut: true,
								positionOpts: { directionH: 'left' },
								onClick: function (item) {
									var path = $(item).attr('path');
									o.bocrud.onPrintClick(path);
								}
							});

						}
					} else {
						printBtn.click(function () {
							o.bocrud.onPrintClick(o.bocrud.config.reportPath);
							return false;
						});
					}
					footer.append(printBtn);
				}

				if (o.fix && !o.bocrud.config.isAdmin && o.userFilters) {
					var ufs = o.userFilters
					if (ufs.length == 1)
						t.bocrudSearch('deserialize', ufs[0].Value);

				}

				t.bocrudSearch('add');
			});
		},
		search: function () {
			var t = $(this);
			var data = t.data('borud-search');
			if (!data)
				return;
			if ($.isFunction(data.options.onPreSearch))
				data.options.onPreSearch.apply(t);

			var tbody = $('#' + data.did);

			if (!tbody.valid()) {
				return;
			}
			if (!data.filters)
				data.filters = [];

			var searchOnPrevRslt = $('#' + data.did + ' table').closest('.bocrud-search-pane').find('select[name="searchOnPrevRslt"]').val();

			if (!searchOnPrevRslt) searchOnPrevRslt = data.options.searchOnPrevRslt;

			if (searchOnPrevRslt == 'None') {
				data.filters.splice(0, data.filters.length);
			}

			var strfilters = t.bocrudSearch('serialize');
			data.filters.push(eval('(' + strfilters + ')'));

			var filters = JSON.serialize(data.filters);

			var g = data.bocrud.grid();
			if (!data.bocrud.config.autoLoadData) {
				g.jqGrid('setGridParam', { datatype: 'json' });
				data.bocrud.config.autoLoadData = true;
			}
			var postData = g.jqGrid('getGridParam', 'postData');
			postData['filters'] = filters;
			g.jqGrid('setGridParam', 'postData', postData)
					.jqGrid("setGridParam", { search: true })
					.trigger("reloadGrid", [{ page: 1 }]);

			if ($.isFunction(data.options.onAfterSearch))
				data.options.onAfterSearch.apply(t);
		},
		show: function () {
			var data = this.data('borud-search');
			if (!data)
				return;
			$('#' + data.did).dialog('open');
			//this.bocrudSearch('reset');
		},
		serialize: function () {
			var data = this.data('borud-search');
			if (!data)
				return;
			var table = $('#' + data.did + ' table');

			var group_op = table.closest('.bocrud-search-pane').find("select[name='groupOp']").val(); // puls "AND" or "OR"
			if (!group_op) group_op = data.options.groupOperation;
			if (group_op == 'undefined' || group_op == 'None' || group_op == "Both") group_op = "AND";

			var srchOnPrev = table.closest('.bocrud-search-pane').find('select[name="searchOnPrevRslt"]').val();
			if (!srchOnPrev) srchOnPrev = data.options.searchOnPrevRslt;
			if (srchOnPrev == 'undefined' || srchOnPrev == 'None' || srchOnPrev == "Both") srchOnPrev = "AND";

			var ruleGroup = "{\"groupOp\":\"" + group_op + "\",\"srchOnPrev\":\"" + srchOnPrev + "\",\"rules\":[";
			table.find("tr.rule").each(function (i) {
				var crow = $(this);

				var f = crow.find("select.field");
				var field_xmlPath = f.attr('xmlPath');
				var field_path = f.attr('fpath');
				var field_name = f.val();
				if (field_name == '')
					return;

				var xml = f.attr('xml');
				var field_caption = crow.next().find('.caption').html();
				var tOp = crow.find("select.operator :selected").val();
				var dinput = crow.find("input.vdata,select.vdata :selected");
				var tData = dinput.val();
				if (dinput.is(':checkbox'))
					tData = dinput.is(':checked');
				tData += "";
				tData = tData.replace(/\\/g, '\\\\').replace(/\"/g, '\\"');
				var field = field_path.length > 0 ? field_path.replace(/,/g, '.') : '';
				var fn = field_name[0] == '*' ? field : (field.length > 0 ? (field + '.' + field_name) : field_name);
				if (i > 0) ruleGroup += ",";
				ruleGroup += "{\"fName\":\"" + field_name + "\",";
				ruleGroup += "\"xmlPath\":\"" + field_xmlPath + "\",";
				ruleGroup += "\"field\":\"" + fn + "\",";
				ruleGroup += "\"fCaption\":\"" + field_caption + "\",";
				ruleGroup += "\"op\":\"" + tOp + "\",";
				ruleGroup += "\"data\":\"" + tData + "\"}";
			});
			ruleGroup += "]}";
			return ruleGroup;
		},
		deserialize: function (filter) {
			var data = this.data('borud-search');
			if (!data)
				return;

			this.bocrudSearch('reset');
			if (!filter)
				return;
			var table = $('#' + data.did + ' table');
			table.find('tr table.rule').remove();

			var fobj = typeof (filter) == 'string' ? eval('(' + filter + ')') : filter;
			if ($.isArray(fobj))
				fobj = fobj[0];

			table.closest('.bocrud-search-pane').find("select[name='groupOp']").val(fobj.groupOp);
			if (fobj.rules)
				for (var i = 0; i < fobj.rules.length; ++i) {
					if (!fobj.rules[i].fName) {
						var fpath = fobj.rules[i].field.split('.');
						fobj.rules[i].fName = fpath.splice(fpath.length - 1, 1);
					}
					if (!fobj.rules[i].xmlPath)
						fobj.rules[i].xmlPath = data.bocrud.config.xml;

					this.bocrudSearch('add', fobj.rules[i]);
				}
		},
		add: function (rule) {
			var t = this;
			var data = this.data('borud-search');
			if (!data)
				return;
			var table = $('#' + data.did + ' table');
			var xfooter = table.find('tr.xfooter');

			var td1 = $('<td></td>');
			var fopt = $('<select class="sf field bocrud-input" xml="' + data.bocrud.config.xml + '" xmlPath="' + data.bocrud.config.xml + '" fpath=""></select>');

			var fopt_databind = function (xml, refrence_field_info) {
				var xinfo = t.bocrudSearch('_getProperties', xml);

				if (refrence_field_info)
					fopt.html("<option value='*" + refrence_field_info.name + "'>-- " + refrence_field_info.caption + " --</option><option value='..'>-- بازگشت --</option>");
				else
					fopt.html("<option value=''>-- انتخاب کنید --</option>");

				for (var i = 0; i < xinfo.ps.length; ++i)
					if (xinfo.ps[i].isSimple)
						fopt.append("<option value='" + xinfo.ps[i].name + "'>" +
							 xinfo.ps[i].caption + "</option>");
					else
						fopt.append("<option value='" + xinfo.ps[i].name +
							 "' xml='" + xinfo.ps[i].xml + "'>&gt;" + xinfo.ps[i].caption + "</option>");
			};

			var caption = $("<div class='caption' />");

			if (!rule)
				fopt_databind(data.bocrud.config.xml, false);

			td1.append(fopt)
			.css('vertical-align', 'top');

			var td2 = $('<td></td>')
			.css('vertical-align', 'top');
			var oopt = $('<select class="sf operator bocrud-input" style="min-width:80px" disabled="disabled"></select>')
			td2.append(oopt);

			var txtValue = $('<input type="text" disabled="disabled" class="sf vdata bocrud-input"/>');
			var td3 = $('<td></td>')
			.css('vertical-align', 'top')
			.append(txtValue);

			if (data.options.searchOnPrevRslt == 'None') {
				if (!window.i) window.i = 0;

				fopt.attr('id', 'fopt' + window.i++);

				fopt.click(function () {
					fopt.find('option').removeAttr('disabled');

					table.find('select.sf.field').each(function () {
						if (fopt.attr('id') != $(this).attr('id')) {
							var v = $(this).val();

							fopt.find('option').each(function () {
								var tv = $(this).attr('value');
								if (v == tv && tv != '')
									$(this).attr('disabled', 'disabled');

							});
						}
					});
				});
			}

			fopt.change(function () {
				var _this = $(this);
				var xmlPath = _this.attr('xmlPath').split(',');
				var fpath = xmlPath.length == 1 ? '' : _this.attr('fpath').split(',');
				var xml = xmlPath[xmlPath.length - 1];
				var xinfo = t.bocrudSearch('_getProperties', xml);
				var val = _this.val(), isRefrenceProperty = false, pinfo;
				if (val[0] == '*') {
					var rpn = val.substring(1, val.length);
					var preXml = xmlPath[xmlPath.length - 2];
					var preXinfo = t.bocrudSearch('_getProperties', preXml);
					pinfo = preXinfo.getByName(rpn);
					xinfo.ps.push(pinfo);
					isRefrenceProperty = true;
				} else {
					if (val != '')
						pinfo = xinfo.getByName(val);
				}

				var cap = caption;
				var txtValue = $('<input type="text" class="sf vdata bocrud-input"/>');
				td3.html(txtValue);

				if (val == '') {

					oopt.html('');
					oopt.attr('disabled', 'disabled');
					txtValue.attr('disabled', 'disabled');
					return;
				}

				if (val != '..') {

					if (!pinfo.isSimple && !isRefrenceProperty) {
						oopt.attr('disabled', 'disabled');
						txtValue.attr('disabled', 'disabled');
						fopt_databind(pinfo.xml, pinfo);
						_this.attr('xmlPath', _this.attr('xmlPath') + ',' + pinfo.xml);
						var fp = _this.attr('fpath');
						_this.attr('fpath', fp.length > 0 ? (fp + ',' + pinfo.name) : pinfo.name);
						cap.html(cap.html() + pinfo.caption + '/');
					}

					oopt.removeAttr('disabled');
					txtValue.removeAttr('disabled');
					oopt.html('');
					if (pinfo && pinfo.sopt && pinfo.sopt.length > 0)
						for (var i = 0; i < pinfo.sopt.length; ++i)
							oopt.append('<option value="' + pinfo.sopt[i] + '">' + $.bocrud.search[pinfo.sopt[i].toUpperCase()] + '</option>');

					if (data.options.fix && !data.options.isAdmin && pinfo.sopt.length == 1)
						oopt.hide();

					if (pinfo.dataValues && pinfo.dataValues.length > 0) {
						var added = false;
						if (pinfo.dataValues.length == 2) {
							var v1 = pinfo.dataValues[0].value;
							var v2 = pinfo.dataValues[1].value;
							if ((v1 == 'true' || v1 == 'false') && (v2 == 'true' || v2 == 'false')) {
								added = true;
								txtValue = $('<input type="checkbox" class="sf vdata bocrud-input" />');
							}
						}
						if (!added) {
							txtValue = $('<select class="sf vdata bocrud-input"/>');
							for (var i = 0; i < pinfo.dataValues.length; ++i)
								txtValue.append('<option value="' + pinfo.dataValues[i].value + '">' + pinfo.dataValues[i].text + '</option>');
						}
						td3.html(txtValue);
					} else {
						switch (!0) {
							case pinfo.dataType == 'date':
								txtValue.datepicker({ showTime: false, changeMonth: true, changeYear: true });
								break;
							case pinfo.dataType == 'time':
								txtValue.timeEntry({ show24Hours: true, spinnerImage: '', separator: ':' });
								break;
							case pinfo.dataType == 'integer':
							case pinfo.dataType == 'float':
								//txtValue.addClass('number');
								break;
                            case pinfo.dataType == 'text':
                                if ($.ui && $.ui.autocomplete && pinfo.autocomplete) {
									txtValue.autocomplete({
										close: function () {
											txtValue.focus();
										},
										open: function () {
											txtValue.focus();
										},
										source: function (term, callback) {
											var d = {
												x: xml,
												p: pinfo.name,
												t: term.term
											};

											$.ajax({
												url: data.bocrud.config.urlPrefix + '/GetSuggestionWords',
												dataType: "html",
												type: "get",
												success: function (data) {
													try {
														callback(eval(data));
													}
													catch (e) { }
												},
												data: d,
												global: false
											});
										},
										select: function (event, ui) {
											txtValue.val(ui.item.value);
										}
									});
								}
								break;
						}
					}
				} // ..
				else {
					var xml = xmlPath.pop();
					var pname = fpath.pop();
					_this.attr('xmlPath', xmlPath.toString());
					_this.attr('fpath', fpath.toString());
					var p = false;
					if (xmlPath.length > 1) {
						var xinfo = t.bocrudSearch('_getProperties', xmlPath[xmlPath.length - 1]);
						p = xinfo.getByName(pname);
					}
					fopt_databind(xmlPath[xmlPath.length - 1], p);
					var caps = cap.html().split('/');
					caps.pop();
					if (xmlPath.length > 1)
						cap.html(caps.toString().replace(/,/g, '/'));
					else
						cap.html('');
					oopt.attr('disabled', 'disabled');
					txtValue.attr('disabled', 'disabled');
				}
				if (rule) {
					if (pinfo.dataType == 'bool' && rule.data == 'true')
						txtValue.attr('checked', 'checked');
					else
						txtValue.val(rule.data);
				}
			});

			if (rule) {
				fopt.html('');
				var xmlPath = rule.xmlPath.split(',');
				fopt.attr('xmlPath', rule.xmlPath);

				var target_xml = xmlPath.pop();
				var target_navfield_obj = null;
				if (xmlPath.length > 0) {
					var fieldpath = rule.field.split('.');
					fieldpath.pop();// this is simple field
					fopt.attr('fpath', fieldpath.join('.'));
					var navf = fieldpath.pop();
					var xinfo = t.bocrudSearch('_getProperties', xmlPath.pop());

					for (var i = 0; i < xinfo.ps.length; ++i) {
						if (xinfo.ps[i].name == navf && !xinfo.ps[i].isSimple) {
							target_navfield_obj = xinfo.ps[i]; break;
						}
					}
				} else
					fopt.attr('fpath', "");

				fopt_databind(target_xml, target_navfield_obj);

				if (!data.options.isAdmin && data.options.fix)
					fopt.attr('disabled', 'disabled');

				caption.html(rule.fCaption);
				fopt.val(rule.field.split('.').pop());
				fopt.change();
				oopt.val(rule.op);

				// txtValue dar method fopt_change bind mishavad
			}

			var td4 = $('<td></td>');
			if (data.options.bocrud.config.isAdmin || !data.options.fix) {
				var delBtn = $('<button type="button" class="btn btn-danger delBtn btn-xs"><i class="icon-minus  bigger-110 icon-only"></i></button>');
				td4.append(delBtn).css('vertical-align', 'top');

				delBtn.hover(
					function () { $(this).addClass("ui-state-hover"); },
					function () { $(this).removeClass("ui-state-hover"); }
					).click(function () {
						t.bocrudSearch('remove', $(this));
					});
			}

			var tr = $('<tr class="rule"></tr>');
			tr.append(td1);
			tr.append(td2);
			tr.append(td3);
			tr.append(td4);

			var trCaption = $('<tr class="rule-caption"></tr>');
			var tdCaption = $('<td colspan="4"></td>');
			tdCaption.append(caption);
			trCaption.append(tdCaption);

			var table = $('<table></table>')
			.append(tr).append(trCaption);
			var totaltd = $('<td class="rule-container"></td>').append(table);

			var added = false;
			var btr = xfooter.prev('tr');
			if (btr.length != 0) {
				var innerTable = btr.find('table.rule');
				if (innerTable.length != 0 && innerTable.find('td.rule-container').length < data.options.cols) {
					innerTable.find('tr:first').append(totaltd);
					added = true;
				}
			}
			if (!added) {
				var innerTable = $('<table class="rule"></table>');
				innerTable.append('<tr></tr>');
				innerTable.find('tr:first').append(totaltd);

				var btr = $('<tr></tr>');
				btr.append('<td></td>').children(0).append(innerTable);
				xfooter.before(btr);
			}

			//xfooter.before(tr);
			//xfooter.before(trCaption);
		},
		remove: function (elem) {
			var data = this.data('borud-search');
			if (!data)
				return;

			var totaltd = $(elem).closest('.rule-container');
			var rowtable = totaltd.closest('table');
			var row = rowtable.closest('tr');
			totaltd.remove();
			if (rowtable.find('td.rule-container:first').length == 0) row.remove();
		},
		reset: function () {
			var data = this.data('borud-search');
			if (!data)
				return;

			var tbody = $('#' + data.did + ' table');
			tbody.find('tr.rule,tr.rule-caption').remove();
			this.bocrudSearch('add');
		},
		xmlCache: [],
		_getProperties: function (xml) {
			var data = this.data('borud-search');
			var xc = $.bocrudSearch.xmlCache;
			for (var i = 0; i < xc.length; ++i) {
				if (xc[i].xml == xml)
					return xc[i];
			}

			var cacheElem = {
				xml: xml,
				ps: [],
				getByName: function (name) {
					for (var i = 0; i < this.ps.length; ++i)
						if (this.ps[i].name == name)
							return this.ps[i];
					return null;
				}
			};

			var params = {};
			params['xml'] = xml;

			$.ajax({
				msg: 'بارگذاری اطلاعات جست‌وجو',
				data: params,
				url: data.bocrud.config.urlPrefix + "/GetProperties",
				type: "POST",
				semantic: true,
				async: false,
				success: function (data) {
					cacheElem.ps = eval('(' + data + ')');
				}
			});

			xc.push(cacheElem);

			return cacheElem;
		}
	});

	$.fn.extend({

		//This is where you write your plugin's name
		bocrudSearch: function (pin) {
			var _this = this;

			if (typeof pin == 'string') {
				var fn = $.bocrudSearch[pin];
				if (!fn) {
					throw ("bocrudSearch - No such method: " + pin);
				}
				var args = $.makeArray(arguments).slice(1);
				return fn.apply(this, args);
			} else if (typeof pin === 'object' || !pin) {
				return $.bocrudSearch.init.apply(this, arguments);
			}
			else {
				$.error('Method ' + pin + ' does not exist on jQuery.bocrudSearch');
			}
		}
	});
})(jQuery);