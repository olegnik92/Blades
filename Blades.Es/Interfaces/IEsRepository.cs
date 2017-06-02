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
        TState GetLastSnapshot<TState>(Resource resource, byte detalizationLevel, out ulong version);

        List<MutationEvent> GetEvents(Resource resource, byte detalizationLevel, ulong startVersion);

        ulong GetVersion(Resource resource);

        List<Guid> GetAllInstanceIds(Guid resourceTypeId);

        void PushSnapshot<TState>(Resource resource, TState state, byte detalizationLevel, ulong version);

        void PushEvent(Resource resource, MutationEvent mutation);
    }
}
