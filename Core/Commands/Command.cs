using System;
using ParserLib;
using System.Linq;
using Core.Continuations;
using System.Collections.Generic;
using static ParserLib.CmdParser;

namespace Core.Commands
{
    public abstract class Command
    {
        public string Id { get; }
        public string Description { get; }
        public string Syntax { get; }
        private LinkedList<Expression> queryItems;
        public ExecutionService ExecutionService { get; set; }

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
                return Execute(Continuation<Expression>.Empty(), pipedResult);
            if (expression.IsCQuery)
            {
                var query = Interop.extractQuery(expression);
                foreach (var item in query)
                {
                    if (item.IsCArgument || item.IsCParameter)
                        queryItems.AddLast(item);
                    else
                        throw new NotImplementedException();
                }
                return Execute(Continuation.Of(queryItems.GetEnumerator()), pipedResult);
            }
            throw new NotImplementedException();
            //todo: add cases
        }

        protected abstract ExecutionResult Execute(Continuation<Expression> continuation, ExecutionResult input);
        
        protected bool HasItem(Func<Expression, bool> pattern)
        {
            return queryItems.FirstOrDefault(pattern) != null;
        }
        protected bool HasParameter(string value)
        {
            var node = queryItems.First;
            while (node != null)
            {
                Expression expression = node.Value;
                if (expression.IsCParameter)
                {
                    string parameter = Interop.extractParameter(expression);
                    if (parameter == value)
                        return true;
                }
                node = node.Next;
            }
            return false;
        }
        protected Expression GetItem(Func<Expression, bool> pattern)
        {
            Expression expression = queryItems.FirstOrDefault(pattern);
            return expression;
        }
        protected (bool, Expression) AsArgument(Expression expr)
        {
            if (expr.IsCArgument)
            {
                return (true, Interop.extractInnerExpression(expr));
            }
            return (false, null);
        }
    }
}