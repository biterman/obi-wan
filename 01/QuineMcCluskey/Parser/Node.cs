using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuineMcCluskey.Parser
{
    public partial class ExpandedBooleanExpression
    {
        protected abstract class Node
        {
            public Token cargo;
            public Node(Token t)
            {
                cargo = t;
            }
            public abstract string interpret();
        }
        protected abstract class TerminalNode : Node
        {
            public TerminalNode(Token t) : base(t)
            {; }
            public override string interpret()
            {
                return cargo.ToString();
            }
        }
        protected abstract class NonTerminalNode : Node
        {
            public NonTerminalNode(Token t) : base(t)
            { }
            public Node left;
            public Node right;
        }
        protected class LiteralNode : TerminalNode
        {
            public LiteralNode(Token t) : base(t)
            { }
        }
        protected class ComplementNode : TerminalNode
        {
            public ComplementNode(Token t) : base(t)
            { }
            public override string interpret()
            {
                return base.interpret() + "'";
            }
        }
        protected class And : NonTerminalNode
        {
            public And(Token t) : base(t)
            { }
            public override string interpret()
            {
                return string.Format("{0}*{1}", left.interpret(), right.interpret());
            }
        }
        protected class Or : NonTerminalNode
        {
            public Or(Token t) : base(t)
            { }
            public override string interpret()
            {
                return string.Format("{0}+{1}", left.interpret(), right.interpret());
            }
        }
        protected class Not : NonTerminalNode
        {
            public Not(Token t) : base(t)
            { }
            public override string interpret()
            {
                if (left is TerminalNode)
                    return string.Format("{0}'", left.interpret());
                return string.Format("({0})'", left.interpret());
            }
        }
    }

}
