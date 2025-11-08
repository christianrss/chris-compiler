using ChrisCompiler.CodeAnalysis.Syntax;

namespace ChrisCompiler.CodeAnalysis
{
    internal sealed class Parser
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
                token = lexer.Lex();
                if (token.Kind != SyntaxKind.WhitespaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            Tokens = tokens.ToArray();
            _Diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            _Diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}> expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();
                var right = ParseExpression(precedence);
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
                var right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
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
