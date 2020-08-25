
$(function () {

    $('[rel="tooltip"]').tooltip();

    $('.help').each(function () {
        var title = $(this).next('.help-details').html();
        $(this).tooltip({
            html: true,
            trigger: 'click focuse hover',
            title: title
        });
    });

    $('#add-feedback').click(function () {
        $('#feedBackForm .validation').trigger('reset');
        var holder = $('#feedBackForm');
        holder.dialog({
            title: 'ثبت بازخورد',
            buttons: {
                'ثبت': function () {

                    var form = holder.find('form:first');
                    if (!form.valid()) return;

                    form.ajaxSubmit({
                        data: {
                            LinkAddresss: window.location.href
                        },
                        url: '/FeedBack/SaveFeedBack',
                        type: 'POST',
                        success: function (response) {
                            holder.dialog('close');

                            noty({
                                text: 'بازخورد شما ثبت گردید، با تشکر',
                                type: 'success',
                                dismissQueue: true,
                                timeout: 2000,
                                layout: 'topCenter',
                                animation: {
                                    open: { height: 'toggle' }, // jQuery animate function property object
                                    close: { height: 'toggle' }, // jQuery animate function property object
                                    easing: 'swing', // easing
                                    speed: 500 // opening & closing animation speed
                                }
                            });
                        }
                    });
                },
                'انصراف': function () {
                    $(this).dialog('close');
                }
            }
        });
    });

   


});
