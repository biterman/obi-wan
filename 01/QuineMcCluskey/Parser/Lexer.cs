using System;
using System.Collections.Generic;
using System.IO;

namespace QuineMcCluskey.Parser
{
    public class Lexer
    {
        private StringReader _inputReader;
        public string expression { get; private set; }
        public List<Token> tokens { get; private set; }

        /* Constructors */
        public Lexer(string expr)
        {
            expression = expr;
            _inputReader = new StringReader(expression);
            Tokenize();
        }

        /* Methods */

        private void Tokenize()
        {
            tokens = new List<Token>();
            Token prev = null;
            Token t = null;
            while (_inputReader.Peek() != -1)
            {
                char c = (char)_inputReader.Read();
                switch (c)
                {
                    case Token.AND:
                        t = new AndOperator();
                        tokens.Add(t);
                        break;
                    case Token.OR:
                        t = new OrOperator();
                        tokens.Add(t);
                        break;
                    case Token.LEFT_PAREN:
                        // Insert implicit AND operators before (
                        if ((prev is RightParenthesis) || (prev is Literal) || (prev is NotOperator))
                            tokens.Add(new AndOperator());
                        t = new LeftParenthesis();
                        tokens.Add(t);
                        break;
                    case Token.RIGHT_PAREN:
                        t = new RightParenthesis();
                        tokens.Add(t);
                        break;
                    case Token.NOT:
                        t = new NotOperator();
                        tokens.Add(t);
                        break;
                    default:
                        t = new Literal(c);
                        // Insert implicit AND operators before Literals
                        if ((prev is RightParenthesis) || (prev is Literal) || (prev is NotOperator))
                            tokens.Add(new AndOperator());
                        tokens.Add(t);
                        break;
                }
                prev = t;
            }
        }
    }
}
