namespace BlazorPropertyGridComponents.Components
{
    public class PropertyChangedInfoNotificationInfoPayload
    {
        public string FullPropertyPath { get; set; }
        public string FieldName { get; set; }
        public object Value { get; set; }
    }
}
