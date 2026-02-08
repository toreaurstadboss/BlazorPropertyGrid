// BlazorPropertyGrid JavaScript Interop
// This file demonstrates how the library provides JavaScript interop features wrapped in a .NET API

window.exampleJsFunctions = {
    showPrompt: function (message) {
        return prompt(message, 'Type anything here');
    }
};

// Export property grid utilities to global scope
window.blazorPropertyGrid = {
    toggleExpandButton: toggleExpandButton,
    updateEditableField: updateEditableField
};
