using System.Linq;
using Core.Commands;
using static CParser;

namespace ConsoleApp.Commands
{
    public class CleanScreenCommand : Command
    {
        public CleanScreenCommand() : base("cls",
                                           "Cleans off the console. Requires no parameters.",
                                           "cls"){}

        public override ExecutionResult Execute(Expression expr)
        {
            bool queryIsEmpty = true;
            if (expr.IsCQuery)
            {
                var list = CExtern.extractQuery(expr);
                var any = list.Any();
                queryIsEmpty = !list.Any();
            }
            if (!expr.IsCEmpty && !queryIsEmpty)
                return ExecutionResult.Error("cls.error: the command does not take any parameters/arguments");

            Program.CleanScreen();

            return ExecutionResult.Empty();
        }
    }
}