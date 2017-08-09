using Blades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blades.DataStore.Interfaces
{
    public interface IEsRepository
    {
        TState GetLastSnapshot<TState>(Resource resource, out ulong version);

        List<MutationEvent> GetEvents(Resource resource, ulong startVersion);

        void PushSnapshot<TState>(Resource resource, TState state, ulong version);

        void PushEvent(Resource resource, MutationEvent mutation);
    }
}
