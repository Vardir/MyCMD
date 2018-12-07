using System;
using Core.Continuations;
using static ParserLib.CmdParser;

namespace Core.Commands.Math
{
    public abstract class MathOneArgumentCommand : MathCommand
    {
        public MathOneArgumentCommand(string id, string description, string syntax) : base(id, description, syntax)
        { }

        protected override ExecutionResult Execute(Continuation<Expression> continuation, ExecutionResult input)
        {
            double operand = 0.0;
            if (input.isSuccessfull)
            {
                try { operand = Convert.ToDouble(input.result); }
                catch
                {
                    return ExecutionResult.Error($"{Id}.error: the first command argument is not a number");
                }
            }
            else
            {
                continuation = continuation.BeginWith(e => TryGetNumber(e, out operand), 
                    (_) => { }, "the first command argument is not a number").Break();

                var message = continuation.Failure.GetMessageOn(
                    onEmpty: $"{Id}.error: not enough parameters",
                    onNotEnded: $"{Id}.error: too many arguments",
                    defaults: $"{Id}.error: {continuation.FailureMessage}");
                if (message != null)
                    return ExecutionResult.Error(message);
            }

            var (error, result) = Calculate(operand);
            if (!string.IsNullOrEmpty(error))
                return ExecutionResult.Error(error);
            return ExecutionResult.Success(result);
        }

        protected abstract (string, double) Calculate(double operand);
    }
}