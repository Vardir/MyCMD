using System;

namespace Core.Attributes
{
    public sealed class FlagAttribute : ParameterAttribute
    {
        public FlagAttribute()
        {
            IsOptional = true;
        }

        public override bool IsAllowedType(Type type) => type == typeof(bool);
        public override object GetDefaultValue() => false;
    }
}