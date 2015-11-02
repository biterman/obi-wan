using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using QuineMcCluskey.Parser;

namespace QuineMcCluskeyTest
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void TestLexerTokenize()
        {
            /*
                Construct token list for AB+A'B'
            */
            List<Token> expected = new List<Token>();
            expected.Add(new Literal('A'));
            expected.Add(new AndOperator());
            expected.Add(new Literal('B'));
            expected.Add(new OrOperator());
            expected.Add(new Literal('A'));
            expected.Add(new NotOperator());
            expected.Add(new AndOperator());
            expected.Add(new Literal('B'));
            expected.Add(new NotOperator());
            //
            // implicit ANDs get inserted by lexer
            Lexer lexer = new Lexer("AB+A'B'");
            CollectionAssert.AreEqual(expected, lexer.tokens);
        }

        [TestMethod]
        public void TestImpliedAndOperators()
        {
            /*
                Construct token list for (A+B)C(D+E)(E'F')'
            */
            List<Token> expected = new List<Token>();
            expected.Add(new LeftParenthesis());
            expected.Add(new Literal('A'));
            expected.Add(new OrOperator());
            expected.Add(new Literal('B'));
            expected.Add(new RightParenthesis());
            expected.Add(new AndOperator());
            expected.Add(new Literal('C'));
            expected.Add(new AndOperator());
            expected.Add(new LeftParenthesis());
            expected.Add(new Literal('D'));
            expected.Add(new OrOperator());
            expected.Add(new Literal('E'));
            expected.Add(new RightParenthesis());
            expected.Add(new AndOperator());
            expected.Add(new LeftParenthesis());
            expected.Add(new Literal('E'));
            expected.Add(new NotOperator());
            expected.Add(new AndOperator());
            expected.Add(new Literal('F'));
            expected.Add(new NotOperator());
            expected.Add(new RightParenthesis());
            expected.Add(new NotOperator());
            //
            Lexer lexer = new Lexer("(A+B)C(D+E)(E'F')'");
            CollectionAssert.AreEqual(expected, lexer.tokens);
        }

        [TestMethod]
        public void TestDoubleNegation()
        {
            List<Token> expected = new List<Token>();
            expected.Add(new Literal('B'));
            expected.Add(new NotOperator());
            expected.Add(new NotOperator());
            Lexer lexer = new Lexer("B''");
            CollectionAssert.AreEqual(expected, lexer.tokens);
        }
    }
}
