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
            if (!expr.IsCEmpty)
                return ExecutionResult.Error("cls.error: the command does not take any parameters/arguments");

            Program.CleanScreen();

            return ExecutionResult.Success(new object());
        }
    }
}