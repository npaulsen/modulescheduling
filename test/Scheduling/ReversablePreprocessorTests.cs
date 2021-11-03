
using Xunit;
using Scheduling;
using System.Linq;

public class ReversablePreprocessorTests
{
    [Fact]
    public void GetOriginalCustomersForCustomersOfPreprocessedInstance_ReturnsOriginalCustomers()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("CC1", modules.Take(1)), new("CC2", modules.Take(2)), new("CC3", modules.Take(3)) };
        var instance = new Instance(modules, customers, new[] { new TimeValueObjective(customers, modules) });
        var sut = new ReversablePreprocessor(instance);

        var preprocessed = sut.PreprocessedInstance;
        var restoredCustomers = sut.GetOriginalCustomers(preprocessed.Customers);

        Assert.Equal(instance.Customers, restoredCustomers);
    }

     [Fact]
    public void GetOriginalCustomersWorksWhenCustomersArePreprocessed()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("CC1", modules.Take(1)), new("CC2", modules.Take(1)), new("CC3", modules.Take(3)) };
        var instance = new Instance(modules, customers, new[] { new TimeValueObjective(customers, modules) });
        var sut = new ReversablePreprocessor(instance);

        var preprocessed = sut.PreprocessedInstance;
        var restoredCustomers = sut.GetOriginalCustomers(preprocessed.Customers);

        Assert.Equal(instance.Customers, restoredCustomers);
    }

    [Fact]
    public void PreprocessedInstanceIsSmaller()
    {
        var modules = new Module[] { new("M1"), new("M2"), new("M3") };
        var customers = new Customer[] { new("CC1", modules.Take(1)), new("CC2", modules.Take(1)), new("CC3", modules.Take(3)) };
        var instance = new Instance(modules, customers, new[] { new TimeValueObjective(customers, modules) });
       
        var sut = new ReversablePreprocessor(instance);
        var preprocessed = sut.PreprocessedInstance;

        Assert.True(instance.Customers.Count() > preprocessed.Customers.Count());
    }
}