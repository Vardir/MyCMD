using System;
using ParserLib;
using System.Linq;
using Core.Attributes;
using System.Reflection;
using static ParserLib.CmdParser;
using System.Collections.Generic;

namespace Core.Commands
{
    public abstract class Command
    {
        private int _index;
        private string pipelinedParameter;
        private Parameter[] parametersArray;

        private readonly List<string> parametersOrder;
        private readonly Dictionary<string, Parameter> parameters;

        public string Id { get; }
        public string Description { get; private set; }
        public ExecutionService ExecutionService { get; set; }

        public Command(string id)
        {
            Id = id;
            parameters = new Dictionary<string, Parameter>();
            parametersOrder = new List<string>();
            Initialize();
        }

        public ExecutionResult Execute(Expression expression) => Execute(expression, ExecutionResult.Empty());
        public ExecutionResult Execute(Expression expression, ExecutionResult pipedResult)
        {
            UnsetAllParameters();
            if (!pipedResult.isEmpty)
            {
                if (pipelinedParameter == null)
                    return Error("command does not accept input from the pipeline");
                Parameter parameter = parameters[pipelinedParameter];
                if (!parameter.CanAssign(pipedResult.result))
                    return Error($"pipelined value is of invalid type, expected: {parameter.GetValueType()} but got {pipedResult.result?.GetType()}");
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
        
        protected abstract ExecutionResult Execute();
        
        protected ExecutionResult Error(string message) => ExecutionResult.Error($"{Id}.error: {message}");
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
        private void UnsetAllParameters()
        {
            foreach (var kvp in parameters)
            {
                kvp.Value.Unset();
            }
        }
       
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
        private (Parameter, string) PushParameter(string id, Parameter previous)
        {
            if (!parameters.TryGetValue(id, out Parameter parameter))
                return (null, $"command does not accept parameter {id}");

            if (previous != null)
            {
                if (previous.IsOptional)
                {
                    previous.Set();
                    _index++;
                }
                else
                    return (null, $"parameter {previous.Id} accepts value but got nothing");
            }
            return (parameter, null);
        }        
    }
}