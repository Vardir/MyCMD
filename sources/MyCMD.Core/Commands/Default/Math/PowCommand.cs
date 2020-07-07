using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Commands.Default.Math.Base;

namespace Vardirsoft.MyCmd.Core.Commands.Default.Math
{
    [AutoRegistrate]
    [Description("Returns a specified number raised the specified power.")]
    public class PowCommand : MathTwoArgumentsCommand
    {
        public PowCommand() : base("pow") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute() => ExecutionResult.Success(System.Math.Pow(left, right));
    }
}