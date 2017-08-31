using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
			if (errors != null)
			{
				Console.WriteLine(errors);
			}
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

		[Test]
		[Explicit]
		public void PrintParentsOfOpenBraces()
		{
			var folder = new DirectoryInfo("d:\\BP1_UserCodeFiles\\");
			var grandparents = new HashSet<string> { "parent,grandparent" };
			folder.GetFiles("*.cs", SearchOption.AllDirectories)
				.Where(f => f.Name.Contains("Accepted"))
				.Select(f => CSharpSyntaxTree.ParseText(f.ContentAsUtf8()))
				.SelectMany(tree => tree.GetRoot().DescendantTokens().Where(t => t.IsKind(SyntaxKind.OpenBraceToken)))
				.Select(t => string.Join(",", t.GetParents().Take(2).Select(p => "" + p.Kind().ToString())))
				.Distinct()
				.OrderBy(z => z)
				.ForEach(g => { grandparents.Add(g); });
			Console.WriteLine(string.Join("\r\n", grandparents));
		}

		[Test(Description =
			"Метод запускает валидатор отступов на прошедших ревью .cs файлах указанной папки, разбивает найденные ошибки по группам и записывает результаты в html-файлы")]
		[Explicit]
		public void BuildHtmlWithErrors()
		{
			var folder = new DirectoryInfo("d:\\BP1_UserCodeFiles\\");
			var filesCode = folder.GetFiles("*.json", SearchOption.AllDirectories)
				.Where(HasBeenReviewed)
				.Select(f => new FileInfo(NewFullNameFromOldFullNameAndExt(f.FullName, ".cs")).ContentAsUtf8());
			var badCodesWithErrors = new Dictionary<string, List<string>>();
			const string spanStart = "<span style=\"background-color: coral;\">";
			const string spanEnd = "</span>";
			foreach (var code in filesCode)
			{
				var errors = new IndentsValidator().FindError(code);
				if (errors == null)
					continue;
				var errorsLines = GroupBadLinesByErrors(errors);
				var codeLines = code.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
					.Select(l => HttpUtility.HtmlEncode(l.Replace(' ', '.').Replace("\t", "  → ")))
					.ToArray();
				foreach (var errorLines in errorsLines)
				{
					var highlightenedCode = new StringBuilder("<pre><code>");
					for (var line = 0; line < codeLines.Length; line++)
					{
						highlightenedCode.AppendLine(errorLines.Value.Contains(line)
							? $"{spanStart}{codeLines[line]}{spanEnd}"
							: $"{codeLines[line]}");
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

			Dictionary<string, HashSet<int>> GroupBadLinesByErrors(string errors)
			{
				return errors
					.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
					.Skip(1)
					.GroupBy(
						error => Regex.Replace(error, "\\d+", string.Empty),
						error =>
						{
							var result = new List<int> { int.Parse(Regex.Match(error, "\\d+").Value) - 1 };
							if (error.Contains("Парные фигурные скобки"))
								result.AddRange(new[]
								{
									int.Parse(Regex.Match(error, "строки \\d+").Value.Substring(7)) - 1,
									int.Parse(Regex.Match(error, ", \\d+").Value.Substring(2)) - 1
								});
							return result;
						},
						(error, lines) => new
						{
							Error = error,
							BadLines = lines.SelectMany(l => l)
						})
					.ToDictionary(g => g.Error, g => new HashSet<int>(g.BadLines));
			}

			bool HasBeenReviewed(FileInfo f)
			{
				return f.Name.Contains("Accepted") &&
						new FileInfo(NewFullNameFromOldFullNameAndExt(f.FullName, ".cs")).Exists &&
						bool.Parse(Regex.Matches(f.ContentAsUtf8(), ".*\"HasBeenReviewed\":.* (true|false),")[0].Groups[1].Value);
			}

			string NewFullNameFromOldFullNameAndExt(string oldFullName, string ext)
			{
				return Path.Combine($"{Path.GetDirectoryName(oldFullName)}",
					$"{Path.GetFileNameWithoutExtension(oldFullName)}{ext}");
			}
		}
	}
}