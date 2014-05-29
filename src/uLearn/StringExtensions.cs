using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace uLearn
{
	public static class StringExtensions
	{
		public static string[] SplitToLines(this string text)
		{
			return Regex.Split(text, @"\r?\n");
		}

		public static string RemoveCommonNesting(this string text)
		{
			var lines = text.SplitToLines();
			var newLines = lines.RemoveCommonNesting();
			return string.Join("\r\n", newLines);
		}

		public static IEnumerable<string> RemoveCommonNesting(this string[] lines)
		{
			var nesting =
				lines.Where(line => line.Trim().Length > 0).DefaultIfEmpty().Min(line => line.TakeWhile(char.IsWhiteSpace).Count());
			var newLines = lines.Select(line => line.Length > nesting ? line.Substring(nesting) : line);
			return newLines;
		}
	}
}