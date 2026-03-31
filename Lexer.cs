namespace bisectionSolver;

using System.Text.RegularExpressions;

class Lexer {
	public enum TokenType {
		Literal,
		Operator,
		Variable,
		EOF
	}

	public readonly struct Token(TokenType type, string value) {
		public readonly TokenType Type = type;
		public readonly string Value = value;
	}

	readonly struct Pattern(TokenType type, string pattern) {
		public readonly TokenType Type = type;
		public readonly Regex Regex = new(pattern);
	}

	readonly static Pattern[] patterns = [
		new(TokenType.Operator, @"^[\+\-\*\/\^\(\)]"),
		new(TokenType.Literal, @"^(?:[+-]?\d+(?:\.\d+)?)"),
		new(TokenType.Variable, @"^x") // for now, only supports 'x' as variable
	];
	readonly static Token EOFTOKEN = new(TokenType.EOF, "");
	readonly static Regex withespaceRegex = new(@"\s+");


	readonly List<Token> tokens;
	int pos = 0;

	public Lexer(List<Token> tokens) {
		this.tokens = tokens;
	}

	public Lexer(string input) {
		tokens = Tokenize(input);
	}

	public bool HasTokensLeft => pos < tokens.Count;
	Token Current => HasTokensLeft ? tokens[pos] : EOFTOKEN;
	public void Reset() => pos = 0;
	public Token Peek() {
		return Current;
	}
	public Token Advance() {
		Token current = Current;
		if (current.Type != TokenType.EOF) pos++;
		return current;
	}

	public static List<Token> Tokenize(string input) {
		string cleanedInput = withespaceRegex.Replace(input, "");
		List<Token> tokens = [];

		while (cleanedInput.Length > 0) {
			bool matched = false;
			foreach (var pattern in patterns) {
				Match match = pattern.Regex.Match(cleanedInput);
				if (match.Success) {
					tokens.Add(new Token(pattern.Type, match.Value));
					cleanedInput = cleanedInput[match.Length..];
					matched = true;
					break;
				}
			}
			if (!matched) {
				throw new Exception($"\nUnexpected token at: \"...{cleanedInput}\"\nFull input: \"{input}\"\n");
			}
		}

		return tokens;
	}

	public static void Test(string input) {
		var tokens = Tokenize(input);
		Console.WriteLine($"Input: {input}");
		Console.WriteLine("Tokens:");
		int index = 0;
		foreach (var token in tokens) {
			Console.WriteLine($"  [{index}] {token.Type}: {token.Value}");
			index++;
		}
	}
}