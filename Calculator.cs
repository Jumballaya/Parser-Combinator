using System;
using System.Collections.Generic;

namespace Calc
{
    public class Calculator
    {
        public Parser<string, int> parser;

        public Calculator()
        {
            Func<string,List<string>> listify = (string s) => {
                var l = new List<string>();
                l.Add(s);
                return l;
            };

            var operators = new List<Parser<string,string>>();
            operators.Add(Util.StringParser("+"));
            operators.Add(Util.StringParser("-"));
            operators.Add(Util.StringParser("*"));
            operators.Add(Util.StringParser("/"));
            var operatorParser = Util.Choice(operators).Map(listify);

            var numberParser = Util.DigitParser().Map(listify);

            var betweenParens = Util.Between(Util.StringParser("("), Util.StringParser(")"));

            Parser<string, List<string>> operationParser;
            var expressionParser = Util.Lazy(() => {
                var ps = new List<Parser<string,List<string>>>();
                ps.Add(numberParser);
                ps.Add(operationParser);
                return Util.Choice(ps);
            });

            var mainSequenceItems = new List<Parser<string, List<string>>>();
            mainSequenceItems.Add(operatorParser);
            mainSequenceItems.Add(Util.StringParser(" ").Map(listify));
            mainSequenceItems.Add(expressionParser);
            mainSequenceItems.Add(Util.StringParser(" ").Map(listify));
            mainSequenceItems.Add(expressionParser);
            Parser<string, List<string>> mainSequence = Util.SequenceOf(mainSequenceItems);

            operationParser = betweenParens(mainSequence);

            this.parser = Util.StringParser("10").Map((string i) => int.Parse(i));
        }
        public void Evaluate(string input)
        {
            State<int> res = parser.Run(input);
            if (res.isError) {
                Console.WriteLine("Error: " + res.error);
            } else {
                Console.WriteLine("Answer: " + res.result);
            }
        }
    }
}