using System;

namespace Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DescriptionAttribute : Attribute
    {
        public string Value { get; }

        public DescriptionAttribute(string value)
        {
            Value = value;
        }
    }
}