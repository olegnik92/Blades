using Blades.Es;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesStartUp.Domain.TestEntity
{
    public class ChangeEvent: MutationEvent
    {
        public TestEntityState NewState { get; set; }
    }
}
