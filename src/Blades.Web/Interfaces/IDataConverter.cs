using Blades.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Web.Interfaces
{
    public interface IDataConverter: IBladesService
    {
        JsonSerializerSettings GetSerializerSettings();

        object ParseRequestData(string rawData, string operationName, DataFromat format);

        string ToJson<T>(T data);
    }
}
