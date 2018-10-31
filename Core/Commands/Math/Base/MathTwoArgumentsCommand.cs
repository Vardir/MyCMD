using System;

namespace Core.Commands.Math
{
    public abstract class MathTwoArgumentsCommand : MathCommand
    {
        public MathTwoArgumentsCommand(string id, string description, string syntax) : base(id, description, syntax)
        { }

        protected override ExecutionResult Execute(ExecutionResult input)
        {
            if (input.isSuccessfull && queryItems.Count != 1)
                return ExecutionResult.Error($"{Id}.error: the command expects 1 argument");
            else if (!input.isSuccessfull && queryItems.Count != 2)
                return ExecutionResult.Error($"{Id}.error: the command expects 2 arguments");

            double left = 0.0;
            double right = 0.0;
            if (input.isSuccessfull)
            {
                try { left = Convert.ToDouble(input.result); }
                catch
                {
                    return ExecutionResult.Error($"{Id}error: the first command argument is not a number");
                }
                var (success, number) = GetNumber(queryItems.First.Value);
                if (!success)
                    return ExecutionResult.Error($"{Id}error: the second command argument is expected to be a number");
                right = number;
            }
            else
            {
                var (success, number) = GetNumber(queryItems.First.Value);
                if (!success)
                    return ExecutionResult.Error($"{Id}error: the first command argument is expected to be a number");
                left = number;

                (success, right) = GetNumber(queryItems.First.Next.Value);
                if (!success)
                    return ExecutionResult.Error($"{Id}error: the second command argument is expected to be a number");
            }
            var (error, result) = Calculate(left, right);
            if (!string.IsNullOrEmpty(error))
                return ExecutionResult.Error(error);
            return ExecutionResult.Success(result);
        }

        protected abstract (string, double) Calculate(double left, double right);
    }
}