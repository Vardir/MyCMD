using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Commands;

namespace Vardirsoft.MyCmd.ConsoleApp.Commands
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