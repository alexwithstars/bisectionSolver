namespace bisectionSolver;

class Solver {
	class Iteration {
		public readonly double Xi;
		public readonly double Xs;
		public readonly double Xm;
		public readonly double F_Xi;
		public readonly double F_Xs;
		public readonly double F_Xm;
		public readonly double E;

		public Iteration(double xi, double xs, double xa, Func<double, double> F) {
			Xi = xi;
			Xs = xs;
			Xm = (xi + xs) / 2;
			F_Xi = F(Xi);
			F_Xs = F(Xs);
			F_Xm = F(Xm);
			if (Xm < 1) {
				E = Math.Abs(Xm - xa);
			} else if (xa == double.PositiveInfinity) {
				E = double.PositiveInfinity;
			} else {
				E = Math.Abs((Xm - xa) / Xm);
			}
		}
	}

	readonly Tuple<double, double> interval;
	readonly double tolerance;
	readonly Func<double, double> F;
	public double solution = 0;
	readonly List<Iteration> iterations = [];


	public Solver(Tuple<double, double> interval, double tolerance, Func<double, double> F) {
		this.interval = interval;
		this.tolerance = tolerance;
		this.F = F;
		Solve();
	}

	void Solve() {
		iterations.Clear();
		double xi = interval.Item1;
		double xs = interval.Item2;
		double xa = double.PositiveInfinity;
		do {
			Iteration iteration = new(xi, xs, xa, F);
			iterations.Add(iteration);
			if (iteration.F_Xi * iteration.F_Xm < 0) {
				xs = iteration.Xm;
			} else {
				xi = iteration.Xm;
			}
			xa = iteration.Xm;
		} while (iterations.Last().E > tolerance && iterations.Last().F_Xm != 0);
		solution = iterations.Last().Xm;
	}

	public void PrintIterations() {
		int[] columnWidths = new int[7];
		foreach (var iteration in iterations) {
			columnWidths[0] = Math.Max(columnWidths[0], iteration.Xi.ToString().Length);
			columnWidths[1] = Math.Max(columnWidths[1], iteration.Xs.ToString().Length);
			columnWidths[2] = Math.Max(columnWidths[2], iteration.Xm.ToString().Length);
			columnWidths[3] = Math.Max(columnWidths[3], iteration.F_Xi.ToString().Length);
			columnWidths[4] = Math.Max(columnWidths[5], (iteration.F_Xm * iteration.F_Xi).ToString().Length);
			columnWidths[5] = Math.Max(columnWidths[4], iteration.F_Xm.ToString().Length);
			columnWidths[6] = Math.Max(columnWidths[6], (iteration.E * 100).ToString().Length + 1); // +1 for the '%' symbol
		}
		Utils.SetTextColor("32"); // Green
		Utils.MultiPrintLine(
			$"{"Xi".PadRight(columnWidths[0])}\t",
			$"{"Xs".PadRight(columnWidths[1])}\t",
			$"{"Xm".PadRight(columnWidths[2])}\t",
			// Temporarily removed columns for F(Xi), F(Xm) and their product to save space
			// $"{"F(Xi)".PadRight(columnWidths[3])}\t",
			// $"{"F(Xm)".PadRight(columnWidths[4])}\t",
			// $"{"F(Xi) * F(Xm)".PadRight(columnWidths[5])}\t",
			$"{"E (%)".PadRight(columnWidths[6])}"
		);
		Utils.SetTextColor("0"); // Reset
		foreach (var iteration in iterations) {
			Utils.MultiPrintLine(
				$"{iteration.Xi.ToString().PadRight(columnWidths[0])}\t",
				$"{iteration.Xs.ToString().PadRight(columnWidths[1])}\t",
				$"{iteration.Xm.ToString().PadRight(columnWidths[2])}\t",
				// Temporarily removed columns for F(Xi), F(Xm) and their product to save space
				// $"{iteration.F_Xi.ToString().PadRight(columnWidths[3])}\t",
				// $"{iteration.F_Xm.ToString().PadRight(columnWidths[4])}\t",
				// $"{(iteration.F_Xi * iteration.F_Xm).ToString().PadRight(columnWidths[5])}\t",
				$"{(iteration.E * 100).ToString()}%"
			);
		}
	}
}