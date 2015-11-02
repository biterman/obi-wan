## C# Solution for the Quine McCluskey challenge ##

The solution implemented here consists of two phases:

1. Arbitrary boolean expression parsing and expansion into disjunctive normal form (DNF or sum of products). This is handled mainly by the class ExpandedBooleanExpression. It uses a recursive descent approach to parse the expression and build an abstract syntax tree (AST). It then applies DeMorgan laws and distributive properties of operators to transform the AST into one that represents an expression in DNF. The grammar parsed is:

```
expression = term [ + term ]
term = factor [ factor ]
factor = literal['] | (expression) [']
literal = A | ... | Z
```

2. Use of QuineMcCluskey method to reduce the expanded expression. This is handled by ReducedBooleanExpression which takes the ExpandedBooleanExpression as input. Minimal expressions are not guaranteed.

ReducedBooleanExpression implements the required solveQuineMcCluskey method statically.

The QuineMcCluskeySolver class is simply a console app that reads a line from stdin as input and prints the output of the solveQuineMcCluskey method.

** Notes: **

- I doubt the style is very idiomatic as this was written while picking up C#, VisualStudio and the recursive descent parsing technique.
- I have assumed that since this builds with the stock VisualStudio 2015 it should build with the required MSBuild Tool.
- Most of the heavy lifting is done by code with no public interface and thus they are covered only indirectly by the unit tests.
