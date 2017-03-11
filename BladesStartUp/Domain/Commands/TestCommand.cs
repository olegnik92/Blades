using Blades.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesStartUp.Domain.Commands
{
    public class SaveTestEntityCommand : Command
    {
        public SaveTestEntityCommand() 
        {
            CommandName = "Сохранение тестовой сущности";
        }

        public int SomeData { get; set; }
    }
}
