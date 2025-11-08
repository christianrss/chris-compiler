namespace ChrisCompiler.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        DivideToken,
        MultiplyToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BinaryExpression,
        NumberExpression,
        StringExpression,
        ParenthesizedExpression,
        PowerToken,
        StringToken,
        BadToken,
        EndOfFileToken,
    }
}