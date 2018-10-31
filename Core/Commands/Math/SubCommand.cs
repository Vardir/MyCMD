namespace Core.Commands.Math
{
    public class SubCommand : MathTwoArgumentsCommand
    {
        public SubCommand() : base("sub",
                                   "Subtract the given arguments if they are numbers.",
                                   "sub <arg1> <arg2>\n\arg1 -- the left-side value\n\targ2 -- the right-side value")
        { }

        protected override (string, double) Calculate(double left, double right) => (null, left - right);
    }
}