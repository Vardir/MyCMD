using System;

namespace Core.Attributes
{
    public sealed class BoolParameterAttribute : ParameterAttribute
    {
        public bool Default { get; }

        public BoolParameterAttribute(bool defaultValue = false)
        {
            Default = defaultValue;
        }

        public override bool IsAllowedType(Type type) => type == typeof(bool);
        public override object GetDefaultValue() => Default;
    }
}