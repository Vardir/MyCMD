using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Parameters
{
    /// <summary>
    /// An attribute to mark field as object parameter of a command
    /// </summary>
    public sealed class ObjectParameterAttribute : ParameterAttribute
    {
        /// <summary>
        /// The default value of the parameter
        /// </summary>
        public object Default { get; }

        public ObjectParameterAttribute(object defaultValue = null)
        {
            Default = defaultValue;
        }

        /// <summary>
        /// Verifies if the given type is valid to set parameter's value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsAllowedType(Type type) => true;

        /// <summary>
        /// Gets the default value of the parameter
        /// </summary>
        /// <returns></returns>
        public override object GetDefaultValue() => Default;
    }
}