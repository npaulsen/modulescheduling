
namespace Scheduling;

public class Improver
{
    public static Solution? OptimalSubsequencesSearch(Solution input, int subSequenceLength = 2, int maxCustomersInSubsequence = 8)
    {
        var instance = input.Instance;

        var coveredModules = new HashSet<Module>();

        var entriesWithFinishingCustomers = input.GenerateReport().Entries.Where(e => e.CoveredCustomers.Any()).ToList();

        for (int i = 0; i < entriesWithFinishingCustomers.Count; i++)
        {
            var subSequence = entriesWithFinishingCustomers.Skip(i).Take(subSequenceLength).SelectMany(e => e.CoveredCustomers);
            // TODO: Extend instance if preprocessable for customers
            var subSequenceInstance = GetSubInstance(subSequence, coveredModules);
            // var reducedSubInstance = subSequenceInstance.WithAggregatedCustomers();
            // if (reducedSubInstance.Customers.Count() < subSequenceInstance.Customers.Count())
            // {
            //     Console.WriteLine($"Could reduce customers from {subSequenceInstance.Customers.Count()} to {reducedSubInstance.Customers.Count()} customers");
            // }
            if (subSequenceInstance.Customers.Count() > maxCustomersInSubsequence)
            {
                Console.WriteLine($"Skipping sequence with {subSequenceInstance.Customers.Count()} customers (i={i})");
                continue;
            }
            var originalValue = subSequenceInstance.Objectives.First().CalculateValue(subSequenceInstance.Customers, new(subSequenceInstance.Modules));

            var subSequenceSolution = OptimalSolver.ExhaustiveSearch(subSequenceInstance);
            var delta = subSequenceSolution.Values.Values[0] - originalValue;
            if (delta > 0)
            {
                Console.WriteLine($"\t{nameof(OptimalSubsequencesSearch)} improved on subsequence with {subSequence.Count()} customers, i={i}, ð={delta}");
                var newSubSequence = subSequenceSolution
                    .GenerateReport()
                    .Entries
                    .SelectMany(e => e.CoveredCustomers)
                    .ToList();
                var customerSequence = entriesWithFinishingCustomers.SelectMany(e => e.CoveredCustomers.ToList()).ToList();

                var newCustomerOrder = entriesWithFinishingCustomers
                    .Take(i)
                    .SelectMany(e => e.CoveredCustomers)
                    .Concat(newSubSequence
                        .Select(nc => customerSequence.Find(c => c.Name == nc.Name)))
                    .Concat(entriesWithFinishingCustomers
                        .Skip(i + subSequenceLength)
                        .SelectMany(e => e.CoveredCustomers));

                var newSolution = Solution.FromCustomerOrder(instance, newCustomerOrder!);
                var solutionDelta = newSolution.Values.Values[^1] - input.Values.Values[^1];
                if (solutionDelta > delta)
                {
                    Console.WriteLine($"\tsolution-ð is even {solutionDelta}!");
                }
                else if (solutionDelta != delta)
                {
                    Console.WriteLine($"\tsolution-ð is only {solutionDelta}!");
                    if (solutionDelta <= 0)
                    {
                        Console.WriteLine("\t-> ignoring result.");
                        continue;
                    }

                    // Console.WriteLine($"Covered modules: {string.Join(" ", coveredModules.Select(m => m.Name))}");
                    // Console.WriteLine($"original sequence: {string.Join(" - ", customerSequence.Select(c => c.Name))}");
                    // Console.WriteLine($"original subsequence: {string.Join(" - ", subSequence.Select(c => c.Name))}");
                    // Console.WriteLine($"{originalValue} => {subSequenceSolution.Values.Values[0]} [{subSequenceSolution.Values.Values[0] - originalValue}]");
                    // Console.WriteLine($"new subsequence: {string.Join(" - ", newSubSequence.Select(c => c.Name))}");
                    // Console.WriteLine($"new sequence: {string.Join(" - ", newCustomerOrder.Select(c => c.Name))}");
                    // PrintTimetable(subSequenceSolution);
                    // Console.WriteLine($"{newSolution.Values} [{solutionDelta}] i={i}");
                    // return newSolution;
                }
                return newSolution;
            }

            foreach (var module in entriesWithFinishingCustomers[i].CoveredCustomers.SelectMany(c => c.Modules))
            {
                coveredModules.Add(module);
            }
        }
        return null;
    }

    private static Instance GetSubInstance(IEnumerable<Customer> customers, HashSet<Module> alreadyCoveredModules)
    {
        var newCustomers = customers
            .Select(c => new Customer(c.Name, c.Modules.Where(m => !alreadyCoveredModules.Contains(m)).ToValueList(), c.Weight));
        var newModules = newCustomers.SelectMany(c => c.Modules).Distinct();
        return new(newModules, newCustomers, new[] { new TimeValueObjective(newCustomers, newModules) });
    }

    static void PrintTimetable(Solution sol)
    {
        foreach (var entry in sol.GenerateReport().Entries)
        {
            var customerString = string.Join(" ", entry.CoveredCustomers.Select(c => $"{c.Name}"));
            Console.WriteLine($"T={entry.Time,3}  {entry.Module.Name,4}  {customerString}");
        }
    }
}