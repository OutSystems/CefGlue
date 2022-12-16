using System;
using System.Linq;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ParametersDeserializerState : BaseDeserializerState<Array>
    {
        private readonly ParametersTypes _parameterTypes;
        private readonly JsonTypeInfo _optionalParameterInfo;

        private long _mandatoryParameterIndex;
        private long _optionalParameterIndex;

        internal record ParametersTypes
        {
            public ParametersTypes(Type[] mandatory, Type optional)
            {
                Mandatory = mandatory;
                Optional = optional;
            }

            public Type[] Mandatory { get; private set; }
            public Type Optional { get; private set; }
        }

        public ParametersDeserializerState(Array objectHolder, ParametersTypes parametersTypes) : base(objectHolder) 
        { 
            _parameterTypes = parametersTypes;
            _optionalParameterInfo =
                parametersTypes.Optional != null ? 
                JsonTypeInfoCache.GetOrAddTypeInfo(parametersTypes.Optional) : 
                null;
        }

        internal static ParametersDeserializerState Create(Utf8JsonReader reader, ParametersTypes parametersTypes)
        {
            var newArray = CreateParametersArray(reader, parametersTypes);
            return new ParametersDeserializerState(newArray, parametersTypes);
        }

        public override bool IsStructObjectType => false;

        public override JsonTypeInfo ObjectTypeInfo => 
            IsAtOptionalParameterPosition ? 
            _optionalParameterInfo :
                _parameterTypes.Mandatory.Any() ?
                JsonTypeInfoCache.GetOrAddTypeInfo(_parameterTypes.Mandatory[_mandatoryParameterIndex]) :
                JsonTypeInfoCache.GetOrAddTypeInfo(typeof(object));
        
        public override void SetValue(object value)
        {
            if (IsAtOptionalParameterPosition)
            {
                var optionalArgs = (Array)ObjectHolder.GetValue(_mandatoryParameterIndex);
                optionalArgs.SetValue(value, _optionalParameterIndex);
                _optionalParameterIndex++;
            }
            else
            {
                ObjectHolder.SetValue(value, _mandatoryParameterIndex);
                _mandatoryParameterIndex++;
            }
            
        }

        public override Array CreateObjectInstance(Utf8JsonReader reader)
        {
            return CreateParametersArray(reader, _parameterTypes);
        }

        private static Array CreateParametersArray(Utf8JsonReader reader, ParametersTypes parametersTypes)
        {
            var arraySize = reader.PeekAndCalculateArraySize();

            if (parametersTypes.Optional == null)
            {
                return Array.CreateInstance(typeof(object), arraySize);
            }

            // reaching here, it's a args array with optional arguments
            // the resulting array will be in the form of [...mandatoryArgs, [optionalArgs]<optionalParamArrayType>]
            var optionalParamArraySize = arraySize - parametersTypes.Mandatory.Length;
            var argsArray = Array.CreateInstance(typeof(object), parametersTypes.Mandatory.Length + 1);
            var optionalParamsArray = Array.CreateInstance(parametersTypes.Optional.GetElementType(), optionalParamArraySize);
            argsArray.SetValue(optionalParamsArray, argsArray.Length - 1);

            return argsArray;
        }

        private bool IsAtOptionalParameterPosition => 
            _optionalParameterIndex > 0 || 
            (_mandatoryParameterIndex == ObjectHolder.Length - 1 && _parameterTypes.Optional != null);
    }
}
