using static CParser;

namespace Core.Commands
{
    public abstract class Command
    {
        public string Id { get; }
        public string Description { get; }
        public string Syntax { get; }
        public ExecutionService ExecutionService { get; set; }

        public Command(string id, string description, string syntax)
        {
            Id = id;
            Description = description;
            Syntax = syntax;
        }

        public abstract ExecutionResult Execute(Expression expr);
    }
}