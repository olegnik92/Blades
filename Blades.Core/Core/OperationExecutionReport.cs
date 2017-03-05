using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public class OperationExecutionReport
    {
        public string ReportString { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public List<Resource> AssociatedResources { get; set; } = new List<Resource>();

        public void InsertChildReport(OperationExecutionReport report)
        {
            Errors.AddRange(report.Errors);
            AssociatedResources.AddRange(report.AssociatedResources);
            ReportString += $@"
                Внутренняя операция:
                {report.ReportString}
            ";
        }
    }
}
