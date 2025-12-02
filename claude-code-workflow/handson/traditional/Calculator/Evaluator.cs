namespace Calculator;

/// <summary>
/// Evaluates an expression AST to produce a numeric result.
/// Uses pattern matching on Expression types for clean, functional-style evaluation.
/// </summary>
public static class Evaluator
{
    /// <summary>
    /// Evaluates the given expression and returns the result.
    /// </summary>
    /// <param name="expression">The expression AST to evaluate</param>
    /// <returns>A Result containing either the computed value or an error message</returns>
    public static Result<double> Evaluate(Expression expression)
    {
        return expression switch
        {
            Expression.Number n => Result<double>.Ok(n.Value),
            Expression.Unary u => EvaluateUnary(u),
            Expression.Binary b => EvaluateBinary(b),
            _ => Result<double>.Fail("Unknown expression type")
        };
    }

    /// <summary>
    /// Evaluates a unary expression (negation).
    /// </summary>
    private static Result<double> EvaluateUnary(Expression.Unary expr)
    {
        var operandResult = Evaluate(expr.Operand);
        if (operandResult.IsFailure)
            return operandResult;

        var operand = ((Result<double>.Success)operandResult).Value;

        return expr.Operator switch
        {
            TokenType.Minus => Result<double>.Ok(-operand),
            _ => Result<double>.Fail($"Unknown unary operator: {expr.Operator}")
        };
    }

    /// <summary>
    /// Evaluates a binary expression (+, -, *, /).
    /// </summary>
    private static Result<double> EvaluateBinary(Expression.Binary expr)
    {
        var leftResult = Evaluate(expr.Left);
        if (leftResult.IsFailure)
            return leftResult;

        var rightResult = Evaluate(expr.Right);
        if (rightResult.IsFailure)
            return rightResult;

        var left = ((Result<double>.Success)leftResult).Value;
        var right = ((Result<double>.Success)rightResult).Value;

        return expr.Operator switch
        {
            TokenType.Plus => Result<double>.Ok(left + right),
            TokenType.Minus => Result<double>.Ok(left - right),
            TokenType.Star => Result<double>.Ok(left * right),
            TokenType.Slash => EvaluateDivision(left, right),
            _ => Result<double>.Fail($"Unknown binary operator: {expr.Operator}")
        };
    }

    /// <summary>
    /// Evaluates division with zero-check.
    /// </summary>
    private static Result<double> EvaluateDivision(double left, double right)
    {
        if (right == 0)
            return Result<double>.Fail("Division by zero");

        return Result<double>.Ok(left / right);
    }
}
