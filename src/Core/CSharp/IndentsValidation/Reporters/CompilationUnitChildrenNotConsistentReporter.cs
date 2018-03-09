using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace uLearn.CSharp.IndentsValidation.Reporters
{
	internal static class CompilationUnitChildrenNotConsistentReporter
	{
		public static IEnumerable<string> Report(SyntaxTree userSolution)
		{
			var childLineIndents = userSolution.GetRoot().ChildNodes()
				.Select(node => node.GetFirstToken())
				.Select(t => new Indent(t))
				.Where(i => i.IndentedTokenIsFirstAtLine)
				.ToList();
			if (!childLineIndents.Any())
			{
				return Enumerable.Empty<string>();
			}
			var firstIndent = childLineIndents.First();
			return childLineIndents
				.Skip(1)
				.Where(i => i.LengthInSpaces != firstIndent.LengthInSpaces)
				.Select(i => BaseStyleValidator.Report(i.IndentedToken, "На верхнем уровне вложенности все узлы должны иметь одинаковый отступ"));
		}
	}
}