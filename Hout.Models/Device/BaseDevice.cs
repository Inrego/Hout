using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Threading.Tasks;

using Hout.Models.ParamValidation;
using Hout.Models.Specifications;

namespace Hout.Models.Device
{
    public abstract class BaseDevice
    {
        public BaseDevice()
        {
            Properties = new PropertyCollection();
        }
        public abstract string Id { get; set; }
        public abstract string Name { get; set; }
        public abstract string IconPath { get; }
        public abstract NameDescCollection<PropertySpecification> PropertySpecifications { get; }
        public PropertyCollection Properties { get; set; }
        public abstract string GetId();
        public abstract NameDescCollection<EventSpecification> EventSpecifications { get; }
        public abstract NameDescCollection<CommandSpecification> CommandSpecifications { get; }
        // TODO: Call ValidateParameters before calling ExecuteCommand
        public abstract Task<CommandResponse> ExecuteCommand(string name, Dictionary<string, object> parameters);

        public delegate void DeviceEventFired(string eventName, Dictionary<string, object> parameters);

        public event DeviceEventFired OnEventFired;
        protected void FireEvent(string name, Dictionary<string, object> parameters)
        {
            var validationErrors = ValidateEventParameters(name, parameters);
            // TODO: Log validation errors
            if (!validationErrors.Any())
                OnEventFired?.Invoke(name, parameters);
        }

        protected List<ValidationResult> ValidateCommandParameters(string name, Dictionary<string, object> parameters)
        {
            CommandSpecification commandSpec;
            if (!CommandSpecifications.TryGetValue(name, out commandSpec))
            {
                var results = new List<ValidationResult>();
                results.Add(new ValidationResult(nameof(name), false, $"No command was found with name: {name}"));
                return results;
            }
            return ValidateParameters(commandSpec.ParameterSpecifications, parameters);
        }

        protected List<ValidationResult> ValidateEventParameters(string name, Dictionary<string, object> parameters)
        {
            EventSpecification eventSpec;
            if (!EventSpecifications.TryGetValue(name, out eventSpec))
            {
                var results = new List<ValidationResult>();
                results.Add(new ValidationResult(nameof(name), false, $"No event was found with name: {name}"));
                return results;
            }
            return ValidateParameters(eventSpec.ParameterSpecifications, parameters);
        }

        private List<ValidationResult> ValidateParameters(NameDescCollection<ParameterSpecification> specifications,
            Dictionary<string, object> parameters)
        {
            var invalidParamNames = parameters.Keys.Except(specifications.Keys);
            var results = new List<ValidationResult>();
            results.AddRange(invalidParamNames.Select(param => new ValidationResult(param, false, $"\"{param}\" is not a valid parameter name.")));
            var missingParamNames = specifications.Where(param => param.Value.IsRequired).Select(param => param.Key).Except(parameters.Keys);
            results.AddRange(missingParamNames.Select(param => new ValidationResult(param, false, $"\"{param}\" is required.")));

            var providedParams = parameters.Keys.Intersect(specifications.Keys);
            var validationResults =
                from paramName in providedParams
                let param = parameters[paramName]
                let paramSpec = specifications[paramName]
                let result = paramSpec.ValidateValue(param)
                select result;
            results.AddRange(validationResults);
            return results;
        } 
    }

}
