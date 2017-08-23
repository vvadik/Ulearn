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
						"^Ошибка отступов! Отступ перед фигурной скобкой на строке \\d+ должен быть таким же, как отступ перед фигурной скобкой на строке \\d+\\.$"),
				// Префикс: Код плохо отформатирован. Автоматически отформатировать код можно ...
				// Парные фигурные скобки (//d+://d+) должны иметь одинаковый отступ.
				// 
				ErrorsLines = new[] { 10, 14, 18, 23, 28, 34, 39, 44, 51, 56, 61, 68, 72, 73 } // todo approvalstest
			},
			new MyTestCase
			{
				Filename = "CompilationUnitChildrenShouldNeverBeIndented.cs",
				ErrorRegex =
					new Regex("^Ошибка отступов! На верхнем уровне вложенности на строке \\d+ не должно быть отступов\\.$"),
				// Лишний отступ
				ErrorsLines = new[] { 1, 3, 7 }
			},
			new MyTestCase
			{
				Filename = "IfBracesNotOnSameLineContentOfBracesShouldBeIndented.cs",
				ErrorRegex =
					new Regex(
						"^Ошибка отступов! На строке \\d+ должен быть отступ\\. Если открывающаяся фигурная скобка закрывается на другой строке, то внутри фигурных скобок должен быть отступ\\.$"),
				// Содержимое парных фигурных скобок должно иметь дополнительный отступ
				ErrorsLines = new[]
					{ 8, 16, 17, 22, 27, 37, 41, 46, 47, 52, 53, 54, 56, 63, 70, 71, 77, 82, 87, 92, 97, 101, 106, 108, 119, 125, 135 }
			},
			new MyTestCase
			{
				Filename = "IfBracesNotOnSameLineContentOfBracesShouldBeConsistent.cs",
				ErrorRegex =
					new Regex(
						"^Ошибка отступов! Отступ на строке \\d+ не консистентен\\. Внутри одних фигурных скобок отступ не должен изменять свой размер\\.$"),
				// Содержимое парных фигурных скобок должно иметь одинаковый отступ
				ErrorsLines = new[] { 16, 26, 33, 41, 48, 58, 59, 67, 74 }
			},
			new MyTestCase
			{
				Filename = "IfBracesNotOnSameLineAndOpenbraceHasContentOnSameLineItShouldBeIndented.cs",
				ErrorRegex =
					new Regex(
						"^Ошибка отступов! На строке \\d+ должен быть отступ\\. Если открывающаяся фигурная скобка закрывается на другой строке, то внутри фигурных скобок должен быть отступ\\.$"),
				ErrorsLines = new[] { 7, 12, 16, 20, 27, 33 }
			}
		};

		[TestCaseSource(nameof(errorsTestCases))]
		public void FindErrors(MyTestCase testCase)
		{
			var code = IncorrectTestDataDir.GetFiles(testCase.Filename).Single().ContentAsUtf8();
			var validator = new IndentsValidator();

			var errors = validator.FindError(CSharpSyntaxTree.ParseText(code));

			errors.Should().NotBeNullOrEmpty();
			var splittedErrors = errors.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			if (splittedErrors.Length != testCase.ErrorsLines.Length)
			{
				Console.WriteLine(errors);
			}
			splittedErrors.ForEach(error => error.Should().MatchRegex(testCase.ErrorRegex.ToString()));
			splittedErrors.Select(e => int.Parse(Regex.Matches(e, "\\d+")[0].Value))
				.ShouldBeEquivalentTo(testCase.ErrorsLines);
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
		public int[] ErrorsLines { get; set; }

		public override string ToString()
		{
			return Filename;
		}
	}
}