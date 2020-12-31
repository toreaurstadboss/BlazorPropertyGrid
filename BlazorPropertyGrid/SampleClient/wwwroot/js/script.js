function toggleExpandButton(elementId) {
    //debugger
    $("#" + elementId).toggleClass("fa-plus");
    $("#" + elementId).toggleClass("fa-minus");
    $("#" + elementId).removeClass("collapse");

    $("#" + elementId).toggleClass("fa-minus-circle");

}