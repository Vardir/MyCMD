using Core.Attributes;

namespace Core.Commands
{
    [Description("Accepts any value as input and returns the value as is")]
    public class ValueCommand : Command
    {
        [ObjectParameter]
        [ObjectValidation(true)]
        [Description("A parameter that accepts values of any type")]
        protected object input;

        public ValueCommand() : base("val") { }

        protected override ExecutionResult Execute() => ExecutionResult.Success(input);
    }
}