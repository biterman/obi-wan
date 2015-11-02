using System;
using System.Collections.Generic;
using System.Linq;

namespace QuineMcCluskey.Parser
{
    /// <summary>
    /// This class represents a boolean expression in non-canonical disjunctive normal form,
    /// that is a sum of products where each term does not necessarily include every 
    /// propositional variable.
    /// </summary>
    public partial class ExpandedBooleanExpression
    {
        // Fields & Properties
        private Node _root;

        private string _asString;
        public string AsString
        {
            private set { _asString = AsString; }
            get { return _asString; }
        }

        private SortedSet<char> _variables = new SortedSet<char>();
        private IEnumerator<Token> _tokens;

        /// <summary>
        /// Construct an expanded boolean expression by parsing the <c>expr</c> parameter
        /// using recursive descent. The grammar parsed is:
        /// <code>
        /// expression = term [ + term ]
        /// term = factor[ * factor]
        /// factor = literal['] | (expression) [']
        /// literal = A | ... | Z
        /// </code>
        /// 
        /// objects of this type should represent a valid non-canonical sum of products 
        /// or nothing at all. Consequently, exceptions are
        /// thrown both from internal construction methods and those bubbling up from the lexer
        /// used to tokenize input.
        /// 
        /// </summary>
        /// <param name="expr">String holding the boolean expression to be parsed</param>
        public ExpandedBooleanExpression(string expr)
        {
            Parse(expr);
            TransformIntoDNF();
            // Generate the final string representation of the DNF expression
            _asString = _root.interpret();
        }

        // Methods
        public override string ToString()
        {
            return _asString;
        }
        public char[] GetAllVariables()
        {
            return _variables.ToArray();
        }
        public int GetVariableCount()
        {
            return _variables.Count;
        }

        private void Parse(string expr)
        {
            _tokens = new Lexer(expr).tokens.GetEnumerator();
            _tokens.MoveNext();
            _variables.Clear();
            _root = expression();
        }

        private Node expression()
        {
            Node n = term();
            while (_tokens.Current is OrOperator)
            {
                Or or = new Or(_tokens.Current);
                or.left = n;
                _tokens.MoveNext();
                n = term();
                or.right = n;
                n = or;
            }
            return n;
        }

        private Node term()
        {
            Node n = factor();
            while (_tokens.Current is AndOperator)
            {
                And and = new And(_tokens.Current);
                and.left = n;
                _tokens.MoveNext();
                n = factor();
                and.right = n;
                n = and;
            }
            return n;
        }

        private Node factor()
        {
            Node n = Literal();
            while (_tokens.Current is NotOperator)
            {
                Not not = new Not(_tokens.Current);
                not.left = n;
                n = not;
                _tokens.MoveNext();
            }
            return n;
        }

        private Node Literal()
        {
            Token t = _tokens.Current;
            _tokens.MoveNext();
            Node n = null;
            if (t is LeftParenthesis)
            {
                n = expression();
                if (_tokens.Current is RightParenthesis)
                {
                    // ignore right parenthesis
                    _tokens.MoveNext();
                }
                else
                {
                    throw new Exception(string.Format("Malformed expression: expected ')' found {0}",_tokens.Current));
                }
                
            }
            else if (t is Literal)
            {
                _variables.Add(t.value);
                n = new LiteralNode(t);
            }
            else
            {
                throw new Exception(string.Format("Malformed expression: parsing literal found {0}",_tokens.Current));
            }
            return n;
        }

        private void TransformIntoDNF()
        {
            // Push negations inwards by applying De Morgan laws recursively
            _root = SearchForNot(_root);
            // Turn the expression into disjunctive normal form by recursively
            // distributing conjunctions over disjunctions (ANDs over ORs)
            _root = ApplyDistributiveProperty(_root);
        }

        private Node SearchForNot(Node current)
        {
            if ((current == null) || (current is TerminalNode))
            {
                return current;
            }
            else if (!(current is Not))
            {
                NonTerminalNode nt = (NonTerminalNode) current;
                nt.left = SearchForNot(nt.left);
                nt.right = SearchForNot(nt.right);
                return nt;
            }
            else
            {
                return ApplyDeMorgan((current as Not).left);
            }
        }

        private Node ApplyDeMorgan(Node current)
        {
            // Pending: There should be a cleaner way of resolving all this casting
            if (current is LiteralNode)
            {
                return new ComplementNode(current.cargo);
            }
            else if (current is ComplementNode)
            {
                return new LiteralNode(current.cargo);
            }
            else if (current is And)
            {
                Or result = new Or(new OrOperator());
                result.left = ApplyDeMorgan((current as And).left);
                result.right = ApplyDeMorgan((current as And).right);
                return result;
            }
            else if (current is Or)
            {
                And result = new And(new AndOperator());
                result.left = ApplyDeMorgan((current as Or).left);
                result.right = ApplyDeMorgan((current as Or).right);
                return result;
            }
            else
            {
                return (current as Not).left;
            }
        }

        private Node ApplyDistributiveProperty(Node current)
        {
            if ((current == null) || (current is TerminalNode))
            {
                return current;
            }
            else if (!(current is And))
            {
                NonTerminalNode nt = (NonTerminalNode)current;
                nt.left = ApplyDistributiveProperty(nt.left);
                nt.right = ApplyDistributiveProperty(nt.right);
                return nt;
            }
            else
            {
                And and = (And) current;
                // Check for an OR child
                Or childOr = (and.left as Or);
                Node otherChild = and.right;
                if (childOr == null)
                {
                    childOr = (and.right as Or);
                    otherChild = and.left;
                }
                // If neither child is an OR simply recurse
                if (childOr == null)
                {
                    and.left = ApplyDistributiveProperty(and.left);
                    and.right = ApplyDistributiveProperty(and.right);
                    return and;
                }
                // If either child is an OR
                // convert (child.left OR child.right) AND otherChild into
                // (child.left AND otherChild) OR (child.right AND otherChild)
                else
                {
                    // Create new operator nodes
                    Or or = new Or(new OrOperator());
                    And left = new And(new AndOperator());
                    And right = new And(new AndOperator());
                    // Populate new terms
                    left.left = childOr.left;
                    left.right = otherChild;
                    right.left = childOr.right;
                    right.right = otherChild;
                    // Recursively apply distributive rule to new terms
                    or.left = ApplyDistributiveProperty(left);
                    or.right = ApplyDistributiveProperty(right);
                    return or;
                }
            }
        }
    }

}
