using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ParserLib;

using Vardirsoft.MyCmd.Core.Attributes;
using Vardirsoft.MyCmd.Core.Attributes.Parameters;
using Vardirsoft.MyCmd.Core.Attributes.Validation;
using Vardirsoft.MyCmd.Core.Helpers;

using static ParserLib.CmdParser;

namespace Vardirsoft.MyCmd.Core.Commands
{
    /// <summary>
    /// A base class for executable command
    /// </summary>
    public abstract class Command
    {
        private readonly List<string> _parametersOrder;
        private readonly Dictionary<string, Parameter> _parameters;
        
        private int _index;
        private string _pipelinedParameter;
        private Parameter[] _parametersArray;

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

        protected Command(string id)
        {
            _parameters = new Dictionary<string, Parameter>();
            _parametersOrder = new List<string>();
           
            Id = id;
            
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
                if (_pipelinedParameter is null)
                    return Error("command does not accept input from the pipeline");
                
                var parameter = _parameters[_pipelinedParameter];
                if (parameter.Validation != null)
                {
                    var error = parameter.Validation.Validate(pipedResult.result);
                    if (error != null)
                        return Error($"parameter {parameter.Id} got invalid value: {error}");
                }
                else if (!parameter.CanAssign(pipedResult.result)) 
                    return Error($"parameter {parameter.Id} accepts {parameter.GetValueType()} but got {pipedResult.result.GetType()}");

                parameter.SetValue(pipedResult.result);
            }
            
            if (expression.IsCEmpty)
                return Execute();

            if (!expression.IsCQuery) 
                throw new NotImplementedException();
            
            var query = Interop.extractQuery(expression);
            Parameter previousParameter = null;

            _index = 0;
            foreach (var item in query)
            {
                if (item.IsCParameter)
                {
                    var id = Interop.extractParameter(item);
                    var (current, message) = PushParameter(id, previousParameter);

                    if (current is null)
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
                    if (_index >= _parametersOrder.Count)
                        return Error("too many parameters given");

                    var parameter = previousParameter ?? _parameters[_parametersOrder[_index]];
                    if (parameter.IsSet && parameter.IsPipelined && previousParameter is null)
                    {
                        if (++_index >= _parametersOrder.Count)
                            return Error("too many parameters given");

                        parameter = _parameters[_parametersOrder[_index]];
                    }

                    var value = Interop.extractObject(Interop.extractInnerExpression(item));
                    var message = SetParameter(parameter, value);

                    if (message != null)
                        return Error(message);

                    previousParameter = null;
                    _index++;
                }
                else throw new InvalidOperationException("command must contain only parameters and arguments");
            }

            foreach (var parameter in _parameters.Select(kvp => kvp.Value).Skip(parameter => parameter.IsSet || parameter.IsFlag))
            {
                if (parameter.IsOptional)
                {
                    parameter.Set();
                }
                else return Error($"parameter {parameter.Id} is not set");
            }

            return Execute();
        }
        
        protected abstract ExecutionResult Execute();
        
        protected ExecutionResult Error(string message) => ExecutionResult.Error($"{Id}.error: {message}");
        
        protected internal Parameter[] GetParameters()
        {
            if (_parametersArray != null)
                return _parametersArray;
            
            _parametersArray = new Parameter[_parameters.Count];
            
            var i = 0;
            foreach (var id in _parametersOrder)
            {
                _parametersArray[i] = _parameters[id];
                i++;
            }
            
            foreach (var kvp in _parameters.Skip(kvp => _parametersArray.Contains(kvp.Value)))
            {
                _parametersArray[i] = kvp.Value;
                i++;
            }
            
            return _parametersArray;
        }

        /// <summary>
        /// Initializes current instance of the command
        /// </summary>
        private void Initialize()
        {
            var type = GetType();
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                ReadFieldAttributes(field);
            }
            
            var descriptionAttr = type.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr != null)
            {
                Description = descriptionAttr.Value;
            }
        }
        
        private void ReadFieldAttributes(FieldInfo field)
        {
            var isOptional = false;
            var isPipelined = false;
            var isFlag = false;
            object defaultValue = null;
            var id = field.Name.Replace("Param", "").ToLower();
            var description = $"Parameter {id}";
            IParameterValidation validation = null;

            var parameterAttr = field.GetCustomAttribute<ParameterAttribute>();
            if (parameterAttr != null)
            {
                if (!parameterAttr.IsAllowedType(field.FieldType))
                    throw new InvalidOperationException("the attribute attached to the field has an invalid type");
                
                if (parameterAttr.IsOptional)
                {
                    defaultValue = parameterAttr.GetDefaultValue();
                }
                
                if (parameterAttr is FlagAttribute)
                {
                    isFlag = true;
                }
                
                id = parameterAttr.Key ?? id;
                isOptional = parameterAttr.IsOptional;
            }
            else return;

            var descriptionAttr = field.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr != null)
            {
                description = descriptionAttr.Value;
            }
            
            var validationAttribute = field.GetCustomAttribute<ParameterValidationAttribute>();
            if (validationAttribute != null)
            {
                validation = validationAttribute;
            }
            
            if (field.GetCustomAttribute<PipelineAttribute>() != null)
            {
                if (_pipelinedParameter != null)
                    throw new InvalidOperationException("the pipeline attribute must be attached only once per class");
                
                isPipelined = true;
            }
            
            Parameter commandParameter;
            if (isOptional && !isFlag)
            {
                commandParameter = new Parameter(id, description, defaultValue, this, field, validation);
            }
            else
            {
                commandParameter = new Parameter(id, description, isFlag, this, field, validation);
            }
            if (isPipelined)
            {
                _pipelinedParameter = id;
                commandParameter.IsPipelined = true;
            }
            
            _parameters.Add(id, commandParameter);
            
            if (!isFlag)
            {
                _parametersOrder.Add(id);
            }
        }
        
        private void UnsetAllParameters()
        {
            foreach (var kvp in _parameters)
            {
                kvp.Value.Unset();
            }
        }
       
        private static string SetParameter(Parameter parameter, object value)
        {
            if (parameter.IsSet)
                return $"parameter {parameter.Id} is already assigned";
            
            if (parameter.Validation != null)
            {
                var error = parameter.Validation.Validate(value);
                if (error != null)
                    return $"parameter {parameter.Id} got invalid value: {error}";
            }
            else if (!parameter.CanAssign(value))
                return $"parameter {parameter.Id} accepts {parameter.GetValueType()} but got {value.GetType()}";
            
            parameter.SetValue(value);
            
            return null;
        }
       
        private (Parameter, string) PushParameter(string id, Parameter previous)
        {
            if (!_parameters.TryGetValue(id, out var parameter))
                return (null, $"command '{id}' does not accept parameter");

            if (previous == null) 
                return (parameter, null);
            
            if (previous.IsOptional)
            {
                previous.Set();
                _index++;
            }
            else return (null, $"parameter '{previous.Id}' accepts value but got nothing");
            
            return (parameter, null);
        }        
    }
}