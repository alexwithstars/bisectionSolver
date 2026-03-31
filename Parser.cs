namespace bisectionSolver;

// pratt parser implementation for parsing user inputted functions
class Parser {
	public enum ExpressionType {
		Atom,
		BinaryOperation,
		UnaryOperation
	}

	public class Expression(ExpressionType type) {
		public readonly ExpressionType Type = type;
	}
	public class Atom(Lexer.Token token): Expression(ExpressionType.Atom) {
		public readonly Lexer.Token Token = token;
	}
	public class BinaryOperation(
		string op, Expression left, Expression right
	): Expression(ExpressionType.BinaryOperation) {
		public readonly string Op = op;
		public Expression Left = left;
		public Expression Right = right;
	}
	public class UnaryOperation(
		string op, Expression operand
	): Expression(ExpressionType.UnaryOperation) {
		public readonly string Op = op;
		public Expression Operand = operand;
	}

	public enum BindingPower {
		Default,
		Additive,
		Multiplicative,
		Exponential,
		Unary
	}

	// true for ltr
	// false for rtl
	static bool IsLeftAssociative(BindingPower op) => op switch {
		BindingPower.Additive => true,
		BindingPower.Multiplicative => true,
		BindingPower.Exponential => false,
		_ => throw new InvalidOperationException($"Unknown operator: {op}")
	};

	static BindingPower GetBindingPower(string op) => op switch {
		"+" => BindingPower.Additive,
		"-" => BindingPower.Additive,
		"*" => BindingPower.Multiplicative,
		"/" => BindingPower.Multiplicative,
		"^" => BindingPower.Exponential,
		"(" => BindingPower.Default,
		_ => throw new InvalidOperationException($"Unknown operator: {op}")
	};

	public static Expression Parse(string input) {
		Lexer lexer = new(input);
		return Parse(lexer);
	}

	public static Expression Parse(Lexer lexer) {
		Expression expr = ParseExp(lexer, BindingPower.Default);
		if (lexer.HasTokensLeft) {
			throw new InvalidOperationException($"Unexpected token: {lexer.Peek().Value}");
		}
		return expr;
	}

	static Expression ParseExp(
		Lexer lexer, BindingPower minBound = BindingPower.Default
	) {
		if (!lexer.HasTokensLeft) {
			throw new InvalidOperationException($"Unexpected end of input");
		}

		Expression left;
		var token = lexer.Advance();
		switch (token.Type) {
			case Lexer.TokenType.Literal:
			case Lexer.TokenType.Variable:
				left = new Atom(token);
				break;
			case Lexer.TokenType.Operator:
				if (lexer.Peek().Type == Lexer.TokenType.EOF) {
					throw new InvalidOperationException("Expected expression after operator");
				}
				switch (token.Value) {
					case "+":
					case "-":
						Expression operand = ParseExp(lexer, BindingPower.Unary);
						left = new UnaryOperation(token.Value, operand);
						break;
					case "(":
						Expression expr = ParseExp(lexer);
						Lexer.Token currentToken = lexer.Advance();
						if (currentToken.Type != Lexer.TokenType.Operator || currentToken.Value != ")") {
							throw new InvalidOperationException("Expected ')'");
						}
						left = expr;
						break;
					default:
						throw new InvalidOperationException($"Unexpected operator: {token.Value}");
				}
				break;
			default:
				throw new InvalidOperationException($"Unexpected token: {token.Value}");
		}

		while (lexer.HasTokensLeft) {
			Lexer.Token opToken = lexer.Peek();
			if (opToken.Type != Lexer.TokenType.Operator) {
				throw new InvalidOperationException($"Expected operator, got: {opToken.Value}");
			}
			string op = opToken.Value;
			if (op == ")") {
				break;
			}
			BindingPower bp = GetBindingPower(op);
			if (bp < minBound || (bp == minBound && IsLeftAssociative(bp))) break;
			lexer.Advance();
			Expression right = ParseExp(lexer, bp);
			left = new BinaryOperation(op, left, right);
		}

		return left;
	}

	public static string ExpressionToString(Expression expr) => expr switch {
		Atom atom => atom.Token.Value,
		BinaryOperation binOp => $"({ExpressionToString(binOp.Left)} {binOp.Op} {ExpressionToString(binOp.Right)})",
		UnaryOperation unOp => $"({unOp.Op} {ExpressionToString(unOp.Operand)})",
		_ => throw new InvalidOperationException("Unknown expression type")
	};

	public static string ExpressionToTree(Expression expr, int indent = 0) {
		return expr switch {
			Atom atom => new string(' ', indent) + atom.Token.Value,
			BinaryOperation binOp => new string(' ', indent) + binOp.Op + "\n" + ExpressionToTree(binOp.Left, indent + 2) + "\n" + ExpressionToTree(binOp.Right, indent + 2),
			UnaryOperation unOp => new string(' ', indent) + unOp.Op + "\n" + ExpressionToTree(unOp.Operand, indent + 2),
			_ => throw new InvalidOperationException("Unknown expression type")
		};
	}
}