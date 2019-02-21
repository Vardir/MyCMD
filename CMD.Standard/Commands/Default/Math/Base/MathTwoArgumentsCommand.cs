using Core.Attributes;

namespace Core.Commands.Math
{
    public abstract class MathTwoArgumentsCommand : Command
    {
        [Pipeline]
        [NumberParameter]
        [Description("The left-side parameter")]
        protected double left;

        [NumberParameter]
        [Description("The right-side parameter")]
        protected double right;

        public MathTwoArgumentsCommand(string id) : base(id) { }
    }
}