using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;

namespace Blades.Interfaces
{
    public interface ILogger: IBladesService
    {
        void Debug(string message);

        void Info(string message);

        void Warning(string message);

        void Error(Exception error);

        void Error(Error error);
    }
}
