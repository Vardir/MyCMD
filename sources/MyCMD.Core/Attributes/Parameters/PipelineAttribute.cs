using System;

namespace Vardirsoft.MyCmd.Core.Attributes.Parameters
{
    /// <summary>
    /// An attribute to mark field as parameter that accepts a pipelined value to a command
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PipelineAttribute : Attribute
    {

    }    
}