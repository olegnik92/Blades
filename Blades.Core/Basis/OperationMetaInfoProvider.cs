using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using Blades.Interfaces;

namespace Blades.Basis
{
    public class OperationMetaInfoProvider : IOperationMetaInfoProvider
    {
        private Dictionary<string, OperationMetaInfo> operationsInfo;
        public OperationMetaInfoProvider()
        {
            operationsInfo = new Dictionary<string, OperationMetaInfo>();
            var operationTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(Operation)) && !t.IsGenericType && !t.IsAbstract);

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
            var genericParent = operationType.BaseType;
            while(!genericParent.BaseType.Equals(typeof(Operation)))
            {
                genericParent = genericParent.BaseType;
            }


            var info = new OperationMetaInfo
            {
                OperationName = ((OperationAttribute)Attribute.GetCustomAttribute(operationType, typeof(OperationAttribute))).Name,
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
