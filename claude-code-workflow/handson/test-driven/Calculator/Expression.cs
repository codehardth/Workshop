namespace Calculator;

public abstract record Expression
{
    public sealed record Number(double Value) : Expression;
    public sealed record Binary(Expression Left, TokenType Operator, Expression Right) : Expression;
    public sealed record Unary(TokenType Operator, Expression Operand) : Expression;

    private Expression() { }
}
