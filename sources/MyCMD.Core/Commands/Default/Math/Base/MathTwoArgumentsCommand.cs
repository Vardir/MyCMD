using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Attributes.Parameters;

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
        protected double leftOperand;

        [NumberParameter]
        [Description("The right-side parameter")]
        protected double rightOperand;

        public MathTwoArgumentsCommand(string id) : base(id) { }
    }
}