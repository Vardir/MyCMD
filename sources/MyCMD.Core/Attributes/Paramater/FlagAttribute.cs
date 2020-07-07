using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Paramater
{
    /// <summary>
    /// An attribute to mark field as flag for a command
    /// </summary>
    public sealed class FlagAttribute : ParameterAttribute
    {
        public FlagAttribute()
        {
            IsOptional = true;
        }

        /// <summary>
        /// Verifies if the given type is valid to set parameter's value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsAllowedType(Type type) => type == typeof(bool);

        /// <summary>
        /// Gets the default value of the parameter
        /// </summary>
        /// <returns></returns>
        public override object GetDefaultValue() => false;
    }
}