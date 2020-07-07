using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Attributes.Paramater;
using Vardirsoft.MyCmd.Core.Attributes.Validation;

namespace Vardirsoft.MyCmd.Core.Commands.Default.Math
{
    [AutoRegistrate]
    [Description("Sums up values in the given list")]
    public class SumManyCommand : Command
    {
        [Pipeline]
        [ArrayParameter]
        [ArrayValidation(MinLength = 1, ValueType = typeof(double))]
        protected object[] inputs;

        public SumManyCommand() : base("summ") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute()
        {
            double sum = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double value = (double)inputs[i];
                sum += value;
            }
            return ExecutionResult.Success(sum);
        }
    }
}