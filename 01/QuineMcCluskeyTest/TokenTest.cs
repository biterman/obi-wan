using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuineMcCluskey;
using QuineMcCluskey.Parser;

namespace QuineMcCluskeyTest
{
    [TestClass]
    public class TokenTest
    {
        [TestMethod]
        public void TestTokenInheritance()
        {
            AndOperator andOp = new AndOperator();
            Assert.IsTrue(andOp.value == '*');
            Assert.IsInstanceOfType(andOp, typeof(AndOperator));
            Assert.IsInstanceOfType(andOp, typeof(OperatorToken));
            Assert.IsInstanceOfType(andOp, typeof(Token));
        }
        [TestMethod]
        public void TestLiteralConstructorWithValidInput()
        {
            Literal a = new Literal('A');
            Literal z = new Literal('Z');
            Assert.IsTrue(a.Match('A') && z.Match('Z'));
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidCharacterException))]
        public void TestLiteralConstructorWithInvalidInput()
        {
            Literal invalid = new Literal(' ');
        }
        [TestMethod]
        public void TestLiteralEquals()
        {
            Literal a1 = new Literal('A');
            Assert.AreNotEqual(a1, null);
            Assert.AreNotEqual(a1, new Literal('B'));
            Literal a2 = new Literal('A');
            Assert.AreEqual(a1, a2);
        }
    }
}
