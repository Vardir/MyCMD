using System;
using Core.Continuations;
using static ParserLib.CmdParser;

namespace Core.Commands.Math
{
    public abstract class MathTwoArgumentsCommand : MathCommand
    {
        public MathTwoArgumentsCommand(string id, string description, string syntax) : base(id, description, syntax)
        { }

        protected override ExecutionResult Execute(Continuation<Expression> continuation, ExecutionResult input)
        {
            double left = 0.0;
            double right = 0.0;
            if (input.isSuccessfull)
            {
                try { left = Convert.ToDouble(input.result); }
                catch
                {
                    return ExecutionResult.Error($"{Id}error: the first command argument is not a number");
                }
                continuation = continuation.BeginWith(e => TryGetNumber(e, out right), 
                    (_) => { }, "the first command argument is not a number").Break();

                var message = getMessage(continuation);
                if (message != null)
                    return ExecutionResult.Error(message);
            }
            else
            {
                continuation = continuation
                .BeginWith(e => TryGetNumber(e, out left), 
                    (_) => { }, "the first command argument is not a number")
                .AndThen(e => TryGetNumber(e, out right),
                    (_) => { }, "the second command argument is not a number")
                .Break();

                var message = getMessage(continuation);
                if (message != null)
                    return ExecutionResult.Error(message);
            }

            var (error, result) = Calculate(left, right);
            if (!string.IsNullOrEmpty(error))
                return ExecutionResult.Error(error);
            return ExecutionResult.Success(result);

            string getMessage(Continuation<Expression> c)
            {
                return c.Failure.GetMessageOn(
                    onEnded: $"{Id}.error: not enough parameters",
                    onEmpty: $"{Id}.error: not enough parameters",
                    onNotEnded: $"{Id}.error: too many arguments",
                    defaults: $"{Id}.error: {c.FailureMessage}");
            }
        }

        protected abstract (string, double) Calculate(double left, double right);        
    }
}