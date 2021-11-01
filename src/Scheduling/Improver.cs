
namespace Scheduling;

public class Improver
{
    public static Solution OptimalSubsequencesSearch(Solution input, int subSequenceLength = 2)
    {
        var instance = input.Instance;
        var customerSequence = input
            .GenerateReport()
            .Entries
            .SelectMany(e => e.CoveredCustomers)
            .ToList();

        var coveredModules = new HashSet<Module>();



        for (int i = 0; i < customerSequence.Count - subSequenceLength; i++)
        {
            var subSequence = customerSequence.Skip(i).Take(subSequenceLength);
            // TODO: Extend instance if preprocessable for customers
            var subSequenceInstance = GetSubInstance(subSequence, coveredModules);
            var originalValue = subSequenceInstance.Objectives.First().CalculateValue(subSequenceInstance.Customers, new(subSequenceInstance.Modules));

            var subSequenceSolution = OptimalSolver.ExhaustiveSearch(subSequenceInstance);
            if (subSequenceSolution.Values.Values[0] > originalValue)
            {
                Console.WriteLine($"Covered modules: {string.Join(" ", coveredModules.Select(m => m.Name))}");
                PrintTimetable(subSequenceSolution);
                Console.WriteLine($"original subsequence: {string.Join(" - ", subSequence.Select(c => c.Name))}");
                Console.WriteLine($"{originalValue} => {subSequenceSolution.Values.Values[0]} [{subSequenceSolution.Values.Values[0] - originalValue}]");
                var newSubSequence = subSequenceSolution
                    .GenerateReport()
                    .Entries
                    .SelectMany(e => e.CoveredCustomers)
                    .ToList();
                Console.WriteLine($"new subsequence: {string.Join(" - ", newSubSequence.Select(c => c.Name))}");
                var newCustomerOrder = customerSequence.Take(i).Concat(newSubSequence.Select(nc => customerSequence.Find(c => c.Name == nc.Name))).Concat(customerSequence.Skip(i + subSequenceLength));
                Console.WriteLine($"new sequence: {string.Join(" - ", newCustomerOrder.Select(c => c.Name))}");

                var newSolution = Solution.FromCustomerOrder(instance, newCustomerOrder);
                Console.WriteLine($"{newSolution.Values} [{newSolution.Values.Values[^1] - input.Values.Values[^1]}] i={i}");
                return newSolution;
            }

            foreach (var module in customerSequence[i].Modules)
            {
                coveredModules.Add(module);
            }
        }


        return input;
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