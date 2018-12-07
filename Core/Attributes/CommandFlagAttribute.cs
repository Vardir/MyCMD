using System;

namespace Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CommandFlagAttribute : Attribute
    {
        public string Id { get; }
        public string Description { get; }

        public CommandFlagAttribute(string id, string description)
        {
            Id = id;
            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class CommandFlagsAttribute : Attribute
    {
    }
}