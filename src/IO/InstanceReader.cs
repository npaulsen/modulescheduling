using Scheduling;

public class InstanceReader
{
    public static Instance FromFile(string path)
        => FromLines(File.ReadAllLines(path));

    public static Instance FromLines(string[] lines)
    {
        var numObjectives = int.Parse(lines[0]);
        var objectives = new List<Objective>();
        foreach (var line in lines.Skip(1).Take(numObjectives))
        {
            var parts = line.Split();
            var type = parts[0];
            Objective objective = type switch
            {
                "Deadline" => new DeadlineObjective(int.Parse(parts[1])),
                "Timevalue" => new TimeValueObjective(),
                _ => throw new Exception("Unrecognized objective type"),
            };
            objectives.Add(objective);
        }

        var customerLookup = new Dictionary<string, List<string>>();
        foreach (var line in lines.Skip(1 + numObjectives))
        {
            var customerId = line.Split()[0];
            var moduleId = line.Split()[1];
            if (moduleId == string.Empty)
            {
                throw new Exception($"instance format mismatch in line:'{line}'");
            }
            if (!customerLookup.ContainsKey(customerId))
            {
                customerLookup[customerId] = new();
            }
            customerLookup[customerId].Add(moduleId);
        }
        var customers = customerLookup
            .Select(kvp => new Customer(kvp.Key, kvp.Value.Distinct().Select(moduleId => new Module(moduleId))));
        var modules = customerLookup.Values.SelectMany(l => l).Distinct().Select(moduleId => new Module(moduleId));

        return new Instance(modules, customers, objectives);
    }
}