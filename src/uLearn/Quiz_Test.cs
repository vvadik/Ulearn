using System;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;

namespace uLearn.Quizes
{
	[TestFixture]
	public class Quiz_Test
	{
		[Test]
		public void Test()
		{
			var serializer = new XmlSerializer(typeof(Quiz));
			var quiz = new Quiz
			{
				Title = "Title",
				Id = "Id",
				Blocks = new QuizBlock[]
				{
					new QuizBlock {Text = "This is quiz!"},
					new IsTrueBlock
					{
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
							new ChoiceItem {Description = "Mew"},
						}
					},
					new FillInBlock
					{
						Text = "What does the fox say?",
						Regexes = new[] {"([Dd]ing )+", "Ap(ap)+"}
					},
				}
			};
			var w1 = new StringWriter();
			var ns = new XmlSerializerNamespaces();
			ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");
			serializer.Serialize(w1, quiz, ns);
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