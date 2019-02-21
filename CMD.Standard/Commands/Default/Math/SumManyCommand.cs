using Core.Attributes;

namespace Core.Commands.Math
{
    [AutoRegistrate]
    [Description("Sums up values in the given list")]
    public class SumManyCommand : Command
    {
        [Pipeline]
        [ArrayParameter]
        [ArrayValidation(minLength: 1, maxLength: int.MaxValue, allowArrayNullReference: false, 
                         allowNullValues: false, arrayValueType: typeof(double))]
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