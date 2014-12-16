using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RebarPosCommands
{
    public class Calculator
    {
        protected static bool IsOperator(char c)
        {
            string ops = "+-*/^()";
            return ops.Contains(c);
        }

        protected static bool IsNumeric(char c)
        {
            string nums = "1234567890.";
            return nums.Contains(c);
        }

        private static Queue<Token> Tokenize(string str)
        {
            Queue<Token> tokens = new Queue<Token>();
            string token = string.Empty;

            foreach (char c in str)
            {
                if (IsNumeric(c))
                {
                    token += c;
                }
                else if (IsOperator(c))
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        tokens.Enqueue(new Token(token));
                        token = string.Empty;
                    }
                    tokens.Enqueue(new Token(c));
                }
            }

            if (!string.IsNullOrEmpty(token))
            {
                tokens.Enqueue(new Token(token));
            }

            return tokens;
        }

        private static Queue<Token> InfixToRPN(Queue<Token> tokens)
        {
            Queue<Token> output = new Queue<Token>();
            Stack<Token> stack = new Stack<Token>();

            while (tokens.Count > 0)
            {
                Token token = tokens.Dequeue();

                if (!token.IsOperator)
                {
                    // If the token is a number, then add it to the output queue.
                    output.Enqueue(token);
                }
                else if (token.Operator == '(')
                {
                    // If the token is a left parenthesis, then push it onto the stack.
                    stack.Push(token);
                }
                else if (token.Operator == ')')
                {
                    if (stack.Count == 0)
                        throw new Exception("Parenthesis MisMatch");

                    // Pop operators from the stack to the output queue until we find opening parenthesis
                    while (stack.Peek().Operator != '(')
                    {
                        output.Enqueue(stack.Pop());
                    }

                    if (stack.Count == 0 || stack.Peek().Operator != '(')
                        throw new Exception("Parenthesis MisMatch");

                    // Pop left parenthesis
                    stack.Pop();
                }
                else
                {
                    Token o1 = token;
                    Token o2 = new Token('*');
                    if (stack.Count > 0) o2 = stack.Peek();
                    // Operator (o1)
                    // While there is an operator token, o2, at the top of the stack, and
                    //   either o1 is left-associative and its precedence is less than or equal to that of o2,
                    //   or o1 is right-associative and its precedence is less than that of o2,
                    //   pop o2 off the stack, onto the output queue;
                    // push o1 onto the stack.
                    while (stack.Count > 0 &&
                        stack.Peek().Operator != '(' && stack.Peek().Operator != ')' &&
                        ((token.IsLeftAssociative && (token <= stack.Peek())) || (!token.IsLeftAssociative && (token < stack.Peek()))))
                    {
                        output.Enqueue(stack.Pop());
                    }
                    stack.Push(token);
                }
            }

            // Pop remaining operators to the output queue.
            while (stack.Count > 0)
            {
                if ((stack.Peek().Operator == '(' || stack.Peek().Operator == ')'))
                    throw new Exception("Parenthesis MisMatch");

                output.Enqueue(stack.Pop());
            }

            return output;
        }

        public static double Evaluate(string formula)
        {
            Queue<Token> infixtokens = Tokenize(formula);
            Queue<Token> rpntokens = InfixToRPN(infixtokens);
            Stack<double> vals = new Stack<double>();

            while (rpntokens.Count > 0)
            {
                Token token = rpntokens.Dequeue();
                if (token.IsOperator)
                {
                    if (vals.Count < 2)
                        throw new Exception("Invalid Formula");
                    double op2 = vals.Pop();
                    double op1 = vals.Pop();
                    double res = token.Eval(op1, op2);
                    vals.Push(res);
                }
                else
                {
                    vals.Push(token.Value);
                }
            }

            if (vals.Count() != 1)
                throw new Exception("Invalid Formula");

            return vals.Peek();
        }

        public static bool IsValid(string formula)
        {
            try
            {
                Evaluate(formula);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryEvaluate(string formula, out double value)
        {
            try
            {
                double val = Evaluate(formula);
                value = val;
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }

        private struct Token
        {
            public char Operator;
            public double Value;
            public bool IsOperator;
            public bool IsLeftAssociative;
            public int Rank;

            public override bool Equals(object obj)
            {
                if (!(obj is Token)) return false;

                return (this == (Token)obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public static bool operator ==(Token t1, Token t2)
            {
                if (!t1.IsOperator || !t2.IsOperator)
                    throw new Exception("Not An Operator");
                return t1.Operator == t2.Operator;
            }
            public static bool operator !=(Token t1, Token t2)
            {
                if (!t1.IsOperator || !t2.IsOperator)
                    throw new Exception("Not An Operator");
                return t1.Operator != t2.Operator;
            }
            public static bool operator <(Token t1, Token t2)
            {
                if (!t1.IsOperator || !t2.IsOperator)
                    throw new Exception("Not An Operator");
                return t1.Rank < t2.Rank;
            }
            public static bool operator >(Token t1, Token t2)
            {
                if (!t1.IsOperator || !t2.IsOperator)
                    throw new Exception("Not An Operator");
                return t1.Rank > t2.Rank;
            }
            public static bool operator <=(Token t1, Token t2)
            {
                if (!t1.IsOperator || !t2.IsOperator)
                    throw new Exception("Not An Operator");
                return t1.Rank <= t2.Rank;
            }
            public static bool operator >=(Token t1, Token t2)
            {
                if (!t1.IsOperator || !t2.IsOperator)
                    throw new Exception("Not An Operator");
                return t1.Rank >= t2.Rank;
            }

            public double Eval(double a, double b)
            {
                if (!IsOperator)
                    throw new Exception("Not An Operator");

                switch (Operator)
                {
                    case '+': return a + b;
                    case '-': return a - b;
                    case '*': return a * b;
                    case '/': return a / b;
                    case '^': return Math.Pow(a, b);
                    default:
                        throw new Exception("Unknown Operator");
                }
            }

            public Token(char op)
            {
                IsOperator = true;
                Operator = op;
                IsLeftAssociative = (op == '+' || op == '-' || op == '*' || op == '/');
                Rank = ((op == '^') ? 2 : ((op == '*' || op == '/') ? 1 : 0));

                Value = 0.0;
            }

            public Token(double val)
            {
                IsOperator = false;
                Operator = '\0';
                IsLeftAssociative = false;
                Rank = 0;

                Value = val;
            }
            public Token(string val)
            {
                IsOperator = false;
                Operator = '\0';
                IsLeftAssociative = false;
                Rank = 0;

                double.TryParse(val, out Value);
            }
        }

    }
}
