using System;

namespace Vardirsoft.MyCmd.Core.Attributes
{
    /// <summary>
    /// An attribute attached to entire command and it's parameters to provide description 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public sealed class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Description value
        /// </summary>
        public string Value { get; }

        public DescriptionAttribute(string value)
        {
            Value = value;
        }
    }
}