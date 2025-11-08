using ChrisCompiler.CodeAnalysis.Syntax;

namespace ChrisCompiler.CodeAnalysis
{
    public class Parser
    {
        public string Text { get; }
        private List<string> _Diagnostics = new List<string>();
        public int Position { get; private set; }
        private readonly SyntaxToken[] Tokens;
        public Parser(string text)
        {
            Text = text;
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.NextToken();
                if (token.Kind != SyntaxKind.WhitespaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            Tokens = tokens.ToArray();
            _Diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            _Diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}> expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null!, null);
        }

        private ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();
            while (Current.IsOperator())
            {
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();
            while (Current.IsOperator())
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            if (Current.Kind == SyntaxKind.StringToken)
            {
                return new StringExpressionSyntax(Current);
            }

            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_Diagnostics, expression, endOfFileToken);
        }

        /*public string Parse()
        {
            string result = string.Empty;
            foreach (var token in Tokens)
            {
                result += $"{token.Kind.ToString()} :  {token.Text} <br/>";
            }
            return result;
        }*/

        public IEnumerable<string> Diagnostics => _Diagnostics;

        private SyntaxToken Current => Peek(0);

        private SyntaxToken Peek(int offset)
        {
            int index = Position + offset;
            if (index >= Tokens.Length)
                return Tokens[Tokens.Length - 1];
            return Tokens[index];
        }

        private SyntaxToken NextToken()
        {
            SyntaxToken current = Current;
            Position++;
            return current;
        }

    }
}
