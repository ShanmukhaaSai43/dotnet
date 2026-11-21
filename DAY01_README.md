# Day 1: C# Fundamentals & Employee Management Console Application

## Overview
This project fulfills the **Day 1 Training Deliverables** of the .NET Engineering Bridge program. It implements an **Employee & Department Management** console application modeling core domain entities, illustrating C# syntax, type semantics, memory allocation, exception handling, and algorithmic problem-solving.

---

## Deliverables Checklist
- [x] Console application written in idiomatic C# (.NET 8.0).
- [x] Demonstrates Value types, Reference types, Nullable types (`int?`, `string?`).
- [x] Custom exceptions and defensive input validation.
- [x] Methods demonstrating `out`, `ref`, and optional parameters.
- [x] High-performance report generation using `StringBuilder`.
- [x] Day 1 LeetCode Solution: **1929. Concatenation of Array**.
- [x] README with run instructions, 3 sample inputs/outputs, and technical notes.

---

## How to Run

### Run the Application (Interactive Mode)
```powershell
dotnet run --project "D:\sai\dotnet\src\EmployeeManagement.ConsoleApp"
```

### Run the Application in Automated Verification Mode (Headless)
```powershell
dotnet run --project "D:\sai\dotnet\src\EmployeeManagement.ConsoleApp" -- --demo
```

### Run Build & Test Commands
```powershell
dotnet build "D:\sai\dotnet\EmployeeManagement.sln"
```

---

## 3 Sample Inputs & Outputs

### Sample 1: Adding a Complete Employee Record
**Input:**
```csharp
service.AddEmployee(
    name: "David Miller",
    email: "david.miller@company.com",
    department: Department.Operations,
    salary: 68000m,
    bonus: 4500m,
    phone: "555-0144",
    address: new EmployeeAddress("456 Industrial Way", "Chicago", "IL", "60601"),
    skills: new[] { "Logistics", "Process Optimization", "ERP" }
);
```
**Output:**
```text
[OUTPUT]: Employee successfully created!
  [ID: 104] David Miller | Dept: Operations | Salary: $68,000.00 | Bonus: $4,500.00 | Phone: 555-0144 | City: Chicago | Active: True
  Total Compensation: $72,500.00
```

---

### Sample 2: Adding an Employee with Nullable Fields Omitted
**Input:**
```csharp
service.AddEmployee(
    name: "Elena Rostova",
    email: "elena.r@company.com",
    department: Department.Marketing,
    salary: 58000m
);
```
**Output:**
```text
[OUTPUT]: Employee successfully created with defaults!
  [ID: 105] Elena Rostova | Dept: Marketing | Salary: $58,000.00 | Bonus: N/A | Phone: Not Provided | City:  | Active: True
  Bonus HasValue: False (Demonstrating Nullable<decimal>)
  Phone is null: True
```

---

### Sample 3: Defensive Exception Handling on Invalid Input
**Input:**
```csharp
service.AddEmployee(
    name: "Test Bad User",
    email: "bad-email-no-at",
    department: Department.Engineering,
    salary: 15000m // Under minimum salary threshold
);
```
**Output:**
```text
[OUTPUT - Handled Gracefully]: Caught EmployeeValidationException!
  Field: email
  Message: A valid email address containing '@' is required.
```

---

## Technical Notes: Value vs Reference Types Used

| Type Name | Classification | Memory Location | Behavior in Code |
| :--- | :--- | :--- | :--- |
| `int`, `decimal`, `bool`, `DateTime` | Built-in Value Type | Stack / Inlined inside class | Holds actual data. Passed by value (copy) unless modified by `ref`/`out`. |
| `Department` (Enum) | Value Type | Stack / Inlined | Type-safe integer constant enumeration. |
| `EmployeeAddress` (Struct) | Custom Value Type | Stack / Inlined | Copying assigns a distinct copy of all struct fields without modifying original. |
| `decimal?`, `int?` | Nullable Value Type (`Nullable<T>`) | Stack / Inlined | Holds a boolean `HasValue` flag and the underlying primitive value. |
| `Employee` | Reference Type (Class) | Managed Heap | Variable holds a pointer/reference to object in heap. Passing copies the reference. |
| `string` | Reference Type (Immutable) | Managed Heap | Strings are immutable; each modification creates a new string object on the heap. |
| `string[]` (Array) | Reference Type | Managed Heap | Fixed-size collection stored on the heap with contiguous memory elements. |
| `StringBuilder` | Reference Type (Class) | Managed Heap | Mutable buffer used to construct tabular reports efficiently without heap fragmentation. |

---

## LeetCode Problem Log: 1929. Concatenation of Array

- **Problem Statement:** Given an integer array `nums` of length `n`, return an array `ans` of length `2n` where `ans[i] == nums[i]` and `ans[i + n] == nums[i]`.
- **Approach:** 
  1. Allocate a single array of size `2 * nums.Length`.
  2. Single loop from `0` to `n - 1`, setting `ans[i] = nums[i]` and `ans[i + n] = nums[i]`.
- **Complexity:**
  - **Time Complexity:** $O(N)$ — single pass over the input array.
  - **Space Complexity:** $O(N)$ — allocated output array (auxiliary complexity is $O(1)$).
- **Edge Cases Tested:**
  1. Single element array (`[7]` $\rightarrow$ `[7, 7]`).
  2. Array with duplicate elements (`[1, 2, 1]` $\rightarrow$ `[1, 2, 1, 1, 2, 1]`).
  3. Negative values and zero (`[-5, 0, 10]` $\rightarrow$ `[-5, 0, 10, -5, 0, 10]`).
- **Lesson Learned:** Direct index calculation `ans[i + n]` enables populating both halves in one pass without calling resize or multiple copy operations.

---

## Interview Questions & Answers

### 1. What is the difference between Value Types and Reference Types in C#?
- **Value Types** (`struct`, `enum`, `int`, `double`, `bool`) hold the data directly and are typically allocated on the thread's execution stack (or inline inside heap objects). When assigned to a new variable or passed to a method, a full bitwise copy of the data is made.
- **Reference Types** (`class`, `interface`, `delegate`, `string`, arrays) store a reference (memory pointer) on the stack pointing to memory on the Managed Heap. When copied, only the reference pointer is copied, so both variables point to the same object instance.

### 2. String vs. StringBuilder — When should you use which?
- `string` is **immutable**. Any modification (like `+` concatenation) allocates a brand new string on the garbage-collected heap and copies the characters.
- `StringBuilder` is **mutable**. It maintains an internal character buffer that dynamically expands as needed without allocating new string objects on every append.
- **Rule of thumb:** Use `string` for simple operations, constants, and few concatenations. Use `StringBuilder` inside loops, dynamic report generators, or when concatenating text more than 4-5 times.

### 3. What are Nullable Reference Types (NRT) in modern C#?
Introduced in C# 8, Nullable Reference Types allow developers to annotate references:
- Non-nullable: `string name` — compiler warns if assigned `null` or if dereferenced without initialization.
- Nullable: `string? notes` — explicit intent that the reference can be null. Requires null checking (`?.`, `??`, `if (notes != null)`) before dereferencing, reducing `NullReferenceException` bugs at runtime.
