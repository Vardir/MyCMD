using System;

namespace Core.Attributes
{
    /// <summary>
    /// An attribute to mark field as array parameter of a command
    /// </summary>
    public sealed class ArrayParameterAttribute : ParameterAttribute
    {
        /// <summary>
        /// The default value of the parameter
        /// </summary>
        public object[] Default { get; }

        public ArrayParameterAttribute(object[] defaultValue = null)
        {
            Default = defaultValue;
        }

        /// <summary>
        /// Verifies if the given type is valid to set parameter's value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsAllowedType(Type type) => type == typeof(object[]);

        /// <summary>
        /// Gets the default value of the parameter
        /// </summary>
        /// <returns></returns>
        public override object GetDefaultValue() => Default;
    }
}