
; (function($) {

    $.bocrud = {
        dir:'ltr',
        captions: {
            bSelect: "Select",
            bCancel: "Cancel",
            bSave:"Save"
        },
        select: {
            bSelect: "Select",
            header: "Select an item(s)",
            checkAllText: "Select all",
            uncheckAllText: "Uncheck all",
            noneSelectedText: "No item was selected",
            selectedText: "# item of # was selected."
        },
        tree: {
            bPaste: "Paste"
        },
        msg: {
            currDel: 'Are you sure to delete item(s) ?',
            gridDel: 'Are you sure to delete item(s) ?',
            wait: 'Please wait a moment ...',
            progress: 'Item in progress',
            sending: 'Sending',
            loading: 'Loading',
            load_form: 'Loading an information form',
            sending_for_validation: 'Validating information',
            updating_form: 'Updating form',
            create_child_item_for: 'Create child item for',
            saving: 'Saving',
            none_item_selected_for_edit: 'Non item was selected for edit!',
            none_item_selected_for_delete: 'Non item(s) was selected for delete!',
            deleting: 'Deleting',
            retrieve_all:'Are you want to load all items ?'
        },
        search: {
            EQ: "Equal",
            NE: "Not equal",
            LT: "Less than",
            LE: "Less than or equal",
            GT: "Greater than",
            GE: "Greater than or equal",
            IN: "In set",
            NI: "Not in set",
            BW: "Begin with",
            BN: "Not begin with",
            EW: "End with",
            EN: "Not end with",
            CN: "Contains",
            NC: "Not contains",
            caption:'Search',
            reset:'Reset',
            print:'Print'
        }
    };
})(jQuery);
