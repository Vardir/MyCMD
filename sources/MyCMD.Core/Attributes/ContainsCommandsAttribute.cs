using System;

namespace Vardirsoft.MyCmd.Core.Attributes
{
    /// <summary>
    /// An assembly-level attribute to specify assembly with commands to automatically registrate
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class ContainsCommandsAttribute : Attribute
    {
        
    }
}