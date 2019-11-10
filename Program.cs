using System;
using System.Collections.Generic;

namespace Calc
{
    class Program
    {
        static void Main(string[] args)
        {       
            // Console.WriteLine("\nExample 1:\n");
            // ExampleOne();
            // Console.WriteLine("\nExample 2:\n");
            // ExampleTwo();
            // Console.WriteLine("\nExample 3:\n");
            // ExampleThree();
            Calculator.Evaluate("10");
        }

        static void ExampleOne()
        {
            Func<List<string>, List<string>> exclaimer = (List<string> res) =>
            {
                for (var i = 0; i < res.Count; i++)
                {
                    res[i] = res[i] + "!!!";
                }
                return res;
            };

            List<Parser<string,string>> puncList = new List<Parser<string,string>>();
            puncList.Add(Util.StringParser("!"));
            puncList.Add(Util.StringParser("."));
            puncList.Add(Util.StringParser("-"));
            puncList.Add(Util.StringParser("?"));

            Parser<string, string> punctuation = Util.Choice(puncList);

            List<Parser<string, string>> parsers = new List<Parser<string, string>>();
            parsers.Add(Util.StringParser("Hello"));
            parsers.Add(Util.StringParser(" "));
            parsers.Add(Util.StringParser("World"));
            parsers.Add(punctuation);

            Parser<string, List<string>> p = Util.SequenceOf(parsers).Map(exclaimer);
            State<List<string>> res = p.Run("Hello World.");

            PrintResult(res);
        }

        static void ExampleTwo()
        {
            Func<List<string>, List<string>> exclaimer = (List<string> res) =>
            {
                for (var i = 0; i < res.Count; i++)
                {
                    res[i] = res[i] + "!!!";
                }
                return res;
            };

            List<Parser<string,string>> helloAndSpace = new List<Parser<string,string>>();
            helloAndSpace.Add(Util.StringParser("Hello"));
            helloAndSpace.Add(Util.StringParser(" "));
            Parser<string, string> helloOrSpace = Util.Choice(helloAndSpace);

            State<List<string>> res = Util.Many(helloOrSpace).Map(exclaimer).Run("Hello Hello Hello Hello");

            PrintResult(res);
        }

        static void ExampleThree()
        {
            Func<List<string>, List<string>> exclaimer = (List<string> res) =>
            {
                for (var i = 0; i < res.Count; i++)
                {
                    res[i] = res[i] + "!!!";
                }
                return res;
            };
            Parser<string,string> leftParen = Util.StringParser("(");
            Parser<string,string> rightParen = Util.StringParser(")");

            State<List<string>> res = Util.Between(leftParen, rightParen)(Util.StringParser("Hello")).Run("(Hello)");

            PrintResult(res);
        }

        static void PrintResult(State<List<string>> res)
        {
            if (res.isError) {
                Console.WriteLine(res.error);
            } else {
                Console.WriteLine(res.result);
                foreach (var str in res.result)
                {
                    Console.WriteLine(str);
                }
            }
        }
    }
}
