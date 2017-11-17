using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CSharp
{
	public abstract class ValidatorTestBase
	{
		protected abstract BaseStyleValidator Validator { get; }

		protected static DirectoryInfo BasicProgrammingDirectory =>
			new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
				"..", "CSharp", "ExampleFiles", "BasicProgramming-master"));
		protected static IEnumerable<FileInfo> BasicProgrammingFiles =>
			BasicProgrammingDirectory
				.EnumerateFiles("*.cs", SearchOption.AllDirectories)
				.Where(f => !f.Name.Equals("Settings.Designer.cs") &&
							!f.Name.Equals("Resources.Designer.cs") &&
							!f.Name.Equals("AssemblyInfo.cs"));

		protected static DirectoryInfo ULearnSubmissionsDirectory =>
			new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
				"..", "CSharp", "ExampleFiles", "submissions"));
		protected static IEnumerable<FileInfo> SubmissionsFiles =>
			ULearnSubmissionsDirectory
				.GetFiles("*.cs", SearchOption.AllDirectories)
				.Where(f => f.Name.Contains("Accepted"));

		public void CheckThatErrorsAreFound(FileInfo file)
		{
			var code = file.ContentAsUtf8();
			var errors = Validator.FindError(code);

			errors.Should().NotBeNullOrEmpty();
		}

		public void CheckThatErrorsAreNotFound(FileInfo file)
		{
			var code = file.ContentAsUtf8();
			var errors = Validator.FindError(code);
			if (errors != null)
			{
				Console.WriteLine(errors);
			}

			errors.Should().BeNullOrEmpty();
		}

		public void CheckErrorsAreNotFound_InBasicProgramming(FileInfo file)
		{
			var fileContent = file.ContentAsUtf8();

			var errors = Validator.FindError(fileContent);

			if (errors != null)
			{
				File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
						"..", "CSharp", "ExampleFiles", "errors", $"{file.Name}_errors.txt"),
					$@"{fileContent}

{errors}");

				Assert.Fail();
			}
		}

		public void CheckThatErrorsAreNotFound_InAcceptedFiles(FileInfo file)
		{
			var fileContent = file.ContentAsUtf8();

			var errors = Validator.FindError(fileContent);
			if (errors != null)
			{
				File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
						"..", "CSharp", "ExampleFiles", "submissions_errors", $"{file.Name}_errors.txt"),
					$@"{fileContent}

{errors}");

				Assert.Fail();
			}
		}
	}
}
