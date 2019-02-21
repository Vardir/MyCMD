using System;

namespace Core.Attributes
{
    public sealed class NumberValidationAttribute : ParameterValidationAttribute
    {
        public double MinValue { get; }
        public double MaxValue { get; }

        public NumberValidationAttribute(double minValue, double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

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