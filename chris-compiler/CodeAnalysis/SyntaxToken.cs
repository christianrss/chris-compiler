namespace ChrisCompiler.CodeAnalysis.Syntax
{
    public class SyntaxToken : SyntaxNode
    {
        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object? Value { get; }
        public SyntaxToken(SyntaxKind kind, int position, string text, object? value = null)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value!;
        }
        public bool IsOperator()
        {
            if (Kind == SyntaxKind.PlusToken
                || Kind == SyntaxKind.MinusToken
                || Kind == SyntaxKind.StarToken
                || Kind == SyntaxKind.SlashToken
                || Kind == SyntaxKind.PowerToken)
            {
                return true;
            }
            return false;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}