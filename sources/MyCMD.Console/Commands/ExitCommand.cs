using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Commands;

namespace Vardirsoft.MyCmd.ConsoleApp.Commands
{
    [AutoRegistrate]
    [Description("Quit application immediate. Requires no parameters.")]
    public class ExitCommand : Command
    {
        public ExitCommand() : base("exit") {}

        protected override ExecutionResult Execute()
        {
            Program.Close();
            return ExecutionResult.Empty();
        }
    }
}