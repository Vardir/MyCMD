using System;
using ParserLib;
using Core.Commands;
using static ParserLib.CmdParser;

namespace ConsoleApp.Commands
{
    public class MulCommand : Command
    {
        public MulCommand() : base("mul",
                                   "Multiplies the given arguments if they are numbers.",
                                   "mul <arg1> <arg2>\n\arg1 -- the left-side value\n\targ2 -- the right-side value")
        { }

        protected override ExecutionResult Execute(ExecutionResult input)
        {
            if (input.isSuccessfull && queryItems.Count != 1)
                return ExecutionResult.Error("mul.error: the command expects 1 argument");
            else if (!input.isSuccessfull && queryItems.Count != 2)
                return ExecutionResult.Error("mul.error: the command expects 2 arguments");

            double left = 0.0;
            double right = 0.0;
            if (input.isSuccessfull)
            {
                try { left = Convert.ToDouble(input.result); }
                catch
                {
                    return ExecutionResult.Error("mul.error: the first command argument is not a number");
                }
                var (success, number) = GetNumber(queryItems.First.Value);
                if (!success)
                    return ExecutionResult.Error("mul.error: the second command argument is expected to be a number");
                right = number;
            }
            else
            {
                var (success, number) = GetNumber(queryItems.First.Value);
                if (!success)
                    return ExecutionResult.Error("mul.error: the first command argument is expected to be a number");
                left = number;

                (success, right) = GetNumber(queryItems.First.Next.Value);
                if (!success)
                    return ExecutionResult.Error("mul.error: the second command argument is expected to be a number");
            }

            return ExecutionResult.Success(left * right);
        }

        private (bool, double) GetNumber(Expression arg)
        {
            if (arg.IsCNumber)
                return (true, Interop.extractNumber(arg));
            else if (!arg.IsCArgument)
                return (false, 0.0);

            return (true, Interop.extractNumber(Interop.extractInnerExpression(arg)));
        }
    }
}