# Day 4: Unit Testing (xUnit), Asynchronous C# & .NET Tooling

## Overview
This project fulfills the **Day 4 Training Deliverables** of the .NET Engineering Bridge program. It introduces an enterprise unit-testing architecture using **xUnit** (`EmployeeManagement.UnitTests`), asynchronous programming with `async`/`await`, non-blocking I/O simulation, cooperative cancellation with `CancellationToken`, concurrent operations with `Task.WhenAll`, and algorithmic problem-solving.

---

## Deliverables Checklist
- [x] Dedicated xUnit test project (`tests/EmployeeManagement.UnitTests`) configured in solution.
- [x] Comprehensive test suites covering domain models, calculation logic, LINQ queries, and async flows.
- [x] **27 Passing Unit Tests** structured strictly following the **Arrange-Act-Assert (AAA)** pattern.
- [x] Asynchronous service with cooperative cancellation handling (`AsyncEmployeeService.cs`).
- [x] Concurrent aggregation using `Task.WhenAll`.
- [x] Day 4 LeetCode Solution: **121. Best Time to Buy and Sell Stock** (One-pass $O(N)$ Greedy).
- [x] Detailed documentation of .NET CLI workflow (`restore`, `build`, `test`), `Task` vs `Thread`, and testing principles.

---

## Unit Testing Architecture & Test Results

```
tests/EmployeeManagement.UnitTests/
├── DomainTests/
│   ├── EmployeeTests.cs             # Polymorphic pay, validations, record equality (5 tests)
│   ├── DepartmentTests.cs           # Encapsulation, duplicate prevention, budget health (4 tests)
│   ├── PayrollCalculatorTests.cs    # Tax deductions, out parameter, invalid rates (2 tests)
│   └── LinqQueryServiceTests.cs     # Where, SelectMany deduplication, pagination (3 tests)
├── AsyncTests/
│   └── AsyncEmployeeServiceTests.cs # Non-blocking fetch, cancellation, Task.WhenAll (3 tests)
└── LeetCodeTests/
    └── BestTimeToBuyAndSellStockTests.cs # Standard cases, decreasing, empty (6 tests)
```

### Test Execution Summary
```text
Starting test execution...
  Passed: 27
  Failed: 0
  Skipped: 0
Total time: 2.9972 Seconds
Status: 100% PASS
```

---

## Key Technical Concepts & Interview Q&A

### 1. The Arrange-Act-Assert (AAA) Pattern
AAA is the industry-standard pattern for writing clean, readable, deterministic unit tests:
- **Arrange:** Set up the test fixtures, parameters, dependencies, and expected outcome.
- **Act:** Execute the single method or property under test.
- **Assert:** Verify that the actual result matches expectations and invariants were preserved.

```csharp
[Fact]
public void CalculateNetPay_ValidEmployee_ShouldDeductTaxAccurately()
{
    // Arrange
    var calculator = new StandardPayrollCalculator(taxRate: 0.20m);
    var employee = new FullTimeEmployee(1, "David", "david@company.com", 1, annualSalary: 120000m);

    // Act
    decimal netPay = calculator.CalculateNetPay(employee, out decimal taxDeducted);

    // Assert
    Assert.Equal(2000m, taxDeducted);
    Assert.Equal(8000m, netPay);
}
```

### 2. What Makes a Good Unit Test? (F.I.R.S.T. Principles)
- **Fast:** Runs in milliseconds so developers run them continuously on every save.
- **Independent / Isolated:** Tests must never depend on the execution order or shared state of other tests.
- **Repeatable:** Produces the identical outcome in any environment (local, CI/CD, docker) without relying on live external databases or networks.
- **Self-Validating:** Automatically determines PASS/FAIL via assertions without human inspection of logs.
- **Timely / Thorough:** Written alongside code covering normal paths, edge cases, and invalid inputs.

### 3. `Task` vs. `Thread` in C#
| Characteristic | OS Thread (`System.Threading.Thread`) | Task (`System.Threading.Tasks.Task`) |
| :--- | :--- | :--- |
| **Abstraction Level** | Low-level OS kernel thread abstraction. | High-level abstraction representing an asynchronous promise/operation. |
| **Resource Cost** | Expensive (~1 MB stack memory per thread, kernel context switching overhead). | Lightweight managed object allocated on the heap; runs on the ThreadPool. |
| **I/O Asynchrony** | Blocks the physical thread while waiting for I/O. | Uses completion ports (IOCP); **zero threads are blocked** while waiting on network/disk. |
| **Composition** | Difficult to coordinate or cancel. | Rich combinators: `Task.WhenAll`, `Task.WhenAny`, and `CancellationToken`. |

### 4. What Do `restore`, `build`, and `test` Do?
- **`dotnet restore`**: Reads project files (`.csproj`), queries package feeds (NuGet), and generates `project.assets.json` resolving all required external packages and dependencies.
- **`dotnet build`**: Compiles C# source code into Intermediate Language (IL) contained inside `.dll` assemblies, performing type checks, syntax validation, and dependency linking. (Implicitly calls restore if needed).
- **`dotnet test`**: Builds the solution, activates the test runner (VSTest / xUnit), discovers all methods decorated with `[Fact]` and `[Theory]`, executes them, and reports test results.

---

## LeetCode Problem Log: 121. Best Time to Buy and Sell Stock

- **Problem:** Maximize single-day profit given stock prices array `prices`.
- **Approach (One-Pass Greedy):**
  - Track `minPrice` observed so far.
  - If current price is lower than `minPrice`, update `minPrice`.
  - Else calculate `currentPrice - minPrice`; if greater than `maxProfit`, update `maxProfit`.
- **Complexity:**
  - **Time Complexity:** $O(N)$ — single iteration through prices.
  - **Space Complexity:** $O(1)$ — requires only two scalar integer variables.
- **Edge Cases Tested:**
  1. Standard profit: `[7, 1, 5, 3, 6, 4]` $\rightarrow$ Buy at 1, sell at 6 = Profit `5`.
  2. Decreasing prices: `[7, 6, 4, 3, 1]` $\rightarrow$ Profit `0`.
  3. Single-day array: `[5]` $\rightarrow$ Profit `0`.
  4. Empty array: `[]` $\rightarrow$ Profit `0`.

---

## How to Run Day 4

### 1. Run All 27 xUnit Tests
```powershell
dotnet test "D:\sai\dotnet\EmployeeManagement.sln"
```

### 2. Run Automated Console Demos & LeetCode Tests
```powershell
dotnet run --project "D:\sai\dotnet\src\EmployeeManagement.ConsoleApp" -- --demo
```

### 3. Run Interactive CLI Menu
```powershell
dotnet run --project "D:\sai\dotnet\src\EmployeeManagement.ConsoleApp"
```
*(Option 8 runs the Async & Cancellation demo; Option 12 runs LeetCode 121).*
