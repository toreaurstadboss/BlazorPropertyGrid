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
function updateEditableField(fieldname, fullpropertypath, newvalue, valueType) {

    const element = document.getElementById(fullpropertypath.replace(/\./g, '_'));
    if (!element) {
        return;
    }

    // Handle boolean values (checkboxes)
    if (element.type === 'checkbox' || valueType === 'boolean') {
        element.checked = newvalue === true || newvalue === 'true' || newvalue === 1;
    }
    else if (element.tagName === 'SELECT' || valueType === 'enum') {
        // Handle select elements (dropdowns and enums)
        // Try to find matching option by value or text (enum name)
        const valueStr = (newvalue !== null && newvalue !== undefined) ? newvalue.toString() : '';
        const options = element.options;
        let found = false;

        // First try exact match on option value
        for (let i = 0; i < options.length; i++) {
            if (options[i].value === valueStr) {
                element.selectedIndex = i;
                found = true;
                break;
            }
        }

        // If not found, try matching by option text (covers enum name vs int mismatch)
        if (!found) {
            for (let i = 0; i < options.length; i++) {
                if (options[i].text === valueStr) {
                    element.selectedIndex = i;
                    found = true;
                    break;
                }
            }
        }

        if (!found) {
            element.value = valueStr;
        }
    }
    else {
        // Handle text/number inputs
        element.value = newvalue;
    }
    
    // Dispatch only the event Blazor listens to for this element type
    if (element.tagName === 'SELECT' || element.type === 'checkbox') {
        const changeEvent = new Event('change', { bubbles: true });
        element.dispatchEvent(changeEvent);
    } else {
        const inputEvent = new Event('input', { bubbles: true });
        element.dispatchEvent(inputEvent);
        const changeEvent = new Event('change', { bubbles: true });
        element.dispatchEvent(changeEvent);
    }
}

/**
 * Updates a select element to show the correct selected option
 * @param {string} elementId - The ID of the select element
 * @param {*} newvalue - The value to select
 */
function updateSelectOption(elementId, newvalue) {
    const element = document.getElementById(elementId);
    if (!element || element.tagName !== 'SELECT') {
        return;
    }

    // Clear all selected options
    const options = element.options;
    for (let i = 0; i < options.length; i++) {
        options[i].selected = false;
    }
    
    // Find and select the option with matching value
    const valueStr = newvalue.toString();
    for (let i = 0; i < options.length; i++) {
        if (options[i].value === valueStr) {
            options[i].selected = true;
            break;
        }
    }
    
    // Also set element.value as fallback
    element.value = valueStr;
}


// Export property grid utilities to global scope
window.blazorPropertyGrid = {
    toggleExpandButton: toggleExpandButton,
    updateEditableField: updateEditableField,
    updateSelectOption: updateSelectOption
};
