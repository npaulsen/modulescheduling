using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Scheduling;

public class PreprocessorTests
{

    [Fact]
    public void WithAggregatedCustomers_JoinsCustomersWithIdenticalModules()
    {
        var modules = new Module[] { new("M1"), new("M2") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Take(1), 5) };
        var objectives = new[] { new DeadlineObjective(123) };
        var instance = new Instance(modules, customers, objectives);

        var output = instance.WithAggregatedCustomers();

        Assert.Equal(instance.Modules, output.Modules);
        Assert.Equal(instance.Objectives, output.Objectives);
        Assert.Equal(new[] { new Customer("C1, C2", modules.Take(1), 6) }, output.Customers);
    }

    [Fact]
    public void WithAggregatedModules_JoinsModulesThatAreAlwaysTogether()
    {
        var modules = new Module[] { new("M1"), new("M2", 3), new("M3") };
        var customers = new Customer[] {
            new("C1", modules.Take(2)),
            new("C2", modules.Take(3))
            };
        var objectives = new[] { new DeadlineObjective(123) };
        var instance = new Instance(modules, customers, objectives);

        var output = instance.WithAggregatedModules();

        var joinedModule = new Module("M1,M2", 4);
        var expectedModules = new[] { joinedModule, new("M3") };
        var expectedCustomers = new Customer[] {
            new("C1", new[] { joinedModule }),
            new("C2", new[] { joinedModule, new("M3") })
        };
        Assert.Equal(expectedModules, output.Modules);
        Assert.Equal(expectedCustomers, output.Customers);
        Assert.Equal(instance.Objectives, output.Objectives);
    }

    [Fact]
    public void WithAggregatedModules_DoesntJoinModulesIfTheyCanOccurAlone()
    {
        var modules = new Module[] { new("M1"), new("M2", 3), new("M3") };
        var customers = new Customer[] {
            new("C1", modules.Take(1)),
            new("C2", modules.Take(2)),
            new("C3", modules.Take(3))
            };
        var objectives = new[] { new DeadlineObjective(123) };
        var instance = new Instance(modules, customers, objectives);

        var output = instance.WithAggregatedModules();

        Assert.Equal(instance.Modules, output.Modules);
        Assert.Equal(instance.Customers, output.Customers);
        Assert.Equal(instance.Objectives, output.Objectives);
    }

    [Fact]
    public void WithAggregatedModules_ObjectivesAreIdentical()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] {
            new("C1", modules.Take(1), 100),
            new("C2", modules.Take(1), 77),
            new("C3", modules.Take(2), 19),
            new("C4", modules.Skip(2).Take(1))
        };
        var objectives = new[] { new TimeValueObjective(customers, modules) };
        var instance = new Instance(modules, customers, objectives);
        var schedule1 = new ModuleSchedule(modules);
        var schedule2 = new ModuleSchedule(modules.Reverse());

        var output = instance.WithAggregatedModules();

        Assert.Equal(new Solution(instance, schedule1).Values, new Solution(output, schedule1).Values);
        Assert.Equal(new Solution(instance, schedule2).Values, new Solution(output, schedule2).Values);
    }
}