# Expression Parser Calculator Library - Requirements Specification

## Status: Complete

---

## Executive Summary

A demonstration project comparing two software development approaches for teaching junior developers:

1. **Traditional Approach** (`/handson/traditional/Calculator`) - Implementation without tests
2. **Test-Driven Approach** (`/handson/test-driven/Calculator` + `Calculator.Tests`) - Systematic TDD methodology

Both projects implement an **expression parser calculator library** with identical functionality. The key difference is the TDD version includes comprehensive test coverage, demonstrating how tests enable safer maintenance and extensibility.

**Target Platform:** .NET 9.0 (C#, nullable enabled, implicit usings)

---

## Functional Requirements

### FR-1: Expression Parsing

The calculator must parse and evaluate mathematical expressions with:

| Feature | Description | Examples |
|---------|-------------|----------|
| **Operators** | Basic arithmetic | `+`, `-`, `*`, `/` |
| **Parentheses** | Grouping | `(2 + 3) * 4` |
| **Unary minus** | Negation | `-5`, `-(3+2)`, `3*-2` |
| **Precedence** | PEMDAS/BODMAS | `2 + 3 * 4 = 14` |
| **Associativity** | Left-to-right | `8 / 4 / 2 = 1` |

### FR-2: Number Types

- Support both **integers** and **floating-point** numbers
- Auto-detect type based on input (`5` vs `5.0`)
- Promote to float when needed (e.g., division results)
- Examples: `5`, `3.14`, `-2.5`, `0.001`

### FR-3: Error Handling

Use the **`Result<T>` pattern** (discriminated union):

```csharp
// Success case
Result<double>.Success(42.0)

// Failure cases
Result<double>.Failure("Division by zero")
Result<double>.Failure("Unexpected character ')' at position 5")
Result<double>.Failure("Mismatched parentheses")
```

Error types to handle:
- Syntax errors (invalid characters, malformed expressions)
- Runtime errors (division by zero)
- Parse errors (mismatched parentheses, unexpected tokens)

### FR-4: REPL Interface

The `Program.cs` provides a Read-Eval-Print Loop with:

| Command | Description |
|---------|-------------|
| *expression* | Evaluate and display result |
| `help` | Show available commands and syntax |
| `clear` | Clear the console screen |
| `exit` | Exit the REPL |

Example session:
```
> 2 + 3 * 4
14
> (2 + 3) * 4
20
> 10 / 0
Error: Division by zero
> help
Expression Calculator - Commands:
  <expression>  Evaluate a mathematical expression
  help          Show this help message
  clear         Clear the screen
  exit          Exit the calculator
> exit
Goodbye!
```

---

## Non-Functional Requirements

### NFR-1: Architecture (Separate Components)

The library must use a **pipeline architecture**:

```
Input String → [Lexer] → Tokens → [Parser] → AST → [Evaluator] → Result<double>
```

Components:
1. **Lexer/Tokenizer** - Converts input string to token stream
2. **Parser** - Builds Abstract Syntax Tree using recursive descent
3. **Evaluator** - Evaluates AST to produce result

### NFR-2: Testability (TDD Version)

The test-driven version must include:
- Unit tests for each component (Lexer, Parser, Evaluator)
- Integration tests for end-to-end expression evaluation
- Edge case coverage (empty input, whitespace, error conditions)
- Minimum **80%+ code coverage** for library code

### NFR-3: Extensibility

Design for future extensions (workshop attendees will add):
- Variables and assignment (`x = 5`, then `x + 3`)
- Additional operators (power `^`, modulo `%`)
- Built-in functions (`sqrt()`, `abs()`, `min()`, `max()`)

### NFR-4: Code Quality

- Use nullable reference types (`#nullable enable`)
- Follow C# naming conventions
- Keep methods focused and testable
- Avoid over-engineering for current scope

---

## Technical Design

### Token Types

```csharp
public enum TokenType
{
    Number,        // 42, 3.14
    Plus,          // +
    Minus,         // -
    Star,          // *
    Slash,         // /
    LeftParen,     // (
    RightParen,    // )
    EndOfInput     // EOF marker
}
```

### AST Node Types

```csharp
public abstract record Expression;
public record NumberExpression(double Value) : Expression;
public record BinaryExpression(Expression Left, TokenType Operator, Expression Right) : Expression;
public record UnaryExpression(TokenType Operator, Expression Operand) : Expression;
```

### Result Type

```csharp
public abstract record Result<T>
{
    public record Success(T Value) : Result<T>;
    public record Failure(string Error) : Result<T>;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;
}
```

### Grammar (Recursive Descent)

```
expression     → term (('+' | '-') term)*
term           → unary (('*' | '/') unary)*
unary          → '-' unary | primary
primary        → NUMBER | '(' expression ')'
```

---

## Acceptance Criteria

### Basic Operations
- [ ] `2 + 3` → `5`
- [ ] `10 - 4` → `6`
- [ ] `3 * 4` → `12`
- [ ] `15 / 3` → `5`

### Precedence
- [ ] `2 + 3 * 4` → `14`
- [ ] `10 - 4 / 2` → `8`
- [ ] `2 * 3 + 4 * 5` → `26`

### Parentheses
- [ ] `(2 + 3) * 4` → `20`
- [ ] `((2 + 3))` → `5`
- [ ] `(2 + (3 * 4))` → `14`

### Unary Minus
- [ ] `-5` → `-5`
- [ ] `--5` → `5`
- [ ] `-(2 + 3)` → `-5`
- [ ] `3 * -2` → `-6`

### Floating Point
- [ ] `3.14 + 2.86` → `6`
- [ ] `10 / 4` → `2.5`
- [ ] `0.1 + 0.2` → `0.3` (approximately)

### Error Handling
- [ ] `10 / 0` → Error: Division by zero
- [ ] `2 + ` → Error: Unexpected end of input
- [ ] `(2 + 3` → Error: Mismatched parentheses
- [ ] `2 @ 3` → Error: Unexpected character '@'

### REPL Commands
- [ ] `help` displays command list
- [ ] `clear` clears screen
- [ ] `exit` terminates program

---

## Project Structure

### Traditional (`/handson/traditional/Calculator`)

```
Calculator/
├── Calculator.csproj
├── Program.cs           # REPL entry point
├── Lexer.cs             # Tokenizer
├── Parser.cs            # Recursive descent parser
├── Evaluator.cs         # Expression evaluator
├── Token.cs             # Token types
├── Expression.cs        # AST nodes
└── Result.cs            # Result<T> type
```

### Test-Driven (`/handson/test-driven/`)

```
Calculator/
├── Calculator.csproj
├── Program.cs
├── Lexer.cs
├── Parser.cs
├── Evaluator.cs
├── Token.cs
├── Expression.cs
└── Result.cs

Calculator.Tests/
├── Calculator.Tests.csproj
├── LexerTests.cs
├── ParserTests.cs
├── EvaluatorTests.cs
└── IntegrationTests.cs
```

---

## Constraints and Dependencies

- **.NET 9.0** runtime required
- **xUnit** for test framework (test-driven version)
- No external NuGet packages for core functionality
- Console application (no GUI)

---

## Q&A History

| # | Question | Answer |
|---|----------|--------|
| 1 | Operations? | Basic arithmetic: `+`, `-`, `*`, `/` with parentheses |
| 2 | Precedence? | PEMDAS/BODMAS with left-to-right associativity |
| 3 | Number types? | Both integers and floating-point, auto-detect |
| 4 | Error handling? | `Result<T>` pattern (success/failure union) |
| 5 | REPL features? | Basic commands: expression eval, `help`, `exit`, `clear` |
| 6 | Target audience? | Junior developers new to TDD |
| 7 | Parser type? | Recursive descent (good for teaching) |
| 8 | Architecture? | Separate components: Lexer → Parser → Evaluator |
| 9 | Unary minus? | Full support: `-5`, `-(3+2)`, `3*-2` |
| 10 | Demo approach? | Same functionality, TDD has high test coverage for maintainability |
| 11 | Future extensions? | Variables, additional operators, functions |

---

## Open Questions

*None - all requirements gathered.*
