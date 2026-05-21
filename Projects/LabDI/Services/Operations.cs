namespace LabDI.Services;

public interface IOperation
{
    string OperationId { get; }
}

public interface IOperationTransient : IOperation { }

public interface IOperationScoped : IOperation { }

public interface IOperationSingleton : IOperation { }

public class OperationService :
    IOperationScoped, IOperationSingleton, IOperationTransient
{
    public string OperationId { get; } = Guid.NewGuid().ToString()[^4..];
}

