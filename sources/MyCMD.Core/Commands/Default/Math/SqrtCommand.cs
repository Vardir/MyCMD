using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Commands.Default.Math.Base;

namespace Vardirsoft.MyCmd.Core.Commands.Default.Math
{
    [AutoRegistrate]
    [Description("Returns the square root of the specified number.")]
    public class SqrtCommand : MathOneArgumentCommand
    {
        public SqrtCommand() : base("sqrt") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute() => ExecutionResult.Success(System.Math.Sqrt(operand));
    }
}