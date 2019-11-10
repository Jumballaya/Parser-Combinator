using System;

namespace Calc
{
    public static class Calculator
    {
        public static void Evaluate(string input)
        {
            Parser<string, int> parser = Util.StringParser("10").Map((string i) => int.Parse(i));

            State<int> res = parser.Run(input);
            if (res.isError) {
                Console.WriteLine("Error: " + res.error);
            } else {
                Console.WriteLine("Answer: " + res.result);
            }
        }
    }
}