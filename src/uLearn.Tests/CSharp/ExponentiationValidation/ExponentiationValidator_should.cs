using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CSharp.ExponentiationValidation
{
	[TestFixture]
	public class ExponentiationValidator_should
	{
		private static DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "ExponentiationValidation", "TestData"));
		private static DirectoryInfo IncorrectTestDataDir => TestDataDir.GetDirectories("Incorrect").Single();
		private static DirectoryInfo CorrectTestDataDir => TestDataDir.GetDirectories("Correct").Single();

		private static IEnumerable<FileInfo> CorrectFiles => CorrectTestDataDir.EnumerateFiles();
		private static IEnumerable<FileInfo> IncorrectFiles => IncorrectTestDataDir.EnumerateFiles();
		
		private static readonly DirectoryInfo basicProgrammingDirectory = new DirectoryInfo(@"C:\work\uLearn\BasicProgramming-master");
		private static IEnumerable<FileInfo> basicProgrammingFiles = basicProgrammingDirectory
			.EnumerateFiles("*.cs", SearchOption.AllDirectories)
			.Where(f => !f.Name.Equals("Settings.Designer.cs") &&
						!f.Name.Equals("Resources.Designer.cs") &&
						!f.Name.Equals("AssemblyInfo.cs"));
		
		private static readonly ExponentiationValidator validator = new ExponentiationValidator();

		[TestCaseSource(nameof(IncorrectFiles))]
		public void FindErrors(FileInfo file)
		{
			var code = file.ContentAsUtf8();
			var errors = validator.FindError(code);
			
			errors.Should().NotBeNullOrEmpty();
		}

		[TestCaseSource(nameof(CorrectFiles))]
		public void NotFindErrors(FileInfo file)
		{
			var code = file.ContentAsUtf8();
			var errors = validator.FindError(code);
			if (errors != null)
			{
				Console.WriteLine(errors);
			}

			errors.Should().BeNullOrEmpty();
		}

		[Explicit]
		[TestCaseSource(nameof(basicProgrammingFiles))]
		public void NotFindErrors_InBasicProgramming(FileInfo file)
		{
			var fileContent = file.ContentAsUtf8();

			var errors = validator.FindError(fileContent);

			if (errors != null)
			{
				File.WriteAllText($@"C:\work\uLearn\errors\{file.Name}_errors.txt", errors);
				Assert.Fail();
			}
		}
	}
}