// BlazorPropertyGrid JavaScript utilities
// All functions use vanilla JavaScript (no jQuery dependency)

/**
 * Toggles the expand/collapse button icon and tooltip for property grid items
 * @param {string} elementId - The ID of the button element to toggle
 */
function toggleExpandButton(elementId) {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Toggle icon classes
    element.classList.toggle("fa-plus");
    element.classList.toggle("fa-minus");
    element.classList.remove("collapse");
    element.classList.toggle("fa-minus-circle");

    // Update tooltip based on current state
    if (element.classList.contains("fa-plus")) {
        element.setAttribute("title", "Click here to expand the next level of the object structure");
    } else {
        element.setAttribute("title", "Click here to collapse the level of the object structure");
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