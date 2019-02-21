using Core.Attributes;

namespace Core.Commands.Math
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