using System;
using System.Collections.Generic;

namespace QuineMcCluskey
{
    public class QuineMcCluskeySolver
    {
        public static void Main(string[] args)
        {
            string input;
            Console.WriteLine("Novosit Obi-Wan Challenge One\nQuine McCluskey's Algorithm\n");
            Console.WriteLine("Type input boolean expression:");
            input = Console.ReadLine();
            try
            {
                Console.WriteLine(string.Format("Reduced expression:\n\t{0}",
                        ReducedBooleanExpression.solveQuineMcCluskey(input)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }

}
