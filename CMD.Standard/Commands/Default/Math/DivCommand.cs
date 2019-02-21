using Core.Attributes;

namespace Core.Commands.Math
{
    [AutoRegistrate]
    [Description("Divides the given numeric arguments.")]
    public class DivCommand : MathTwoArgumentsCommand
    {
        public DivCommand() : base("div") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute()
        {
            if (right == 0.0)
                return Error("zero-division exception");

            return ExecutionResult.Success(left / right);
        }
    }
}