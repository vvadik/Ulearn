using System.IO;
using System.Xml.Serialization;
using ApprovalTests.Reporters;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Model.Blocks;
using Ulearn.Common.Extensions;

namespace uLearn.Quizes
{
	[TestFixture]
	public class Quiz_should
	{
		[Test, UseReporter(typeof(DiffReporter))]
		public void BeSerializable()
		{
			var serializer = new XmlSerializer(typeof(Quiz));
			var quiz = new Quiz
			{
				Title = "Title",
				Id = "Id",
				Blocks = new SlideBlock[]
				{
					new MdBlock { Markdown = "This is quiz!" },
					new IsTrueBlock
					{
						Id = "1",
						Text = "Это утверждение ложно",
					},
					new ChoiceBlock
					{
						Text = "What is the \nbest color?",
						Items = new[]
						{
							new ChoiceItem { Id = "1", Description = "black", IsCorrect = true },
							new ChoiceItem { Id = "2", Description = "green" },
							new ChoiceItem { Id = "3", Description = "red" },
						}
					},
					new ChoiceBlock
					{
						Multiple = true,
						Text = "What does the fox say?",
						Items = new[]
						{
							new ChoiceItem { Description = "Apapapapa", IsCorrect = true },
							new ChoiceItem { Description = "Ding ding ding", IsCorrect = true },
							new ChoiceItem { Description = "Mew" },
						}
					},
					new FillInBlock
					{
						Text = "What does the fox say?",
						Regexes = new[] { new RegexInfo { Pattern = "([Dd]ing )+" }, new RegexInfo { Pattern = "Ap(ap)+" } },
						Sample = "Apapap"
					},
				}
			};
			var w1 = new StringWriter();
			var ns = new XmlSerializerNamespaces();
			ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");
			serializer.Serialize(w1, quiz, ns);
			ApprovalTests.Approvals.Verify(w1.ToString());
			w1.ToString().DeserializeXml<Quiz>();
		}

		[Test]
		public void BeDeserializable()
		{
			var q = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Quizes", "test.quiz.xml")).DeserializeXml<Quiz>();
			QuizSlideLoader.BuildUp(q, new Unit(null, new DirectoryInfo(TestContext.CurrentContext.TestDirectory)), "QuizzesTest", CourseSettings.DefaultSettings);
			q.ShouldBeEquivalentTo(new Quiz
			{
				Id = "{DB95DA10-7DD2-46CA-BFB2-6D0D7554B83F}",
				Title = "title",
				Blocks = new SlideBlock[]
				{
					new MdBlock("para"),
					new CodeBlock("code", null),
					new MdBlock("para2"),
					new CodeBlock("code2", null),
					new MdBlock("para3"),
					new MdBlock("note") { Hide = true },
					new MdBlock("hidden") { Hide = true },
				}
			});
		}

		[Test]
		public void DeserializeNormalizedXml()
		{
			var q = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Quizes", "normalizedQuiz.xml")).DeserializeXml<Quiz>();
			q.Blocks.Length.Should().Be(8);
		}
	}
}