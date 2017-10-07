using Blades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blades.DataStore.Interfaces
{
    public interface IEsRepository
    {
        TState GetLastSnapshot<TState>(ResourceInfo resource, out ulong version);

        List<MutationEvent> GetEvents(ResourceInfo resource, ulong startVersion);

        void PushSnapshot<TState>(ResourceInfo resource, TState state, ulong version);

        void PushEvent(ResourceInfo resource, MutationEvent mutation);
    }
}
