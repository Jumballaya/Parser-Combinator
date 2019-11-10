using System;
using System.Collections.Generic;

namespace Calc {
    public class State<T> {
        public string target;
        public int index;
        public T result;
        public string error;
        public bool isError;

        // Create brand new state
        public State(string target)
        {
            this.target = target;
            this.index = 0;
            this.error = "";
            this.isError = false;
        }

        // Update the state
        public State(State<T> old, int index, T result)
        {
            this.index = index;
            this.result = result;
            this.target = old.target;
            this.error = old.error;
            this.isError = old.isError;
        }

        // Create Error State
        public State(State<T> old, string error)
        {
            this.index = old.index;
            this.result = old.result;
            this.target = old.target;
            this.error = error;
            this.isError = true;
        }

        // Create from individual values
        public State(string target, int index, T result, string error, bool isError)
        {
            this.target = target;
            this.index = index;
            this.result = result;
            this.error = error;
            this.isError = isError;
        }
    }
}