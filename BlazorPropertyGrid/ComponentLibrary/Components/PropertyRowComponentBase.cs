using System.Collections.Generic;
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
        }
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

    }
}
