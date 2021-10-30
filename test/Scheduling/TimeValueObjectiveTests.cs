using System;
using System.Linq;
using Xunit;

namespace Scheduling;

public class TimeValueObjectiveTests
{
    [Fact]
    public void WhenACustomerIsCovered3UnitsBeforeEnd_ValueIsThree()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3"), new("M4") };
        var customers = new Customer[] { new("C1", modules.Take(1)) };
        var schedule = new ModuleSchedule(modules);
        var sut = new TimeValueObjective();

        var val = sut.CalculateValue(customers, schedule);

        Assert.Equal(3, val);
    }

    [Fact]
    public void WhenACustomerIsCovered3UnitsBeforeEndAndAnotherIsOne_ValueIsFour()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3"), new("M4") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Take(3)) };
        var schedule = new ModuleSchedule(modules);
        var sut = new TimeValueObjective();

        var val = sut.CalculateValue(customers, schedule);

        Assert.Equal(4, val);
    }
}