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
 * @param {string} fullpropertypath - The full property path 
 * @param {*} newvalue - The new value to set
 */
function updateEditableField(fieldname, fullpropertypath, newvalue) {

   debugger 

    const element = document.getElementById(fullpropertypath.replace(/\./g, '_'));
    if (!element) {
        return;
    }

    // Handle boolean values (checkboxes)
    if (newvalue === true || newvalue === false) {
        element.checked = newvalue;
    }
    else if (element.tagName === 'SELECT') {
        // Handle select elements (dropdowns for enums)
        // Use the enum's integer value
        element.value = newvalue.toString();
    }
    else {
        // Handle text/number inputs
        element.value = newvalue;
    }
    
    // Trigger input and change events for Blazor binding
    const inputEvent = new Event('input', { bubbles: true });
    element.dispatchEvent(inputEvent);
    
    const changeEvent = new Event('change', { bubbles: true });
    element.dispatchEvent(changeEvent);
}


// Export property grid utilities to global scope
window.blazorPropertyGrid = {
    toggleExpandButton: toggleExpandButton,
    updateEditableField: updateEditableField
};
