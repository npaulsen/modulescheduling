using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Scheduling;

public class SolutionTests
{
    private record FixedValueDummyObjective(int Value) : Objective
    {
        public override int CalculateValue(IEnumerable<Customer> _, ModuleSchedule __) => Value;
    }

    [Fact]
    public void Solution_GetsValueFromObjectives()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Skip(1)) };
        var objectives = new[] { new FixedValueDummyObjective(123), new FixedValueDummyObjective(456) };
        var instance = new Instance(modules, customers, objectives);
        var schedule = new ModuleSchedule(modules);

        var solution = new Solution(instance, schedule);

        Assert.Equal(new ObjectiveValues(123, 456), solution.Values);
    }

    [Fact]
    public void Solution_GeneratesAReport()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Skip(1).Take(1)), new("C2", modules.Take(2)) };
        var objectives = new[] { new DeadlineObjective(2) };
        var instance = new Instance(modules, customers, objectives);
        var schedule = new ModuleSchedule(modules);
        var solution = new Solution(instance, schedule);

        var report = solution.GenerateReport();

        var expectedReport = new Report(new TimeReportEntry[] {
            new(1, modules[0], Enumerable.Empty<Customer>().ToValueList()),
            new(2, modules[1], customers.ToValueList()),
            new(3, modules[2], Enumerable.Empty<Customer>().ToValueList()),
        }.ToValueList());
        Assert.Equal(expectedReport, report);
    }
}