(function ($) {
    var oldText;
    var loading = 'loading';
    var reset = 'reset';
    $.fn.buttonV2 = function (action,content) {

        if (action === loading) {
            oldText = $(this).html();
            if (content === null || content === undefined)
                $(this).html($(this).data(loading));
            else
                $(this).html(content);
           
        }
        if (action === reset) {
            $(this).html(oldText);
        }
    };

}(jQuery));