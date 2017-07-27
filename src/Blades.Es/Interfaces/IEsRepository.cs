using Blades.Core;
using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Es.Interfaces
{
    public interface IEsRepository: IBladesService
    {
        TState GetLastSnapshot<TState>(Resource resource, out List<ulong> versions);

        List<MutationEvent> GetEvents(Resource resource, byte detalizationLevel, List<ulong> startVersions);

        List<Guid> GetAllInstanceIds(Guid resourceTypeId);

        void PushSnapshot<TState>(Resource resource, TState state, List<ulong> versions);

        void PushEvent(Resource resource, MutationEvent mutation);
    }
}
