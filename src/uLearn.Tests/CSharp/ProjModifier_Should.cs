using System;
using System.IO;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using NUnit.Framework;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[NUnit.Framework.TestFixture]
	public class ProjModifier_Should
	{
		[NUnit.Framework.Test]
		public void ReplaceLinksWithItems()
		{
			var project = new Project("CSharp/TestProject/ProjDir/test.csproj");
			var copies = ProjModifier.ReplaceLinksWithItemsCopiedToProjectDir(project);
			copies.Should().HaveCount(1);
			var writer = new StringWriter();
			foreach (var fileToCopy in copies)
			{
				writer.WriteLine("Copy " + fileToCopy.SourceFile + " to " + fileToCopy.DestinationFile);
			}
			project.Save(writer);
			Approvals.Verify(writer.ToString());
		}
	}
}