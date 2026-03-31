namespace bisectionSolver;

class Interpreter {
	enum OptimizationResult {
		Optimized,
		Unchanged
	}

	public static Func<double, double> Transform(Parser.Expression expression) {
		Parser.Expression optimizedExpr = expression;
		Optimize(ref optimizedExpr);
		return x => Evaluate(optimizedExpr, x);
	}

	static OptimizationResult Optimize(ref Parser.Expression expression) {
		switch (expression) {
			case Parser.Atom atom:
				return atom.Token.Type switch {
					Lexer.TokenType.Literal => OptimizationResult.Optimized,
					Lexer.TokenType.Variable => OptimizationResult.Unchanged,
					_ => throw new InvalidOperationException($"Unexpected token type: {atom.Token.Type}")
				};
			case Parser.BinaryOperation binOp:
				var left = Optimize(ref binOp.Left);
				var right = Optimize(ref binOp.Right);
				if (left == OptimizationResult.Optimized && right == OptimizationResult.Optimized) {
					expression = new Parser.Atom(new Lexer.Token(
						Lexer.TokenType.Literal,
						Evaluate(binOp).ToString()
					));
					return OptimizationResult.Optimized;
				}
				return OptimizationResult.Unchanged;
			case Parser.UnaryOperation unOp:
				var operand = Optimize(ref unOp.Operand);
				if (operand == OptimizationResult.Optimized) {
					expression = new Parser.Atom(new Lexer.Token(
						Lexer.TokenType.Literal,
						Evaluate(unOp).ToString()
					));
					return OptimizationResult.Optimized;
				}
				return OptimizationResult.Unchanged;
			default:
				throw new InvalidOperationException("Unknown expression type");
		}
	}

	public static double Evaluate(Parser.Expression expression, double x = 0) {
		switch (expression) {
			case Parser.Atom atom:
				return atom.Token.Type switch {
					Lexer.TokenType.Literal => double.Parse(atom.Token.Value),
					Lexer.TokenType.Variable => x,
					_ => throw new InvalidOperationException($"Unexpected token type: {atom.Token.Type}")
				};
			case Parser.BinaryOperation binOp:
				double left = Evaluate(binOp.Left, x);
				double right = Evaluate(binOp.Right, x);
				return binOp.Op switch {
					"+" => left + right,
					"-" => left - right,
					"*" => left * right,
					"/" => left / right,
					"^" => Math.Pow(left, right),
					_ => throw new InvalidOperationException($"Unknown operator: {binOp.Op}")
				};
			case Parser.UnaryOperation unOp:
				double operand = Evaluate(unOp.Operand, x);
				return unOp.Op switch {
					"+" => operand,
					"-" => -operand,
					_ => throw new InvalidOperationException($"Unknown operator: {unOp.Op}")
				};
			default:
				throw new InvalidOperationException("Unknown expression type");
		}
	}

	// [Overloads]
	public static double Evaluate(string input) {
		return Evaluate(Parser.Parse(input));
	}
	public static double Evaluate(Parser.Expression expression) {
		return Evaluate(expression, 0);
	}

	public static Func<double, double> Transform(string input) {
		return Transform(Parser.Parse(input));
	}
}