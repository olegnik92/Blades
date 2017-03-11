using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Es;

namespace BladesStartUp.Domain.UsersList
{
    public class UsersListState
    {
        public List<UserBase> AllUsers { get; set; } = new List<UserBase>();

        public List<UserBase> ActiveUsers { get; set; } = new List<UserBase>();
    }

}
