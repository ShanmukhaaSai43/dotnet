# .NET Engineering Training: Capstone Project

**Capstone Domain:** Employee & Department Management System  
**Framework:** .NET 8.0 (C# 12 / ASP.NET Core)  
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
├── DAY05_GATE1_README.md                      # Day 5: Gate 1 Assessment (Week 1 Passed)
├── DAY06_README.md                            # Day 6: ASP.NET Core Web API & REST CRUD
├── DAY07_README.md                            # Day 7: DI Lifetimes & Custom Middleware
├── DAY08_README.md                            # Day 8: Layered Architecture & Validation
├── EmployeeManagement.sln                     # Main Visual Studio Solution
│
├── src/
│   ├── EmployeeManagement.API/                # Presentation Layer (Thin Controllers & Middleware)
│   │   ├── Controllers/
│   │   │   ├── EmployeesController.cs         # Thin Controller: Delegates to IEmployeeService
│   │   │   ├── DepartmentsController.cs       # Thin Controller: Delegates to IDepartmentService
│   │   │   └── DiagnosticsController.cs       # DI Lifetimes & Configuration diagnostics
│   │   ├── Middleware/
│   │   │   ├── RequestLoggingMiddleware.cs    # X-Correlation-Id & X-Response-Time-Ms
│   │   │   └── GlobalExceptionHandlingMiddleware.cs # RFC 7807 ProblemDetails Exception Handler
│   │   ├── Services/
│   │   │   └── Lifetimes/
│   │   │       └── IOperationService.cs       # Transient, Scoped, Singleton contracts
│   │   ├── Configuration/
│   │   │   └── PayrollSettings.cs             # IOptions<PayrollSettings> POCO
│   │   ├── DTOs/
│   │   │   ├── EmployeeDtos.cs                # Re-exported Application DTOs
│   │   │   └── DepartmentDtos.cs              # Re-exported Application DTOs
│   │   ├── EmployeeManagement.API.http        # Runnable HTTP requests collection
│   │   └── Program.cs                         # Middleware Pipeline & DI Registration
│   │
│   ├── EmployeeManagement.Application/        # Application Layer (Use Cases & Validation)
│   │   ├── Common/Exceptions/                 # AppException, ValidationException, NotFoundException, etc.
│   │   ├── DTOs/                              # Validated Request/Response DTOs
│   │   ├── Interfaces/                        # IEmployeeService, IDepartmentService
│   │   ├── Services/                          # EmployeeService, DepartmentService
│   │   ├── Validators/                        # FluentValidation Rules (Create, Update, Patch)
│   │   └── DependencyInjection.cs             # services.AddApplication() Extension
│   │
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
    └── EmployeeManagement.UnitTests/          # xUnit Test Suite (94 Passing Tests)
        ├── ApiTests/
        │   └── EmployeesControllerTests.cs    # Thin Controller async delegation & status tests
        ├── ApplicationTests/
        │   ├── CreateEmployeeRequestValidatorTests.cs # FluentValidation rules & boundary tests
        │   ├── EmployeeServiceTests.cs        # Application Service CRUD, uniqueness & exception tests
        │   └── GlobalExceptionHandlingMiddlewareTests.cs # RFC 7807 ProblemDetails mapping tests
        ├── DiAndMiddlewareTests/
        │   ├── ServiceLifetimeTests.cs        # Transient, Scoped, Singleton tests
        │   └── RequestLoggingMiddlewareTests.cs # Correlation ID & pipeline tests
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

## Daily Modules & Progress

| Day | Module | Deliverables | Status |
| :--- | :--- | :--- | :---: |
| **Day 1** | **C# Fundamentals & Syntax** | Console App, Value/Ref Types, StringBuilder, Custom Exceptions, LeetCode 1929 | **Completed** ✅ |
| **Day 2** | **Object-Oriented C# & SOLID** | Domain Class Library, Abstract Classes, Polymorphism, Records, Interfaces, LeetCode 1342 | **Completed** ✅ |
| **Day 3** | **Modern C# & LINQ** | 10 LINQ Operations, GroupBy DTOs, Deferred Execution (`yield return`), Delegates/Events, LeetCode 217 | **Completed** ✅ |
| **Day 4** | **Unit Testing & Async C#** | xUnit suite (27 tests, AAA pattern), Async method with cancellation, LeetCode 121 | **Completed** ✅ |
| **Day 5** | **Mini-Build Checkpoint (Gate 1)** | Fixed defect #BUG-101, Regression Test suite (51 tests), Code Walkthrough Guide, LeetCode 242 | **GATE 1 PASSED** 🏆 |
| **Day 6** | **ASP.NET Core REST API Basics**| Web API project, Employees & Departments CRUD (GET/POST/PUT/PATCH/DELETE), DTOs, Swagger, .http file, 58 tests | **Completed** ✅ |
| **Day 7** | **Dependency Injection & Middleware**| Built-in DI lifetimes (Transient/Scoped/Singleton), Custom logging middleware, IOptions configuration, 64 tests | **Completed** ✅ |
| **Day 8** | **Layered Architecture & Validation**| Clean 3-Tier separation (API/Application/Domain), FluentValidation, Global Exception Handler (RFC 7807), 94 tests | **Completed** ✅ |
| **Day 9** | **EF Core & SQL Integration**| DbContext, Entity configurations, SQL Server migrations, 1-to-many relationships, pagination | Next Up ⏳ |

---

## How to Run

### Run the Web API (with Swagger UI)
```powershell
dotnet run --project "src/EmployeeManagement.API"
```

### Run All 94 Unit Tests
```powershell
dotnet test
```
