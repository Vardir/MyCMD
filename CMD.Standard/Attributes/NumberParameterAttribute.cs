using System;

namespace Core.Attributes
{
    public sealed class NumberParameterAttribute : ParameterAttribute
    {
        public double Default { get; }

        public NumberParameterAttribute(double defaultValue = 0)
        {
            Default = defaultValue;
        }

        public override bool IsAllowedType(Type type) => type == typeof(double);
        public override object GetDefaultValue() => Default;
    }
}