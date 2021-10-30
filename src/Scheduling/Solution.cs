using System.Collections.Immutable;

namespace Scheduling;

public record ModuleSchedule(IEnumerable<Module> ScheduledModules);

public record ObjectiveValues(ImmutableListWithValueSemantics<int> Values)
{
    public ObjectiveValues(params int[] values)
        : this(values as IEnumerable<int>) { }

    public ObjectiveValues(IEnumerable<int> values)
        : this(values.ToValueList()) { }
}

public class Solution
{
    public readonly ModuleSchedule Schedule;
    public readonly Instance Instance;

    public readonly ObjectiveValues Values;

    public Solution(Instance instance, ModuleSchedule schedule)
    {
        Schedule = schedule;
        Instance = instance;
        Values = CalculateValues();
    }

    private ObjectiveValues CalculateValues() =>   new ObjectiveValues(Instance.Objectives.Select(objective => {
        var coveredModules = new HashSet<Module>(Schedule.ScheduledModules.Take(objective.Deadline));
        return Instance.Customers.Count(c => c.Modules.All(coveredModules.Contains));
    }));

    public Report GenerateReport()
    {
        var coveredModules = new HashSet<Module>();
        var reportEntries = new List<TimeReportEntry>();
        foreach(var (module, timeIndex) in Schedule.ScheduledModules.Select((m,i) => (m,i)))
        {
            var time = timeIndex + 1;
            coveredModules.Add(module);
            var newlyCoveredCustomers = Instance.Customers.Where(c => c.Modules.Contains(module) && c.Modules.All(coveredModules.Contains));
            reportEntries.Add(new TimeReportEntry(time, module, newlyCoveredCustomers.ToValueList())); 
        }
        return new Report(reportEntries.ToValueList());
    }
}

public record Report(ImmutableListWithValueSemantics<TimeReportEntry> Entries);

public record TimeReportEntry(int Time,Module Module, ImmutableListWithValueSemantics<Customer> CoveredCustomers);



