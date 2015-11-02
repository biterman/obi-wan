using System;
using QuineMcCluskey.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using QuineMcCluskey;

namespace QuineMcCluskeyTest
{
    [TestClass]
    public class ReducedBooleanExpressionTest
    {
        private ReducedBooleanExpression expr;
        private List<ExpressionTestCase> validCases = new List<ExpressionTestCase>();

        [TestInitialize]
        public void Initialize()
        {
            validCases.Add(new ExpressionTestCase("AAB(C+D')+A(B+D)", "AD+AB", "ABCD"));
            validCases.Add(new ExpressionTestCase("AB+CD", "AB+CD", "ABCD"));
            validCases.Add(new ExpressionTestCase("(P+Q)(R+S)", "PR+PS+QR+QS", "PQRS"));
            validCases.Add(new ExpressionTestCase("AB+AB'+BC+B'C'+A'D+AD", "B'C'+BC+A+D", "ABCD"));
            validCases.Add(new ExpressionTestCase("A'BC'D'+AB'C'D'+AB'CD'+AB'CD+ABC'D'+ABCD+AB'C'D+ABCD'", "BC'D'+AC+AB'", "ABCD"));
            validCases.Add(new ExpressionTestCase("A'B'C'D'+A'B'C'D+A'B'CD'+A'BC'D+A'BCD'+A'BCD+AB'C'D'+AB'C'D+AB'CD'+ABCD'", "B'C'+CD'+A'BC+A'C'D", "ABCD"));
            validCases.Add(new ExpressionTestCase("(C'D')(A'B+AB'+AB)", "AC'D'+BC'D'", "ABCD"));
            validCases.Add(new ExpressionTestCase("ABC+A'BC+CD+CD'", "C", "ABCD"));
            validCases.Add(new ExpressionTestCase("(M'N')''", "M'N'", "MN"));
        }
        [TestMethod]
        public void TestValidReducedExpressions()
        {
            foreach (ExpressionTestCase t in validCases)
            {
                expr = new ReducedBooleanExpression(t.inputExpression);
                Assert.AreEqual(t.expectedExpression, expr.ToString());
            }
        }
    }
}
