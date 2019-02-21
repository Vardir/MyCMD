using Core.Attributes;

namespace Core.Commands.Math
{
    [AutoRegistrate]
    [Description("Multiplies the given arguments if they are numbers.")]
    public class MulCommand : MathTwoArgumentsCommand
    {
        public MulCommand() : base("mul") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute() => ExecutionResult.Success(left * right);
    }
}