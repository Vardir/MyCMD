namespace Core.Commands.Math
{
    public class DivCommand : MathTwoArgumentsCommand
    {
        public DivCommand() : base("div",
                                   "Devides the given arguments if they are numbers.",
                                   "div <arg1> <arg2>\n\targ1 -- the left-side value\n\targ2 -- the right-side value")
        { }

        protected override (string, double) Calculate(double left, double right)
        {
            if (right == 0.0)
                return ("div.error: zero-division exception", 0.0);

            return (null, left / right);
        }
    }
}