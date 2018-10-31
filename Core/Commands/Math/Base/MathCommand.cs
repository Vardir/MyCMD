using ParserLib;
using static ParserLib.CmdParser;

namespace Core.Commands.Math
{
    public abstract class MathCommand : Command
    {
        public MathCommand(string id, string description, string syntax) : base(id, description, syntax)
        { }

        protected (bool, double) GetNumber(Expression arg)
        {
            if (arg.IsCNumber)
                return (true, Interop.extractNumber(arg));
            else if (!arg.IsCArgument)
                return (false, 0.0);

            return (true, Interop.extractNumber(Interop.extractInnerExpression(arg)));
        }
    }
}