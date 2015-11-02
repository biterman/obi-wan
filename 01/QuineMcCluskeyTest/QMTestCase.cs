namespace QuineMcCluskeyTest
{
    public struct ExpressionTestCase
    {
        public string inputExpression;
        public string expectedExpression;
        public int variableCount;
        public char[] variables;
        public ExpressionTestCase(string input, string expected, string variables)
        {
            inputExpression = input;
            expectedExpression = expected;
            this.variables = variables.ToCharArray();
            variableCount = variables.Length;
        }
    }

    public struct InvalidExpressionTestCase
    {
        public string inputExpression;
        public string expectedExceptionMessage;
        public InvalidExpressionTestCase(string input, string expected)
        {
            inputExpression = input;
            expectedExceptionMessage = expected;
        }
    }
}