namespace Scheduling;

public class OptimalSolver
{
    public static Solution ExhaustiveSearch(Instance instance)
    {
        var objective = new TimeValueObjective(instance.Customers, instance.Modules);
        var best = (Schedule: null as ModuleSchedule, Value: -1);
        foreach (var customersPermutation in GetPermutations(instance.Customers))
        {
            var modules = new List<Module>();
            foreach (var customer in customersPermutation)
            {
                modules.AddRange(customer.Modules.Where(m => !modules.Contains(m)));
            }
            var schedule = new ModuleSchedule(modules);
            var value = objective.CalculateValue(instance.Customers, schedule);
            if (value > best.Value)
            {
                best = (schedule, value);
            }
        }
        return new(instance, best.Schedule!);
    }

    static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list)
    {
        return GetPermutations(list, list.Count());

        static IEnumerable<IEnumerable<T>> GetPermutations(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}