using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Validation
{
    /// <summary>
    /// An attribute to specify validation rules for an array parameter
    /// </summary>
    public sealed class ArrayValidationAttribute : ParameterValidationAttribute
    {
        /// <summary>
        /// A flag to specify whether array can contain null values or not (default: true)
        /// </summary>
        public bool AllowNullValues { get; set; }
        /// <summary>
        /// A flag to specify whether parameter can contain null reference (default: false)
        /// </summary>
        public bool AllowArrayNullReference { get; set; }
        /// <summary>
        /// Minimum length of the array (default: 0)
        /// </summary>
        public int MinLength { get; set; }
        /// <summary>
        /// Maximum length of the array (default: 2 147 483 647)
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// Restricts type of values that can be contained in the array (default: null)
        /// </summary>
        public Type ValueType { get; set; }

        public ArrayValidationAttribute()
        {
            MinLength = 0;
            MaxLength = int.MaxValue;
            ValueType = null;
            AllowNullValues = true;
            AllowArrayNullReference = false;
        }

        /// <summary>
        /// Validates a value for attached parameter, returns null if no errors found
        /// </summary>
        /// <param name="value">A value to validate</param>
        /// <returns></returns>
        public override string Validate(object obj)
        {
            if (!AllowArrayNullReference && obj == null)
                return "array null reference is not allowed";
            if (obj == null)
                return null;
            if (!(obj is object[] array))
                return $"expected array but [{obj.GetType()}] given";
            if (array.Length < MinLength)
                return $"minimal array length is {MinLength} but got {array.Length}";
            if (array.Length > MaxLength)
                return $"maximal array length is {MaxLength} but got {array.Length}";
            if (ValueType != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    Type valueType = array[i]?.GetType();
                    if (valueType == null && !AllowNullValues)
                        return "null values are not allowed in the array";
                    if (valueType == null)
                        continue;
                    if (valueType != ValueType)
                        return $"expected values' type [{ValueType}] but got [{valueType}]";
                }
            }
            return null;
        }
    }
}