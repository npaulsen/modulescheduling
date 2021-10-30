namespace Scheduling;
public class Instance
{
    private readonly List<Module> _modules;
    private readonly List<Customer> _customers;
    private readonly List<Objective> _objectives;

    public IEnumerable<Module> Modules => _modules;
    public IEnumerable<Customer> Customers => _customers;

    public IEnumerable<Objective> Objectives => _objectives;

    public Instance(IEnumerable<Module> modules, IEnumerable<Customer> customers, IEnumerable<Objective> objectives)
    {
        _modules = modules.ToList();
        _customers = customers.ToList();
        _objectives = objectives.ToList();
    }
}

public record Customer(string Name, ImmutableListWithValueSemantics<Module> Modules)
{
    public Customer(string name, IEnumerable<Module> modules) : this(name, modules.ToValueList()) { }
};

public record Module(string Name);

public abstract record Objective()
{
    public abstract int CalculateValue(IEnumerable<Customer> customers, ModuleSchedule schedule);
};

public record DeadlineObjective(int Deadline) : Objective
{
    public override int CalculateValue(IEnumerable<Customer> customers, ModuleSchedule schedule)
    {
        var coveredModules = new HashSet<Module>(schedule.ScheduledModules.Take(Deadline));
        return customers.Count(c => c.Modules.All(coveredModules.Contains));
    }
}

public record TimeValueObjective() : Objective
{
    public override int CalculateValue(IEnumerable<Customer> customers, ModuleSchedule schedule)
    {
        var remainingTime = schedule.ScheduledModules.Count();
        var coveredModules = new HashSet<Module>();
        var value = 0;
        foreach (var module in schedule.ScheduledModules)
        {
            coveredModules.Add(module);
            remainingTime -= 1;
            var coveredCustomers = customers.Count(c => 
                c.Modules.Contains(module) && c.Modules.All(coveredModules.Contains));
            value += coveredCustomers * remainingTime;
        }
        return value;
    }
}