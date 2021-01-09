using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace BlazorPropertyGridComponents.Components
{

    /// <summary>
    /// Node class for hierarchical structure of property info for an object of given object graph structure.
    /// </summary>
    public class PropertyInfoAtLevelNodeComponent
    {
        public PropertyInfoAtLevelNodeComponent()
        {
            SubProperties = new Dictionary<string, PropertyInfoAtLevelNodeComponent>();
        }

        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        public Type PropertyType { get; set; }
        public Dictionary<string, PropertyInfoAtLevelNodeComponent> SubProperties { get; private set; }
        public string FullPropertyPath { get; set; }
        public bool IsClass { get; set; }
        public bool IsEditable { get; set; }

        public object NewValue { get; set; }

        public EventCallback<PropertyInfoAtLevelNodeComponent> ValueSetCallback { get; set; }

    }

}