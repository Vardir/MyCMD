using System;

namespace Core.Commands.Math
{
    public abstract class MathOneArgumentCommand : MathCommand
    {
        public MathOneArgumentCommand(string id, string description, string syntax) : base(id, description, syntax)
        { }

        protected override ExecutionResult Execute(ExecutionResult input)
        {
            if (input.isSuccessfull && queryItems.Count != 0)
                return ExecutionResult.Error($"{Id}.error: the command expects 0 arguments");
            else if (!input.isSuccessfull && queryItems.Count != 1)
                return ExecutionResult.Error($"{Id}.error: the command expects 1 argument");

            double operand = 0.0;
            if (input.isSuccessfull)
            {
                try { operand = Convert.ToDouble(input.result); }
                catch
                {
                    return ExecutionResult.Error($"{Id}error: the first command argument is not a number");
                }
            }
            else
            {
                var (success, number) = GetNumber(queryItems.First.Value);
                if (!success)
                    return ExecutionResult.Error($"{Id}error: the command argument is expected to be a number");
                operand = number;
            }
            var (error, result) = Calculate(operand);
            if (!string.IsNullOrEmpty(error))
                return ExecutionResult.Error(error);
            return ExecutionResult.Success(result);
        }

        protected abstract (string, double) Calculate(double operand);
    }
}