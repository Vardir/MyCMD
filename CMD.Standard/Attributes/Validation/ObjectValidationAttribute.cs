namespace Core.Attributes
{
    public sealed class ObjectValidationAttribute : ParameterValidationAttribute
    {
        public bool AllowNulls { get; }

        public ObjectValidationAttribute(bool allowNulls)
        {
            AllowNulls = allowNulls;
        }

        public override string Validate(object value)
        {
            if (value == null && !AllowNulls)
                return "null values are not allowed";
            return null;
        }
    }
}