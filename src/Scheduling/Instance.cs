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

public record Customer(string Name, IEnumerable<Module> Modules);

public record Module(string Name);

public record Objective(int Deadline);