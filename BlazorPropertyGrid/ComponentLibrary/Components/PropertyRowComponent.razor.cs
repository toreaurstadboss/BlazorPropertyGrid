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

        [Parameter] public EventCallback<object> OnValueSet { get; set; }

        [Parameter] public HierarchicalPropertyInfo PropertyInfoAtLevel { get; set; }

        [Parameter] public int Depth { get; set; }

        [Parameter] public List<string> DisplayedFullPropertyPaths { get; set; }

        [Inject] protected IJSRuntime JsRunTime { get; set; }

        protected void ToggleExpandButton(MouseEventArgs e, string buttonId)
        {
            JsRunTime.InvokeVoidAsync("blazorPropertyGrid.toggleExpandButton", buttonId);
        }

        protected async void SetValue(ChangeEventArgs e, HierarchicalPropertyInfo propertyInfoAtLevel)
        {
            if (propertyInfoAtLevel == null)
                return;
            try
            {
                var value = e.Value;
                
                // Handle enum type conversion - support both integer and name string values
                if (propertyInfoAtLevel.PropertyType.IsEnum && value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    var valueStr = value.ToString();
                    if (int.TryParse(valueStr, out int intValue))
                    {
                        value = Enum.ToObject(propertyInfoAtLevel.PropertyType, intValue);
                    }
                    else if (Enum.TryParse(propertyInfoAtLevel.PropertyType, valueStr, ignoreCase: true, out var parsedEnum))
                    {
                        value = parsedEnum;
                    }
                }
                
                propertyInfoAtLevel.NewValue = value;
                // Update PropertyValue so UI reflects the change immediately
                propertyInfoAtLevel.PropertyValue = value;

                await propertyInfoAtLevel.ValueSetCallback.InvokeAsync(propertyInfoAtLevel);

            }
            catch (Exception err)
            {
                Console.WriteLine(err); //TODO: fix up
            }

        }

    }
}
