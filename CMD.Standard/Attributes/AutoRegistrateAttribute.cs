using System;

namespace Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AutoRegistrateAttribute : Attribute
    {
        public AutoRegistrateAttribute() { }
    }
}