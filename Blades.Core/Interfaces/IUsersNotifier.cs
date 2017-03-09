using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;

namespace Blades.Interfaces
{
    public interface IUsersNotifier: IBladesService
    {
        void SendMessage(Guid userId, NotifyMessage message, bool useGuaranteedDelivery = false);

        void SendMessageToOnlineUsers(NotifyMessage message);
    }
}
