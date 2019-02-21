using System.Text.RegularExpressions;

namespace Core.Attributes
{
    /// <summary>
    /// An attribute to specify validation rules for a string parameter
    /// </summary>
    public sealed class StringValidationAttribute : ParameterValidationAttribute
    {
        /// <summary>
        /// A flag to specify whether the parameter can contain null/empty string
        /// </summary>
        public bool AllowNullOrEmpty { get; }
        /// <summary>
        /// Minimum length allowed for string
        /// </summary>
        public int MinLength { get; }
        /// <summary>
        /// Maximum length allowed for string
        /// </summary>
        public int MaxLength { get; }
        /// <summary>
        /// Regular expression pattern to validate a string
        /// </summary>
        public string RegexPattern { get; }

        public StringValidationAttribute(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
        public StringValidationAttribute(int minLength, int maxLength, string regexPattern)
            : this(minLength, maxLength)
        {
            RegexPattern = regexPattern;
        }

        /// <summary>
        /// Validates a value for attached parameter, returns null if no errors found
        /// </summary>
        /// <param name="value">A value to validate</param>
        /// <returns></returns>
        public override string Validate(object value)
        {
            string str = value as string;
            if (str == null && value != null)
                return $"invalid value type, expected [System.String] but got [{value.GetType()}]";
            if (string.IsNullOrEmpty(str))
            {
                if (!AllowNullOrEmpty)
                    return "null/empty string is not allowed";
                return null;
            }
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