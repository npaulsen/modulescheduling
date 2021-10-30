using System;
using System.Linq;
using Xunit;

namespace Scheduling;

public class DeadlineObjectiveTests
{
    [Fact]
    public void WhenTheDeadlineIsZero_HasObjectiveValueZero()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Skip(1)) };
        var schedule = new ModuleSchedule(modules);
        var sut = new DeadlineObjective(0);

        var val = sut.CalculateValue(customers, schedule);

        Assert.Equal(0, val);
    }

    [Fact]
    public void WhenTheDeadlineIsThree_HasObjectiveValueTwo()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Skip(1)) };
        var schedule = new ModuleSchedule(modules);
        var sut = new DeadlineObjective(3);

        var val = sut.CalculateValue(customers, schedule);

        Assert.Equal(2, val);
    }

    [Fact]
    public void DoesntCountCustomerWithPartialModuleCoverage()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Skip(1)) };
        var schedule = new ModuleSchedule(modules);
        var sut = new DeadlineObjective(2);

        var val = sut.CalculateValue(customers, schedule);

        Assert.Equal(1, val);
    }
}