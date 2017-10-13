using System;
using System.Reflection;
using Blades.Core.Services;

namespace Blades.Core.ServicesBase
{
    public class OperationsMetaInfoProvider
    {
        private ITypeMap _typeMap;
        public OperationsMetaInfoProvider(ITypeMap typeMap)
        {
            this._typeMap = typeMap;
        }


        public OperationMetaInfo GetInfo(Guid operationTypeId)
        {
            var operationType = _typeMap.Get(operationTypeId);
            
            var genericParent = operationType.GetTypeInfo();
            while(!genericParent.BaseType.Equals(typeof(Operation)))
            {
                genericParent = genericParent.BaseType.GetTypeInfo();
            }

            return new OperationMetaInfo
            {
                ClassType = operationType,
                DataType = genericParent.GenericTypeArguments?[0],
                ResultType = genericParent.GenericTypeArguments?[1],
            };
        }
    }
}