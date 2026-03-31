namespace bisectionSolver;

class Program {
	static void Repl() {
		Console.WriteLine("Welcome to the Bisection Method Solver!");
		Console.WriteLine("Type 'exit' or 'q' to quit.");
		while (true) {
			string input = Utils.GetInput("Insert Math Function to start\n>> ");
			if (input.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) break;
			else if (input.Equals("q", StringComparison.CurrentCultureIgnoreCase)) break;
			try {
				Func<double, double> F = Interpreter.Transform(input);
				string[]? inputs = Utils.MultipleInputs([
					"Li",
					"Ls",
					"Tolerance"
				]);
				if (inputs == null) {
					continue;
				}
				Solver solver = new(
					Tuple.Create(double.Parse(inputs[0]), double.Parse(inputs[1])),
					double.Parse(inputs[2]),
					F
				);
				Console.Clear();
				Console.WriteLine($"Function: {input}");
				Console.WriteLine($"Interval: [{inputs[0]}, {inputs[1]}]");
				Console.WriteLine($"Tolerance: {inputs[2]}%\n");
				Console.WriteLine("Iterations:");
				solver.PrintIterations();
				Console.WriteLine($"\nSolution: {solver.solution}\n");
				Utils.WaitForEnter();
				Console.Clear();
			} catch (Exception ex) {
				Console.WriteLine("Error: " + ex.Message);
			}
		}
		Utils.FullClear();
		Console.WriteLine("Goodbye!");
	}

	static void SmplRepl() {
		Console.WriteLine("Math solver");
		Console.WriteLine("Type 'exit' or 'q' to quit.");
		while (true) {
			string input = Utils.GetInput(">> ");
			if (input.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) break;
			else if (input.Equals("q", StringComparison.CurrentCultureIgnoreCase)) break;
			try {
				Console.WriteLine(Interpreter.Evaluate(input));
			} catch (Exception ex) {
				Console.WriteLine("Error: " + ex.Message);
			}
		}
		Utils.FullClear();
		Console.WriteLine("Goodbye!");
	}

	static void Main() {
		Console.WriteLine("Select mode:");
		int option = Utils.ShowListMenu([
			"Bisection Method Solver",
			"Simple Expression Evaluator",
			"Exit"
		]);
		Utils.FullClear();
		switch (option) {
			case 0:
				Repl();
				break;
			case 1:
				SmplRepl();
				break;
			case 2:
			default:
				Utils.Exit();
				break;
		}
	}
}