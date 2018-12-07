namespace Core.Commands.Math
{
    public class SqrtCommand : MathOneArgumentCommand
    {
        public SqrtCommand() : base("sqrt",
                                    "Returns the square root of the specified number.",
                                    "sqrt <arg>\n\targ -- the number")
        { }

        protected override (string, double) Calculate(double operand) => (null, System.Math.Sqrt(operand));
    }
}