using Calculator;

namespace Calculator.Tests;

/// <summary>
/// Comprehensive unit tests for the Parser class.
/// Tests parsing of numbers, expressions, operator precedence, parentheses, and error handling.
/// Verifies AST structure for correct operator precedence and associativity.
/// </summary>
public class ParserTests
{
    #region Helper Methods

    /// <summary>
    /// Helper to parse a string expression by first tokenizing it, then parsing the tokens.
    /// </summary>
    private static Result<Expression> ParseString(string input)
    {
        var lexResult = Lexer.Tokenize(input);
        if (lexResult.IsFailure)
        {
            return Result<Expression>.Fail(((Result<List<Token>>.Failure)lexResult).Error);
        }

        var tokens = ((Result<List<Token>>.Success)lexResult).Value;
        return Parser.Parse(tokens);
    }

    /// <summary>
    /// Helper to extract the successful expression from a result.
    /// </summary>
    private static Expression GetExpression(Result<Expression> result)
    {
        Assert.True(result.IsSuccess, $"Expected successful parse, but got: {GetError(result)}");
        return ((Result<Expression>.Success)result).Value;
    }

    /// <summary>
    /// Helper to extract error message from a failed result.
    /// </summary>
    private static string GetError(Result<Expression> result)
    {
        if (result.IsFailure)
        {
            return ((Result<Expression>.Failure)result).Error;
        }
        return string.Empty;
    }

    /// <summary>
    /// Helper to create a token list from individual tokens.
    /// Automatically appends EndOfInput if not present.
    /// </summary>
    private static List<Token> CreateTokens(params Token[] tokens)
    {
        var list = tokens.ToList();
        if (list.Count == 0 || list[^1].Type != TokenType.EndOfInput)
        {
            var position = list.Count > 0 ? list[^1].Position + list[^1].Lexeme.Length : 0;
            list.Add(new Token(TokenType.EndOfInput, "", position));
        }
        return list;
    }

    /// <summary>
    /// Asserts that the expression is a Number with the expected value.
    /// </summary>
    private static void AssertNumber(Expression expr, double expected)
    {
        var number = Assert.IsType<Expression.Number>(expr);
        Assert.Equal(expected, number.Value);
    }

    /// <summary>
    /// Asserts that the expression is a Binary expression with the expected operator.
    /// Returns the binary expression for further inspection.
    /// </summary>
    private static Expression.Binary AssertBinary(Expression expr, TokenType expectedOperator)
    {
        var binary = Assert.IsType<Expression.Binary>(expr);
        Assert.Equal(expectedOperator, binary.Operator);
        return binary;
    }

    /// <summary>
    /// Asserts that the expression is a Unary expression with the expected operator.
    /// Returns the unary expression for further inspection.
    /// </summary>
    private static Expression.Unary AssertUnary(Expression expr, TokenType expectedOperator)
    {
        var unary = Assert.IsType<Expression.Unary>(expr);
        Assert.Equal(expectedOperator, unary.Operator);
        return unary;
    }

    #endregion

    #region 1. Simple Number Parsing Tests

    [Fact]
    public void Parse_SingleDigitNumber_ReturnsNumberExpression()
    {
        // Arrange & Act
        var result = ParseString("5");

        // Assert
        var expr = GetExpression(result);
        AssertNumber(expr, 5.0);
    }

    [Fact]
    public void Parse_MultiDigitNumber_ReturnsNumberExpression()
    {
        // Arrange & Act
        var result = ParseString("42");

        // Assert
        var expr = GetExpression(result);
        AssertNumber(expr, 42.0);
    }

    [Fact]
    public void Parse_DecimalNumber_ReturnsNumberExpression()
    {
        // Arrange & Act
        var result = ParseString("3.14");

        // Assert
        var expr = GetExpression(result);
        AssertNumber(expr, 3.14);
    }

    [Fact]
    public void Parse_Zero_ReturnsNumberExpression()
    {
        // Arrange & Act
        var result = ParseString("0");

        // Assert
        var expr = GetExpression(result);
        AssertNumber(expr, 0.0);
    }

    [Theory]
    [InlineData("0", 0.0)]
    [InlineData("1", 1.0)]
    [InlineData("100", 100.0)]
    [InlineData("0.5", 0.5)]
    [InlineData("123.456", 123.456)]
    [InlineData("999999", 999999.0)]
    public void Parse_VariousNumbers_ReturnsCorrectValue(string input, double expected)
    {
        // Arrange & Act
        var result = ParseString(input);

        // Assert
        var expr = GetExpression(result);
        var number = Assert.IsType<Expression.Number>(expr);
        Assert.Equal(expected, number.Value, precision: 10);
    }

    #endregion

    #region 2. Binary Expression Parsing Tests

    [Fact]
    public void Parse_Addition_ReturnsBinaryExpressionWithCorrectStructure()
    {
        // Arrange & Act: "2 + 3"
        var result = ParseString("2 + 3");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Plus);
        AssertNumber(binary.Left, 2.0);
        AssertNumber(binary.Right, 3.0);
    }

    [Fact]
    public void Parse_Subtraction_ReturnsBinaryExpressionWithCorrectStructure()
    {
        // Arrange & Act: "4 - 1"
        var result = ParseString("4 - 1");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Minus);
        AssertNumber(binary.Left, 4.0);
        AssertNumber(binary.Right, 1.0);
    }

    [Fact]
    public void Parse_Multiplication_ReturnsBinaryExpressionWithCorrectStructure()
    {
        // Arrange & Act: "3 * 4"
        var result = ParseString("3 * 4");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Star);
        AssertNumber(binary.Left, 3.0);
        AssertNumber(binary.Right, 4.0);
    }

    [Fact]
    public void Parse_Division_ReturnsBinaryExpressionWithCorrectStructure()
    {
        // Arrange & Act: "10 / 2"
        var result = ParseString("10 / 2");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Slash);
        AssertNumber(binary.Left, 10.0);
        AssertNumber(binary.Right, 2.0);
    }

    [Theory]
    [InlineData("1 + 2", TokenType.Plus)]
    [InlineData("1 - 2", TokenType.Minus)]
    [InlineData("1 * 2", TokenType.Star)]
    [InlineData("1 / 2", TokenType.Slash)]
    public void Parse_AllBinaryOperators_ReturnsCorrectOperatorType(string input, TokenType expectedOp)
    {
        // Arrange & Act
        var result = ParseString(input);

        // Assert
        var expr = GetExpression(result);
        var binary = Assert.IsType<Expression.Binary>(expr);
        Assert.Equal(expectedOp, binary.Operator);
    }

    [Fact]
    public void Parse_BinaryWithDecimals_ParsesCorrectly()
    {
        // Arrange & Act: "1.5 + 2.5"
        var result = ParseString("1.5 + 2.5");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Plus);
        AssertNumber(binary.Left, 1.5);
        AssertNumber(binary.Right, 2.5);
    }

    #endregion

    #region 3. Unary Minus Parsing Tests

    [Fact]
    public void Parse_UnaryMinusNumber_ReturnsUnaryExpressionWithNumber()
    {
        // Arrange & Act: "-5"
        var result = ParseString("-5");

        // Assert
        var expr = GetExpression(result);
        var unary = AssertUnary(expr, TokenType.Minus);
        AssertNumber(unary.Operand, 5.0);
    }

    [Fact]
    public void Parse_DoubleUnaryMinus_ReturnsNestedUnaryExpressions()
    {
        // Arrange & Act: "--5" should parse as -(-5)
        var result = ParseString("--5");

        // Assert
        var expr = GetExpression(result);
        var outer = AssertUnary(expr, TokenType.Minus);
        var inner = AssertUnary(outer.Operand, TokenType.Minus);
        AssertNumber(inner.Operand, 5.0);
    }

    [Fact]
    public void Parse_TripleUnaryMinus_ReturnsTriplyNestedUnaryExpressions()
    {
        // Arrange & Act: "---5" should parse as -(-(-5))
        var result = ParseString("---5");

        // Assert
        var expr = GetExpression(result);
        var first = AssertUnary(expr, TokenType.Minus);
        var second = AssertUnary(first.Operand, TokenType.Minus);
        var third = AssertUnary(second.Operand, TokenType.Minus);
        AssertNumber(third.Operand, 5.0);
    }

    [Fact]
    public void Parse_UnaryMinusWithParenthesizedExpression_ReturnsUnaryWithBinaryOperand()
    {
        // Arrange & Act: "-(3+2)"
        var result = ParseString("-(3+2)");

        // Assert
        var expr = GetExpression(result);
        var unary = AssertUnary(expr, TokenType.Minus);
        var binary = AssertBinary(unary.Operand, TokenType.Plus);
        AssertNumber(binary.Left, 3.0);
        AssertNumber(binary.Right, 2.0);
    }

    [Fact]
    public void Parse_MultiplicationWithUnaryMinus_ParsesCorrectly()
    {
        // Arrange & Act: "3*-2" should parse as 3 * (-2)
        var result = ParseString("3*-2");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Star);
        AssertNumber(binary.Left, 3.0);
        var unary = AssertUnary(binary.Right, TokenType.Minus);
        AssertNumber(unary.Operand, 2.0);
    }

    [Fact]
    public void Parse_AdditionWithUnaryMinus_ParsesCorrectly()
    {
        // Arrange & Act: "5 + -3"
        var result = ParseString("5 + -3");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Plus);
        AssertNumber(binary.Left, 5.0);
        var unary = AssertUnary(binary.Right, TokenType.Minus);
        AssertNumber(unary.Operand, 3.0);
    }

    [Fact]
    public void Parse_SubtractionWithUnaryMinus_ParsesCorrectly()
    {
        // Arrange & Act: "5 - -3" should parse as 5 - (-3)
        var result = ParseString("5 - -3");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Minus);
        AssertNumber(binary.Left, 5.0);
        var unary = AssertUnary(binary.Right, TokenType.Minus);
        AssertNumber(unary.Operand, 3.0);
    }

    [Fact]
    public void Parse_UnaryMinusWithDecimal_ParsesCorrectly()
    {
        // Arrange & Act: "-3.14"
        var result = ParseString("-3.14");

        // Assert
        var expr = GetExpression(result);
        var unary = AssertUnary(expr, TokenType.Minus);
        AssertNumber(unary.Operand, 3.14);
    }

    #endregion

    #region 4. Operator Precedence Tests

    [Fact]
    public void Parse_AdditionWithMultiplication_MultiplicationHasHigherPrecedence()
    {
        // Arrange & Act: "2 + 3 * 4" should parse as 2 + (3 * 4)
        var result = ParseString("2 + 3 * 4");

        // Assert: Root is +, right child is *
        var expr = GetExpression(result);
        var addExpr = AssertBinary(expr, TokenType.Plus);
        AssertNumber(addExpr.Left, 2.0);

        var mulExpr = AssertBinary(addExpr.Right, TokenType.Star);
        AssertNumber(mulExpr.Left, 3.0);
        AssertNumber(mulExpr.Right, 4.0);
    }

    [Fact]
    public void Parse_SubtractionWithDivision_DivisionHasHigherPrecedence()
    {
        // Arrange & Act: "10 - 4 / 2" should parse as 10 - (4 / 2)
        var result = ParseString("10 - 4 / 2");

        // Assert: Root is -, right child is /
        var expr = GetExpression(result);
        var subExpr = AssertBinary(expr, TokenType.Minus);
        AssertNumber(subExpr.Left, 10.0);

        var divExpr = AssertBinary(subExpr.Right, TokenType.Slash);
        AssertNumber(divExpr.Left, 4.0);
        AssertNumber(divExpr.Right, 2.0);
    }

    [Fact]
    public void Parse_MultiplicationBeforeAddition_CorrectAST()
    {
        // Arrange & Act: "1 * 2 + 3" should parse as (1 * 2) + 3
        var result = ParseString("1 * 2 + 3");

        // Assert: Root is +, left child is *
        var expr = GetExpression(result);
        var addExpr = AssertBinary(expr, TokenType.Plus);

        var mulExpr = AssertBinary(addExpr.Left, TokenType.Star);
        AssertNumber(mulExpr.Left, 1.0);
        AssertNumber(mulExpr.Right, 2.0);

        AssertNumber(addExpr.Right, 3.0);
    }

    [Fact]
    public void Parse_ComplexPrecedence_MultipleOperators()
    {
        // Arrange & Act: "2 * 3 + 4 * 5" should parse as (2 * 3) + (4 * 5)
        var result = ParseString("2 * 3 + 4 * 5");

        // Assert
        var expr = GetExpression(result);
        var addExpr = AssertBinary(expr, TokenType.Plus);

        var leftMul = AssertBinary(addExpr.Left, TokenType.Star);
        AssertNumber(leftMul.Left, 2.0);
        AssertNumber(leftMul.Right, 3.0);

        var rightMul = AssertBinary(addExpr.Right, TokenType.Star);
        AssertNumber(rightMul.Left, 4.0);
        AssertNumber(rightMul.Right, 5.0);
    }

    [Fact]
    public void Parse_DivisionBeforeSubtraction_CorrectAST()
    {
        // Arrange & Act: "8 / 2 - 1" should parse as (8 / 2) - 1
        var result = ParseString("8 / 2 - 1");

        // Assert
        var expr = GetExpression(result);
        var subExpr = AssertBinary(expr, TokenType.Minus);

        var divExpr = AssertBinary(subExpr.Left, TokenType.Slash);
        AssertNumber(divExpr.Left, 8.0);
        AssertNumber(divExpr.Right, 2.0);

        AssertNumber(subExpr.Right, 1.0);
    }

    [Fact]
    public void Parse_UnaryHasHighestPrecedence()
    {
        // Arrange & Act: "-2 * 3" should parse as (-2) * 3
        var result = ParseString("-2 * 3");

        // Assert: Root is *, left child is unary -
        var expr = GetExpression(result);
        var mulExpr = AssertBinary(expr, TokenType.Star);

        var unary = AssertUnary(mulExpr.Left, TokenType.Minus);
        AssertNumber(unary.Operand, 2.0);

        AssertNumber(mulExpr.Right, 3.0);
    }

    #endregion

    #region 5. Parentheses Grouping Tests

    [Fact]
    public void Parse_ParenthesizedAdditionTimesNumber_OverridesPrecedence()
    {
        // Arrange & Act: "(2 + 3) * 4" should parse as (2 + 3) * 4, overriding normal precedence
        var result = ParseString("(2 + 3) * 4");

        // Assert: Root is *, left child is +
        var expr = GetExpression(result);
        var mulExpr = AssertBinary(expr, TokenType.Star);

        var addExpr = AssertBinary(mulExpr.Left, TokenType.Plus);
        AssertNumber(addExpr.Left, 2.0);
        AssertNumber(addExpr.Right, 3.0);

        AssertNumber(mulExpr.Right, 4.0);
    }

    [Fact]
    public void Parse_DoubleParentheses_ParsesCorrectly()
    {
        // Arrange & Act: "((2 + 3))"
        var result = ParseString("((2 + 3))");

        // Assert: Inner expression is binary +
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Plus);
        AssertNumber(binary.Left, 2.0);
        AssertNumber(binary.Right, 3.0);
    }

    [Fact]
    public void Parse_ParenthesesAroundNumber_ReturnsNumber()
    {
        // Arrange & Act: "(42)"
        var result = ParseString("(42)");

        // Assert
        var expr = GetExpression(result);
        AssertNumber(expr, 42.0);
    }

    [Fact]
    public void Parse_MultipleParenthesizedGroups_ParsesCorrectly()
    {
        // Arrange & Act: "(1 + 2) * (3 + 4)"
        var result = ParseString("(1 + 2) * (3 + 4)");

        // Assert
        var expr = GetExpression(result);
        var mulExpr = AssertBinary(expr, TokenType.Star);

        var leftAdd = AssertBinary(mulExpr.Left, TokenType.Plus);
        AssertNumber(leftAdd.Left, 1.0);
        AssertNumber(leftAdd.Right, 2.0);

        var rightAdd = AssertBinary(mulExpr.Right, TokenType.Plus);
        AssertNumber(rightAdd.Left, 3.0);
        AssertNumber(rightAdd.Right, 4.0);
    }

    [Fact]
    public void Parse_NumberTimesParenthesizedGroup_ParsesCorrectly()
    {
        // Arrange & Act: "4 * (2 + 3)"
        var result = ParseString("4 * (2 + 3)");

        // Assert
        var expr = GetExpression(result);
        var mulExpr = AssertBinary(expr, TokenType.Star);
        AssertNumber(mulExpr.Left, 4.0);

        var addExpr = AssertBinary(mulExpr.Right, TokenType.Plus);
        AssertNumber(addExpr.Left, 2.0);
        AssertNumber(addExpr.Right, 3.0);
    }

    [Fact]
    public void Parse_DeeplyNestedParentheses_ParsesCorrectly()
    {
        // Arrange & Act: "((((1))))"
        var result = ParseString("((((1))))");

        // Assert
        var expr = GetExpression(result);
        AssertNumber(expr, 1.0);
    }

    #endregion

    #region 6. Left Associativity Tests

    [Fact]
    public void Parse_ChainedSubtraction_IsLeftAssociative()
    {
        // Arrange & Act: "8 - 4 - 2" should parse as (8 - 4) - 2
        var result = ParseString("8 - 4 - 2");

        // Assert: Root is -, left child is also -
        var expr = GetExpression(result);
        var outerSub = AssertBinary(expr, TokenType.Minus);

        var innerSub = AssertBinary(outerSub.Left, TokenType.Minus);
        AssertNumber(innerSub.Left, 8.0);
        AssertNumber(innerSub.Right, 4.0);

        AssertNumber(outerSub.Right, 2.0);
    }

    [Fact]
    public void Parse_ChainedDivision_IsLeftAssociative()
    {
        // Arrange & Act: "12 / 3 / 2" should parse as (12 / 3) / 2
        var result = ParseString("12 / 3 / 2");

        // Assert: Root is /, left child is also /
        var expr = GetExpression(result);
        var outerDiv = AssertBinary(expr, TokenType.Slash);

        var innerDiv = AssertBinary(outerDiv.Left, TokenType.Slash);
        AssertNumber(innerDiv.Left, 12.0);
        AssertNumber(innerDiv.Right, 3.0);

        AssertNumber(outerDiv.Right, 2.0);
    }

    [Fact]
    public void Parse_ChainedAddition_IsLeftAssociative()
    {
        // Arrange & Act: "1 + 2 + 3" should parse as (1 + 2) + 3
        var result = ParseString("1 + 2 + 3");

        // Assert
        var expr = GetExpression(result);
        var outerAdd = AssertBinary(expr, TokenType.Plus);

        var innerAdd = AssertBinary(outerAdd.Left, TokenType.Plus);
        AssertNumber(innerAdd.Left, 1.0);
        AssertNumber(innerAdd.Right, 2.0);

        AssertNumber(outerAdd.Right, 3.0);
    }

    [Fact]
    public void Parse_ChainedMultiplication_IsLeftAssociative()
    {
        // Arrange & Act: "2 * 3 * 4" should parse as (2 * 3) * 4
        var result = ParseString("2 * 3 * 4");

        // Assert
        var expr = GetExpression(result);
        var outerMul = AssertBinary(expr, TokenType.Star);

        var innerMul = AssertBinary(outerMul.Left, TokenType.Star);
        AssertNumber(innerMul.Left, 2.0);
        AssertNumber(innerMul.Right, 3.0);

        AssertNumber(outerMul.Right, 4.0);
    }

    [Fact]
    public void Parse_MixedChainedOperators_CorrectAssociativity()
    {
        // Arrange & Act: "10 - 5 + 2" should parse as (10 - 5) + 2
        var result = ParseString("10 - 5 + 2");

        // Assert
        var expr = GetExpression(result);
        var outerAdd = AssertBinary(expr, TokenType.Plus);

        var innerSub = AssertBinary(outerAdd.Left, TokenType.Minus);
        AssertNumber(innerSub.Left, 10.0);
        AssertNumber(innerSub.Right, 5.0);

        AssertNumber(outerAdd.Right, 2.0);
    }

    [Fact]
    public void Parse_MixedMultiplicationDivision_CorrectAssociativity()
    {
        // Arrange & Act: "8 * 4 / 2" should parse as (8 * 4) / 2
        var result = ParseString("8 * 4 / 2");

        // Assert
        var expr = GetExpression(result);
        var outerDiv = AssertBinary(expr, TokenType.Slash);

        var innerMul = AssertBinary(outerDiv.Left, TokenType.Star);
        AssertNumber(innerMul.Left, 8.0);
        AssertNumber(innerMul.Right, 4.0);

        AssertNumber(outerDiv.Right, 2.0);
    }

    #endregion

    #region 7. Syntax Error Tests

    [Fact]
    public void Parse_MissingRightOperand_ReturnsError()
    {
        // Arrange & Act: "2 +"
        var result = ParseString("2 +");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("end of input", error.ToLower());
    }

    [Fact]
    public void Parse_LeadingPlusOperator_ReturnsError()
    {
        // Arrange & Act: "+ 3"
        var result = ParseString("+ 3");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_LeadingMultiplyOperator_ReturnsError()
    {
        // Arrange & Act: "* 3"
        var result = ParseString("* 3");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_LeadingDivideOperator_ReturnsError()
    {
        // Arrange & Act: "/ 3"
        var result = ParseString("/ 3");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_TrailingMinusOperator_ReturnsError()
    {
        // Arrange & Act: "5 -"
        var result = ParseString("5 -");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("end of input", error.ToLower());
    }

    [Fact]
    public void Parse_TrailingMultiplyOperator_ReturnsError()
    {
        // Arrange & Act: "5 *"
        var result = ParseString("5 *");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("end of input", error.ToLower());
    }

    [Fact]
    public void Parse_TrailingDivideOperator_ReturnsError()
    {
        // Arrange & Act: "5 /"
        var result = ParseString("5 /");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("end of input", error.ToLower());
    }

    [Fact]
    public void Parse_DoubleOperatorNotUnary_ReturnsError()
    {
        // Arrange & Act: "2 + * 3" - plus followed by multiply (not unary minus)
        var result = ParseString("2 + * 3");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_ConsecutiveNumbers_ReturnsError()
    {
        // Arrange & Act: "2 3"
        var result = ParseString("2 3");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("unexpected", error.ToLower());
    }

    [Fact]
    public void Parse_OnlyOperator_ReturnsError()
    {
        // Arrange & Act: "+"
        var result = ParseString("+");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_OnlyUnaryMinus_ReturnsError()
    {
        // Arrange & Act: "-"
        var result = ParseString("-");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("end of input", error.ToLower());
    }

    #endregion

    #region 8. Mismatched Parentheses Tests

    [Fact]
    public void Parse_MissingClosingParenthesis_ReturnsError()
    {
        // Arrange & Act: "(2 + 3"
        var result = ParseString("(2 + 3");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("parentheses", error.ToLower());
    }

    [Fact]
    public void Parse_MissingOpeningParenthesis_ReturnsError()
    {
        // Arrange & Act: "2 + 3)"
        var result = ParseString("2 + 3)");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("unexpected", error.ToLower());
    }

    [Fact]
    public void Parse_ExtraClosingParenthesis_ReturnsError()
    {
        // Arrange & Act: "(2 + 3))"
        var result = ParseString("(2 + 3))");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_ExtraOpeningParenthesis_ReturnsError()
    {
        // Arrange & Act: "((2 + 3)"
        var result = ParseString("((2 + 3)");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("parentheses", error.ToLower());
    }

    [Fact]
    public void Parse_EmptyParentheses_ReturnsError()
    {
        // Arrange & Act: "()"
        var result = ParseString("()");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_JustOpeningParenthesis_ReturnsError()
    {
        // Arrange & Act: "("
        var result = ParseString("(");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_JustClosingParenthesis_ReturnsError()
    {
        // Arrange & Act: ")"
        var result = ParseString(")");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("parentheses", error.ToLower());
    }

    [Fact]
    public void Parse_ReversedParentheses_ReturnsError()
    {
        // Arrange & Act: ")2 + 3("
        var result = ParseString(")2 + 3(");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_NestedMissingClosing_ReturnsError()
    {
        // Arrange & Act: "((1 + 2)"
        var result = ParseString("((1 + 2)");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("parentheses", error.ToLower());
    }

    #endregion

    #region 9. Empty Expression Error Tests

    [Fact]
    public void Parse_EmptyString_ReturnsError()
    {
        // Arrange & Act
        var result = ParseString("");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("empty", error.ToLower());
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsError()
    {
        // Arrange & Act
        var result = ParseString("   ");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("empty", error.ToLower());
    }

    [Fact]
    public void Parse_TabsAndNewlinesOnly_ReturnsError()
    {
        // Arrange & Act
        var result = ParseString("\t\n\r\n");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Parse_EmptyTokenList_ReturnsError()
    {
        // Arrange: Create a parser with an empty token list
        var tokens = new List<Token>();

        // Act
        var result = Parser.Parse(tokens);

        // Assert
        Assert.True(result.IsFailure);
        var error = ((Result<Expression>.Failure)result).Error;
        Assert.Contains("empty", error.ToLower());
    }

    [Fact]
    public void Parse_OnlyEndOfInputToken_ReturnsError()
    {
        // Arrange: Token list with only EndOfInput
        var tokens = new List<Token> { new Token(TokenType.EndOfInput, "", 0) };

        // Act
        var result = Parser.Parse(tokens);

        // Assert
        Assert.True(result.IsFailure);
        var error = ((Result<Expression>.Failure)result).Error;
        Assert.Contains("empty", error.ToLower());
    }

    #endregion

    #region 10. Complex Nested Expression Tests

    [Fact]
    public void Parse_NestedParenthesesWithBinaryOperations_ParsesCorrectly()
    {
        // Arrange & Act: "(2 + (3 * 4))"
        var result = ParseString("(2 + (3 * 4))");

        // Assert
        var expr = GetExpression(result);
        var addExpr = AssertBinary(expr, TokenType.Plus);
        AssertNumber(addExpr.Left, 2.0);

        var mulExpr = AssertBinary(addExpr.Right, TokenType.Star);
        AssertNumber(mulExpr.Left, 3.0);
        AssertNumber(mulExpr.Right, 4.0);
    }

    [Fact]
    public void Parse_DeeplyNestedMixedOperations_ParsesCorrectly()
    {
        // Arrange & Act: "((1 + 2) * (3 + 4))"
        var result = ParseString("((1 + 2) * (3 + 4))");

        // Assert
        var expr = GetExpression(result);
        var mulExpr = AssertBinary(expr, TokenType.Star);

        var leftAdd = AssertBinary(mulExpr.Left, TokenType.Plus);
        AssertNumber(leftAdd.Left, 1.0);
        AssertNumber(leftAdd.Right, 2.0);

        var rightAdd = AssertBinary(mulExpr.Right, TokenType.Plus);
        AssertNumber(rightAdd.Left, 3.0);
        AssertNumber(rightAdd.Right, 4.0);
    }

    [Fact]
    public void Parse_ComplexNestedWithUnary_ParsesCorrectly()
    {
        // Arrange & Act: "-(2 + (3 * -4))"
        var result = ParseString("-(2 + (3 * -4))");

        // Assert
        var expr = GetExpression(result);
        var outerUnary = AssertUnary(expr, TokenType.Minus);

        var addExpr = AssertBinary(outerUnary.Operand, TokenType.Plus);
        AssertNumber(addExpr.Left, 2.0);

        var mulExpr = AssertBinary(addExpr.Right, TokenType.Star);
        AssertNumber(mulExpr.Left, 3.0);

        var innerUnary = AssertUnary(mulExpr.Right, TokenType.Minus);
        AssertNumber(innerUnary.Operand, 4.0);
    }

    [Fact]
    public void Parse_TripleNestedParentheses_ParsesCorrectly()
    {
        // Arrange & Act: "(((1 + 2)))"
        var result = ParseString("(((1 + 2)))");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Plus);
        AssertNumber(binary.Left, 1.0);
        AssertNumber(binary.Right, 2.0);
    }

    [Fact]
    public void Parse_MultiLevelNestedWithAllOperators_ParsesCorrectly()
    {
        // Arrange & Act: "((2 + 3) * 4 - 5) / 2"
        var result = ParseString("((2 + 3) * 4 - 5) / 2");

        // Assert: Should parse as (((2+3)*4) - 5) / 2
        var expr = GetExpression(result);
        var divExpr = AssertBinary(expr, TokenType.Slash);

        var subExpr = AssertBinary(divExpr.Left, TokenType.Minus);
        AssertNumber(subExpr.Right, 5.0);

        var mulExpr = AssertBinary(subExpr.Left, TokenType.Star);
        AssertNumber(mulExpr.Right, 4.0);

        var addExpr = AssertBinary(mulExpr.Left, TokenType.Plus);
        AssertNumber(addExpr.Left, 2.0);
        AssertNumber(addExpr.Right, 3.0);

        AssertNumber(divExpr.Right, 2.0);
    }

    [Fact]
    public void Parse_NestedWithUnaryAtMultipleLevels_ParsesCorrectly()
    {
        // Arrange & Act: "(-1) + (-(2 * 3))"
        var result = ParseString("(-1) + (-(2 * 3))");

        // Assert
        var expr = GetExpression(result);
        var addExpr = AssertBinary(expr, TokenType.Plus);

        var leftUnary = AssertUnary(addExpr.Left, TokenType.Minus);
        AssertNumber(leftUnary.Operand, 1.0);

        var rightUnary = AssertUnary(addExpr.Right, TokenType.Minus);
        var mulExpr = AssertBinary(rightUnary.Operand, TokenType.Star);
        AssertNumber(mulExpr.Left, 2.0);
        AssertNumber(mulExpr.Right, 3.0);
    }

    #endregion

    #region Additional Edge Cases and Integration Tests

    [Fact]
    public void Parse_VeryLongExpression_ParsesCorrectly()
    {
        // Arrange & Act: "1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10"
        var result = ParseString("1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10");

        // Assert: Should succeed and be left-associative
        Assert.True(result.IsSuccess);
        var expr = GetExpression(result);
        Assert.IsType<Expression.Binary>(expr);
    }

    [Fact]
    public void Parse_AlternatingOperatorTypes_ParsesCorrectly()
    {
        // Arrange & Act: "1 + 2 * 3 - 4 / 2"
        // Should parse as: (1 + (2*3)) - (4/2)  because of precedence and left-to-right
        // Actually: ((1 + (2*3)) - (4/2))
        var result = ParseString("1 + 2 * 3 - 4 / 2");

        // Assert
        var expr = GetExpression(result);
        var outerSub = AssertBinary(expr, TokenType.Minus);

        var addExpr = AssertBinary(outerSub.Left, TokenType.Plus);
        AssertNumber(addExpr.Left, 1.0);

        var mulExpr = AssertBinary(addExpr.Right, TokenType.Star);
        AssertNumber(mulExpr.Left, 2.0);
        AssertNumber(mulExpr.Right, 3.0);

        var divExpr = AssertBinary(outerSub.Right, TokenType.Slash);
        AssertNumber(divExpr.Left, 4.0);
        AssertNumber(divExpr.Right, 2.0);
    }

    [Fact]
    public void Parse_ExpressionWithExtraWhitespace_ParsesCorrectly()
    {
        // Arrange & Act: "  2   +   3  "
        var result = ParseString("  2   +   3  ");

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Plus);
        AssertNumber(binary.Left, 2.0);
        AssertNumber(binary.Right, 3.0);
    }

    [Fact]
    public void Parse_NoWhitespaceBetweenTokens_ParsesCorrectly()
    {
        // Arrange & Act: "2+3*4"
        var result = ParseString("2+3*4");

        // Assert
        var expr = GetExpression(result);
        var addExpr = AssertBinary(expr, TokenType.Plus);
        AssertNumber(addExpr.Left, 2.0);

        var mulExpr = AssertBinary(addExpr.Right, TokenType.Star);
        AssertNumber(mulExpr.Left, 3.0);
        AssertNumber(mulExpr.Right, 4.0);
    }

    [Fact]
    public void Parse_UnaryMinusInsideDeepNesting_ParsesCorrectly()
    {
        // Arrange & Act: "(((-5)))"
        var result = ParseString("(((-5)))");

        // Assert
        var expr = GetExpression(result);
        var unary = AssertUnary(expr, TokenType.Minus);
        AssertNumber(unary.Operand, 5.0);
    }

    [Fact]
    public void Parse_MultipleUnaryInBinaryContext_ParsesCorrectly()
    {
        // Arrange & Act: "-1 * -2"
        var result = ParseString("-1 * -2");

        // Assert
        var expr = GetExpression(result);
        var mulExpr = AssertBinary(expr, TokenType.Star);

        var leftUnary = AssertUnary(mulExpr.Left, TokenType.Minus);
        AssertNumber(leftUnary.Operand, 1.0);

        var rightUnary = AssertUnary(mulExpr.Right, TokenType.Minus);
        AssertNumber(rightUnary.Operand, 2.0);
    }

    [Fact]
    public void Parse_UnaryFollowedByParenthesizedUnary_ParsesCorrectly()
    {
        // Arrange & Act: "-(-5)"
        var result = ParseString("-(-5)");

        // Assert
        var expr = GetExpression(result);
        var outerUnary = AssertUnary(expr, TokenType.Minus);
        var innerUnary = AssertUnary(outerUnary.Operand, TokenType.Minus);
        AssertNumber(innerUnary.Operand, 5.0);
    }

    [Theory]
    [InlineData("1+2", TokenType.Plus)]
    [InlineData("1-2", TokenType.Minus)]
    [InlineData("1*2", TokenType.Star)]
    [InlineData("1/2", TokenType.Slash)]
    public void Parse_CompactBinaryExpressions_ParseCorrectly(string input, TokenType expectedOp)
    {
        // Arrange & Act
        var result = ParseString(input);

        // Assert
        var expr = GetExpression(result);
        var binary = Assert.IsType<Expression.Binary>(expr);
        Assert.Equal(expectedOp, binary.Operator);
    }

    [Fact]
    public void Parse_ParserConstructor_ThrowsOnNullTokens()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Parser(null!));
    }

    #endregion

    #region Direct Token Tests (Testing Parser with raw tokens)

    [Fact]
    public void Parse_TokenList_NumberToken_ReturnsNumberExpression()
    {
        // Arrange
        var tokens = CreateTokens(
            new Token(TokenType.Number, "42", 0)
        );

        // Act
        var result = Parser.Parse(tokens);

        // Assert
        var expr = GetExpression(result);
        AssertNumber(expr, 42.0);
    }

    [Fact]
    public void Parse_TokenList_SimpleBinaryExpression_ParsesCorrectly()
    {
        // Arrange
        var tokens = CreateTokens(
            new Token(TokenType.Number, "2", 0),
            new Token(TokenType.Plus, "+", 1),
            new Token(TokenType.Number, "3", 2)
        );

        // Act
        var result = Parser.Parse(tokens);

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Plus);
        AssertNumber(binary.Left, 2.0);
        AssertNumber(binary.Right, 3.0);
    }

    [Fact]
    public void Parse_TokenList_UnaryMinus_ParsesCorrectly()
    {
        // Arrange
        var tokens = CreateTokens(
            new Token(TokenType.Minus, "-", 0),
            new Token(TokenType.Number, "5", 1)
        );

        // Act
        var result = Parser.Parse(tokens);

        // Assert
        var expr = GetExpression(result);
        var unary = AssertUnary(expr, TokenType.Minus);
        AssertNumber(unary.Operand, 5.0);
    }

    [Fact]
    public void Parse_TokenList_ParenthesizedExpression_ParsesCorrectly()
    {
        // Arrange: (2 + 3)
        var tokens = CreateTokens(
            new Token(TokenType.LeftParen, "(", 0),
            new Token(TokenType.Number, "2", 1),
            new Token(TokenType.Plus, "+", 2),
            new Token(TokenType.Number, "3", 3),
            new Token(TokenType.RightParen, ")", 4)
        );

        // Act
        var result = Parser.Parse(tokens);

        // Assert
        var expr = GetExpression(result);
        var binary = AssertBinary(expr, TokenType.Plus);
        AssertNumber(binary.Left, 2.0);
        AssertNumber(binary.Right, 3.0);
    }

    #endregion
}
