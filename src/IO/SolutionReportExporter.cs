using System.Text;

namespace Scheduling.IO;

public class SolutionReportExporter {
    public static string BuildCsvReport(Solution solution)
    {
        var totalEffort = solution.Instance.Modules.Sum(m => m.Effort);
        var report = new StringBuilder();

        var coveredModules = new HashSet<Module>();
        var reportEntries = new List<TimeReportEntry>();
        var time = 0;
        var totalFinishedCustomers = 0;
        foreach (var module in solution.Schedule.ScheduledModules)
        {
            time += module.Effort;
            coveredModules.Add(module);
            var newlyCoveredCustomers = solution.Instance.Customers.Where(c => c.Modules.Contains(module) && c.Modules.All(coveredModules.Contains));
            var numberOfFinishingCustomers = newlyCoveredCustomers.Sum(c => c.Weight);
            totalFinishedCustomers += numberOfFinishingCustomers;
            report.Append(string.Join(';', new []
            {
                $"{time}",
                $"{module.Name}",
                $"{numberOfFinishingCustomers}",
                $"{totalFinishedCustomers}",
                $"{numberOfFinishingCustomers * (totalEffort - time)}",
                string.Join(" ", newlyCoveredCustomers.Select(c => c.Name))
            }) + Environment.NewLine);
        }
        return report.ToString();
    }
}