# bisectionSolver

This repository is a code dump from a school project.

The main goal was to implement the bisection method for finding roots of functions. It also includes a custom Pratt parser to handle math expressions written as text.

## Project context

- Built for an academic assignment.
- Shared mainly as a reference.
- Not intended as a polished production library.

## What it does

- Tokenizes math expressions.
- Parses expressions using a Pratt parser (operator precedence and associativity).
- Evaluates expressions for a given value of `x`.
- Runs the bisection method on an interval `[a, b]`.

## Main structure

- `Lexer.cs`: turns input text into tokens.
- `Parser.cs`: Pratt parser for expression parsing.
- `Interpreter.cs`: evaluates parsed expressions.
- `Solver.cs`: bisection method implementation.
- `Program.cs`: entry point.
- `Utils.cs`: helper utilities.

## About the Pratt parser

Even though the core topic is bisection, a key part of the project is the Pratt parsing implementation. It makes expression handling cleaner and easier to extend than a very basic ad-hoc parser.

## Run

With .NET 8 installed:

```bash
dotnet run
```

If you want to build first:

```bash
dotnet build
dotnet run
```
> AI-assisted generated README, reviewed and edited by @alexwithstars.