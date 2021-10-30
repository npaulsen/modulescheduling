using System;
using System.Linq;
using Xunit;

namespace Scheduling;

public class SolutionTests
{
    [Fact]
    public void Solution_WhenTheDeadlineIsZero_HasObjectiveValueZero()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Skip(1)) };
        var objectives = new[] { new Objective(0) };
        var instance = new Instance(modules, customers, objectives);
        var schedule = new ModuleSchedule(modules);

        var solution = new Solution(instance, schedule);

        Assert.Equal(new ObjectiveValues(0), solution.Values);
    }

    [Fact]
    public void Solution_WhenTheInstanceProvidesTwoObjectives_HasTwoObjectiveValues()
    {
        var instance = new Instance(Array.Empty<Module>(), Array.Empty<Customer>(), new Objective[] { new(0), new(0) });

        var solution = new Solution(instance, new ModuleSchedule(Array.Empty<Module>()));

        Assert.Equal(2, solution.Values.Values.Count);
    }

    [Fact]
    public void Solution_WhenTheDeadlineIsThree_HasObjectiveValueTwo()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Skip(1)) };
        var objectives = new[] { new Objective(3) };
        var instance = new Instance(modules, customers, objectives);
        var schedule = new ModuleSchedule(modules);

        var solution = new Solution(instance, schedule);

        Assert.Equal(new ObjectiveValues(2), solution.Values);
    }

    [Fact]
    public void Solution_DoesntCountCustomerWithPartialModuleCoverage()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Skip(1)) };
        var objectives = new[] { new Objective(2) };
        var instance = new Instance(modules, customers, objectives);
        var schedule = new ModuleSchedule(modules);

        var solution = new Solution(instance, schedule);

        Assert.Equal(new ObjectiveValues(1), solution.Values);
    }

    [Fact]
    public void Solution_GeneratesAReport()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Skip(1).Take(1)), new("C2", modules.Take(2)) };
        var objectives = new[] { new Objective(2) };
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