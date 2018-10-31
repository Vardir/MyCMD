namespace Core.Commands.Math
{
    public class SqrtCommand : MathOneArgumentCommand
    {
        public SqrtCommand() : base("sqrt",
                                    "Returns the square root of the specified number.",
                                    "sqrt <arg1> <arg2>\n\arg1 -- the left-side value\n\targ2 -- the right-side value")
        { }

        protected override (string, double) Calculate(double operand) => (null, System.Math.Sqrt(operand));
    }
}