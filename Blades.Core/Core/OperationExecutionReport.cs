using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public class OperationExecutionReport
    {
        public List<string> ReportStrings { get; set; } = new List<string>();

        public List<string> Errors { get; set; } = new List<string>();

        public List<Resource> AssociatedResources { get; set; } = new List<Resource>();

        public List<OperationExecutionReport> SubReports { get; private set; } = new List<OperationExecutionReport>();

        public OperationExecutionReport()
        {

        }

        public OperationExecutionReport(string reportString)
        {
            ReportStrings.Add(reportString);
        }
    }
}
