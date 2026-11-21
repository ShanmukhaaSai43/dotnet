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
├── DAY01_README.md                            # Day 1: C# Fundamentals & Syntax
├── DAY02_README.md                            # Day 2: OOP Pillars & SOLID Abstractions
├── DAY03_README.md                            # Day 3: Modern C# & LINQ Transformations
├── DAY04_README.md                            # Day 4: Unit Testing (xUnit) & Async C#
├── DAY05_GATE1_README.md                      # Day 5: Gate 1 Assessment & Checkpoint
├── EmployeeManagement.sln                     # Main Visual Studio Solution
│
├── src/
│   ├── EmployeeManagement.Domain/             # Core Domain Model & Business Logic
│   │   ├── Common/
│   │   │   └── BaseEntity.cs                  # Abstract Base Entity
│   │   ├── ValueObjects/
│   │   │   └── Address.cs                     # C# Record (Value Equality)
│   │   ├── Enums/
│   │   │   └── EmployeeType.cs                # Employee & Department Enums
│   │   ├── Entities/
│   │   │   ├── Employee.cs                    # Abstract Base Employee
│   │   │   ├── FullTimeEmployee.cs            # Polymorphic Full-Time Entity
│   │   │   ├── ContractEmployee.cs            # Polymorphic Contract Entity
│   │   │   └── Department.cs                  # Encapsulation & Aggregation
│   │   ├── DTOs/
│   │   │   ├── EmployeeSummaryDto.cs          # Query Projection DTO
│   │   │   └── DepartmentPayrollReportDto.cs  # Aggregation GroupBy Report DTO
│   │   ├── Events/
│   │   │   └── EmployeeEvents.cs              # Custom Delegates & EventHandler<T>
│   │   ├── Extensions/
│   │   │   └── CollectionExtensions.cs        # Deferred Execution (yield return)
│   │   ├── Services/
│   │   │   ├── CompensationService.cs         # Fixed Defect BUG-101 (Bonus Calculation)
│   │   │   ├── AsyncEmployeeService.cs        # Async TAP & CancellationToken Service
│   │   │   ├── LinqQueryService.cs            # 10 Testable LINQ Queries & Pattern Matching
│   │   │   └── PayrollCalculator.cs           # SRP Payroll & Tax Calculator
│   │   ├── Contracts/
│   │   │   ├── IAsyncEmployeeService.cs       # Async TAP Contract
│   │   │   ├── IRepository.cs                 # Generic Repository Contract
│   │   │   ├── IEmployeeRepository.cs         # Specialized Employee Repository
│   │   │   └── IPayrollCalculator.cs          # Payroll Abstraction Contract
│   │   ├── Repositories/
│   │   │   └── InMemoryEmployeeRepository.cs  # Decoupled In-Memory Storage
│   │   └── LeetCode/
│   │       ├── NumberOfStepsToReduceZero.cs   # Day 2 LeetCode 1342
│   │       ├── ContainsDuplicate.cs           # Day 3 LeetCode 217
│   │       ├── BestTimeToBuyAndSellStock.cs   # Day 4 LeetCode 121
│   │       └── ValidAnagram.cs                # Day 5 LeetCode 242
│   │
│   └── EmployeeManagement.ConsoleApp/         # Console Application & Demos
│       ├── Demos/
│       │   ├── TypeSemanticsDemo.cs           # Day 1 Value vs Reference
│       │   ├── OopAndSolidDemo.cs             # Day 2 Polymorphism & Records
│       │   ├── LinqAndModernCSharpDemo.cs     # Day 3 LINQ, Lazy Eval & Events
│       │   └── AsyncAndToolingDemo.cs         # Day 4 Async & Cancellation Demos
│       ├── LeetCode/
│       │   └── ConcatenationOfArray.cs        # Day 1 LeetCode 1929
│       └── Program.cs                         # Interactive CLI & Demo runner
│
└── tests/
    └── EmployeeManagement.UnitTests/          # xUnit Test Suite (37 Passing Tests)
        ├── DomainTests/
        │   ├── EmployeeTests.cs               # Pay calculation & validation tests
        │   ├── DepartmentTests.cs             # Encapsulation & budget tests
        │   ├── PayrollCalculatorTests.cs      # Tax deduction tests
        │   └── LinqQueryServiceTests.cs       # Queries, grouping & pagination tests
        ├── AsyncTests/
        │   └── AsyncEmployeeServiceTests.cs   # Non-blocking fetch & cancellation tests
        ├── RegressionTests/
        │   └── CompensationDefectRegressionTests.cs # BUG-101 fix & boundary tests
        └── LeetCodeTests/
            ├── BestTimeToBuyAndSellStockTests.cs
            └── ValidAnagramTests.cs
```

---

## Week 1 Progress Summary (Gate 1 Passed)

| Day | Module | Deliverables | Status |
| :--- | :--- | :--- | :---: |
| **Day 1** | **C# Fundamentals & Syntax** | Console App, Value/Ref Types, StringBuilder, Custom Exceptions, LeetCode 1929 | **Completed** ✅ |
| **Day 2** | **Object-Oriented C# & SOLID** | Domain Class Library, Abstract Classes, Polymorphism, Records, Interfaces, LeetCode 1342 | **Completed** ✅ |
| **Day 3** | **Modern C# & LINQ** | 10 LINQ Operations, GroupBy DTOs, Deferred Execution (`yield return`), Delegates/Events, LeetCode 217 | **Completed** ✅ |
| **Day 4** | **Unit Testing & Async C#** | xUnit suite (27 tests, AAA pattern), Async method with cancellation, LeetCode 121 | **Completed** ✅ |
| **Day 5** | **Mini-Build Checkpoint (Gate 1)** | Fixed defect #BUG-101, Regression Test suite (37 total tests), Code Walkthrough Guide, LeetCode 242 | **GATE 1 PASSED** 🏆 |

---

## How to Run

### Run All 37 Unit Tests
```powershell
dotnet test
```

### Run Automated Demos & Verification Mode
```powershell
dotnet run --project "src/EmployeeManagement.ConsoleApp" -- --demo
```
