using Xunit;
using System.Collections.Generic;
using Scheduling;
using System;
using System.Linq;

namespace IO;

public class InstanceReaderTests
{
    [Fact]
    public void FromLines_ReadsObjectives()
    {
        var lines = new[] {
            "Objective Deadline 10",
            "Objective Deadline 11",
            "Module module1 3",
            "Customer 1 module1"
        };

        var inst = InstanceReader.FromLines(lines);

        var expectedObjectives = new List<Objective> {
            new DeadlineObjective(10),
            new DeadlineObjective(11)
        };
        Assert.Equal(expectedObjectives, inst.Objectives);
    }

    [Fact]
    public void FromLines_WhenObjectiveIsTimeValue_ReturnsInstanceWithTimevalueObjective()
    {
        var lines = new[] {
            "Objective Timevalue",
            "Customer 1 1"
        };

        var inst = InstanceReader.FromLines(lines);

        Assert.IsType<TimeValueObjective>(inst.Objectives.First());
    }

    [Fact]
    public void FromLines_WhenOneCustomerHasOneModule_ReturnsInstanceWithOneCustomerAndOneModule()
    {
        var lines = new[] {
            "Objective Deadline 999",
            "Customer customer module"
        };

        var inst = InstanceReader.FromLines(lines);
        var expectedModule = new Module("module");
        var expectedCustomer = new Customer("customer", new[] { expectedModule });

        Assert.Equal(new[] { expectedCustomer }, inst.Customers);
        Assert.Equal(new[] { expectedModule }, inst.Modules);
    }

    [Fact]
    public void FromLines_WhenOneCustomerHasMultipleModules_ReturnsInstanceWithOneCustomerAndMultipleModules()
    {
        var lines = new[] {
            "Objective Deadline 999",
            "Customer c1 m1",
            "Customer c1 m2"
        };

        var inst = InstanceReader.FromLines(lines);

        var expectedCustomer = new Customer("c1", new Module[] { new("m1"), new("m2") });
        var expectedModules = new Module[] { new("m1"), new("m2") };
        Assert.Equal(new[] { expectedCustomer }, inst.Customers);
        Assert.Equal(expectedModules, inst.Modules);
    }

    [Fact]
    public void FromLines_WhenOneCustomerHasMultipleModulesInOneLine_ReturnsInstanceWithOneCustomerAndMultipleModules()
    {
        var lines = new[] {
            "Objective Deadline 999",
            "Customer c1 m1 m2",
        };

        var inst = InstanceReader.FromLines(lines);

        var expectedCustomer = new Customer("c1", new Module[] { new("m1"), new("m2") });
        var expectedModules = new Module[] { new("m1"), new("m2") };
        Assert.Equal(new[] { expectedCustomer }, inst.Customers);
        Assert.Equal(expectedModules, inst.Modules);
    }

    [Fact]
    public void FromLines_WhenOneModuleIsNeededByMultipleCustomer_ReturnsInstanceWithMultipleCustomersAndOneModule()
    {
        var lines = new[] {
            "Objective Deadline 999",
            "Customer c1 m1",
            "Customer c2 m1"
        };

        var inst = InstanceReader.FromLines(lines);

        var expectedCustomers = new Customer[] {
            new ("c1", new Module[] { new("m1") }),
            new ("c2", new Module[] { new("m1") }),
        };
        var expectedModule = new Module[] { new("m1") };
        Assert.Equal(expectedCustomers, inst.Customers);
        Assert.Equal(expectedModule, inst.Modules);
    }

    [Fact]
    public void FromLines_ParsesModuleEfforts()
    {
        var lines = new[] {
            "Objective Deadline 999",
            "Module m1 5",
            "Module m2 1",
            "Customer c1 m1",
            "Customer c1 m2"
        };

        var inst = InstanceReader.FromLines(lines);

        var expectedCustomers = new Customer[] {
            new ("c1", new Module[] { new("m1", 5), new("m2") }),
        };
        var expectedModule = new Module[] { new("m1", 5), new("m2") };
        Assert.Equal(expectedCustomers, inst.Customers);
        Assert.Equal(expectedModule, inst.Modules);
    }
}