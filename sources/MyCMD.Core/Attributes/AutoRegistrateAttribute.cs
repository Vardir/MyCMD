using System;

namespace Vardirsoft.MyCmd.Core.Attributes
{
    /// <summary>
    /// An attribute indicates the command to automatically registrate in execution service
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AutoRegistrateAttribute : Attribute
    {
        public AutoRegistrateAttribute() { }
    }
}