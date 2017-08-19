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
		private static DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "CSharp", "IndentsValidation", "TestData"));
		private static DirectoryInfo IncorrectTestDataDir => TestDataDir.GetDirectories("Incorrect").Single();
		private static DirectoryInfo CorrectTestDataDir => TestDataDir.GetDirectories("Correct").Single();

		// ReSharper disable once MemberCanBePrivate.Global
		public static Tuple<string, Regex>[] errorsTestCases =
		{
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

			Tuple.Create(
				"BraceTokensShouldBeAligned.cs",
				new Regex("Ошибка отступов! Размер отступа на строке \\d+ должен совпадать с размером отступа на строке \\d+")),

			Tuple.Create(
				"TabsMixedWithSpaces.cs",
				new Regex("Ошибка отступов! Не стоит мешать табы с пробелами в отступах строк: +"))
		};

		[TestCaseSource(nameof(errorsTestCases))]
		public void FindErrors(Tuple<string, Regex> filenameAndErrorRegex)
		{
			var code = IncorrectTestDataDir.GetFiles(filenameAndErrorRegex.Item1).Single().ContentAsUtf8();
			var validator = new IndentsValidator();

			var errors = validator.FindError(CSharpSyntaxTree.ParseText(code));

			errors.Should().MatchRegex(filenameAndErrorRegex.Item2.ToString());
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public static string[] noErrorsFilenames =
		{
			"ContentOfBraceTokensKeepsIndentationLength.cs",
			"CompilationUnitChildrenNeverIndented.cs",
			"BlockChildrenIndented.cs",
			"BlockChildrenNotIndentedInAccessors.cs",
			"BaseTypesChildrenAlwaysIndented.cs",
		};

		public static string[] noErrorsFilenamesWithSpaces => noErrorsFilenames
			.Select(filename => CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8().Replace("\t", "    "))
			.ToArray();

		[TestCaseSource(nameof(noErrorsFilenames))]
		[TestCaseSource(nameof(noErrorsFilenamesWithSpaces))]
		public void NotFindErrors(string filename)
		{
			var code = CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var validator = new IndentsValidator();

			var errors = validator.FindError(CSharpSyntaxTree.ParseText(code));

			errors.Should().BeNull();
		}
	}
}