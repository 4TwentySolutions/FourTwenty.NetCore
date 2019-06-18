var oldPos;
$.fn.showLoader = function () {
    oldPos = this.css("position");
    this.css("position", "relative");
    this.prepend("<div class=\"loader\"><div></div></div>");
};


$.fn.hideLoader = function () {
    this.css("position", oldPos);
    this.find(".loader").remove();
};