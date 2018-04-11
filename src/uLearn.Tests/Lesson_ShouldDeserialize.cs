using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Model;
using uLearn.Model.Blocks;
using Ulearn.Common.Extensions;

namespace uLearn
{
	[TestFixture]
	public class Lesson_ShouldDeserialize
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}
		
		[Test]
		public void SimpleMdBlock()
		{
			var blocks = DeserializeBlocks("<md>asd</md>");
			blocks.ShouldBeEquivalentTo(new[] { new MdBlock("asd") });
		}

		[Test]
		public void SeveralMdBlocks()
		{
			var blocks = DeserializeBlocks("<md>1</md><md>2</md>");
			blocks.ShouldBeEquivalentTo(new[] { new MdBlock("1"), new MdBlock("2") });
		}

		[Test]
		public void MixedContentInsideMd()
		{
			var blocks = DeserializeBlocks("<md>1<code>2</code>3</md>");
			blocks.ShouldBeEquivalentTo(new SlideBlock[] { new MdBlock("1"), new CodeBlock("2", null), new MdBlock("3") });
		}

		[Test]
		public void OtherTypesOfBlocksAreOK()
		{
			var blocks = DeserializeBlocks("<md>1</md><code>2</code><youtube>sdfsdfsdf</youtube>");
			blocks.ShouldBeEquivalentTo(new SlideBlock[] { new MdBlock("1"), new CodeBlock("2", null), new MdBlock("3") });
		}

		[Test]
		public void InstructorNotesInsideMd()
		{
			var blocks = DeserializeBlocks("<md>1<note>secret</note>2<code>3</code>4</md>");
			blocks.ShouldBeEquivalentTo(new SlideBlock[]
			{
				new MdBlock("1"),
				new MdBlock("secret") { Hide = true },
				new MdBlock("2"),
				new CodeBlock("3", null),
				new MdBlock("4")
			});
		}

		[Test]
		public void UserCodeFileName()
		{
			var ex = DeserializeLesson("<proj-exercise>" +
										"<user-code-file-name>user-code-name-111</user-code-file-name>" +
										"</proj-exercise>").Blocks.Single();
			((ProjectExerciseBlock)ex).UserCodeFilePath.Should().Be("user-code-name-111");
		}

		[Test]
		public void UserCodeFilePath()
		{
			var ex = DeserializeLesson("<proj-exercise>" +
										"<user-code-file-path>user-code-name-111</user-code-file-path>" +
										"</proj-exercise>").Blocks.Single();
			((ProjectExerciseBlock)ex).UserCodeFilePath.Should().Be("user-code-name-111");
		}

		[Test]
		public void ExerciseHints()
		{
			var ex = DeserializeLesson("<proj-exercise>" +
										"<hint>Hint 1</hint>" +
										"<hint>Hint 2</hint>" +
										"</proj-exercise>").Blocks.Single() as ProjectExerciseBlock;
			if (ex == null)
				Assert.Fail("Can't parse exercise with hints");
			ex.Hints.Should().HaveCount(2);
			ex.Hints.Should().Contain(new List<string> { "Hint 1", "Hint 2" });
			ex.HintsMd.Should().HaveSameCount(ex.Hints);
		}

		private static Lesson DeserializeLesson(string blocksXml)
		{
			var input = $@"
<Lesson xmlns='https://ulearn.azurewebsites.net/lesson'>
{blocksXml}
</Lesson>";
			File.WriteAllText("temp.xml", input);
			var fileInfo = new FileInfo("temp.xml");
			return fileInfo.DeserializeXml<Lesson>();
		}

		private static SlideBlock[] DeserializeBlocks(string blocksXml)
		{
			var buildUpContext = new BuildUpContext(new Unit(null, new DirectoryInfo(".")), CourseSettings.DefaultSettings, null, "Test", "Заголовок слайда");
			var blocks = DeserializeLesson(blocksXml).Blocks;
			return blocks.SelectMany(b => b.BuildUp(buildUpContext, ImmutableHashSet<string>.Empty)).ToArray();
		}
	}
	
}