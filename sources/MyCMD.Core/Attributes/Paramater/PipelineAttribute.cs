using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Paramater
{
    /// <summary>
    /// An attribute to mark field as parameter that accepts a pipelined value to a command
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PipelineAttribute : Attribute
    {

    }    
}