global using static System.Console;
using Scheduling;

var instance = InstanceReader.FromFile("../../data/i1.txt");
WriteLine($"Instance with {instance.Customers.Count()} customers / {instance.Modules.Count()} modules");

var dummy = new Solution(instance, new (instance.Modules));
var bestSolutionsForObjective = instance.Objectives.Select(_ => dummy).ToArray();

var limit = 1000;
var rand = new Random();
for (int iteration = 0; iteration < limit; iteration++)
{
    var randomSol = new Solution(instance, new(instance.Modules.OrderBy(_ => rand.Next())));
    WriteLine(randomSol.Values);
    for (var i = 0; i < instance.Objectives.Count(); i++) {
        var currentValue = randomSol.Values.Values[i];
        if (currentValue > bestSolutionsForObjective[i].Values.Values[i]) {
            bestSolutionsForObjective[i] = randomSol;
        }
    }
}
foreach (var solution in bestSolutionsForObjective)
{
    foreach (var entry in solution.GenerateReport().Entries) 
    {
        PrintEntry(entry);
    }
    WriteLine(solution.Values);
}


static void PrintEntry(TimeReportEntry entry) {
    var customerString = string.Join(" ", entry.CoveredCustomers.Select(c => $"C{c.Name}"));
    WriteLine($"T={entry.Time,3}  M{entry.Module.Name,3}  {customerString}");
}