using System;

namespace Core.Attributes
{
    public sealed class ObjectParameterAttribute : ParameterAttribute
    {
        public object Default { get; }

        public ObjectParameterAttribute(object defaultValue = null)
        {
            Default = defaultValue;
        }

        public override bool IsAllowedType(Type type) => true;
        public override object GetDefaultValue() => Default;
    }
}