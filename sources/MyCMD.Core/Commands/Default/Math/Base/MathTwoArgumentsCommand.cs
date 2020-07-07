using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Attributes.Paramater;

namespace Vardirsoft.MyCmd.Core.Commands.Default.Math.Base
{
    /// <summary>
    /// A base class for math commands with two double-precision numeric parameters
    /// </summary>
    public abstract class MathTwoArgumentsCommand : Command
    {
        [Pipeline]
        [NumberParameter]
        [Description("The left-side parameter")]
        protected double left;

        [NumberParameter]
        [Description("The right-side parameter")]
        protected double right;

        public MathTwoArgumentsCommand(string id) : base(id) { }
    }
}