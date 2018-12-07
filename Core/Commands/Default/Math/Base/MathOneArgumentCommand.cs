using Core.Attributes;

namespace Core.Commands.Math
{
    public abstract class MathOneArgumentCommand : MathCommand
    {
        [Pipeline]
        [NumberParameter(Key = "o")]
        [Description("Operand of the command")]
        protected double operand;

        public MathOneArgumentCommand(string id) : base(id)
        { }

        protected override ExecutionResult Execute()
        {
            var (error, result) = Calculate(operand);
            if (!string.IsNullOrEmpty(error))
                return ExecutionResult.Error(error);
            return ExecutionResult.Success(result);
        }

        protected abstract (string, double) Calculate(double operand);
    }
}