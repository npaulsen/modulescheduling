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

    private ObjectiveValues CalculateValues()
    => new(Instance.Objectives.Select(objective => objective.CalculateValue(Instance.Customers, Schedule)));

    public Report GenerateReport()
    {
        var coveredModules = new HashSet<Module>();
        var reportEntries = new List<TimeReportEntry>();
        var time = 0;
        foreach (var module in Schedule.ScheduledModules)
        {
            time += module.Effort;
            coveredModules.Add(module);
            var newlyCoveredCustomers = Instance.Customers.Where(c => c.Modules.Contains(module) && c.Modules.All(coveredModules.Contains));
            reportEntries.Add(new TimeReportEntry(time, module, newlyCoveredCustomers.ToValueList()));
        }
        return new Report(reportEntries.ToValueList());
    }

    public static Solution FromCustomerOrder(Instance instance, IEnumerable<Customer> customers)
    {
        var modules = new List<Module>();
        foreach (var customer in customers)
        {
            modules.AddRange(customer.Modules.Where(m => !modules.Contains(m)));
        }
        return new(instance, new(modules));
    }
}

public record Report(ImmutableListWithValueSemantics<TimeReportEntry> Entries);

public record TimeReportEntry(int Time, Module Module, ImmutableListWithValueSemantics<Customer> CoveredCustomers);



