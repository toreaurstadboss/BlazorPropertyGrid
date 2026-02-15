using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlazorPropertyGridComponents.Components
{
    public partial class PropertyGridComponent : ComponentBase
    {

        [Inject] public IJSRuntime JsRuntime { get; set; }

        [Parameter] public object DataContext { get; set; }

        [Parameter] public string ObjectTitle { get; set; }

        [Parameter] public EventCallback<PropertyChangedInfoNotificationInfoPayload> PropertySetValueCallback { get; set; }

        [Parameter] public bool IsEditingAllowed { get; set; }

        public string CssStyleEditbutton { get; set; }

        public Dictionary<string, HierarchicalPropertyInfo> Props { get; set; }


        public PropertyGridComponent()
        {
            Props = new Dictionary<string, HierarchicalPropertyInfo>();
            CssStyleEditbutton = "color:white";
        }

        protected override void OnParametersSet()
        {
            Props.Clear();
            if (DataContext == null)
                return;

            Props["ROOT"] = MapPropertiesOfDataContext(string.Empty, DataContext, null);
            SetEditFlag();

            StateHasChanged();
        }


        private bool IsNestedProperty(PropertyInfo pi) =>
            pi.PropertyType.IsClass && pi.PropertyType.Namespace != "System";

        private HierarchicalPropertyInfo MapPropertiesOfDataContext(string propertyPath, object parentObject,
            PropertyInfo currentProp)
        {
            if (parentObject == null)
                return null;

            var publicProperties = parentObject.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var propertyNode = new HierarchicalPropertyInfo
            {
                PropertyName = currentProp?.Name ?? "ROOT",
                PropertyValue = parentObject,
                PropertyType = parentObject.GetType(),
                FullPropertyPath = TrimFullPropertyPath($"{propertyPath}.{currentProp?.Name}") ?? "ROOT",
                IsClass = parentObject.GetType().IsClass && parentObject.GetType().Namespace != "System",
            };
            propertyNode.ValueSetCallback = new EventCallback<HierarchicalPropertyInfo>(this, new Action<HierarchicalPropertyInfo>(OnValueSetCallback));

            foreach (var p in publicProperties)
            {
                var propertyValue = p.GetValue(parentObject, null);

                if (!IsNestedProperty(p))
                {
                    var subprop = new HierarchicalPropertyInfo
                    {
                        IsClass = false,
                        FullPropertyPath = TrimFullPropertyPath($"{propertyPath}.{p.Name}"),
                        PropertyName = p.Name,
                        PropertyValue = propertyValue,
                        PropertyType = p.PropertyType
                        //note - SubProperties are default empty if not nested property of course.
                    };
                    subprop.ValueSetCallback = new EventCallback<HierarchicalPropertyInfo>(this, new Action<HierarchicalPropertyInfo>(OnValueSetCallback));

                    propertyNode.SubProperties.Add(p.Name, subprop);
                }
                else
                {
                    var subprop = new HierarchicalPropertyInfo
                    {
                        IsClass = true,
                        FullPropertyPath = propertyPath + p.Name,
                        PropertyName = p.Name,
                        PropertyValue = MapPropertiesOfDataContext(TrimFullPropertyPath($"{propertyPath}.{p.Name}"),
                            propertyValue, p),
                        PropertyType = p.PropertyType
                        //note - SubProperties are default empty if not nested property of course.
                    };

                    subprop.ValueSetCallback = new EventCallback<HierarchicalPropertyInfo>(this, new Action<HierarchicalPropertyInfo>(OnValueSetCallback));

                    //we need to add the sub property but recurse also call to fetch the nested properties 
                    propertyNode.SubProperties.Add(p.Name, subprop);
                }
            }

            return propertyNode;
        }

        protected void ToggleEditButton(MouseEventArgs e)
        {
            IsEditingAllowed = !IsEditingAllowed; //toggle edit flag 

            SetEditFlag();

            if (CssStyleEditbutton.IndexOf("white", StringComparison.Ordinal) >= 0)
                CssStyleEditbutton = "color:yellow;background:orange";
            else
                CssStyleEditbutton = "color:white";
        }

        private void SetEditFlag()
        {
            foreach (var prop in this.Props)
            {
                SetEditFlagRecursive(prop.Value, IsEditingAllowed);
            }
        }

        private void SetEditFlagRecursive(HierarchicalPropertyInfo prop, bool isEditable)
        {
            prop.IsEditable = isEditable;
            if (prop.SubProperties.Any())
            {
                foreach (var subprop in prop.SubProperties)
                {
                    SetEditFlagRecursive(subprop.Value, isEditable);
                }
            }

            if (prop.PropertyValue is HierarchicalPropertyInfo)
            {
                var castedPropertyValue = (HierarchicalPropertyInfo)prop.PropertyValue;
                if (castedPropertyValue.SubProperties != null)
                {
                    foreach (var subprop in castedPropertyValue.SubProperties)
                    {
                        SetEditFlagRecursive(subprop.Value, isEditable);
                    }
                }
            }
        }

        protected void OnValueSetCallback(HierarchicalPropertyInfo p)
        {
            if (!PropertySetValueCallback.HasDelegate)
                return;

            var value = p.NewValue;
            var valueType = "text";

            if (p.PropertyType.IsEnum)
            {
                valueType = "enum";
                // Convert enum to its name string for the form's InputSelect compatibility
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    if (Enum.IsDefined(p.PropertyType, value))
                    {
                        value = Enum.GetName(p.PropertyType, value);
                    }
                    else
                    {
                        value = value.ToString();
                    }
                }
                else
                {
                    value = null;
                }
            }
            else if (p.PropertyType == typeof(bool))
            {
                valueType = "boolean";
            }
            else if (p.PropertyType == typeof(int) || p.PropertyType == typeof(double) ||
                     p.PropertyType == typeof(float) || p.PropertyType == typeof(decimal))
            {
                valueType = "number";
            }
            else if (p.PropertyType == typeof(DateTime))
            {
                valueType = "date";
            }

            PropertySetValueCallback.InvokeAsync(new PropertyChangedInfoNotificationInfoPayload
            {
                FieldName = p.PropertyName,
                FullPropertyPath = p.FullPropertyPath,
                Value = value,
                ValueType = valueType
            });

        }

        protected void toggleExpandButton(MouseEventArgs e, string buttonId)
        {
            JsRuntime.InvokeVoidAsync("blazorPropertyGrid.toggleExpandButton", buttonId);
        }

        private string TrimFullPropertyPath(string fullpropertypath)
        {
            if (string.IsNullOrEmpty(fullpropertypath))
                return fullpropertypath;
            return fullpropertypath.TrimStart('.').TrimEnd('.');
        }

    }

}