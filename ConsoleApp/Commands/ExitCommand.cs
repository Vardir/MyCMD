using Core.Commands;
using static CParser;

namespace ConsoleApp.Commands
{
    public class ExitCommand : Command
    {
        public ExitCommand() : base("exit", 
                                    "Quit application immediate. Requires no parameters.", 
                                    "exit") {}

        public override ExecutionResult Execute(Expression expr)
        {
            if (!expr.IsCEmpty)
                return ExecutionResult.Error("exit.error: the command does not take any parameters/arguments");

            Program.Close();

            return ExecutionResult.Success(new object());
        }
    }
}