# .NET Engineering Training: Capstone Project

**Capstone Domain:** Employee & Department Management System  
**Framework:** .NET 8.0 (C# 12)  
**Author:** Shanmukha-Sai (<kalugurisai2002@gmail.com>)

---

## Repository Structure

```
D:\sai\dotnet\
│
├── .gitignore
├── PLAN.md                                    # 3-Week Curriculum & Roadmap
├── README.md                                  # Repository Overview
├── DAY01_README.md                            # Day 1 Deliverables (Syntax & Types)
├── DAY02_README.md                            # Day 2 Deliverables (OOP & SOLID)
├── DAY03_README.md                            # Day 3 Deliverables (Modern C# & LINQ)
├── EmployeeManagement.sln                     # Main Visual Studio Solution
│
└── src/
    ├── EmployeeManagement.Domain/             # Core Domain Model & Business Logic
    │   ├── Common/
    │   │   └── BaseEntity.cs                  # Abstract Base Entity
    │   ├── ValueObjects/
    │   │   └── Address.cs                     # C# Record (Value Equality)
    │   ├── Enums/
    │   │   └── EmployeeType.cs                # Employee & Department Enums
    │   ├── Entities/
    │   │   ├── Employee.cs                    # Abstract Base Employee
    │   │   ├── FullTimeEmployee.cs            # Polymorphic Full-Time Entity
    │   │   ├── ContractEmployee.cs            # Polymorphic Contract Entity
    │   │   └── Department.cs                  # Encapsulation & Aggregation
    │   ├── DTOs/
    │   │   ├── EmployeeSummaryDto.cs          # Query Projection DTO
    │   │   └── DepartmentPayrollReportDto.cs  # Aggregation GroupBy Report DTO
    │   ├── Events/
    │   │   └── EmployeeEvents.cs              # Custom Delegates & EventHandler<T>
    │   ├── Extensions/
    │   │   └── CollectionExtensions.cs        # Deferred Execution (yield return)
    │   ├── Services/
    │   │   ├── LinqQueryService.cs            # 10 Testable LINQ Queries & Pattern Matching
    │   │   └── PayrollCalculator.cs           # SRP Payroll & Tax Calculator
    │   ├── Contracts/
    │   │   ├── IRepository.cs                 # Generic Repository Contract
    │   │   ├── IEmployeeRepository.cs         # Specialized Employee Repository
    │   │   └── IPayrollCalculator.cs          # Payroll Abstraction Contract
    │   ├── Repositories/
    │   │   └── InMemoryEmployeeRepository.cs  # Decoupled In-Memory Storage
    │   └── LeetCode/
    │       ├── NumberOfStepsToReduceZero.cs   # Day 2 LeetCode 1342
    │       └── ContainsDuplicate.cs           # Day 3 LeetCode 217
    │
    └── EmployeeManagement.ConsoleApp/         # Console Application & Demos
        ├── Demos/
        │   ├── TypeSemanticsDemo.cs           # Day 1 Value vs Reference
        │   ├── OopAndSolidDemo.cs             # Day 2 Polymorphism & Records
        │   └── LinqAndModernCSharpDemo.cs     # Day 3 LINQ, Lazy Eval & Events
        ├── LeetCode/
        │   └── ConcatenationOfArray.cs        # Day 1 LeetCode 1929
        └── Program.cs                         # Interactive CLI & Demo runner
```

---

## Daily Modules & Progress

| Day | Module | Deliverables | Status |
| :--- | :--- | :--- | :--- |
| **Day 1** | **C# Fundamentals & Syntax** | Console App, Value/Ref Types, StringBuilder, Custom Exceptions, LeetCode 1929 | **Completed** ✅ |
| **Day 2** | **Object-Oriented C# & SOLID** | Domain Class Library, Abstract Classes, Polymorphism, Records, Interfaces, LeetCode 1342 | **Completed** ✅ |
| **Day 3** | **Modern C# & LINQ** | 10 LINQ Operations, GroupBy DTOs, Deferred Execution (`yield return`), Delegates/Events, LeetCode 217 | **Completed** ✅ |
| **Day 4** | **Unit Testing & Async C#** | xUnit suite (5+ tests, AAA pattern), Async method with cancellation | Next Up ⏳ |
| **Day 5** | **Mini-Build Checkpoint (Gate 1)** | Complete tested mini-app, injected bug regression test | Planned ⏳ |

---

## How to Run

### Automated Demonstration Mode
```powershell
dotnet run --project "src/EmployeeManagement.ConsoleApp" -- --demo
```

### Interactive Menu
```powershell
dotnet run --project "src/EmployeeManagement.ConsoleApp"
```
