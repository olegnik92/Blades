using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public class NotifyMessage
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string MessageRole => "NotifyMessage";
    }


    public class NotifyMessage<TData> : NotifyMessage
    {
        public TData Data { get; set; }
    }

    public class NotifyMessage<TData, TAttrs> : NotifyMessage<TData>
    {
        public TAttrs Attributes { get; set; }
    }

}
