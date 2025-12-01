# Calculator Library - TDD vs Traditional Development Demo

## Executive Summary

This project demonstrates the difference between two development approaches for a simple calculator library:

1. **Traditional Development** (`handson/traditional/Calculator`) - coding without tests, demonstrating common pitfalls
2. **Test-Driven Development** (`handson/test-driven/Calculator` + `Calculator.Tests`) - systematic, testable approach showcasing best practices

The goal is to create educational material for workshop participants to understand the value of TDD.

---

## Functional Requirements

### Calculator Operations

| Operation | Method | Description |
|-----------|--------|-------------|
| Add | `Add(a, b)` | Returns sum of two numbers |
| Subtract | `Subtract(a, b)` | Returns difference of two numbers |
| Multiply | `Multiply(a, b)` | Returns product of two numbers |
| Divide | `Divide(a, b)` | Returns quotient of two numbers |
| Square Root | `SquareRoot(n)` | Returns square root of a number |
| Power | `Power(base, exp)` | Returns base raised to exponent |
| Modulo | `Modulo(a, b)` | Returns remainder of division |

### Interactive Console Demo
- REPL-style interface with user input
- Allow users to select operations and enter numbers
- Display results or errors appropriately

---

## Non-Functional Requirements

### Traditional Version (Intentional Anti-patterns)
- **Edge case bugs**: Division by zero not handled, negative square root crashes
- **Poor structure**: Tightly coupled code, static methods everywhere
- **Hard to test**: No interfaces, no dependency injection
- **No separation of concerns**: Business logic mixed with console I/O

### TDD Version (Best Practices)
- **Interface-based design**: `ICalculator` interface for dependency injection
- **Result pattern**: `Result<T>` for error handling (no exceptions for expected errors)
- **Comprehensive tests**: Happy path + edge cases + boundary values + Theory/InlineData
- **Clean architecture**: Separation of concerns, testable components

---

## Technical Specifications

### Traditional Version Structure
```
handson/traditional/Calculator/
├── Calculator.csproj
├── Program.cs          # Everything in one file, static methods
└── (no tests)
```

### TDD Version Structure
```
handson/test-driven/
├── Calculator/
│   ├── Calculator.csproj
│   ├── Program.cs           # Interactive console demo
│   ├── ICalculator.cs       # Interface definition
│   ├── Calculator.cs        # Implementation
│   └── Result.cs            # Result<T> pattern implementation
└── Calculator.Tests/
    ├── Calculator.Tests.csproj
    └── CalculatorTests.cs   # Comprehensive test suite
```

### Result Pattern Design
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    public static Result<T> Success(T value);
    public static Result<T> Failure(string error);
}
```

---

## Acceptance Criteria

### Traditional Version
- [ ] All 7 operations implemented as static methods
- [ ] No unit tests
- [ ] Contains intentional bugs (division by zero, negative sqrt)
- [ ] Tightly coupled design (hard to unit test)
- [ ] Interactive console that may crash on edge cases

### TDD Version
- [ ] `ICalculator` interface defined with all 7 operations
- [ ] `Calculator` class implements `ICalculator`
- [ ] `Result<T>` pattern used for error handling
- [ ] Comprehensive test suite with:
  - [ ] Happy path tests for each operation
  - [ ] Edge case tests (div by zero, negative sqrt, etc.)
  - [ ] Boundary value tests (max/min values, zero)
  - [ ] Theory/InlineData parameterized tests
- [ ] All tests pass
- [ ] Interactive console demo works correctly

---

## Dependencies and Constraints

### Technology Stack
- .NET (existing project structure)
- C# language
- xUnit test framework (already configured)

### Constraints
- Must work with existing project structure in `handson/` directory
- Educational focus - code should be clear and demonstrative

---

## User Stories

1. **As a workshop participant**, I want to see a traditional implementation with hidden bugs, so I can understand what can go wrong without tests.

2. **As a workshop participant**, I want to see a TDD implementation with comprehensive tests, so I can learn best practices for testable code.

3. **As a workshop facilitator**, I want an interactive demo, so participants can try the calculator and discover bugs themselves.

---

## Assumptions

- This is for educational/workshop purposes (not production code)
- Participants have basic C# and .NET knowledge
- The focus is on demonstrating TDD benefits, not building a feature-complete calculator
- Numbers are `double` type for simplicity

---

## Q&A History

**Q1: What calculator operations should the library support?**
- A1: Basic + Advanced (Add, Subtract, Multiply, Divide + Square root, Power, Modulo)

**Q2: What 'pitfalls' of traditional development should be demonstrated?**
- A2: Both issues - edge case bugs (division by zero, negative square root) AND poor structure/design (tightly coupled, static methods)

**Q3: How should the TDD version be designed to showcase testability?**
- A3: Interface-based design (ICalculator interface with dependency injection support)

**Q4: How should the TDD version handle errors (e.g., division by zero)?**
- A4: Result pattern - return Result<T> with success/failure (functional approach)

**Q5: What level of test coverage should be demonstrated in the TDD version?**
- A5: Comprehensive - Happy path + edge cases + boundary values + Theory/InlineData examples

**Q6: Should Program.cs include a console demo showing the calculator in action?**
- A6: Yes, interactive REPL-style calculator with user input

---

## Open Questions

*None - all requirements have been clarified.*
