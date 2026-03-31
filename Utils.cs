namespace bisectionSolver;

record Pos(int Left, int Top);

class Utils {
	public static string TextColor(string color) {
		return $"\x1b[{color}m";
	}

	public static void SetTextColor(string color) {
		Console.Write(TextColor(color));
	}

	public static string GetInput(string prompt){
		Console.Write(prompt);
		return Console.ReadLine() ?? "";
	}

	public static Pos SaveCursor() {
		var (left, top) = Console.GetCursorPosition();
		return new Pos(left, top);
	}
	public static void RestoreCursor(Pos coords) {
		Console.SetCursorPosition(coords.Left, coords.Top);
	}
	public static void ClearBeforeCursor() {
		Console.Write("\x1b[1J");
	}
	public static void ClearAfterCursor() {
		Console.Write("\x1b[0J");
	}
	public static void ClearOutside() {
		Console.Write("\x1b[3J");
	}
	public static void FullClear() {
		Console.Clear();
		ClearOutside();
	}

	public static void WaitForEnter() {
		Console.CursorVisible = false;
		bool done = false;
		Console.WriteLine("Press ↲ to continue...");
		while(!done) {
			ConsoleKeyInfo info = Console.ReadKey(intercept:true);
			if(info.Key == ConsoleKey.Enter) {
				done = true;
			}
		}
		Console.CursorVisible = true;
	}

	public static bool? UpDownSelect(ref int indexOption, int maxOption) {
		Console.WriteLine("↑↓ to navigate and ↲ to select");
		ConsoleKeyInfo info = Console.ReadKey(intercept:true);
		if(info.Key == ConsoleKey.UpArrow) {
			if(indexOption <= 0) indexOption=maxOption;
			else indexOption--;
		}
		if(info.Key == ConsoleKey.DownArrow) {
			if(indexOption >= maxOption) indexOption=0;
			else indexOption++;
		}
		if(info.Key == ConsoleKey.Escape) return null;
		return info.Key == ConsoleKey.Enter;
	}

	public static bool? FourDirectionSelect
	(ref int indexOptionX, int maxOptionX, ref int indexOptionY, int maxOptionY) {
		Console.WriteLine("←→↑↓ to navigate and ↲ to select");
		ConsoleKeyInfo info = Console.ReadKey(intercept:true);
		if(info.Key == ConsoleKey.LeftArrow) {
			if(indexOptionX <= 0) indexOptionX=maxOptionX;
			else indexOptionX--;
		}
		if(info.Key == ConsoleKey.RightArrow) {
			if(indexOptionX >= maxOptionX) indexOptionX=0;
			else indexOptionX++;
		}
		if(info.Key == ConsoleKey.UpArrow) {
			if(indexOptionY <= 0) indexOptionY=maxOptionY;
			else indexOptionY--;
		}
		if(info.Key == ConsoleKey.DownArrow) {
			if(indexOptionY >= maxOptionY) indexOptionY=0;
			else indexOptionY++;
		}
		if(info.Key == ConsoleKey.Escape) return null;
		return info.Key == ConsoleKey.Enter;
	}

	public static int ShowListMenu(string[] options, string accent = "44") {
		bool done = false;
		int indexOption = 0;
		Console.CursorVisible = false;
		Pos saveCoords = SaveCursor();
		while(!done) {
			RestoreCursor(saveCoords);
			for(int i = 0; i < options.Length; i++) {
				bool selected = indexOption == i;
				Console.Write($" {(selected ? $"> \x1b[{accent}m" : "  ")}{options[i]}");
				Console.WriteLine("\x1b[0m");
			}
			Console.WriteLine();
			bool? res = UpDownSelect(ref indexOption, options.Length-1);
			done = res ?? true;
			if(res == null) {
				indexOption = -1;
			}
		}
		Console.CursorVisible = true;
		return indexOption;
	}

	public static void Exit() {
		Console.Clear();
		Console.WriteLine(" Bye :3");
		Environment.Exit(0);
	}

	public static string[]? MultipleInputs(string[] fields, string[]? defaultValues = null) {
		string[] buffers = new string[fields.Length];
		for(int i = 0; i < buffers.Length; i++) {
			buffers[i] = defaultValues?[i] ?? "";
		}
		int index = 0;

		Pos saveCoords = SaveCursor();
		while(true) {
			Console.CursorVisible = false;
			RestoreCursor(saveCoords);
			ClearAfterCursor();

			for(int i = 0; i <= index; i++) {
				Console.Write($"\n{fields[i]}: {buffers[i]}");
			}
			var (left, top) = Console.GetCursorPosition();
			for(int i = index+1; i < fields.Length; i++) {
				Console.Write($"\n{fields[i]}: {buffers[i]}");
			}
			
			Console.WriteLine(
				"\n\nUse ↑↓ to navigate between fields\n"+
				"↲ to next/save, ← to delete\n"+
				"esc to cancel"
			);

			Console.SetCursorPosition(left, top);
			Console.CursorVisible = true;

			ConsoleKeyInfo info = Console.ReadKey(intercept:true);
			if(info.Key == ConsoleKey.Enter) {
				if(index == fields.Length - 1) {
					break;
				} else index++;
			} else if(info.Key == ConsoleKey.Escape) {
				return null;
			} else if(info.Key == ConsoleKey.Backspace) {
				if(buffers[index].Length > 0) {
					buffers[index] = buffers[index][..^1];
				}
			} else if(info.Key == ConsoleKey.UpArrow) {
				if(index > 0) index--;
			} else if(info.Key == ConsoleKey.DownArrow) {
				if(index < fields.Length - 1) index++;
			} else if(!char.IsControl(info.KeyChar)) {
				buffers[index] += info.KeyChar;
			}
		}
		return buffers;
	}

	public static void MultiPrint(params string[] args) {
		foreach (var arg in args) {
			Console.Write(arg);
		}
	}

	public static void MultiPrintLine(params string[] args) {
		MultiPrint(args);
		Console.WriteLine();
	}

	public static bool Confirm(string message) {
		Console.WriteLine(message);
		int res = ShowListMenu(["Yes", "No"]);
		return res == 0;
	}
}