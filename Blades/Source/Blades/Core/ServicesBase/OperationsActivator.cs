using System;
using System.Reflection;
using Blades.Core.Services;

namespace Blades.Core.ServicesBase
{
    public class OperationsActivator: IOperationsActivator
    {
        private readonly OperationsMetaInfoProvider _metaInfoProvider;
        public OperationsActivator(ITypeMap typeMap)
        {
            _metaInfoProvider = new OperationsMetaInfoProvider(typeMap);
        }

        
        public Operation Create(Guid operationTypeId, object data, UserInfo user)
        {
            var metaInfo = _metaInfoProvider.GetInfo(operationTypeId);
            var operationInstance = CreateInstance(metaInfo.ClassType);
            operationInstance.Id = Guid.NewGuid();
            operationInstance.User = user;
            
            var dataProperty = metaInfo.ClassType.GetTypeInfo().GetProperty("Data");
            dataProperty.SetValue(operationInstance, data);
            
            return operationInstance;
        }
        
        
        protected virtual Operation CreateInstance(Type operationType)
        {
            return (Operation)Activator.CreateInstance(operationType);
        }
    }
}