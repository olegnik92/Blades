using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Owin.WebSocket;
using Microsoft.Owin;
using Blades.Interfaces;
using Blades.Core;
using Blades.Extensions;
using System.Security.Claims;
using Newtonsoft.Json.Serialization;
using Blades.Web.Interfaces;

namespace Blades.Web
{
    public class ClientConnection : WebSocketConnection
    {
        public UserInfo User { get; private set; }
        public Guid Id { get; private set; } = Guid.NewGuid();

        private static string[] testMessageSeparator = new string[] { "@@@@@" };


        protected IDataConverter converter;
        protected IOperationsExecutor executor;
        protected ILogger log;
        public ClientConnection(IDataConverter converter, IOperationsExecutor executor, ILogger log)
        {
            this.converter = converter;
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
                    var dataFormat = dataParts[0].ToDataFormatEnum();
                    var operationName = dataParts[1];
                    var rawData = dataParts[2];
                    var data = converter.ParseRequestData(rawData, operationName, dataFormat);

                    Task.Run(() => executor.Execute(operationName, data, User));
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
                var json = converter.ToJson(message);
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
