using Core.Attributes;

namespace Core.Commands.Math
{
    [Description("Returns the square root of the specified number.")]
    public class SqrtCommand : MathOneArgumentCommand
    {
        public SqrtCommand() : base("sqrt")
        { }

        protected override (string, double) Calculate(double operand) => (null, System.Math.Sqrt(operand));
    }
}