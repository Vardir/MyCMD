using Core.Attributes;

namespace Core.Commands.Math
{
    [Description("Returns a specified number raised the specified power.")]
    public class PowCommand : MathTwoArgumentsCommand
    {
        public PowCommand() : base("pow")
        { }

        protected override (string, double) Calculate(double left, double right) => (null, System.Math.Pow(left, right));
    }
}