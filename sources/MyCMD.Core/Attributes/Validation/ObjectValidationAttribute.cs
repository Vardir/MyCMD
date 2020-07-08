using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Validation
{
    /// <summary>
    /// An attribute to specify validation rules for an object parameter
    /// </summary>
    public sealed class ObjectValidationAttribute : ParameterValidationAttribute
    {
        /// <summary>
        /// A flag to specify whether the parameter can contain null value (default: true)
        /// </summary>
        public bool AllowNulls { get; set; }
        
        /// <summary>
        /// A restriction of value type allowed for the parameter
        /// </summary>
        public Type TypeRestriction { get; }

        public ObjectValidationAttribute()
        {
            AllowNulls = true;
            TypeRestriction = null;
        }
        
        public ObjectValidationAttribute(Type restriction)
        {
            AllowNulls = true;
            TypeRestriction = restriction;
        }

        /// <summary>
        /// Validates a value for attached parameter, returns null if no errors found
        /// </summary>
        /// <param name="value">A value to validate</param>
        /// <returns></returns>
        public override string Validate(object value)
        {
            if (value is null && !AllowNulls)
                return "null values are not allowed";
            
            if (TypeRestriction != null && value?.GetType() != TypeRestriction)
                return "object was of invalid type";
            
            return null;
        }
    }
}