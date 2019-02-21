using System;

namespace Core.Attributes
{
    /// <summary>
    /// An attribute to specify validation rules for a numeric-based parameter
    /// </summary>
    public sealed class NumberValidationAttribute : ParameterValidationAttribute
    {
        /// <summary>
        /// Minimum value allowed for parameter
        /// </summary>
        public double MinValue { get; }
        /// <summary>
        /// Maximum value allowed for parameter
        /// </summary>
        public double MaxValue { get; }

        public NumberValidationAttribute(double minValue, double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /// <summary>
        /// Validates a value for attached parameter, returns null if no errors found
        /// </summary>
        /// <param name="value">A value to validate</param>
        /// <returns></returns>
        public override string Validate(object value)
        {
            if (value == null)
                return "number can not be null";
            double d;
            try
            {
                d = Convert.ToDouble(value);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            if (d < MinValue)
                return "value is lesser than minimum";
            if (d > MaxValue)
                return "value is greater than maximum";
            return null;
        }
    }
}