using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Domain.Services;

namespace EmployeeManagement.ConsoleApp.Demos;

/// <summary>
/// Day 4 Demonstrations:
/// 1. Asynchronous non-blocking data fetching (async / await)
/// 2. Cooperative Cancellation via CancellationToken
/// 3. Concurrent processing using Task.WhenAll
/// </summary>
public static class AsyncAndToolingDemo
{
    public static async Task RunAllDemosAsync()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    DAY 4 DEMO: ASYNC PROGRAMMING & CANCELLATION       ");
        Console.WriteLine("=======================================================\n");

        var repo = new InMemoryEmployeeRepository();
        repo.Add(new FullTimeEmployee(1, "Ada Lovelace", "ada@computing.org", 1, 140000m));
        repo.Add(new FullTimeEmployee(2, "Charles Babbage", "charles@analytical.org", 1, 120000m));
        repo.Add(new ContractEmployee(3, "Nikola Tesla", "nikola@wardenclyffe.org", 2, 95m, 160, DateTime.UtcNow.AddMonths(6)));

        var depts = new List<Department>
        {
            new Department(1, "Computing Core", "CC", 35000m),
            new Department(2, "Advanced Research", "AR", 25000m)
        };

        var asyncService = new AsyncEmployeeService(repo, depts);

        await DemoAsyncFetchAsync(asyncService);
        await DemoCancellationTokenAsync(asyncService);
        await DemoConcurrentTaskWhenAllAsync(asyncService);
    }

    private static async Task DemoAsyncFetchAsync(AsyncEmployeeService service)
    {
        Console.WriteLine("--- 1. Non-Blocking Async Fetch (async / await) ---");
        Console.WriteLine("Starting asynchronous employee fetch...");

        var employees = await service.FetchAllEmployeesAsync();
        Console.WriteLine($"[ASYNC FETCH COMPLETED] Successfully fetched {employees.Count} employees without blocking the thread!");

        foreach (var emp in employees)
        {
            Console.WriteLine($"  - [ID: {emp.Id}] {emp.Name} ({emp.Type}) - Pay: {emp.CalculateMonthlyPay():C2}");
        }
        Console.WriteLine();
    }

    private static async Task DemoCancellationTokenAsync(AsyncEmployeeService service)
    {
        Console.WriteLine("--- 2. Cooperative Cancellation via CancellationToken ---");
        
        using var cts = new CancellationTokenSource();
        // Trigger cancellation immediately to demonstrate cooperative abort
        cts.Cancel();
        Console.WriteLine("CancellationToken cancelled BEFORE async operation begins.");

        try
        {
            Console.WriteLine("Attempting to call FetchAllEmployeesAsync(cts.Token)...");
            await service.FetchAllEmployeesAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[CANCELLATION CAUGHT] Operation was successfully cancelled as requested via CancellationToken!");
            Console.ResetColor();
        }
        Console.WriteLine();
    }

    private static async Task DemoConcurrentTaskWhenAllAsync(AsyncEmployeeService service)
    {
        Console.WriteLine("--- 3. Concurrent Fan-Out using Task.WhenAll ---");
        var deptIds = new[] { 1, 2 };

        Console.WriteLine("Fetching department headcounts concurrently across departments [1, 2]...");
        var headcounts = await service.FetchDepartmentHeadcountsConcurrentlyAsync(deptIds);

        foreach (var (dept, count) in headcounts)
        {
            Console.WriteLine($"  - {dept}: {count} active staff");
        }
        Console.WriteLine();
    }
}
