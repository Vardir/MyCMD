using Core.Attributes;

namespace Core.Commands.Math
{
    public abstract class MathTwoArgumentsCommand : MathCommand
    {
        [Pipeline]
        [NumberParameter(Key = "l")]
        [Description("Left operand of the command")]
        protected double leftOperand;
        
        [NumberParameter(Key = "r")]
        [Description("Right operand of the command")]
        protected double rightOperand;

        public MathTwoArgumentsCommand(string id) : base(id)
        { }

        protected override ExecutionResult Execute()
        {
            var (error, result) = Calculate(leftOperand, rightOperand);
            if (!string.IsNullOrEmpty(error))
                return ExecutionResult.Error(error);
            return ExecutionResult.Success(result);
        }

        protected abstract (string, double) Calculate(double left, double right);        
    }
}