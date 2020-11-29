using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Core.Courses.Slides.Exercises
{
	[XmlRoot("slide.polygon", IsNullable = false, Namespace = "https://ulearn.me/schema/v2")]
	public class PolygonExerciseSlide : ExerciseSlide
	{
		public override void Validate(SlideLoadingContext context)
		{
			if (string.IsNullOrEmpty(StatementsPath))
				throw new CourseLoadingException("В slide.polygon должен находиться атрибут Statements");
			base.Validate(context);
		}

		public override void BuildUp(SlideLoadingContext context)
		{
			var statementsFilePath = Path.Combine(context.Unit.Directory.FullName, StatementsPath, "russian", "problem-properties.json");
			var json = File.ReadAllText(statementsFilePath);
			var statements = JsonConvert.DeserializeObject<Statements>(json);
			var markdown = $"{statements.Legend}"; // Потом тут сделать какой-то формат
			Blocks = Blocks.Append(new MarkdownBlock(markdown))
				.ToArray();
			base.BuildUp(context);
		}
	}

	public class Statements
	{
		public string Name { get; set; }
		public string Scoring { get; set; }
		public string Notes { get; set; }
		public string Legend { get; set; }
		public string AuthorName { get; set; }
		public string AuthorLogin { get; set; }
		public string Language { get; set; }
		public int TimeLimit { get; set; }
		public int MemoryLimit { get; set; }
		public string Input { get; set; }
		public string Output { get; set; }
		public string InputFile { get; set; }
		public string OutputFile { get; set; }
		public SampleTest[] SampleTests { get; set; }
	}

	public class SampleTest
	{
		public string Input { get; set; }
		public string Output { get; set; }
	}
}