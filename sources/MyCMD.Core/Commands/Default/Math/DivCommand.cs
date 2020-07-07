using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Commands.Default.Math.Base;

namespace Vardirsoft.MyCmd.Core.Commands.Default.Math
{
    [AutoRegistrate]
    [Description("Divides the given numeric arguments.")]
    public class DivCommand : MathTwoArgumentsCommand
    {
        public DivCommand() : base("div") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute()
        {
            if (right == 0.0)
                return Error("zero-division exception");

            return ExecutionResult.Success(left / right);
        }
    }
}