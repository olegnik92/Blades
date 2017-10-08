using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using Blades.Interfaces;
using System.Reflection;

namespace Blades.Basis
{
    public class OperationMetaInfoProvider : IOperationMetaInfoProvider
    {
        private Dictionary<string, OperationMetaInfo> operationsInfo;
        public OperationMetaInfoProvider()
        {
            operationsInfo = new Dictionary<string, OperationMetaInfo>();
            var operationTypes = AppHelper.GetAllTypes()
                .Where(t => t.GetTypeInfo().IsSubclassOf(typeof(Operation)) && !t.GetTypeInfo().IsGenericType && !t.GetTypeInfo().IsAbstract);

            foreach(var t in operationTypes)
            {
                var info = CreateTypeMetaInfo(t);
                if (operationsInfo.ContainsKey(info.OperationName))
                {
                    throw new Exception($"Operation {info.OperationName} duplicated");
                }

                operationsInfo.Add(info.OperationName, info);
            }
        }


        private OperationMetaInfo CreateTypeMetaInfo(Type operationType)
        {
            var genericParent = operationType.GetTypeInfo();
            while(!genericParent.BaseType.Equals(typeof(Operation)))
            {
                genericParent = genericParent.BaseType.GetTypeInfo();
            }


            var info = new OperationMetaInfo
            {
                OperationName = operationType.GetTypeInfo().GetCustomAttribute<OperationAttribute>().Name,
                ClassType = operationType,
                ExecuteTypes = genericParent.GenericTypeArguments
            };

            return info;
        }


        public OperationMetaInfo Get(string operationName)
        {
            OperationMetaInfo result;
            if (operationsInfo.TryGetValue(operationName,  out result))
            {
                return result;
            }
            throw new Exception($"Operation {operationName} execute type not found");
        }
    }
}
