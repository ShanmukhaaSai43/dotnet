using System.Diagnostics;
using System.Text;
using EmployeeManagement.ConsoleApp.Models;

namespace EmployeeManagement.ConsoleApp.Demos;

/// <summary>
/// Interactive and automated demonstration of C# Type Semantics:
/// 1. Value Types (Structs, Primitives) vs Reference Types (Classes)
/// 2. Nullable Value Types vs Nullable Reference Types
/// 3. Passing parameters: by value vs by ref vs out
/// 4. String immutability vs StringBuilder performance
/// </summary>
public static class TypeSemanticsDemo
{
    public static void RunAllDemos()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    DAY 1 DEMO: C# TYPE SEMANTICS & INTERVIEW TOPICS   ");
        Console.WriteLine("=======================================================\n");

        DemoValueVsReference();
        DemoPassingByRefAndOut();
        DemoNullableTypes();
        DemoStringVsStringBuilder();
    }

    private static void DemoValueVsReference()
    {
        Console.WriteLine("--- 1. Value Types (Struct) vs Reference Types (Class) ---");

        // A. Value Type (Struct) copy semantics
        var originalAddress = new EmployeeAddress("100 Main St", "Dallas", "TX", "75001");
        var copiedAddress = originalAddress; // Bitwise copy on the stack!
        copiedAddress.City = "Houston"; // Modifying the copy does NOT affect the original

        Console.WriteLine($"[Struct - Value Type]");
        Console.WriteLine($"Original Address City: {originalAddress.City} (Expected: Dallas)");
        Console.WriteLine($"Copied Address City:   {copiedAddress.City} (Expected: Houston)");
        Console.WriteLine($"Are they independent?  {originalAddress.City != copiedAddress.City}");

        // B. Reference Type (Class) reference semantics
        var originalEmployee = new Employee { Id = 1, Name = "Alice", Salary = 60000m };
        var employeeReference = originalEmployee; // Copies reference (pointer), points to same heap object!
        employeeReference.Salary = 75000m; // Modifies the object on the heap!

        Console.WriteLine($"\n[Class - Reference Type]");
        Console.WriteLine($"Original Employee Salary:  {originalEmployee.Salary:C} (Expected: $75,000.00)");
        Console.WriteLine($"Reference Employee Salary: {employeeReference.Salary:C} (Expected: $75,000.00)");
        Console.WriteLine($"Did both reflect change?   {ReferenceEquals(originalEmployee, employeeReference)} (Same Heap Reference)");
        Console.WriteLine();
    }

    private static void DemoPassingByRefAndOut()
    {
        Console.WriteLine("--- 2. Parameter Passing: By Value vs 'ref' vs 'out' ---");

        // Pass by value (default for value types)
        decimal salary = 50000m;
        TryModifyByValue(salary);
        Console.WriteLine($"Salary after TryModifyByValue(salary): {salary:C} (Unchanged, passed copy)");

        // Pass by ref
        ModifyByRef(ref salary);
        Console.WriteLine($"Salary after ModifyByRef(ref salary):   {salary:C} (Modified in-place via reference)");

        // Out parameter
        if (TryCalculateTax(salary, out decimal taxAmount))
        {
            Console.WriteLine($"Tax calculated via 'out' parameter:    {taxAmount:C}");
        }
        Console.WriteLine();
    }

    private static void TryModifyByValue(decimal val) => val += 10000m;
    private static void ModifyByRef(ref decimal val) => val += 10000m;
    private static bool TryCalculateTax(decimal income, out decimal tax)
    {
        tax = income * 0.20m; // out parameter MUST be assigned before method returns
        return true;
    }

    private static void DemoNullableTypes()
    {
        Console.WriteLine("--- 3. Nullable Value Types (int?, decimal?) & Nullable Reference Types ---");

        // Nullable Value Type: Nullable<T> is a struct under the hood
        int? nullableDepartmentId = null;
        Console.WriteLine($"int? HasValue: {nullableDepartmentId.HasValue}");
        Console.WriteLine($"int? with null-coalescing (??): {nullableDepartmentId ?? -1} (Fallback to -1)");

        nullableDepartmentId = 5;
        Console.WriteLine($"int? after assigning 5: {nullableDepartmentId.Value}");

        // Nullable Reference Type (C# 8+ compiler safety)
        string? optionalNotes = null;
        Console.WriteLine($"string? Length using null-conditional (?.): {optionalNotes?.Length ?? 0}");
        Console.WriteLine();
    }

    private static void DemoStringVsStringBuilder()
    {
        Console.WriteLine("--- 4. String (Immutable) vs StringBuilder (Mutable) ---");
        const int iterations = 15000;

        // String concatenation (creates O(N^2) allocations on heap)
        var sw1 = Stopwatch.StartNew();
        string s = string.Empty;
        for (int i = 0; i < iterations; i++)
        {
            s += "x";
        }
        sw1.Stop();

        // StringBuilder append (reuses internal buffer on heap)
        var sw2 = Stopwatch.StartNew();
        var sb = new StringBuilder(iterations);
        for (int i = 0; i < iterations; i++)
        {
            sb.Append("x");
        }
        string result = sb.ToString();
        sw2.Stop();

        Console.WriteLine($"Concatenating {iterations:N0} strings via '+=' took: {sw1.ElapsedMilliseconds} ms (High memory allocations)");
        Console.WriteLine($"Appending {iterations:N0} strings via StringBuilder took: {sw2.ElapsedMilliseconds} ms (Single mutable buffer)");
        Console.WriteLine($"Speedup: {(sw1.ElapsedMilliseconds > 0 ? (double)sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1) : 1):F1}x faster!");
        Console.WriteLine();
    }
}
