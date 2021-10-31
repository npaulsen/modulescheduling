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

public record Customer(string Name, ImmutableListWithValueSemantics<Module> Modules, int Weight = 1)
{
    public Customer(string name, IEnumerable<Module> modules, int weight = 1) : this(name, modules.ToValueList(), weight) { }
};

public record Module(string Name, int Effort = 1);

public abstract record Objective()
{
    public abstract int CalculateValue(IEnumerable<Customer> customers, ModuleSchedule schedule);
};

public record DeadlineObjective(int Deadline) : Objective
{
    public override int CalculateValue(IEnumerable<Customer> customers, ModuleSchedule schedule)
    {
        var coveredModules = new HashSet<Module>();
        var time = 0;
        foreach (var module in schedule.ScheduledModules)
        {
            time += module.Effort;
            if (time > Deadline)
            {
                break;
            }
            coveredModules.Add(module);
        }
        return customers.Where(c => c.Modules.All(coveredModules.Contains)).Sum(c => c.Weight);
    }
}

public record TimeValueObjective : Objective
{
    private readonly Dictionary<Module, List<Customer>> _customersForModule;


    public TimeValueObjective(IEnumerable<Customer> customers, IEnumerable<Module> modules)
    {
        _customersForModule = modules.ToDictionary(m => m, _ => new List<Customer>());
        foreach (var customer in customers)
        {
            foreach (var module in customer.Modules)
            {
                _customersForModule[module].Add(customer);
            }
        }
    }

    public override int CalculateValue(IEnumerable<Customer> customers, ModuleSchedule schedule)
    {
        var remainingTime = schedule.ScheduledModules.Sum(m => m.Effort);
        var remainingModulesFor = customers.ToDictionary(c => c, c => c.Modules.Count);

        var value = 0;
        foreach (var module in schedule.ScheduledModules)
        {
            remainingTime -= module.Effort;
            foreach (var customer in _customersForModule[module])
            {
                remainingModulesFor[customer] -= 1;
                if (remainingModulesFor[customer] == 0)
                {
                    value += customer.Weight * remainingTime;
                }
            }
        }
        return value;
    }
}
