using System;

namespace Vardirsoft.MyCmd.Core.Attributes
{
    /// <summary>
    /// An assembly-level attribute to specify assembly with commands to be automatically registered
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ContainsCommandsAttribute : Attribute
    {
        
    }
}