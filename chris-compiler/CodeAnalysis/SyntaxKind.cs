namespace ChrisCompiler.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        EndOfFileToken,
        WhitespaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        PowerToken,
        StringToken,

        // Expressions
        NumberExpression,
        BinaryExpression,
        ParenthesizedExpression,
        StringExpression,
    }
}