using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace BlazorPropertyGridComponents.Components
{

    /// <summary>
    /// Node class for hierarchical structure of property info for an object of given object graph structure.
    /// </summary>
    public class HierarchicalPropertyInfo
    {
        public HierarchicalPropertyInfo()
        {
            SubProperties = new Dictionary<string, HierarchicalPropertyInfo>();
        }

        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        public Type PropertyType { get; set; }
        public Dictionary<string, HierarchicalPropertyInfo> SubProperties { get; private set; }
        public string FullPropertyPath { get; set; }
        public bool IsClass { get; set; }
        public bool IsEditable { get; set; }

        public object NewValue { get; set; }

        public EventCallback<HierarchicalPropertyInfo> ValueSetCallback { get; set; }

    }

}