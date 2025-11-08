using System.Globalization;
using ChrisCompiler.CodeAnalysis.Syntax;

namespace ChrisCompiler.CodeAnalysis
{
    internal sealed class Lexer
    {
        private readonly string Text;
        private int Position;
        private List<string> _Diagnostics = new List<string>();

        public Lexer(string text)
        {
            Text = text;
            Position = 0;
        }

        public IEnumerable<string> Diagnostics => _Diagnostics;

        private char Current
        {
            get
            {
                if (Position >= Text.Length)
                    return '\0';
                return Text[Position];
            }
        }

        private void Next()
        {
            Position++;
        }

        public SyntaxToken? NumberTokenize()
        {
            if (char.IsDigit(Current))
            {
                int start = Position;
                while (char.IsDigit(Current) || Current == '.')
                    this.Next();

                int length = Position - start;
                string text = Text.Substring(start, length);

                if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                    this._Diagnostics.Add($"The fragment {text} isn't valid number.");

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }
            return null;
        }

        public SyntaxToken? WhitespaceTokenize()
        {
            if (char.IsWhiteSpace(Current))
            {
                int start = Position;
                while (char.IsWhiteSpace(Current))
                    this.Next();

                int length = Position - start;
                string text = Text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }
            return null;
        }

        public SyntaxToken? StringTokenize()
        {
            if (Current == '\'')
            {
                Next();
                int start = Position;
                while (Current != '\'' && Position <= Text.Length)
                    Next();

                if (Current == '\'')
                    Next();

                int length = Position + 1 - start;
                string text = Text.Substring(start - 1, length);
                string value = text.Replace("'", "");
                return new SyntaxToken(SyntaxKind.StringToken, start, text, value);
            }
            return null;
        }

        public SyntaxToken? OperatorTokenize()
        {
            if (Current == '+')
                return new SyntaxToken(SyntaxKind.PlusToken, Position++, "+", null);
            if (Current == '-')
                return new SyntaxToken(SyntaxKind.MinusToken, Position++, "-", null);
            if (Current == '*')
                return new SyntaxToken(SyntaxKind.StarToken, Position++, "*", null);
            if (Current == '/')
                return new SyntaxToken(SyntaxKind.SlashToken, Position++, "/", null);
            if (Current == '^')
                return new SyntaxToken(SyntaxKind.PowerToken, Position++, "^", null);
            if (Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, Position++, "(", null);
            if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, Position++, ")", null);
            return null;
        }

        public SyntaxToken NextToken()
        {
            if (Position >= Text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, Position, "\0");

            SyntaxToken? token = null;

            token = this.NumberTokenize();
            if (token != null)
                return token;

            token = this.WhitespaceTokenize();
            if (token != null)
                return token;

            token = this.OperatorTokenize();
            if (token != null)
                return token;

            token = this.StringTokenize();
            if (token != null)
                return token;

            this._Diagnostics.Add($"Unexpected character '{Current}' at position {Position}.");

            return new SyntaxToken(SyntaxKind.BadToken, Position++, Text.Substring(Position - 1, 1), null);
        }

    }
}