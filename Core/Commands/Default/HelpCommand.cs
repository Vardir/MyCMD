using System.Text;
using Core.Attributes;

namespace Core.Commands
{
    [Description("Prints out the information about the given command.")]
    public class HelpCommand : Command
    {
        [StringParameter(Key="id")]
        [Description("ID of the command")]
        protected string id;

        private readonly StringBuilder builder;

        public HelpCommand() : base("help")
        {
            builder = new StringBuilder();
        }

        protected override ExecutionResult Execute()
        {            
            Command cmd = ExecutionService.FindCommand(id);
            if (cmd == null)
                return ExecutionResult.Error($"can not find command with ID '{id}'");
            string syntax = BuildSyntax(cmd);
            return ExecutionResult.Success($"ID: {id}\nDescription: {cmd.Description}\nSyntax: {syntax}");
        }

        private string BuildSyntax(Command cmd)
        {
            builder.Clear();
            builder.Append(cmd.Id);
            builder.Append(' ');
            Parameter[] array = cmd.GetParameters();
            for (int p = 0; p < array.Length; p++)
            {
                Parameter parameter = array[p];
                if (parameter.IsFlag)
                {
                    builder.Append('[');
                    builder.Append('-');
                    builder.Append(parameter.Id);
                    builder.Append(']');
                    builder.Append(' ');
                }
                else if (parameter.IsOptional)
                {
                    builder.Append('[');
                    builder.Append('-');
                    builder.Append(parameter.Id);
                    builder.Append(' ');
                    builder.Append("<arg>");
                    builder.Append(']');
                    builder.Append(' ');
                }
                else
                {
                    builder.Append('-');
                    builder.Append(parameter.Id);
                    builder.Append(' ');
                    builder.Append("<arg>");
                    builder.Append(' ');
                }
            }
            if (array.Length > 0)
            {
                builder.Append('\n');
                builder.Append("where:");
                for (int p = 0; p < array.Length; p++)
                {
                    Parameter parameter = array[p];
                    builder.Append($"\n\t-{parameter.Id} -- {parameter.Description}");
                }
            }
            return builder.ToString();
        }
    }
}