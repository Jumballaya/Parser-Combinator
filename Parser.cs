using System;

namespace Calc
{
    public class Parser<T,A>
    {
        public Func<State<T>, State<A>> tformFn;
        public Parser(Func<State<T>, State<A>> tformFn)
        {
            this.tformFn = tformFn;
        }

        // Run from target string
        public State<A> Run(string target)
        {
            State<T> s = new State<T>(target);
            State<A> nextState = this.tformFn(s);
            return nextState;
        }

        // Run from previous state
        public State<A> Run(State<T> state)
        {
            State<A> nextState = this.tformFn(state);
            return nextState;
        }

        public Parser<T,B> Map<B>(Func<A,B> fn)
        {
            Func<State<T>,State<B>> newTformFn = (State<T> state) => {
                State<A> nextState = this.Run(state);
                if (nextState.isError) {
                    State<B> finalError = new State<B>(nextState.target, nextState.index, default(B), nextState.error, nextState.isError);
                    return finalError;
                }
                State<B> finalState = new State<B>(nextState.target, nextState.index, fn(nextState.result), nextState.error, nextState.isError);
                return finalState;
            };
            return new Parser<T,B>(newTformFn);
        }

        public Parser<T,B> Chain<B>(Func<A, Parser<A,B>> fn)
        {
            Func<State<T>, State<B>> newTformFn = (State<T> state) => {
                State<A> nextState = this.tformFn(state);
                if (nextState.isError) {
                    return new State<B>(nextState.target, nextState.index, default(B), nextState.error, nextState.isError);
                }
                Parser<A,B> nextParser = fn(nextState.result);
                return nextParser.tformFn(nextState);
            };
            return new Parser<T,B>(newTformFn);
        }
    }
}