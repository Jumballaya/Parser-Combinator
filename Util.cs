using System;
using System.Collections.Generic;

namespace Calc
{
    static public class Util
    {

        // Parses out a given string from the state's target
        static public Parser<string, string> StringParser(string s)
        {
            Func<State<string>,State<string>> str = (State<string> state) => {
                if (state.index >= state.target.Length) {
                    return new State<string>(state, "Unexpected end of input on index " + state.index);
                }
                if (state.isError) {
                    return new State<string>(state.target, state.index, "", state.error, state.isError);
                }
                string sliced = state.target.Substring(state.index);
                if (sliced.Length == 0) {
                    string msg = "Tried to match " + s + " but got unexpected end of input.";
                    return new State<string>(state.target, state.index, "", msg, true);
                }
                if (sliced.StartsWith(s)) {
                    return new State<string>(state.target, state.index + s.Length, s, state.error, state.isError);
                }
                string errMsg = "Tried to match '" + s + "' but got '" + state.target.Substring(state.index) + "'.";
                return new State<string>(state.target, state.index, "", errMsg, true);
            };
            return new Parser<string,string>(str);
        }

        // Parses in order of parser list
        static public Parser<T,List<A>> SequenceOf<T,A>(List<Parser<T,A>> parsers)
        {
            Func<State<T>,State<List<A>>> tfn = (State<T> state) => {
                if (state.isError) {
                    return new State<List<A>>(state.target, state.index, new List<A>(), state.error, state.isError);
                }
                List<A> res = new List<A>();
                State<A> nextState = parsers[0].Run(state);
                res.Add(nextState.result);
                parsers.RemoveAt(0);

                foreach (var p in parsers)
                {
                    State<T> s = new State<T>(nextState.target, nextState.index, state.result, nextState.error, nextState.isError);
                    nextState = p.Run(s);
                    res.Add(nextState.result);
                }

                if (nextState.isError) {
                    List<A> ret = new List<A>();
                    ret.Add(nextState.result);
                    return new State<List<A>>(nextState.target, nextState.index, ret, nextState.error, nextState.isError);
                }

                return new State<List<A>>(nextState.target, nextState.index, res, nextState.error, nextState.isError);
            };

            return new Parser<T,List<A>>(tfn);
        }

        // Parses using the first successful parser in the list
        static public Parser<T,A> Choice<T,A>(List<Parser<T,A>> parsers)
        {
            Func<State<T>,State<A>> tfn = (State<T> state) => {
                if (state.isError) {
                    return new State<A>(state.target, state.index, default(A), state.error, state.isError);
                }

                foreach (Parser<T,A> p in parsers)
                {
                    State<A> nextState = p.tformFn(state);
                    if (!nextState.isError) {
                        return nextState;
                    }
                }

                return new State<A>(state.target, state.index, default(A), "Unable to match with any parser at index: " + state.index, true);
            };

            return new Parser<T,A>(tfn);
        }

        // Parses as many of the given parser as possible
        static public Parser<T,List<A>> Many<T,A>(Parser<T,A> p)
        {
            Func<State<T>,State<List<A>>> tfn = (State<T> state) => {
                if (state.isError) {
                    return new State<List<A>>(state.target, state.index, new List<A>(), state.error, state.isError);
                }
                List<A> res = new List<A>();
                State<T> nextState = state;
                State<A> testState = default(State<A>);

                while (true)
                {
                    testState = p.tformFn(nextState);
                    if (!testState.isError) {
                        nextState = new State<T>(testState.target, testState.index, nextState.result, testState.error, testState.isError);
                        res.Add(testState.result);
                    } else {
                        break;
                    }
                }

                return new State<List<A>>(nextState.target, nextState.index, res, nextState.error, nextState.isError);
            };

            return new Parser<T,List<A>>(tfn);
        }

        // Parses a parser between left and right parsed values
        static public Func<Parser<T,A>,Parser<T,List<A>>> Between<T,A>(Parser<T,A> left, Parser<T,A> right)
        {
            Func<List<A>, List<A>> trim = (List<A> res) =>
            {
                res.RemoveAt(0);
                res.RemoveAt(res.Count - 1);
                return res;
            };
            return (Parser<T,A> content) => {
                List<Parser<T,A>> parsers = new List<Parser<T,A>>();
                parsers.Add(left);
                parsers.Add(content);
                parsers.Add(right);
                return SequenceOf(parsers).Map(trim);
            };
        }
    }
}