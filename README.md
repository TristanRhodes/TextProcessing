# TextProcessing

#### Project Build Status: ![Project Build Status](https://github.com/TristanRhodes/TextProcessing/actions/workflows/dotnet.yml/badge.svg)

#### Running Solution

> git clone https://github.com/TristanRhodes/TextProcessing.git
> 
> cd TextProcessing
> 
> dotnet test

### Multi Language Text Processing

This is a small research project to go along with a series of blog posts. It covers Regex, two phase parsing (Lexing/Tokenising and Expression Parsing), combinatorial parsers, scannerless parsing and using [Sprache](https://github.com/sprache/Sprache). It is written in C# .Net 5.0 with the end result being a simple two phase, multi language expression parser.

<p align="center">
	<img src="https://github.com/TristanRhodes/TextProcessing/blob/master/assets/multi-language-parsing.png?raw=true" alt="Multi Language Parsing" width="80%"/>
</p>

It was started as a small research project to go along with a series of blog posts, and is a playground for Lexing / Parsing and text processing in general.

## Blog Posts

### [Regex Matching](https://tristan-rhodes.co.uk/2021/12/11/regex-redemption.html)
Using regex to match different string formats of DayOfWeek and ClockTime.

### [Tokenisation](https://tristan-rhodes.co.uk/2021/12/19/tokenisation.html)
Breaking a longer DayTime string into recognized Day and LocalTime parts.

### [Expression Parsing](https://tristan-rhodes.co.uk/2022/01/07/expression-parsing.html)
Use a basic suite of Object Orientated IParser<T> implementations to parse a DayTime string and a number of other simple expressions.

```csharp
// Separate two part element context => DayTime range
"Pickup Mon 08:00 dropoff wed 17:00"

// Range elements with different separators => Open Days range and Hours Range
"Open Mon to Fri 08:00 - 18:00"

// Repeating tokens => List of tour times
"Tours 10:00 12:00 14:00 17:00 20:00"

// Repeating complex elements => List of event day times
"Events Tuesday 18:00 Wednesday 15:00 Friday 12:00"
```

### [Functional Expression Parsing](https://tristan-rhodes.co.uk/2022/01/16/expression-parsing-monads.html)
Replace all IParser<T> interfaces with Delegates and go all in on functional combinators.

### [Scannerless Parsing](https://tristan-rhodes.co.uk/2022/01/25/scannerless-parsers.html)
Instead of using an array of pre-parsed Tokens we're going to use the string/Char[] array directly and implement our parser in [Sprache](https://github.com/sprache/Sprache). 

## Parser Implementations

### [Object Orientated](https://github.com/TristanRhodes/TextProcessing/tree/master/TextProcessing/OO)
A simple tokeniser and parser system implemented using Interfaces and an Object Orientated style.

### [Functional](https://github.com/TristanRhodes/TextProcessing/tree/master/TextProcessing/Functional)
A simple tokeniser and parser system implemented using Delegates and monads in a functional style.

### [Scannerless in Sprache](https://github.com/TristanRhodes/TextProcessing/tree/master/TextProcessing/SpracheParsers)
An implementation of the demo parsers written in [Sprache](https://github.com/sprache/Sprache), the scannerless C# functional parsing library.