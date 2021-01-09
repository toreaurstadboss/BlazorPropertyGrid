using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorPropertyGridComponents.Components
{
    public class PropertyRowComponentBase : ComponentBase
    {


        public PropertyRowComponentBase()
        {
            DisplayedFullPropertyPaths = new List<string>();
            ValueIsNotSet = "[Value is blank]";
        }

        public bool IsEditingAllowed { get; set; }

        public string ValueIsNotSet { get; set; }

        [Parameter]
        public EventCallback<object> OnValueSet { get; set; }

        [Parameter]
        public PropertyInfoAtLevelNodeComponent PropertyInfoAtLevel { get; set; }

        [Parameter]
        public int Depth { get; set; }

        [Parameter]
        public List<string> DisplayedFullPropertyPaths { get; set; }

        [Inject]
        protected IJSRuntime JsRunTime { get; set; }

        protected void toggleExpandButton(MouseEventArgs e, string buttonId)
        {
            JsRunTime.InvokeVoidAsync("toggleExpandButton", buttonId);
        }

        protected async void setValue(ChangeEventArgs e, PropertyInfoAtLevelNodeComponent propertyInfoAtLevel)
        {
            if (propertyInfoAtLevel == null)
                return;
            try
            {
                propertyInfoAtLevel.NewValue = e.Value; 

                await propertyInfoAtLevel.ValueSetCallback.InvokeAsync(propertyInfoAtLevel);

            }
            catch (Exception err)
            {
                Console.WriteLine(err); //TODO: fix up
            }

        }

    }
}
