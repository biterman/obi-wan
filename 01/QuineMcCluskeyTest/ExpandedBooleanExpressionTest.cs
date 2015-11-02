using System;
using QuineMcCluskey.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace QuineMcCluskeyTest
{
    [TestClass]
    public class ExpandedBooleanExpressionTest
    {
        private ExpandedBooleanExpression expr;
        private List<ExpressionTestCase> validCases = new List<ExpressionTestCase>();
        private List<InvalidExpressionTestCase> invalidCases = new List<InvalidExpressionTestCase>();
        //
        [TestInitialize]
        public void Initialize()
        {
            validCases.Add(new ExpressionTestCase("A", "A", "A"));
            validCases.Add(new ExpressionTestCase("Z'", "Z'", "Z"));
            validCases.Add(new ExpressionTestCase("AB'+(CD)'", "A*B'+C'+D'", "ABCD"));
            validCases.Add(new ExpressionTestCase("A(B+C)", "B*A+C*A", "ABC"));
            validCases.Add(new ExpressionTestCase("(P+Q)(R+S)","R*P+S*P+R*Q+S*Q","PQRS"));
            validCases.Add(new ExpressionTestCase("N''", "N", "N"));
            validCases.Add(new ExpressionTestCase("(M'N')'", "M+N", "MN"));
            validCases.Add(new ExpressionTestCase("(M'N')''", "M'*N'", "MN"));
            //
            invalidCases.Add(new InvalidExpressionTestCase("((A)", "Malformed expression: expected ')'"));
            invalidCases.Add(new InvalidExpressionTestCase("+A+B", "Malformed expression: parsing literal"));
            invalidCases.Add(new InvalidExpressionTestCase("'(AB)", "Malformed expression: parsing literal"));
            invalidCases.Add(new InvalidExpressionTestCase("A+(BC)+", "Malformed expression: parsing literal"));
        }

        [TestMethod]
        public void TestVariableCount()
        {
            foreach (ExpressionTestCase te in validCases)
            {
                expr = new ExpandedBooleanExpression(te.inputExpression);
                Assert.AreEqual(expr.GetVariableCount(), te.variableCount);
            }
        }

        [TestMethod]
        public void TestVariableParsing()
        {
            foreach (ExpressionTestCase te in validCases)
            {
                expr = new ExpandedBooleanExpression(te.inputExpression);
                CollectionAssert.AreEqual(expr.GetAllVariables(), te.variables);
            }
        }

        [TestMethod]
        public void TestValidExpandedExpressions()
        {
            foreach (ExpressionTestCase t in validCases)
            {
                expr = new ExpandedBooleanExpression(t.inputExpression);
                Assert.AreEqual(t.expectedExpression, expr.ToString());
            }
        }
        
        [TestMethod]
        public void TestInvalidExpandedExpressions()
        {
            foreach (InvalidExpressionTestCase t in invalidCases)
            {
                try
                {
                    expr = new ExpandedBooleanExpression(t.inputExpression);
                }
                catch (Exception e)
                {
                    StringAssert.Contains(e.Message, t.expectedExceptionMessage);
                }
            }
        }
    }
}
