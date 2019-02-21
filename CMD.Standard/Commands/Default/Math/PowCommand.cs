using Core.Attributes;

namespace Core.Commands.Math
{
    [AutoRegistrate]
    [Description("Returns a specified number raised the specified power.")]
    public class PowCommand : MathTwoArgumentsCommand
    {
        public PowCommand() : base("pow") { }

        protected override ExecutionResult Execute() => ExecutionResult.Success(System.Math.Pow(left, right));
    }
}