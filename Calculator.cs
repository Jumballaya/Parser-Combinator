using System.Collections.Generic;

namespace Calc
{
    public static class Calculator
    {
        static int Evaluate(string input)
        {
            Parser<string, int> parser = Util.StringParser("10").Map((string i) => int.Parse(i));

            
            return parser.Run(input).result;
        }
    }
}