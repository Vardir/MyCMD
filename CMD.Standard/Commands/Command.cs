using System;
using ParserLib;
using System.Linq;
using Core.Attributes;
using System.Reflection;
using static ParserLib.CmdParser;
using System.Collections.Generic;

namespace Core.Commands
{
    /// <summary>
    /// A base class for executable command
    /// </summary>
    public abstract class Command
    {
        private int _index;
        private string pipelinedParameter;
        private Parameter[] parametersArray;

        private readonly List<string> parametersOrder;
        private readonly Dictionary<string, Parameter> parameters;

        /// <summary>
        /// The ID of the command
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// The description of the command
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Parent execution service
        /// </summary>
        public ExecutionService ExecutionService { get; set; }

        public Command(string id)
        {
            Id = id;
            parameters = new Dictionary<string, Parameter>();
            parametersOrder = new List<string>();
            Initialize();
        }

        /// <summary>
        /// Executes a command on the given expression
        /// </summary>
        /// <param name="expression">Expression to interpret</param>
        /// <returns></returns>
        public ExecutionResult Execute(Expression expression) => Execute(expression, ExecutionResult.Empty());
        /// <summary>
        /// Executes a command on the given expression using previous execution result
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pipedResult"></param>
        /// <returns></returns>
        public ExecutionResult Execute(Expression expression, ExecutionResult pipedResult)
        {
            UnsetAllParameters();
            if (!pipedResult.isEmpty)
            {
                if (pipelinedParameter == null)
                    return Error("command does not accept input from the pipeline");
                Parameter parameter = parameters[pipelinedParameter];
                if (parameter.Validation != null)
                {
                    string error = parameter.Validation.Validate(pipedResult.result);
                    if (error != null)
                        return Error($"parameter {parameter.Id} got invalid value: {error}");
                }
                else if (!parameter.CanAssign(pipedResult.result))
                    return Error($"parameter {parameter.Id} accepts {parameter.GetValueType()} but got {pipedResult.result.GetType()}");
                parameter.SetValue(pipedResult.result);
            }
            if (expression.IsCEmpty)
                return Execute();
            if (expression.IsCQuery)
            {
                var query = Interop.extractQuery(expression);
                _index = 0;
                Parameter previousParameter = null;
                foreach (var item in query)
                {
                    if (item.IsCParameter)
                    {
                        string id = Interop.extractParameter(item);
                        var (current, message) = PushParameter(id, previousParameter);
                        if (current == null)
                            return Error(message);
                        if (current.IsFlag)
                        {
                            current.Set();
                            continue;
                        }
                        previousParameter = current;
                    }
                    else if (item.IsCArgument)
                    {
                        if (_index >= parametersOrder.Count)
                            return Error("too many parameters given");
                        Parameter parameter = previousParameter ?? parameters[parametersOrder[_index]];
                        if (parameter.IsSet && parameter.Id == pipelinedParameter && previousParameter == null)
                        {
                            if (++_index >= parametersOrder.Count)
                                return Error("too many parameters given");
                            parameter = parameters[parametersOrder[_index]];
                        }
                        object value = Interop.extractObject(Interop.extractInnerExpression(item));
                        string message = SetParameter(parameter, value);
                        if (message != null)
                            return Error(message);
                        previousParameter = null;
                        _index++;
                    }
                    else
                        throw new InvalidOperationException("command must contain only parameters and arguments");
                }
                foreach (var kvp in parameters)
                {
                    Parameter parameter = kvp.Value;
                    if (parameter.IsSet || parameter.IsFlag)
                        continue;
                    if (parameter.IsOptional)
                        parameter.Set();
                    else
                        return Error($"parameter {parameter.Id} is not set");
                }
                return Execute();
            }
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Execution routine of the command
        /// </summary>
        /// <returns></returns>
        protected abstract ExecutionResult Execute();
        
        /// <summary>
        /// Shortcut to generate execution result based on command's ID
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ExecutionResult Error(string message) => ExecutionResult.Error($"{Id}.error: {message}");
        /// <summary>
        /// Gets all the command parameters
        /// </summary>
        /// <returns></returns>
        protected internal Parameter[] GetParameters()
        {
            if (parametersArray != null)
                return parametersArray;
            parametersArray = new Parameter[parameters.Count];
            int i = 0;
            foreach (var id in parametersOrder)
            {
                parametersArray[i] = parameters[id];
                i++;
            }
            foreach (var kvp in parameters)
            {
                if (parametersArray.Contains(kvp.Value))
                    continue;
                parametersArray[i] = kvp.Value;
                i++;
            }
            return parametersArray;
        }

        /// <summary>
        /// Initializes current instance of the command
        /// </summary>
        private void Initialize()
        {
            Type type = GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int f = 0; f < fields.Length; f++)
            {
                ReadFieldAttributes(fields[f]);
            }
            var descriptionAttr = type.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr != null)
                Description = descriptionAttr.Value;
        }
        /// <summary>
        /// Reads all the attributes of each field to identify and registrate command parameters
        /// </summary>
        /// <param name="field"></param>
        private void ReadFieldAttributes(FieldInfo field)
        {
            bool isOptional = false;
            bool isPipelined = false;
            bool isFlag = false;
            object defaultValue = null;
            string id = field.Name.Replace("Param", "").ToLower();
            string description = $"Parameter {id}";
            IParameterValidation validation = null;

            var parameterAttr = field.GetCustomAttribute<ParameterAttribute>();
            if (parameterAttr != null)
            {
                if (!parameterAttr.IsAllowedType(field.FieldType))
                    throw new InvalidOperationException("the attribute attached to the field has an invalid type");
                if (parameterAttr.IsOptional)
                    defaultValue = parameterAttr.GetDefaultValue();
                if (parameterAttr is FlagAttribute)
                    isFlag = true;
                id = parameterAttr.Key ?? id;
                isOptional = parameterAttr.IsOptional;
            }
            else return;

            var descriptionAttr = field.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr != null)
                description = descriptionAttr.Value;            
            var validationAttribute = field.GetCustomAttribute<ParameterValidationAttribute>();
            if (validationAttribute != null)
                validation = validationAttribute;
            if (field.GetCustomAttribute<PipelineAttribute>() != null)
            {
                if (pipelinedParameter != null)
                    throw new InvalidOperationException("the pipeline attribute must be attached only once per class");
                isPipelined = true;
            }

            if (isPipelined)
                pipelinedParameter = id;
            Parameter commandParameter;
            if (isOptional && !isFlag)
                commandParameter = new Parameter(id, description, defaultValue, this, field, validation);
            else
                commandParameter = new Parameter(id, description, isFlag, this, field, validation);
            parameters.Add(id, commandParameter);
            if (!isFlag)
                parametersOrder.Add(id);
        }
        /// <summary>
        /// Clears out values stored in parameters
        /// </summary>
        private void UnsetAllParameters()
        {
            foreach (var kvp in parameters)
            {
                kvp.Value.Unset();
            }
        }
       
        /// <summary>
        /// Stores the given value in the given parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string SetParameter(Parameter parameter, object value)
        {
            if (parameter.IsSet)
                return $"parameter {parameter.Id} is already assigned";
            if (parameter.Validation != null)
            {
                string error = parameter.Validation.Validate(value);
                if (error != null)
                    return $"parameter {parameter.Id} got invalid value: {error}";
            }
            else if (!parameter.CanAssign(value))
                return $"parameter {parameter.Id} accepts {parameter.GetValueType()} but got {value.GetType()}";
            parameter.SetValue(value);
            return null;
        }
        /// <summary>
        /// Pushes the given parameter based on the previous one
        /// </summary>
        /// <param name="id"></param>
        /// <param name="previous"></param>
        /// <returns></returns>
        private (Parameter, string) PushParameter(string id, Parameter previous)
        {
            if (!parameters.TryGetValue(id, out Parameter parameter))
                return (null, $"command '{id}' does not accept parameter");

            if (previous != null)
            {
                if (previous.IsOptional)
                {
                    previous.Set();
                    _index++;
                }
                else
                    return (null, $"parameter '{previous.Id}' accepts value but got nothing");
            }
            return (parameter, null);
        }        
    }
}