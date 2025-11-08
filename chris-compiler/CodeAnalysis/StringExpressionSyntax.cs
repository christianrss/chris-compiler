using ChrisCompiler.CodeAnalysis.Syntax;

namespace ChrisCompiler.CodeAnalysis.Syntax
{
    public sealed class StringExpressionSyntax : ExpressionSyntax
    {
        public StringExpressionSyntax(SyntaxToken stringToken)
        {
            StringToken = stringToken;
        }

        public override SyntaxKind Kind => SyntaxKind.StringExpression;
        public SyntaxToken StringToken { get; }
        
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return StringToken;
        }

    }
}