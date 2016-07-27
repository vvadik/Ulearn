using System.Collections.Immutable;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Model;
using uLearn.Model.Blocks;

namespace uLearn
{
    [TestFixture]
    public class Lesson_ShouldDeserialize
    {
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

        private static SlideBlock[] DeserializeBlocks(string blocksXml)
        {
            var input = $@"
<Lesson xmlns='https://ulearn.azurewebsites.net/lesson'>
{blocksXml}
</Lesson>";
            File.WriteAllText("temp.xml", input);
            var fileInfo = new FileInfo("temp.xml");
            var buildUpContext = new BuildUpContext(new DirectoryInfo("."), CourseSettings.DefaultSettings, null);
            return fileInfo.DeserializeXml<Lesson>().Blocks
                .SelectMany(b => b.BuildUp(buildUpContext, ImmutableHashSet<string>.Empty))
                .ToArray();
        }
    }


}