using System.Globalization;

namespace Calculator;

/// <summary>
/// Converts an input string into a stream of tokens.
/// The lexer handles numbers (integers and floating-point), operators, and parentheses.
/// </summary>
public class Lexer
{
    private readonly string _source;
    private int _position;
    private readonly List<Token> _tokens = new();

    /// <summary>
    /// Creates a new lexer for the given source string.
    /// </summary>
    public Lexer(string source)
    {
        _source = source ?? string.Empty;
    }

    /// <summary>
    /// Static helper method to tokenize a source string.
    /// </summary>
    /// <param name="source">The input expression string to tokenize</param>
    /// <returns>A Result containing either a list of tokens or an error message</returns>
    public static Result<List<Token>> Tokenize(string source)
    {
        var lexer = new Lexer(source);
        return lexer.ScanTokens();
    }

    /// <summary>
    /// Scans all tokens from the source string.
    /// </summary>
    /// <returns>A Result containing either a list of tokens or an error message</returns>
    private Result<List<Token>> ScanTokens()
    {
        while (!IsAtEnd())
        {
            SkipWhitespace();

            if (IsAtEnd())
                break;

            var result = ScanToken();
            if (result.IsFailure)
            {
                return Result<List<Token>>.Fail(((Result<Token>.Failure)result).Error);
            }

            _tokens.Add(((Result<Token>.Success)result).Value);
        }

        // Add end-of-input marker
        _tokens.Add(new Token(TokenType.EndOfInput, "", _position));

        return Result<List<Token>>.Ok(_tokens);
    }

    /// <summary>
    /// Scans a single token from the current position.
    /// </summary>
    private Result<Token> ScanToken()
    {
        var c = Peek();
        var startPosition = _position;

        // Single-character tokens
        switch (c)
        {
            case '+':
                Advance();
                return Result<Token>.Ok(new Token(TokenType.Plus, "+", startPosition));
            case '-':
                Advance();
                return Result<Token>.Ok(new Token(TokenType.Minus, "-", startPosition));
            case '*':
                Advance();
                return Result<Token>.Ok(new Token(TokenType.Star, "*", startPosition));
            case '/':
                Advance();
                return Result<Token>.Ok(new Token(TokenType.Slash, "/", startPosition));
            case '(':
                Advance();
                return Result<Token>.Ok(new Token(TokenType.LeftParen, "(", startPosition));
            case ')':
                Advance();
                return Result<Token>.Ok(new Token(TokenType.RightParen, ")", startPosition));
        }

        // Numbers (including numbers starting with decimal point like .5)
        if (char.IsDigit(c) || (c == '.' && PeekNext() is char next && char.IsDigit(next)))
        {
            return ScanNumber();
        }

        // Unknown character
        return Result<Token>.Fail($"Unexpected character '{c}' at position {_position}");
    }

    /// <summary>
    /// Scans a numeric literal (integer or floating-point).
    /// </summary>
    private Result<Token> ScanNumber()
    {
        var startPosition = _position;

        // Consume integer part
        while (!IsAtEnd() && char.IsDigit(Peek()))
        {
            Advance();
        }

        // Check for decimal point followed by digits
        if (!IsAtEnd() && Peek() == '.' && PeekNext() is char next && char.IsDigit(next))
        {
            // Consume the decimal point
            Advance();

            // Consume fractional part
            while (!IsAtEnd() && char.IsDigit(Peek()))
            {
                Advance();
            }
        }

        var lexeme = _source.Substring(startPosition, _position - startPosition);

        // Validate the number can be parsed
        if (double.TryParse(lexeme, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
        {
            return Result<Token>.Ok(new Token(TokenType.Number, lexeme, startPosition));
        }

        return Result<Token>.Fail($"Invalid number '{lexeme}' at position {startPosition}");
    }

    /// <summary>
    /// Skips any whitespace characters at the current position.
    /// </summary>
    private void SkipWhitespace()
    {
        while (!IsAtEnd() && char.IsWhiteSpace(Peek()))
        {
            Advance();
        }
    }

    /// <summary>
    /// Returns the current character without advancing.
    /// </summary>
    private char Peek()
    {
        return _source[_position];
    }

    /// <summary>
    /// Returns the next character without advancing, or null if at end.
    /// </summary>
    private char? PeekNext()
    {
        if (_position + 1 >= _source.Length)
            return null;
        return _source[_position + 1];
    }

    /// <summary>
    /// Advances to the next character and returns the current one.
    /// </summary>
    private char Advance()
    {
        return _source[_position++];
    }

    /// <summary>
    /// Returns true if we've reached the end of input.
    /// </summary>
    private bool IsAtEnd()
    {
        return _position >= _source.Length;
    }
}
