using System;

namespace Vardirsoft.MyCmd.Core.Attributes
{
    /// <summary>
    /// An attribute indicates the command is to be automatically registered in execution service
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AutoRegisterAttribute : Attribute
    {
        
    }
}