using Core.Attributes;

namespace Core.Commands.Math
{
    [Description("Sums the given arguments if they are numbers.")]
    public class SumCommand : MathTwoArgumentsCommand
    {
        public SumCommand() : base("sum"){}

        protected override (string, double) Calculate(double left, double right) => (null, left + right);
    }
}