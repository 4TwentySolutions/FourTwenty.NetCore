(function (fourTwenty, $, undefined) {
    $(function () {
        if ($("[data-pager]").length > 0) {
            $(document.body).delegate('[data-pager] a', 'click', async function (e) {
                var dataPager = $(e.target).closest("[data-pager]");
                fourTwenty.invokePager(e,
                    {
                        url: e.target.href,
                        data: {
                            sidx: dataPager.data("pager-sidx"),
                            sord: dataPager.data("pager-sord"),
                            customFilter: JSON.stringify(getAllTableFilters($(`#${dataPager.data("pager-table")}`)))
                        },
                        elementToUpdate: document.getElementById(dataPager.data("pager-target"))
                    });
            });
        }
    });

    fourTwenty.deleteItem = async function (options) {
        /// <summary>Generic method for item deletion with confirmation window.</summary>  
        /// <param name="options" type="Object"> Options for item to delete. Contains: {url, data, showApiAlert, alertTitle, buttonLoadingElement}</param >
        /// <returns type="Object">Returns an answer from server</returns>  

        // Assigning defaults
        if (typeof options === 'undefined') {
            options = {};
        }

        var settings = $.extend({
            showApiAlert: true,
            alertTitle: "Delete confirmation",
            buttonLoadingElement: null,
            requestType: "DELETE"
        }, options);


        return new Promise((resolve, reject) => {
            if (settings.buttonLoadingElement !== null)
                $(settings.buttonLoadingElement).buttonV2("loading");
            var deleteCallback = async function (result) {
                try {
                    if (result) {
                        var answer = await $.ajax({
                            url: settings.url,
                            data: settings.data,
                            type: settings.requestType
                        });
                        resolve(answer);
                    } else {
                        reject(new ConfirmPopupError());
                    }
                } catch (e) {
                    if (settings.showApiAlert) {
                        fourTwenty.displayPopupError(e.statusText);
                    }
                    reject(new ApiError(e));
                } finally {
                    if (settings.buttonLoadingElement !== null)
                        $(settings.buttonLoadingElement).buttonV2("reset");
                }
            };
            window.bootbox.confirm({
                title: settings.alertTitle,
                message: settings.alertMessage,
                callback: deleteCallback
            });
        });
    };

    fourTwenty.invokePager = async function (event, options) {
        /// <summary>Invoke an ajax pager, with parameters.</summary>  
        /// <param name="event" type="Event">Event from anchor tag element</param>  
        /// <param name="options" type="Object"> Options for pager. Contains: {url, data, elementToUpdate, showApiAlert, alertTitle, successCallback}</param >
        /// <returns type="void"></returns>  

        //Prevent default anchor event
        event.preventDefault();
        // Assigning defaults
        if (typeof options === 'undefined') {
            options = {};
        }
        var settings = $.extend({
            data: {},
            url: null,
            elementToUpdate: null,
            showApiAlert: true,
            alertTitle: "Something went wrong",
            successCallback: null
        },
            options);

        if (event.target.href !== "") {
            try {
                settings.data["page"] = fourTwenty.getParameterByName("page", event.target.href);
                var answer = await $.get(settings.url, settings.data);
                $(settings.elementToUpdate).html(answer);
                if (settings.successCallback !== null)
                    settings.successCallback(answer);
            } catch (e) {
                if (settings.showApiAlert) {
                    fourTwenty.displayPopupError(e.statusText, settings.alertTitle);
                } else {
                    throw new ApiError(e);
                }
            }
        }
    };

    fourTwenty.showServerPopup = async function (options) {
        /// <summary>Get popup from url, and display it.</summary>  
        /// <param name="options" type="Object"> Multiple options for popup.
        /// Contains: {url, data, showButtonLoading(true,false /default - true), preventDoubleClick(true,false /default - true), 
        /// domElement, removeOnHide(true, false /default - true), hiddenCallback(function), shownCallback(function), showApiAlert(true, false /default - true),
        /// alertTitle, isAjaxForm(true,false /default - false), ajaxFormOptions}
        /// </param >
        /// <returns type="void"></returns> 

        // Assigning defaults
        if (typeof options === 'undefined') {
            options = {};
        }

        var settings = $.extend({
            url: null,
            data: {},
            showButtonLoading: true,
            preventDoubleClick: true,
            domElement: null,
            removeOnHide: true,
            hiddenCallback: null,
            shownCallback: null,
            showApiAlert: true,
            alertTitle: "Something went wrong",
            isAjaxForm: false,
            ajaxFormOptions: null
        },
            options);

        try {
            if (settings.domElement !== null) {
                if ($(settings.domElement).is(":disabled") && settings.preventDoubleClick) {
                    return;
                }
                if (settings.preventDoubleClick)
                    $(settings.domElement).prop("disabled", true);
                if (settings.showButtonLoading)
                    $(settings.domElement).buttonV2("loading");
            }
            var $popupResult = $(await $.get(settings.url, settings.data));
            if (settings.removeOnHide || settings.hiddenCallback !== null) {
                $popupResult.on("hidden.bs.modal",
                    function (e) {
                        if (settings.hiddenCallback !== null)
                            settings.hiddenCallback(e);
                        if (settings.removeOnHide)
                            $(e.target).remove();
                    });
            }
            if (settings.shownCallback !== null)
                $popupResult.on("shown.bs.modal", settings.shownCallback);
            $(document.body).append($popupResult);
            $popupResult.modal("show");
            var $popupForm = $($popupResult.find("form"));
            if ($popupForm !== undefined && $popupForm !== null && $popupForm.length > 0) {
                $.validator.unobtrusive.parse($popupForm);
                if (settings.isAjaxForm)
                    $popupForm.ajaxForm(settings.ajaxFormOptions);
            }
        } catch (e) {
            if (settings.showApiAlert) {
                fourTwenty.displayPopupError(e.statusText, settings.alertTitle);
            } else {
                throw new ApiError(e);
            }
        } finally {
            if (settings.domElement !== null) {
                if (settings.showButtonLoading)
                    $(settings.domElement).buttonV2("reset");
                if (settings.preventDoubleClick)
                    $(settings.domElement).prop("disabled", false);
            }
        }
    };

    fourTwenty.displayPopupError = function (errorText, title) {
        if (title === undefined || title === "")
            title = "Something went wrong!";
        window.bootbox.alert({
            title: title,
            centerVertical: true,
            message:
                `<div class="text-center text-danger"><i class="fas fa-7x fa-times-circle"/><br/><p class="text-dark mt-3">${
                errorText}</p></div>`
        });
    };

    fourTwenty.getParameterByName = function (name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    };

    fourTwenty.sleep = function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    };
}(window.fourTwenty = window.fourTwenty || {}, jQuery));

class ConfirmPopupError extends Error {

}

class ApiError extends Error {
    constructor(response) {
        super(response.statusText); // (1)
        this.name = "ApiError"; // (2)
        this.response = response;
    }
}