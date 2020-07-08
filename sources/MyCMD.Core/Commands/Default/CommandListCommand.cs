using Vardirsoft.MyCmd.Core.Attributes;

namespace Vardirsoft.MyCmd.Core.Commands.Default
{
    [AutoRegister]
    [Description("Prints out all available commands.")]
    public class CommandListCommand : Command
    {
        public CommandListCommand() : base("clist") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute() => ExecutionResult.Success(string.Join("; ", ExecutionService.GetAllCommandsIDs()));
    }
}