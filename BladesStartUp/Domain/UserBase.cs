using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesStartUp.Domain
{
    public class UserBase
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public String Login { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}
