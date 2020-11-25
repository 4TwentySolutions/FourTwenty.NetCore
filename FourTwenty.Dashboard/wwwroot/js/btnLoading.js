(function ($) {
    var oldText;
    var loading = 'loading';
    var reset = 'reset';
    $.fn.buttonV2 = function (action, content, isDisable = false) {

        if (action === loading) {
            if (isDisable)
                $(this).prop("disabled", true);
            oldText = $(this).html();
            if (content === null || content === undefined)
                $(this).html($(this).data(loading));
            else
                $(this).html(content);

        }
        if (action === reset) {
            $(this).html(oldText);
            if (isDisable)
                $(this).prop("disabled", false);
        }
    };

}(jQuery));