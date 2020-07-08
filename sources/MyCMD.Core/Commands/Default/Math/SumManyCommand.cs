using System.Linq;

using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Attributes.Parameters;
using Vardirsoft.MyCmd.Core.Attributes.Validation;

namespace Vardirsoft.MyCmd.Core.Commands.Default.Math
{
    [AutoRegister]
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
        protected override ExecutionResult Execute() => ExecutionResult.Success(inputs.Cast<double>().Sum());
    }
}