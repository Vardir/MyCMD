using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Commands.Default.Math.Base;

namespace Vardirsoft.MyCmd.Core.Commands.Default.Math
{
    [AutoRegister]
    [Description("Subtract the given arguments if they are numbers.")]
    public class SubCommand : MathTwoArgumentsCommand
    {
        public SubCommand() : base("sub") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute() => ExecutionResult.Success(leftOperand - rightOperand);
    }
}