namespace Calculator.Tests;

/// <summary>
/// Comprehensive test suite for the Calculator class.
/// Demonstrates TDD best practices with happy path, edge case,
/// parameterized, and boundary tests.
/// </summary>
public class CalculatorTests
{
    private readonly ICalculator _calculator;

    public CalculatorTests()
    {
        _calculator = new Calculator();
    }

    #region Happy Path Tests - Basic Operations

    [Fact]
    public void Add_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        double a = 5;
        double b = 3;

        // Act
        var result = _calculator.Add(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(8, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Subtract_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        double a = 10;
        double b = 4;

        // Act
        var result = _calculator.Subtract(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(6, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Multiply_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        double a = 6;
        double b = 7;

        // Act
        var result = _calculator.Multiply(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Divide_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        double a = 20;
        double b = 4;

        // Act
        var result = _calculator.Divide(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void SquareRoot_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        double n = 16;

        // Act
        var result = _calculator.SquareRoot(n);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Power_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        double baseNum = 2;
        double exponent = 10;

        // Act
        var result = _calculator.Power(baseNum, exponent);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1024, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Modulo_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        double a = 17;
        double b = 5;

        // Act
        var result = _calculator.Modulo(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value);
        Assert.Null(result.Error);
    }

    #endregion

    #region Edge Case Tests - Error Conditions

    [Fact]
    public void Divide_ByZero_ReturnsFailure()
    {
        // Arrange
        double a = 10;
        double b = 0;

        // Act
        var result = _calculator.Divide(a, b);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot divide by zero.", result.Error);
    }

    [Fact]
    public void SquareRoot_OfNegativeNumber_ReturnsFailure()
    {
        // Arrange
        double n = -4;

        // Act
        var result = _calculator.SquareRoot(n);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot calculate square root of a negative number.", result.Error);
    }

    [Fact]
    public void Modulo_ByZero_ReturnsFailure()
    {
        // Arrange
        double a = 10;
        double b = 0;

        // Act
        var result = _calculator.Modulo(a, b);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot perform modulo by zero.", result.Error);
    }

    [Fact]
    public void SquareRoot_OfZero_ReturnsSuccess()
    {
        // Arrange
        double n = 0;

        // Act
        var result = _calculator.SquareRoot(n);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void Power_WithZeroExponent_ReturnsOne()
    {
        // Arrange
        double baseNum = 999;
        double exponent = 0;

        // Act
        var result = _calculator.Power(baseNum, exponent);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Power_WithNegativeExponent_ReturnsFraction()
    {
        // Arrange
        double baseNum = 2;
        double exponent = -1;

        // Act
        var result = _calculator.Power(baseNum, exponent);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0.5, result.Value);
    }

    #endregion

    #region Parameterized Tests - Multiple Scenarios

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(-1, -1, -2)]
    [InlineData(0, 5, 5)]
    [InlineData(1.5, 2.5, 4)]
    [InlineData(100, 200, 300)]
    [InlineData(-50, 50, 0)]
    public void Add_MultipleScenarios_ReturnsExpectedResult(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Add(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData(10, 3, 7)]
    [InlineData(5, 5, 0)]
    [InlineData(0, 5, -5)]
    [InlineData(-5, -3, -2)]
    [InlineData(100.5, 50.5, 50)]
    [InlineData(-10, 5, -15)]
    public void Subtract_MultipleScenarios_ReturnsExpectedResult(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Subtract(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData(2, 3, 6)]
    [InlineData(-2, 3, -6)]
    [InlineData(-2, -3, 6)]
    [InlineData(0, 100, 0)]
    [InlineData(1.5, 2, 3)]
    [InlineData(10, 10, 100)]
    public void Multiply_MultipleScenarios_ReturnsExpectedResult(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Multiply(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData(10, 2, 5)]
    [InlineData(9, 3, 3)]
    [InlineData(7, 2, 3.5)]
    [InlineData(-10, 2, -5)]
    [InlineData(-10, -2, 5)]
    [InlineData(1, 4, 0.25)]
    public void Divide_MultipleValidScenarios_ReturnsExpectedResult(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Divide(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData(4, 2)]
    [InlineData(9, 3)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(25, 5)]
    [InlineData(100, 10)]
    public void SquareRoot_MultipleValidScenarios_ReturnsExpectedResult(double n, double expected)
    {
        // Act
        var result = _calculator.SquareRoot(n);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData(2, 3, 8)]
    [InlineData(5, 0, 1)]
    [InlineData(2, -1, 0.5)]
    [InlineData(10, 2, 100)]
    [InlineData(3, 3, 27)]
    [InlineData(2, 10, 1024)]
    public void Power_MultipleScenarios_ReturnsExpectedResult(double baseNum, double exponent, double expected)
    {
        // Act
        var result = _calculator.Power(baseNum, exponent);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData(10, 3, 1)]
    [InlineData(17, 5, 2)]
    [InlineData(20, 4, 0)]
    [InlineData(7, 3, 1)]
    [InlineData(15.5, 3, 0.5)]
    public void Modulo_MultipleValidScenarios_ReturnsExpectedResult(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Modulo(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    #endregion

    #region Boundary Tests - Extreme Values

    [Fact]
    public void Add_WithMaxValue_HandlesCorrectly()
    {
        // Arrange
        double a = double.MaxValue;
        double b = 0;

        // Act
        var result = _calculator.Add(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(double.MaxValue, result.Value);
    }

    [Fact]
    public void Add_WithMinValue_HandlesCorrectly()
    {
        // Arrange
        double a = double.MinValue;
        double b = 0;

        // Act
        var result = _calculator.Add(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(double.MinValue, result.Value);
    }

    [Fact]
    public void Multiply_WithMaxValue_ProducesInfinity()
    {
        // Arrange
        double a = double.MaxValue;
        double b = 2;

        // Act
        var result = _calculator.Multiply(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(double.IsPositiveInfinity(result.Value));
    }

    [Fact]
    public void Multiply_VerySmallNumbers_HandlesCorrectly()
    {
        // Arrange
        double a = 1e-300;
        double b = 1e-100;

        // Act
        var result = _calculator.Multiply(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        // Result should be very close to zero but still positive
        Assert.True(result.Value >= 0);
    }

    [Fact]
    public void Divide_VerySmallByLarge_ReturnsNearZero()
    {
        // Arrange
        double a = 1e-300;
        double b = 1e300;

        // Act
        var result = _calculator.Divide(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value >= 0 && result.Value < 1e-100);
    }

    [Fact]
    public void SquareRoot_VerySmallPositive_HandlesCorrectly()
    {
        // Arrange
        double n = 1e-100;

        // Act
        var result = _calculator.SquareRoot(n);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value > 0);
        // Square root of 1e-100 should be 1e-50
        Assert.Equal(1e-50, result.Value, 10);
    }

    [Fact]
    public void Subtract_MaxMinusMin_HandlesCorrectly()
    {
        // Arrange
        double a = double.MaxValue;
        double b = double.MaxValue;

        // Act
        var result = _calculator.Subtract(a, b);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void Power_LargeExponent_ProducesInfinity()
    {
        // Arrange
        double baseNum = 10;
        double exponent = 1000;

        // Act
        var result = _calculator.Power(baseNum, exponent);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(double.IsPositiveInfinity(result.Value));
    }

    #endregion
}
