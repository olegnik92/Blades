using Blades.Interfaces;
using Blades.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Blades.Core;

namespace Blades.Web.Basis
{
    public class DataConverter : IDataConverter
    {
        private JsonSerializerSettings jsonSettings;
        private IOperationMetaInfoProvider metaInfo;

        public DataConverter(IOperationMetaInfoProvider metaInfo, ITypeIdMap map)
        {
            this.metaInfo = metaInfo;

            jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new TypeIdBinder(map)
            };

        }

        public JsonSerializerSettings GetSerializerSettings()
        {
            return jsonSettings;
        }

        public object ParseRequestData(string rawData, string operationName, DataFromat format)
        {
            if (format == DataFromat.Json)
            {
                var info = metaInfo.Get(operationName);
                var data = JsonConvert.DeserializeObject(rawData, info.DataType, jsonSettings);
                return data;
            }
            throw new NotSupportedException();
        }

        public string ToJson<T>(T data)
        {
            return JsonConvert.SerializeObject(data, typeof(T), jsonSettings);
        }
    }
}
