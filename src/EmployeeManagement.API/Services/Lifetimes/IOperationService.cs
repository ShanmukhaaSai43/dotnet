namespace EmployeeManagement.API.Services.Lifetimes;

public interface IOperation
{
    Guid OperationId { get; }
    DateTime CreatedAt { get; }
}

public interface IOperationTransient : IOperation { }
public interface IOperationScoped : IOperation { }
public interface IOperationSingleton : IOperation { }

/// <summary>
/// Demonstrates Service Lifetimes in ASP.NET Core.
/// Generates a unique GUID upon instantiation.
/// </summary>
public class OperationService : IOperationTransient, IOperationScoped, IOperationSingleton
{
    public Guid OperationId { get; }
    public DateTime CreatedAt { get; }

    public OperationService()
    {
        OperationId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
