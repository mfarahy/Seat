
function apply_mxgraph(uniqueId, base64xml, layout, vStyle, eStyle, isEditable) {


    if (!mxClient.isBrowserSupported()) {
        mxUtils.error('Browser is not supported!', 200, false);
    }
    else {
        mxConstants.DEFAULT_FONTFAMILY = 'yekan,tahoma';
        mxGraphHandler.prototype.guidesEnabled = true;
        mxEdgeHandler.prototype.snapToTerminals = true;
        mxGraph.prototype.autoSizeCellsOnAdd = true;
        mxGraph.prototype.autoSizeCells = true;

        var container = $('#' + uniqueId).get(0);
        var graph = new mxGraph(container);

        graph.setAutoSizeCells(true);
        graph.setAllowDanglingEdges(false);
        graph.setCellsEditable(false);
        // Enables HTML markup in all labels
        graph.setHtmlLabels(true);
        graph.setCellsLocked(true);

        var casts = {};
        casts[mxConstants.STYLE_AUTOSIZE] = 'int';
        casts[mxConstants.STYLE_SPACING] = 'int';
        casts[mxConstants.STYLE_SPACING_LEFT] = 'int';
        casts[mxConstants.STYLE_ROUNDED] = 'int';
        casts[mxConstants.STYLE_FONTSTYLE] = 'int';
        casts[mxConstants.STYLE_PERIMETER_SPACING] = 'int';
        casts[mxConstants.STYLE_STROKEWIDTH] = 'int';
        casts[mxConstants.STYLE_EXIT_X] = 'float';
        casts[mxConstants.STYLE_STROKEWIDTH] = 'float';
        casts[mxConstants.STYLE_EXIT_X] = 'float';
        casts[mxConstants.STYLE_EXIT_Y] = 'float';
        casts[mxConstants.STYLE_EXIT_PERIMETER] = 'float';
        casts[mxConstants.STYLE_ENTRY_X] = 'float';
        casts[mxConstants.STYLE_ENTRY_Y] = 'float';
        casts[mxConstants.STYLE_ENTRY_PERIMETER] = 'float';
        casts[mxConstants.STYLE_EDGE] = 'eval';

        var parseStyle = function (style, targetStyle) {
            for (var k in style) {
                var value = style[k];
                if (casts[k] == 'int')
                    value = parseInt(value);
                if (casts[k] == 'float')
                    value = parseFloat(value);
                if (casts[k] == 'eval')
                    value = eval('(' + value + ')');
                targetStyle[k] = value;
            }
        }

        var defaultVertexStyle = graph.getStylesheet().createDefaultVertexStyle();
        parseStyle(vStyle, defaultVertexStyle);
        graph.getStylesheet().putDefaultVertexStyle(defaultVertexStyle);

        var defaultEdgeStyle = graph.getStylesheet().createDefaultEdgeStyle();
        parseStyle(eStyle, defaultEdgeStyle);
        graph.getStylesheet().putDefaultEdgeStyle(defaultEdgeStyle);

        var xml = Base64.decode(base64xml);
        var doc = mxUtils.parseXml(xml);
        var enc = new mxCodec(doc.documentElement.ownerDocument);
        enc.decode(doc.documentElement, graph.getModel());

        var cells = graph.getModel().cells;
        var groups = [];
        for (var key in cells) {
            var cell = cells[key];

            graph.autoSizeCell(cell);
            if (cell.parent && cell.parent.id != '0' && cell.parent.id != '1') {
                var exists = false;
                for (var g = 0; g < groups.length; ++g) {
                    if (groups[g].id == cell.parent.id) {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                    groups.push(cell.parent);
            }
        }
        for (var i = 0; i < groups.length; ++i) {
            graph.updateGroupBounds(groups[i], 5, true, 2, 2, 2, 2);
        }

        if (!layout || layout == null || layout.length == 0) layout = 'mxFastOrganicLayout';
        var layout = eval('(new ' + layout + '(graph))');
        graph.getModel().beginUpdate();
        try {
            layout.execute(graph.getDefaultParent());
        }
        catch (e) {
            throw e;
        }
        finally {

            // Default values are 6, 1.5, 20
            var morph = new mxMorphing(graph, 10, 1.7, 20);
            morph.addListener(mxEvent.DONE, function () {
                graph.getModel().endUpdate();
            });
            morph.startAnimation();

        }
    }
};