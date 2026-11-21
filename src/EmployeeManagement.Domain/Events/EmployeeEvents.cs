namespace EmployeeManagement.Domain.Events;

public class EmployeeHiredEventArgs : EventArgs
{
    public int EmployeeId { get; }
    public string EmployeeName { get; }
    public string DepartmentName { get; }
    public DateTime HiredAt { get; }

    public EmployeeHiredEventArgs(int id, string name, string deptName)
    {
        EmployeeId = id;
        EmployeeName = name;
        DepartmentName = deptName;
        HiredAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Demonstrates custom delegates and events in C#.
/// </summary>
public class EmployeeNotifier
{
    // Custom Delegate definition
    public delegate void EmployeeNotificationHandler(string message, ConsoleColor color);

    // Event using built-in EventHandler<T>
    public event EventHandler<EmployeeHiredEventArgs>? EmployeeHired;

    public void OnEmployeeHired(int id, string name, string deptName)
    {
        EmployeeHired?.Invoke(this, new EmployeeHiredEventArgs(id, name, deptName));
    }
}
