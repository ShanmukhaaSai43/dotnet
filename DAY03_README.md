# Day 3: Modern C#, Collections & LINQ Data Transformations

## Overview
This project fulfills the **Day 3 Training Deliverables** of the .NET Engineering Bridge program. It implements an extensive set of LINQ queries, projections, grouping, and aggregations over in-memory domain collections (`EmployeeManagement.Domain`), demonstrating deferred execution, custom extension methods with `yield return`, delegates, events, modern pattern matching, and algorithmic problem-solving.

---

## Deliverables Checklist
- [x] LINQ exercise service (`LinqQueryService.cs`) featuring 10 queries.
- [x] Unit-testable methods for filtering, grouping, sorting, and projecting.
- [x] Input, output, and algorithmic time/space complexity documented for every query.
- [x] Custom extension method demonstrating deferred execution (`yield return`).
- [x] Custom delegates and event publisher-subscriber pattern (`EmployeeNotifier`).
- [x] Modern C# pattern matching with switch expressions & relational patterns.
- [x] Day 3 LeetCode Solution: **217. Contains Duplicate** (Optimal $O(N)$ Hash Set + LINQ).
- [x] Interview preparation Q&A (`IEnumerable` vs `IQueryable`, `List` vs `Dictionary`, deferred execution).

---

## 10 LINQ Queries Specification & Complexity

| Query # | Name & Operator | Input Description | Output Description | Time Complexity | Space Complexity |
| :---: | :--- | :--- | :--- | :---: | :---: |
| **Q1** | **Filtering (`Where`)** | `IEnumerable<Employee>`, `minMonthlyPay` | Filtered list of active high-earning employees | $O(N)$ | $O(K)$ |
| **Q2** | **Projection & Join (`Select` & `Join`)** | Employees & Departments collections | Flattens into `EmployeeSummaryDto` with department names | $O(N + M)$ | $O(N)$ |
| **Q3** | **Multi-Column Sorting (`OrderBy` & `ThenBy`)** | `IEnumerable<Employee>` | Sorted list: Department ascending, MonthlyPay descending | $O(N \log N)$ | $O(N)$ |
| **Q4** | **Grouping & Aggregation (`GroupBy`)** | Employees & Departments collections | List of `DepartmentPayrollReportDto` with Headcount, Totals, Averages, and Budget status | $O(N)$ | $O(D)$ |
| **Q5** | **Lookup Dictionary (`ToDictionary`)** | `IEnumerable<Employee>` | `Dictionary<int, List<Employee>>` for $O(1)$ department employee lookups | $O(N)$ | $O(N)$ |
| **Q6** | **Collection Flattening (`SelectMany`)** | List of string arrays (`string[][]`) | Single distinct sorted list of all unique skills across all employees | $O(N \cdot S)$ | $O(U)$ |
| **Q7** | **Custom Reduction (`Aggregate`)** | `IEnumerable<Employee>` | Cumulative total monthly payroll obligation | $O(N)$ | $O(1)$ |
| **Q8** | **Deduplication (`DistinctBy`)** | `IEnumerable<Employee>` | First representative employee from each unique city | $O(N)$ | $O(C)$ |
| **Q9** | **Pagination (`Skip` & `Take`)** | `IEnumerable<Employee>`, `pageNumber`, `pageSize` | Subset page of employees and total record count | $O(\text{Skip} + \text{Take})$ | $O(\text{Take})$ |
| **Q10**| **Pattern Matching Switch** | Single `Employee` entity | String classification tier based on employee type and salary | $O(1)$ | $O(1)$ |

---

## Key Concepts & Interview Q&A

### 1. `IEnumerable<T>` vs. `IQueryable<T>`
- **`IEnumerable<T>` (In-Memory Processing):**
  - Found in `System.Collections.Generic`.
  - Executes in memory (LINQ to Objects).
  - All data is loaded into application memory first; filters and projections execute on the client side.
  - Ideal for local collections, lists, arrays.
- **`IQueryable<T>` (Out-of-Process / Database Processing):**
  - Found in `System.Linq`.
  - Extends `IEnumerable<T>` by wrapping an **Expression Tree** (`Expression`).
  - Translates the LINQ expressions directly into native SQL queries before execution on the database engine.
  - Filters (`Where`), projections (`Select`), and pagination (`Skip/Take`) execute directly inside SQL Server.

### 2. What is Deferred Execution (Lazy Evaluation)?
- In LINQ, queries are **not executed at the time they are defined**.
- Instead, the query expression is stored as an iterator. Execution is deferred until the data is actually iterated over (e.g. using a `foreach` loop, or calling immediate execution methods like `.ToList()`, `.ToArray()`, `.Count()`, or `.First()`).
- **Benefit:** Allows composing dynamic queries (e.g. adding filters conditionally) without incurring unnecessary computation or database roundtrips.

### 3. `List<T>` vs. `Dictionary<TKey, TValue>` — Lookup Performance
- **`List<T>`**:
  - Linear search: Finding an element by ID (`list.FirstOrDefault(x => x.Id == id)`) checks elements sequentially $\rightarrow$ **$O(N)$ time**.
  - For 100,000 items, searching takes up to 100,000 comparisons.
- **`Dictionary<TKey, TValue>`**:
  - Hash-table lookup: `dict[id]` or `dict.TryGetValue(id, out val)` computes the hash code of the key $\rightarrow$ **$O(1)$ amortized time**.
  - For 100,000 items, lookup takes essentially 1 operation.

---

## LeetCode Problem Log: 217. Contains Duplicate

- **Problem:** Given an integer array `nums`, return `true` if any value appears at least twice in the array, and `false` if every element is distinct.
- **Approach:**
  - Initialize a `HashSet<int>`.
  - For each number `n` in `nums`:
    - Call `seen.Add(n)`. In .NET, `HashSet.Add(x)` returns `false` if the item is already present!
    - If `false`, immediately return `true` (short-circuit).
  - Return `false` if loop finishes.
- **Complexity:**
  - **Time Complexity:** $O(N)$ average time — single pass with $O(1)$ hash set insertions.
  - **Space Complexity:** $O(N)$ auxiliary space to store unique numbers.
- **Edge Cases Tested:**
  1. Standard with duplicates: `[1, 2, 3, 1]` $\rightarrow$ `true`.
  2. All unique elements: `[1, 2, 3, 4]` $\rightarrow$ `false`.
  3. Multiple repeating duplicates: `[1, 1, 1, 3, ...]` $\rightarrow$ `true`.
  4. Single-element array: `[42]` $\rightarrow$ `false`.
  5. Negative duplicate elements: `[-1, -1]` $\rightarrow$ `true`.

---

## How to Run Day 3

### Automated Verification Mode (Runs all 3 days + LINQ Demos + All 3 LeetCode tests)
```powershell
dotnet run --project "D:\sai\dotnet\src\EmployeeManagement.ConsoleApp" -- --demo
```

### Interactive Menu
```powershell
dotnet run --project "D:\sai\dotnet\src\EmployeeManagement.ConsoleApp"
```
*(Choose Option 7 for Day 3 LINQ Demos, or Option 10 for Day 3 LeetCode 217).*
