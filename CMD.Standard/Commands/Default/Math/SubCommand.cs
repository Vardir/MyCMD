using Core.Attributes;

namespace Core.Commands.Math
{
    [AutoRegistrate]
    [Description("Subtract the given arguments if they are numbers.")]
    public class SubCommand : MathTwoArgumentsCommand
    {
        public SubCommand() : base("sub") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute() => ExecutionResult.Success(left - right);
    }
}