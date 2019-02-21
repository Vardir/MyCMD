using Core.Attributes;

namespace Core.Commands.Math
{
    [AutoRegistrate]
    [Description("Sums the given arguments if they are numbers.")]
    public class SumCommand : MathTwoArgumentsCommand
    {
        public SumCommand() : base("sum"){}

        protected override ExecutionResult Execute() => ExecutionResult.Success(left + right);
    }
}