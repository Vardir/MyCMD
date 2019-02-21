using System;

namespace Core.Attributes
{
    /// <summary>
    /// An attribute to specify validation rules for an array parameter
    /// </summary>
    public sealed class ArrayValidationAttribute : ParameterValidationAttribute
    {
        /// <summary>
        /// A flag to specify whether array can contain null values or not
        /// </summary>
        public bool AllowNullValues { get; }
        /// <summary>
        /// A flag to specify whether parameter can contain null reference
        /// </summary>
        public bool AllowArrayNullReference { get; }
        /// <summary>
        /// Minimum length of the array
        /// </summary>
        public int MinLength { get; }
        /// <summary>
        /// Maximum length of the array
        /// </summary>
        public int MaxLength { get; }
        /// <summary>
        /// Restricts type of values that can be contained in the array
        /// </summary>
        public Type ValueType { get; }

        public ArrayValidationAttribute(bool allowArrayNullReference, bool allowNullValues)
        {
            MinLength = 0;
            MaxLength = int.MaxValue;
            AllowNullValues = allowNullValues;
            AllowArrayNullReference = allowArrayNullReference;
        }
        public ArrayValidationAttribute(int minLength, int maxLength, bool allowArrayNullReference, bool allowNullValues)
            : this(allowArrayNullReference, allowNullValues)
        {
            if (minLength < 0)
                throw new ArgumentException("Minimum value can not be less than 0");
            if (minLength > maxLength)
                throw new ArgumentException("Minimum value can not be greater than maximum");
            MinLength = minLength;
            MaxLength = maxLength;
        }
        public ArrayValidationAttribute(int minLength, int maxLength, bool allowArrayNullReference, bool allowNullValues, Type arrayValueType)
            : this(minLength, maxLength, allowArrayNullReference, allowNullValues)
        {
            ValueType = arrayValueType;
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