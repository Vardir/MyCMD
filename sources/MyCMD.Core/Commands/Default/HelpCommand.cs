using System.Text;

using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Attributes.Parameters;

namespace Vardirsoft.MyCmd.Core.Commands.Default
{
    [AutoRegister]
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
            var cmd = ExecutionService.FindCommand(id);
            
            if (cmd == null)
                return ExecutionResult.Error($"can not find command with ID '{id}'");
            
            return ExecutionResult.Success($"ID: {id}\nDescription: {cmd.Description}\nSyntax: {BuildSyntax(cmd)}");
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
            
            var array = cmd.GetParameters();
            foreach (var parameter in array)
            {
                if (parameter.IsFlag)
                {
                   WriteParameterAsFlag(builder, parameter.Id);
                }
                else if (parameter.IsOptional)
                {
                    WriteParameterAsOptional(builder, parameter.Id);
                }
                else
                {
                    WriteParameter(builder, parameter.Id);
                }
            }

            if (array.Length == 0) 
                return builder.ToString();

            builder.Append('\n');
            builder.Append("where:");

            foreach (var parameter in array)
            {
                builder.Append($"\n\t-{parameter.Id} -- {(parameter.IsPipelined ? "(pipelined)" : null)} {parameter.Description}");
            }

            return builder.ToString();
        }

        private static void WriteParameterAsFlag(StringBuilder builder, string parameterId)
        {
            builder.Append('[');
            builder.Append('-');
            builder.Append(parameterId);
            builder.Append(']');
            builder.Append(' ');
        }
        
        private static void WriteParameterAsOptional(StringBuilder builder, string parameterId)
        {
            builder.Append('[');
            builder.Append('-');
            builder.Append(parameterId);
            builder.Append(' ');
            builder.Append("<arg>");
            builder.Append(']');
            builder.Append(' ');
        }

        private static void WriteParameter(StringBuilder builder, string parameterId)
        {
            builder.Append('-');
            builder.Append(parameterId);
            builder.Append(' ');
            builder.Append("<arg>");
            builder.Append(' ');
        }
    }
}