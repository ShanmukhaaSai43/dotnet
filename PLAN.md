# .NET Practical Engineering Program: Roadmap & Plan

## 1. Project Overview & Working Context
- **Workspace Directory**: `D:\sai\dotnet`
- **Capstone Domain**: **Employee & Department Management System**
- **Approach**: Hands-on, practical day-by-day implementation with clean architectural separation, comprehensive unit tests, and Gate assessments.

---

## 2. Environment Status & Missing Prerequisites

| Tool / Component | Current Status | Action Needed | Installation Method |
| :--- | :--- | :--- | :--- |
| **Git** | Installed (`v2.51.2`) | None (Repo initialized) | Already available |
| **VS Code** | Installed | Install C# Extensions | Extensions: `ms-dotnettools.csdevkit`, `humao.rest-client` |
| **.NET SDK** | **MISSING** | **Must Install .NET 8.0 SDK (LTS)** | Download from Microsoft or run `winget install Microsoft.DotNet.SDK.8` |
| **Database Engine** | Missing LocalDB/Express | Install SQL Server Express / LocalDB | Install SQL Server Express or Visual Studio Build Tools with LocalDB |
| **API Client** | Optional | Install Postman or use VS Code `.http` files | Postman app or REST Client extension in VS Code |

---

## 3. Daily Implementation Plan

### Week 1: C# Fundamentals & OOP (Gate 1)
- **Day 1: C# Fundamentals & Syntax**
  - Topics: Value vs Reference Types, Nullable types, Methods, Arrays, Exception handling.
  - Deliverable: Console application for Employee management + README + Value/Reference type explanation.
  - LeetCode: *Concatenation of Array*

- **Day 2: Object-Oriented C# & SOLID**
  - Topics: Encapsulation, Inheritance, Polymorphism, Interfaces vs Abstract classes, SOLID principles.
  - Deliverable: Domain Class Library (Employee, Department, BaseEntity) + Class diagram + SOLID writeup.
  - LeetCode: *Number of Steps to Reduce a Number to Zero*

- **Day 3: Modern C# & LINQ**
  - Topics: Generics, Delegates, Lambdas, Events, LINQ queries (`Where`, `Select`, `GroupBy`, `OrderBy`).
  - Deliverable: LINQ queries file (8+ operations on Employee/Department dataset) with unit-testable methods.
  - LeetCode: *Contains Duplicate*

- **Day 4: Unit Testing & Async Programming**
  - Topics: xUnit, Arrange-Act-Assert, deterministic tests, `async`/`await`, `Task`, `CancellationToken`.
  - Deliverable: xUnit test suite (5+ tests) + Async data fetching method with cancellation support.
  - LeetCode: *Best Time to Buy and Sell Stock*

- **Day 5: Capstone Mini-Build & Gate 1 Assessment**
  - Topics: Refactoring, debugging, boundary testing, regression test demonstration.
  - Deliverable: Functional structured C# mini-application + full test suite + fixed injected bug with regression test.
  - LeetCode: *Valid Anagram*
  - **Gate 1 Checkpoint**: Working code, passing unit tests, live code walkthrough.

---

### Week 2: ASP.NET Core Web API (Gate 2)
- **Day 6: ASP.NET Core & RESTful APIs**
  - Topics: Request lifecycle, Controllers, Routing, DTOs, HTTP verbs & status codes, Swagger/OpenAPI.
  - Deliverable: Working Web API with Employee & Department CRUD + Swagger + Postman/`.http` test collection.

- **Day 7: Dependency Injection & Custom Middleware**
  - Topics: Service Lifetimes (Transient, Scoped, Singleton), Constructor Injection, Middleware pipeline.
  - Deliverable: DI registered services + Custom request/response logging middleware + architecture diagram.

- **Day 8: Layered Architecture & Request Validation**
  - Topics: Clean 3-Tier separation (API, Application/Services, Infrastructure/Data), FluentValidation, Global Exception Handling.
  - Deliverable: Refactored solution structure + validation rules + centralized exception handler.

- **Day 9: Entity Framework Core & SQL Integration**
  - Topics: `DbContext`, `DbSet`, Entity relationships, Migrations, Tracking vs `AsNoTracking`, Pagination/Filtering.
  - Deliverable: EF Core database integration with SQL Server migrations, 1-to-many relationship (Department to Employees), pagination.

- **Day 10: Security, JWT & Gate 2 Checkpoint**
  - Topics: Authentication vs Authorization, JWT tokens, Claims & Roles, 401 vs 403, secret management.
  - Deliverable: Secure endpoints with JWT, role-based policies, automated tests for authorization rules.
  - **Gate 2 Checkpoint**: Full API demonstration with DB persistence, security, and architectural walkthrough.

---

## 4. Proposed Solution Structure

```
D:\sai\dotnet\
│
├── .gitignore
├── PLAN.md
│
└── EmployeeManagement/
    ├── EmployeeManagement.sln
    │
    ├── src/
    │   ├── EmployeeManagement.API/             # Controllers, Middlewares, Program.cs
    │   ├── EmployeeManagement.Application/     # Services, DTOs, Validators, Interfaces
    │   ├── EmployeeManagement.Domain/          # Core entities (Employee, Department), Enums
    │   └── EmployeeManagement.Infrastructure/  # EF Core DbContext, Repositories, Migrations
    │
    └── tests/
        ├── EmployeeManagement.UnitTests/       # xUnit unit tests
        └── EmployeeManagement.IntegrationTests/# Integration & API tests
```
