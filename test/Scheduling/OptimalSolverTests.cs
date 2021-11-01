using Xunit;
using Scheduling;
using System.Linq;

public class OptimalSolverTests
{

    [Fact]
    public void ExhaustiveSearch_WhenModuleRequirementsIncrease_ReturnsOptimalValue()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Take(2)), new("C3", modules.Take(3)) };
        var instance = new Instance(modules, customers, new[] { new TimeValueObjective(customers, modules) });

        var solution = OptimalSolver.ExhaustiveSearch(instance);

        Assert.Equal(3, solution.Values.Values[0]);
    }

    [Fact]
    public void ExhaustiveSearch_ReturnsOptimalValue()
    {
        var modules = new Module[] { new("M1", 2), new("M2"), new("M3") };
        var customers = new Customer[] { new("C1", modules.Take(1)), new("C2", modules.Take(2)), new("C3", modules.Take(3)), new("C4", modules.Skip(1).Take(1)) };
        var instance = new Instance(modules, customers, new[] { new TimeValueObjective(customers, modules) });

        var solution = OptimalSolver.ExhaustiveSearch(instance);

        Assert.Equal(5, solution.Values.Values[0]);
    }
}