using System.Text.RegularExpressions;

namespace Vardirsoft.MyCmd.Core.Attributes.Validation
{
    /// <summary>
    /// An attribute to specify validation rules for a string parameter
    /// </summary>
    public sealed class StringValidationAttribute : ParameterValidationAttribute
    {
        /// <summary>
        /// A flag to specify whether the parameter can contain null/empty string (default: true)
        /// </summary>
        public bool AllowNullOrEmpty { get; set; }
        
        /// <summary>
        /// Minimum length allowed for string (default: 0)
        /// </summary>
        public int MinLength { get; set; }
        
        /// <summary>
        /// Maximum length allowed for string (default: 2 147 483 647)
        /// </summary>
        public int MaxLength { get; set; }
        
        /// <summary>
        /// Regular expression pattern to validate a string (default: null)
        /// </summary>
        public string RegexPattern { get; set; }

        public StringValidationAttribute()
        {
            AllowNullOrEmpty = true;
            MinLength = 0;
            MaxLength = int.MaxValue;
            RegexPattern = null;
        }

        /// <summary>
        /// Validates a value for attached parameter, returns null if no errors found
        /// </summary>
        /// <param name="value">A value to validate</param>
        /// <returns></returns>
        public override string Validate(object value)
        {
            var str = value as string;
            if (str is null && value != null)
                return $"invalid value type, expected [System.String] but got [{value.GetType()}]";
            
            if (string.IsNullOrEmpty(str))
                return !AllowNullOrEmpty ? "null/empty string is not allowed" : null;
            
            if (str.Length < MinLength)
                return "string length is lesser than minimum";
            
            if (str.Length > MaxLength)
                return "string length is greater than maximum";
            
            if (RegexPattern != null && !Regex.IsMatch(str, RegexPattern))
                return "string does not match pattern";
            
            return null;
        }
    }
}