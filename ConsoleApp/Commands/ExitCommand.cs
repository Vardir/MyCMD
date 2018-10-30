using Core.Commands;

namespace ConsoleApp.Commands
{
    public class ExitCommand : Command
    {
        public ExitCommand() : base("exit", 
                                    "Quit application immediate. Requires no parameters.", 
                                    "exit") {}

        protected override ExecutionResult Execute(ExecutionResult _)
        {
            if (queryItems.Count > 0)
                return ExecutionResult.Error("exit.error: the command does not take any parameters/arguments");

            Program.Close();

            return ExecutionResult.Empty();
        }
    }
}