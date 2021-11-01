global using static System.Console;
using Scheduling;

var instance = InstanceReader.FromFile("../../data/i3.txt");
WriteLine($"Instance with {instance.Customers.Count()} customers / {instance.Modules.Count()} modules /  total effort: {instance.Modules.Sum(m => m.Effort)}");

WriteLine("Preprocessing.");
instance = instance.WithAggregatedCustomers().WithAggregatedModules();
WriteLine($"Instance with {instance.Customers.Count()} customers / {instance.Modules.Count()} modules /  total effort: {instance.Modules.Sum(m => m.Effort)}");


WriteLine($"# customers containing other customers: {instance.Customers.Count(c1 => instance.Customers.Any(c2 => c1 != c2 && !c1.Modules.Except(c2.Modules).Any()))}");
WriteLine($"# customers containing other customers: {instance.Customers.Count(c1 => instance.Customers.Any(c2 => c1 != c2 && !c2.Modules.Except(c1.Modules).Any()))}");
WriteLine($"# customers with identical modules: {instance.Customers.Count(c1 => instance.Customers.Any(c2 => c1 != c2 && c1.Modules.SequenceEqual(c2.Modules)))}");

// PrintGraphviz(instance);

var dummy = new Solution(instance, new(instance.Modules));
var bestSolutionsForObjective = instance.Objectives.Select(_ => dummy).ToArray();

var limit = 3;
var rand = new Random();
for (int iteration = 0; iteration < limit; iteration++)
{
    var randomSol = Solution.FromCustomerOrder(instance, instance.Customers.OrderBy(c => c.Modules.Count * rand.NextDouble()));
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
PrintTimetable(bestSolutionsForObjective[^1]);

PrintObjectiveValues(instance, bestSolutionsForObjective);

WriteLine("Attempting to improve timevalue..");
var improved = Improver.OptimalSubsequencesSearch(bestSolutionsForObjective[^1]);
System.Console.WriteLine(improved.Schedule.ScheduledModules.Count());
System.Console.WriteLine(improved.Values);

PrintTimetable(improved);


static void PrintTimetable(Solution sol)
{
    foreach (var entry in sol.GenerateReport().Entries)
    {
        var customerString = string.Join(" ", entry.CoveredCustomers.Select(c => $"{c.Name}"));
        WriteLine($"T={entry.Time,3}  {entry.Module.Name,4}  {customerString}");
    }
}

static void PrintObjectiveValues(Instance instance, Solution[] solutions)
{
    for (var i = 0; i < instance.Objectives.Count(); i++)
    {
        var objective = instance.Objectives.ElementAt(i);
        var solution = solutions[i];
        WriteLine($"{i}: {objective} {solution.Values.Values[i]} (all: {solution.Values})");
    }
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