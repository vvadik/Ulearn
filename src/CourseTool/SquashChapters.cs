using System;
using System.IO;
using CommandLine;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	[Verb("clear-olx", HelpText = "Remove not existent items from course")]
	public class ClearOlx : AbstractOptions
	{
		public override void DoExecute()
		{
			var loadOptions = new EdxLoadOptions();
			loadOptions.FailOnNonExistingItem = false;
			loadOptions.HandleNonExistentItemTypeName = (type, url) => Console.WriteLine($"Skipped non existent item  type:{type} urlName:{url}");
			var folderName = Path.Combine(Dir, "olx");
			Console.WriteLine("Loading");
			var course = EdxCourse.Load(folderName, loadOptions);
			Console.WriteLine("Saving");
			course.Save(folderName);
		}
	}

	[Verb("squash-olx-chapters", HelpText = "Move sequentials to chapters")]
	public class SquashOlxChapters : AbstractOptions
	{
		public override void DoExecute()
		{
			Console.WriteLine("squashing");
			new OlxSquasher().SquashCourse(Path.Combine(Dir, "olx"));
		}
	}
	[Verb("desquash-olx-chapters", HelpText = "Move sequentials from chapter")]
	public class DesquashOlxChapters : AbstractOptions
	{
		public override void DoExecute()
		{
			Console.WriteLine("desquashing");
			new OlxSquasher().DesquashCourse(Path.Combine(Dir, "olx"));
		}
	}

	[Verb("create-targz", HelpText = "Create course.tar.gz from olx directory")]
	public class CreateTarGz : AbstractOptions
	{
		[Option('t', "tar-gz", HelpText = "Filepath of course tar.gz file")]
		public string CourseTarGz { get; set; }

		public override void DoExecute()
		{
			EdxInteraction.CreateEdxCourseArchive(Dir, Config.ULearnCourseId);
		}
	}
	[Verb("create-olx", HelpText = "Unpack course.tar.gz to olx directory")]
	public class CreateOlx : AbstractOptions
	{
		[Option('t', "tar-gz", HelpText = "Filepath of course tar.gz file")]
		public string CourseTarGz { get; set; }

		public override void DoExecute()
		{
			EdxInteraction.ExtractEdxCourseArchive(Dir, Dir.GetSingleFile(CourseTarGz ?? "*.tar.gz"));
		}
	}
}