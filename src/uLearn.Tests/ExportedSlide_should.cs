using System.Collections.Generic;
using log4net;
using log4net.Config;
using NUnit.Framework;
using uLearn.Model.Blocks;
using uLearn.Quizes;
using Ulearn.Common.Extensions;

namespace uLearn
{
	[TestFixture]
	public class ExportedSlide_should
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ExportedSlide_should));

		[SetUp]
		public void SetUp()
		{
			BasicConfigurator.Configure();
		}

		[Test]
		public void Test()
		{
			var info = new ExportedSlide
			{
				Title = "Test title",
				MaxScore = 10,
				Blocks = new List<SlideBlock>
				{
					new CodeBlock("test-code", "cs"),
					new ChoiceBlock
					{
						Multiple = false,
						Items = new[]
						{
							new ChoiceItem
							{
								Description = "Test description",
								Explanation = "Test explanation",
								IsCorrect = ChoiceItemCorrectness.True
							},
						}
					},
					new SingleFileExerciseBlock
					{
						CodeFile = "",
						ExerciseInitialCode = "Test exercise initial code",
						CorrectnessScore = 5,
					}
				}
			};
			log.Info(info.XmlSerialize());
		}
	}

}
