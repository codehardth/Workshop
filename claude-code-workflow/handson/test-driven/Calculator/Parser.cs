namespace Calculator;

/// <summary>
/// Recursive descent parser for mathematical expressions.
/// Converts a list of tokens into an Abstract Syntax Tree (AST).
///
/// Grammar:
///   expression -> term (('+' | '-') term)*
///   term       -> unary (('*' | '/') unary)*
///   unary      -> '-' unary | primary
///   primary    -> NUMBER | '(' expression ')'
///
/// Operator precedence (lowest to highest):
///   1. Addition, Subtraction (+, -)
///   2. Multiplication, Division (*, /)
///   3. Unary minus (-)
/// </summary>
public class Parser
{
    private readonly List<Token> _tokens;
    private int _current;

    /// <summary>
    /// Creates a new parser for the given token list.
    /// </summary>
    /// <param name="tokens">The list of tokens to parse</param>
    /// <exception cref="ArgumentNullException">Thrown when tokens is null</exception>
    public Parser(List<Token> tokens)
    {
        _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        _current = 0;
    }

    /// <summary>
    /// Parses the token stream into an expression AST.
    /// </summary>
    /// <returns>A Result containing either the parsed Expression or an error message</returns>
    public Result<Expression> Parse()
    {
        // Handle empty input
        if (_tokens.Count == 0)
        {
            return Result<Expression>.Fail("Empty expression: no tokens to parse");
        }

        // Handle input with only EndOfInput token
        if (_tokens.Count == 1 && _tokens[0].Type == TokenType.EndOfInput)
        {
            return Result<Expression>.Fail("Empty expression: expected a number or '('");
        }

        var result = ParseExpression();
        if (result.IsFailure)
            return result;

        // Ensure all tokens were consumed (except EndOfInput)
        if (!IsAtEnd())
        {
            var token = Peek();
            return Result<Expression>.Fail(
                $"Unexpected token '{token.Lexeme}' at position {token.Position}");
        }

        return result;
    }

    /// <summary>
    /// Parses an expression: term (('+' | '-') term)*
    /// Handles addition and subtraction (lowest precedence binary operators).
    /// </summary>
    private Result<Expression> ParseExpression()
    {
        var leftResult = ParseTerm();
        if (leftResult.IsFailure)
            return leftResult;

        var left = ((Result<Expression>.Success)leftResult).Value;

        while (Match(TokenType.Plus, TokenType.Minus))
        {
            var operatorType = Previous().Type;
            var rightResult = ParseTerm();
            if (rightResult.IsFailure)
                return rightResult;

            var right = ((Result<Expression>.Success)rightResult).Value;
            left = new Expression.Binary(left, operatorType, right);
        }

        return Result<Expression>.Ok(left);
    }

    /// <summary>
    /// Parses a term: unary (('*' | '/') unary)*
    /// Handles multiplication and division (higher precedence than +/-).
    /// </summary>
    private Result<Expression> ParseTerm()
    {
        var leftResult = ParseUnary();
        if (leftResult.IsFailure)
            return leftResult;

        var left = ((Result<Expression>.Success)leftResult).Value;

        while (Match(TokenType.Star, TokenType.Slash))
        {
            var operatorType = Previous().Type;
            var rightResult = ParseUnary();
            if (rightResult.IsFailure)
                return rightResult;

            var right = ((Result<Expression>.Success)rightResult).Value;
            left = new Expression.Binary(left, operatorType, right);
        }

        return Result<Expression>.Ok(left);
    }

    /// <summary>
    /// Parses a unary expression: '-' unary | primary
    /// Handles unary minus with right-associativity (e.g., --5 = -(-5)).
    /// Supports: -5, -(3+2), 3*-2
    /// </summary>
    private Result<Expression> ParseUnary()
    {
        if (Match(TokenType.Minus))
        {
            var operatorType = Previous().Type;
            var operandResult = ParseUnary(); // Recursive for right-associativity
            if (operandResult.IsFailure)
                return operandResult;

            var operand = ((Result<Expression>.Success)operandResult).Value;
            return Result<Expression>.Ok(new Expression.Unary(operatorType, operand));
        }

        return ParsePrimary();
    }

    /// <summary>
    /// Parses a primary expression: NUMBER | '(' expression ')'
    /// Handles the highest precedence: literals and parenthesized expressions.
    /// </summary>
    private Result<Expression> ParsePrimary()
    {
        // Number literal
        if (Match(TokenType.Number))
        {
            var token = Previous();
            var value = token.NumericValue ?? 0;
            return Result<Expression>.Ok(new Expression.Number(value));
        }

        // Parenthesized expression
        if (Match(TokenType.LeftParen))
        {
            var openParenPosition = Previous().Position;

            // Check for empty parentheses
            if (Check(TokenType.RightParen))
            {
                return Result<Expression>.Fail(
                    $"Empty parentheses at position {openParenPosition}");
            }

            var exprResult = ParseExpression();
            if (exprResult.IsFailure)
                return exprResult;

            if (!Match(TokenType.RightParen))
            {
                return Result<Expression>.Fail(
                    $"Mismatched parentheses: missing ')' for '(' at position {openParenPosition}");
            }

            return exprResult;
        }

        // Unexpected right parenthesis
        if (Check(TokenType.RightParen))
        {
            var token = Peek();
            return Result<Expression>.Fail(
                $"Mismatched parentheses: unexpected ')' at position {token.Position}");
        }

        // Unexpected end of input
        if (IsAtEnd())
        {
            return Result<Expression>.Fail("Unexpected end of input: expected a number or '('");
        }

        // Any other unexpected token
        var unexpectedToken = Peek();
        return Result<Expression>.Fail(
            $"Unexpected token '{unexpectedToken.Lexeme}' at position {unexpectedToken.Position}");
    }

    #region Helper Methods

    /// <summary>
    /// Checks if the current token matches any of the given types.
    /// If so, advances past the token and returns true.
    /// </summary>
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if the current token is of the given type.
    /// Does not consume the token.
    /// </summary>
    private bool Check(TokenType type)
    {
        if (IsAtEnd())
            return false;
        return Peek().Type == type;
    }

    /// <summary>
    /// Advances to the next token and returns the previous one.
    /// </summary>
    private Token Advance()
    {
        if (!IsAtEnd())
            _current++;
        return Previous();
    }

    /// <summary>
    /// Returns true if we've reached the end of the token stream.
    /// </summary>
    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EndOfInput;
    }

    /// <summary>
    /// Returns the current token without advancing.
    /// </summary>
    private Token Peek()
    {
        return _tokens[_current];
    }

    /// <summary>
    /// Returns the most recently consumed token.
    /// </summary>
    private Token Previous()
    {
        return _tokens[_current - 1];
    }

    #endregion

    #region Static Convenience Methods

    /// <summary>
    /// Static helper method to parse a token list directly.
    /// </summary>
    /// <param name="tokens">The list of tokens to parse</param>
    /// <returns>A Result containing either the parsed Expression or an error message</returns>
    public static Result<Expression> Parse(List<Token> tokens)
    {
        return new Parser(tokens).Parse();
    }

    #endregion
}
