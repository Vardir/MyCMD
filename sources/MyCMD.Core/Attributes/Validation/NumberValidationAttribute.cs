using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Validation
{
    /// <summary>
    /// An attribute to specify validation rules for a numeric-based parameter
    /// </summary>
    public sealed class NumberValidationAttribute : ParameterValidationAttribute
    {
        /// <summary>
        /// Minimum value allowed for parameter (default: ±5,0 × 10E−324)
        /// </summary>
        public double MinValue { get; set; }
        
        /// <summary>
        /// Maximum value allowed for parameter (default: ±1,7 × 10E308)
        /// </summary>
        public double MaxValue { get; set; }

        public NumberValidationAttribute()
        {
            MinValue = double.MinValue;
            MaxValue = double.MaxValue;
        }
        
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
            if (value is null)
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
            
            return d > MaxValue ? "value is greater than maximum" : null;
        }
    }
}