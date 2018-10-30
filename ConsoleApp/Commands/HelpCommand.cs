using Core.Commands;
using static CParser;

namespace ConsoleApp.Commands
{
    public class HelpCommand : Command
    {
        public HelpCommand() : base("help",
                                    "Prints out the information about command.",
                                    "help <cmd>\n\tcmd -- command ID"){}

        protected override ExecutionResult Execute(ExecutionResult input)
        {
            if (queryItems.Count != 1)
                return ExecutionResult.Error("help.error: the command requires 1 argument");
            
            Expression arg = queryItems.First.Value;
            if (arg.IsCArgument)
                arg = CExtern.extractInnerExpression(arg);

            if (!arg.IsCVar)
                return ExecutionResult.Error("help.error: the command requires argument: command ID");

            string id = CExtern.extractVar(arg);
            Command cmd = ExecutionService.FindCommand(id);
            if (cmd == null)
                return ExecutionResult.Error($"help.error: can not find command with ID '{id}'");

            return ExecutionResult.Success($"ID: {id}\nDescription: {cmd.Description}\nSyntax: {cmd.Syntax}");
        }
    }
}