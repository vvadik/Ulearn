using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CSharp
{
	[TestFixture]
	public class IndentsValidator_Should
	{
		private DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "CSharp", "IndentsValidation", "TestData"));
		private DirectoryInfo IncorrectTestDataDir => TestDataDir.GetDirectories("Incorrect").Single();
		private DirectoryInfo CorrectTestDataDir => TestDataDir.GetDirectories("Correct").Single();

		// ReSharper disable once MemberCanBePrivate.Global
		public static Tuple<string, Regex>[] errorsTestCases = {
			Tuple.Create(
				"ContentOfBraceTokensShouldKeepIndentationLength.cs", 
				new Regex("Ошибка отступов! Отступ на строке \\d+ должен быть не меньше отступа на строке \\d+")),

			Tuple.Create(
				"CompilationUnitChildrenShouldNeverBeIndented.cs", 
				new Regex("Ошибка отступов! На строке \\d+ не должно быть отступа")),

			Tuple.Create(
				"BlockChildrenShouldBeIndented.cs", 
				new Regex("Ошибка отступов! На строке \\d+ в позиции \\d+ должен быть отступ")),

			Tuple.Create(
				"BaseTypesChildrenShouldBeIndented.cs", 
				new Regex("Ошибка отступов! На строке \\d+ в позиции \\d+ должен быть отступ")),
		};

		[TestCaseSource(nameof(errorsTestCases))]
		public void FindErrors(Tuple<string ,Regex> filenameAndErrorRegex)
		{
			var code = IncorrectTestDataDir.GetFiles(filenameAndErrorRegex.Item1).Single().ContentAsUtf8();
			var validator = new IndentsValidator();

			var errors = validator.FindError(CSharpSyntaxTree.ParseText(code));

			errors.Should().MatchRegex(filenameAndErrorRegex.Item2.ToString());
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public static string[] noErrorsFilenames = {
				"ContentOfBraceTokensKeepsIndentationLength.cs",
				"CompilationUnitChildrenNeverIndented.cs",
				"BlockChildrenIndented.cs",
				"BlockChildrenMightBeNotIndented.cs",
				"BaseTypesChildrenAlwaysIndented.cs",
		};

		[TestCaseSource(nameof(noErrorsFilenames))]
		public void NotFindErrors(string filename)
		{
			var code = CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var validator = new IndentsValidator();

			var errors = validator.FindError(CSharpSyntaxTree.ParseText(code));

			errors.Should().BeNull();
		}
	}
}