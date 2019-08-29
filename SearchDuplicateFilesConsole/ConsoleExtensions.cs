using System;

namespace SearchDuplicateFilesConsole
{
	public static class ConsoleExtensions
	{
		public static void WriteInColor<T>(T value, ConsoleColor foregroundColor)
		{
			var previousForegroundColor = Console.ForegroundColor;
			Console.ForegroundColor = foregroundColor;
			Console.Write(value);
			Console.ForegroundColor = previousForegroundColor;
		}
	}
}