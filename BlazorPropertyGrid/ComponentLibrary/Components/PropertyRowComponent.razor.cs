using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;

namespace BlazorPropertyGridComponents.Components
{
    public partial class PropertyRowComponent : ComponentBase
    {


        public PropertyRowComponent()
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
            JsRunTime.InvokeVoidAsync("blazorPropertyGrid.toggleExpandButton", buttonId);
        }

        protected async void setValue(ChangeEventArgs e, PropertyInfoAtLevelNodeComponent propertyInfoAtLevel)
        {
            if (propertyInfoAtLevel == null)
                return;
            try
            {
                var value = e.Value;
                
                // Handle enum type conversion
                if (propertyInfoAtLevel.PropertyType.IsEnum && value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    // Convert the integer value to the enum type
                    int intValue = int.Parse(value.ToString());
                    value = Enum.ToObject(propertyInfoAtLevel.PropertyType, intValue);
                }
                
                propertyInfoAtLevel.NewValue = value;

                await propertyInfoAtLevel.ValueSetCallback.InvokeAsync(propertyInfoAtLevel);

            }
            catch (Exception err)
            {
                Console.WriteLine(err); //TODO: fix up
            }

        }

    }
}
