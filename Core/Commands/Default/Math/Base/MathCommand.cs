using ParserLib;
using static ParserLib.CmdParser;

namespace Core.Commands.Math
{
    public abstract class MathCommand : Command
    {
        public MathCommand(string id) : base(id)
        { }

        protected (bool, double) GetNumber(Expression arg)
        {
            if (arg.IsCNumber)
                return (true, Interop.extractNumber(arg));
            else if (!arg.IsCArgument)
                return (false, 0.0);
            var inner = Interop.extractInnerExpression(arg);
            return GetNumber(inner);
        }
        protected bool TryGetNumber(Expression arg, out double number)
        {
            var (success, nb) = GetNumber(arg);
            number = nb;
            return success;
        }
    }
}