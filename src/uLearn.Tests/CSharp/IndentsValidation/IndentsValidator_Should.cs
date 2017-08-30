using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;


namespace uLearn.CSharp.IndentsValidation
{
	[TestFixture]
	public class IndentsValidator_Should
	{
		private static DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "IndentsValidation", "TestData"));

		private static DirectoryInfo IncorrectTestDataDir => TestDataDir.GetDirectories("Incorrect").Single();
		private static DirectoryInfo CorrectTestDataDir => TestDataDir.GetDirectories("Correct").Single();

		private static string[] correctFilenames => CorrectTestDataDir.GetFiles().Select(f => f.Name).ToArray();
		private static string[] incorrectFilenames => IncorrectTestDataDir.GetFiles().Select(f => f.Name).ToArray();

		[Test]
		[TestCaseSource(nameof(incorrectFilenames))]
		[UseReporter(typeof(DiffReporter))]
		public void FindErrors(string filename)
		{
			var code = IncorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var errors = new IndentsValidator().FindError(code);
			using (ApprovalResults.ForScenario(filename))
			{
				Approvals.Verify(errors);
			}
		}

		[TestCaseSource(nameof(correctFilenames))]
		public void NotFindErrors(string filename)
		{
			var code = CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var errors = new IndentsValidator().FindError(code);
			errors.Should().BeNullOrEmpty();
		}

		[Test]
		[Explicit]
		public void NotFindErrors_On_Cs_Files_Of_Folder()
		{
			var folder = new DirectoryInfo("d:\\_work\\BasicProgramming\\");
			var filesCode = folder.GetFiles("*.cs", SearchOption.AllDirectories)
				.Where(f => !f.Name.Equals("Settings.Designer.cs") && !f.Name.Equals("Resources.Designer.cs"))
				.Select(f => Tuple.Create(f.FullName, f.ContentAsUtf8()));
			var failed = false;
			foreach (var tuple in filesCode)
			{
				var errors = new IndentsValidator().FindError(tuple.Item2);

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

		[Test(Description =
			"Метод запускает валидатор отступов на .cs файлах указанной папки, разбивает найденные ошибки по группам и записывает результаты в html-файлы")]
		[Explicit]
		public void BuildHtmlWithErrors()
		{
			var folder = new DirectoryInfo("d:\\BP1_UserCodeFiles\\");
			var filesCode = folder.GetFiles("*.cs", SearchOption.AllDirectories)
				.Where(f => !f.Name.Equals("Settings.Designer.cs") && !f.Name.Equals("Resources.Designer.cs"))
				.Select(f => f.ContentAsUtf8());
			var badCodesWithErrors = new Dictionary<string, List<string>>();
			const string spanStart = "<span style=\"background-color: coral;\">";
			const string spanEnd = "</span>";
			foreach (var code in filesCode)
			{
				var errors = new IndentsValidator().FindError(code);
				if (errors == null)
					continue;
				var errorsLines = errors
					.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
					.Skip(1)
					.GroupBy(
						error => Regex.Replace(error, "\\d+", string.Empty),
						error => int.Parse(Regex.Match(error, "\\d+").Value) - 1)
					.ToDictionary(group => group.Key, group => new HashSet<int>(group));
				var codeLines = code.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
				foreach (var errorLines in errorsLines)
				{
					var highlightenedCode = new StringBuilder("<pre><code>");
					for (var line = 0; line < codeLines.Length; line++)
					{
						highlightenedCode.AppendLine(errorLines.Value.Contains(line)
							? $"{spanStart}{codeLines[line].Replace(' ', '.').Replace("\t", "  → ")}{spanEnd}"
							: $"{codeLines[line].Replace(' ', '.').Replace("\t", "  → ")}");
					}
					if (!badCodesWithErrors.ContainsKey(errorLines.Key))
					{
						badCodesWithErrors[errorLines.Key] = new List<string>();
					}
					highlightenedCode.AppendLine("</pre></code>");
					badCodesWithErrors[errorLines.Key].Add(highlightenedCode.ToString());
				}
			}
			var i = 0;
			foreach (var badCodesByError in badCodesWithErrors)
			{
				File.WriteAllText(
					Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Errors{i++}.html"),
					$@"<div style=""white-space: pre;"">

{badCodesByError.Key}

{string.Join("\r\n", badCodesByError.Value.Shuffle())}
</div>",
					Encoding.UTF8);
			}
		}
	}
}