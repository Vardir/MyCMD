namespace Core.Commands.Math
{
    public class MulCommand : MathTwoArgumentsCommand
    {
        public MulCommand() : base("mul",
                                   "Multiplies the given arguments if they are numbers.",
                                   "mul <arg1> <arg2>\n\arg1 -- the left-side value\n\targ2 -- the right-side value")
        { }

        protected override (string, double) Calculate(double left, double right) => (null, left * right);
    }
}