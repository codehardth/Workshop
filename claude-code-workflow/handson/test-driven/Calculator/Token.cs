namespace Calculator;

public enum TokenType
{
    Number,
    Plus,
    Minus,
    Star,
    Slash,
    LeftParen,
    RightParen,
    EndOfInput
}

public record Token(TokenType Type, string Lexeme, int Position)
{
    public double? NumericValue => Type == TokenType.Number ? double.Parse(Lexeme) : null;
}
