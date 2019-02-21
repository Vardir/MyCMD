using Core.Attributes;

namespace Core.Commands
{
    [Description("A test command")]
    public class TestCommand : Command
    {
        [Flag(Key = "flag")]
        [Description("A test flag parameter")]
        protected bool bParam;

        [Pipeline]
        [StringParameter(IsOptional = true)]
        [Description("A test string parameter")]
        protected string sParam;

        [NumberParameter]
        [Description("A test double parameter")]
        protected double dParam;

        [ArrayParameter]
        [ArrayValidation(2, 5, false, false, typeof(int))]
        [Description("A test array parameter")]
        protected object[] aParam;

        [ObjectParameter]
        [Description("A test object parameter")]
        protected object oParam;

        public TestCommand() : base("test") { }

        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected override ExecutionResult Execute()
        {
            if (bParam)
                return ExecutionResult.Success(null);
            return Error("flag is not set");
        }
    }
}