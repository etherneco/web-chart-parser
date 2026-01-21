using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WebChartParse.Models
{
    public class Parser
    {
        private const string UnaryNegate = "neg";

        public double Parse(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return 0;
            }

            string normalized = NormalizeExpression(expression);
            List<Token> tokens = Tokenize(normalized);
            List<Token> expandedTokens = InsertImplicitMultiplication(tokens);
            List<Token> rpn = ToRpn(expandedTokens);
            return EvaluateRpn(rpn);
        }

        private static string NormalizeExpression(string expression)
        {
            string trimmed = expression.Trim();
            StringBuilder builder = new StringBuilder(trimmed.Length);

            for (int i = 0; i < trimmed.Length; i++)
            {
                char current = trimmed[i];
                switch (current)
                {
                    case '×':
                    case '·':
                        builder.Append('*');
                        break;
                    case '÷':
                        builder.Append('/');
                        break;
                    case '−':
                    case '–':
                    case '—':
                        builder.Append('-');
                        break;
                    case '√':
                        builder.Append("sqrt");
                        break;
                    case 'π':
                        builder.Append("pi");
                        break;
                    case ',':
                        if (IsDecimalComma(trimmed, i))
                        {
                            builder.Append('.');
                        }
                        else
                        {
                            builder.Append(',');
                        }
                        break;
                    default:
                        builder.Append(current);
                        break;
                }
            }

            return builder.ToString().ToLowerInvariant();
        }

        private static bool IsDecimalComma(string value, int index)
        {
            if (index <= 0 || index >= value.Length - 1)
            {
                return false;
            }

            return char.IsDigit(value[index - 1]) && char.IsDigit(value[index + 1]);
        }

        private static List<Token> Tokenize(string expression)
        {
            List<Token> tokens = new List<Token>();

            int index = 0;
            while (index < expression.Length)
            {
                char current = expression[index];

                if (char.IsWhiteSpace(current))
                {
                    index++;
                    continue;
                }

                if (char.IsDigit(current) || current == '.')
                {
                    int start = index;
                    bool hasDot = current == '.';
                    index++;

                    while (index < expression.Length)
                    {
                        char next = expression[index];
                        if (char.IsDigit(next))
                        {
                            index++;
                            continue;
                        }

                        if (next == '.' && !hasDot)
                        {
                            hasDot = true;
                            index++;
                            continue;
                        }

                        break;
                    }

                    string numberText = expression.Substring(start, index - start);
                    double number = double.Parse(numberText, CultureInfo.InvariantCulture);
                    tokens.Add(Token.Number(number));
                    continue;
                }

                if (char.IsLetter(current))
                {
                    int start = index;
                    index++;
                    while (index < expression.Length && char.IsLetter(expression[index]))
                    {
                        index++;
                    }

                    string identifier = expression.Substring(start, index - start);
                    tokens.Add(Token.Identifier(identifier));
                    continue;
                }

                switch (current)
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '^':
                        tokens.Add(Token.Operator(current.ToString()));
                        index++;
                        break;
                    case '(':
                        tokens.Add(Token.LeftParen());
                        index++;
                        break;
                    case ')':
                        tokens.Add(Token.RightParen());
                        index++;
                        break;
                    case ',':
                        tokens.Add(Token.Comma());
                        index++;
                        break;
                    default:
                        index++;
                        break;
                }
            }

            return tokens;
        }

        private static List<Token> InsertImplicitMultiplication(List<Token> tokens)
        {
            List<Token> expanded = new List<Token>();

            for (int i = 0; i < tokens.Count; i++)
            {
                Token current = tokens[i];
                if (expanded.Count > 0 && NeedsImplicitMultiply(expanded[expanded.Count - 1], current))
                {
                    expanded.Add(Token.Operator("*"));
                }

                expanded.Add(current);
            }

            return expanded;
        }

        private static bool NeedsImplicitMultiply(Token left, Token right)
        {
            if (left.Type == TokenType.Identifier && right.Type == TokenType.LeftParen && !IsConstant(left.Text))
            {
                return false;
            }

            bool leftValue = left.Type == TokenType.Number
                || left.Type == TokenType.Identifier
                || left.Type == TokenType.RightParen;
            bool rightValue = right.Type == TokenType.Number
                || right.Type == TokenType.Identifier
                || right.Type == TokenType.LeftParen;

            return leftValue && rightValue;
        }

        private static List<Token> ToRpn(List<Token> tokens)
        {
            List<Token> output = new List<Token>();
            Stack<Token> operators = new Stack<Token>();

            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];
                switch (token.Type)
                {
                    case TokenType.Number:
                        output.Add(token);
                        break;
                    case TokenType.Identifier:
                        if (IsFunction(tokens, i))
                        {
                            operators.Push(Token.Function(token.Text));
                        }
                        else
                        {
                            output.Add(token);
                        }
                        break;
                    case TokenType.Comma:
                        while (operators.Count > 0 && operators.Peek().Type != TokenType.LeftParen)
                        {
                            output.Add(operators.Pop());
                        }
                        break;
                    case TokenType.Operator:
                        Token opToken = NormalizeOperator(tokens, i, token.Text);
                        while (operators.Count > 0 && operators.Peek().Type == TokenType.Operator)
                        {
                            Token top = operators.Peek();
                            if (HasHigherPrecedence(top, opToken))
                            {
                                output.Add(operators.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }

                        operators.Push(opToken);
                        break;
                    case TokenType.LeftParen:
                        operators.Push(token);
                        break;
                    case TokenType.RightParen:
                        while (operators.Count > 0 && operators.Peek().Type != TokenType.LeftParen)
                        {
                            output.Add(operators.Pop());
                        }

                        if (operators.Count > 0 && operators.Peek().Type == TokenType.LeftParen)
                        {
                            operators.Pop();
                        }

                        if (operators.Count > 0 && operators.Peek().Type == TokenType.Function)
                        {
                            output.Add(operators.Pop());
                        }
                        break;
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop());
            }

            return output;
        }

        private static bool IsFunction(List<Token> tokens, int index)
        {
            if (index >= tokens.Count - 1)
            {
                return false;
            }

            if (IsConstant(tokens[index].Text))
            {
                return false;
            }

            return tokens[index + 1].Type == TokenType.LeftParen;
        }

        private static Token NormalizeOperator(List<Token> tokens, int index, string op)
        {
            if (op != "-")
            {
                return Token.Operator(op);
            }

            if (index == 0)
            {
                return Token.Operator(UnaryNegate);
            }

            Token prev = tokens[index - 1];
            if (prev.Type == TokenType.Operator || prev.Type == TokenType.LeftParen || prev.Type == TokenType.Comma)
            {
                return Token.Operator(UnaryNegate);
            }

            return Token.Operator(op);
        }

        private static bool HasHigherPrecedence(Token stackOperator, Token current)
        {
            OperatorInfo stackInfo = GetOperatorInfo(stackOperator.Text);
            OperatorInfo currentInfo = GetOperatorInfo(current.Text);

            if (stackInfo.Precedence > currentInfo.Precedence)
            {
                return true;
            }

            if (stackInfo.Precedence == currentInfo.Precedence && !currentInfo.RightAssociative)
            {
                return true;
            }

            return false;
        }

        private static OperatorInfo GetOperatorInfo(string op)
        {
            switch (op)
            {
                case UnaryNegate:
                    return new OperatorInfo(3, true, 1);
                case "^":
                    return new OperatorInfo(4, true, 2);
                case "*":
                case "/":
                    return new OperatorInfo(2, false, 2);
                case "+":
                case "-":
                    return new OperatorInfo(1, false, 2);
                default:
                    return new OperatorInfo(0, false, 2);
            }
        }

        private static double EvaluateRpn(List<Token> rpn)
        {
            Stack<double> values = new Stack<double>();

            foreach (Token token in rpn)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        values.Push(token.Number);
                        break;
                    case TokenType.Identifier:
                        values.Push(ResolveConstant(token.Text));
                        break;
                    case TokenType.Operator:
                        values.Push(EvaluateOperator(token.Text, values));
                        break;
                    case TokenType.Function:
                        values.Push(EvaluateFunction(token.Text, values));
                        break;
                }
            }

            return values.Count == 1 ? values.Pop() : 0;
        }

        private static double EvaluateOperator(string op, Stack<double> values)
        {
            OperatorInfo info = GetOperatorInfo(op);

            if (info.OperandCount == 1)
            {
                double value = values.Count > 0 ? values.Pop() : 0;
                return -value;
            }

            double right = values.Count > 0 ? values.Pop() : 0;
            double left = values.Count > 0 ? values.Pop() : 0;

            switch (op)
            {
                case "+":
                    return left + right;
                case "-":
                    return left - right;
                case "*":
                    return left * right;
                case "/":
                    return right == 0 ? 0 : left / right;
                case "^":
                    return Math.Pow(left, right);
                default:
                    return 0;
            }
        }

        private static double EvaluateFunction(string name, Stack<double> values)
        {
            switch (name)
            {
                case "abs":
                    return Math.Abs(Pop(values));
                case "sqrt":
                    double sqrtValue = Pop(values);
                    return sqrtValue < 0 ? 0 : Math.Sqrt(sqrtValue);
                case "sin":
                    return Math.Sin(Pop(values));
                case "cos":
                    return Math.Cos(Pop(values));
                case "tan":
                case "tg":
                    return Math.Tan(Pop(values));
                case "ctg":
                    double tanValue = Math.Tan(Pop(values));
                    return tanValue == 0 ? 0 : 1 / tanValue;
                case "asin":
                    return Math.Asin(Pop(values));
                case "acos":
                    return Math.Acos(Pop(values));
                case "atan":
                    return Math.Atan(Pop(values));
                case "sinh":
                    return Math.Sinh(Pop(values));
                case "cosh":
                    return Math.Cosh(Pop(values));
                case "tanh":
                    return Math.Tanh(Pop(values));
                case "log":
                    return Math.Log10(Pop(values));
                case "ln":
                    return Math.Log(Pop(values));
                case "exp":
                    return Math.Exp(Pop(values));
                case "floor":
                    return Math.Floor(Pop(values));
                case "ceil":
                case "ceiling":
                    return Math.Ceiling(Pop(values));
                case "round":
                    return Math.Round(Pop(values));
                case "pow":
                    {
                        double exponent = Pop(values);
                        double baseValue = Pop(values);
                        return Math.Pow(baseValue, exponent);
                    }
                case "min":
                    {
                        double right = Pop(values);
                        double left = Pop(values);
                        return Math.Min(left, right);
                    }
                case "max":
                    {
                        double right = Pop(values);
                        double left = Pop(values);
                        return Math.Max(left, right);
                    }
                case "atan2":
                    {
                        double x = Pop(values);
                        double y = Pop(values);
                        return Math.Atan2(y, x);
                    }
                default:
                    return 0;
            }
        }

        private static double Pop(Stack<double> values)
        {
            return values.Count > 0 ? values.Pop() : 0;
        }

        private static double ResolveConstant(string name)
        {
            switch (name)
            {
                case "pi":
                    return Math.PI;
                case "e":
                    return Math.E;
                default:
                    return 0;
            }
        }

        private static bool IsConstant(string name)
        {
            return name == "pi" || name == "e";
        }

        private readonly struct OperatorInfo
        {
            public OperatorInfo(int precedence, bool rightAssociative, int operandCount)
            {
                Precedence = precedence;
                RightAssociative = rightAssociative;
                OperandCount = operandCount;
            }

            public int Precedence { get; }

            public bool RightAssociative { get; }

            public int OperandCount { get; }
        }

        private enum TokenType
        {
            Number,
            Identifier,
            Operator,
            Function,
            LeftParen,
            RightParen,
            Comma,
        }

        private sealed class Token
        {
            private Token(TokenType type, string text, double number)
            {
                Type = type;
                Text = text;
                Number = number;
            }

            public TokenType Type { get; }

            public string Text { get; }

            public double Number { get; }

            public static Token Number(double value)
            {
                return new Token(TokenType.Number, string.Empty, value);
            }

            public static Token Identifier(string text)
            {
                return new Token(TokenType.Identifier, text, 0);
            }

            public static Token Operator(string text)
            {
                return new Token(TokenType.Operator, text, 0);
            }

            public static Token Function(string text)
            {
                return new Token(TokenType.Function, text, 0);
            }

            public static Token LeftParen()
            {
                return new Token(TokenType.LeftParen, "(", 0);
            }

            public static Token RightParen()
            {
                return new Token(TokenType.RightParen, ")", 0);
            }

            public static Token Comma()
            {
                return new Token(TokenType.Comma, ",", 0);
            }
        }
    }
}
