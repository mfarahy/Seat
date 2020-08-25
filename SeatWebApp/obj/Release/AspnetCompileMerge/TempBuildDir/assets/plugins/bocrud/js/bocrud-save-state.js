


function bocrud_save_state() {

    var storage = new serverStorage('../Bocrud');

    //onloadcomplete -- save selection state
    $.bocrud.globalEvents.push({
        type: 'onloadcomplete', func: function () {

            if (!this.config.saveStates) return;

            var currBocrud = this;

            // save selection state
            if (!currBocrud.config.parent) {
                var ls_page_key = currBocrud.config.xml + '-grid-page';
                var ls_sel_key = currBocrud.config.xml + '-grid-sel';
                var ls_selbox_key = currBocrud.config.xml + '-grid-selbox';


                var fn = function (currBocrud) {

                    var g = currBocrud.grid();

                    var sn = null;
                    try {
                        sn = JSON.parse(storage.getItem(ls_sel_key));
                    }
                    catch (e) {
                        sn = null;
                    }
                    if (sn) {
                        for (var i = 0; i < sn.length; ++i) {
                            var dataIds = g.jqGrid("getDataIDs");
                            var found = false;
                            for (var j = 0; j < dataIds.length; ++j)
                                if (dataIds[j] == sn[i]) {
                                    found = true; break;
                                }
                            if (found) {
                                g.jqGrid("setSelection", sn[i], false);
                                currBocrud.grid_onSelectRow(sn[i]);
                            }
                        }
                    }

                    var grid_state_str = storage.getItem(currBocrud.config.xml + "-grid-colmodel");
                    if (grid_state_str) {
                        var colModel = g.jqGrid('getGridParam', 'colModel');
                        var grid_state = JSON.parse(grid_state_str);
                        if (grid_state) {
                            for (var i = 0; i < grid_state.c.length; ++i) {
                                if (grid_state.c[i].h)
                                    g.jqGrid("hideCol", grid_state.c[i].i);
                                else
                                    g.jqGrid("showCol", grid_state.c[i].i);
                            }

                            if (!g.jqGrid('getGridParam', 'autowidth'))
                                g.jqGrid('setGridWidth', grid_state.w);

                            var gw = g.jqGrid('getGridParam', 'width') * 0.95;

                            var oldgw = grid_state.w;


                            for (var i = 0; i < grid_state.c.length; ++i)
                                if (!grid_state.c[i].h)
                                    grid_state.c[i].w = grid_state.c[i].w * gw / oldgw;

                            // {
                            var ge = g.get(0).grid;
                            var gid = currBocrud.config.id + "-Grid";

                            for (var i = 0; i < grid_state.c.length; ++i) {
                                var orgCol = null, orgColIndex = -1;
                                for (var j = 0; j < colModel.length; ++j) {
                                    if (colModel[j].index == grid_state.c[i].i) {
                                        orgColIndex = j;
                                        orgCol = colModel[j]; break;
                                    }
                                }
                                if (orgCol) {

                                    var idx;

                                    if (orgCol.width != grid_state.c[i].w) {
                                        for (var j = 0; j < ge.headers.length; ++j) {
                                            if (ge.headers[j].el.id == gid + "_" + orgCol.index) {
                                                idx = j; break;
                                            }
                                        }
                                        ge.resizing = {
                                            idx: idx
                                        };
                                        ge.headers[idx].newWidth = grid_state.c[i].w;
                                        ge.dragEnd();
                                    }
                                }
                            }
                            //}

                        }
                        if (grid_state.aw) {
                            $.bocrud.alignGrid(g.closest('.ui-jqgrid').parent());
                        }
                    }
                }

                setTimeout(fn, 500, currBocrud);

                currBocrud.addEvent('ongridcomplete', function () {
                    var fn = function (currBocrud) {
                        var g = currBocrud.grid();
                        var pn = g.getGridParam('page');
                        storage.setItem(ls_page_key, pn.toString());

                        storage.setItem(ls_selbox_key, g.getGridParam('rowNum'));

                    }
                    setTimeout(fn, 0, currBocrud);

                }, 'bocrud_save_state');

                var save_grid_selection_state = function (gridObj) {
                    var ms = gridObj.jqGrid('getGridParam', 'multiselect');
                    var s = ms ? gridObj.jqGrid('getGridParam', 'selarrrow') :
        [gridObj.jqGrid('getGridParam', 'selrow')];


                    if (s && s.length > 0) {
                        storage.setItem(ls_sel_key, JSON.serialize(s));
                    }
                    else {
                        storage.removeItem(ls_sel_key);
                    }
                };

                currBocrud.addEvent('onselectrow', function (gridObj, sn) {

                    var fn = function (gridObj) {
                        save_grid_selection_state(gridObj);
                    }
                    setTimeout(fn, 0, gridObj);

                }, 'bocrud_save_state');

                currBocrud.addEvent('ondeselectrow', function (gridObj, rowid) {
                    var fn = function (gridObj, rowid) {
                        var ms = gridObj.jqGrid('getGridParam', 'multiselect');
                        if (!ms) {
                            storage.removeItem(ls_sel_key);
                        } else
                            save_grid_selection_state(gridObj, rowid);
                    }
                    setTimeout(fn, 0, gridObj, rowid);

                }, 'bocrud_save_state');
            }
        }
        , key: 'bocrud_save_state'
    });

    var save_grid_column_state = function (b, g) {

        if (!b.config.saveStates) return;

        var grid_state = {
            aw: g.jqGrid('getGridParam', 'autowidth'),
            w: g.jqGrid('getGridParam', 'width'),
            c: []
        };

        var colModel = g.jqGrid('getGridParam', 'colModel');
        for (var i = 0; i < colModel.length; ++i)
            if (colModel[i].index) {
                grid_state.c.push({
                    h: colModel[i].hidden,
                    i: colModel[i].index,
                    w: colModel[i].width
                });
            }
        storage.setItem(b.config.xml + "-grid-colmodel", JSON.serialize(grid_state));
    }

    //ongridcolumnchanged
    $.bocrud.globalEvents.push({
        type: 'ongridcolumnchanged', func: function (g) {

            save_grid_column_state(this, g);

        }, key: 'bocrud_save_state'
    });

    //ongridcolumnresize
    $.bocrud.globalEvents.push({
        type: 'ongridcolumnresize', func: function (g, nw, idx) {
            save_grid_column_state(this, g);

        }, key: 'bocrud_save_state'
    });

    //ongridsorted
    $.bocrud.globalEvents.push({
        type: 'ongridsorted', func: function (g, index, idxcol, so) {

            if (!this.config.saveStates) return;

            var fn = function (b) {

                storage.setItem(b.config.xml + "-grid-ordering", JSON.serialize({
                    sortname: index,
                    sortorder: so
                }));
            }

            setTimeout(fn, 0, this);

        }, key: 'bocrud_save_state'
    });

    //onfindbtnclicked
    $.bocrud.globalEvents.push({
        type: 'onfindbtnclicked', func: function () {

            if (!this.config.saveStates) return;

            var fn = function (currBocrud) {
                var fv = {};
                var search_form_id = "search-" + currBocrud.config.id;
                var data = $('#' + search_form_id).formSerialize();

                storage.setItem(currBocrud.config.xml + '-search-form', data);
            }

            setTimeout(fn, 0, this);

        }, key: 'bocrud_save_state'
    });

    //onsearchstyled
    $.bocrud.globalEvents.push({
        type: 'onsearchstyled', func: function (buttons, formObj, place_holder) {

            if (!this.config.saveStates) return;

            var fn = function (currBocrud) {

                var g = currBocrud.grid();

                var ordering_state_str = storage.getItem(currBocrud.config.xml + "-grid-ordering");
                if (ordering_state_str) {
                    var ordering_state = JSON.parse(ordering_state_str);
                    if (ordering_state) {
                        g.jqGrid('setGridParam', {
                            sortname: ordering_state.sortname,
                            sortorder: ordering_state.sortorder
                        });
                    }
                }


                var get_page_number = function () {
                    var ls_page_key = currBocrud.config.xml + '-grid-page';

                    var pn = 1;
                    try {
                        pn = parseInt(storage.getItem(ls_page_key), 10);
                    }
                    catch (e) {
                        pn = 1;
                    }
                    return pn;
                }

                var try_set_row_num = function () {
                    var ls_selbox_key = currBocrud.config.xml + '-grid-selbox';
                    var rn = -1;
                    try {
                        rn = parseInt(storage.getItem(ls_selbox_key), 10);
                    }
                    catch (e) {
                        rn = 10;
                    }
                    if (rn > 0) {
                        g.setGridParam({ rowNum: rn });
                        var selbox = $('.ui-pg-selbox', g.getGridParam('pager'));
                        var events = selbox.data('events');
                        selbox.removeData('events');
                        selbox.val(rn);
                        selbox.data('events', events);
                    }
                    return rn;
                }
                try_set_row_num();


                $('a[href="#' + currBocrud.config.id + '-sacc"]').click();
                /*
                var data_fetched = false;
                var sfv = storage.getItem(currBocrud.config.xml + '-search-form');

				if (sfv) {
					var fv = JSON.parse(sfv);
					var search_form_id = "search-" + currBocrud.config.id;

					var search_form = $('#' + search_form_id);

					var set_input_value = function (container,bcv) {
						container.find('.bocrud-input:input,.bocrud-control-content>input')
							.each(function (i) {
								$(this).val(bcv[i]).trigger("change");
							});
						container.addClass('state-set-value');					
					}

					var set_next_control_value = function () {

						var control = search_form.find('.bocrud-control').filter(':not(.state-set-value):first');

						if (control.length == 0) {
							//buttons[$.jgrid.search.Find](null, get_page_number());
							//data_fetched = true;
							$('a[href="#' + currBocrud.config.id + '-sacc"]').click();
							return;
						}

						var bcv = fv[control.attr('name')];

						if (!bcv || bcv.length == 0) {
							control.addClass('state-set-value');
							set_next_control_value();
							return;
						}

						if (control.find('.conditional').length>0) {

							currBocrud.addEvent('onactiondone', function (actions, propId, xml) {
								currBocrud.removeEvent('onactiondone', 'bocrud_load_state');

								var has_update = false;

								for (var i = 0; i < actions.length; ++i)
									if (actions[i].action == "Update") {
										has_update = true;
										break;
									}

								if (has_update) {
									currBocrud.addEvent('onpartialupdatedone', function () {
										currBocrud.removeEvent('onpartialupdatedone', 'bocrud_load_state');

										set_next_control_value();

									}, 'bocrud_load_state');
								} else
									set_next_control_value();


							}, 'bocrud_load_state');

							set_input_value(control, bcv);

						} else {
							set_input_value(control, bcv);

							set_next_control_value();
						}
					}

					set_next_control_value();
					
				}
                */
                //if (!data_fetched && currBocrud.config.autoLoadData) {
                //	currBocrud.do_refresh(get_page_number(), true);
                //}

                console.log('onsearchstyled bocrud_load_state was runed and removed');
            };

            setTimeout(fn, 0, this);

            this.removeEvent('onsearchstyled', 'bocrud_load_state');
            return false;
        }, key: 'bocrud_load_state'
    });

    //onresetsearchclick
    $.bocrud.globalEvents.push({
        type: 'onresetsearchclick', func: function () {

            if (!this.config.saveStates) return true;

            $.noty.success('لطفا چند لحظه صبر نمایید...');

            var f = function (b) {

                if (b.config.parent || !b.config.saveStates) return;

                var xml = b.config.xml;
                storage.removeItem(xml + '-grid-sel');
                storage.removeItem(xml + "-grid-ordering");
                storage.removeItem(xml + '-grid-page');
                storage.removeItem(xml + '-grid-selbox');
                storage.removeItem(xml + "-grid-colmodel");
                storage.removeItem(xml + '-search-form', function () {
                    window.location.reload(true);
                });
            }
            setTimeout(f, 0, this);

            return false;

        }, key: 'bocrud_save_state'
    });


    window.f5keypressed = false;
    var onf5 = function (event) {
        var e = event || window.event;

        if (f5keypressed) return;
        if (e.keyCode == 116 && e.ctrlKey) {
            f5keypressed = true;
            e.stopPropagation();
            e.preventDefault();

            var action_queue = 0;

            var oncomplete = function () {
                action_queue--;
                if (action_queue <= 0)
                    window.location.reload(true);
            }

            for (var bk in window.bocruds) {

                var b = window.bocruds[bk];

                if (b.config.parent || !b.config.saveStates) continue;

                var xml = b.config.xml;
                action_queue += 6;
                storage.removeItem(xml + '-search-form', oncomplete);
                storage.removeItem(xml + '-grid-sel', oncomplete);
                storage.removeItem(xml + "-grid-ordering", oncomplete);
                storage.removeItem(xml + '-grid-page', oncomplete);
                storage.removeItem(xml + '-grid-selbox', oncomplete);
                storage.removeItem(xml + "-grid-colmodel", oncomplete);
            }

        } else {
            f5keypressed = false;
        }
    };

    //$(document).on('keydown.state-manager', onf5);
    //$(document).on('keyup.state-manager', onf5);
    //$(document).on('keypress.state-manager', onf5);
};
$(bocrud_save_state);