using System;
using System.Collections.Generic;
using static CParser;

namespace Core.Commands
{
    public abstract class Command
    {
        public string Id { get; }
        public string Description { get; }
        public string Syntax { get; }
        public ExecutionService ExecutionService { get; set; }

        protected LinkedList<Expression> queryItems;

        public Command(string id, string description, string syntax)
        {
            Id = id;
            Description = description;
            Syntax = syntax;
            queryItems = new LinkedList<Expression>();
        }

        public ExecutionResult Execute(Expression expression) => Execute(expression, ExecutionResult.Empty());
        public ExecutionResult Execute(Expression expression, ExecutionResult pipedResult)
        {
            queryItems.Clear();
            if (expression.IsCEmpty)
                return Execute(pipedResult);
            if (expression.IsCQuery)
            {
                var query = CExtern.extractQuery(expression);
                foreach (var item in query)
                {
                    if (item.IsCArgument || item.IsCParameter)
                        queryItems.AddLast(item);
                    else
                        throw new NotImplementedException();
                }
                return Execute(pipedResult);
            }
            throw new NotImplementedException();
            //todo: add cases
        }

        protected abstract ExecutionResult Execute(ExecutionResult input);
    }
}