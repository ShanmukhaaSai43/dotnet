namespace EmployeeManagement.Domain.Common;

/// <summary>
/// Abstract Base Class demonstrating Inheritance and Common Entity Attributes.
/// All domain entities inherit from BaseEntity.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
    }

    protected BaseEntity(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Entity ID must be greater than zero.");

        Id = id;
    }

    public void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
