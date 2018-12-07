using Core.Attributes;

namespace Core.Commands.Math
{
    [Description("Divides the given arguments if they are numbers.")]
    public class DivCommand : MathTwoArgumentsCommand
    {
        public DivCommand() : base("div")
        { }

        protected override (string, double) Calculate(double left, double right)
        {
            if (right == 0.0)
                return ("div.error: zero-division exception", 0.0);

            return (null, left / right);
        }
    }
}