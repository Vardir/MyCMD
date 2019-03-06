using System;
using Core.Attributes;
using System.Reflection;

namespace Core.Commands
{
    /// <summary>
    /// A generalized class to represent a command parameter
    /// </summary>
    public class Parameter
    {
        private readonly Command container;
        private readonly FieldInfo backingField;

        /// <summary>
        /// Indicates that parameter is flag
        /// </summary>
        public bool IsFlag { get; }
        /// <summary>
        /// Indicates that parameter is optional and can be omitted while command is executed
        /// </summary>
        public bool IsOptional { get; }
        /// <summary>
        /// Indicates that parameter value is set
        /// </summary>
        public bool IsSet { get; private set; }
        /// <summary>
        /// Indicates that parameter receives pipelined value
        /// </summary>
        public bool IsPipelined { get; internal set; }
        /// <summary>
        /// A default value for the parameter
        /// </summary>
        public object DefaultValue { get; }
        /// <summary>
        /// The ID of the parameter
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// The description of the parameter
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// A validation that verifies value before setting parameter
        /// </summary>
        public IParameterValidation Validation { get; }

        private Parameter(string id, string description, bool isOptional, IParameterValidation validation)
        {
            Id = id;
            IsOptional = isOptional;
            Description = description;
            Validation = validation;
        }
        
        public Parameter(string id, string description, bool isFlag, 
                         Command container, FieldInfo backingField, IParameterValidation validation)
            : this(id, description, false, validation)
        {
            IsOptional = isFlag;
            IsFlag = isFlag;
            this.container = container;
            this.backingField = backingField;
        }
        public Parameter(string id, string description, object defaultValue, 
                         Command container, FieldInfo backingField, IParameterValidation validation)
            : this(id, description, true, validation)
        {
            IsOptional = true;
            this.container = container;
            if (validation != null)
            {
                string error = validation.Validate(defaultValue);
                if (error != null)
                    throw new ArgumentException(error, nameof(defaultValue));
            }
            DefaultValue = defaultValue;
            this.backingField = backingField;
        }

        /// <summary>
        /// Clears out value from the parameter
        /// </summary>
        public void Unset()
        {
            IsSet = false;
            if (IsFlag)
                backingField.SetValue(container, false);
            else if (IsOptional)
                backingField.SetValue(container, DefaultValue);
        }
        /// <summary>
        /// Sets a default value to parameter if parameter is optional/flag
        /// </summary>
        public void Set()
        {
            if (!IsOptional && !IsFlag)
                throw new InvalidOperationException("can not set parameter without default value");
            IsSet = true;
            if (IsFlag)
                backingField.SetValue(container, true);
            else
                backingField.SetValue(container, DefaultValue);
        }
        /// <summary>
        /// Stores the given value to the parameter
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(object value)
        {
            if (IsFlag)
                throw new InvalidOperationException("can no set flag to a custom value");
            IsSet = true;
            backingField.SetValue(container, value);
        }

        /// <summary>
        /// Verifies if the given value can be stored in the parameter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CanAssign(object value) => backingField.FieldType == value.GetType();
        /// <summary>
        /// Gets type of the internal storage of the parameter
        /// </summary>
        /// <returns></returns>
        public Type GetValueType() => backingField.FieldType;
    }
}