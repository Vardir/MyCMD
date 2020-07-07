using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Paramater
{
    /// <summary>
    /// An attribute to mark field as parameter of a command
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class ParameterAttribute : Attribute
    {
        /// <summary>
        /// Specifies if the parameter has to be optional so that it can be omitted while using command 
        /// </summary>
        public bool IsOptional { get; set; }
        /// <summary>
        /// The ID of the parameter used to find it
        /// </summary>
        public string Key { get; set; }

        public ParameterAttribute()
        {
            
        }

        /// <summary>
        /// Verifies if the given type is valid to set parameter's value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool IsAllowedType(Type type);

        /// <summary>
        /// Gets the default value of the parameter
        /// </summary>
        /// <returns></returns>
        public abstract object GetDefaultValue();
    }
}