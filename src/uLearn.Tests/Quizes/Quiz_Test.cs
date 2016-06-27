using System;
using System.IO;
using System.Xml.Serialization;
using ApprovalTests.Core;
using ApprovalTests.Reporters;
using NUnit.Framework;
using uLearn.Model.Blocks;

namespace uLearn.Quizes
{
    public class TeamCityFriendlyReporter : IEnvironmentAwareReporter
    {
        private readonly FrameworkAssertReporter ciReporter = new FrameworkAssertReporter();
        private readonly DiffReporter uiReporter = new DiffReporter();
        public void Report(string approved, string received)
        {
            if (Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME") != null)
                ciReporter.Report(approved, received);
            else
            {
                uiReporter.Report(approved, received);
                //ciReporter.Report(approved, received);
            }
        }

        public bool IsWorkingInThisEnvironment(string forFile)
        {
            return true;
        }
    }

    [TestFixture]
	public class Quiz_Test
	{
		[Test, UseReporter(typeof(TeamCityFriendlyReporter))]
		public void Test()
		{
			var serializer = new XmlSerializer(typeof(Quiz));
			var quiz = new Quiz
			{
				Title = "Title",
				Id = "Id",
				Blocks = new SlideBlock[]
				{
					new MdBlock {Markdown = "This is quiz!"},
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
							new ChoiceItem {Id="1", Description = "black", IsCorrect = true},
							new ChoiceItem {Id="2", Description = "green"},
							new ChoiceItem {Id="3", Description = "red"},
						}
					},
					new ChoiceBlock
					{
						Multiple = true,
						Text = "What does the fox say?",
						Items = new[]
						{
							new ChoiceItem {Description = "Apapapapa", IsCorrect = true},
							new ChoiceItem {Description = "Ding ding ding", IsCorrect = true},
							new ChoiceItem {Description = "Mew!"},
						}
					},
					new FillInBlock
					{
						Text = "What does the fox say?",
						Regexes = new[] {new RegexInfo {Pattern = "([Dd]ing )+"}, new RegexInfo {Pattern = "Ap(ap)+"}},
						Sample = "Apapap"
					},
				}
			};
			var w1 = new StringWriter();
			var ns = new XmlSerializerNamespaces();
			ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");
			serializer.Serialize(w1, quiz, ns);
			ApprovalTests.Approvals.Verify(w1.ToString());
			Console.WriteLine(w1.ToString());
			Console.WriteLine();
			var quiz2 = serializer.Deserialize(new StringReader(w1.ToString()));
			var w2 = new StringWriter();
			serializer.Serialize(w2, quiz2, ns);
			Console.WriteLine(w2.ToString());
			Assert.AreEqual(w1.ToString(), w2.ToString());
		}
	}
}