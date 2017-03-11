using Blades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Commands
{
    public class Command
    {
        public Guid Id { get; set; }

        public string CommandName { get; set; }

        public DateTime Time { get; set; }

        public UserInfo User { get; set; }

        public List<OperationExecutionReport> ExecutionReports { get; private set; } 

        public Command()
        {
            CommandName = "Команда без имени";
        }

        public void Init(UserInfo user)
        {
            Id = Guid.NewGuid();
            Time = DateTime.Now;
            User = user;
            ExecutionReports = new List<OperationExecutionReport>();
        }
    }
}
