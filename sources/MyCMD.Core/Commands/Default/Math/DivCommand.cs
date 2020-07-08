using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Commands.Default.Math.Base;

namespace Vardirsoft.MyCmd.Core.Commands.Default.Math
{
    [AutoRegister]
    [Description("Divides the given numeric arguments.")]
    public class DivCommand : MathTwoArgumentsCommand
    {
        public DivCommand() : base("div") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute() => rightOperand == 0.0 ? Error("zero-division exception") : ExecutionResult.Success(leftOperand / rightOperand);
    }
}