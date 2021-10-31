namespace Scheduling;

public class Preprocessor
{
    public static Instance Preprocess(Instance input)
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

        var newCustomers = identicalCustomers.Select(cluster => new Customer(string.Join(", ", cluster.Select(c => c.Name)), cluster.First().Modules, cluster.Sum(c => c.Weight)));
        var objectives = input.Objectives.Select(objective => objective switch
        {
            TimeValueObjective t => new TimeValueObjective(newCustomers, input.Modules),
            _ => objective
        });
        return new Instance(input.Modules, newCustomers, objectives);
    }
}