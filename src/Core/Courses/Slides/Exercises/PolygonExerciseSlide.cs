using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;

namespace Ulearn.Core.Courses.Slides.Exercises
{
	[XmlRoot("slide.polygon", IsNullable = false, Namespace = "https://ulearn.me/schema/v2")]
	public class PolygonExerciseSlide : ExerciseSlide
	{
		[XmlElement("polygonPath")]
		public string PolygonPath { get; set; }
		
		public override void Validate(SlideLoadingContext context)
		{
			if (string.IsNullOrEmpty(PolygonPath))
				throw new CourseLoadingException("В slide.polygon должен находиться атрибут polygonPath");
			base.Validate(context);
		}

		public override void BuildUp(SlideLoadingContext context)
		{
			var statementsFilePath = Path.Combine(context.Unit.Directory.FullName, PolygonPath, "statements", "russian", "problem-properties.json");
			var json = File.ReadAllText(statementsFilePath);
			var statements = JsonConvert.DeserializeObject<Statements>(json);
			
			Title = statements.Name;
			
			var polygonExercise = Blocks.Single(block => block is PolygonExerciseBlock) as UniversalExerciseBlock;
			polygonExercise.ExerciseDirPath = Path.Combine(PolygonPath);
			(polygonExercise as PolygonExerciseBlock).MsPerTest = statements.TimeLimit;

			Blocks = new[] { GenerateDescription(statements) }
				.Concat(Blocks)
				.ToArray();
			base.BuildUp(context);
		}

		private MarkdownBlock GenerateDescription(Statements statements)
		{
			return new MarkdownBlock
			{
				InnerBlocks = new SlideBlock[]
				{
					GenerateRestrictions(statements),
					GenerateLegend(statements),
					GenerateTestDescription(statements),
					RenderInOutTable(statements.SampleTests),
					GenerateNotes(statements)
				}
			};
		}

		private MarkdownBlock GenerateNotes(Statements statements)
		{
			return !string.IsNullOrEmpty(statements.Notes)
				? new MarkdownBlock($"### Замечание\n{statements.Notes}") 
				: new MarkdownBlock("</br>");
		}

		private MarkdownBlock GenerateRestrictions(Statements statements)
		{
			return new MarkdownBlock($"Ограничение по времени: {statements.TimeLimit / 1000d} сек.");
		}

		private MarkdownBlock GenerateLegend(Statements statements)
		{
			return new MarkdownBlock(statements.Legend);
		}

		private MarkdownBlock GenerateTestDescription(Statements statements)
		{
			var sb = new StringBuilder();
			
			sb.AppendLine("#### Формат входных данных");
			sb.AppendLine(statements.Input);

			sb.AppendLine("#### Формат выходных данных");
			sb.AppendLine(statements.Output);

			return new MarkdownBlock(sb.ToString());
		}

		private MarkdownBlock RenderInOutTable(SampleTest[] sampleTests)
		{
			var sb = new StringBuilder();
			sb.AppendLine("#### Примеры");
			sb.Append("<table>");
			sb.Append("<thead>" +
						"<tr>" +
							"<th>Тест</th>" +
							"<th>Ответ</th>" +
						"</tr>" +
					"</thead>");
			foreach (var sampleTest in sampleTests)
			{
				var inputLines = sampleTest.Input.Split('\r', '\n').Where(s => !string.IsNullOrEmpty(s));
				var outputLines = sampleTest.Output.Split('\r', '\n').Where(s => !string.IsNullOrEmpty(s));
				sb.Append("<tr>");
				sb.Append($"<td>{string.Join("</br>", inputLines)}</td>");
				sb.Append($"<td>{string.Join("</br>", outputLines)}</td>");
				sb.Append("</tr>");
			}

			sb.Append("</table>");

			return new MarkdownBlock(sb.ToString());
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