using Blades.Es;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesStartUp.Domain.UsersList
{
    public class ChangeUserEvent : MutationEvent
    {
        public UserBase User;
    }
}
