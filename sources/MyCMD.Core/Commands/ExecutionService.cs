using System;
using System.Collections.Generic;
using System.Reflection;
using ParserLib;
using Vardirsoft.MyCmd.Core.Attributes;
using static ParserLib.CmdParser;

namespace Vardirsoft.MyCmd.Core.Commands
{
    /// <summary>
    /// An execution service that contains commands, executes commands on the string queries
    /// </summary>
    public class ExecutionService
    {
        private readonly Dictionary<string, Command> commands;

        public ExecutionService()
        {
            commands = new Dictionary<string, Command>();
            LoadCommands();
        }

        /// <summary>
        /// Adds and registrates the given command to internal storage
        /// </summary>
        /// <param name="cmd"></param>
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
        /// <summary>
        /// Adds and registrates a set of commands to internal storage
        /// </summary>
        /// <param name="commands"></param>
        public void AddCommands(IEnumerable<Command> commands)
        {
            if (commands == null)
                throw new ArgumentNullException("commands");

            foreach (var cmd in commands)
                AddCommand(cmd);
        }

        /// <summary>
        /// Searches for a command by the given ID, returns null if nothing found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Command FindCommand(string id)
        {
            if (commands.TryGetValue(id, out Command cmd))
                return cmd;
            return null;
        }
        /// <summary>
        /// Reads the given line and tries to execute the query
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public ExecutionResult Execute(string line)
        {
            var result = Interop.runParser(line);
            
            if (result.successfull)
                return Execute(Interop.extractInnerExpression(result.expression));
            else
                return ExecutionResult.Error(result.message);
        }
        /// <summary>
        /// Gets IDs of all commands stored in service
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllCommandsIDs()
        {
            foreach (var kvp in commands)
            {
                yield return kvp.Key;
            }
        }

        /// <summary>
        /// Loads and registrates commands from the current domain loaded assemblies
        /// </summary>
        private void LoadCommands()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].GetCustomAttribute<ContainsCommandsAttribute>() == null)
                    continue;
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    var attr = types[j].GetCustomAttribute<AutoRegistrateAttribute>();
                    if (attr == null)
                        continue;
                    AddCommand(Activator.CreateInstance(types[j]) as Command);
                }
            }
        }

        /// <summary>
        /// Executes the given expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private ExecutionResult Execute(Expression expr)
        {
            if (expr.IsCCommand)
            {
                var (id, query) = Interop.extractCmd(expr);
                return extractAndExecute(id, query, ExecutionResult.Empty());
            }
            else if (expr.IsCPipeline)
            {
                var cmds = Interop.extractPipeline(expr);
                ExecutionResult result = ExecutionResult.Empty();
                foreach (var cmd in cmds)
                {
                    result = extractAndExecute(cmd.Item1, cmd.Item2, result);
                    if (!result.isSuccessful && !result.isEmpty)
                    {
                        return ExecutionResult.Error($"{result.errorMessage}\ncmd.error: can not compute pipeline");
                    }
                }
                return result;
            }
            else if (expr.IsCNumber)
                return ExecutionResult.Success(Interop.extractNumber(expr));
            else if (expr.IsCString)
                return ExecutionResult.Success(Interop.extractString(expr));
            else if (expr.IsCArray)
                return ExecutionResult.Success(Interop.extractArray(expr));

            return ExecutionResult.Error("cmd.error: can not execute the given expression");

            ExecutionResult extractAndExecute(string id, Expression query, ExecutionResult input)
            {                
                if (commands.TryGetValue(id, out Command cmd))
                    return cmd.Execute(query, input);
                else
                    return ExecutionResult.Error($"cmd.error: can not find command '{id}'");
            }
        }
    }
}