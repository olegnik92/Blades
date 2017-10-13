using System.Collections.Generic;

namespace Blades.Core
{
    public class ExecutionReport
    {
        public List<string> ReportStrings { get; set; } = new List<string>();

        public List<Error> Errors { get; set; } = new List<Error>();

        public List<DomainObjectInfo> AssociatedObjects { get; set; } = new List<DomainObjectInfo>();

        public List<ExecutionReport> SubReports { get; set; } = new List<ExecutionReport>();

        public ExecutionReport()
        {

        }

        public ExecutionReport(string reportString)
        {
            ReportStrings.Add(reportString);
        }
    }
}