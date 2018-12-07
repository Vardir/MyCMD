using System;
using System.Reflection;

namespace Core.Commands
{
    public class Parameter
    {
        private readonly Command container;
        private readonly FieldInfo backingField;

        public bool IsFlag { get; }
        public bool IsOptional { get; }
        public bool HasDefault { get; }
        public bool IsSet { get; private set; }
        public object DefaultValue { get; }
        public string Id { get; }
        public string Description { get; }

        private Parameter(string id, string description, bool isOptional)
        {
            Id = id;
            IsOptional = isOptional;
            Description = description;
        }

        public Parameter(string id, string description)
            : this(id, description, true)
        {
            IsFlag = true;
        }
        public Parameter(string id, string description, Command container, FieldInfo backingField)
            : this(id, description, false)
        {
            HasDefault = false;
            this.container = container;
            this.backingField = backingField;
        }
        public Parameter(string id, string description, bool isOptional, object defaultValue, Command container, FieldInfo backingField)
            : this(id, description, isOptional)
        {
            HasDefault = true;
            this.container = container;
            DefaultValue = defaultValue;
            this.backingField = backingField;
        }

        public void Unset()
        {
            IsSet = false;
            if (HasDefault)
                backingField.SetValue(container, DefaultValue);
        }
        public void Set()
        {
            if (!HasDefault && !IsFlag)
                throw new InvalidOperationException("can not set parameter without default value");
            IsSet = true;
            backingField?.SetValue(container, DefaultValue);
        }
        public void SetValue(object value)
        {
            IsSet = true;
            backingField?.SetValue(container, value);
        }

        public bool CanAssign(object value) => backingField.FieldType == value.GetType();
        public Type GetValueType() => backingField.FieldType;
    }
}