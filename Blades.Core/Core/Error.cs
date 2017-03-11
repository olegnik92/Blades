using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public class Error
    {
        public Guid Id { get; set; }

        public DateTime Time { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public Error InnerError;

        public Error()
        {
            Id = Guid.NewGuid();
            Time = DateTime.Now;
        }

        public Error(Exception err): this()
        {
            Message = err.Message;
            StackTrace = err.StackTrace;
            if(err.InnerException != null)
            {
                InnerError = new Error(err.InnerException);
            }
        }

        public Error(string message): this()
        {
            Message = message;
        }
    }
}
