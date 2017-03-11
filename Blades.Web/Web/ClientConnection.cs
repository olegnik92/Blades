using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Owin.WebSocket;
using Newtonsoft.Json;
using Microsoft.Owin;
using Blades.Interfaces;
using Blades.Core;
using Blades.Extensions;
using System.Security.Claims;
using Newtonsoft.Json.Serialization;

namespace Blades.Web
{
    public class ClientConnection : WebSocketConnection
    {
        public UserInfo User { get; private set; }
        public Guid Id { get; private set; } = Guid.NewGuid();

        private static string[] testMessageSeparator = new string[] { "@@@@@" };
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };


        protected IOperationMetaInfoProvider metaInfo;
        protected IOperationsExecutor executor;
        protected ILogger log;
        public ClientConnection(IOperationMetaInfoProvider metaInfo, IOperationsExecutor executor, ILogger log)
        {
            this.metaInfo = metaInfo;
            this.executor = executor;
            this.log = log;
        }


        public override void OnOpen()
        {
            try
            {
                base.OnOpen();
                log.Info($"Web socket connection {Id} OPENED for UserId: {User.Id}");
                ClientConnectionsManager.Instance.AddConnection(this);
            }
            catch(Exception err)
            {
                log.Error(err);
            }

        }

        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        {
            try
            {
                if (type == WebSocketMessageType.Text)
                {
                    var dataStr = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
                    var dataParts = dataStr.Split(testMessageSeparator, 3, StringSplitOptions.None);
                    if (OperationRequestTypes.JsonOperation.Equals(dataParts[0]))
                    {
                        var operationName = dataParts[1];
                        var operationData = dataParts[2];
                        var info = metaInfo.Get(operationName);
                        var data = JsonConvert.DeserializeObject(operationData, info.DataType);
                        Task.Run(() => executor.Execute(operationName, data, User));
                    }
                }
                return base.OnMessageReceived(message, type);
            }
            catch (Exception err)
            {
                log.Error(err);
                return null;
            }
        }


        public Task SendMessage(NotifyMessage message)
        {
            try
            {
                var json = JsonConvert.SerializeObject(message, jsonSettings);
                var data = Encoding.UTF8.GetBytes(json);
                return SendText(data, true);
            }
            catch (Exception err)
            {
                log.Error(err);
                return null;
            }
        }


        public override void OnReceiveError(Exception error)
        {
            log.Error(error);
            base.OnReceiveError(error);
        }

        public override void OnClose(WebSocketCloseStatus? closeStatus, string closeStatusDescription)
        {
            try
            {
                base.OnClose(closeStatus, closeStatusDescription);
                log.Info($"Web socket connection {Id} CLOSED for UserId: {User.Id}");
                var result = ClientConnectionsManager.Instance.RemoveConnection(this);
                if (!result)
                {
                    log.Warning($"Web socket connection {Id} HAS NOT BEEN REMOVED");
                }
            }
            catch(Exception err)
            {
                log.Error(err);
            }
        }

        public override bool AuthenticateRequest(IOwinRequest request)
        {
            try
            {
                var principal = request.User as ClaimsPrincipal;
                if ((principal?.Identity.GetUserId() ?? Guid.Empty).Equals(Guid.Empty))
                {
                    return false;
                }

                this.User = new UserInfo(principal);
                this.User.Location = request.RemoteIpAddress;
                this.User.UserAgent = request.Headers.Get("User-Agent");
                return true;
            }
            catch(Exception err)
            {
                log.Error(err);
                return false;
            }

        }
    }
}
