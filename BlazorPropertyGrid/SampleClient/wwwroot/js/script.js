function toggleExpandButton(elementId) {
	//debugger
	$("#" + elementId).toggleClass("fa-plus");
	$("#" + elementId).toggleClass("fa-minus");
	$("#" + elementId).removeClass("collapse");

	$("#" + elementId).toggleClass("fa-minus-circle");

	if ($("#" + elementId).hasClass("fa-plus")) {
		$("#" + elementId).attr("title", "Click here to expand the next level of the object structure");
	} else {
		$("#" + elementId).attr("title", "Click here to collapse the level of the object structure");
	}

}


function updateEditableField(fieldname, fullpropertypath, newvalue) {
	$("#" + fieldname).val(newvalue);
}