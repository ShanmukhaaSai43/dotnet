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
├── DAY01_README.md                            # Day 1 Deliverables & Technical Notes
├── EmployeeManagement.sln                     # Main Visual Studio Solution
│
└── src/
    └── EmployeeManagement.ConsoleApp/         # Day 1 Console Application
        ├── Models/
        │   └── Employee.cs                    # Struct (Value type) & Class (Reference type)
        ├── Services/
        │   └── EmployeeService.cs             # In-memory operations, StringBuilder report
        ├── Exceptions/
        │   └── EmployeeValidationException.cs # Custom domain exceptions
        ├── Demos/
        │   └── TypeSemanticsDemo.cs           # Value vs Ref, in/out/ref, String vs StringBuilder
        ├── LeetCode/
        │   └── ConcatenationOfArray.cs        # LeetCode 1929 Solution & Test Runner
        └── Program.cs                         # Interactive CLI & Automated Demo runner
```

---

## Daily Modules & Progress

| Day | Module | Deliverables | Status |
| :--- | :--- | :--- | :--- |
| **Day 1** | **C# Fundamentals & Syntax** | Console App, Value/Ref Types, StringBuilder, Custom Exceptions, LeetCode 1929 | **Completed** ✅ |
| **Day 2** | **Object-Oriented C# & SOLID** | Domain Library (3+ classes), Interfaces, UML Diagram | Next Up ⏳ |
| **Day 3** | **Modern C# & LINQ** | 8+ LINQ operations, Grouping/Filtering, Unit-testable methods | Planned ⏳ |
| **Day 4** | **Unit Testing & Async C#** | xUnit suite (5+ tests, AAA pattern), Async method with cancellation | Planned ⏳ |
| **Day 5** | **Mini-Build Checkpoint (Gate 1)** | Complete tested mini-app, injected bug regression test | Planned ⏳ |

---

## How to Run Day 1 Console App

### 1. Interactive Menu
```powershell
dotnet run --project "src/EmployeeManagement.ConsoleApp"
```

### 2. Automated Demonstration (3 Sample Scenarios + Type Semantics + LeetCode Tests)
```powershell
dotnet run --project "src/EmployeeManagement.ConsoleApp" -- --demo
```
