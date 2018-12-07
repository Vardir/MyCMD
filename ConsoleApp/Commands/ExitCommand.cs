using Core.Commands;
using Core.Continuations;
using static ParserLib.CmdParser;

namespace ConsoleApp.Commands
{
    public class ExitCommand : Command
    {
        public ExitCommand() : base("exit", 
                                    "Quit application immediate. Requires no parameters.", 
                                    "exit") {}

        protected override ExecutionResult Execute(Continuation<Expression> continuation, ExecutionResult input)
        {
            continuation = continuation.BeginWith(e => true, (e) => { }, null).Break();

            if (continuation.Failure != ContinuationFailure.EmptySource)                
                return ExecutionResult.Error("exit.error: the command does not take any parameters/arguments");

            Program.Close();

            return ExecutionResult.Empty();
        }
    }
}