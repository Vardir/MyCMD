using Core.Attributes;

namespace Core.Commands.Math
{
    public abstract class MathOneArgumentCommand : Command
    {
        [Pipeline]
        [NumberParameter]
        [Description("The operand to calculate on")]
        protected double operand;

        public MathOneArgumentCommand(string id) : base(id) { }
    }
}