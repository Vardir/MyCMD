using Vardirsoft.MyCmd.Core.Attributes;

namespace Vardirsoft.MyCmd.Core.Commands.Default
{
    [AutoRegistrate]
    [Description("Prints out all available commands.")]
    public class CommandListCommand : Command
    {
        public CommandListCommand() : base("clist") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute()
        {
            var commands = ExecutionService.GetAllCommandsIDs();            
            return ExecutionResult.Success(string.Join("; ", commands));
        }
    }
}