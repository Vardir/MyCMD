using Core.Commands;
using Core.Continuations;
using static ParserLib.CmdParser;

namespace ConsoleApp.Commands
{
    public class CleanScreenCommand : Command
    {
        public CleanScreenCommand() : base("cls",
                                           "Cleans off the console. Requires no parameters.",
                                           "cls"){}

        protected override ExecutionResult Execute(Continuation<Expression> continuation, ExecutionResult input)
        {
            continuation = continuation.BeginWith(e => true, (e) => { }, null).Break();

            if (continuation.Failure != ContinuationFailure.EmptySource)
                return ExecutionResult.Error("cls.error: the command does not take any parameters/arguments");

            Program.CleanScreen();

            return ExecutionResult.Empty();
        }
    }
}