# Blazor property grid

This is a property grid component that can be used to inspect objects in Blazor apps.
It supports properties that are at the root level or at nested levels. This means you can also inspect properties that have got a nested structure, internal properties again of their own.

It has been tested using Blazor WASM (Web-Assembly) running with .NET 10. You will find a sample client in the project _BlazoSampleClient_.

The property grid itself is inside the razor class library _BlazorPropertyGridComponents_. 

The property grid component is licensed with MIT license and provided with an as-is guarantee. If used inside production envionment, it is your own responsibility that the component works. You may freely branch this repository or even make use of it in commercial libraries. 
The author has provided this Blazor component as a mere academic exercise and hobby-project to learn Blazor more in detail. 

Screenshot of the component is shown below. The property grid is shown to the right and is editing an object that is also the object the Edit Form to the left is editing. Changes are picked up automatically via data binding in the running Blazor WASM app.

![Picture of running sample client showing the Blazor Property Grid](https://raw.githubusercontent.com/toreaurstadboss/BlazorPropertyGrid/main/BlazorPropertyGrid/ComponentLibrary/screengrabv12.png)

Please note that this component expects that you use Bootstrap and Font Awesome. It will be fairly easy to avoid this coupling if needed to switch over to some other CSS framework. For now, these two libs are requried. See the `libman.json` file in the class library for the particular version in use. There are also some styles set up the file `styles.css` in class library.


```json
{
  
  {
  "version": "1.0",
  "defaultProvider": "cdnjs",
  "libraries": [
    {
      "library": "bootstrap@5.3.3",
      "destination": "wwwroot/bootstrap/"
    },
    {
      "library": "font-awesome@6.5.1",
      "destination": "wwwroot/font-awesome/"
    }
  ]
}

}

``` 

The component supports both inspect an object and the properties and also edit the properties. 

The supported data types of properties that can be inspected and edited are the fundamental data types :

* DateTime (both DateTime and DateTime showing only Date)
* Float
* Decimal
* Int
* Double
* String
* Bool
* Enums (will be using SELECT / OPTION(s))

In case you have wishes for forking the repo, do so freely. This component is MIT license.
 

## Using the component - API 

The following razor markup shows the component in use. Please note that the callback logic
shown here in the <em>PropertySetValueCallback</em> is only necessary if it is required to 
update visually the change that is done with the PropertyGridComponent, if a property has been edited and updated.

```xml

  <div class="col-md-6"> <!-- Proprety grid is shown in the right column-->
        <h4 class="mb-3">Property Grid Component Demo</h4>

        <PropertyGridComponent 
            PropertySetValueCallback="OnPropertyValueSet" 
            ObjectTitle="Property grid - Edit form for : 'Customer Details'" 
            DataContext="@exampleModel">
        </PropertyGridComponent>

    </div>



@code {
    private void OnPropertyValueSet(PropertyChangedInfoNotificationInfoPayload pi){
        if(pi != null){
            JsRunTime.InvokeVoidAsync("updateEditableField", pi.FieldName, pi.FullPropertyPath, pi.Value); 
        }
    }

    private CustomerModel exampleModel = new CustomerModel
    {
        Address = new AddressInfo
        {
            Zipcode = 7045,
            AddressDetails = new AddressInfoDetails
            {
                Box = "PO Box 123"
            }
        }
    };

    private void HandleValidSubmit()
    {

    }

}


```
You will find the Javascript method _updateEditableField_ in the `script.js` file in the class library.

It is also possible and suggested to instead do the updating using C#. The code shown will describe how to reflect the change if you can see the object the PropertyGrid inspects&instead elsewhere on the page of the Blazor app.

An example is shown below of such a callback function and it can be extracted to a helper class since it will not change much. Remember to also do the call _StateHasChanged()_ :

```csharp

@code {

    private void OnPropertyValueSet(PropertyChangedInfoNotificationInfoPayload pi)
    {
        if (pi == null)
            return;

        // Update the model property directly via reflection so Blazor re-renders correctly
        SetPropertyByPath(exampleModel, pi.FullPropertyPath, pi.Value, pi.ValueType);
        StateHasChanged();
    }

    private void SetPropertyByPath(object target, string propertyPath, object value, string valueType)
    {
        if (target == null || string.IsNullOrEmpty(propertyPath))
            return;

        var parts = propertyPath.Split('.');
        var current = target;

        // Navigate to the parent object
        for (int i = 0; i < parts.Length - 1; i++)
        {
            var prop = current.GetType().GetProperty(parts[i]);
            if (prop == null) return;
            current = prop.GetValue(current);
            if (current == null) return;
        }

        // Set the final property
        var finalProp = current.GetType().GetProperty(parts[^1]);
        if (finalProp == null) return;

        try
        {
            object convertedValue = value;

            if (finalProp.PropertyType.IsEnum && value != null)
            {
                var valStr = value.ToString();
                if (int.TryParse(valStr, out int intVal))
                    convertedValue = Enum.ToObject(finalProp.PropertyType, intVal);
                else
                    convertedValue = Enum.Parse(finalProp.PropertyType, valStr, ignoreCase: true);
            }
            else if (finalProp.PropertyType == typeof(bool) && value != null)
            {
                convertedValue = Convert.ToBoolean(value);
            }
            else if (value != null && finalProp.PropertyType != typeof(string))
            {
                convertedValue = Convert.ChangeType(value, finalProp.PropertyType);
            }

            finalProp.SetValue(current, convertedValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to set {propertyPath}: {ex.Message}");
        }
    }

}


```

The following properties must be set on the component:
* DataContext - set this to the object the property grid component should display and optionally edit.

The following properties should be set on the component:
* ObjectTitle - the name of the object. 

And the following should be set if editing is desired AND you want to reflect the edited changes back again elsewhere on the page visually (the properties edited are already updated of course):

* PropertySetValueCallback - delegate method to handle editing. 
The method must accept one parameter of type 
*PropertyChangedInfoNotificationInfoPayload*


## Pull requests / suggestions ? 
If you have suggestions or pull requests, please contact. This component provides a free basic property grid for use in Blazor community. It has no warranty and comes with a standard MIT license.


Last update: 14th February, 2026 . (v12)

Tore Aurstad
