using System;

namespace Core.Attributes
{
    public sealed class ArrayParameterAttribute : ParameterAttribute
    {
        public object[] Default { get; }

        public ArrayParameterAttribute(object[] defaultValue = null)
        {
            Default = defaultValue;
        }

        public override bool IsAllowedType(Type type) => type == typeof(object[]);
        public override object GetDefaultValue() => Default;
    }
}