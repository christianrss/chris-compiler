using ChrisCompiler.CodeAnalysis.Syntax;

namespace ChrisCompiler.CodeAnalysis
{
    class Evaluator
    {
        private readonly ExpressionSyntax _root;
        public Evaluator(ExpressionSyntax root)
        {
            this._root = root;
        }

        public object? Evaluate()
        {
            return EvaluateExpression(_root);
        }


        public int OperatorPrecedence(SyntaxToken op)
        {
            if (op.Kind == SyntaxKind.PlusToken || op.Kind == SyntaxKind.MinusToken)
            {
                return 0;
            }
            else if (op.Kind == SyntaxKind.MultiplyToken || op.Kind == SyntaxKind.DivideToken)
            {
                return 1;
            }
            else if (op.Kind == SyntaxKind.PowerToken)
            {
                return 2;
            }
            return -1;
        }

        public bool GreaterPrecedence(SyntaxToken op1, SyntaxToken op2)
        {
            return this.OperatorPrecedence(op1) >= this.OperatorPrecedence(op2);
        }

        public bool IsParenthesis(SyntaxToken token)
        {
            if (token.Kind == SyntaxKind.OpenParenthesisToken || token.Kind == SyntaxKind.CloseParenthesisToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
                
        public void ApplyOperator(Stack<SyntaxToken> operators, Stack<SyntaxToken> values)
        {
            var op = operators.Pop();
            var right = values.Pop();
            var left = values.Pop();

            decimal value = 0;

            switch (op.Kind)
            {
                case SyntaxKind.PlusToken:
                    value = Convert.ToDecimal(left.Value) + Convert.ToDecimal(right.Value);
                    break;

                case SyntaxKind.MinusToken:
                    value = Convert.ToDecimal(left.Value) - Convert.ToDecimal(right.Value);
                    break;

                case SyntaxKind.MultiplyToken:
                    value = Convert.ToDecimal(left.Value) * Convert.ToDecimal(right.Value);
                    break;

                case SyntaxKind.DivideToken:
                    value = Convert.ToDecimal(left.Value) / Convert.ToDecimal(right.Value);
                    break;

                case SyntaxKind.PowerToken:
                    value = (decimal)Math.Pow(
                        Convert.ToDouble(left.Value),
                        Convert.ToDouble(right.Value)
                    );
                    break;
            }

            values.Push(new SyntaxToken(SyntaxKind.NumberToken, -1, value.ToString(), value));
        }

        public void ListExpressionTokens(ExpressionSyntax expression, List<SyntaxToken> expressionTokens)
        {
            if (expression is NumberExpressionSyntax n)
            {
                expressionTokens.Add(n.NumberToken);
            }
            else if (expression is BinaryExpressionSyntax b)
            {
                ListExpressionTokens(b.Left, expressionTokens);
                expressionTokens.Add(b.OperatorToken);
                ListExpressionTokens(b.Right, expressionTokens);
            }
            else if (expression is ParenthesizedExpressionSyntax p)
            {
                expressionTokens.Add(p.OpenParenthesisToken);
                ListExpressionTokens(p.Expression, expressionTokens);
                expressionTokens.Add(p.CloseParenthesisToken);
            }
        }

        public object EvaluateOperation(ExpressionSyntax expression)
        {
            var values = new Stack<SyntaxToken>();
            var operators = new Stack<SyntaxToken>();

            List<SyntaxToken> expressionTokens = new List<SyntaxToken>();
            ListExpressionTokens(expression, expressionTokens);

            foreach (var token in expressionTokens)
            {
                if (token.Kind == SyntaxKind.NumberToken)
                {
                    values.Push(token);
                }
                else if (token.Kind == SyntaxKind.OpenParenthesisToken)
                {
                    operators.Push(token);
                }
                else if (token.Kind == SyntaxKind.CloseParenthesisToken)
                {
                    while (operators.Count > 0 &&
                        operators.Peek().Kind != SyntaxKind.OpenParenthesisToken)
                    {
                        this.ApplyOperator(operators, values);
                    }
                    operators.Pop();
                }
                else
                {
                    while (operators.Count > 0 &&
                        this.IsParenthesis(operators.Peek()) == false &&
                        GreaterPrecedence(operators.Peek(), token))
                    {
                        this.ApplyOperator(operators, values);
                    }
                    operators.Push(token);
                }
            }

            while (operators.Count > 0)
            {
                this.ApplyOperator(operators, values);
            }

            return values.Peek().Value!;
        }


        private object? EvaluateExpression(ExpressionSyntax node)
        {
            string eval = "";
            if (node is NumberExpressionSyntax n)
                eval += Convert.ToDecimal(n.NumberToken.Value);
            if (node is BinaryExpressionSyntax b)
                eval += EvaluateOperation(b);
            if (node is ParenthesizedExpressionSyntax p)
                eval += EvaluateOperation(p);
            if (node is StringExpressionSyntax s)
                eval += Convert.ToString(s.StringToken.Value);

            return eval;

            //throw new Exception($"Unexpected node {node.Kind}");
        }
    }

}