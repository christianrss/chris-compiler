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

                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, 0.00m);
            }
            return null;
        }

        public SyntaxToken? OperatorTokenize()
        {
            return Current switch
            {
                '+' => new SyntaxToken(SyntaxKind.PlusToken, Position++, "+", null),
                '-' => new SyntaxToken(SyntaxKind.MinusToken, Position++, "-", null),
                '*' => new SyntaxToken(SyntaxKind.StarToken, Position++, "*", null),
                '/' => new SyntaxToken(SyntaxKind.SlashToken, Position++, "/", null),
                '^' => new SyntaxToken(SyntaxKind.PowerToken, Position++, "^", null),
                '(' => new SyntaxToken(SyntaxKind.OpenParenthesisToken, Position++, "(", null),
                ')' => new SyntaxToken(SyntaxKind.CloseParenthesisToken, Position++, ")", null),
                _ => null
            };
        }

        public SyntaxToken Lex()
        {
            if (Position >= Text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, Position, "\0", null);

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

            this._Diagnostics.Add($"Unexpected character '{Current}' at position {Position}.");

            return new SyntaxToken(SyntaxKind.BadToken, Position++, Text.Substring(Position - 1, 1), 0.00m);
        }

    }
}