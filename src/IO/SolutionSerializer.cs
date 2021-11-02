namespace Scheduling.IO;

public class SolutionSerializer
{
    public static void Serialize(Solution s, string path)
    {
        var customerOrder = s.GenerateReport().Entries.SelectMany(e => e.CoveredCustomers);
        File.WriteAllLines(path, customerOrder.Select(c => c.Name));
    }

    public static Solution Deserialize(Instance instance, string path)
    {
        var customerOrder = File.ReadAllLines(path).Select(line => instance.Customers.First(c => c.Name == line));
        return Solution.FromCustomerOrder(instance, customerOrder);
    }
}