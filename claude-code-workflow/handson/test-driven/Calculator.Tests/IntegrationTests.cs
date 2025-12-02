using Calculator;

namespace Calculator.Tests;

/// <summary>
/// End-to-end integration tests for the Expression Calculator.
/// Tests the complete pipeline: Lexer.Tokenize -> Parser.Parse -> Evaluator.Evaluate
/// </summary>
public class IntegrationTests
{
    #region Helper Methods

    /// <summary>
    /// Executes the full calculator pipeline: Lexer -> Parser -> Evaluator
    /// </summary>
    private static Result<double> Calculate(string input)
    {
        var tokenResult = Lexer.Tokenize(input);
        if (tokenResult is Result<List<Token>>.Failure f1)
            return Result<double>.Fail(f1.Error);

        var parseResult = Parser.Parse(((Result<List<Token>>.Success)tokenResult).Value);
        if (parseResult is Result<Expression>.Failure f2)
            return Result<double>.Fail(f2.Error);

        return Evaluator.Evaluate(((Result<Expression>.Success)parseResult).Value);
    }

    #endregion

    #region Success Test Cases

    [Theory]
    [InlineData("2 + 3", 5)]
    [InlineData("2 + 3 * 4", 14)]
    [InlineData("(2 + 3) * 4", 20)]
    [InlineData("-5", -5)]
    [InlineData("3 * -2", -6)]
    [InlineData("10 / 4", 2.5)]
    public void Calculate_ValidExpression_ReturnsExpectedResult(string input, double expected)
    {
        var result = Calculate(input);

        Assert.True(result.IsSuccess, $"Expected success for '{input}' but got failure");
        var value = ((Result<double>.Success)result).Value;
        Assert.Equal(expected, value);
    }

    #endregion

    #region Error Test Cases

    [Theory]
    [InlineData("10 / 0", "Division by zero")]
    [InlineData("(2 + 3", "parentheses")]
    public void Calculate_InvalidExpression_ReturnsExpectedError(string input, string expectedErrorSubstring)
    {
        var result = Calculate(input);

        Assert.True(result.IsFailure, $"Expected failure for '{input}' but got success");
        var error = ((Result<double>.Failure)result).Error;
        Assert.Contains(expectedErrorSubstring, error, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}
