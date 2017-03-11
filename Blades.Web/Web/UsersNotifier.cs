using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using Blades.Interfaces;

namespace Blades.Web
{
    public class UsersNotifier : IUsersNotifier
    {
        private ILogger logger;
        public UsersNotifier(ILogger logger)
        {
            this.logger = logger;
        }

        public void SendMessage(Guid userId, NotifyMessage message, bool useGuaranteedDelivery = false)
        {
            if (useGuaranteedDelivery)
            {
                throw new NotImplementedException(); //TODO добавить очередь задач
            }
            else
            {
                try
                {
                    ClientConnectionsManager.Instance.GetUserConnections(userId)
                        .ForEach(c => c.SendMessage(message));
                }
                catch (Exception error)
                {
                    logger.Error(error);
                }               
            }            
        }

        public void SendMessageToOnlineUsers(NotifyMessage message)
        {
            try
            {
                ClientConnectionsManager.Instance.GetAllConnections()
                    .ForEach(c => c.SendMessage(message));
            }
            catch(Exception error)
            {
                logger.Error(error);
            }
        }
    }
}
