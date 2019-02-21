namespace Core.Attributes
{
    /// <summary>
    /// An attribute to specify validation rules for an object parameter
    /// </summary>
    public sealed class ObjectValidationAttribute : ParameterValidationAttribute
    {
        /// <summary>
        /// A flag to specify whether the parameter can contain null value
        /// </summary>
        public bool AllowNulls { get; }

        public ObjectValidationAttribute(bool allowNulls)
        {
            AllowNulls = allowNulls;
        }

        /// <summary>
        /// Validates a value for attached parameter, returns null if no errors found
        /// </summary>
        /// <param name="value">A value to validate</param>
        /// <returns></returns>
        public override string Validate(object value)
        {
            if (value == null && !AllowNulls)
                return "null values are not allowed";
            return null;
        }
    }
}