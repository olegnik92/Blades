using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using System.Reflection;

namespace Blades.Basis
{
    public class OperationsActivator : IOperationsActivator
    {
        private IOperationMetaInfoProvider metaInfoProvider;
        public OperationsActivator(IOperationMetaInfoProvider metaInfoProvider)
        {
            this.metaInfoProvider = metaInfoProvider;
        }

        public Operation Create(string operationName, object data, UserInfo user)
        {
            var metaInfo = metaInfoProvider.Get(operationName);
            var operationInstance = CreateInstance(metaInfo.ClassType);
            operationInstance.User = user;
            var dataProperty = metaInfo.ClassType.GetProperty("Data");
            dataProperty.SetValue(operationInstance, data);

            return operationInstance;
        }

        protected virtual Operation CreateInstance(Type operationType)
        {
            return (Operation)Activator.CreateInstance(operationType);
        }
    }
}
