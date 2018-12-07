using ParserLib;
using Core.Continuations;
using static ParserLib.CmdParser;

namespace Core.Commands
{
    public class HelpCommand : Command
    {
        public HelpCommand() : base("help",
                                    "Prints out the information about command.",
                                    "help <cmd>\n\tcmd -- command ID") {}

        protected override ExecutionResult Execute(Continuation<Expression> continuation, ExecutionResult input)
        {
            Expression arg = null;

            continuation = continuation.BeginWith(e => e.IsCArgument,
                e => arg = Interop.extractInnerExpression(e),
                "expected command argument").Break();

            if (continuation.Failure != ContinuationFailure.None)
            {
                var message = continuation.Failure.GetMessageOn(
                    onEnded: $"help.error: not enough parameters",
                    onEmpty: $"help.error: not enough parameters",
                    onNotEnded: $"help.error: too many arguments",
                    defaults: $"help.error: {continuation.FailureMessage}");
                if (message != null)
                    return ExecutionResult.Error(message);        
            }
            if (!arg.IsCVar)
                return ExecutionResult.Error("help.error: the command requires argument: command ID");

            string id = Interop.extractVar(arg);
            Command cmd = ExecutionService.FindCommand(id);
            if (cmd == null)
                return ExecutionResult.Error($"help.error: can not find command with ID '{id}'");

            return ExecutionResult.Success($"ID: {id}\nDescription: {cmd.Description}\nSyntax: {cmd.Syntax}");
        }
    }
}