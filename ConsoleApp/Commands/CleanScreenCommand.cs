using Core.Commands;

namespace ConsoleApp.Commands
{
    public class CleanScreenCommand : Command
    {
        public CleanScreenCommand() : base("cls",
                                           "Cleans off the console. Requires no parameters.",
                                           "cls"){}

        protected override ExecutionResult Execute(ExecutionResult input)
        {
            if (queryItems.Count > 0)
                return ExecutionResult.Error("cls.error: the command does not take any parameters/arguments");

            Program.CleanScreen();

            return ExecutionResult.Empty();
        }
    }
}