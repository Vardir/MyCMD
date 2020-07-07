using System.Text;
using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Attributes.Paramater;

namespace Vardirsoft.MyCmd.Core.Commands.Default
{
    [AutoRegistrate]
    [Description("Prints out the information about the given command.")]
    public class HelpCommand : Command
    {
        [StringParameter("help", IsOptional = true)]
        [Description("ID of the command")]
        protected string id;

        private readonly StringBuilder builder;

        public HelpCommand() : base("help")
        {
            builder = new StringBuilder();
        }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute()
        {            
            Command cmd = ExecutionService.FindCommand(id);
            if (cmd == null)
                return ExecutionResult.Error($"can not find command with ID '{id}'");
            string syntax = BuildSyntax(cmd);
            return ExecutionResult.Success($"ID: {id}\nDescription: {cmd.Description}\nSyntax: {syntax}");
        }

        /// <summary>
        /// Builds the syntax of the given command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
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
                    builder.Append($"\n\t-{parameter.Id} -- {(parameter.IsPipelined ? "(pipelined)" : null)} {parameter.Description}");
                }
            }
            return builder.ToString();
        }
    }
}