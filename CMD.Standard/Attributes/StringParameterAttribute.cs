using System;

namespace Core.Attributes
{
    public sealed class StringParameterAttribute : ParameterAttribute
    {
        public string Default { get; }

        public StringParameterAttribute(string defaultValue = "")
        {
            Default = defaultValue;
        }

        public override bool IsAllowedType(Type type) => type == typeof(string);
        public override object GetDefaultValue() => Default;
    }
}