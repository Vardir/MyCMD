using Core.Commands;
using Core.Attributes;

namespace ConsoleApp.Commands
{
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