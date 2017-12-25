using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class VerbInMethodNameValidator : BaseStyleValidator
	{
		private readonly HashSet<string> englishVerbs;

		public VerbInMethodNameValidator()
		{
			var lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "uLearn\\Csharp\\VerbInMethodNameValidation\\31K verbs.txt"));

			var nameComparer = new NameIgnoreCaseComparer();
			exceptionsMethodNames = new HashSet<string>(new[] { "Main" }, nameComparer);
			exceptionsPreposition = new HashSet<string>(new[] { "For", "To", "With", "From", "At" }, nameComparer);
			englishVerbs = new HashSet<string>(lines, nameComparer);
		}

		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return InspectAll<MethodDeclarationSyntax>(userSolution, InspectMethodsNames);
		}

		private IEnumerable<string> InspectMethodsNames(MethodDeclarationSyntax methodDeclarationSyntax)
		{
			var syntaxToken = methodDeclarationSyntax.Identifier();
			if (exceptionsMethodNames.Contains(syntaxToken.ValueText))
				yield break;

			var wordsInName = SplitMethodName(syntaxToken.ValueText).ToList();

			if (exceptionsPreposition.Contains(wordsInName.First()))
				yield break;

			foreach (var word in wordsInName)
			{
				if (englishVerbs.Contains(word))
					yield break;
			}

			yield return Report(syntaxToken, "В названии метода отсутствует глагол");
		}

		private IEnumerable<string> SplitMethodName(string methodName)
		{
			var word = "";
			foreach (var letter in methodName)
			{
				if (char.IsUpper(letter) && word != "")
				{
					yield return word;
					word = "";
				}
				word += letter;
			}
			yield return word;
		}

		private readonly HashSet<string> exceptionsMethodNames;
		private readonly HashSet<string> exceptionsPreposition;
	}
}