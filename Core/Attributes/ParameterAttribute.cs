using System;

namespace Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class ParameterAttribute : Attribute
    {
        public bool IsOptional { get; set; }
        public bool HasDefault { get; set; }
        public string Key { get; set; }

        public ParameterAttribute()
        {
            
        }

        public abstract bool IsAllowedType(Type type);
        public abstract object GetDefaultValue();
    }
}