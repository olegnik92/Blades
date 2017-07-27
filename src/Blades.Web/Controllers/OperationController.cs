using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Blades.Core;
using Blades.Web.Interfaces;
using Blades.Interfaces;
using System.Net.Http;

namespace Blades.Web.Controllers
{
    [Route("api/[controller]")]
    public class OperationController : Controller
    {
        protected object RequestData { get; set; }

        protected IDataConverter converter;
        protected IOperationsExecutor executor;
        protected ILogger log;
        public OperationController(IDataConverter converter, IOperationsExecutor executor, ILogger log)
        {
            this.converter = converter;
            this.executor = executor;
            this.log = log;
        }

        [HttpPost]
        public async Task<object> Execute([FromBody]string rawData)
        {
            try
            {
                var requestFormat = Request.Headers["x-blades-operation-request-type"].FirstOrDefault().ToDataFormatEnum();
                var operationName = Request.Headers["x-blades-operation-name"].FirstOrDefault();
                RequestData = converter.ParseRequestData(rawData, operationName, requestFormat);


                var user = new UserInfo(this.User);
                user.Location = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                user.UserAgent = Request.Headers["User-Agent"].FirstOrDefault();


                var result = await Task.Run(() => executor.Execute(operationName, RequestData, user));
                return result;
            }
            catch (OperationExecutionException err)
            {
                var response = new HttpResponseMessage(GetHttpStatusCode(err.Status));
                response.Content = new StringContent(string.Join("   \n", err.Errors?.Select(e => e.Message)));
                return response;
            }
            catch (Exception err)
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
