using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blades.DataStore
{
    public class CreateJournalItem: MutationEvent
    {
        public object InitialState { get; set; }
    }
}
