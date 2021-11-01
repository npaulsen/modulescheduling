namespace Scheduling;

public static class PreprocessingInstanceExtensions
{
    public static Instance WithAggregatedCustomers(this Instance input)
    {
        var identicalCustomers = new List<List<Customer>>();
        foreach (var customer in input.Customers)
        {
            var identicals = identicalCustomers.Find(l => l.Any(c2 => c2.Modules.SequenceEqual(customer.Modules)));
            if (identicals is not null)
            {
                identicals.Add(customer);
            }
            else
            {
                identicalCustomers.Add(new() { customer });
            }
        }

        var newCustomers = identicalCustomers
            .Select(cluster => new Customer(
                string.Join(", ", cluster.Select(c => c.Name)),
                cluster.First().Modules,
                cluster.Sum(c => c.Weight)));
        var objectives = input.Objectives.Select(objective => objective switch
        {
            TimeValueObjective t => new TimeValueObjective(newCustomers, input.Modules),
            _ => objective
        });
        return new Instance(input.Modules, newCustomers, objectives);
    }

    public static Instance WithAggregatedModules(this Instance input)
    {
        var customersForModule = input.Modules.ToDictionary(m => m, _ => new List<Customer>());
        foreach (var customer in input.Customers)
        {
            foreach (var module in customer.Modules)
            {
                customersForModule[module].Add(customer);
            }
        }
        var modulesWithTheirUsage = input.Modules.GroupBy(m => customersForModule[m].ToValueList());
        var moduleMap = modulesWithTheirUsage.ToDictionary(moduleGroup => moduleGroup.First(),
            moduleGroup => new Module(string.Join(",", moduleGroup.Select(m => m.Name)), moduleGroup.Sum(m => m.Effort)));
        var newModules = moduleMap.Values;

        var newCustomers = input.Customers.Select(c => new Customer(c.Name, c.Modules.Where(m => moduleMap.ContainsKey(m)).Select(m => moduleMap[m]), c.Weight));

        var objectives = input.Objectives.Select(objective => objective switch
        {
            TimeValueObjective t => new TimeValueObjective(newCustomers, newModules),
            _ => objective
        });
        return new Instance(newModules, newCustomers, objectives);
    }
}