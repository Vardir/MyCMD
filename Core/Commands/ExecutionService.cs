using System;
using System.Collections.Generic;
using static CParser;

namespace Core.Commands
{
    public class ExecutionService
    {
        private readonly Dictionary<string, Command> commands;
        
        public ExecutionService()
        {
            commands = new Dictionary<string, Command>();
        }

        public void AddCommand(Command cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            if (string.IsNullOrWhiteSpace(cmd.Id))
                throw new ArgumentException("cmd ID is not valid");

            if (commands.ContainsKey(cmd.Id))
                return;

            commands.Add(cmd.Id, cmd);
            cmd.ExecutionService = this;
        }
        public void AddCommands(IEnumerable<Command> commands)
        {
            if (commands == null)
                throw new ArgumentNullException("commands");

            foreach (var cmd in commands)
                AddCommand(cmd);
        }

        public Command FindCommand(string id)
        {
            if (commands.TryGetValue(id, out Command cmd))
                return cmd;
            return null;
        }
        public ExecutionResult Execute(string line)
        {
            var result = CExtern.runParser(line);
            
            if (result.successfull)
                return Execute(CExtern.extractInnerExpression(result.expression));
            else
                return ExecutionResult.Error(result.message);
        }

        private ExecutionResult Execute(Expression expr)
        {
            if (expr.IsCCommand)
            {
                var (id, query) = CExtern.extractCmd(expr);
                if (commands.TryGetValue(id, out Command cmd))
                    return cmd.Execute(query);
                else
                    return ExecutionResult.Error($"cmd.error: can not find command '{id}'");
            }
            else if (expr.IsCBoolean)
                return ExecutionResult.Success(CExtern.extractBoolean(expr));
            else if (expr.IsCNumber)
                return ExecutionResult.Success(CExtern.extractNumber(expr));
            else if (expr.IsCString)
                return ExecutionResult.Success(CExtern.extractString(expr));

            return ExecutionResult.Error("cmd.error: can not execute the given expression");
        }
    }
}