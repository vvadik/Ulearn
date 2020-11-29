using System.Xml.Serialization;

namespace Ulearn.Core.Courses.Slides.Exercises.Blocks
{
	[XmlType("exercise.polygon")]
	public class PolygonExerciseBlock : UniversalExerciseBlock
	{
		public override string DockerImageName => "algorithms-sandbox";
		public override bool NoStudentZip => true;
		public override string UserCodeFilePath => "solution.txt";
		public override string RunCommand => "python3 main.py";
		public override string Region => "Task";
		public override string[] PathsToExcludeForChecker => new[]
		{
			"files",
			"scripts",
			"solutions",
			"statements",
			"statements-sections",
			"check.exe",
			"doall.bat",
			"problem.xml",
			"tags",
			"wipe.bat"
		};

	}
}