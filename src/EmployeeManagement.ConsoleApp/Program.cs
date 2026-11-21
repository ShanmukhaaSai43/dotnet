using EmployeeManagement.ConsoleApp.Demos;
using EmployeeManagement.ConsoleApp.Exceptions;
using EmployeeManagement.ConsoleApp.LeetCode;
using EmployeeManagement.ConsoleApp.Models;
using EmployeeManagement.ConsoleApp.Services;
using EmployeeManagement.Domain.LeetCode;

namespace EmployeeManagement.ConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("====================================================================");
        Console.WriteLine("   .NET Core Engineering: Day 4 - Unit Testing & Async C#            ");
        Console.WriteLine("   Domain: Employee & Department Management System                  ");
        Console.WriteLine("====================================================================\n");

        var employeeService = new EmployeeService();

        // If run with '--demo' argument or non-interactively, run automated verification
        if (args.Length > 0 && args[0].Equals("--demo", StringComparison.OrdinalIgnoreCase) || Console.IsInputRedirected)
        {
            RunAutomatedSampleScenarios(employeeService);
            TypeSemanticsDemo.RunAllDemos();
            OopAndSolidDemo.RunAllDemos();
            LinqAndModernCSharpDemo.RunAllDemos();
            await AsyncAndToolingDemo.RunAllDemosAsync();
            ConcatenationOfArraySolution.RunTests();
            NumberOfStepsToReduceZeroSolution.RunTests();
            ContainsDuplicateSolution.RunTests();
            BestTimeToBuyAndSellStockSolution.RunTests();
            return;
        }

        // Interactive Menu loop
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. View All Employee Records (Formatted Report via StringBuilder)");
            Console.WriteLine("2. Add New Employee (Validation & Nullable demo)");
            Console.WriteLine("3. Find Employee By ID ('out' parameter demo)");
            Console.WriteLine("4. Apply Annual Raise ('ref' parameter demo)");
            Console.WriteLine("5. Run Day 1 Type Semantics Demos (Value vs Ref)");
            Console.WriteLine("6. Run Day 2 OOP & SOLID Demos (Polymorphism, Records, Interfaces)");
            Console.WriteLine("7. Run Day 3 Modern C# & LINQ Demos (10 Queries, Deferred Execution)");
            Console.WriteLine("8. Run Day 4 Async & Cancellation Demos (Task, CancellationToken, WhenAll)");
            Console.WriteLine("9. Run Day 1 LeetCode: 1929. Concatenation of Array");
            Console.WriteLine("10. Run Day 2 LeetCode: 1342. Number of Steps to Reduce to Zero");
            Console.WriteLine("11. Run Day 3 LeetCode: 217. Contains Duplicate");
            Console.WriteLine("12. Run Day 4 LeetCode: 121. Best Time to Buy and Sell Stock");
            Console.WriteLine("13. Run Full Automated Verification (--demo)");
            Console.WriteLine("0. Exit");
            Console.Write("\nEnter choice (0-13): ");

            string? choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine(employeeService.GenerateFormattedReport());
                    break;
                case "2":
                    HandleAddEmployeeInteractive(employeeService);
                    break;
                case "3":
                    HandleFindEmployeeInteractive(employeeService);
                    break;
                case "4":
                    HandleApplyRaiseInteractive(employeeService);
                    break;
                case "5":
                    TypeSemanticsDemo.RunAllDemos();
                    break;
                case "6":
                    OopAndSolidDemo.RunAllDemos();
                    break;
                case "7":
                    LinqAndModernCSharpDemo.RunAllDemos();
                    break;
                case "8":
                    await AsyncAndToolingDemo.RunAllDemosAsync();
                    break;
                case "9":
                    ConcatenationOfArraySolution.RunTests();
                    break;
                case "10":
                    NumberOfStepsToReduceZeroSolution.RunTests();
                    break;
                case "11":
                    ContainsDuplicateSolution.RunTests();
                    break;
                case "12":
                    BestTimeToBuyAndSellStockSolution.RunTests();
                    break;
                case "13":
                    RunAutomatedSampleScenarios(employeeService);
                    TypeSemanticsDemo.RunAllDemos();
                    OopAndSolidDemo.RunAllDemos();
                    LinqAndModernCSharpDemo.RunAllDemos();
                    await AsyncAndToolingDemo.RunAllDemosAsync();
                    ConcatenationOfArraySolution.RunTests();
                    NumberOfStepsToReduceZeroSolution.RunTests();
                    ContainsDuplicateSolution.RunTests();
                    BestTimeToBuyAndSellStockSolution.RunTests();
                    break;
                case "0":
                    running = false;
                    Console.WriteLine("Exiting. Have a great day!");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please enter a number between 0 and 13.");
                    break;
            }
        }
    }

    public static void RunAutomatedSampleScenarios(EmployeeService service)
    {
        Console.WriteLine("====================================================================");
        Console.WriteLine("              3 SAMPLE INPUT AND OUTPUT DEMONSTRATIONS              ");
        Console.WriteLine("====================================================================\n");

        Console.WriteLine(">>> SAMPLE 1: Adding a Valid Employee Record (Nullable Bonus & Phone included)");
        try
        {
            var emp1 = service.AddEmployee(
                name: "David Miller",
                email: "david.miller@company.com",
                department: Department.Operations,
                salary: 68000m,
                bonus: 4500m,
                phone: "555-0144",
                address: new EmployeeAddress("456 Industrial Way", "Chicago", "IL", "60601"),
                skills: new[] { "Logistics", "Process Optimization", "ERP" }
            );
            Console.WriteLine($"[OUTPUT]: Employee successfully created!");
            Console.WriteLine($"  {emp1}");
            Console.WriteLine($"  Total Compensation: {emp1.TotalCompensation:C}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR]: {ex.Message}\n");
        }

        Console.WriteLine(">>> SAMPLE 2: Adding Employee with Omitted Nullable Values (Bonus=null, Phone=null)");
        try
        {
            var emp2 = service.AddEmployee(
                name: "Elena Rostova",
                email: "elena.r@company.com",
                department: Department.Marketing,
                salary: 58000m
            );
            Console.WriteLine($"[OUTPUT]: Employee successfully created with defaults!");
            Console.WriteLine($"  {emp2}");
            Console.WriteLine($"  Bonus HasValue: {emp2.Bonus.HasValue} (Demonstrating Nullable<decimal>)");
            Console.WriteLine($"  Phone is null: {emp2.PhoneNumber == null}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR]: {ex.Message}\n");
        }

        Console.WriteLine(">>> SAMPLE 3: Exception Handling on Invalid Input (Invalid email & negative salary)");
        try
        {
            Console.WriteLine("Attempting to insert employee with invalid email 'bad-email' and salary 15000...");
            service.AddEmployee(
                name: "Test Bad User",
                email: "bad-email-no-at",
                department: Department.Engineering,
                salary: 15000m
            );
        }
        catch (EmployeeValidationException ex)
        {
            Console.WriteLine($"[OUTPUT - Handled Gracefully]: Caught EmployeeValidationException!");
            Console.WriteLine($"  Field: {ex.FieldName}");
            Console.WriteLine($"  Message: {ex.Message}\n");
        }
    }

    private static void HandleAddEmployeeInteractive(EmployeeService service)
    {
        try
        {
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter Email: ");
            string email = Console.ReadLine() ?? "";

            Console.WriteLine("Select Department: 1. Engineering, 2. HR, 3. Finance, 4. Marketing, 5. Operations");
            Console.Write("Department (1-5): ");
            int deptNum = int.TryParse(Console.ReadLine(), out int d) ? d : 1;
            var department = (Department)Math.Clamp(deptNum, 1, 5);

            Console.Write("Enter Salary (e.g. 65000): ");
            decimal salary = decimal.TryParse(Console.ReadLine(), out decimal s) ? s : 30000m;

            Console.Write("Enter Bonus (optional, press Enter to skip): ");
            string? bonusInput = Console.ReadLine();
            decimal? bonus = string.IsNullOrWhiteSpace(bonusInput) ? null : decimal.Parse(bonusInput);

            Console.Write("Enter City: ");
            string city = Console.ReadLine() ?? "Unknown";

            var address = new EmployeeAddress("", city, "", "");
            var created = service.AddEmployee(name, email, department, salary, bonus, null, address);

            Console.WriteLine($"\n[SUCCESS] Employee added successfully!");
            Console.WriteLine(created);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERROR] Failed to add employee: {ex.Message}");
        }
    }

    private static void HandleFindEmployeeInteractive(EmployeeService service)
    {
        Console.Write("Enter Employee ID to search: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (service.TryGetEmployeeById(id, out EmployeeManagement.ConsoleApp.Models.Employee? employee) && employee != null)
            {
                Console.WriteLine($"\nFound: {employee}");
            }
            else
            {
                Console.WriteLine($"\nEmployee with ID '{id}' was not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }

    private static void HandleApplyRaiseInteractive(EmployeeService service)
    {
        Console.Write("Enter Employee ID to adjust salary: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (service.TryGetEmployeeById(id, out EmployeeManagement.ConsoleApp.Models.Employee? emp) && emp != null)
            {
                Console.Write($"Current Salary is {emp.Salary:C}. Enter % raise (e.g. 10 for 10%): ");
                if (decimal.TryParse(Console.ReadLine(), out decimal pct))
                {
                    decimal currentSalary = emp.Salary;
                    service.ApplyAnnualRaise(ref currentSalary, pct);
                    emp.Salary = currentSalary;
                    Console.WriteLine($"New Salary is: {emp.Salary:C}");
                }
            }
            else
            {
                Console.WriteLine("Employee not found.");
            }
        }
    }
}
