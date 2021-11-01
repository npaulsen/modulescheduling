using Scheduling;

public class InstanceReader
{
    public static Instance FromFile(string path)
        => FromLines(File.ReadAllLines(path));

    public static Instance FromLines(string[] lines)
    {
        var customerLookup = new Dictionary<string, List<string>>();
        var moduleLookup = new Dictionary<string, Module>();
        var objectiveLines = new List<string[]>();
        foreach (var line in lines)
        {
            var parts = line.Split();
            switch (parts[0])
            {
                case "Objective":
                    objectiveLines.Add(parts);
                    break;
                case "Module":
                    moduleLookup.Add(parts[1], new(parts[1], int.Parse(parts[2])));
                    break;
                case "Customer":
                    var customerId = parts[1];
                    var moduleIds = parts.Skip(2);
                    if (!customerLookup.ContainsKey(customerId))
                    {
                        customerLookup[customerId] = new();
                    }
                    customerLookup[customerId].AddRange(moduleIds);
                    break;
                default:
                    throw new Exception($"Unable to parse isntance line:'{line}'");
            }
        }
        // Add modules without explicit declaration.
        foreach (var moduleId in customerLookup.Values.SelectMany(l => l).Distinct())
        {
            if (!moduleLookup.ContainsKey(moduleId))
            {
                moduleLookup[moduleId] = new(moduleId);
            }
        }
        var customers = customerLookup
            .Select(kvp => new Customer(kvp.Key, kvp.Value.Distinct().Select(moduleId => moduleLookup[moduleId])));
        var modules = customers.SelectMany(c => c.Modules).Distinct();
        var objectives = objectiveLines.Select<string[], Objective>(parts => parts[1] switch
         {
             "Deadline" => new DeadlineObjective(int.Parse(parts[2])),
             "Timevalue" => new TimeValueObjective(customers, modules),
             _ => throw new Exception("Unrecognized objective type"),
         });

        return new Instance(modules, customers, objectives);
    }
}