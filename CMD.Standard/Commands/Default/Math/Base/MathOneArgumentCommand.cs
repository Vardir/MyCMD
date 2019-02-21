using Core.Attributes;

namespace Core.Commands.Math
{
    /// <summary>
    /// A base class for math commands with one double-precision numeric parameter
    /// </summary>
    public abstract class MathOneArgumentCommand : Command
    {
        [Pipeline]
        [NumberParameter]
        [Description("The operand to calculate on")]
        protected double operand;

        public MathOneArgumentCommand(string id) : base(id) { }
    }
}