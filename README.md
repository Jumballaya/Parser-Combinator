# Parser Combinator
Generate text-parsers by composing parser functions together. The Parser class acts as a Monad, holding the transformed values as you go.

Builtin Utils in the Util static class like:

1. StringParser -- Parse a given string from a target string
2. SequenceOf -- Give it a List of Parsers and it will parse them in order
3. Choice -- Give it a List of Parsers and it will use the first one that works
4. Many -- It will parse as many of a given parser as possible
5. Between -- Given 2 parsers, it will give back a function that takes a third parser and will parse the third values from between the first two