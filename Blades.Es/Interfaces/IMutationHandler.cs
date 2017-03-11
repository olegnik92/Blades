using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Es.Interfaces
{
    public interface IMutationHandler<TState, TEvent> where TEvent: MutationEvent
    {
        TState Apply(TState state, TEvent mutation);
    }
}
