using System;
using Core.Attributes;

namespace Core.Commands
{
    [Description("A test command")]
    public class TestCommand : Command
    {
        [Flag(Key = "b")]
        [Description("A test flag parameter")]
        protected bool bParam;

        [Pipeline]
        [StringParameter(Key = "s", IsOptional = true)]
        [Description("A test string parameter")]
        protected string sParam;

        [NumberParameter(Key = "d")]
        [Description("A test double parameter")]
        protected double dParam;

        public TestCommand() : base("test")
        {

        }

        protected override ExecutionResult Execute()
        {
            throw new NotImplementedException();
        }
    }
}