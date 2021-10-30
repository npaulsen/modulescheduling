global using static System.Console;
using Scheduling;

var instance = InstanceReader.FromFile("../../data/i1.txt");
WriteLine($"Instance with {instance.Customers.Count()} customers / {instance.Modules.Count()} modules");

var dummy = new Solution(instance, new(instance.Modules));
var bestSolutionsForObjective = instance.Objectives.Select(_ => dummy).ToArray();

var limit = 1000;
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
foreach (var solution in bestSolutionsForObjective)
{
    // PrintTimeTable(solution);
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