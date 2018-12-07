using Core.Attributes;

namespace Core.Commands
{
    [Description("Prints out all available commands.")]
    public class CommandListCommand : Command
    {
        public CommandListCommand() : base("clist")
        { }

        protected override ExecutionResult Execute()
        {
            var commands = ExecutionService.GetAllCommandsIDs();            
            return ExecutionResult.Success(string.Join("; ", commands));
        }
    }
}