using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Commands.Default.Math.Base;

namespace Vardirsoft.MyCmd.Core.Commands.Default.Math
{
    [AutoRegistrate]
    [Description("Sums the given arguments if they are numbers.")]
    public class SumCommand : MathTwoArgumentsCommand
    {
        public SumCommand() : base("sum"){}

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute() => ExecutionResult.Success(left + right);
    }
}