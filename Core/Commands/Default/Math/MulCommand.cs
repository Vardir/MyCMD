using Core.Attributes;

namespace Core.Commands.Math
{
    [Description("Multiplies the given arguments if they are numbers.")]
    public class MulCommand : MathTwoArgumentsCommand
    {
        public MulCommand() : base("mul")
        { }

        protected override (string, double) Calculate(double left, double right) => (null, left * right);
    }
}