using Core.Continuations;
using static ParserLib.CmdParser;

namespace Core.Commands
{
    public class CommandListCommand : Command
    {
        public CommandListCommand() : base("clist",
                                           "Prints out all available commands.",
                                           "clist")
        { }

        protected override ExecutionResult Execute(Continuation<Expression> continuation, ExecutionResult input)
        {
            continuation = continuation.BeginWith(e => true, (e) => { }, null).Break();

            if (continuation.Failure != ContinuationFailure.EmptySource)
                return ExecutionResult.Error("clist.error: the command does not take any parameters/arguments");

            var commands = ExecutionService.GetAllCommandsIDs();            
            return ExecutionResult.Success(string.Join("; ", commands));
        }
    }
}