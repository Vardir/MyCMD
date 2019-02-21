using System;

namespace Core.Attributes
{
    public interface IParameterValidation
    {
        string Validate(object value);
    }
    public abstract class ParameterValidationAttribute : Attribute, IParameterValidation
    {
       public abstract string Validate(object value);
    }
}