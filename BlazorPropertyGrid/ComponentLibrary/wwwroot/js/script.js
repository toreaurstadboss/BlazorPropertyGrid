// BlazorPropertyGrid JavaScript utilities
// All functions use vanilla JavaScript (no jQuery dependency)

function toggleExpandButton(elementId) {
    const button = document.getElementById(elementId);
    if (!button) return;

    button.classList.toggle("fa-plus");
    button.classList.toggle("fa-minus");
    button.classList.toggle("fa-minus-circle");
    button.classList.remove("collapse");

    const isExpanded = button.classList.contains("fa-minus");
    button.title = isExpanded 
        ? "Click here to collapse the level of the object structure"
        : "Click here to expand the next level of the object structure";

    const target = button.getAttribute("data-target");
    const targetDiv = target ? document.querySelector(target) : button.nextElementSibling;
    if (targetDiv) {
        targetDiv.classList.toggle("show");
    }
}

/**
 * Updates an editable form field with a new value from the property grid
 * @param {string} fieldname - The ID of the form field to update
 * @param {string} fullpropertypath - The full property path (currently unused)
 * @param {*} newvalue - The new value to set
 */
function updateEditableField(fieldname, fullpropertypath, newvalue) {
    const element = document.getElementById(fieldname);
    if (!element) {
        return;
    }

    // Handle boolean values (checkboxes)
    if (newvalue === true || newvalue === false) {
        element.checked = newvalue;
    }
    else {
        // Handle text/number inputs
        element.value = newvalue;

        // Trigger change event for Blazor binding
        const event = new Event('change', { bubbles: true });
        element.dispatchEvent(event);
    }
}


// Export property grid utilities to global scope
window.blazorPropertyGrid = {
    toggleExpandButton: toggleExpandButton,
    updateEditableField: updateEditableField
};
