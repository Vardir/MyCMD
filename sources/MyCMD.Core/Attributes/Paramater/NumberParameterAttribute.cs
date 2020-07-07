using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Paramater
{
    /// <summary>
    /// An attribute to mark field as numeric-based parameter of a command
    /// </summary>
    public sealed class NumberParameterAttribute : ParameterAttribute
    {
        /// <summary>
        /// The default value of the parameter
        /// </summary>
        public double Default { get; }

        public NumberParameterAttribute(double defaultValue = 0)
        {
            Default = defaultValue;
        }

        /// <summary>
        /// Verifies if the given type is valid to set parameter's value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsAllowedType(Type type) => type == typeof(double);

        /// <summary>
        /// Gets the default value of the parameter
        /// </summary>
        /// <returns></returns>
        public override object GetDefaultValue() => Default;
    }
}