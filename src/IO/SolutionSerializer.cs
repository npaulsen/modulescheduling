namespace Scheduling.IO;

public class SolutionSerializer
{
    public static void Serialize(Solution s, string path)
    {
        var moduleOrder = s.GenerateReport().Entries.SelectMany(e => e.CoveredCustomers).SelectMany(c => c.Modules).Distinct();
        File.WriteAllLines(path, moduleOrder.Select(m => m.Name));
    }

    public static Solution Deserialize(Instance instance, string path)
    {
        var moduleOrder = File.ReadAllLines(path).Select(line => {
            var m = instance.Modules.FirstOrDefault(m => string.Equals(m.Name,line,StringComparison.InvariantCultureIgnoreCase));
            if (m is null) {
                throw new Exception(line);
            }
            return m;
        });
        return new Solution(instance,new(moduleOrder));
    }
}