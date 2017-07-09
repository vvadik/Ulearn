using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using test;
using uLearn.Extensions;
using uLearn.Model;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[TestFixture]
	public class ProjectExerciseBlock_Should
	{
		private string slideFolderPath => Path.Combine(TestContext.CurrentContext.TestDirectory, "CSharp", "TestProject");
		private string csProjFilePath => Path.Combine("ProjDir", "test.csproj");

		[Test]
		public void FindSolutionFile_OnBuildUp()
		{
			var ex = new ProjectExerciseBlock
			{
				SlideFolderPath = new DirectoryInfo(slideFolderPath),
				CsProjFilePath = csProjFilePath,
				UserCodeFileName = $"{nameof(MeaningOfLifeTask)}.cs"
			};
			var correctSolutionCode = ex.SolutionFile.ContentAsUtf8();
			var ctx = new BuildUpContext(ex.SlideFolderPath, CourseSettings.DefaultSettings, null, String.Empty);

			ex.BuildUp(ctx, ImmutableHashSet<string>.Empty)
				.Any(block => block is CodeBlock cb && cb.Code.Equals(correctSolutionCode))
				.Should().BeTrue();
		}
	}
}