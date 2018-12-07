namespace Core.Commands.Math
{
    public class PowCommand : MathTwoArgumentsCommand
    {
        public PowCommand() : base("pow",
                                   "Returns a specified number raised the specified power.",
                                   "pow <arg1> <arg2>\n\targ1 -- the number to power\n\targ2 -- the power")
        { }

        protected override (string, double) Calculate(double left, double right) => (null, System.Math.Pow(left, right));
    }
}