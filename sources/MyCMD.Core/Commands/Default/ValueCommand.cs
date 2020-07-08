using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Attributes.Parameters;
using Vardirsoft.MyCmd.Core.Attributes.Validation;

namespace Vardirsoft.MyCmd.Core.Commands.Default
{
    [AutoRegister]
    [Description("Accepts any value as input and returns the value as is")]
    public class ValueCommand : Command
    {
        [ObjectParameter]
        [ObjectValidation(AllowNulls = true)]
        [Description("A parameter that accepts values of any type")]
        protected object input;

        public ValueCommand() : base("val") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute() => ExecutionResult.Success(input);
    }
}