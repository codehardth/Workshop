using Calculator;

namespace Calculator.Tests;

/// <summary>
/// Comprehensive unit tests for the Lexer class.
/// Tests tokenization of numbers, operators, parentheses, whitespace handling, and error cases.
/// </summary>
public class LexerTests
{
    #region Helper Methods

    /// <summary>
    /// Helper to extract tokens from a successful result.
    /// </summary>
    private static List<Token> GetTokens(Result<List<Token>> result)
    {
        Assert.True(result.IsSuccess, "Expected successful tokenization");
        return ((Result<List<Token>>.Success)result).Value;
    }

    /// <summary>
    /// Helper to extract error message from a failed result.
    /// </summary>
    private static string GetError(Result<List<Token>> result)
    {
        Assert.True(result.IsFailure, "Expected tokenization failure");
        return ((Result<List<Token>>.Failure)result).Error;
    }

    #endregion

    #region Integer Tokenization Tests

    [Fact]
    public void Tokenize_SingleDigitInteger_ReturnsNumberAndEndOfInput()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("5");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("5", tokens[0].Lexeme);
        Assert.Equal(5.0, tokens[0].NumericValue);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_MultiDigitInteger42_ReturnsNumberTokenWithCorrectValue()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("42");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("42", tokens[0].Lexeme);
        Assert.Equal(42.0, tokens[0].NumericValue);
        Assert.Equal(0, tokens[0].Position);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_LargeInteger_ReturnsCorrectNumericValue()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("123456789");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(123456789.0, tokens[0].NumericValue);
        Assert.Equal("123456789", tokens[0].Lexeme);
    }

    [Fact]
    public void Tokenize_Zero_ReturnsNumberTokenWithZeroValue()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("0");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(0.0, tokens[0].NumericValue);
    }

    [Theory]
    [InlineData("0", 0.0)]
    [InlineData("1", 1.0)]
    [InlineData("9", 9.0)]
    [InlineData("10", 10.0)]
    [InlineData("100", 100.0)]
    [InlineData("999", 999.0)]
    public void Tokenize_VariousIntegers_ReturnsCorrectNumericValue(string input, double expectedValue)
    {
        // Arrange & Act
        var result = Lexer.Tokenize(input);

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(expectedValue, tokens[0].NumericValue);
    }

    #endregion

    #region Floating-Point Tokenization Tests

    [Fact]
    public void Tokenize_DecimalNumber314_ReturnsNumberTokenWithCorrectValue()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("3.14");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("3.14", tokens[0].Lexeme);
        Assert.Equal(3.14, tokens[0].NumericValue);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_DecimalWithLeadingZero_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("0.5");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(0.5, tokens[0].NumericValue);
        Assert.Equal("0.5", tokens[0].Lexeme);
    }

    [Fact]
    public void Tokenize_SmallDecimal_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("0.001");

        // Assert
        var tokens = GetTokens(result);
        Assert.NotNull(tokens[0].NumericValue);
        Assert.Equal(0.001, tokens[0].NumericValue!.Value, precision: 10);
    }

    [Fact]
    public void Tokenize_LongDecimalPrecision_PreservesValue()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("0.123456789");

        // Assert
        var tokens = GetTokens(result);
        Assert.NotNull(tokens[0].NumericValue);
        Assert.Equal(0.123456789, tokens[0].NumericValue!.Value, precision: 10);
    }

    [Theory]
    [InlineData("0.1", 0.1)]
    [InlineData("1.5", 1.5)]
    [InlineData("2.5", 2.5)]
    [InlineData("10.25", 10.25)]
    [InlineData("99.99", 99.99)]
    [InlineData("123.456", 123.456)]
    public void Tokenize_VariousDecimals_ReturnsCorrectNumericValue(string input, double expectedValue)
    {
        // Arrange & Act
        var result = Lexer.Tokenize(input);

        // Assert
        var tokens = GetTokens(result);
        Assert.NotNull(tokens[0].NumericValue);
        Assert.Equal(expectedValue, tokens[0].NumericValue!.Value, precision: 10);
    }

    [Fact]
    public void Tokenize_DecimalWithoutLeadingZero_ReturnsCorrectValue()
    {
        // Arrange & Act - Testing ".5" which should be supported per the Lexer implementation
        var result = Lexer.Tokenize(".5");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(".5", tokens[0].Lexeme);
        Assert.Equal(0.5, tokens[0].NumericValue);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Theory]
    [InlineData(".1", 0.1)]
    [InlineData(".25", 0.25)]
    [InlineData(".999", 0.999)]
    [InlineData(".123456", 0.123456)]
    public void Tokenize_DecimalsWithoutLeadingZero_ReturnsCorrectValues(string input, double expectedValue)
    {
        // Arrange & Act
        var result = Lexer.Tokenize(input);

        // Assert
        var tokens = GetTokens(result);
        Assert.NotNull(tokens[0].NumericValue);
        Assert.Equal(expectedValue, tokens[0].NumericValue!.Value, precision: 10);
    }

    #endregion

    #region Operator Tokenization Tests

    [Fact]
    public void Tokenize_PlusOperator_ReturnsPlusToken()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("+");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Plus, tokens[0].Type);
        Assert.Equal("+", tokens[0].Lexeme);
        Assert.Equal(0, tokens[0].Position);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_MinusOperator_ReturnsMinusToken()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("-");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Minus, tokens[0].Type);
        Assert.Equal("-", tokens[0].Lexeme);
        Assert.Equal(0, tokens[0].Position);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_StarOperator_ReturnsStarToken()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("*");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Star, tokens[0].Type);
        Assert.Equal("*", tokens[0].Lexeme);
        Assert.Equal(0, tokens[0].Position);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_SlashOperator_ReturnsSlashToken()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("/");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Slash, tokens[0].Type);
        Assert.Equal("/", tokens[0].Lexeme);
        Assert.Equal(0, tokens[0].Position);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Theory]
    [InlineData("+", TokenType.Plus, "+")]
    [InlineData("-", TokenType.Minus, "-")]
    [InlineData("*", TokenType.Star, "*")]
    [InlineData("/", TokenType.Slash, "/")]
    public void Tokenize_AllOperators_ReturnsCorrectTypeAndLexeme(string input, TokenType expectedType, string expectedLexeme)
    {
        // Arrange & Act
        var result = Lexer.Tokenize(input);

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(expectedLexeme, tokens[0].Lexeme);
    }

    #endregion

    #region Parentheses Tokenization Tests

    [Fact]
    public void Tokenize_LeftParenthesis_ReturnsLeftParenToken()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("(");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.LeftParen, tokens[0].Type);
        Assert.Equal("(", tokens[0].Lexeme);
        Assert.Equal(0, tokens[0].Position);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_RightParenthesis_ReturnsRightParenToken()
    {
        // Arrange & Act
        var result = Lexer.Tokenize(")");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.RightParen, tokens[0].Type);
        Assert.Equal(")", tokens[0].Lexeme);
        Assert.Equal(0, tokens[0].Position);
        Assert.Equal(TokenType.EndOfInput, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_EmptyParentheses_ReturnsLeftRightParenTokens()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("()");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.LeftParen, tokens[0].Type);
        Assert.Equal(0, tokens[0].Position);
        Assert.Equal(TokenType.RightParen, tokens[1].Type);
        Assert.Equal(1, tokens[1].Position);
        Assert.Equal(TokenType.EndOfInput, tokens[2].Type);
    }

    [Fact]
    public void Tokenize_NestedParentheses_ReturnsAllParenTokens()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("(())");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.LeftParen, tokens[0].Type);
        Assert.Equal(TokenType.LeftParen, tokens[1].Type);
        Assert.Equal(TokenType.RightParen, tokens[2].Type);
        Assert.Equal(TokenType.RightParen, tokens[3].Type);
        Assert.Equal(TokenType.EndOfInput, tokens[4].Type);
    }

    #endregion

    #region Expression Tokenization Tests

    [Fact]
    public void Tokenize_SimpleAdditionExpression_ReturnsNumberPlusNumberEndOfInput()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("2 + 3");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(4, tokens.Count);

        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(2.0, tokens[0].NumericValue);

        Assert.Equal(TokenType.Plus, tokens[1].Type);

        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(3.0, tokens[2].NumericValue);

        Assert.Equal(TokenType.EndOfInput, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_SimpleSubtractionExpression_ReturnsCorrectTokens()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("10 - 4");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(TokenType.Minus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(TokenType.EndOfInput, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_SimpleMultiplicationExpression_ReturnsCorrectTokens()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("5 * 6");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(TokenType.Star, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(TokenType.EndOfInput, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_SimpleDivisionExpression_ReturnsCorrectTokens()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("8 / 2");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(TokenType.Slash, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(TokenType.EndOfInput, tokens[3].Type);
    }

    #endregion

    #region Whitespace Handling Tests

    [Fact]
    public void Tokenize_ExpressionWithExcessiveWhitespace_IgnoresAllWhitespace()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("  2  +  3  ");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(2.0, tokens[0].NumericValue);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(3.0, tokens[2].NumericValue);
        Assert.Equal(TokenType.EndOfInput, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_LeadingWhitespace_IgnoresLeadingSpaces()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("   42");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(42.0, tokens[0].NumericValue);
    }

    [Fact]
    public void Tokenize_TrailingWhitespace_IgnoresTrailingSpaces()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("42   ");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(42.0, tokens[0].NumericValue);
    }

    [Fact]
    public void Tokenize_TabCharacters_TreatsTabsAsWhitespace()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("2\t+\t3");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
    }

    [Fact]
    public void Tokenize_NewlineCharacters_TreatsNewlinesAsWhitespace()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("2\n+\n3");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
    }

    [Fact]
    public void Tokenize_MixedWhitespace_HandlesAllWhitespaceTypes()
    {
        // Arrange & Act
        var result = Lexer.Tokenize(" \t\n 5 \t\n ");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(5.0, tokens[0].NumericValue);
    }

    [Fact]
    public void Tokenize_NoWhitespaceBetweenTokens_TokenizesCorrectly()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("2+3");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(2.0, tokens[0].NumericValue);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(3.0, tokens[2].NumericValue);
    }

    #endregion

    #region Empty Input Tests

    [Fact]
    public void Tokenize_EmptyString_ReturnsOnlyEndOfInputToken()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("");

        // Assert
        var tokens = GetTokens(result);
        Assert.Single(tokens);
        Assert.Equal(TokenType.EndOfInput, tokens[0].Type);
        Assert.Equal("", tokens[0].Lexeme);
        Assert.Equal(0, tokens[0].Position);
    }

    [Fact]
    public void Tokenize_WhitespaceOnlyInput_ReturnsOnlyEndOfInputToken()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("   ");

        // Assert
        var tokens = GetTokens(result);
        Assert.Single(tokens);
        Assert.Equal(TokenType.EndOfInput, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_MixedWhitespaceOnlyInput_ReturnsOnlyEndOfInputToken()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("  \t\n  \r\n  ");

        // Assert
        var tokens = GetTokens(result);
        Assert.Single(tokens);
        Assert.Equal(TokenType.EndOfInput, tokens[0].Type);
    }

    #endregion

    #region Invalid Character Error Tests

    [Fact]
    public void Tokenize_InvalidAtSymbol_ReturnsFailureWithErrorMessage()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("2 @ 3");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("Unexpected character '@'", error);
        Assert.Contains("position", error);
    }

    [Fact]
    public void Tokenize_InvalidDollarSign_ReturnsFailure()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("$100");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("Unexpected character '$'", error);
    }

    [Fact]
    public void Tokenize_InvalidHashSign_ReturnsFailure()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("#");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("Unexpected character '#'", error);
    }

    [Fact]
    public void Tokenize_InvalidAmpersand_ReturnsFailure()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("1 & 2");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("Unexpected character '&'", error);
    }

    [Theory]
    [InlineData("@", "@")]
    [InlineData("$", "$")]
    [InlineData("#", "#")]
    [InlineData("&", "&")]
    [InlineData("!", "!")]
    [InlineData("?", "?")]
    [InlineData("^", "^")]
    [InlineData("%", "%")]
    [InlineData("=", "=")]
    [InlineData("[", "[")]
    [InlineData("]", "]")]
    [InlineData("{", "{")]
    [InlineData("}", "}")]
    public void Tokenize_VariousInvalidCharacters_ReturnsFailureWithCharacterInMessage(string input, string expectedChar)
    {
        // Arrange & Act
        var result = Lexer.Tokenize(input);

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains($"Unexpected character '{expectedChar}'", error);
    }

    [Fact]
    public void Tokenize_InvalidCharacterInMiddle_ReportsCorrectPosition()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("2 + @ 3");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("position 4", error);
    }

    [Fact]
    public void Tokenize_InvalidCharacterAtStart_ReportsPositionZero()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("@5");

        // Assert
        Assert.True(result.IsFailure);
        var error = GetError(result);
        Assert.Contains("position 0", error);
    }

    #endregion

    #region Complex Expression Tests

    [Fact]
    public void Tokenize_ComplexParenthesizedExpression_ReturnsAllTokensInOrder()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("(2 + 3) * 4");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(8, tokens.Count);

        Assert.Equal(TokenType.LeftParen, tokens[0].Type);
        Assert.Equal("(", tokens[0].Lexeme);

        Assert.Equal(TokenType.Number, tokens[1].Type);
        Assert.Equal(2.0, tokens[1].NumericValue);

        Assert.Equal(TokenType.Plus, tokens[2].Type);

        Assert.Equal(TokenType.Number, tokens[3].Type);
        Assert.Equal(3.0, tokens[3].NumericValue);

        Assert.Equal(TokenType.RightParen, tokens[4].Type);
        Assert.Equal(")", tokens[4].Lexeme);

        Assert.Equal(TokenType.Star, tokens[5].Type);

        Assert.Equal(TokenType.Number, tokens[6].Type);
        Assert.Equal(4.0, tokens[6].NumericValue);

        Assert.Equal(TokenType.EndOfInput, tokens[7].Type);
    }

    [Fact]
    public void Tokenize_DeeplyNestedParentheses_ReturnsCorrectTokenSequence()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("((1))");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(6, tokens.Count);
        Assert.Equal(TokenType.LeftParen, tokens[0].Type);
        Assert.Equal(TokenType.LeftParen, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(1.0, tokens[2].NumericValue);
        Assert.Equal(TokenType.RightParen, tokens[3].Type);
        Assert.Equal(TokenType.RightParen, tokens[4].Type);
        Assert.Equal(TokenType.EndOfInput, tokens[5].Type);
    }

    [Fact]
    public void Tokenize_MultipleOperations_ReturnsCorrectTokenSequence()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("1 + 2 - 3 * 4 / 5");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(10, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(TokenType.Minus, tokens[3].Type);
        Assert.Equal(TokenType.Number, tokens[4].Type);
        Assert.Equal(TokenType.Star, tokens[5].Type);
        Assert.Equal(TokenType.Number, tokens[6].Type);
        Assert.Equal(TokenType.Slash, tokens[7].Type);
        Assert.Equal(TokenType.Number, tokens[8].Type);
        Assert.Equal(TokenType.EndOfInput, tokens[9].Type);
    }

    [Fact]
    public void Tokenize_UnaryMinusBeforeNumber_TokenizesAsSeparateTokens()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("-5");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Minus, tokens[0].Type);
        Assert.Equal(TokenType.Number, tokens[1].Type);
        Assert.Equal(5.0, tokens[1].NumericValue);
        Assert.Equal(TokenType.EndOfInput, tokens[2].Type);
    }

    [Fact]
    public void Tokenize_ConsecutiveOperators_TokenizesEachSeparately()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("+-*/");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.Plus, tokens[0].Type);
        Assert.Equal(TokenType.Minus, tokens[1].Type);
        Assert.Equal(TokenType.Star, tokens[2].Type);
        Assert.Equal(TokenType.Slash, tokens[3].Type);
        Assert.Equal(TokenType.EndOfInput, tokens[4].Type);
    }

    [Fact]
    public void Tokenize_ComplexExpressionWithDecimals_TokenizesCorrectly()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("(3.14 + 2.5) * 1.5");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(8, tokens.Count);
        Assert.Equal(3.14, tokens[1].NumericValue);
        Assert.Equal(2.5, tokens[3].NumericValue);
        Assert.Equal(1.5, tokens[6].NumericValue);
    }

    #endregion

    #region Position Tracking Tests

    [Fact]
    public void Tokenize_SimpleExpression_TracksCorrectPositions()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("2 + 3");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(0, tokens[0].Position); // '2' at position 0
        Assert.Equal(2, tokens[1].Position); // '+' at position 2
        Assert.Equal(4, tokens[2].Position); // '3' at position 4
    }

    [Fact]
    public void Tokenize_MultiDigitNumbers_TracksStartPosition()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("123 + 456");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(0, tokens[0].Position);  // '123' starts at 0
        Assert.Equal(4, tokens[1].Position);  // '+' at position 4
        Assert.Equal(6, tokens[2].Position);  // '456' starts at 6
    }

    [Fact]
    public void Tokenize_ExpressionWithLeadingWhitespace_TracksCorrectPositions()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("   5 + 3");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(3, tokens[0].Position); // '5' at position 3 (after 3 spaces)
        Assert.Equal(5, tokens[1].Position); // '+' at position 5
        Assert.Equal(7, tokens[2].Position); // '3' at position 7
    }

    [Fact]
    public void Tokenize_EndOfInputToken_HasCorrectPosition()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("42");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens[1].Position); // EndOfInput at position 2 (after "42")
    }

    #endregion

    #region NumericValue Property Tests

    [Fact]
    public void NumericValue_ForNumberToken_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("42");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(42.0, tokens[0].NumericValue);
    }

    [Fact]
    public void NumericValue_ForOperatorToken_ReturnsNull()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("+");

        // Assert
        var tokens = GetTokens(result);
        Assert.Null(tokens[0].NumericValue);
    }

    [Fact]
    public void NumericValue_ForParenToken_ReturnsNull()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("(");

        // Assert
        var tokens = GetTokens(result);
        Assert.Null(tokens[0].NumericValue);
    }

    [Fact]
    public void NumericValue_ForEndOfInputToken_ReturnsNull()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("");

        // Assert
        var tokens = GetTokens(result);
        Assert.Null(tokens[0].NumericValue);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Tokenize_SingleNumber_ReturnsNumberAndEndOfInput()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("7");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(2, tokens.Count);
    }

    [Fact]
    public void Tokenize_MultipleParenthesesWithNumber_ReturnsCorrectTokenCount()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("(((5)))");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(8, tokens.Count); // 3 left parens + 1 number + 3 right parens + EndOfInput
    }

    [Fact]
    public void Tokenize_LongExpression_HandlesCorrectly()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("1+2+3+4+5+6+7+8+9+10");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(20, tokens.Count); // 10 numbers + 9 plus signs + EndOfInput
    }

    [Fact]
    public void Tokenize_NumberFollowedByParen_TokenizesSeparately()
    {
        // Arrange & Act
        var result = Lexer.Tokenize("5(");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(TokenType.LeftParen, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_ParenFollowedByNumber_TokenizesSeparately()
    {
        // Arrange & Act
        var result = Lexer.Tokenize(")5");

        // Assert
        var tokens = GetTokens(result);
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.RightParen, tokens[0].Type);
        Assert.Equal(TokenType.Number, tokens[1].Type);
    }

    #endregion
}
