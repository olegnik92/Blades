using Blades.Core;
using Blades.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ValueProviders.Providers;

namespace Blades.Web
{
    public class OperationController : ApiController
    {
        protected IOperationMetaInfoProvider metaInfo;
        protected IOperationsExecutor executor;
        protected ILogger log;
        public OperationController(IOperationMetaInfoProvider metaInfo, IOperationsExecutor executor, ILogger log)
        {
            this.metaInfo = metaInfo;
            this.executor = executor;
            this.log = log;
        }

        protected object RequestData { get; set; }

        [HttpPost]
        public async Task<object> ExecutePost()
        {
            try
            {
                var requestType = Request.Headers.GetValues("x-blades-operation-request-type").FirstOrDefault();
                var operationName = Request.Headers.GetValues("x-blades-operation-name").FirstOrDefault();
                var operationMetaInfo = metaInfo.Get(operationName);
                if (requestType == OperationRequestTypes.JsonOperation)
                {
                    string rawJson = await Request.Content.ReadAsStringAsync();
                    RequestData = JsonConvert.DeserializeObject(rawJson, operationMetaInfo.DataType);
                }

                var context = Request.GetOwinContext();
                var user = new UserInfo(context.Request.User as ClaimsPrincipal);
                user.Location = context.Request.RemoteIpAddress;
                user.UserAgent = context.Request.Headers.Get("User-Agent");
                var result = await Task.Run(() => executor.Execute(operationName, RequestData, user));
                return result;
            }
            catch(OperationExecutionException err)
            {
                var response = new HttpResponseMessage(GetHttpStatusCode(err.Status));
                response.Content = new StringContent(string.Join("   \n", err.Errors));
                return response;
            }
            catch(Exception err)
            {
                log.Error(err);
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("FATAL SERVER ERROR");
                return response;
            }
        }

        private HttpStatusCode GetHttpStatusCode(OperationExecutionStatus status)
        {
            switch (status)
            {
                case OperationExecutionStatus.ExecutionWithoutErrors:
                    {
                        return HttpStatusCode.OK;
                    }
                case OperationExecutionStatus.DataValidationErrors:
                    {
                        return HttpStatusCode.BadRequest;
                    }
                case OperationExecutionStatus.PermissionsValidationErrors:
                    {
                        return HttpStatusCode.Forbidden;
                    }
                case OperationExecutionStatus.AuthorizationErrors:
                    {
                        return HttpStatusCode.Unauthorized;
                    }
                default:
                    {
                        return HttpStatusCode.InternalServerError;
                    }
            }
        }
    }
}
