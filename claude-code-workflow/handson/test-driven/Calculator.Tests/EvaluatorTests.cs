using Calculator;

namespace Calculator.Tests;

using Moq;

/// <summary>
/// Comprehensive unit tests for the Evaluator class.
/// Tests are organized by category: basic operations, operator precedence,
/// parentheses (via AST structure), unary operations, division by zero,
/// floating-point arithmetic, complex expressions, and nested unary operations.
///
/// All tests construct Expression AST nodes directly to test the Evaluator
/// in isolation from the Lexer and Parser.
/// </summary>
public class EvaluatorTests
{
    #region Helper Methods

    /// <summary>
    /// Creates a Number expression node.
    /// </summary>
    private static Expression Num(double value) => new Expression.Number(value);

    /// <summary>
    /// Creates a Binary expression node.
    /// </summary>
    private static Expression Binary(Expression left, TokenType op, Expression right) =>
        new Expression.Binary(left, op, right);

    /// <summary>
    /// Creates a Unary expression node.
    /// </summary>
    private static Expression Unary(TokenType op, Expression operand) =>
        new Expression.Unary(op, operand);

    /// <summary>
    /// Asserts that the result is a success with the expected value.
    /// </summary>
    private static void AssertSuccess(Result<double> result, double expected, int precision = 10)
    {
        Assert.True(result.IsSuccess, $"Expected success but got failure: {(result is Result<double>.Failure f ? f.Error : "unknown")}");
        var actual = ((Result<double>.Success)result).Value;
        Assert.Equal(expected, actual, precision);
    }

    /// <summary>
    /// Asserts that the result is a failure containing the expected error substring.
    /// </summary>
    private static void AssertFailure(Result<double> result, string expectedErrorSubstring)
    {
        Assert.True(result.IsFailure, "Expected failure but got success");
        var error = ((Result<double>.Failure)result).Error;
        Assert.Contains(expectedErrorSubstring, error);
    }

    #endregion

    #region 1. Basic Operations Tests

    [Fact]
    public void Evaluate_Addition_2Plus3Equals5()
    {
        // 2 + 3 = 5

        // Arrange
        var expr = Binary(
            Num(2),
            TokenType.Plus,
            Num(3));

        // Act
        var result = Evaluator.Evaluate(expr);

        // Assert
        AssertSuccess(result, 5.0);
    }

    [Fact]
    public void Evaluate_Subtraction_10Minus4Equals6()
    {
        // 10 - 4 = 6
        var expr = Binary(Num(10), TokenType.Minus, Num(4));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 6.0);
    }

    [Fact]
    public void Evaluate_Multiplication_3Times4Equals12()
    {
        // 3 * 4 = 12
        var expr = Binary(Num(3), TokenType.Star, Num(4));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 12.0);
    }

    [Fact]
    public void Evaluate_Division_15DividedBy3Equals5()
    {
        // 15 / 3 = 5
        var expr = Binary(Num(15), TokenType.Slash, Num(3));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 5.0);
    }

    [Theory]
    [InlineData(0, 5, 5)] // 0 + 5 = 5
    [InlineData(5, 0, 5)] // 5 + 0 = 5
    [InlineData(-2, -3, -5)] // -2 + -3 = -5
    [InlineData(-5, 10, 5)] // -5 + 10 = 5
    [InlineData(100, 200, 300)] // 100 + 200 = 300
    public void Evaluate_Addition_VariousOperands(double a, double b, double expected)
    {
        var expr = Binary(Num(a), TokenType.Plus, Num(b));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, expected);
    }

    [Theory]
    [InlineData(5, 5, 0)] // 5 - 5 = 0
    [InlineData(3, 5, -2)] // 3 - 5 = -2
    [InlineData(-5, -3, -2)] // -5 - (-3) = -2
    [InlineData(0, 5, -5)] // 0 - 5 = -5
    [InlineData(100, 30, 70)] // 100 - 30 = 70
    public void Evaluate_Subtraction_VariousOperands(double a, double b, double expected)
    {
        var expr = Binary(Num(a), TokenType.Minus, Num(b));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, expected);
    }

    [Theory]
    [InlineData(5, 0, 0)] // 5 * 0 = 0
    [InlineData(0, 5, 0)] // 0 * 5 = 0
    [InlineData(42, 1, 42)] // 42 * 1 = 42
    [InlineData(-3, -4, 12)] // -3 * -4 = 12
    [InlineData(-2, 5, -10)] // -2 * 5 = -10
    [InlineData(7, 8, 56)] // 7 * 8 = 56
    public void Evaluate_Multiplication_VariousOperands(double a, double b, double expected)
    {
        var expr = Binary(Num(a), TokenType.Star, Num(b));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, expected);
    }

    [Theory]
    [InlineData(10, 2, 5)] // 10 / 2 = 5
    [InlineData(0, 5, 0)] // 0 / 5 = 0
    [InlineData(42, 1, 42)] // 42 / 1 = 42
    [InlineData(-12, -4, 3)] // -12 / -4 = 3
    [InlineData(-10, 2, -5)] // -10 / 2 = -5
    [InlineData(100, 4, 25)] // 100 / 4 = 25
    public void Evaluate_Division_VariousOperands(double a, double b, double expected)
    {
        var expr = Binary(Num(a), TokenType.Slash, Num(b));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, expected);
    }

    #endregion

    #region 2. Operator Precedence Tests (via AST structure)

    [Fact]
    public void Evaluate_Precedence_MultiplicationBeforeAddition()
    {
        // AST for: 2 + 3 * 4 = 2 + (3 * 4) = 2 + 12 = 14
        // The AST encodes precedence: addition at root, multiplication as right child
        var multiplication = Binary(Num(3), TokenType.Star, Num(4));
        var expr = Binary(Num(2), TokenType.Plus, multiplication);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 14.0);
    }

    [Fact]
    public void Evaluate_Precedence_DivisionBeforeSubtraction()
    {
        // AST for: 10 - 4 / 2 = 10 - (4 / 2) = 10 - 2 = 8
        var division = Binary(Num(4), TokenType.Slash, Num(2));
        var expr = Binary(Num(10), TokenType.Minus, division);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 8.0);
    }

    [Fact]
    public void Evaluate_Precedence_MultipleHighPrecedenceOps()
    {
        // AST for: 2 * 3 + 4 * 5 = (2 * 3) + (4 * 5) = 6 + 20 = 26
        var leftMul = Binary(Num(2), TokenType.Star, Num(3));
        var rightMul = Binary(Num(4), TokenType.Star, Num(5));
        var expr = Binary(leftMul, TokenType.Plus, rightMul);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 26.0);
    }

    [Fact]
    public void Evaluate_Precedence_MixedOperations()
    {
        // AST for: 1 + 2 * 3 - 4 / 2 = 1 + 6 - 2 = 5
        // Structure: (1 + (2 * 3)) - (4 / 2)
        var mul = Binary(Num(2), TokenType.Star, Num(3));
        var add = Binary(Num(1), TokenType.Plus, mul);
        var div = Binary(Num(4), TokenType.Slash, Num(2));
        var expr = Binary(add, TokenType.Minus, div);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 5.0);
    }

    [Fact]
    public void Evaluate_Associativity_LeftToRightSubtraction()
    {
        // AST for: 10 - 4 - 2 = (10 - 4) - 2 = 6 - 2 = 4
        var firstSub = Binary(Num(10), TokenType.Minus, Num(4));
        var expr = Binary(firstSub, TokenType.Minus, Num(2));

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 4.0);
    }

    [Fact]
    public void Evaluate_Associativity_LeftToRightDivision()
    {
        // AST for: 8 / 4 / 2 = (8 / 4) / 2 = 2 / 2 = 1
        var firstDiv = Binary(Num(8), TokenType.Slash, Num(4));
        var expr = Binary(firstDiv, TokenType.Slash, Num(2));

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 1.0);
    }

    #endregion

    #region 3. Parentheses Tests (via AST structure)

    [Fact]
    public void Evaluate_Parentheses_AdditionBeforeMultiplication()
    {
        // AST for: (2 + 3) * 4 = 5 * 4 = 20
        // Parentheses change AST structure: addition is left child of multiplication
        var addition = Binary(Num(2), TokenType.Plus, Num(3));
        var expr = Binary(addition, TokenType.Star, Num(4));

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 20.0);
    }

    [Fact]
    public void Evaluate_Parentheses_SubtractionBeforeDivision()
    {
        // AST for: (10 - 4) / 2 = 6 / 2 = 3
        var subtraction = Binary(Num(10), TokenType.Minus, Num(4));
        var expr = Binary(subtraction, TokenType.Slash, Num(2));

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 3.0);
    }

    [Fact]
    public void Evaluate_Parentheses_NestedParens()
    {
        // AST for: ((2 + 3) * 4) = 5 * 4 = 20
        // Multiple layers of parentheses produce same tree structure
        var inner = Binary(Num(2), TokenType.Plus, Num(3));
        var expr = Binary(inner, TokenType.Star, Num(4));

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 20.0);
    }

    [Fact]
    public void Evaluate_Parentheses_RightGrouping()
    {
        // AST for: 2 * (3 + 4) = 2 * 7 = 14
        var addition = Binary(Num(3), TokenType.Plus, Num(4));
        var expr = Binary(Num(2), TokenType.Star, addition);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 14.0);
    }

    [Fact]
    public void Evaluate_Parentheses_DeeplyNested()
    {
        // AST for: ((1 + 2) * (3 + 4)) = 3 * 7 = 21
        var left = Binary(Num(1), TokenType.Plus, Num(2));
        var right = Binary(Num(3), TokenType.Plus, Num(4));
        var expr = Binary(left, TokenType.Star, right);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 21.0);
    }

    [Fact]
    public void Evaluate_Parentheses_ComplexNesting()
    {
        // AST for: (2 + (3 * (4 + 5))) = 2 + (3 * 9) = 2 + 27 = 29
        var innermost = Binary(Num(4), TokenType.Plus, Num(5));
        var middle = Binary(Num(3), TokenType.Star, innermost);
        var expr = Binary(Num(2), TokenType.Plus, middle);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 29.0);
    }

    #endregion

    #region 4. Unary Minus Tests

    [Fact]
    public void Evaluate_UnaryMinus_NegativeFive()
    {
        // -5 = -5
        var expr = Unary(TokenType.Minus, Num(5));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -5.0);
    }

    [Fact]
    public void Evaluate_UnaryMinus_DoubleNegative()
    {
        // --5 = 5
        var inner = Unary(TokenType.Minus, Num(5));
        var expr = Unary(TokenType.Minus, inner);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 5.0);
    }

    [Fact]
    public void Evaluate_UnaryMinus_ParenthesizedExpression()
    {
        // -(3) = -3
        var expr = Unary(TokenType.Minus, Num(3));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -3.0);
    }

    [Fact]
    public void Evaluate_UnaryMinus_OfSum()
    {
        // -(2 + 3) = -5
        var sum = Binary(Num(2), TokenType.Plus, Num(3));
        var expr = Unary(TokenType.Minus, sum);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -5.0);
    }

    [Fact]
    public void Evaluate_UnaryMinus_OfDifference()
    {
        // -(10 - 3) = -7
        var diff = Binary(Num(10), TokenType.Minus, Num(3));
        var expr = Unary(TokenType.Minus, diff);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -7.0);
    }

    [Fact]
    public void Evaluate_UnaryMinus_OfProduct()
    {
        // -(4 * 5) = -20
        var product = Binary(Num(4), TokenType.Star, Num(5));
        var expr = Unary(TokenType.Minus, product);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -20.0);
    }

    [Fact]
    public void Evaluate_UnaryMinus_Zero()
    {
        // -0 = 0 (or -0, which equals 0 in IEEE 754)
        var expr = Unary(TokenType.Minus, Num(0));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 0.0);
    }

    [Fact]
    public void Evaluate_UnaryMinus_OfNegativeNumber()
    {
        // -(-5) represented as unary minus of a negative number literal
        var expr = Unary(TokenType.Minus, Num(-5));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 5.0);
    }

    [Fact]
    public void Evaluate_UnaryMinus_InBinaryExpression()
    {
        // 3 * -2 = -6
        var negTwo = Unary(TokenType.Minus, Num(2));
        var expr = Binary(Num(3), TokenType.Star, negTwo);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -6.0);
    }

    [Fact]
    public void Evaluate_UnaryMinus_BothOperands()
    {
        // -3 + -2 = -5
        var negThree = Unary(TokenType.Minus, Num(3));
        var negTwo = Unary(TokenType.Minus, Num(2));
        var expr = Binary(negThree, TokenType.Plus, negTwo);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -5.0);
    }

    #endregion

    #region 5. Division By Zero Tests

    [Fact]
    public void Evaluate_DivisionByZero_ReturnsFailure()
    {
        // 10 / 0 = Failure
        var expr = Binary(Num(10), TokenType.Slash, Num(0));
        var result = Evaluator.Evaluate(expr);
        AssertFailure(result, "Division by zero");
    }

    [Fact]
    public void Evaluate_DivisionByZero_ZeroDividedByZero()
    {
        // 0 / 0 = Failure
        var expr = Binary(Num(0), TokenType.Slash, Num(0));
        var result = Evaluator.Evaluate(expr);
        AssertFailure(result, "Division by zero");
    }

    [Fact]
    public void Evaluate_DivisionByZero_NegativeDividend()
    {
        // -5 / 0 = Failure
        var expr = Binary(Num(-5), TokenType.Slash, Num(0));
        var result = Evaluator.Evaluate(expr);
        AssertFailure(result, "Division by zero");
    }

    [Fact]
    public void Evaluate_DivisionByZero_InComplexExpression()
    {
        // (5 + 5) / (3 - 3) = 10 / 0 = Failure
        var numerator = Binary(Num(5), TokenType.Plus, Num(5));
        var denominator = Binary(Num(3), TokenType.Minus, Num(3));
        var expr = Binary(numerator, TokenType.Slash, denominator);

        var result = Evaluator.Evaluate(expr);
        AssertFailure(result, "Division by zero");
    }

    [Fact]
    public void Evaluate_DivisionByZero_InNestedExpression()
    {
        // 1 + (10 / 0) = Failure (should propagate error)
        var division = Binary(Num(10), TokenType.Slash, Num(0));
        var expr = Binary(Num(1), TokenType.Plus, division);

        var result = Evaluator.Evaluate(expr);
        AssertFailure(result, "Division by zero");
    }

    [Fact]
    public void Evaluate_DivisionByZero_MultipleDivisionsFirstFails()
    {
        // (10 / 0) / 2 = Failure
        var firstDiv = Binary(Num(10), TokenType.Slash, Num(0));
        var expr = Binary(firstDiv, TokenType.Slash, Num(2));

        var result = Evaluator.Evaluate(expr);
        AssertFailure(result, "Division by zero");
    }

    [Fact]
    public void Evaluate_DivisionByZero_WithUnaryMinus()
    {
        // 5 / -0 = Division by zero (since -0 equals 0)
        var negZero = Unary(TokenType.Minus, Num(0));
        var expr = Binary(Num(5), TokenType.Slash, negZero);

        var result = Evaluator.Evaluate(expr);
        AssertFailure(result, "Division by zero");
    }

    #endregion

    #region 6. Floating-Point Arithmetic Tests

    [Fact]
    public void Evaluate_FloatingPoint_DivisionWithRemainder()
    {
        // 10 / 4 = 2.5
        var expr = Binary(Num(10), TokenType.Slash, Num(4));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 2.5);
    }

    [Fact]
    public void Evaluate_FloatingPoint_PiApproximation()
    {
        // 3.14 + 2.86 = 6.0
        var expr = Binary(Num(3.14), TokenType.Plus, Num(2.86));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 6.0);
    }

    [Fact]
    public void Evaluate_FloatingPoint_SmallDecimals()
    {
        // 0.1 + 0.2 approximately equals 0.3
        var expr = Binary(Num(0.1), TokenType.Plus, Num(0.2));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 0.3);
    }

    [Fact]
    public void Evaluate_FloatingPoint_Multiplication()
    {
        // 2.5 * 4 = 10.0
        var expr = Binary(Num(2.5), TokenType.Star, Num(4));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 10.0);
    }

    [Fact]
    public void Evaluate_FloatingPoint_Division()
    {
        // 7.5 / 2.5 = 3.0
        var expr = Binary(Num(7.5), TokenType.Slash, Num(2.5));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 3.0);
    }

    [Fact]
    public void Evaluate_FloatingPoint_NegativeDecimals()
    {
        // -1.5 + -2.5 = -4.0
        var expr = Binary(Num(-1.5), TokenType.Plus, Num(-2.5));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -4.0);
    }

    [Theory]
    [InlineData(1.5, 2.5, 4.0)]
    [InlineData(0.5, 0.5, 1.0)]
    [InlineData(10.5, 4.5, 15.0)]
    [InlineData(0.001, 0.002, 0.003)]
    [InlineData(99.99, 0.01, 100.0)]
    public void Evaluate_FloatingPoint_VariousAdditions(double a, double b, double expected)
    {
        var expr = Binary(Num(a), TokenType.Plus, Num(b));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, expected);
    }

    [Theory]
    [InlineData(1, 3, 0.333333333333333)]
    [InlineData(2, 3, 0.666666666666667)]
    [InlineData(22, 7, 3.14285714285714)] // Pi approximation
    public void Evaluate_FloatingPoint_RepeatingDecimals(double a, double b, double expected)
    {
        var expr = Binary(Num(a), TokenType.Slash, Num(b));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, expected, precision: 5); // Lower precision for repeating decimals
    }

    #endregion

    #region 7. Complex Expressions Tests

    [Fact]
    public void Evaluate_Complex_MultipleOperations()
    {
        // 2 + 3 * 4 - 6 / 2 = 2 + 12 - 3 = 11
        var mul = Binary(Num(3), TokenType.Star, Num(4));
        var div = Binary(Num(6), TokenType.Slash, Num(2));
        var addMul = Binary(Num(2), TokenType.Plus, mul);
        var expr = Binary(addMul, TokenType.Minus, div);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 11.0);
    }

    [Fact]
    public void Evaluate_Complex_NestedBinaryOperations()
    {
        // ((1 + 2) * (3 + 4)) / (5 - 3) = (3 * 7) / 2 = 21 / 2 = 10.5
        var left = Binary(Num(1), TokenType.Plus, Num(2));
        var right = Binary(Num(3), TokenType.Plus, Num(4));
        var mul = Binary(left, TokenType.Star, right);
        var sub = Binary(Num(5), TokenType.Minus, Num(3));
        var expr = Binary(mul, TokenType.Slash, sub);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 10.5);
    }

    [Fact]
    public void Evaluate_Complex_UnaryAndBinaryMixed()
    {
        // -2 * (3 + -4) = -2 * -1 = 2
        var negTwo = Unary(TokenType.Minus, Num(2));
        var negFour = Unary(TokenType.Minus, Num(4));
        var sum = Binary(Num(3), TokenType.Plus, negFour);
        var expr = Binary(negTwo, TokenType.Star, sum);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 2.0);
    }

    [Fact]
    public void Evaluate_Complex_DeeplyNestedExpression()
    {
        // (((1 + 2) + 3) + 4) + 5 = 15
        var e1 = Binary(Num(1), TokenType.Plus, Num(2));
        var e2 = Binary(e1, TokenType.Plus, Num(3));
        var e3 = Binary(e2, TokenType.Plus, Num(4));
        var expr = Binary(e3, TokenType.Plus, Num(5));

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 15.0);
    }

    [Fact]
    public void Evaluate_Complex_AllOperators()
    {
        // 10 + 5 - 3 * 2 / 1 = 10 + 5 - 6 = 9
        var mul = Binary(Num(3), TokenType.Star, Num(2));
        var div = Binary(mul, TokenType.Slash, Num(1));
        var add = Binary(Num(10), TokenType.Plus, Num(5));
        var expr = Binary(add, TokenType.Minus, div);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 9.0);
    }

    [Fact]
    public void Evaluate_Complex_LongChain()
    {
        // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 = 55
        var expr = Num(1);

        for (var i = 2; i <= 10; i++)
        {
            expr = Binary(expr, TokenType.Plus, Num(i));
        }

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 55.0);
    }

    [Fact]
    public void Evaluate_Complex_FloatingPointWithUnary()
    {
        // -(3.5 + 2.5) * 2 = -6 * 2 = -12
        var sum = Binary(Num(3.5), TokenType.Plus, Num(2.5));
        var negSum = Unary(TokenType.Minus, sum);
        var expr = Binary(negSum, TokenType.Star, Num(2));

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -12.0);
    }

    [Fact]
    public void Evaluate_Complex_QuadraticFormulaPart()
    {
        // (-b + discriminant) / (2 * a) where b=4, discriminant=2, a=2
        // (-4 + 2) / (2 * 2) = -2 / 4 = -0.5
        var negB = Unary(TokenType.Minus, Num(4));
        var numerator = Binary(negB, TokenType.Plus, Num(2));
        var denominator = Binary(Num(2), TokenType.Star, Num(2));
        var expr = Binary(numerator, TokenType.Slash, denominator);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -0.5);
    }

    #endregion

    #region 8. Nested Unary Operations Tests

    [Fact]
    public void Evaluate_NestedUnary_TripleMinus()
    {
        // ---5 = -5
        var neg1 = Unary(TokenType.Minus, Num(5));
        var neg2 = Unary(TokenType.Minus, neg1);
        var expr = Unary(TokenType.Minus, neg2);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -5.0);
    }

    [Fact]
    public void Evaluate_NestedUnary_QuadrupleMinus()
    {
        // ----5 = 5
        var neg1 = Unary(TokenType.Minus, Num(5));
        var neg2 = Unary(TokenType.Minus, neg1);
        var neg3 = Unary(TokenType.Minus, neg2);
        var expr = Unary(TokenType.Minus, neg3);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 5.0);
    }

    [Fact]
    public void Evaluate_NestedUnary_FiveMinus()
    {
        // -----5 = -5
        var neg1 = Unary(TokenType.Minus, Num(5));
        var neg2 = Unary(TokenType.Minus, neg1);
        var neg3 = Unary(TokenType.Minus, neg2);
        var neg4 = Unary(TokenType.Minus, neg3);
        var expr = Unary(TokenType.Minus, neg4);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -5.0);
    }

    [Fact]
    public void Evaluate_NestedUnary_OnExpression()
    {
        // ---(2 + 3) = -(5) = -5
        var sum = Binary(Num(2), TokenType.Plus, Num(3));
        var neg1 = Unary(TokenType.Minus, sum);
        var neg2 = Unary(TokenType.Minus, neg1);
        var expr = Unary(TokenType.Minus, neg2);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -5.0);
    }

    [Fact]
    public void Evaluate_NestedUnary_WithBinaryOps()
    {
        // --3 + ---2 = 3 + (-2) = 1
        var doubleNegThree = Unary(TokenType.Minus, Unary(TokenType.Minus, Num(3)));
        var tripleNegTwo = Unary(TokenType.Minus, Unary(TokenType.Minus, Unary(TokenType.Minus, Num(2))));
        var expr = Binary(doubleNegThree, TokenType.Plus, tripleNegTwo);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 1.0);
    }

    [Fact]
    public void Evaluate_NestedUnary_MultiplicationChain()
    {
        // --2 * ---3 = 2 * (-3) = -6
        var doubleNegTwo = Unary(TokenType.Minus, Unary(TokenType.Minus, Num(2)));
        var tripleNegThree = Unary(TokenType.Minus, Unary(TokenType.Minus, Unary(TokenType.Minus, Num(3))));
        var expr = Binary(doubleNegTwo, TokenType.Star, tripleNegThree);

        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -6.0);
    }

    #endregion

    #region Edge Cases and Special Values

    [Fact]
    public void Evaluate_SingleNumber_ReturnsValue()
    {
        var expr = Num(42);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 42.0);
    }

    [Fact]
    public void Evaluate_SingleNumber_Zero()
    {
        var expr = Num(0);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 0.0);
    }

    [Fact]
    public void Evaluate_SingleNumber_Negative()
    {
        var expr = Num(-42);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, -42.0);
    }

    [Fact]
    public void Evaluate_SingleNumber_Large()
    {
        var expr = Num(1e15);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 1e15);
    }

    [Fact]
    public void Evaluate_SingleNumber_Small()
    {
        var expr = Num(1e-15);
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 1e-15);
    }

    [Fact]
    public void Evaluate_LargeNumberArithmetic()
    {
        // 1000000 * 1000000 = 1000000000000
        var expr = Binary(Num(1e6), TokenType.Star, Num(1e6));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 1e12);
    }

    [Fact]
    public void Evaluate_SmallNumberArithmetic()
    {
        // 0.000001 * 0.000001 = 0.000000000001
        var expr = Binary(Num(1e-6), TokenType.Star, Num(1e-6));
        var result = Evaluator.Evaluate(expr);
        AssertSuccess(result, 1e-12);
    }

    #endregion

    #region Special Cases

    [Theory]
    [InlineData(10, 15)]
    [InlineData(15, 10)]
    [InlineData(-10, -15)]
    public void Evaluate_WhenSubtract_MustBeGreaterOrEqualToZero(int a, int b)
    {
        // Arrange
        var expr = Binary(Num(a), TokenType.Minus, Num(b));

        // Act
        var result = Evaluator.Evaluate(expr);

        // Assert
        Assert.True(result.Match(r => r, r => -1) >= 0);
    }

    [Theory]
    [InlineData(100, 12)]
    [InlineData(100, 10)]
    [InlineData(100, 1)]
    public void Evaluate_WhenMultiply_MustNotOverOneThousand(int a, int b)
    {
        // Arrange
        var mutatedB = SideEffectValue(b);
        var expr = Binary(Num(a), TokenType.Star, Num(mutatedB));

        // Act
        var result = Evaluator.Evaluate(expr);

        // Assert
        Assert.True(result.Map(r => r <= 1000).Match(r => r, r => false));
    }

    public static int SideEffectValue(int x)
    {
        return x + 1;
    }

    #endregion
}