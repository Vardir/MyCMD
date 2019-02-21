using Core.Commands;
using Core.Attributes;

namespace ConsoleApp.Commands
{
    [AutoRegistrate]
    [Description("Cleans off the console. Requires no parameters.")]
    public class CleanScreenCommand : Command
    {
        public CleanScreenCommand() : base("cls"){}

        protected override ExecutionResult Execute()
        {
            Program.CleanScreen();
            return ExecutionResult.Empty();
        }
    }
}