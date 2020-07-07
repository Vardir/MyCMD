using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Validation
{
    /// <summary>
    /// An interface to implement parameter validation routine
    /// </summary>
    public interface IParameterValidation
    {
        /// <summary>
        /// Validates a value for attached parameter, returns null if no errors found
        /// </summary>
        /// <param name="value">A value to validate</param>
        /// <returns></returns>
        string Validate(object value);
    }

    /// <summary>
    /// Base class for all parameter validation types
    /// </summary>
    public abstract class ParameterValidationAttribute : Attribute, IParameterValidation
    {
        /// <summary>
        /// Validates a value for attached parameter, returns null if no errors found
        /// </summary>
        /// <param name="value">A value to validate</param>
        /// <returns></returns>
        public abstract string Validate(object value);
    }
}