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
├── EmployeeManagement.sln                     # Main Visual Studio Solution
│
└── src/
    ├── EmployeeManagement.Domain/             # Day 2 Domain Model Class Library
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
    │   ├── Contracts/
    │   │   ├── IRepository.cs                 # Generic Repository Contract
    │   │   ├── IEmployeeRepository.cs         # Specialized Employee Repository
    │   │   └── IPayrollCalculator.cs          # Payroll Abstraction Contract
    │   ├── Services/
    │   │   └── PayrollCalculator.cs           # SRP Payroll & Tax Calculator
    │   ├── Repositories/
    │   │   └── InMemoryEmployeeRepository.cs  # Decoupled In-Memory Storage
    │   └── LeetCode/
    │       └── NumberOfStepsToReduceZero.cs   # LeetCode 1342 Solution & Tests
    │
    └── EmployeeManagement.ConsoleApp/         # Console Application & Demos
        ├── Demos/
        │   ├── TypeSemanticsDemo.cs           # Day 1 Value vs Reference
        │   └── OopAndSolidDemo.cs             # Day 2 Polymorphism, Records, Interfaces
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
| **Day 3** | **Modern C# & LINQ** | 8+ LINQ operations, Grouping/Filtering, Unit-testable methods | Next Up ⏳ |
| **Day 4** | **Unit Testing & Async C#** | xUnit suite (5+ tests, AAA pattern), Async method with cancellation | Planned ⏳ |
| **Day 5** | **Mini-Build Checkpoint (Gate 1)** | Complete tested mini-app, injected bug regression test | Planned ⏳ |

---

## How to Run Console App

### 1. Interactive Menu
```powershell
dotnet run --project "src/EmployeeManagement.ConsoleApp"
```

### 2. Automated Demonstration Mode (All Demos & LeetCode Tests)
```powershell
dotnet run --project "src/EmployeeManagement.ConsoleApp" -- --demo
```
