using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorPropertyGridComponents.Components
{
    public class PropertyGridComponentBase : ComponentBase
    {

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter] public object DataContext { get; set; }

        [Parameter] public string ObjectTitle { get; set; }


        [Parameter]
        public EventCallback<PropertyChangedInfoNotificationInfoPayload> PropertySetValueCallback { get; set; }

        public string CssStyleEditbutton { get; set; }


        [Parameter] public bool IsEditingAllowed { get; set; }

        public Dictionary<string, PropertyInfoAtLevelNodeComponent> Props { get; set; }


        public PropertyGridComponentBase()
        {
            Props = new Dictionary<string, PropertyInfoAtLevelNodeComponent>();
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

        private PropertyInfoAtLevelNodeComponent MapPropertiesOfDataContext(string propertyPath, object parentObject,
            PropertyInfo currentProp)
        {
            if (parentObject == null)
                return null;

            var publicProperties = parentObject.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var propertyNode = new PropertyInfoAtLevelNodeComponent
            {
                PropertyName = currentProp?.Name ?? "ROOT",
                PropertyValue = parentObject,
                PropertyType = parentObject.GetType(),
                FullPropertyPath = TrimFullPropertyPath($"{propertyPath}.{currentProp?.Name}") ?? "ROOT",
                IsClass = parentObject.GetType().IsClass && parentObject.GetType().Namespace != "System",
            };
            propertyNode.ValueSetCallback = new EventCallback<PropertyInfoAtLevelNodeComponent>(this, new Action<PropertyInfoAtLevelNodeComponent>(OnValueSetCallback));

            foreach (var p in publicProperties)
            {
                var propertyValue = p.GetValue(parentObject, null); 

                if (!IsNestedProperty(p))
                {
                    var subprop = new PropertyInfoAtLevelNodeComponent
                    {
                        IsClass = false,
                        FullPropertyPath = TrimFullPropertyPath($"{propertyPath}.{p.Name}"),
                        PropertyName = p.Name,
                        PropertyValue = propertyValue,
                        PropertyType = p.PropertyType
                        //note - SubProperties are default empty if not nested property of course.
                    };
                    subprop.ValueSetCallback = new EventCallback<PropertyInfoAtLevelNodeComponent>(this, new Action<PropertyInfoAtLevelNodeComponent>(OnValueSetCallback));

                    propertyNode.SubProperties.Add(p.Name, subprop);
                }
                else
                {
                    var subprop = new PropertyInfoAtLevelNodeComponent
                    {
                        IsClass = true,
                        FullPropertyPath = propertyPath + p.Name,
                        PropertyName = p.Name,
                        PropertyValue = MapPropertiesOfDataContext(TrimFullPropertyPath($"{propertyPath}.{p.Name}"),
                            propertyValue, p),
                        PropertyType = p.PropertyType
                        //note - SubProperties are default empty if not nested property of course.
                    };

                    subprop.ValueSetCallback = new EventCallback<PropertyInfoAtLevelNodeComponent>(this, new Action<PropertyInfoAtLevelNodeComponent>(OnValueSetCallback));

                    //we need to add the sub property but recurse also call to fetch the nested properties 
                    propertyNode.SubProperties.Add(p.Name, subprop);
                }
            }

            return propertyNode;
        }

        protected void toggleEditButton(MouseEventArgs e)
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

        private void SetEditFlagRecursive(PropertyInfoAtLevelNodeComponent prop, bool isEditable)
        {
            prop.IsEditable = isEditable;
            if (prop.SubProperties.Any())
            {
                foreach (var subprop in prop.SubProperties)
                {
                    SetEditFlagRecursive(subprop.Value, isEditable);
                }
            }

            if (prop.PropertyValue is PropertyInfoAtLevelNodeComponent)
            {
                var castedPropertyValue = (PropertyInfoAtLevelNodeComponent) prop.PropertyValue;
                if (castedPropertyValue.SubProperties != null)
                {
                    foreach (var subprop in castedPropertyValue.SubProperties)
                    {
                        SetEditFlagRecursive(subprop.Value, isEditable);
                    }
                }
            }
        }

        protected void OnValueSetCallback(PropertyInfoAtLevelNodeComponent p)
        {
            if (!PropertySetValueCallback.HasDelegate)
                return;
            PropertySetValueCallback.InvokeAsync(new PropertyChangedInfoNotificationInfoPayload
            {
                FieldName = p.PropertyName,
                FullPropertyPath = p.FullPropertyPath,
                Value = p.NewValue
            });

        }

        protected void toggleExpandButton(MouseEventArgs e, string buttonId)
        {
            JsRuntime.InvokeVoidAsync("toggleExpandButton", buttonId);
        }

        private string TrimFullPropertyPath(string fullpropertypath)
        {
            if (string.IsNullOrEmpty(fullpropertypath))
                return fullpropertypath;
            return fullpropertypath.TrimStart('.').TrimEnd('.');
        }

    }

}