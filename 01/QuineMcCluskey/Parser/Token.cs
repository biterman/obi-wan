//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using System;

namespace QuineMcCluskey.Parser
{
    public abstract class Token : IEquatable<Token>
    {
        public const char AND = '*';
        public const char OR = '+';
        public const char NOT = '\'';
        public const char LEFT_PAREN = '(';
        public const char RIGHT_PAREN = ')';
        public const char START_ALPHA = 'A';
        public const char END_ALPHA = 'Z';

        public char value { get; protected set; }
        public virtual bool Match(char c)
        {
            return value == c;
        }
        public override string ToString()
        {
            return value.ToString();
        }
        // Override Equals and GetHashCode to make it easier to check equality
        // on collections of tokens, particularly during testing (see LexerTest.cs)
        public override bool Equals(object obj)
        {
            return Equals(obj as Token);
        }
        public bool Equals(Token t)
        {
            return (t != null) && (value == t.value);
        }
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }

    public abstract class OperatorToken : Token
    { }

    public class AndOperator : OperatorToken
    {
        public AndOperator()
        {
            value = Token.AND;
        }
        
    }

    public class OrOperator : OperatorToken
    {
        public OrOperator()
        {
            value = Token.OR;
        }
    }

    public class NotOperator : OperatorToken
    {
        public NotOperator()
        {
            value = NOT;
        }
    }

    public class LeftParenthesis : Token
    {
        public LeftParenthesis()
        {
            value = LEFT_PAREN;
        }
    }

    public class RightParenthesis : Token
    {
        public RightParenthesis()
        {
            value = RIGHT_PAREN;
        }
    }

    public class Literal : Token
    {
        public Literal(char c)
        {
            if (!IsValid(c))
            {
                throw new InvalidCharacterException("Invalid character (" + c.ToString() + ")");
            }
            //
            value = c;
        }
        public static bool IsValid(char c)
        {
            return (c >= START_ALPHA && c <= END_ALPHA);
        }
        public override bool Match(char c)
        {
            return IsValid(c);
        }
    }

    [Serializable()]
    public class InvalidCharacterException : System.Exception
    {
        public InvalidCharacterException() : base() { }
        public InvalidCharacterException(string message) : base(message) { }
        public InvalidCharacterException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidCharacterException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }

}
