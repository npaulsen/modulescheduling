using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Scheduling;

public class PreprocessorTests
{

    [Fact]
    public void Preprocess_JoinsCustomersWithIdenticalModules()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Take(1), 5) };
        var objectives = new[] { new DeadlineObjective(123) };
        var instance = new Instance(modules, customers, objectives);

        var output = Preprocessor.Preprocess(instance);

        Assert.Equal(instance.Modules, output.Modules);
        Assert.Equal(instance.Objectives, output.Objectives);
        Assert.Equal(new[] { new Customer("C1, C2", modules.Take(1), 6) }, output.Customers);
    }

    [Fact]
    public void Preprocess_ObjectivesAreIdentical()
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

        var output = Preprocessor.Preprocess(instance);

        Assert.Equal(new Solution(instance, schedule1).Values, new Solution(output, schedule1).Values);
        Assert.Equal(new Solution(instance, schedule2).Values, new Solution(output, schedule2).Values);
    }
}