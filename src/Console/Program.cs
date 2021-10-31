global using static System.Console;
using Scheduling;

var instance = InstanceReader.FromFile("../../data/i3.txt");
WriteLine($"Instance with {instance.Customers.Count()} customers / {instance.Modules.Count()} modules /  total effort: {instance.Modules.Sum(m => m.Effort)}");

WriteLine("Preprocessing.");
instance = Preprocessor.Preprocess(instance);
WriteLine($"Instance with {instance.Customers.Count()} customers / {instance.Modules.Count()} modules /  total effort: {instance.Modules.Sum(m => m.Effort)}");


WriteLine($"# customers containing other customers: {instance.Customers.Count(c1 => instance.Customers.Any(c2 => c1 != c2 && !c1.Modules.Except(c2.Modules).Any()))}");
WriteLine($"# customers containing other customers: {instance.Customers.Count(c1 => instance.Customers.Any(c2 => c1 != c2 && !c2.Modules.Except(c1.Modules).Any()))}");
WriteLine($"# customers with identical modules: {instance.Customers.Count(c1 => instance.Customers.Any(c2 => c1 != c2 && c1.Modules.SequenceEqual(c2.Modules)))}");

// PrintGraphviz(instance);

var dummy = new Solution(instance, new(instance.Modules));
var bestSolutionsForObjective = instance.Objectives.Select(_ => dummy).ToArray();

var limit = 100;
var rand = new Random();
for (int iteration = 0; iteration < limit; iteration++)
{
    var randomSol = SolveBasedOnCustomerOrder(instance, instance.Customers.OrderBy(c => c.Modules.Count * rand.NextDouble()));
    // WriteLine(randomSol.Values);
    for (var i = 0; i < instance.Objectives.Count(); i++)
    {
        var currentValue = randomSol.Values.Values[i];
        if (currentValue > bestSolutionsForObjective[i].Values.Values[i])
        {
            bestSolutionsForObjective[i] = randomSol;
        }
    }
}

PrintObjectiveValues(instance, bestSolutionsForObjective);

static void PrintObjectiveValues(Instance instance, Solution[] solutions)
{
    for (var i = 0; i < instance.Objectives.Count(); i++)
    {
        var objective = instance.Objectives.ElementAt(i);
        var solution = solutions[i];
        WriteLine($"{i}: {objective} {solution.Values.Values[i]} (all: {solution.Values})");
    }
}

static Solution SolveBasedOnCustomerOrder(Instance instance, IEnumerable<Customer> customers)
{
    var modules = new List<Module>();
    foreach (var customer in customers)
    {
        modules.AddRange(customer.Modules.Where(m => !modules.Contains(m)));
    }
    return new(instance, new(modules));
}


void PrintGraphviz(Instance instance)
{
    WriteLine("digraph G {");

    var identicalCustomers = new List<List<Customer>>();
    foreach (var customer in instance.Customers)
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

    var newCustomers = identicalCustomers.Select(cluster => new Customer(string.Join(", ", cluster.Select(c => c.Name)), cluster.First().Modules));

    var subsets = newCustomers.ToDictionary(c => c, c1 => new HashSet<Customer>(newCustomers.Where(c2 => c1 != c2 && !c1.Modules.Except(c2.Modules).Any())));

    foreach (var (c1, others) in subsets)
    {
        // WriteLine($"{c1.Name}: {string.Join(",", others.Select(c => c.Name))}");
        foreach (var c2 in others.Where(c2 => others.All(c3 => !subsets[c3].Contains(c2))))
        {
            WriteLine($"  \"{c1.Name}\" -> \"{c2.Name}\"");
        }
    }
    WriteLine("}");
}