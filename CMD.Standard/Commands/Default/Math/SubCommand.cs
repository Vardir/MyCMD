using Core.Attributes;

namespace Core.Commands.Math
{
    [Description("Subtract the given arguments if they are numbers.")]
    public class SubCommand : MathTwoArgumentsCommand
    {
        public SubCommand() : base("sub")
        { }

        protected override (string, double) Calculate(double left, double right) => (null, left - right);
    }
}