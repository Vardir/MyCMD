using System.Linq;
using Core.Commands;
using static CParser;

namespace ConsoleApp.Commands
{
    public class HelpCommand : Command
    {
        public HelpCommand() : base("help",
                                    "Prints out the information about command.",
                                    "help <cmd>\n\tcmd -- command ID"){}

        public override ExecutionResult Execute(Expression expr)
        {
            if (!expr.IsCQuery)
                return ExecutionResult.Error("help.error: the command requires a query to execute on");

            var query = CExtern.extractQuery(expr);

            Expression arg = null;
            try { arg = query.Single(); }
            catch { }

            if (arg != null && arg.IsCArgument)
                arg = CExtern.extractInnerExpression(arg);

            if (arg == null || !arg.IsCVar)
                return ExecutionResult.Error("help.error: the command requires argument: command ID");

            string id = CExtern.extractVar(arg);
            Command cmd = ExecutionService.FindCommand(id);
            if (cmd == null)
                return ExecutionResult.Error($"help.error: can not find command with ID '{id}'");

            return ExecutionResult.Success($"ID: {id}\nDescription: {cmd.Description}\nSyntax: {cmd.Syntax}");
        }
    }
}