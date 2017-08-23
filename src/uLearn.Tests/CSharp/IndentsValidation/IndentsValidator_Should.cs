using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ApprovalUtilities.Utilities;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CSharp
{
	[TestFixture]
	public class IndentsValidator_Should
	{
		public static DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "IndentsValidation", "TestData"));

		private static DirectoryInfo IncorrectTestDataDir => TestDataDir.GetDirectories("Incorrect").Single();
		private static DirectoryInfo CorrectTestDataDir => TestDataDir.GetDirectories("Correct").Single();

		// ReSharper disable once MemberCanBePrivate.Global
		public static readonly MyTestCase[] errorsTestCases =
		{
			new MyTestCase
			{
				Filename = "BracesShouldBeAlignedIfOpenBraceHasLeadingTriviaAndBracesNotOnSameLine.cs",
				ErrorRegex =
					new Regex(
						"Ошибка отступов! Размер отступа \\d+ \\(в (табах|пробелах){1}\\) на строке \\d+ должен совпадать с размером отступа \\d+ на строке \\d+$"),
				ErrorsCount = 14
			},
			new MyTestCase
			{
				Filename = "CompilationUnitChildrenShouldNeverBeIndented.cs",
				ErrorRegex = new Regex("Ошибка отступов! На строке \\d+ не должно быть отступа$"),
				ErrorsCount = 3
			},
			new MyTestCase
			{
				Filename = "IfBracesNotOnSameLineContentOfBracesShouldBeIndented.cs",
				ErrorRegex =
					new Regex("Ошибка отступов! На строке \\d+ в позиции \\d+ должен быть отступ размером в \\d+ (табов|пробелов)$"),
				ErrorsCount = 41
			},
			new MyTestCase
			{
				Filename = "IfOpenBraceHasContentOnSameLineItShouldBeIndented.cs",
				ErrorRegex = new Regex("Ошибка отступов! На строке \\d+ после фигурной скобки должен быть отступ$"),
				ErrorsCount = 6
			},
			new MyTestCase
			{
				Filename = "MultilineArgumentListShouldBeIndented.cs",
				ErrorRegex = new Regex("Ошибка отступов! На строке \\d+ в позиции \\d+ должен быть отступ$")
			}
		};

		[TestCaseSource(nameof(errorsTestCases))]
		public void FindErrors(MyTestCase testCase)
		{
			var code = IncorrectTestDataDir.GetFiles(testCase.Filename).Single().ContentAsUtf8();
			var validator = new IndentsValidator();

			var errors = validator.FindError(CSharpSyntaxTree.ParseText(code));

			var splittedErrors = errors.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			if (splittedErrors.Length != testCase.ErrorsCount)
			{
				Console.WriteLine(errors);
			}
			splittedErrors.ForEach(error => error.Should().MatchRegex(testCase.ErrorRegex.ToString()));
			splittedErrors.Length.Should().Be(testCase.ErrorsCount);
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public static MyTestCase[] noErrorsTestCases => CorrectTestDataDir.GetFiles()
			.Select(f => new MyTestCase { Filename = f.Name })
			.ToArray();

		[TestCaseSource(nameof(noErrorsTestCases))]
		public void NotFindErrors(MyTestCase testCase)
		{
			var code = CorrectTestDataDir.GetFiles(testCase.Filename).Single().ContentAsUtf8();
			var validator = new IndentsValidator();

			var errors = validator.FindError(CSharpSyntaxTree.ParseText(code));

			if (errors != null)
			{
				Console.WriteLine(errors);
			}
			errors.Should().BeNull();
		}

		[Test]
		public void NotFindErrors_On_Course_Cs_Files()
		{
			var filepaths = CorrectTestDataDir.GetFiles("TestFilepaths.txt").Single().ContentAsUtf8()
				.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			var pathAndCode = filepaths.Select(filepath => Tuple.Create(filepath, File.ReadAllText(filepath)));
			var failed = false;
			foreach (var tuple in pathAndCode.Where(tuple => !tuple.Item1.EndsWith("Settings.Designer.cs") &&
															!tuple.Item1.EndsWith("Resources.Designer.cs")))
			{
				var validator = new IndentsValidator();

				var errors = validator.FindError(CSharpSyntaxTree.ParseText(tuple.Item2));

				if (errors != null)
				{
					failed = true;
					Console.WriteLine(tuple.Item1);
					Console.WriteLine(errors);
					Console.WriteLine();
				}
			}
			if (failed)
				Assert.Fail();
		}
	}

	public class MyTestCase
	{
		public string Filename { get; set; }
		public Regex ErrorRegex { get; set; }
		public int ErrorsCount { get; set; }

		public override string ToString()
		{
			return Filename;
		}
	}
}