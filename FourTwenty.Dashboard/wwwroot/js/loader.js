$.fn.showLoader = function () {
    this.data("old-position", this.css("position"));
    this.css("position", "relative");
    this.prepend("<div class=\"loader\"><div></div></div>");
};


$.fn.hideLoader = function () {
    this.css("position", this.data("old-position"));
    this.find(".loader").remove();
};