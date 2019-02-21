using System.Text.RegularExpressions;

namespace Core.Attributes
{
    public sealed class StringValidationAttribute : ParameterValidationAttribute
    {
        public bool AllowNullOrEmpty { get; }
        public int MinLength { get; }
        public int MaxLength { get; }
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