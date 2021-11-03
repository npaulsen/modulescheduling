namespace Scheduling;

public class ReversablePreprocessor {

    private readonly Instance _instance;
    private Instance? _preprocessedInstance;

    private readonly Dictionary<string,Customer> _customerMapping;
    private readonly Dictionary<string,Module> _moduleMapping;

    public Instance PreprocessedInstance => _preprocessedInstance ?? Preprocess();

    public ReversablePreprocessor(Instance instance)
    {
        _instance = instance;
        _customerMapping = new ();
        _moduleMapping = new ();
    }

    public IEnumerable<Customer> GetOriginalCustomers(IEnumerable<Customer> customers) =>
        customers.SelectMany(c => c.Name.Split(",").Select(name => _customerMapping[name]));

    private Instance Preprocess() {
        var normalizedInstance = MapNames();
        _preprocessedInstance = normalizedInstance.WithAggregatedCustomers().WithAggregatedModules();
        return _preprocessedInstance;
    }

    private Instance MapNames() {
        var newModules = new List<Module>();
        var newCustomers = new List<Customer>();
        var mappedModules = new Dictionary<Module,Module>();
        var moduleIndex = 0;
        foreach (var (customer,i) in _instance.Customers.Select((c,i) => (c,i))) {
            var newName = $"C{i}";
            _customerMapping[newName] = customer;
            var newCustomerModules = new List<Module>();
            foreach (var module in customer.Modules)
            {
                if (!mappedModules.ContainsKey(module)) {
                    var newModuleName = $"M{moduleIndex}";
                    _moduleMapping[newModuleName] = module;
                    var newModule = new Module(newModuleName,module.Effort);
                    mappedModules[module] = newModule;
                    newModules.Add(newModule);
                    moduleIndex++;
                } 
                newCustomerModules.Add(mappedModules[module]);
            }
            newCustomers.Add(new(newName,newCustomerModules, customer.Weight));
        }
        var newObjectives = _instance.Objectives.Select(obj => obj is TimeValueObjective ? new TimeValueObjective(newCustomers,newModules) : obj);
        return new Instance(newModules,newCustomers,newObjectives);
    }
}